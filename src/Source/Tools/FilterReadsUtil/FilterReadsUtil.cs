using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Bio;
using Bio.IO.FastA;
using Bio.Util.ArgumentParser;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace FilterReadsUtil
{
    /// <summary>
    /// Class to filter reads.
    /// </summary>
    public class FilterReadsUtil
    {
        /// <summary>
        /// Class to hold command line options.
        /// </summary>
        class CommandLineOptions
        {
            /// <summary>
            /// Option to show help.
            /// </summary>
            public bool Help = false;

            /// <summary>
            /// Verbose output option.
            /// </summary>
            public bool Verbose = false;

            /// <summary>
            /// Verbose output option.
            /// </summary>
            public bool FilterAmbiguousReads = true;

            /// <summary>
            /// Holds Output filename.
            /// </summary>
            public string OutputFile;

            /// <summary>
            /// Holds ambiguous output filename.
            /// </summary>
            public string AmbiguousOutputFile;

            /// <summary>
            /// input filename.
            /// </summary>
            public string Filename;

            /// <summary>
            /// Initializes a new instance of CommandLineOptions class.
            /// </summary>
            public CommandLineOptions()
            {
                //  use assignments in the constructor to avoid the warning about unwritten variables
                Help = false;
                Verbose = false;
                OutputFile = null;
                Filename = null;
                AmbiguousOutputFile = null;
                FilterAmbiguousReads = true;
            }
        }

        /// <summary>
        /// Blocking collection to hold ambiguous reads.
        /// </summary>
        private static BlockingCollection<ISequence> filteredAmbiguousReads = null;

        /// <summary>
        /// Process the command line arguments.
        /// </summary>
        /// <param name="args">commandline arguments.</param>
        /// <returns>Command line options.</returns>
        private static CommandLineOptions ProcessCommandLine(string[] args)
        {
            CommandLineOptions filterReadsParams = new CommandLineOptions();
            CommandLineArguments parser = new CommandLineArguments();

            // Add parameters
            parser.Parameter(ArgumentType.DefaultArgument, "Filename", ArgumentValueType.String, "", "Input fasta file to filter reads.");
            parser.Parameter(ArgumentType.Optional, "AmbiguousOutputFile", ArgumentValueType.String, "a", "Filtered ambiguous reads output file");
            parser.Parameter(ArgumentType.Optional, "OutputFile", ArgumentValueType.String, "o", "Output file");
            parser.Parameter(ArgumentType.Optional, "FilterAmbiguousReads", ArgumentValueType.Bool, "fa", "Filter ambiguous reads");
            parser.Parameter(ArgumentType.Optional, "Verbose", ArgumentValueType.Bool, "v", "Display Verbose logging");
            parser.Parameter(ArgumentType.Optional, "Help", ArgumentValueType.Bool, "h", "Show the help information with program options and a description of program operation.");

            try
            {
                parser.Parse(args, filterReadsParams);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("\nException while processing Command Line arguments [{0}]", e.Message);
                Console.Error.WriteLine(Properties.Resource.FilterReadsUtilHelp);
                Environment.Exit(-1);
            }

            if (filterReadsParams.OutputFile != null)
            {   // redirect stdout
                FileStream fsConsoleOut = new FileStream(filterReadsParams.OutputFile, FileMode.Create);
                StreamWriter swConsoleOut = new StreamWriter(fsConsoleOut);
                Console.SetOut(swConsoleOut);
                swConsoleOut.AutoFlush = true;
            }

            return (filterReadsParams);
        }

        /// <summary>
        /// Gets string to display for slpash screen.
        /// </summary>
        /// <returns>String to display in slpash screen.</returns>
        private static string SplashString()
        {
            const string splashString = "\nFilterReadsUtil v2.0 - Utility to filter reads"
                                      + "\n  Copyright (c) 2011-2014, The Outercurve Foundation.";
            return (splashString);
        }

        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">Commandline arguments.</param>
        private static void Main(string[] args)
        {
            try
            {
                Stopwatch swFilterUtil = Stopwatch.StartNew();

                Console.Error.WriteLine(SplashString());

                if (args.Length > 0)
                {
                    CommandLineOptions options = ProcessCommandLine(args);

                    if (options.Help)
                    {
                        Console.WriteLine(Properties.Resource.FilterReadsUtilHelp);
                    }
                    else
                    {
                        FileInfo refFileinfo = new FileInfo(options.Filename);
                        long refFileLength = refFileinfo.Length;
                        swFilterUtil.Restart();
                        IEnumerable<ISequence> reads = ParseFastA(options.Filename);
                        IEnumerable<ISequence> filteredReads = FilterAmbiguousReads(reads);
                        Task task = null;
                        if (!string.IsNullOrEmpty(options.AmbiguousOutputFile))
                        {
                            filteredAmbiguousReads = new BlockingCollection<ISequence>();
                            task = Task.Run(() => WriteAmbiguousReads(filteredAmbiguousReads, options.AmbiguousOutputFile));
                        }

                        WriteOutput(filteredReads);

                        if (task != null)
                        {
                            Task.WaitAll(task);
                        }
                    }
                }
                else
                {
                    Console.Error.WriteLine(Properties.Resource.FilterReadsUtilHelp);
                }
            }
            catch (Exception ex)
            {
                DisplayException(ex);
            }
        }

        /// <summary>
        /// Separates the ambiguous reads from the specified input reads from Unambiguous reads.
        /// Unambiguous reads are returned as IEnumerable, and ambiguous reads are added to the 
        /// filteredAmbiguousReads collection.
        /// </summary>
        /// <param name="inputReads">Input reads.</param>
        /// <returns>Reads without ambiguous symbols.</returns>
        private static IEnumerable<ISequence> FilterAmbiguousReads(IEnumerable<ISequence> inputReads)
        {
            foreach (ISequence seq in inputReads)
            {
                if (!seq.Alphabet.HasAmbiguity)
                {
                    yield return seq;
                }
                else if (filteredAmbiguousReads != null)
                {
                    filteredAmbiguousReads.Add(seq);
                }
            }

            if (filteredAmbiguousReads != null)
            {
                filteredAmbiguousReads.CompleteAdding();
            }
        }

        /// <summary>
        /// Parses a FastA file which has one or more sequences.
        /// </summary>
        /// <param name="filename">Path to the file to be parsed.</param>
        /// <returns>List of ISequence objects</returns>
        private static IEnumerable<ISequence> ParseFastA(string filename)
        {
            // A new parser to import a file
            FastAParser parser = new FastAParser();
            return parser.Parse(filename);
        }

        /// <summary>
        /// Writes the reads to StandardOutput.
        /// </summary>
        /// <param name="reads">Reads to write.</param>
        private static void WriteOutput(IEnumerable<ISequence> reads)
        {
            FastAFormatter formatter = new FastAFormatter();
            foreach (ISequence seq in reads)
            {
                Console.WriteLine(formatter.FormatString(seq));
            }
        }

        /// <summary>
        /// Writes ambiguous reads that are filtered out to the specified file.
        /// </summary>
        /// <param name="ambiguousReads">Reads with ambiguous symbols.</param>
        /// <param name="ambiguousFilename">File to write.</param>
        private static void WriteAmbiguousReads(BlockingCollection<ISequence> ambiguousReads, string ambiguousFilename)
        {
            FastAFormatter formatter = new FastAFormatter() { AutoFlush = true };
            using (formatter.Open(ambiguousFilename))
            {
                while (!ambiguousReads.IsCompleted)
                {
                    ISequence seq;
                    if (ambiguousReads.TryTake(out seq, -1))
                    {
                        formatter.Format(seq);
                    }
                }
            }
        }

        /// <summary>
        /// Display Exception Messages, if inner exception found then displays the inner exception.
        /// </summary>
        /// <param name="ex">The Exception.</param>
        private static void DisplayException(Exception ex)
        {
            if (ex.InnerException == null || string.IsNullOrEmpty(ex.InnerException.Message))
            {
                Console.Error.WriteLine("\n" + ex.Message);
            }
            else
            {
                DisplayException(ex.InnerException);
            }
        }
    }
}
