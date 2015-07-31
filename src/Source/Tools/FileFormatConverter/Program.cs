using System;
using System.Linq;

using Bio.Util.ArgumentParser;

using FileFormatConverter.Properties;

namespace FileFormatConverter
{
    internal class Program
    {
        /// <summary>
        /// main entry point for application
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            Console.WriteLine(Resources.SplashScreen);
            try
            {
                if (args == null || args.Length < 1)
                {
                    Console.WriteLine(Resources.UsageHelp);
                }
                else
                {
                    ConvertFileFormat(args.ToArray());
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
                else
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// ConvertFileFormat method that will call the appropriate class to perform
        /// file conversion, this class using the command line parser to parse
        /// command line options and then calls the FileFormatConverter.convertFile method
        /// which does the work
        /// </summary>
        /// <param name="args"></param>
        private static void ConvertFileFormat(string[] args)
        {
            var ffc = new FileFormatConverter();
            var parser = new CommandLineArguments();

            //add parameters
            parser.Parameter(
                ArgumentType.DefaultArgument,
                "FileList",
                ArgumentValueType.MultipleUniqueStrings,
                "",
                "Input and Output files.");

            if (args.Length > 0)
            {
                try
                {
                    parser.Parse(args, ffc);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(Resources.UsageHelp);
                    Environment.Exit(-1);
                }

                if (ffc.FileList.Length != 2)
                {
                    Console.WriteLine(GetUsageMessage(ffc));
                }
                else
                {
                    ffc.InputFile = ffc.FileList[0];
                    ffc.OutputFile = ffc.FileList[1];

                    ffc.ConvertFile();
                }
            }
            else
            {
                Console.WriteLine(GetUsageMessage(ffc));
            }
        }

        /// <summary>
        /// gets the usage message, if the FileFormatConverter class is available we can also write the
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
                usage += Environment.NewLine;
                usage += Environment.NewLine;
                usage += String.Format("File extensions for parsing:\n{0}", ffc.ListOfExtensionsToParse());
                usage += Environment.NewLine;
                usage += Environment.NewLine;
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