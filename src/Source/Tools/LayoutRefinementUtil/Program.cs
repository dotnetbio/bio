using System;
using Bio.Util.ArgumentParser;
using LayoutRefinementUtil.Properties;

namespace LayoutRefinementUtil
{
    /// <summary>
    /// Class that provides options to execute step 3 (LayoutRefinement) of Comparative assembly.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method of the utility.
        /// </summary>
        /// <param name="args">Arguments to the main method.</param>
        public static void Main(string[] args)
        {
            Output.WriteLine(OutputLevel.Required, Resources.LayoutRefinementSplashScreen);
            try
            {
                if ((args == null) || (args.Length < 2))
                {
                    Output.WriteLine(OutputLevel.Required, Resources.LayoutRefinementUtilHelp);
                }
                else
                {
                    if (args[0].Equals("Help", StringComparison.OrdinalIgnoreCase))
                    {
                        Output.WriteLine(OutputLevel.Required, Resources.LayoutRefinementUtilHelp);
                    }
                    else
                    {
                        RepeatResolver(args);
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
                Output.WriteLine(OutputLevel.Error, "Error: " + ex.Message);
            }
            else
            {
                CatchInnerException(ex.InnerException);
            }
        }

        /// <summary>
        /// Parses RepeatResolver command line parameters.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void RepeatResolver(string[] args)
        {
            LayoutRefinementArguments arguments = new LayoutRefinementArguments();
            CommandLineArguments parser = new CommandLineArguments();

            // Add parameters
            parser.Parameter(ArgumentType.DefaultArgument, "FilePath", ArgumentValueType.MultipleUniqueStrings, "", "File path");
            parser.Parameter(ArgumentType.Optional, "Help", ArgumentValueType.Bool, "h", "Print the help information.");
            parser.Parameter(ArgumentType.Optional, "OutputFile", ArgumentValueType.String, "o", "Output file");
            parser.Parameter(ArgumentType.Optional, "Verbose", ArgumentValueType.Bool, "v", "Display verbose logging during processing.");

            if (args.Length > 1)
            {
                try
                {
                    parser.Parse(args, arguments);
                }
                catch (ArgumentException ex)
                {
                    Output.WriteLine(OutputLevel.Error, ex.Message);
                    Output.WriteLine(OutputLevel.Required, Resources.LayoutRefinementUtilHelp);
                    return;
                }

                if (arguments.Help || arguments.FilePath.Length != 2)
                {
                    Output.WriteLine(OutputLevel.Required, Resources.LayoutRefinementUtilHelp);
                }
                else
                {
                    if (arguments.Verbose)
                        Output.TraceLevel = OutputLevel.Information | OutputLevel.Verbose;
                    else
                        Output.TraceLevel = OutputLevel.Information;

                    arguments.RefineLayout();
                }
            }
            else
            {
                Output.WriteLine(OutputLevel.Required, Resources.LayoutRefinementUtilHelp);
            }
        }

        #endregion
    }
}
