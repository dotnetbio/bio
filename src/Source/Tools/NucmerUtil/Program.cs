using System;
using System.Diagnostics;
using Bio.Util.ArgumentParser;
using NucmerUtil.Properties;

namespace NucmerUtil
{
    /// <summary>
    /// Class having main method for NUCmer Utility.
    /// </summary>
    public class Program
    {
        #region MainMethod

        /// <summary>
        /// Main Method for parsing command line arguments.
        /// </summary>
        /// <param name="args">Command line arguments passed.</param>
        public static void Main(string[] args)
        {
            DisplayErrorMessage(Resources.NUCmerSplashScreen);
            try
            {
                if ((args == null) || (args.Length < 2))
                {
                    DisplayErrorMessage(Resources.NUCmerUtilHelp);
                }
                else
                {
                    Stopwatch nucmerWatch = new Stopwatch();
                    nucmerWatch.Restart();
                    Alignment(args);
                    nucmerWatch.Stop();
                    Console.Error.WriteLine("    Total NucmerUtil Runtime: {0}", nucmerWatch.Elapsed);
                }
            }
            catch (Exception ex)
            {
                DisplayException(ex);
            }

            
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses NUCmer command line parameters.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Alignment(string[] args)
        {
            NucmerArguments arguments = new NucmerArguments();
            CommandLineArguments parser = new CommandLineArguments();

            // Add Nucmer parameters
            parser.Parameter(ArgumentType.DefaultArgument, "FilePath", ArgumentValueType.MultipleUniqueStrings, "", "File path");
            parser.Parameter(ArgumentType.Optional, "Mum", ArgumentValueType.Bool, "m", "Use anchor matches that are unique in both the reference and query.");
            parser.Parameter(ArgumentType.Optional, "MumReference", ArgumentValueType.Bool, "r", "Use anchor matches that are unique in the reference but not necessarily unique in the query (default behavior).");
            parser.Parameter(ArgumentType.Optional, "MaxMatch", ArgumentValueType.Bool, "x", "Use all anchor matches regardless of their uniqueness.");
            parser.Parameter(ArgumentType.Optional, "BreakLength", ArgumentValueType.Int, "b", "Distance an alignment extension will attempt to extend poor scoring regions before giving up (default 200).");
            parser.Parameter(ArgumentType.Optional, "MinCluster", ArgumentValueType.Int, "c", "Minimum cluster length (default 65).");
            parser.Parameter(ArgumentType.Optional, "DiagFactor", ArgumentValueType.Int, "d", "Maximum diagonal difference factor for clustering, i.e. diagonal difference / match separation (default 0.12).");
            parser.Parameter(ArgumentType.Optional, "Forward", ArgumentValueType.Bool, "f", "Use only the forward strand of the query sequence.");
            parser.Parameter(ArgumentType.Optional, "MaxGap", ArgumentValueType.Int, "g", "Maximum gap between two adjacent matches in a cluster (default 90).");
            parser.Parameter(ArgumentType.Optional, "Help", ArgumentValueType.Bool, "h", "Print the help information.");
            parser.Parameter(ArgumentType.Optional, "MinMatch", ArgumentValueType.Int, "l", "Minimum length of an maximal exact match (default 20).");
            parser.Parameter(ArgumentType.Optional, "OutputFile", ArgumentValueType.String, "o", "Output file");
            parser.Parameter(ArgumentType.Optional, "Reverse", ArgumentValueType.Bool, "e", "Use only the reverse complement of the query sequence.");
            parser.Parameter(ArgumentType.Optional, "NotExtend", ArgumentValueType.Bool, "n", "Toggle the outward extension of alignments from their anchoring clusters." +
                "Setting this flag will prevent alignment extensions but still align the DNA between clustered matches and create the .delta file.");
            parser.Parameter(ArgumentType.Optional, "Verbose", ArgumentValueType.Bool, "v", "Display verbose logging during processing.");
            parser.Parameter(ArgumentType.Optional, "FixedSeparation", ArgumentValueType.Bool, "s", "Maximum fixed diagonal difference.");

            if (args.Length > 1)
            {
                try
                {
                    parser.Parse(args, arguments);
                }
                catch (ArgumentException ex)
                {
                    DisplayErrorMessage(ex.Message);
                    DisplayErrorMessage(Resources.NUCmerUtilHelp);
                }

                if (arguments.Help)
                {
                    DisplayErrorMessage(Resources.NUCmerUtilHelp);
                }
                else if (arguments.FilePath.Length == 2)
                {
                    arguments.Align();
                }
                else
                {
                    DisplayErrorMessage(Resources.NUCmerUtilHelp);
                }
            }
            else
            {
                DisplayErrorMessage(Resources.NUCmerUtilHelp);
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
                DisplayErrorMessage(ex.Message);
            }
            else
            {
                DisplayException(ex.InnerException);
            }
        }

        /// <summary>
        /// Display error message on console.
        /// </summary>
        /// <param name="message">Error message.</param>
        private static void DisplayErrorMessage(string message)
        {
            Console.Write(message);
        }

        #endregion
    }
}
