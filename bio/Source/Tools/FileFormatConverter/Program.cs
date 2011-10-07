using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bio.Util.ArgumentParser;
using FileFormatConverter.Properties;

namespace FileFormatConverter
{
    class Program
    {
        /// <summary>
        /// main entry point for application
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            DisplayErrorMessage(Resources.SplashScreen);
            try
            {
                if (args == null || args.Length < 1)
                {
                    DisplayErrorMessage(Resources.UsageHelp);
                }
                else
                {
                    string[] arguments = new string[args.Length];
                    for (int index = 0; index < args.Length; index++)
                    {
                        arguments[index] = args[index];
                    }

                    ConvertFileFormat(arguments);
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

        /// <summary>
        /// ConvertFileFormat method that will call the appropriate class to perform
        /// file conversion, this class using the commandline parser to parse 
        /// command line options and then calls the FileFormatConverter.convertFile method
        /// which does the work
        /// </summary>
        /// <param name="args"></param>
        private static void ConvertFileFormat(string[] args)
        {
            FileFormatConverter ffc = new FileFormatConverter();
            CommandLineArguments parser = new CommandLineArguments();

            //add parameters
            parser.Parameter(ArgumentType.Optional, "Help", ArgumentValueType.Bool, "h", "Print the help information.");
            parser.Parameter(ArgumentType.Optional, "OutputFile", ArgumentValueType.String, "o", "Output file formatted as specified.");
            parser.Parameter(ArgumentType.Optional, "InputFile", ArgumentValueType.String, "i", "Input file");

            if (args.Length > 0)
            {
                try
                {
                    parser.Parse(args, ffc);
                }
                catch (ArgumentException ex)
                {
                    DisplayErrorMessage(ex.Message);
                    DisplayErrorMessage(Resources.UsageHelp);
                    Environment.Exit(-1);
                }
                if (ffc.Help)
                {
                    string usage = GetUsageMessage(ffc);
                    DisplayErrorMessage(usage);
                }
                else
                {
                    ffc.ConvertFile();
                }
            }
            else
            {
                string usage = GetUsageMessage(ffc);
                DisplayErrorMessage(usage);
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
        /// gets the usage message, if the fileformatconverter class is available we can also write the
        /// available file extensions that we can parse and convert to otherwise just use static
        /// usage text
        /// </summary>
        /// <param name="ffc"></param>
        /// <returns></returns>
        private static string GetUsageMessage(FileFormatConverter ffc)
        {
            string usage = string.Empty;

            if (ffc != null)
            {
                usage = Resources.UsageHelp;
                usage += System.Environment.NewLine;
                usage += System.Environment.NewLine;
                usage += String.Format("File extensions for parsing:\n{0}", ffc.ListOfExtensionsToParse());
                usage += System.Environment.NewLine;
                usage += System.Environment.NewLine;
                usage += String.Format("File extensions for conversion:\n{0}", ffc.ListOfExtensionsForConversion());
            }
            else
            {
                usage = Resources.UsageHelp;
            }

            return usage;
        }
    }
}

