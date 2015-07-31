using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BedStats.Properties;

using Bio;
using Bio.IO.Bed;
using Bio.Util.ArgumentParser;

using Tools.VennDiagram;

namespace BedStats
{
    internal class BedStats
    {
        private static BedStatsArguments arguments;
        private static bool verbose;
        private static bool createExcelWorkbook;
        private static bool createVennToolInputFile;
        private static string excelWorkbookFullPath;
        private static string vennToolInputFileFullPath;

        private static void Splash()
        {
            // Display the program's splash screene
            Console.WriteLine("\nBedStats V2.0 - Copyright (c) 2011-2014, The Outercurve Foundation.\n");
        }

        public static BedStatsArguments ProcessCommandLineArguments(string[] args)
        {
            var parsedArgs = new BedStatsArguments();
            var parser = new CommandLineArguments();

            // Add the parameters
            parser.Parameter(
                ArgumentType.DefaultArgument,
                "inputFiles",
                ArgumentValueType.MultipleUniqueStrings,
                string.Empty,
                "List of 2 or 3 input .BED files to process");
            parser.Parameter(
                ArgumentType.Optional,
                "xlFilename",
                ArgumentValueType.String,
                string.Empty,
                "Create an Excel file with BED stats");
            parser.Parameter(
                ArgumentType.Optional,
                "outputVennTool",
                ArgumentValueType.String,
                string.Empty,
                "Output file for use with VennTool");
            parser.Parameter(
                ArgumentType.Optional,
                "normalizeInputs",
                ArgumentValueType.Bool,
                string.Empty,
                "normalizeInput .BED files prior to processing");
            parser.Parameter(
                ArgumentType.Optional,
                "verbose",
                ArgumentValueType.Bool,
                string.Empty,
                "Display Verbose logging during processing");
            parser.Parameter(ArgumentType.Optional, "help", ArgumentValueType.Bool, string.Empty, "Displays the help");
            try
            {
                parser.Parse(args, parsedArgs);
            }
            catch (ArgumentParserException e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(Resource.BedStatsHelp);
                Environment.Exit(-1);
            }

            if (parsedArgs.Help)
            {
                Console.Error.WriteLine(Resource.BedStatsHelp);
                Environment.Exit(-1);
            }
            /*
             * Do any and all follow-up command line argument validation required
             */
            if ((parsedArgs.inputFiles == null) || (parsedArgs.inputFiles.Length < 2)
                || (parsedArgs.inputFiles.Length > 3))
            {
                Console.Error.WriteLine(
                    "\nProcessCommandLineArguments Failed to find expected number of file arguments. [2 or 3 required]");
                Environment.Exit(-1);
            }

            createExcelWorkbook = ((parsedArgs.xlFilename != null) && (parsedArgs.xlFilename.Count() != 0));
            if (createExcelWorkbook)
            {
                excelWorkbookFullPath = Path.GetFullPath(parsedArgs.xlFilename);
            }

            createVennToolInputFile = ((parsedArgs.outputVennTool != null) && (parsedArgs.outputVennTool.Count() != 0));
            if (createVennToolInputFile)
            {
                vennToolInputFileFullPath = Path.GetFullPath(parsedArgs.outputVennTool);
            }
            verbose = parsedArgs.verbose;
            return parsedArgs;
        }

        // 
        // default printing of List<ISequenceRange>
        //
        public static void ListSequenceRangeToString(IList<ISequenceRange> l)
        {
            // Display the first 10 rows if data (if they are there.)
            int cLinesToDisplay = Math.Min(l.Count, 10);

            for (int i = 0; i < cLinesToDisplay; ++i)
            {
                Console.WriteLine("{0}, {1}, {2}", l[i].ID, l[i].Start, l[i].End);
            }
            if (cLinesToDisplay < l.Count)
            {
                Console.Error.WriteLine("...");
            }
            Console.Error.WriteLine();
        }

        //
        // print 
        public static long SequenceRangeGroupingCBases(SequenceRangeGrouping srg)
        {
            var srgm = new SequenceRangeGroupingMetrics(srg);
            return (srgm.bases);
        }

        // default printing of SequenceRangeGrouping
        //
        public static void SequenceRangeGroupingToString(SequenceRangeGrouping srg, string name)
        {
            Console.Error.Write("[{0}] : SeqeuenceRangeGrouping: ", name);
            var srgm = new SequenceRangeGroupingMetrics(srg);
            Console.Error.WriteLine("{0}, {1}, {2}", srgm.groups, srgm.ranges, srgm.bases);

            foreach (string id in srg.GroupIDs)
            {
                Console.Error.WriteLine("--GroupID: {0}, {1}", id, srg.GetGroup(id).Count());
                ListSequenceRangeToString(srg.GetGroup(id));
            }
            Console.Error.WriteLine();
        }

        //
        // Read a Bed file into memory
        //
        public static SequenceRangeGrouping ReadBedFile(string filename)
        {
            var parser = new BedParser();
            IList<ISequenceRange> listSequenceRange = parser.ParseRange(filename);
            if (verbose)
            {
                //listSequenceRange.ToString();
                Console.Error.WriteLine("Processed File: {0}", filename);
                ListSequenceRangeToString(listSequenceRange);
            }

            var srg = new SequenceRangeGrouping(listSequenceRange);
            if (arguments.normalizeInputs)
            {
                srg.MergeOverlaps(); // could be called Normalize() or Cannonicalize()
            }
            return srg;
        }

        public static int Main(string[] args)
        {
            try
            {
                Splash();
                arguments = ProcessCommandLineArguments(args);

                if (arguments.inputFiles.Count() == 2)
                {
                    // now read the 2 BED files and do the operation to isolate each set
                    SequenceRangeGrouping srg1 = ReadBedFile(arguments.inputFiles[0]);
                    SequenceRangeGrouping srg2 = ReadBedFile(arguments.inputFiles[1]);

                    SequenceRangeGrouping srgOnly_A, srgOnly_B, srgOnly_AB;

                    VennToNodeXL.CreateSequenceRangeGroupingsForVennDiagram(
                        srg1,
                        srg2,
                        out srgOnly_A,
                        out srgOnly_B,
                        out srgOnly_AB);

                    var srgmOnly_A = new SequenceRangeGroupingMetrics(srgOnly_A);
                    var srgmOnly_B = new SequenceRangeGroupingMetrics(srgOnly_B);
                    var srgmOnly_AB = new SequenceRangeGroupingMetrics(srgOnly_AB);

                    if (createVennToolInputFile)
                    {
                        Console.Error.Write("\nWriting file [{0}]", vennToolInputFileFullPath);
                        var VennOutput = new StreamWriter(vennToolInputFileFullPath);
                        VennOutput.WriteLine("{0} {1} {2}", srgmOnly_A.bases, srgmOnly_B.bases, srgmOnly_AB.bases);
                        VennOutput.Close();
                        Console.Error.Write(" ... Done.");
                    }

                    if (createExcelWorkbook)
                    {
                        // create the Excel workbook with a NodeXL Venn diagram
                        Console.Error.Write("\nWriting file [{0}]", excelWorkbookFullPath);
                        var vdd = new VennDiagramData(srgmOnly_A.bases, srgmOnly_B.bases, srgmOnly_AB.bases);
                        try
                        {
                            VennToNodeXL.CreateVennDiagramNodeXLFile(excelWorkbookFullPath, vdd);
                            Console.Error.Write(" ... Done.\n");
                        }
                        catch (Exception e)
                        {
                            Console.Error.Write("Error:  Unable to create Excel workbook.");
                            DisplayException(e);
                            Environment.Exit(-1);
                        }
                    }
                    if (verbose)
                    {
                        Console.Error.Write("\nDump Sequence Groupings from two files\n");
                        SequenceRangeGroupingToString(srgOnly_A, "srgOnly_A");
                        SequenceRangeGroupingToString(srgOnly_B, "srgOnly_B");
                        SequenceRangeGroupingToString(srgOnly_AB, "srgOnly_AB");
                        Console.Error.Write("\nEnd Sequence group from twoe files\n");
                    }

                    Console.Write("\nOutput basepair count for each set");
                    Console.Write("\nGroupA,GroupB,GroupAB");
                    Console.Write("\n{0},{1},{2}\n", srgmOnly_A.bases, srgmOnly_B.bases, srgmOnly_AB.bases);
                }
                else if (arguments.inputFiles.Count() == 3)
                {
                    // TODO:  Reduce memory usage by re-using the SRGs after debugging is complete
                    //
                    // now read the 3 BED files and do the operation to isolate each set
                    SequenceRangeGrouping srg1 = ReadBedFile(arguments.inputFiles[0]);
                    SequenceRangeGrouping srg2 = ReadBedFile(arguments.inputFiles[1]);
                    SequenceRangeGrouping srg3 = ReadBedFile(arguments.inputFiles[2]);

                    SequenceRangeGrouping srgOnly_A,
                                          srgOnly_B,
                                          srgOnly_C,
                                          srgOnly_AB,
                                          srgOnly_AC,
                                          srgOnly_BC,
                                          srgOnly_ABC;

                    VennToNodeXL.CreateSequenceRangeGroupingsForVennDiagram(
                        srg1,
                        srg2,
                        srg3,
                        out srgOnly_A,
                        out srgOnly_B,
                        out srgOnly_C,
                        out srgOnly_AB,
                        out srgOnly_AC,
                        out srgOnly_BC,
                        out srgOnly_ABC);

                    /*
                     * We have the set information data for the three files.  
                     * Now what?
                     */
                    // generate the intersection Venn metrics
                    var srgmOnly_A = new SequenceRangeGroupingMetrics(srgOnly_A);
                    var srgmOnly_B = new SequenceRangeGroupingMetrics(srgOnly_B);
                    var srgmOnly_C = new SequenceRangeGroupingMetrics(srgOnly_C);
                    var srgmOnly_AB = new SequenceRangeGroupingMetrics(srgOnly_AB);
                    var srgmOnly_AC = new SequenceRangeGroupingMetrics(srgOnly_AC);
                    var srgmOnly_BC = new SequenceRangeGroupingMetrics(srgOnly_BC);
                    var srgmOnly_ABC = new SequenceRangeGroupingMetrics(srgOnly_ABC);

                    if (createVennToolInputFile)
                    {
                        Console.Error.Write("\nWriting file [{0}]", vennToolInputFileFullPath);
                        var VennOutput = new StreamWriter(vennToolInputFileFullPath);
                        VennOutput.WriteLine(
                            "{0} {1} {2} {3} {4} {5} {6}",
                            srgmOnly_A.bases,
                            srgmOnly_B.bases,
                            srgmOnly_C.bases,
                            srgmOnly_AB.bases,
                            srgmOnly_AC.bases,
                            srgmOnly_BC.bases,
                            srgmOnly_ABC.bases);
                        VennOutput.Close();
                        Console.Error.Write(" ... Done.");
                    }

                    if (createExcelWorkbook)
                    {
                        // create the NodeXL Venn diagram filefile
                        var vdd = new VennDiagramData(
                            srgmOnly_A.bases,
                            srgmOnly_B.bases,
                            srgmOnly_C.bases,
                            srgmOnly_AB.bases,
                            srgmOnly_AC.bases,
                            srgmOnly_BC.bases,
                            srgmOnly_ABC.bases);
                        // create the Excel workbook with a NodeXL Venn diagram
                        Console.Error.Write("\nWriting file [{0}]", excelWorkbookFullPath);
                        try
                        {
                            VennToNodeXL.CreateVennDiagramNodeXLFile(excelWorkbookFullPath, vdd);
                            Console.Error.Write(" ... Done.\n");
                        }
                        catch (Exception e)
                        {
                            Console.Error.Write("\nError:  Unable to create Excel workbook.");
                            DisplayException(e);
                            Environment.Exit(-1);
                        }
                    }
                    if (verbose)
                    {
                        Console.Error.Write("\nDump Sequence Groupings from three files\n");
                        SequenceRangeGroupingToString(srgOnly_A, "srgOnly_A");
                        SequenceRangeGroupingToString(srgOnly_B, "srgOnly_B");
                        SequenceRangeGroupingToString(srgOnly_C, "srgOnly_C");
                        SequenceRangeGroupingToString(srgOnly_AB, "srgOnly_AB");
                        SequenceRangeGroupingToString(srgOnly_AC, "srgOnly_AC");
                        SequenceRangeGroupingToString(srgOnly_BC, "srgOnly_BC");
                        SequenceRangeGroupingToString(srgOnly_ABC, "srgOnly_ABC");
                        Console.Error.Write("\nEnd Sequence group from three files\n");
                    }

                    Console.Write("\nOutput basepair count for each set");
                    Console.Write("\nGroupA,GroupB,GroupC,GroupAB,GroupAC,GroupBC,GroupABC");
                    Console.Write(
                        "\n{0},{1},{2},{3},{4},{5},{6}\n",
                        srgmOnly_A.bases,
                        srgmOnly_B.bases,
                        srgmOnly_C.bases,
                        srgmOnly_AB.bases,
                        srgmOnly_AC.bases,
                        srgmOnly_BC.bases,
                        srgmOnly_ABC.bases);
                }
            }
            catch (Exception ex)
            {
                DisplayException(ex);
            }
            return 0;
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