using System;
using Bio.Util.ArgumentParser;
using SamUtil.Properties;

namespace SamUtil
{
    /// <summary>
    /// Class having main method for SAMUtility.
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
            try
            {
                DisplayErrorMessage(Resources.SamUtilSplashScreen);
                DisplayErrorMessage(Resources.AttributionText);

                if (args.Length < 1)
                {
                    DisplayErrorMessage(Resources.SAMUtilHelp);
                }
                else
                {
                    string[] arguments = new string[args.Length - 1];
                    for (int index = 1; index < args.Length; index++)
                    {
                        arguments[index - 1] = args[index];
                    }

                    if (args[0].Equals("Import", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ImportOption(arguments);
                    }
                    else if (args[0].Equals("Sort", StringComparison.InvariantCultureIgnoreCase))
                    {
                        SortOption(arguments);
                    }
                    else if (args[0].Equals("Merge", StringComparison.InvariantCultureIgnoreCase))
                    {
                        MergeOption(arguments);
                    }
                    else if (args[0].Equals("View", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ViewOption(arguments);
                    }
                    else if (args[0].Equals("Index", StringComparison.InvariantCultureIgnoreCase))
                    {
                        IndexOption(arguments);
                    }
                    else if (args[0].Equals("Chimera", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ChimericRegionOption(arguments);
                    }
                    else if (args[0].Equals("Orphan", StringComparison.InvariantCultureIgnoreCase))
                    {
                        OrphanRegionOption(arguments);
                    }
                    else if (args[0].Equals("LengthAnomaly", StringComparison.InvariantCultureIgnoreCase))
                    {
                        LengthAnomalyOption(arguments);
                    }
                    else if (args[0].Equals("CoverageProfile", StringComparison.InvariantCultureIgnoreCase))
                    {
                        SeqDistributionOption(arguments);
                    }
                    else if (args[0].Equals("NucleotideDistribution", StringComparison.InvariantCultureIgnoreCase))
                    {
                        SeqPossibleOccurence(arguments);
                    }
                    else
                    {
                        DisplayErrorMessage(Resources.SAMUtilHelp);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                {
                    DisplayErrorMessage(ex.InnerException.Message);
                }
                else
                {
                    DisplayErrorMessage(ex.Message);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parse command line arguments for view command.
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        private static void ViewOption(string[] args)
        {
            View options = new View();
            CommandLineArguments parser = new CommandLineArguments();

            // Add the parameters
            parser.Parameter(ArgumentType.Optional, "BAMOutput", ArgumentValueType.Bool, "b", "Output BAM");
            parser.Parameter(ArgumentType.Optional, "Header", ArgumentValueType.Bool, "h", "Print header for the SAM output");
            parser.Parameter(ArgumentType.Optional, "HeaderOnly", ArgumentValueType.Bool, "H", "Print header only (no alignments)");
            parser.Parameter(ArgumentType.Optional, "SAMInput", ArgumentValueType.Bool, "S", "Input is SAM format");
            parser.Parameter(ArgumentType.Optional, "UnCompressedBAM", ArgumentValueType.Bool, "u", "Uncompressed BAM output");
            parser.Parameter(ArgumentType.Optional, "FlagInHex", ArgumentValueType.Bool, "x", "Output FLAG in HEX");
            parser.Parameter(ArgumentType.Optional, "FlagAsString", ArgumentValueType.Bool, "X", "Output FLAG in string");
            parser.Parameter(ArgumentType.Optional, "ReferenceNamesAndLength", ArgumentValueType.String, "t",
                "List of reference names and lengths in a tab limited file rest all field will be ignored.");
            parser.Parameter(ArgumentType.Optional, "ReferenceSequenceFile", ArgumentValueType.String, "T", "Reference sequence file");
            parser.Parameter(ArgumentType.Optional, "OutputFilename", ArgumentValueType.String, "o", "Output file name");
            parser.Parameter(ArgumentType.Optional, "FlagRequired", ArgumentValueType.Int, "f", "Required flag");
            parser.Parameter(ArgumentType.Optional, "FilteringFlag", ArgumentValueType.Int, "F", "Filtering flag");
            parser.Parameter(ArgumentType.Optional, "QualityMinimumMapping", ArgumentValueType.Int, "q", "Minimum mapping quality");
            parser.Parameter(ArgumentType.Optional, "Library", ArgumentValueType.String, "l", "Only output reads in library");
            parser.Parameter(ArgumentType.Optional, "ReadGroup", ArgumentValueType.String, "r", "Only output reads in read group");
            parser.Parameter(ArgumentType.Optional, "Region", ArgumentValueType.String, "R",
                "A region can be presented, for example, in the following format:\n" +
                        "          ‘chr2’ (the whole chr2),\n" +
                        "          ‘chr2:1000000’ (region starting from 1,000,000bp)\n" +
                        "          or ‘chr2:1,000,000-2,000,000’\n" +
                        "          (region between 1,000,000 and 2,000,000bp including the end points).\n" +
                        "          The coordinate is 1-based.\n");
            parser.Parameter(ArgumentType.Optional, "Help", ArgumentValueType.Bool, "?", "");
            parser.Parameter(ArgumentType.DefaultArgument, "InputFilePath", ArgumentValueType.String, "", "Input SAM/BAM file path");

            if (args.Length > 0)
            {
                try
                {
                    parser.Parse(args, options);
                }
                catch (ArgumentParserException ex)
                {
                    DisplayErrorMessage(ex.Message);
                    DisplayErrorMessage(Resources.ViewHelp);
                    Environment.Exit(-1);
                }
                if (options.Help)
                {
                    DisplayErrorMessage(Resources.ViewHelp);
                }
                else
                {
                    options.ViewResult();
                }
            }
            else
            {
                DisplayErrorMessage(Resources.ViewHelp);
            }
        }

        /// <summary>
        /// Parse command line arguments for index command.
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        private static void IndexOption(string[] args)
        {
            Index options = new Index();
            CommandLineArguments parser = new CommandLineArguments();

            // Add the parameters
            parser.Parameter(ArgumentType.DefaultArgument, "InputFilename", ArgumentValueType.String, "", "Input BAM file name");
            parser.Parameter(ArgumentType.Optional, "OutputFilename", ArgumentValueType.String, "o", "Output file name");
            parser.Parameter(ArgumentType.Optional, "Help", ArgumentValueType.Bool, "", "");

            if (args.Length > 0)
            {
                try
                {
                    parser.Parse(args, options);
                }
                catch (ArgumentParserException ex)
                {
                    DisplayErrorMessage(ex.Message);
                    DisplayErrorMessage(Resources.IndexHelp);
                    Environment.Exit(-1);
                }
                if (options.Help)
                {
                    DisplayErrorMessage(Resources.IndexHelp);
                }
                else
                {
                    options.GenerateIndexFile();
                }
            }
            else
            {
                DisplayErrorMessage(Resources.IndexHelp);
            }
        }

        /// <summary>
        /// Parse command line arguments for Import command
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        private static void ImportOption(string[] args)
        {
            Import options = new Import();
            CommandLineArguments parser = new CommandLineArguments();

            // Add the parameters
            parser.Parameter(ArgumentType.DefaultArgument, "InputFilename", ArgumentValueType.String, "", "Input filename");
            parser.Parameter(ArgumentType.Optional, "Help", ArgumentValueType.Bool, "", "");
            parser.Parameter(ArgumentType.Optional, "ReferenceListFile", ArgumentValueType.String, "r", "Tab delimited file");
            parser.Parameter(ArgumentType.Optional, "OutputFilename", ArgumentValueType.String, "o", "Output file name");

            if (args != null && args.Length > 0)
            {
                try
                {
                    parser.Parse(args, options);
                }
                catch (ArgumentParserException ex)
                {
                    DisplayErrorMessage(ex.Message);
                    DisplayErrorMessage(Resources.ImportHelp);
                    Environment.Exit(-1);
                }

                if (options.Help)
                {
                    DisplayErrorMessage(Resources.ImportHelp);
                }
                else
                {
                    options.DoImport();
                }
            }
            else
            {
                DisplayErrorMessage(Resources.ImportHelp);
            }
        }

        /// <summary>
        /// Parse command line arguments for Merge command
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        private static void MergeOption(string[] args)
        {
            Merge options = new Merge();
            CommandLineArguments parser = new CommandLineArguments();

            //Add the parameters
            parser.Parameter(ArgumentType.Optional, "SortByReadName", ArgumentValueType.Bool, "n", "Sort by read name");
            parser.Parameter(ArgumentType.Optional, "HeaderFile", ArgumentValueType.String, "h", "Copy the Header from this file");
            parser.Parameter(ArgumentType.Optional, "Help", ArgumentValueType.Bool, "", "");
            parser.Parameter(ArgumentType.Optional, "OutputFilename", ArgumentValueType.String, "o", "Output file name");
            parser.Parameter(ArgumentType.DefaultArgument, "FilePaths", ArgumentValueType.MultipleUniqueStrings, "", "File Paths");

            if (args != null && args.Length >= 2)
            {
                try
                {
                    parser.Parse(args, options);
                }
                catch (ArgumentParserException ex)
                {
                    DisplayErrorMessage(ex.Message);
                    DisplayErrorMessage(Resources.MergeHelp);
                    Environment.Exit(-1);
                }
                if (options.Help)
                {
                    DisplayErrorMessage(Resources.MergeHelp);
                }
                else
                {
                    options.DoMerge();
                }
            }
            else
            {
                DisplayErrorMessage(Resources.MergeHelp);
            }
        }

        /// <summary>
        /// Parse command line arguments for sort command
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        private static void SortOption(string[] args)
        {
            Sort options = new Sort();
            CommandLineArguments parser = new CommandLineArguments();

            // Add the parameters
            parser.Parameter(ArgumentType.DefaultArgument, "InputFilename", ArgumentValueType.String, "", "File Paths");
            parser.Parameter(ArgumentType.Optional, "SortByReadName", ArgumentValueType.Bool, "n", "Sort by read name");
            parser.Parameter(ArgumentType.Optional, "Help", ArgumentValueType.Bool, "", "");
            parser.Parameter(ArgumentType.Optional, "OutputFilename", ArgumentValueType.String, "o", "Output file name");

            if (args.Length > 0)
            {
                try
                {
                    parser.Parse(args, options);
                }
                catch (ArgumentParserException ex)
                {
                    DisplayErrorMessage(ex.Message);
                    DisplayErrorMessage(Resources.SortHelp);
                    Environment.Exit(-1);
                }
                if (options.Help)
                {
                    DisplayErrorMessage(Resources.SortHelp);
                }
                else
                {
                    options.DoSort();
                }
            }
            else
            {
                DisplayErrorMessage(Resources.SortHelp);
            }
        }

        /// <summary>
        /// Parse command line arguments for Chimeric regions command
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        private static void ChimericRegionOption(string[] args)
        {
            ChimericRegions options = new ChimericRegions();
            CommandLineArguments parser = new CommandLineArguments();

            // Add the parameters
            parser.Parameter(ArgumentType.Optional, "SAMInput", ArgumentValueType.Bool, "S", "Input is SAM format");
            parser.Parameter(ArgumentType.Optional, "Help", ArgumentValueType.Bool, "", "");

            try
            {
                parser.Parse(args, options);
            }
            catch (ArgumentParserException ex)
            {
                DisplayErrorMessage(ex.Message);
                DisplayErrorMessage(Resources.ChimericRegionsHelp);
                Environment.Exit(-1);
            }
            if (options.Help)
            {
                DisplayErrorMessage(Resources.ChimericRegionsHelp);
            }
            else
            {
                options.DisplayChimericRegions(args[0]);
            }

        }

        /// <summary>
        /// Parse command line arguments for Orphan regions command
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        private static void OrphanRegionOption(string[] args)
        {
            Orphans options = new Orphans();
            CommandLineArguments parser = new CommandLineArguments();

            // Add the parameters
            parser.Parameter(ArgumentType.Optional, "SAMInput", ArgumentValueType.Bool, "S", "Input is SAM format");
            parser.Parameter(ArgumentType.Optional, "Help", ArgumentValueType.Bool, "", "");

            try
            {
                parser.Parse(args, options);
            }
            catch (ArgumentParserException ex)
            {
                DisplayErrorMessage(ex.Message);
                DisplayErrorMessage(Resources.OrphanRegionsHelp);
                Environment.Exit(-1);
            }

            if (options.Help)
            {
                DisplayErrorMessage(Resources.OrphanRegionsHelp);
            }
            else
            {
                options.DisplayOrpanChromosomes(args[0]);
            }

        }

        /// <summary>
        /// Parse command line arguments for Length anomaly regions command
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        private static void LengthAnomalyOption(string[] args)
        {
            LengthAnomaly options = new LengthAnomaly();

            if (args.Length > 2)
            {
                CommandLineArguments parser = new CommandLineArguments();

                // Add the parameters
                parser.Parameter(ArgumentType.Optional, "SAMInput", ArgumentValueType.Bool, "S", "Input is SAM format");
                parser.Parameter(ArgumentType.Optional, "Help", ArgumentValueType.Bool, "", "");

                try
                {
                    parser.Parse(args, options);
                }
                catch (ArgumentParserException ex)
                {
                    DisplayErrorMessage(ex.Message);
                    DisplayErrorMessage(Resources.LengthAnomalyHelp);
                    Environment.Exit(-1);
                }

                if (options.Help)
                {
                    DisplayErrorMessage(Resources.LengthAnomalyHelp);
                }
                else
                {
                    options.LengthAnamoly(args[0],
                       float.Parse(args[1]), float.Parse(args[2]));
                }
            }
            else
            {
                DisplayErrorMessage(Resources.LengthAnomalyHelp);
            }
        }

        /// <summary>
        /// Parse command line arguments for Sequence Distribution table for DNA
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        private static void SeqDistributionOption(string[] args)
        {
            SequenceCoverage options = new SequenceCoverage();
            CommandLineArguments parser = new CommandLineArguments();

            // Add the parameters
            parser.Parameter(ArgumentType.Optional, "SAMInput", ArgumentValueType.Bool, "S", "Input is SAM format");
            parser.Parameter(ArgumentType.Optional, "Help", ArgumentValueType.Bool, "", "");

            try
            {
                parser.Parse(args, options);
            }
            catch (ArgumentParserException ex)
            {
                DisplayErrorMessage(ex.Message);
                DisplayErrorMessage(Resources.DNACoverageHelp);
                Environment.Exit(-1);
            }

            if (options.Help)
            {
                DisplayErrorMessage(Resources.DNACoverageHelp);
            }
            else
            {
                options.DisplaySequenceItemOccurences(args[0], false);
            }

        }

        /// <summary>
        /// Parse command line arguments for Sequence Item Possible Occurence
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        private static void SeqPossibleOccurence(string[] args)
        {

            SequenceCoverage options = new SequenceCoverage();
            CommandLineArguments parser = new CommandLineArguments();

            // Add the parameters
            parser.Parameter(ArgumentType.Optional, "SAMInput", ArgumentValueType.Bool, "S", "Input is SAM format");
            parser.Parameter(ArgumentType.Optional, "Help", ArgumentValueType.Bool, "", "");

            try
            {
                parser.Parse(args, options);
            }
            catch (ArgumentParserException ex)
            {
                DisplayErrorMessage(ex.Message);
                DisplayErrorMessage(Resources.DNAPossibleOccurenceHelp);
                Environment.Exit(-1);
            }

            if (options.Help)
            {
                DisplayErrorMessage(Resources.DNAPossibleOccurenceHelp);
            }

            else
            {
                options.DisplaySequenceItemOccurences(args[0], true);
            }

        }
        /// <summary>
        /// Display error message on console.
        /// </summary>
        /// <param name="message">Error message.</param>
        private static void DisplayErrorMessage(string message)
        {
            Console.WriteLine(message);
        }

        #endregion
    }
}
