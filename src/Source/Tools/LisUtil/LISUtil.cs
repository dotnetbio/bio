using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Bio;
using Bio.Extensions;
using Bio.IO.FastA;
using Bio.Algorithms.MUMmer;
using Bio.Algorithms.MUMmer.LIS;
using Bio.Algorithms.SuffixTree;
using Bio.Util.ArgumentParser;
using LISUtil.Properties;

namespace LisUtil
{
    class LISUtil
    {
        private static FileStream fileStreamConsoleOut;
        private static StreamWriter streamWriterConsoleOut;
        private static TextWriter textWriterConsoleOutSave;

        private static CommandLineOptions ProcessCommandLine(string[] args)
        {
            CommandLineOptions myArgs = new CommandLineOptions();
            CommandLineArguments parser = new CommandLineArguments();
            AddParameters(parser);
            try
            {
                parser.Parse(args, myArgs);
            }

            catch (ArgumentParserException e)
            {
                Console.Error.WriteLine("\nException while processing Command Line arguments [{0}]", e.Message);
                Console.Error.WriteLine(Resources.LisUtilHelp);
                Environment.Exit(-1);
            }

            if (myArgs.Help)
            {
                Console.WriteLine(Resources.LisUtilHelp);
                Environment.Exit(-1);
            }

            /*
             * Process all the arguments for 'semantic' correctness
             */
            if (!myArgs.PerformLISOnly)
            {
                if ((myArgs.MaxMatch && myArgs.Mum)
                    || (myArgs.MaxMatch && myArgs.Mumreference)
                    || (myArgs.Mum && myArgs.Mumreference))
                {
                    Console.Error.WriteLine("\nError: only one of -maxmatch, -mum, -mumreference options can be specified.");
                    Environment.Exit(-1);
                }

                if (!myArgs.Mumreference && !myArgs.Mum && !myArgs.MaxMatch)
                {
                    myArgs.Mumreference = true;
                }
            }

            if (myArgs.FileList == null || myArgs.FileList.Length == 0)
            {
                Console.Error.WriteLine("\nError: Atleast one input file needed.");
                Environment.Exit(-1);
            }

            if (myArgs.FileList.Length < 2 && !myArgs.PerformLISOnly)
            {
                Console.Error.WriteLine("\nError: A reference file and at least 1 query file are required.");
                Environment.Exit(-1);
            }

            if (myArgs.FileList.Length < 1 && myArgs.PerformLISOnly)
            {
                Console.Error.WriteLine("\nError: A file containing list of MUMs required.");
                Environment.Exit(-1);
            }

            if ((myArgs.Length <= 2) || (myArgs.Length >= (8 * 1024)))
            {
                // TODO: What are real reasonable mum length limits?
                Console.Error.WriteLine("\nError: mum length must be between 10 and 1024.");
                Environment.Exit(-1);
            }

            if (myArgs.Both && myArgs.ReverseOnly)
            {
                Console.Error.WriteLine("\nError: only one of -both or -reverseOnly options can be specified.");
                Environment.Exit(-1);
            }

            if (myArgs.C && (!myArgs.Both && !myArgs.ReverseOnly))
            {
                Console.Error.WriteLine("\nError: c requires one of either /b or /r options.");
                Environment.Exit(-1);
            }

            if (myArgs.OutputFile != null)
            {   // redirect stdout
                textWriterConsoleOutSave = Console.Out;
                fileStreamConsoleOut = new FileStream(myArgs.OutputFile, FileMode.Create);
                streamWriterConsoleOut = new StreamWriter(fileStreamConsoleOut);
                Console.SetOut(streamWriterConsoleOut);
                streamWriterConsoleOut.AutoFlush = true;
            }

            return (myArgs);
        }

        private static string SplashString()
        {
            const string SplashString = "\nLisUtil v2.0 - Longest Increasing Subsequence Utility"
                                      + "\n  Copyright (c) 2011-2014, The Outercurve Foundation.";
            return (SplashString);
        }

        // Given a list of sequences, create a new list with only the Reverse Complements
        //   of the original sequences.
        private static IEnumerable<ISequence> ReverseComplementSequenceList(IEnumerable<ISequence> sequenceList)
        {
            foreach (ISequence seq in sequenceList)
            {
                ISequence seqReverseComplement = seq.GetReverseComplementedSequence();
                if (seqReverseComplement != null)
                {
                    seqReverseComplement.MarkAsReverseComplement();
                    yield return seqReverseComplement;
                }
            }
        }

        // Given a list of sequences, create a new list with the original sequence followed
        // by the Reverse Complement of that sequence.
        private static IEnumerable<ISequence> AddReverseComplementsToSequenceList(IEnumerable<ISequence> sequenceList)
        {
            foreach (ISequence seq in sequenceList)
            {
                yield return seq;

                ISequence seqReverseComplement = seq.GetReverseComplementedSequence();
                if (seqReverseComplement != null)
                {
                    seqReverseComplement.MarkAsReverseComplement();
                    yield return seqReverseComplement;
                }
            }
        }

        private static void WriteMums(IEnumerable<Match> mums, ISequence refSequence, ISequence querySequence, CommandLineOptions myArgs)
        {
            // write the QuerySequenceId
            string displayID = querySequence.ID;
            Console.Write("> {0}", displayID);
            if (myArgs.DisplayQueryLength)
            {
                Console.Write(" "+querySequence.Count);
            }

            Console.WriteLine();

            bool isReverseComplement = myArgs.C && querySequence.IsMarkedAsReverseComplement();

            // foreach (MaxUniqueMatch m in sortedMums)
            // {
            // Start is 1 based in literature but in programming (e.g. MaxUniqueMatch) they are 0 based.  
            // Add 1
            foreach (Match match in mums)
            {
                Console.WriteLine(
                        "{0,8}  {1,8}  {2,8}",
                        match.ReferenceSequenceOffset + 1,
                        !isReverseComplement ? match.QuerySequenceOffset + 1 : querySequence.Count - match.QuerySequenceOffset,
                        match.Length);
                if (myArgs.ShowMatchingString)
                {
                    for (int i = 0; i < match.Length; ++i)
                    {
                        // mummer uses all lowercase and .NET Bio uses uppercase...convert on display
                        Console.Write(char.ToLowerInvariant((char)querySequence[match.QuerySequenceOffset + i]));
                    }

                    Console.WriteLine();
                }
            }
        }

        private static void WriteMums(IEnumerable<Match> mums)
        {
            foreach (Match match in mums)
            {
                Console.WriteLine(
                        "{0,8}  {1,8}  {2,8}",
                        match.ReferenceSequenceOffset + 1,
                        match.QuerySequenceOffset + 1,
                        match.Length);
            }
        }

        private static void ShowSequence(ISequence seq)
        {
            const long DefaultSequenceDisplayLength = 25;

            Console.Error.WriteLine("--- Sequence Dump ---");
            Console.Error.WriteLine("     Type: {0}", seq.GetType());
            Console.Error.WriteLine("       ID: {0}", seq.ID);
            Console.Error.WriteLine("    Count: {0}", seq.Count);
            Console.Error.WriteLine(" Alphabet: {0}", seq.Alphabet);
            long lengthToPrint = (seq.Count <= DefaultSequenceDisplayLength) ? seq.Count : DefaultSequenceDisplayLength;
            StringBuilder printString = new StringBuilder((int)lengthToPrint);
            for (int i = 0; i < lengthToPrint; ++i)
            {
                printString.Append((char)seq[i]);
            }

            Console.Error.WriteLine(" Sequence: {0}{1}", printString, lengthToPrint >= DefaultSequenceDisplayLength ? "..." : String.Empty);
            Console.Error.WriteLine();
        }

        private static void Main(string[] args)
        {
            try
            {
                Stopwatch stopWatchMumUtil = Stopwatch.StartNew();
                Stopwatch stopWatchInterval = new Stopwatch();
                Console.Error.WriteLine(SplashString());
                if (args.Length > 0)
                {
                    CommandLineOptions myArgs = ProcessCommandLine(args);

                    TimeSpan writetime = new TimeSpan();
                    LongestIncreasingSubsequence lis = new LongestIncreasingSubsequence();
                    IEnumerable<Match> mums;
                    if (myArgs.PerformLISOnly)
                    {
                        stopWatchInterval.Restart();
                        IList<Match> parsedMUMs = ParseMums(myArgs.FileList[0]);
                        stopWatchInterval.Stop();

                        if (myArgs.Verbose)
                        {
                            Console.Error.WriteLine();
                            Console.Error.WriteLine("  Processed MUM file: {0}", Path.GetFullPath(myArgs.FileList[0]));
                            Console.Error.WriteLine("        Total MUMs: {0:#,000}", parsedMUMs.Count);
                            Console.Error.WriteLine("            Read/Processing time: {0}", stopWatchInterval.Elapsed);
                        }

                        stopWatchInterval.Restart();
                        IList<Match> sortedMUMs = lis.SortMum(parsedMUMs);
                        stopWatchInterval.Stop();

                        if (myArgs.Verbose)
                        {
                            Console.Error.WriteLine();
                            Console.Error.WriteLine("  Sort MUM time: {0}", stopWatchInterval.Elapsed);
                        }

                        stopWatchInterval.Restart();
                        if (sortedMUMs.Count != 0)
                        {
                            mums = lis.GetLongestSequence(sortedMUMs);

                            stopWatchInterval.Stop();
                            if (myArgs.Verbose)
                            {
                                Console.Error.WriteLine();
                                Console.Error.WriteLine("  Perform LIS time: {0}", stopWatchInterval.Elapsed);
                            }

                            stopWatchInterval.Restart();
                            WriteMums(mums);
                            stopWatchInterval.Stop();
                            if (myArgs.Verbose)
                            {
                                Console.Error.WriteLine();
                                Console.Error.WriteLine("  Write MUM time: {0}", stopWatchInterval.Elapsed);
                            }

                            stopWatchMumUtil.Stop();
                        }
                        else
                        {
                            stopWatchInterval.Stop();
                            if (myArgs.Verbose)
                            {
                                Console.Error.WriteLine();
                                Console.Error.WriteLine("  Perform LIS time: {0}", stopWatchInterval.Elapsed);
                            }

                            stopWatchInterval.Restart();
                            stopWatchInterval.Stop();
                            if (myArgs.Verbose)
                            {
                                Console.Error.WriteLine();
                                Console.Error.WriteLine("  Write MUM time: {0}", stopWatchInterval.Elapsed);
                            }

                            stopWatchMumUtil.Stop();
                        }
                    }
                    else
                    {
                        FileInfo refFileinfo = new FileInfo(myArgs.FileList[0]);
                        long refFileLength = refFileinfo.Length;
                        refFileinfo = null;

                        stopWatchInterval.Restart();
                        IEnumerable<ISequence> referenceSequences = ParseFastA(myArgs.FileList[0]);
                        ISequence referenceSequence = referenceSequences.First();
                        stopWatchInterval.Stop();
                        if (myArgs.Verbose)
                        {
                            Console.Error.WriteLine();
                            Console.Error.WriteLine("  Processed Reference FastA file: {0}", Path.GetFullPath(myArgs.FileList[0]));
                            Console.Error.WriteLine("        Length of first Sequence: {0:#,000}", referenceSequence.Count);
                            Console.Error.WriteLine("            Read/Processing time: {0}", stopWatchInterval.Elapsed);
                            Console.Error.WriteLine("            File Size           : {0}", refFileLength);
                        }

                        FileInfo queryFileinfo = new FileInfo(myArgs.FileList[1]);
                        long queryFileLength = queryFileinfo.Length;
                        refFileinfo = null;

                        stopWatchInterval.Restart();
                        IEnumerable<ISequence> querySequences = ParseFastA(myArgs.FileList[1]);
                        stopWatchInterval.Stop();
                        if (myArgs.Verbose)
                        {
                            Console.Error.WriteLine();
                            Console.Error.WriteLine("      Processed Query FastA file: {0}", Path.GetFullPath(myArgs.FileList[1]));
                            Console.Error.WriteLine("            Read/Processing time: {0}", stopWatchInterval.Elapsed);
                            Console.Error.WriteLine("            File Size           : {0}", queryFileLength);
                        }

                        if (myArgs.ReverseOnly)
                        {
                            stopWatchInterval.Restart();
                            querySequences = ReverseComplementSequenceList(querySequences);
                            stopWatchInterval.Stop();
                            if (myArgs.Verbose)
                            {
                                Console.Error.WriteLine("         Reverse Complement time: {0}", stopWatchInterval.Elapsed);
                            }
                        }
                        else if (myArgs.Both)
                        {   // add the reverse complement sequences to the query list too
                            stopWatchInterval.Restart();
                            querySequences = AddReverseComplementsToSequenceList(querySequences);
                            stopWatchInterval.Stop();
                            if (myArgs.Verbose)
                            {
                                Console.Error.WriteLine("     Add Reverse Complement time: {0}", stopWatchInterval.Elapsed);
                            }
                        }

                        TimeSpan mummerTime = new TimeSpan(0, 0, 0);
                        stopWatchInterval.Restart();
                        Sequence seq = referenceSequence as Sequence;
                        if (seq == null)
                        {
                            throw new ArgumentException("MUMmer supports only Sequence class");
                        }

                        MUMmer mummer = new MUMmer(seq);
                        stopWatchInterval.Stop();
                        if (myArgs.Verbose)
                        {
                            Console.Error.WriteLine("Suffix tree construction time: {0}", stopWatchInterval.Elapsed);
                        }

                        mummer.LengthOfMUM = myArgs.Length;
                        mummer.NoAmbiguity = myArgs.NoAmbiguity;
                        long querySeqCount = 0;
                        double sumofSeqLength = 0;
                        if (myArgs.MaxMatch)
                        {
                            foreach (ISequence querySeq in querySequences)
                            {
                                stopWatchInterval.Restart();
                                IList<Match> mumList = GetMumsForLIS(mummer.GetMatchesUniqueInReference(querySeq));
                                if (mumList.Count != 0)
                                {
                                    mums = lis.GetLongestSequence(lis.SortMum(mumList));
                                    stopWatchInterval.Stop();
                                    mummerTime = mummerTime.Add(stopWatchInterval.Elapsed);
                                    stopWatchInterval.Restart();
                                    WriteMums(mums, referenceSequence, querySeq, myArgs);
                                    stopWatchInterval.Stop();
                                    writetime = writetime.Add(stopWatchInterval.Elapsed);
                                    querySeqCount++;
                                    sumofSeqLength += querySeq.Count;
                                }
                                else
                                {
                                    stopWatchInterval.Stop();
                                    mummerTime = mummerTime.Add(stopWatchInterval.Elapsed);
                                    stopWatchInterval.Restart();
                                    stopWatchInterval.Stop();
                                    writetime = writetime.Add(stopWatchInterval.Elapsed);
                                    querySeqCount++;
                                    sumofSeqLength += querySeq.Count;
                                }
                            }

                            if (myArgs.Verbose)
                            {
                                Console.Error.WriteLine("             Number of query Sequences: {0}", querySeqCount);
                                Console.Error.WriteLine("             Average length of query Sequences: {0}", sumofSeqLength / querySeqCount);
                                Console.Error.WriteLine("  Compute GetMumsMaxMatch() with LIS time: {0}", mummerTime);
                            }
                        }
                        else if (myArgs.Mum)
                        {
                            foreach (ISequence querySeq in querySequences)
                            {
                                stopWatchInterval.Restart();
                                IList<Match> mumList = GetMumsForLIS(mummer.GetMatchesUniqueInReference(querySeq));
                                if (mumList.Count != 0)
                                {
                                    mums = lis.GetLongestSequence(lis.SortMum(mumList));
                                    stopWatchInterval.Stop();
                                    mummerTime = mummerTime.Add(stopWatchInterval.Elapsed);
                                    stopWatchInterval.Restart();
                                    WriteMums(mums, referenceSequence, querySeq, myArgs);
                                    stopWatchInterval.Stop();
                                    writetime = writetime.Add(stopWatchInterval.Elapsed);
                                    querySeqCount++;
                                    sumofSeqLength += querySeq.Count;
                                }
                                else
                                {
                                    stopWatchInterval.Stop();
                                    mummerTime = mummerTime.Add(stopWatchInterval.Elapsed);
                                    stopWatchInterval.Restart();
                                    stopWatchInterval.Stop();
                                    writetime = writetime.Add(stopWatchInterval.Elapsed);
                                    querySeqCount++;
                                    sumofSeqLength += querySeq.Count;
                                }
                            }

                            if (myArgs.Verbose)
                            {
                                Console.Error.WriteLine("             Number of query Sequences: {0}", querySeqCount);
                                Console.Error.WriteLine("             Average length of query Sequences: {0}", sumofSeqLength / querySeqCount);
                                Console.Error.WriteLine("       Compute GetMumsMum() with LIS time: {0}", mummerTime);
                            }
                        }
                        else if (myArgs.Mumreference)
                        {
                            // NOTE:
                            //     mum3.GetMUMs() this really implements the GetMumReference() functionality
                            // mums = mum3.GetMumsReference( referenceSequences[0], querySequences);     // should be
                            foreach (ISequence querySeq in querySequences)
                            {
                                stopWatchInterval.Restart();
                                IList<Match> mumList = GetMumsForLIS(mummer.GetMatchesUniqueInReference(querySeq));
                                if (mumList.Count != 0)
                                {
                                    mums = lis.GetLongestSequence(lis.SortMum(mumList));
                                    stopWatchInterval.Stop();
                                    mummerTime = mummerTime.Add(stopWatchInterval.Elapsed);
                                    stopWatchInterval.Restart();

                                    // do sort
                                    // WriteLongestIncreasingSubsequences
                                    WriteMums(mums, referenceSequence, querySeq, myArgs);
                                    stopWatchInterval.Stop();
                                    writetime = writetime.Add(stopWatchInterval.Elapsed);
                                    querySeqCount++;
                                    sumofSeqLength += querySeq.Count;
                                }
                                else
                                {
                                    stopWatchInterval.Stop();
                                    mummerTime = mummerTime.Add(stopWatchInterval.Elapsed);
                                    stopWatchInterval.Restart();
                                    stopWatchInterval.Stop();
                                    writetime = writetime.Add(stopWatchInterval.Elapsed);
                                    querySeqCount++;
                                    sumofSeqLength += querySeq.Count;
                                }
                            }

                            if (myArgs.Verbose)
                            {
                                Console.Error.WriteLine("             Number of query Sequences: {0}", querySeqCount);
                                Console.Error.WriteLine("             Average length of query Sequences: {0}", sumofSeqLength / querySeqCount);
                                Console.Error.WriteLine(" Compute GetMumsReference() time: {0}", mummerTime);
                            }
                        }
                        else
                        {
                            // cannot happen as argument processing already asserted one of the three options must be specified
                            Console.Error.WriteLine("\nError: one of /maxmatch, /mum, /mumreference options must be specified.");
                            Environment.Exit(-1);

                            // kill the error about unitialized use of 'mums' in the next block...the compiler does not recognize 
                            // Environment.Exit() as a no-return function
                            throw new Exception("Never hit this");
                        }
                    }

                    if (myArgs.Verbose)
                    {
                        Console.Error.WriteLine("                WriteMums() time: {0}", writetime);
                    }

                    stopWatchMumUtil.Stop();
                    if (myArgs.Verbose)
                    {
                        Console.Error.WriteLine("           Total LisUtil Runtime: {0}", stopWatchMumUtil.Elapsed);
                    }
                }
                else
                {
                    Console.WriteLine(Resources.LisUtilHelp);
                }
             }
            catch (Exception ex)
            {
                DisplayException(ex);
            }
        }

        /// <summary>
        /// Generates a list of MUMs for computing LIS.
        /// </summary>
        /// <param name="mums">MUMs generated by the MUMmer.</param>
        /// <returns>List of MUMs.</returns>
        private static IList<Match> GetMumsForLIS(IEnumerable<Match> mums)
        {
            return mums.Select(sortedMum => new Match { ReferenceSequenceOffset = sortedMum.ReferenceSequenceOffset, QuerySequenceOffset = sortedMum.QuerySequenceOffset, Length = sortedMum.Length }).ToList();
        }

        /// <summary>
        /// Parses MUMs from the input file.
        /// </summary>
        /// <param name="filename">MUM file name.</param>
        /// <returns>List of MUMs.</returns>
        private static IList<Match> ParseMums(string filename)
        {
            // TODO: Parse files with multiple query sequences
            IList<Match> mumList = new List<Match>();
            try
            {
                using (TextReader tr = File.OpenText(filename))
                {
                    string line;
                    while ((line = tr.ReadLine()) != null)
                    {
                        if (!line.StartsWith(">"))
                        {
                            string[] items = line.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (items[0] != ">")
                            {
                                Match mum2 = new Match
                                {
                                    ReferenceSequenceOffset = Convert.ToInt32(items[0]),
                                    QuerySequenceOffset = Convert.ToInt32(items[1]),
                                    Length = Convert.ToInt32(items[2])
                                };
                                mumList.Add(mum2);
                            }
                        }
                    }
                }

                return mumList;
            }
            catch
            {
                throw new FileFormatException(Resources.FileNotInProperFormat);
            }
        }

        /// <summary>
        /// Parses a FastA file which has one or more sequences.
        /// </summary>
        /// <param name="filename">Path to the file to be parsed.</param>
        /// <returns>List of ISequence objects.</returns>
        private static IEnumerable<ISequence> ParseFastA(string filename)
        {
            // A new parser to import a file
            FastAParser parser = new FastAParser();
            return parser.Parse(filename);
        }

        /// <summary>
        /// Display Exception Messages, if inner exception found then displays the inner exception.
        /// </summary>
        /// <param name="ex">The Exception.</param>
        private static void DisplayException(Exception ex)
        {
            if (ex.InnerException == null || string.IsNullOrEmpty(ex.InnerException.Message))
            {
                Console.Error.WriteLine(ex.Message);
            }
            else
            {
                DisplayException(ex.InnerException);
            }
        }

        private static void AddParameters(CommandLineArguments parser)
        {
            parser.Parameter(ArgumentType.Optional, "Help", ArgumentValueType.Bool, "h", 
                "Show the help information with program options and a description of program operation.");
            parser.Parameter(ArgumentType.Optional, "Verbose", ArgumentValueType.Bool, "v", "Display Verbose logging during processing");
            parser.Parameter(ArgumentType.Optional, "Mumreference", ArgumentValueType.Bool, "", "Compute all MUM candidates in reference [default]");
            parser.Parameter(ArgumentType.Optional, "Mum", ArgumentValueType.Bool, "", 
                "Compute Maximal Unique Matches, strings that are unique in both the reference and query sets");
            parser.Parameter(ArgumentType.Optional, "MaxMatch", ArgumentValueType.Bool, "", "Compute all maximal matches ignoring uniqueness");
            parser.Parameter(ArgumentType.Optional, "NoAmbiguity", ArgumentValueType.Bool, "n", 
                "Disallow ambiguity character matches and only match A, T, C, or G (case insensitive)");
            parser.Parameter(ArgumentType.Optional, "Length", ArgumentValueType.Int, "l", "Minimum match length [20]");
            parser.Parameter(ArgumentType.Optional, "Both", ArgumentValueType.Bool, "b", "Compute forward and reverse complement matches");
            parser.Parameter(ArgumentType.Optional, "ReverseOnly", ArgumentValueType.Bool, "r", "Compute only reverse complement matches");
            parser.Parameter(ArgumentType.Optional, "DisplayQueryLength", ArgumentValueType.Bool, "d", "Show the length of the query sequence");
            parser.Parameter(ArgumentType.Optional, "ShowMatchingString", ArgumentValueType.Bool, "s", "Show the matching substring in the output");
            parser.Parameter(ArgumentType.Optional, "C", ArgumentValueType.Bool, "c", 
                "Report the query position of a reverse complement match relative to the forward strand of the query sequence");
            parser.Parameter(ArgumentType.Optional, "OutputFile", ArgumentValueType.String, "o", "Output file");
            parser.Parameter(ArgumentType.Optional, "PerformLISOnly", ArgumentValueType.Bool, "p", "Perform LIS only on input list of MUMs");
            parser.Parameter(ArgumentType.DefaultArgument, "FileList", ArgumentValueType.MultipleUniqueStrings, "", "Query file(s) containing the query strings");

        }

        private class CommandLineOptions
        {
            // Mummer commandline from summer interns
            //   mummer -maxmatch -b -c -s -n chromosome_1_1_1_1.fasta read_chrom1_2000.fasta > mummer2000-n.txt
            // translates to
            //   mumutil -maxmatch -b -c -s -n chromosome_1_1_1_1.fasta read_chrom1_2000.fasta > mummer2000-n.txt

            public bool Help;

            public bool Verbose;


            public bool Mumreference;

            public bool Mum;

            public bool MaxMatch;


            public bool NoAmbiguity;            // -n 


            public int Length;                  // -l #


            public bool Both;                   // -b

            public bool ReverseOnly;            // -r


            public bool DisplayQueryLength;     // -d

            
            public bool ShowMatchingString;     // -s


            public bool C;                      // -c
#if false
            public bool F;             // -F
#endif
#if false
            
            public string referenceFilename;
            public string[] queryFiles;
#endif

            public string OutputFile;

            public string[] FileList;

            public bool PerformLISOnly = false;

            public CommandLineOptions()
            {
                // use assignments in the constructor to avoid the warning about unwritten variables
                this.Help = false;
                this.Verbose = false;
                this.Mumreference = false;
                this.Mum = false;
                this.MaxMatch = false;
                this.NoAmbiguity = false;
                this.Length = 20;
                this.Both = false;
                this.ReverseOnly = false;
                this.DisplayQueryLength = false;
                this.ShowMatchingString = false;
                this.C = false;
                this.OutputFile = null;
                this.FileList = null;
            }
        }
    }
}
