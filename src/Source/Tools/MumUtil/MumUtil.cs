using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Bio;
using Bio.Algorithms.MUMmer;
using Bio.Algorithms.SuffixTree;
using Bio.Extensions;
using Bio.IO.FastA;
using Bio.Util.ArgumentParser;
using MumUtil.Properties;

namespace MumUtil
{
    class MumUtil
    {
        class CommandLineOptions
        {
            //
            // Mummer commandline from summer interns
            //   mummer -maxmatch -b -c -s -n chromosome_1_1_1_1.fasta read_chrom1_2000.fasta > mummer2000-n.txt
            // translates to
            //   mumutil -maxmatch -b -c -s -n chromosome_1_1_1_1.fasta read_chrom1_2000.fasta > mummer2000-n.txt
            //
            
            public bool help = false;
            public bool verbose;
            public bool mumreference;           //
            public bool mum;                    // 
            public bool maxmatch;               // 
            public bool noAmbiguity;            // -n 
            public int length;                  // -l #
            public bool both;                   // -b
            public bool reverseOnly;            // -r
            public bool displayQueryLength;     // -d
            public bool showMatchingString;     // -s
            public bool c;                      // -c
#if false
            [Argument(ArgumentType.AtMostOnce, HelpText = "Force 4 column output format that prepends every match line with the reference sequence identifer")]
            public bool F;             // -F
#endif
#if false
            [Argument(ArgumentType.Required, HelpText = "Reference file containing the reference FASTA information")]
            public string referenceFilename;
            [DefaultArgument(ArgumentType.MultipleUnique, HelpText = "Query file(s) containing the query FASTA sequence information")]
            public string[] queryFiles;
#endif
            public string outputFile;
            public string[] fileList;

            public CommandLineOptions()
            {
                //  use assignments in the constructor to avoid the warning about unwritten variables
                help = false;
                verbose = false;
                mumreference = false;
                mum = false;
                maxmatch = false;
                noAmbiguity = false;
                length = 20;
                both = false;
                reverseOnly = false;
                displayQueryLength = false;
                showMatchingString = false;
                c = false;
                outputFile = null;
                //referenceFilename = null;
                //queryFiles = null;
                fileList = null;
            }
        }
        static TimeSpan mummerTime = new TimeSpan();
        static TimeSpan writetime = new TimeSpan();
        static FileStream fsConsoleOut;
        static StreamWriter swConsoleOut;
        static TextWriter twConsoleOutSave;
        static TimeSpan timeTakenToGetReverseComplement = new TimeSpan();
        static TimeSpan timeTakenToParseQuerySequences = new TimeSpan();
        static CommandLineOptions ProcessCommandLine(string[] args)
        {
            CommandLineOptions myArgs = new CommandLineOptions();
            CommandLineArguments parser = new CommandLineArguments();
            AddParameters(parser);
            try
            {
                parser.Parse(args, myArgs);
            }

            catch (Exception e)
            {
                Console.Error.WriteLine("\nException while processing Command Line arguments [{0}]", e.Message);
                Environment.Exit(-1);
            }

            if (myArgs.help)
            {
                Console.WriteLine(Resources.MumUtilHelp);
                Environment.Exit(-1);
            }

            /*
             * Process all the arguments for 'semantic' correctness
             */
            if ((myArgs.maxmatch && myArgs.mum)
                || (myArgs.maxmatch && myArgs.mumreference)
                || (myArgs.mum && myArgs.mumreference)
                )
            {
                Console.Error.WriteLine("\nError: only one of -maxmatch, -mum, -mumreference options can be specified.");
                Environment.Exit(-1);
            }
            if (!myArgs.mumreference && !myArgs.mum && !myArgs.maxmatch)
            {
                myArgs.mumreference = true;
            }
            if ((myArgs.fileList == null) || (myArgs.fileList.Length < 2))
            {
                Console.Error.WriteLine("\nError: A reference file and at least 1 query file are required.");
                Environment.Exit(-1);
            }
            if ((myArgs.length <= 0) || (myArgs.length >= (8 * 1024)))   // TODO: What are real reasonable mum length limits?
            {
                Console.Error.WriteLine("\nError: mum length must be between 1 and 1024.");
                Environment.Exit(-1);
            }
            if (myArgs.both && myArgs.reverseOnly)
            {
                Console.Error.WriteLine("\nError: only one of -both or -reverseOnly options can be specified.");
                Environment.Exit(-1);
            }
            if (myArgs.c && (!myArgs.both && !myArgs.reverseOnly))
            {
                Console.Error.WriteLine("\nError: c requires one of either /b or /r options.");
                Environment.Exit(-1);
            }
            if (myArgs.outputFile != null)
            {   // redirect stdout
                twConsoleOutSave = Console.Out;
                fsConsoleOut = new FileStream(myArgs.outputFile, FileMode.Create);
                swConsoleOut = new StreamWriter(fsConsoleOut);
                Console.SetOut(swConsoleOut);
                swConsoleOut.AutoFlush = true;
            }

            return (myArgs);
        }

        static string SplashString()
        {
            const string splashString = "\nMumUtil v2.0 - Maximal Unique Match Utility"
                                      + "\nCopyright (c) 2011-2014, The Outercurve Foundation.\n\n"
                                      + "NucmerUtil and MummerUtil are .NET re-implementations of MUMmer3.0, which was supported in part by the National Science Foundation under grants IIS-9902923 and IIS-9820497, and by the National Institutes of Health under grants R01-LM06845 and N01-AI-15447.  The development of the original reference was a joint effort by Stefan Kurtz of the University of Hamburg and Adam Phillippy, Art Delcher and Steven Salzberg at TIGR.\n";
            return (splashString);
        }

        //
        // Given a list of sequences, create a new list with only the Reverse Complements
        //   of the original sequences.
        static IEnumerable<ISequence> ReverseComplementSequenceList(IEnumerable<ISequence> sequenceList)
        {
            Stopwatch swInterval = new Stopwatch();
            swInterval.Restart();
            foreach (ISequence seq in sequenceList)
            {
                // Stop the watch after each query sequence parsed.
                swInterval.Stop();

                // Add the query sequence parse time.
                timeTakenToParseQuerySequences = timeTakenToParseQuerySequences.Add(swInterval.Elapsed);

                swInterval.Restart();
                ISequence seqReverseComplement = seq.GetReverseComplementedSequence();
                if (seqReverseComplement != null)
                    seqReverseComplement.MarkAsReverseComplement();
                swInterval.Stop();

                // Add the reverse complement time.
                timeTakenToGetReverseComplement = timeTakenToGetReverseComplement.Add(swInterval.Elapsed);

                yield return seqReverseComplement;

                // Start the watch for next query sequence parse.
                swInterval.Restart();
            }

            // Stop watch if there are not query sequences left.
            swInterval.Stop();
        }

        //
        // Given a list of sequences, create a new list with the orginal sequence followed
        // by the Reverse Complement of that sequence.
        static IEnumerable<ISequence> AddReverseComplementsToSequenceList(IEnumerable<ISequence> sequenceList)
        {
            Stopwatch swInterval = new Stopwatch();
            swInterval.Restart();
            foreach (ISequence seq in sequenceList)
            {
                // Stop the watch after each query sequence parsed.
                swInterval.Stop();

                // Add the query sequence parse time.
                timeTakenToParseQuerySequences = timeTakenToParseQuerySequences.Add(swInterval.Elapsed);

                swInterval.Restart();
                ISequence seqReverseComplement = seq.GetReverseComplementedSequence();
                if (seqReverseComplement != null)
                    seqReverseComplement.MarkAsReverseComplement();
                swInterval.Stop();

                // Add the reverse complement time.
                timeTakenToGetReverseComplement = timeTakenToGetReverseComplement.Add(swInterval.Elapsed);

                yield return seq;
                yield return seqReverseComplement;

                // Start the watch for next query sequence parse.
                swInterval.Restart();
            }

            // Stop watch if there are not query sequences left.
            swInterval.Stop();
        }

        private static void WriteMums(IEnumerable<Match> mums, Sequence refSequence, Sequence querySequence, CommandLineOptions myArgs)
        {
            // write the QuerySequenceId
            string DisplayID = querySequence.ID;
            Console.Write("> {0}", DisplayID);
            if (myArgs.displayQueryLength)
            {
                Console.Write(" " + querySequence.Count);
            }
            Console.WriteLine();

            // DISCUSSION:
            //   If a ReverseComplement sequence, MUMmer has the option to print the index start point relative 
            //   to the original sequence we tagged the reversed DisplayID with " Reverse" so _if_ we find a 
            //   " Reverse" on the end of the ID, assume it is a ReverseComplement and reverse the index
            bool isReverseComplement = myArgs.c && querySequence.IsMarkedAsReverseComplement();

            // Start is 1 based in literature but in programming (e.g. MaxUniqueMatch) they are 0 based.  
            // Add 1
            Stopwatch swInterval = new Stopwatch();
            swInterval.Start();

            foreach (Match match in mums)
            {
                swInterval.Stop();
                // Add time taken by GetMatches().
                mummerTime = mummerTime.Add(swInterval.Elapsed);


                swInterval.Restart();
                Console.WriteLine("{0,8}  {1,8}  {2,8}",
                        match.ReferenceSequenceOffset + 1,
                        !isReverseComplement ? match.QuerySequenceOffset + 1 : querySequence.Count - match.QuerySequenceOffset,
                        match.Length);
                if (myArgs.showMatchingString)
                {
                    StringBuilder sb = new StringBuilder((int)match.Length);
                    for (int i = 0; i < match.Length; ++i)
                    {
                        sb.Append((char)querySequence[match.QuerySequenceOffset + i]);
                    }

                    Console.WriteLine(sb.ToString());
                }

                swInterval.Stop();
                writetime = writetime.Add(swInterval.Elapsed);
                swInterval.Restart();
            }
        }

        static void ShowSequence(Sequence seq)
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
            Console.Error.WriteLine(" Sequence: {0}{1}", printString, lengthToPrint >= DefaultSequenceDisplayLength ? "..." : "");
            Console.Error.WriteLine();
        }

        static void Main(string[] args)
        {
            try
            {
                //            DateTime dStart = DateTime.Now;
                Stopwatch swMumUtil = Stopwatch.StartNew();
                Stopwatch swInterval = new Stopwatch();
                
                Console.Error.WriteLine(SplashString());
                if (args.Length > 0)
                {
                    CommandLineOptions myArgs = ProcessCommandLine(args);
                    if (myArgs.help)
                    {
                        Console.WriteLine(Resources.MumUtilHelp);
                    }
                    else
                    {
                        FileInfo refFileinfo = new FileInfo(myArgs.fileList[0]);
                        long refFileLength = refFileinfo.Length;
                        refFileinfo = null;

                        swInterval.Restart();
                        IEnumerable<ISequence> referenceSequences = ParseFastA(myArgs.fileList[0]);
                        Sequence referenceSequence = referenceSequences.First() as Sequence;
                        swInterval.Stop();
                        if (myArgs.verbose)
                        {
                            Console.Error.WriteLine();
                            Console.Error.WriteLine("  Processed Reference FastA file: {0}", Path.GetFullPath(myArgs.fileList[0]));
                            Console.Error.WriteLine("        Length of first Sequence: {0:#,000}", referenceSequence.Count);
                            Console.Error.WriteLine("            Read/Processing time: {0}", swInterval.Elapsed);
                            Console.Error.WriteLine("            File size           : {0:#,000} bytes", refFileLength);
                        }

                        FileInfo queryFileinfo = new FileInfo(myArgs.fileList[1]);
                        long queryFileLength = queryFileinfo.Length;
                        refFileinfo = null;

                        IEnumerable<ISequence> parsedQuerySequences = ParseFastA(myArgs.fileList[1]);

                        IEnumerable<ISequence> querySequences = parsedQuerySequences;

                        if (myArgs.reverseOnly)
                        {
                            // convert to reverse complement sequences
                            querySequences = ReverseComplementSequenceList(parsedQuerySequences);
                        }
                        else if (myArgs.both)
                        {
                            // add the reverse complement sequences along with query sequences.
                            querySequences = AddReverseComplementsToSequenceList(parsedQuerySequences);
                        }

                        // DISCUSSION:
                        // Three possible outputs desired.  Globally unique 'mum' (v1), unique in reference sequence (v2), 
                        //   or get the maximum matches of length or greater.  
                        //
                        mummerTime = new TimeSpan();
                        writetime = new TimeSpan();
                        IEnumerable<Match> mums;
                        long memoryAtStart = 0;
                        long memoryAtEnd = 0;
                        if (myArgs.verbose)
                        {
                            swMumUtil.Stop();
                            memoryAtStart = GC.GetTotalMemory(true);
                            swMumUtil.Start();
                        }

                        swInterval.Restart();
                        MultiWaySuffixTree suffixTreee = new MultiWaySuffixTree(referenceSequence);
                        swInterval.Stop();

                        if (myArgs.verbose)
                        {
                            swMumUtil.Stop();
                            memoryAtEnd = GC.GetTotalMemory(true);
                            swMumUtil.Start();
                        }

                        MUMmer mummer = new MUMmer(suffixTreee);

                        if (myArgs.verbose)
                        {
                            Console.Error.WriteLine();
                            Console.Error.WriteLine("Suffix tree construction time   : {0}", swInterval.Elapsed);
                            Console.Error.WriteLine("Memory consumed by Suffix tree  : {0:#,000}", memoryAtEnd - memoryAtStart);
                            Console.Error.WriteLine("Total edges created             : {0:#,000}", suffixTreee.EdgesCount);
                            Console.Error.WriteLine("Memory per edge                 : {0:#,000.00} bytes", (((double)(memoryAtEnd - memoryAtStart)) / suffixTreee.EdgesCount));
                            Console.Error.WriteLine();
                            Console.Error.WriteLine("      Processed Query FastA file: {0}", Path.GetFullPath(myArgs.fileList[1]));
                            Console.Error.WriteLine("            File Size           : {0:#,000} bytes", queryFileLength);
                        }

                        mummer.LengthOfMUM = myArgs.length;
                        mummer.NoAmbiguity = myArgs.noAmbiguity;

                        long querySeqCount = 0;
                        double sumofSeqLength = 0;
                        TimeSpan totalTimetakenToProcessQuerySequences = new TimeSpan();
                        string outputOption = string.Empty;

                        if (myArgs.maxmatch)
                        {
                            outputOption = "GetMumsMaxMatch()";
                            swInterval.Restart();
                            foreach (Sequence qSeq in querySequences)
                            {
                                // Stop the wath after each query sequence parsed.
                                swInterval.Stop();

                                // Add total time to process query sequence.
                                // if reverse complement option is set, includes reverse complement time also.
                                totalTimetakenToProcessQuerySequences = totalTimetakenToProcessQuerySequences.Add(swInterval.Elapsed);

                                mums = mummer.GetMatches(qSeq);

                                WriteMums(mums, referenceSequence, qSeq, myArgs);

                                querySeqCount++;
                                sumofSeqLength += qSeq.Count;

                                // Start the watch for next query sequence parse.
                                swInterval.Restart();
                            }

                            swInterval.Stop();
                        }
                        else if (myArgs.mum)
                        {
                            // mums = mum3.GetMumsMum( referenceSequences[0], querySequences);
                            outputOption = "GetMumsMum()";
                            swInterval.Restart();
                            foreach (Sequence qSeq in querySequences)
                            {
                                // Stop the wath after each query sequence parsed.
                                swInterval.Stop();

                                // Add total time to process query sequence.
                                // if reverse complement option is set, includes reverse complement time also.
                                totalTimetakenToProcessQuerySequences = totalTimetakenToProcessQuerySequences.Add(swInterval.Elapsed);

                                swInterval.Restart();
                                // TODO: After implementing GetMatchesUniqueInBothReferenceAndQuery() in MUMmer
                                ////       GetMatchesUniqueInReference() with GetMatchesUniqueInBothReferenceAndQuery() in the line below.
                                mums = mummer.GetMatchesUniqueInReference(qSeq);
                                swInterval.Stop();

                                // Add time taken by GetMatchesUniqueInBothReferenceAndQuery().
                                mummerTime = mummerTime.Add(swInterval.Elapsed);

                                swInterval.Restart();
                                WriteMums(mums, referenceSequence, qSeq, myArgs);
                                swInterval.Stop();

                                // Add time taken by write matches.
                                writetime = writetime.Add(swInterval.Elapsed);

                                querySeqCount++;
                                sumofSeqLength += qSeq.Count;

                                // Start the watch for next query sequence parse.
                                swInterval.Restart();
                            }

                            swInterval.Stop();
                        }
                        else if (myArgs.mumreference)
                        {
                            // NOTE:
                            //     mum3.GetMUMs() this really implements the GetMumReference() functionality
                            // mums = mum3.GetMumsReference( referenceSequences[0], querySequences);     // should be
                            //swInterval.Restart();
                            outputOption = "GetMumsReference()";
                            swInterval.Restart();
                            foreach (Sequence qSeq in querySequences)
                            {
                                // Stop the watch after each query sequence parsed.
                                swInterval.Stop();

                                // Add total time to process query sequence.
                                // if reverse complement option is set, includes reverse complement time also.
                                totalTimetakenToProcessQuerySequences = totalTimetakenToProcessQuerySequences.Add(swInterval.Elapsed);

                                swInterval.Restart();
                                mums = mummer.GetMatchesUniqueInReference(qSeq);
                                swInterval.Stop();

                                // Add time taken by GetMatchesUniqueInReference().
                                mummerTime = mummerTime.Add(swInterval.Elapsed);

                                swInterval.Restart();
                                WriteMums(mums, referenceSequence, qSeq, myArgs);
                                swInterval.Stop();

                                // Add time taken by write matches.
                                writetime = writetime.Add(swInterval.Elapsed);
                                querySeqCount++;
                                sumofSeqLength += qSeq.Count;

                                // Start the watch for next query sequence parse.
                                swInterval.Restart();
                            }

                            swInterval.Stop();
                        }
                        else
                        {
                            // cannot happen as argument processing already asserted one of the three options must be specified
                            Console.Error.WriteLine("\nError: one of /maxmatch, /mum, /mumreference options must be specified.");
                            Environment.Exit(-1);
                            // kill the error about unitialized use of 'mums' in the next block...the compiler does not recognize 
                            //   Environment.Exit() as a no-return function
                            throw new Exception("Never hit this");
                        }

                        if (myArgs.verbose)
                        {
                            if (myArgs.reverseOnly || myArgs.both)
                            {
                                Console.Error.WriteLine("        Read/Processing time          : {0}", timeTakenToParseQuerySequences);
                                Console.Error.WriteLine("     Reverse Complement time          : {0}", timeTakenToGetReverseComplement);
                                Console.Error.WriteLine("     Total time taken to Process reads: {0}", totalTimetakenToProcessQuerySequences);
                            }
                            else
                            {
                                Console.Error.WriteLine("        Read/Processing time          : {0}", totalTimetakenToProcessQuerySequences);
                            }

                            Console.Error.WriteLine();
                            Console.Error.WriteLine("         Number of query Sequences        : {0:#,000}", querySeqCount);
                            Console.Error.WriteLine("         Average length of query Sequences: {0:#,000}", sumofSeqLength / querySeqCount);
                            Console.Error.WriteLine();
                            Console.Error.WriteLine("Compute {0,20} time              : {1}", outputOption, mummerTime);
                            Console.Error.WriteLine("                WriteMums() time          : {0}", writetime);
                        }

                        swMumUtil.Stop();
                        if (myArgs.verbose)
                        {
                            Console.Error.WriteLine("           Total MumUtil Runtime      : {0}", swMumUtil.Elapsed);
                        }
                    }
                }
                else
                {
                    Console.WriteLine(Resources.MumUtilHelp);
                }
                     
            }
            catch (Exception ex)
            {
                DisplayException(ex);
            }

        }

        /// <summary>
        /// Parses a FastA file which has one or more sequences.
        /// </summary>
        /// <param name="filename">Path to the file to be parsed.</param>
        /// <returns>List of ISequence objects</returns>
        static IEnumerable<ISequence> ParseFastA(string filename)
        {
            // A new parser to import a file
            return new FastAParser().Parse(filename);
        }

        private static void AddParameters(CommandLineArguments parser)
        {
            parser.Parameter(ArgumentType.Optional, "help", ArgumentValueType.Bool, "h", "Show the help information with program options and a description of program operation.");
            parser.Parameter(ArgumentType.Optional, "Verbose", ArgumentValueType.Bool, "v", "Display verbose logging during processing.");
            parser.Parameter(ArgumentType.Optional, "mumreference", ArgumentValueType.Bool, "m", "Compute all MUM candidates in reference [default]");
            parser.Parameter(ArgumentType.Optional, "mum", ArgumentValueType.Bool, "", "Compute Maximal Unique Matches, strings that are unique in both the reference and query sets");
            parser.Parameter(ArgumentType.Optional, "maxmatch", ArgumentValueType.Bool, "", "Compute all maximal matches ignoring uniqueness");
            parser.Parameter(ArgumentType.Optional, "noAmbiguity", ArgumentValueType.Bool, "n", "Disallow ambiguity character matches and only match A, T, C, or G (case insensitive)");
            parser.Parameter(ArgumentType.Optional, "length", ArgumentValueType.Int, "l", "Minimium match length [20]");
            parser.Parameter(ArgumentType.Optional, "both", ArgumentValueType.Bool, "b", "Compute forward and reverse complement matches");
            parser.Parameter(ArgumentType.Optional, "reverseOnly", ArgumentValueType.Bool, "r", "Compute only reverse complement matches");
            parser.Parameter(ArgumentType.Optional, "displayQueryLength", ArgumentValueType.Bool, "d", "Show the length of the query sequence");
            parser.Parameter(ArgumentType.Optional, "showMatchingString", ArgumentValueType.Bool, "s", "Show the matching substring in the output");
            parser.Parameter(ArgumentType.Optional, "c", ArgumentValueType.Bool, "c", "Report the query position of a reverse complement match relative to the forward strand of the query sequence");
            parser.Parameter(ArgumentType.Optional, "outputFile", ArgumentValueType.String, "o", "Output file");
            parser.Parameter(ArgumentType.DefaultArgument, "fileList", ArgumentValueType.MultipleUniqueStrings, "", "Query file(s) containing the query strings");

        }

        /// <summary>
        /// Display Exception Messages, if inner exception found then displays the inner exception.
        /// </summary>
        /// <param name="ex">The Exception.</param>
        private static void DisplayException(Exception ex)
        {
            if (ex.InnerException == null || string.IsNullOrEmpty(ex.InnerException.Message))
            {
                Console.Error.WriteLine("\n"+ex.Message);
            }
            else
            {
                DisplayException(ex.InnerException);
            }
        }
    }
}
