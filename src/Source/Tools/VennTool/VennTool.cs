using System;
using System.IO;
using Bio.Util.ArgumentParser;
using Tools.VennDiagram;

namespace VennTool
{
    class VennTool
    {
        static VennToolArguments arguments;

        static void Splash()
        {
            // Display the program's splash screen
            Console.WriteLine("VennTool V2.0\r\nCopyright (c) 2011-2014, The Outercurve Foundation.");
            Console.WriteLine();
        }

        public static VennToolArguments ProcessCommandLineArguments(string[] args)
        {
            VennToolArguments parsedArgs = new VennToolArguments();
            CommandLineArguments parser = new CommandLineArguments();
            
            // Add parameters
            parser.Parameter(ArgumentType.DefaultArgument, "regionArray", ArgumentValueType.MultipleInts, "", 
                "Values 3 or 7 values for regions in chart, [A B AB] or [A B C AB AC BC ABC]");
            parser.Parameter(ArgumentType.Optional, "xl", ArgumentValueType.String, "", "XL OutputFile");
            parser.Parameter(ArgumentType.Optional, "polar", ArgumentValueType.Bool, "", "Write result using polar coordinates");
            parser.Parameter(ArgumentType.Optional, "presort", ArgumentValueType.Bool, "", "PreSort .BED files prior to processing");
            parser.Parameter(ArgumentType.Optional, "Verbose", ArgumentValueType.Bool, "v", "Display Verbose output during processing");
            parser.Parameter(ArgumentType.Optional, "Help", ArgumentValueType.Bool, "h", "Print the help information.");

            try
            {
                parser.Parse(args, parsedArgs);
            }
            catch(ArgumentParserException ex)
            {
                Console.Error.WriteLine(ex.Message);
                DisplayHelpScreen();
                Environment.Exit(-1);
            }

            if (parsedArgs.Help)
            {
                DisplayHelpScreen();
                Environment.Exit(-1);
            }

            /*
             * Do any and all follow-up command line argument validation required
             */

            if ((parsedArgs.regionArray == null)
                || ((parsedArgs.regionArray.Length != 3) && (parsedArgs.regionArray.Length != 7)))
            {
                Console.Error.WriteLine("\nProcessCommandLineArguments failed to find the expected number of arguments. [3 or 7]");
                Environment.Exit(-1);
            }

            if (parsedArgs.Verbose)
            {
                Console.WriteLine(parsedArgs.Verbose);
                Console.Write("RegionArray Size: {0}\n   [", parsedArgs.regionArray.Length);
                for (int i = 0; i < parsedArgs.regionArray.Length; ++i)
                {
                    if (i == 0)
                        Console.Write(parsedArgs.regionArray[i]);
                    else
                        Console.Write(", {0}", parsedArgs.regionArray[i]);
                }
                Console.WriteLine("]");
            }

            return parsedArgs;
        }

        /// <summary>
        /// Show the help screen.
        /// </summary>
        private static void DisplayHelpScreen()
        {
            Console.WriteLine("Usage:  VennTool [options] <A B AB> or <A B C AB AC BC ABC>");
            Console.WriteLine("Where <A B AB> and <A B C AB AC BC ABC> are 3 or 7 values for regions in chart.");
            Console.WriteLine();
            Console.WriteLine("Description: VennTool generates a Venn Diagram from a region array.");
            Console.WriteLine();

            Console.WriteLine("Optional Parameters:");
            Console.WriteLine();
            Console.WriteLine("-xl:<string>   XL OutputFile");
            Console.WriteLine("-polar         Write result using polar coordinates");
            Console.WriteLine("-presort       PreSort .BED files prior to processing");
            Console.WriteLine("-v             Display Verbose output during processing");
            Console.WriteLine("-h             Print the help information.");
        }

        public static void Main(string[] args)
        {
            Splash();
            arguments = ProcessCommandLineArguments(args);
            VennDiagramData vdd = new VennDiagramData(arguments.regionArray);

            if (arguments.polar)
            {
                vdd.WritePolarVennDiagramData();
            }
            else
            {
                vdd.WriteVennDiagramData();
            }
            if (arguments.xl.Length > 0)
            {
                // produce an XL file with the 'right stuff'
                // Make sure we pass a complete filename path too.
                string filename = Path.GetFullPath(arguments.xl);
                Console.WriteLine("Produce Excel VennDiagram file: {0}", filename);
                VennToNodeXL.CreateVennDiagramNodeXLFile(filename, vdd);

            }
        }
    }
}
