using System;
using Bio.Util.ArgumentParser;
using ComparativeUtil.Properties;

namespace ComparativeUtil
{
    /// <summary>
    /// Program class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main function of program class.
        /// </summary>
        /// <param name="args">Arguments to the main function.</param>
        public static void Main(string[] args)
        {
            DisplayErrorMessage(Resources.ComparativeSplashScreen);
            DisplayErrorMessage("\n");
            try
            {
                if ((args == null) || (args.Length < 1))
                {
                    DisplayErrorMessage(Resources.AssembleHelp);
                }
                else
                {
                    if (args[0].Equals("Help", StringComparison.OrdinalIgnoreCase))
                    {
                        DisplayErrorMessage(Resources.AssembleHelp);
                    }
                    else
                    {
                        Assemble(args);
                    }
                }
            }
            catch (Exception ex)
            {
                CatchInnerException(ex);
            }
        }

        #region Private Methods

        /// <summary>
        /// Catches Inner Exception Messages.
        /// </summary>
        /// <param name="ex">The Exception.</param>
        private static void CatchInnerException(Exception ex)
        {
            if (ex.InnerException == null || string.IsNullOrEmpty(ex.InnerException.Message))
            {
                DisplayErrorMessage(ex.Message);
            }
            else
            {
                CatchInnerException(ex.InnerException);
            }
        }

        /// <summary>
        /// Assemble function.
        /// </summary>
        /// <param name="args">Arguments to Assemble.</param>
        private static void Assemble(string[] args)
        {
            AssembleArguments options = new AssembleArguments();
            CommandLineArguments parser = new CommandLineArguments();
            AddParameters(parser);

            if (args.Length > 0)
            {
                try
                {
                    parser.Parse(args, options);
                }
                catch (ArgumentParserException ex)
                {
                    DisplayErrorMessage(ex.Message);
                    DisplayErrorMessage(Resources.AssembleHelp);
                    Environment.Exit(-1);
                }

                if (options.Help)
                {
                    DisplayErrorMessage(Resources.AssembleHelp);
                }
                else
                {
                    if (options.FilePath != null)
                    {
                        options.AssembleSequences();
                    }
                    else
                    {
                        DisplayErrorMessage(Resources.AssembleHelp);
                        Environment.Exit(-1);
                    }

                }
            }
            else
            {
                DisplayErrorMessage(Resources.AssembleHelp);
            }
        }

        private static void AddParameters(CommandLineArguments parser)
        {
            parser.Parameter(ArgumentType.DefaultArgument, "FilePath", ArgumentValueType.MultipleUniqueStrings, "", "File path");
            parser.Parameter(ArgumentType.Optional, "KmerLength", ArgumentValueType.Int, "k", "Set kmer length");
            parser.Parameter(ArgumentType.Optional, "Scaffold", ArgumentValueType.Bool, "s", "Run scaffolding step after generating contigs.");
            parser.Parameter(ArgumentType.Optional, "MumLength", ArgumentValueType.Int, "m", "Mum Length");
            parser.Parameter(ArgumentType.Optional, "Help", ArgumentValueType.Bool, "h", "Print the help information.");
            parser.Parameter(ArgumentType.Optional, "OutputFile", ArgumentValueType.String, "o", "Output file");
            parser.Parameter(ArgumentType.Optional, "CloneLibraryName", ArgumentValueType.String, "n", "Clone Library Name");
            parser.Parameter(ArgumentType.Optional, "MeanLengthOfInsert", ArgumentValueType.Int, "i", "Mean Length of clone library.");
            parser.Parameter(ArgumentType.Optional, "StandardDeviationOfInsert", ArgumentValueType.Int, "sd", "Standard Deviation of Clone Library.");
            parser.Parameter(ArgumentType.Optional, "Verbose", ArgumentValueType.Bool, "v", "Display verbose logging during processing.");
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
