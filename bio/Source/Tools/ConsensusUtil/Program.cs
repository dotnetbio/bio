using System;
using Bio.Util.ArgumentParser;
using ConsensusUtil.Properties;

namespace ConsensusUtil
{
    /// <summary>
    /// Class that provides options to execute step 4 (ConsensusGeneration) of Comparative assembly.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method of the utility.
        /// </summary>
        /// <param name="args">Arguments to the main method.</param>
        public static void Main(string[] args)
        {
            DisplayErrorMessage(Resources.ConsensusSplashScreen);
            try
            {
                if ((args == null) || (args.Length < 1))
                {
                    DisplayErrorMessage(Resources.ConsensusUtilHelp);
                }
                else
                {
                    if (args[0].Equals("Help", StringComparison.OrdinalIgnoreCase))
                    {
                        DisplayErrorMessage(Resources.ConsensusUtilHelp);
                    }
                    else
                    {
                        ConsensusGeneration(args);
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
        /// Parses ConsensusGeneration command line parameters.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void ConsensusGeneration(string[] args)
        {
            ConsensusArguments arguments = new ConsensusArguments();
            CommandLineArguments parser = new CommandLineArguments();

            // Add the parameters
            parser.Parameter(ArgumentType.DefaultArgument, "FilePath", ArgumentValueType.MultipleUniqueStrings, "", "File path");
            parser.Parameter(ArgumentType.Optional, "Help", ArgumentValueType.Bool, "h", "Print the help information.");
            parser.Parameter(ArgumentType.Optional, "OutputFile", ArgumentValueType.String, "o", "Output file.");
            parser.Parameter(ArgumentType.Optional, "Verbose", ArgumentValueType.Bool, "v", "Display verbose logging during processing.");

            if (args.Length > 1)
            {
                try
                {
                    parser.Parse(args, arguments);
                }
                catch (ArgumentException ex)
                {
                    DisplayErrorMessage(ex.Message);
                    DisplayErrorMessage(Resources.ConsensusUtilHelp);
                    Environment.Exit(-1);
                }

                if (arguments.Help)
                {
                    DisplayErrorMessage(Resources.ConsensusUtilHelp);
                }
                else if (arguments.FilePath.Length == 2)
                {
                    arguments.GenerateConsensus();
                }
                else
                {
                    DisplayErrorMessage(Resources.ConsensusUtilHelp);
                }
            }
            else
            {
                DisplayErrorMessage(Resources.ConsensusUtilHelp);
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
