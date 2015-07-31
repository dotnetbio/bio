using System;
using Bio.Util.ArgumentParser;
using ScaffoldUtil.Properties;

namespace ScaffoldUtil
{
    /// <summary>
    /// Class that provides options to execute step 5 (ScaffoldGeneration) of Comparative assembly.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method of the utility.
        /// </summary>
        /// <param name="args">Arguments to the main method.</param>
        public static void Main(string[] args)
        {
            DisplayErrorMessage(Resources.ScaffoldSplashScreen);
            try
            {
                if ((args == null) || (args.Length < 2))
                {
                    DisplayErrorMessage(Resources.ScaffoldUtilHelp);
                }
                else
                {
                    if (args[0].Equals("Help", StringComparison.OrdinalIgnoreCase))
                    {
                        DisplayErrorMessage(Resources.ScaffoldUtilHelp);
                    }
                    else
                    {
                        ScaffoldGeneration(args);
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
                Console.Error.WriteLine("Error: " + ex.Message);
            }
            else
            {
                CatchInnerException(ex.InnerException);
            }
        }

        /// <summary>
        /// Parses ScaffoldGeneration command line parameters.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void ScaffoldGeneration(string[] args)
        {
            ScaffoldArguments arguments = new ScaffoldArguments();
            CommandLineArguments parser = new CommandLineArguments();

            // Add scaffold parameters
            parser.Parameter(ArgumentType.DefaultArgument, "FilePath", ArgumentValueType.MultipleUniqueStrings, "", "File path");
            parser.Parameter(ArgumentType.Optional, "Help", ArgumentValueType.Bool, "h", "Print the help information.");
            parser.Parameter(ArgumentType.Optional, "OutputFile", ArgumentValueType.String, "o", "Output file");
            parser.Parameter(ArgumentType.Optional, "Verbose", ArgumentValueType.Bool, "v", "Display verbose logging during processing.");
            parser.Parameter(ArgumentType.Optional, "KmerLength", ArgumentValueType.Int, "k", "Length of k-mer");
            parser.Parameter(ArgumentType.Optional, "Redundancy", ArgumentValueType.Int, "r", "Number of paired read required to connect two contigs.");
            parser.Parameter(ArgumentType.Optional, "Depth", ArgumentValueType.Int, "d", "Depth for graph traversal.");
            parser.Parameter(ArgumentType.Optional, "CloneLibraryName", ArgumentValueType.String, "n", "Clone Library Name");
            parser.Parameter(ArgumentType.Optional, "MeanLengthOfInsert", ArgumentValueType.Int, "i", "Mean Length of clone library.");
            parser.Parameter(ArgumentType.Optional, "StandardDeviationOfInsert", ArgumentValueType.Int, "sd", "Standard Deviation of Clone Library.");

            if (args.Length > 1)
            {
                try
                {
                    parser.Parse(args, arguments);
                }
                catch (ArgumentParserException ex)
                {
                    DisplayErrorMessage(ex.Message);
                    DisplayErrorMessage(Resources.ScaffoldUtilHelp);
                    Environment.Exit(-1);
                }

                if (arguments.Help)
                {
                    DisplayErrorMessage(Resources.ScaffoldUtilHelp);
                }
                else if (arguments.FilePath.Length == 2)
                {
                    arguments.GenerateScaffolds();
                }
                else
                {
                    DisplayErrorMessage(Resources.ScaffoldUtilHelp);
                } 
            }
            else
            {
                DisplayErrorMessage(Resources.ScaffoldUtilHelp);
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

        /// <summary>
        /// Removes the command line.
        /// </summary>
        /// <param name="args">Arguments to remove.</param>
        /// <returns>Returns the arguments.</returns>
        private static string[] RemoveCommandLine(string[] args)
        {
            string[] arguments = new string[args.Length - 1];
            for (int index = 0; index < args.Length - 1; index++)
            {
                arguments[index] = args[index + 1];
            }

            return arguments;
        }

        #endregion
    }
}
