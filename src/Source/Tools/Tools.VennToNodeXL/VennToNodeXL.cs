using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Bio;
using Tools.VennDiagram.Properties;
using Microsoft.Office.Interop.Excel;
using Microsoft.Research.CommunityTechnologies.AppLib;

namespace Tools.VennDiagram
{
    /// <summary>
    /// This class takes care of processing the SequenceRangeGroupings and create venn diagram.
    /// </summary>
    public class VennToNodeXL
    {
        /// <summary>
        /// BED headers to be displayed on top of bed data
        /// </summary>
        private static string[] BedHeaders = { "Chromosome", "Start", "Stop", "Length" };

        /// <summary>
        /// BED headers to be displayed on top of bed data of result sheet for 2 input files.
        /// </summary>
        private static string[] BedHeadersForTwoInput = { "Chromosome", "Start", "Stop", "Length", "Length", "Length" };

        /// <summary>
        /// BED headers to be displayed on top of bed data of result sheet for 3 input files.
        /// </summary>
        private static string[] BedHeadersForThreeInput = { "Chromosome", "Start", "Stop", "Length", "Length", "Length", "Length", "Length", "Length", "Length" };

        /// <summary>
        /// Holds count fomula used to count number of sequence ranges.
        /// </summary>
        private const string CountFormulaFormat = "=COUNT({0}{1}:{2}{3})";

        /// <summary>
        /// Holds SUM formula used to get sum length of sequence ranges.
        /// </summary>
        private const string SumFormulaFormat = "=SUM({0}{1}:{2}{3})";

        //*************************************************************************
        //  Method: CreateVennDiagramNodeXLFile()
        //
        /// <summary>
        /// Demonstrates how the NodeXL template can be used to display a Venn
        /// diagram.
        /// </summary>
        ///
        /// <remarks>
        /// Requirements:
        ///
        /// <para>
        /// Excel 2007.
        /// </para>
        ///
        /// <para>
        /// The NodeXL Excel 2007 Template must be installed, and the version must
        /// be 1.0.1.105 or above.  This is available at
        /// http://nodexl.codeplex.com/Release/ProjectReleases.aspx.
        /// </para>
        ///
        /// <para>
        /// NodeXL, Graph, Layout must be set to Polar Absolute in the Excel
        /// ribbon.  NodeXL provided a new sheet to drive this during the
        /// period of initial visibility.  You must have everything setup
        /// properly before setting the worksheet to visible.
        /// </para>
        ///
        /// </remarks>
        //*************************************************************************
        public static void CreateVennDiagramNodeXLFile
            (
            String filename,
            VennDiagramData vdd
            )
        {
            // Create a workbook instance of the NodeXL template
            //   Populate it iwht the Venn.
            // Dev10 errors on the original (next) line.  replaced with the line below.
            //Application oApplication = new ApplicationClass();
            Application oApplication = (Application)Activator.CreateInstance(Type.GetTypeFromProgID("Excel.Application"));
            Workbook oWorkbook = CreateNodeXLVennDiagramWorkbook(oApplication, vdd);
            oWorkbook.SaveAs(filename, XlFileFormat.xlWorkbookDefault, null, null, false, false, XlSaveAsAccessMode.xlExclusive, null, true, null, null, null);
            oApplication.Quit();
        }

        /// <summary>
        /// Sets up the NodeXL workbook
        /// </summary>
        /// <param name="oWorkbook">NodeXL workbook instance</param>
        /// <param name="vdd">VennDiagramData object</param>
        /// <returns>Nodexl workbook</returns>
        public static Workbook
        CreateNodeXLVennDiagramWorkbook
            (
            Application oApplication,
            VennDiagramData vdd
            )
        {
            String sTemplatePath;

            if (!TryGetTemplatePath(oApplication, out sTemplatePath))
            {
                OnUnexpectedCondition(Properties.Resources.NodeXLNotInstalled);
            }

            Workbook oWorkbook = oApplication.Workbooks.Add(sTemplatePath);

            // Tell NodeXL to start up in the "Polar Absolute" mode 
            //   and auto display on startup
            ListObject oPerWorkbookSettingsTable;

            if (!ExcelUtil.TryGetTable(oWorkbook, "Misc", "PerWorkbookSettings",
                out oPerWorkbookSettingsTable))
            {
                OnUnexpectedCondition(Properties.Resources.PerWorkbookTableMissing);
            }

            SetLayoutAndReadWorkbook(oPerWorkbookSettingsTable);

            // The workbook consists of multiple worksheets, but for a Venn diagram
            // with no edges, only the vertex worksheet needs to be filled in.
            // Get the vertex table on the vertex worksheet.
            ListObject oVertexTable;

            if (!ExcelUtil.TryGetTable(oWorkbook, "Vertices", "Vertices",
                out oVertexTable))
            {
                OnUnexpectedCondition(Properties.Resources.VertexTableMissing);
            }

            // make Vertex tab active
            Worksheet oWorksheet;
            ExcelUtil.TryGetWorksheet(oWorkbook, "Vertices", out oWorksheet);

            // See this posting for an explanation of the strange cast:
            //   http://blogs.officezealot.com/maarten/archive/2006/01/02/8918.aspx
            ((_Worksheet)oWorksheet).Activate();

            AddVennVertices(oVertexTable, vdd);
            //AddAlternativeVertexLabels(oVertexTable);     // when we need to implement out a different labeling scheme...
            return oWorkbook;
        }

        /// <summary>
        /// Calculates overlaps of the given regions
        /// </summary>
        /// <param name="srgA">Group A</param>
        /// <param name="srgB">Group B</param>
        /// <param name="srgOnly_A">Out parameter : Only A</param>
        /// <param name="srgOnly_B">Out parameter : Only B</param>
        /// <param name="srgOnly_AB">Out parameter : Only C</param>
        public static void
        CreateSequenceRangeGroupingsForVennDiagram
            (
            SequenceRangeGrouping srgA,
            SequenceRangeGrouping srgB,
            out SequenceRangeGrouping srgOnly_A,
            out SequenceRangeGrouping srgOnly_B,
            out SequenceRangeGrouping srgOnly_AB
            )
        {
            // Create the proper intersected sets from the two original SequenceRangeGroups
            SequenceRangeGrouping srgAB = srgA.MergeOverlaps(srgB); // use set terminology (Union) or boolean logic (Or)
            srgOnly_A = srgAB.Subtract(srgB, 1, SubtractOutputType.NonOverlappingPiecesOfIntervals); // TODO: Subtract and Intersect should use same 'logic' (for bool 3rd arg)
            srgOnly_B = srgAB.Subtract(srgA, 1, SubtractOutputType.NonOverlappingPiecesOfIntervals);
            srgOnly_AB = srgA.Intersect(srgB, 1, IntersectOutputType.OverlappingPiecesOfIntervals);
        }

        /// <summary>
        /// Calculates overlaps of the given regions
        /// </summary>
        /// <param name="srgA">Group A</param>
        /// <param name="srgB">Group B</param>
        /// <param name="srgC">Group C</param>
        /// <param name="srgOnly_A">Out parameter : Only A</param>
        /// <param name="srgOnly_B">Out parameter : Only B</param>
        /// <param name="srgOnly_C">Out parameter : Only C</param>
        /// <param name="srgOnly_AB">Out parameter : Only AB</param>
        /// <param name="srgOnly_AC">Out parameter : Only AC</param>
        /// <param name="srgOnly_BC">Out parameter : Only BC</param>
        /// <param name="srgOnly_ABC">Out parameter : Only ABC</param>
        public static void
        CreateSequenceRangeGroupingsForVennDiagram
            (
            SequenceRangeGrouping srgA,
            SequenceRangeGrouping srgB,
            SequenceRangeGrouping srgC,
            out SequenceRangeGrouping srgOnly_A,
            out SequenceRangeGrouping srgOnly_B,
            out SequenceRangeGrouping srgOnly_C,
            out SequenceRangeGrouping srgOnly_AB,
            out SequenceRangeGrouping srgOnly_AC,
            out SequenceRangeGrouping srgOnly_BC,
            out SequenceRangeGrouping srgOnly_ABC
            )
        {
            // create the proper sets for a 3 circle Venn Diagram
            SequenceRangeGrouping srgBC = srgB.MergeOverlaps(srgC);
            srgOnly_A = srgA.Subtract(srgBC, 1, SubtractOutputType.NonOverlappingPiecesOfIntervals);

            SequenceRangeGrouping srgAC = srgA.MergeOverlaps(srgC);
            srgOnly_B = srgB.Subtract(srgAC, 1, SubtractOutputType.NonOverlappingPiecesOfIntervals);

            SequenceRangeGrouping srgAB = srgA.MergeOverlaps(srgB);
            srgOnly_C = srgC.Subtract(srgAB, 1, SubtractOutputType.NonOverlappingPiecesOfIntervals);

            srgAB = srgA.Intersect(srgB, 1, IntersectOutputType.OverlappingPiecesOfIntervals);
            srgOnly_AB = srgAB.Subtract(srgC, 1, SubtractOutputType.NonOverlappingPiecesOfIntervals);
            srgAC = srgA.Intersect(srgC, 1, IntersectOutputType.OverlappingPiecesOfIntervals);
            srgOnly_AC = srgAC.Subtract(srgB, 1, SubtractOutputType.NonOverlappingPiecesOfIntervals);
            srgBC = srgB.Intersect(srgC, 1, IntersectOutputType.OverlappingPiecesOfIntervals);
            srgOnly_BC = srgBC.Subtract(srgA, 1, SubtractOutputType.NonOverlappingPiecesOfIntervals);

            srgOnly_ABC = srgAB.Intersect(srgC, 1, IntersectOutputType.OverlappingPiecesOfIntervals);
        }

        /// <summary>
        /// Creates venn diagram by processing the overlaps of the two given regions
        /// </summary>
        /// <param name="oApplication">Excel application object</param>
        /// <param name="srgA">SequenceRangeGrouping for group A</param>
        /// <param name="srgB">SequenceRangeGrouping for group B</param>
        /// <returns>NodeXL workbook</returns>
        public static Workbook
        CreateNodeXLVennDiagramWorkbookFromSequenceRangeGroupings
            (
            Microsoft.Office.Interop.Excel.Application oApplication,
            SequenceRangeGrouping srgA,
            SequenceRangeGrouping srgB
            )
        {
            // create the proper sets for VennDiagram
            SequenceRangeGrouping srgOnly_A, srgOnly_B, srgOnly_AB;
            CreateSequenceRangeGroupingsForVennDiagram(srgA, srgB, out srgOnly_A, out srgOnly_B, out srgOnly_AB);

            SequenceRangeGroupingMetrics srgmOnly_A = new SequenceRangeGroupingMetrics(srgOnly_A);
            SequenceRangeGroupingMetrics srgmOnly_B = new SequenceRangeGroupingMetrics(srgOnly_B);
            SequenceRangeGroupingMetrics srgmOnly_AB = new SequenceRangeGroupingMetrics(srgOnly_AB);

            VennDiagramData vdd = new VennDiagramData(srgmOnly_A.bases
                , srgmOnly_B.bases
                , srgmOnly_AB.bases);

            Workbook oWorkbook = CreateNodeXLVennDiagramWorkbook(oApplication, vdd);

            // write source data to workbook
            DisplaySourceData(srgA, Resources.A, oWorkbook);
            DisplaySourceData(srgB, Resources.B, oWorkbook);

            // Write overlap data to a sheet
            Worksheet outputSheet = oWorkbook.Sheets.Add(Type.Missing, oWorkbook.Sheets[oWorkbook.Sheets.Count], 1, XlSheetType.xlWorksheet);
            outputSheet.Name = Resources.OverlapsSheetName;
            WriteOverlapData(outputSheet, srgOnly_A, srgOnly_B, srgOnly_AB);

            oApplication.Visible = true;
            return oWorkbook;
        }

        /// <summary>
        /// Creates venn diagram by processing the overlaps of the three given regions
        /// </summary>
        /// <param name="oApplication">Excel application object</param>
        /// <param name="srgA">SequenceRangeGrouping for group A</param>
        /// <param name="srgB">SequenceRangeGrouping for group B</param>
        /// <param name="srgC">SequenceRangeGrouping for group C</param>
        /// <returns>NodeXL workbook</returns>
        public static Workbook
        CreateNodeXLVennDiagramWorkbookFromSequenceRangeGroupings
            (
            Microsoft.Office.Interop.Excel.Application oApplication,
            SequenceRangeGrouping srgA,
            SequenceRangeGrouping srgB,
            SequenceRangeGrouping srgC
            )
        {
            SequenceRangeGrouping srgOnly_A, srgOnly_B, srgOnly_C, srgOnly_AB, srgOnly_AC, srgOnly_BC, srgOnly_ABC;
            CreateSequenceRangeGroupingsForVennDiagram(srgA
                , srgB
                , srgC
                , out srgOnly_A
                , out srgOnly_B
                , out srgOnly_C
                , out srgOnly_AB
                , out srgOnly_AC
                , out srgOnly_BC
                , out srgOnly_ABC);

            // generate the intersection Venn metrics
            SequenceRangeGroupingMetrics srgmOnly_A = new SequenceRangeGroupingMetrics(srgOnly_A);
            SequenceRangeGroupingMetrics srgmOnly_B = new SequenceRangeGroupingMetrics(srgOnly_B);
            SequenceRangeGroupingMetrics srgmOnly_C = new SequenceRangeGroupingMetrics(srgOnly_C);
            SequenceRangeGroupingMetrics srgmOnly_AB = new SequenceRangeGroupingMetrics(srgOnly_AB);
            SequenceRangeGroupingMetrics srgmOnly_AC = new SequenceRangeGroupingMetrics(srgOnly_AC);
            SequenceRangeGroupingMetrics srgmOnly_BC = new SequenceRangeGroupingMetrics(srgOnly_BC);
            SequenceRangeGroupingMetrics srgmOnly_ABC = new SequenceRangeGroupingMetrics(srgOnly_ABC);

            // create the NodeXL Venn diagram filefile
            VennDiagramData vdd = new VennDiagramData(srgmOnly_A.bases
                , srgmOnly_B.bases
                , srgmOnly_C.bases
                , srgmOnly_AB.bases
                , srgmOnly_AC.bases
                , srgmOnly_BC.bases
                , srgmOnly_ABC.bases);

            // To ensure NodeXL displays the diagram, DONOT make the application 
            // visible or update the screen until the parameters are all set up.
            oApplication.ScreenUpdating = false;
            Workbook oWorkbook = CreateNodeXLVennDiagramWorkbook(oApplication, vdd);

            // write source data to workbook
            DisplaySourceData(srgA, Resources.A, oWorkbook);
            DisplaySourceData(srgB, Resources.B, oWorkbook);
            DisplaySourceData(srgC, Resources.C, oWorkbook);

            // Write overlap data to a sheet
            Worksheet outputSheet = oWorkbook.Sheets.Add(Type.Missing, oWorkbook.Sheets[oWorkbook.Sheets.Count], 1, XlSheetType.xlWorksheet);
            outputSheet.Name = Resources.OverlapsSheetName;
            WriteOverlapData(outputSheet, srgOnly_A, srgOnly_B, srgOnly_C, srgOnly_AB, srgOnly_AC, srgOnly_BC, srgOnly_ABC);

            oApplication.ScreenUpdating = true;
            oApplication.Visible = true;
            return oWorkbook;
        }

        /// <summary>
        /// This method will dump the region overlap data to a sheet in the NodeXL workbook.
        /// </summary>
        /// <param name="outputSheet">Sheet to which the data should be written</param>
        /// <param name="srgOnly_A">Only A</param>
        /// <param name="srgOnly_B">Only B</param>
        /// <param name="srgOnly_C">Only C</param>
        /// <param name="srgOnly_AB">Only AB</param>
        /// <param name="srgOnly_AC">Only AC</param>
        /// <param name="srgOnly_BC">Only BC</param>
        /// <param name="srgOnly_ABC">Only ABC</param>
        /// <returns>NodeXL sheet</returns>
        private static Worksheet WriteOverlapData(
            Worksheet outputSheet,
            SequenceRangeGrouping srgOnly_A,
            SequenceRangeGrouping srgOnly_B,
            SequenceRangeGrouping srgOnly_C,
            SequenceRangeGrouping srgOnly_AB,
            SequenceRangeGrouping srgOnly_AC,
            SequenceRangeGrouping srgOnly_BC,
            SequenceRangeGrouping srgOnly_ABC
            )
        {
            outputSheet.Application.ScreenUpdating = false;
            outputSheet.Application.EnableEvents = false;
            List<ISequenceRange> seqs_AList = null;
            List<ISequenceRange> seqs_BList = null;
            List<ISequenceRange> seqs_ABList = null;
            List<ISequenceRange> seqs_CList = null;
            List<ISequenceRange> seqs_ACList = null;
            List<ISequenceRange> seqs_BCList = null;
            List<ISequenceRange> seqs_ABCList = null;

            int dataStartRow = 7;
            int currentRow;

            try
            {
                // write the header
                outputSheet.Range["A2"].Value2 = Resources.RegionsCountLabel;
                outputSheet.Range["A3"].Value2 = Resources.BasePairsCountLabel;
                outputSheet.Range["A4"].Value2 = Resources.PerOfBasePairsInRegionLabel;

                outputSheet.Range["D1"].Value2 = Resources.OnlyA;
                outputSheet.Range["E1"].Value2 = Resources.OnlyB;
                outputSheet.Range["F1"].Value2 = Resources.OnlyC;
                outputSheet.Range["G1"].Value2 = Resources.OnlyAB;
                outputSheet.Range["H1"].Value2 = Resources.OnlyAC;
                outputSheet.Range["I1"].Value2 = Resources.OnlyBC;
                outputSheet.Range["J1"].Value2 = Resources.OnlyABC;
                outputSheet.Range["K1"].Value2 = Resources.Total;

                outputSheet.Range["A6", "J6"].Value2 = BedHeadersForThreeInput;
                outputSheet.Range["A6", "J6"].Cells.Borders[XlBordersIndex.xlEdgeBottom].Weight = XlBorderWeight.xlThin;

                outputSheet.Range["A1", "J1"].Cells.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                outputSheet.Range["A6", "J6"].Cells.HorizontalAlignment = XlHAlign.xlHAlignCenter;

                currentRow = dataStartRow;

                // Get all groupID's in all ranges
                List<string> groupIDs = new List<string>();
                groupIDs.AddRange(srgOnly_A.GroupIDs);
                groupIDs.AddRange(srgOnly_B.GroupIDs);
                groupIDs.AddRange(srgOnly_C.GroupIDs);
                groupIDs.AddRange(srgOnly_AB.GroupIDs);
                groupIDs.AddRange(srgOnly_AC.GroupIDs);
                groupIDs.AddRange(srgOnly_BC.GroupIDs);
                groupIDs.AddRange(srgOnly_ABC.GroupIDs);
                int prevRow = dataStartRow;
                // Display all overlap data
                foreach (string groupID in groupIDs.Distinct())
                {
                    seqs_AList = srgOnly_A.GetGroup(groupID);
                    seqs_BList = srgOnly_B.GetGroup(groupID);
                    seqs_CList = srgOnly_C.GetGroup(groupID);
                    seqs_ABList = srgOnly_AB.GetGroup(groupID);
                    seqs_ACList = srgOnly_AC.GetGroup(groupID);
                    seqs_BCList = srgOnly_BC.GetGroup(groupID);
                    seqs_ABCList = srgOnly_ABC.GetGroup(groupID);

                    if (seqs_AList == null) seqs_AList = new List<ISequenceRange>();
                    if (seqs_BList == null) seqs_BList = new List<ISequenceRange>();
                    if (seqs_CList == null) seqs_CList = new List<ISequenceRange>();
                    if (seqs_ABList == null) seqs_ABList = new List<ISequenceRange>();
                    if (seqs_ACList == null) seqs_ACList = new List<ISequenceRange>();
                    if (seqs_BCList == null) seqs_BCList = new List<ISequenceRange>();
                    if (seqs_ABCList == null) seqs_ABCList = new List<ISequenceRange>();


                    int indexA = 0;
                    int indexB = 0;
                    int indexC = 0;
                    int indexAB = 0;
                    int indexAC = 0;
                    int indexBC = 0;
                    int indexABC = 0;

                    int totalCount = seqs_AList.Count + seqs_BList.Count + seqs_CList.Count + seqs_ABList.Count + seqs_ACList.Count + seqs_BCList.Count + +seqs_ABCList.Count;
                    object[,] output = new object[totalCount, 10];

                    while (indexA < seqs_AList.Count || indexB < seqs_BList.Count || indexC < seqs_CList.Count ||
                           indexAB < seqs_ABList.Count || indexAC < seqs_ACList.Count || indexBC < seqs_BCList.Count || indexABC < seqs_ABCList.Count)
                    {
                        ISequenceRange a = indexA < seqs_AList.Count ? seqs_AList[indexA] : null;
                        ISequenceRange b = indexB < seqs_BList.Count ? seqs_BList[indexB] : null;
                        ISequenceRange c = indexC < seqs_CList.Count ? seqs_CList[indexC] : null;
                        ISequenceRange ab = indexAB < seqs_ABList.Count ? seqs_ABList[indexAB] : null;
                        ISequenceRange ac = indexAC < seqs_ACList.Count ? seqs_ACList[indexAC] : null;
                        ISequenceRange bc = indexBC < seqs_BCList.Count ? seqs_BCList[indexBC] : null;
                        ISequenceRange abc = indexABC < seqs_ABCList.Count ? seqs_ABCList[indexABC] : null;

                        ISequenceRange range = GetSmallestSeqRangeToDisplay(a, b, c, ab, ac, bc, abc);

                        if (range == null)
                        {
                            indexA++;
                            indexB++;
                            indexC++;
                            indexAB++;
                            indexAC++;
                            indexBC++;
                            indexABC++;

                            continue;
                        }

                        output[currentRow - prevRow, 0] = range.ID;
                        output[currentRow - prevRow, 1] = range.Start;
                        output[currentRow - prevRow, 2] = range.End;

                        if (range == a)
                        {
                            output[currentRow - prevRow, 3] = Math.Abs(range.End - range.Start);
                            indexA++;
                        }
                        else if (range == b)
                        {
                            output[currentRow - prevRow, 4] = Math.Abs(range.End - range.Start);
                            indexB++;
                        }
                        else if (range == c)
                        {
                            output[currentRow - prevRow, 5] = Math.Abs(range.End - range.Start);
                            indexC++;
                        }
                        else if (range == ab)
                        {
                            output[currentRow - prevRow, 6] = Math.Abs(range.End - range.Start);
                            indexAB++;
                        }
                        else if (range == ac)
                        {
                            output[currentRow - prevRow, 7] = Math.Abs(range.End - range.Start);
                            indexAC++;
                        }
                        else if (range == bc)
                        {
                            output[currentRow - prevRow, 8] = Math.Abs(range.End - range.Start);
                            indexBC++;
                        }
                        else 
                        {
                            // display
                            output[currentRow - prevRow, 9] = Math.Abs(range.End - range.Start);
                            indexABC++;
                        }

                        currentRow++;
                    }

                    outputSheet.Range["A" + prevRow.ToString(), "J" + (currentRow -1).ToString()].Value2 = output;
                    prevRow = currentRow;
                }

                string column = "D";
                string formula = string.Format(CultureInfo.InvariantCulture, CountFormulaFormat, column, dataStartRow, column, currentRow - 1);
                outputSheet.Range[column + "2"].Formula = formula;
                column = "E";
                formula = string.Format(CultureInfo.InvariantCulture, CountFormulaFormat, column, dataStartRow, column, currentRow - 1);
                outputSheet.Range[column + "2"].Formula = formula;
                column = "F";
                formula = string.Format(CultureInfo.InvariantCulture, CountFormulaFormat, column, dataStartRow, column, currentRow - 1);
                outputSheet.Range[column + "2"].Formula = formula;
                column = "G";
                formula = string.Format(CultureInfo.InvariantCulture, CountFormulaFormat, column, dataStartRow, column, currentRow - 1);
                outputSheet.Range[column + "2"].Formula = formula;
                column = "H";
                formula = string.Format(CultureInfo.InvariantCulture, CountFormulaFormat, column, dataStartRow, column, currentRow - 1);
                outputSheet.Range[column + "2"].Formula = formula;
                column = "I";
                formula = string.Format(CultureInfo.InvariantCulture, CountFormulaFormat, column, dataStartRow, column, currentRow - 1);
                outputSheet.Range[column + "2"].Formula = formula;
                column = "J";
                formula = string.Format(CultureInfo.InvariantCulture, CountFormulaFormat, column, dataStartRow, column, currentRow - 1);
                outputSheet.Range[column + "2"].Formula = formula;

                formula = string.Format(CultureInfo.InvariantCulture, SumFormulaFormat, "D", 2, "J", 2);
                outputSheet.Range["K2"].Formula = formula;

                column = "D";
                formula = string.Format(CultureInfo.InvariantCulture, SumFormulaFormat, column, dataStartRow, column, currentRow - 1);
                outputSheet.Range[column + "3"].Formula = formula;
                column = "E";
                formula = string.Format(CultureInfo.InvariantCulture, SumFormulaFormat, column, dataStartRow, column, currentRow - 1);
                outputSheet.Range[column + "3"].Formula = formula;
                column = "F";
                formula = string.Format(CultureInfo.InvariantCulture, SumFormulaFormat, column, dataStartRow, column, currentRow - 1);
                outputSheet.Range[column + "3"].Formula = formula;
                column = "G";
                formula = string.Format(CultureInfo.InvariantCulture, SumFormulaFormat, column, dataStartRow, column, currentRow - 1);
                outputSheet.Range[column + "3"].Formula = formula;
                column = "H";
                formula = string.Format(CultureInfo.InvariantCulture, SumFormulaFormat, column, dataStartRow, column, currentRow - 1);
                outputSheet.Range[column + "3"].Formula = formula;
                column = "I";
                formula = string.Format(CultureInfo.InvariantCulture, SumFormulaFormat, column, dataStartRow, column, currentRow - 1);
                outputSheet.Range[column + "3"].Formula = formula;
                column = "J";
                formula = string.Format(CultureInfo.InvariantCulture, SumFormulaFormat, column, dataStartRow, column, currentRow - 1);
                outputSheet.Range[column + "3"].Formula = formula;

                formula = string.Format(CultureInfo.InvariantCulture, SumFormulaFormat, "D", 3, "J", 3);
                outputSheet.Range["K3"].Formula = formula;


                outputSheet.Range["D4"].Formula = "=D3/K3";
                outputSheet.Range["E4"].Formula = "=E3/K3";
                outputSheet.Range["F4"].Formula = "=F3/K3";
                outputSheet.Range["G4"].Formula = "=G3/K3";
                outputSheet.Range["H4"].Formula = "=H3/K3";
                outputSheet.Range["I4"].Formula = "=I3/K3";
                outputSheet.Range["J4"].Formula = "=J3/K3";


                formula = string.Format(CultureInfo.InvariantCulture, SumFormulaFormat, "D", 4, "J", 4);
                outputSheet.Range["K4"].Formula = formula;
                outputSheet.Range["D4", "K4"].Cells.NumberFormat = "0.00%";

                outputSheet.UsedRange.Columns.AutoFit();
            }
            finally
            {
                outputSheet.Application.ScreenUpdating = true;
                outputSheet.Application.EnableEvents = true;
            }

            return outputSheet;
        }

        /// <summary>
        /// This method will dump the region overlap data to a sheet in the NodeXL workbook.
        /// </summary>
        /// <param name="outputSheet">Sheet to which the data should be dumped to</param>
        /// <param name="srgOnly_A">Only A</param>
        /// <param name="srgOnly_B">Only A</param>
        /// <param name="srgOnly_AB">Only A</param>
        /// <returns>Worksheet after writing data</returns>
        private static Worksheet WriteOverlapData(Worksheet outputSheet, SequenceRangeGrouping srgOnly_A, SequenceRangeGrouping srgOnly_B, SequenceRangeGrouping srgOnly_AB)
        {
            outputSheet.Application.ScreenUpdating = false;
            outputSheet.Application.EnableEvents = false;
            List<ISequenceRange> seqs_AList = null;
            List<ISequenceRange> seqs_BList = null;
            List<ISequenceRange> seqs_ABList = null;
            int dataStartRow = 7;
            int currentRow;
            try
            {
                // write the header
                outputSheet.Range["A2"].Value2 = Resources.RegionsCountLabel;
                outputSheet.Range["A3"].Value2 = Resources.BasePairsCountLabel;
                outputSheet.Range["A4"].Value2 = Resources.PerOfBasePairsInRegionLabel;
                outputSheet.Range["D1"].Value2 = Resources.OnlyA;
                outputSheet.Range["E1"].Value2 = Resources.OnlyB;
                outputSheet.Range["F1"].Value2 = Resources.OnlyAB;
                outputSheet.Range["G1"].Value2 = Resources.Total;

                outputSheet.Range["A6", "F6"].Value2 = BedHeadersForTwoInput;
                outputSheet.Range["A6", "F6"].Cells.Borders[XlBordersIndex.xlEdgeBottom].Weight = XlBorderWeight.xlThin;
                outputSheet.Range["A1", "F1"].Cells.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                outputSheet.Range["A6", "F6"].Cells.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                currentRow = dataStartRow;

                // Get all groupID's in all ranges
                List<string> groupIDs = new List<string>();
                groupIDs.AddRange(srgOnly_A.GroupIDs);
                groupIDs.AddRange(srgOnly_B.GroupIDs);
                groupIDs.AddRange(srgOnly_AB.GroupIDs);

                int prevRow = dataStartRow;

                // Display all overlap data
                foreach (string groupID in groupIDs.Distinct())
                {
                    seqs_AList = srgOnly_A.GetGroup(groupID);
                    seqs_BList = srgOnly_B.GetGroup(groupID);
                    seqs_ABList = srgOnly_AB.GetGroup(groupID);

                    if (seqs_AList == null) seqs_AList = new List<ISequenceRange>();
                    if (seqs_BList == null) seqs_BList = new List<ISequenceRange>();
                    if (seqs_ABList == null) seqs_ABList = new List<ISequenceRange>();

                    int indexA = 0;
                    int indexB = 0;
                    int indexAB = 0;

                    object[,] output = new object[seqs_AList.Count + seqs_BList.Count + seqs_ABList.Count, 6];

                    while (indexA < seqs_AList.Count || indexB < seqs_BList.Count || indexAB < seqs_ABList.Count)
                    {
                        ISequenceRange a = indexA < seqs_AList.Count ? seqs_AList[indexA] : null;
                        ISequenceRange b = indexB < seqs_BList.Count ? seqs_BList[indexB] : null;
                        ISequenceRange ab = indexAB < seqs_ABList.Count ? seqs_ABList[indexAB] : null;

                        ISequenceRange range = GetSmallestSeqRangeToDisplay(a, b, ab);

                        if (range == null)
                        {
                            indexA++;
                            indexB++;
                            indexAB++;

                            continue;
                        }

                        output[currentRow - prevRow, 0] = range.ID;
                        output[currentRow - prevRow, 1] = range.Start;
                        output[currentRow - prevRow, 2] = range.End;

                        if (range == a)
                        {
                            // display
                            output[currentRow - prevRow, 3] = Math.Abs(range.End - range.Start);
                            indexA++;
                        }
                        else if (range == b)
                        {
                            // display
                            output[currentRow - prevRow, 4] = Math.Abs(range.End - range.Start);
                            indexB++;
                        }
                        else
                        {
                            // display
                            output[currentRow - prevRow, 5] = Math.Abs(range.End - range.Start);
                            indexAB++;
                        }

                        currentRow++;
                    }

                    outputSheet.Range["A" + prevRow.ToString(), "F" + (currentRow -1).ToString()].Value2 = output;
                    prevRow = currentRow;
                }

                string formula = string.Format(CultureInfo.InvariantCulture, CountFormulaFormat, "D", dataStartRow, "D", currentRow - 1);
                outputSheet.Range["D2"].Formula = formula;
                formula = string.Format(CultureInfo.InvariantCulture, CountFormulaFormat, "E", dataStartRow, "E", currentRow - 1);
                outputSheet.Range["E2"].Formula = formula;
                formula = string.Format(CultureInfo.InvariantCulture, CountFormulaFormat, "F", dataStartRow, "F", currentRow - 1);
                outputSheet.Range["F2"].Formula = formula;
                formula = string.Format(CultureInfo.InvariantCulture, SumFormulaFormat, "D", 2, "F", 2);
                outputSheet.Range["G2"].Formula = formula;

                formula = string.Format(CultureInfo.InvariantCulture, SumFormulaFormat, "D", dataStartRow, "D", currentRow - 1);
                outputSheet.Range["D3"].Formula = formula;
                formula = string.Format(CultureInfo.InvariantCulture, SumFormulaFormat, "E", dataStartRow, "E", currentRow - 1);
                outputSheet.Range["E3"].Formula = formula;
                formula = string.Format(CultureInfo.InvariantCulture, SumFormulaFormat, "F", dataStartRow, "F", currentRow - 1);
                outputSheet.Range["F3"].Formula = formula;
                formula = string.Format(CultureInfo.InvariantCulture, SumFormulaFormat, "D", 3, "F", 3);
                outputSheet.Range["G3"].Formula = formula;

                outputSheet.Range["D4"].Formula = "=D3/G3";
                outputSheet.Range["E4"].Formula = "=E3/G3";
                outputSheet.Range["F4"].Formula = "=F3/G3";
                formula = string.Format(CultureInfo.InvariantCulture, SumFormulaFormat, "D", 4, "F", 4);
                outputSheet.Range["G4"].Formula = formula;
                outputSheet.Range["D4", "G4"].Cells.NumberFormat = "0.00%";

                outputSheet.UsedRange.Columns.AutoFit();
            }
            finally
            {
                outputSheet.Application.ScreenUpdating = true;
                outputSheet.Application.EnableEvents = true;
            }

            return outputSheet;
        }

        /// <summary>
        /// Gets the smallest sequence range among the specified sequence ranges. 
        /// </summary>
        /// <param name="seqRanges">Sequence ranges</param>
        private static ISequenceRange GetSmallestSeqRangeToDisplay(params ISequenceRange[] seqRanges)
        {
            ISequenceRange seqRange = null;

            foreach (ISequenceRange range in seqRanges)
            {
                if (range == null) continue;
                if (seqRange == null)
                {
                    seqRange = range;
                    continue;
                }

                if (range.CompareTo(seqRange) < 0)
                    seqRange = range;
            }

            return seqRange;
        }

        /// <summary>
        /// Write source sequence ranges to the given workbook in a new sheet
        /// </summary>
        /// <param name="sourceSequenceRanges">Source sequence ranges</param>
        /// <param name="sheetName">Name to be given for the new sheet</param>
        /// <param name="targetWorkbook">Workbook in which the sheet has to be added</param>
        /// <returns>Newly created sheet with source data.</returns>
        private static Worksheet DisplaySourceData(SequenceRangeGrouping sourceSequenceRanges, string sheetName, Workbook targetWorkbook)
        {
            //create new sheet
            Worksheet outputSheet = targetWorkbook.Sheets.Add(Type.Missing, targetWorkbook.Sheets[targetWorkbook.Sheets.Count], 1, XlSheetType.xlWorksheet);
            outputSheet.Name = sheetName;

            targetWorkbook.Application.ScreenUpdating = false;
            targetWorkbook.Application.EnableEvents = false;

            try
            {
                // write the header
                outputSheet.Range["A1", "C1"].Value2 = BedHeaders;

                // write bed data
                int currentRow = 2;
                foreach (string groupID in sourceSequenceRanges.GroupIDs)
                {
                    Range outputRange = WriteSequenceRangeAt(outputSheet.Cells[currentRow, 1], sourceSequenceRanges.GetGroup(groupID), false);
                    currentRow += outputRange.Rows.Count;
                }

                outputSheet.UsedRange.Columns.AutoFit();
            }
            finally
            {
                targetWorkbook.Application.ScreenUpdating = true;
                targetWorkbook.Application.EnableEvents = true;
            }

            return outputSheet;
        }

        /// <summary>
        /// Write the given list of sequence ranges to a range
        /// </summary>
        /// <param name="outputStartRange">Starting cell of the output range</param>
        /// <param name="source">List of sequence range objects</param>
        /// <returns>Range with the data written</returns>
        private static Range WriteSequenceRangeAt(Range outputStartRange, List<ISequenceRange> source, bool writeLength)
        {
            object[,] output = new object[source.Count, writeLength ? 4 : 3];

            int currentRow = 0;
            foreach (ISequenceRange sequence in source)
            {
                output[currentRow, 0] = sequence.ID;
                output[currentRow, 1] = sequence.Start;
                output[currentRow, 2] = sequence.End;

                if (writeLength)
                {
                    output[currentRow, 3] = Math.Abs(sequence.End - sequence.Start).ToString();
                }

                currentRow++;
            }

            outputStartRange = outputStartRange.get_Resize(output.GetLength(0), output.GetLength(1));
            outputStartRange.Value2 = output;

            return outputStartRange;
        }

        /// <summary>
        /// Adds a vertex to the Vertex table of nodeXL
        /// </summary>
        /// <param name="vertexTable">Vertex table</param>
        /// <param name="vc">VennCircle object</param>
        /// <param name="idx">Zero based table row</param>
        /// <param name="columnName">Column Name</param>
        /// <param name="color">Circle color</param>
        private static void
        AddVennVertex
        (
            ListObject vertexTable,
            VennCircle vc,
            int idx,
            string columnName,
            string color
        )
        {
            string[] sLabelPosition = { "Middle Left", "Middle Right", "Bottom Center" };
            PolarCoordinate pc = new PolarCoordinate(vc.center);

            // Add vertex to table
            SetTableCellValue(vertexTable, idx, VertexColumnName, columnName);
            SetTableCellValue(vertexTable, idx, ColorColumnName, color);
            SetTableCellValue(vertexTable, idx, ShapeColumnName, "Disk");
            SetTableCellValue(vertexTable, idx, OpacityColumnName, 30);
            SetTableCellValue(vertexTable, idx, VisibilityColumnName, "Show");
            SetTableCellValue(vertexTable, idx, LabelColumnName, "Group " + columnName);
            SetTableCellValue(vertexTable, idx, LabelPositionColumnName, sLabelPosition[idx]);
            SetTableCellValue(vertexTable, idx, PolarRColumnName, pc.Rho);
            SetTableCellValue(vertexTable, idx, PolarAngleColumnName, (pc.Theta * (180.0 / Math.PI)));
            SetTableCellValue(vertexTable, idx, SizeColumnName, RadiusWpfToVertexSize(vc.radius));
        }

        //*************************************************************************
        //  Method: AddVennVertices()
        //
        /// <summary>
        /// Adds three vertices to the NodeXL workbook to create a Venn diagram.
        /// </summary>
        ///
        /// <param name="vertexTable">
        /// The vertex table in the NodeXL workbook.
        /// </param>
        //*************************************************************************
        private static void
        AddVennVertices
        (
            ListObject vertexTable,
            VennDiagramData vddOriginal
        )
        {
            Debug.Assert(vertexTable != null);

            VennDiagramData vdd = vddOriginal.CenterVennDiagramData();
            vdd.ScaleTo(150);

            // Add two or three vertices.  Name them via the Vertex column.
            AddVennVertex(vertexTable, vdd.CircleA, 0, "A", "Red");
            AddVennVertex(vertexTable, vdd.CircleB, 1, "B", "Green");

            if (vdd.vennType == VennDiagramData.VennTypes.ThreeCircle)
            {
                AddVennVertex(vertexTable, vdd.CircleC, 2, "C", "Blue");
            }
        }

        //*************************************************************************
        //  Method: AddAlternativeVertexLabels()
        //
        /// <summary>
        /// Demonstrates alternative ways to label the Venn vertices.
        /// </summary>
        ///
        /// <param name="vertexTable">
        /// The vertex table in the NodeXL workbook.
        /// </param>
        //*************************************************************************

        private static void
        AddAlternativeVertexLabels
        (
            ListObject vertexTable
        )
        {
            Debug.Assert(vertexTable != null);

            // AddVennVertices() annotated the Venn vertices by setting the Label
            // and Label Position columns.  Because these annotations get drawn
            // with the vertices' specified Color and Opacity values, it might be
            // better to use an alternative labelling technique.


            // Alternative 1: Use a tiny disk with a centered label.
            //
            // Add a very small disk (essentially invisible), and annotate it with
            // centered text.

            Int32 iTableRow = 3;

            SetTableCellValue(vertexTable, iTableRow, VertexColumnName,
                "Alternative Label 1");

            SetTableCellValue(vertexTable, iTableRow, ColorColumnName, "Black");
            SetTableCellValue(vertexTable, iTableRow, ShapeColumnName, "Disk");
            SetTableCellValue(vertexTable, iTableRow, SizeColumnName, 0);
            SetTableCellValue(vertexTable, iTableRow, OpacityColumnName, 100);
            SetTableCellValue(vertexTable, iTableRow, VisibilityColumnName, "Show");

            SetTableCellValue(vertexTable, iTableRow, LabelColumnName,
                "Alternative Label 1");

            SetTableCellValue(vertexTable, iTableRow, LabelPositionColumnName,
                "Middle Center");

            SetTableCellValue(vertexTable, iTableRow, PolarRColumnName, 130.0);
            SetTableCellValue(vertexTable, iTableRow, PolarAngleColumnName, 270.0);


            // Alternative 2: Use a Label shape.
            //
            // Add a vertex and set its Shape to Label.  This uses the same Label
            // column that was used for the Disk shapes, but instead of the text
            // being drawn as an annotation, the Label shape gets drawn as a box
            // containing the specified text.
            //
            // The Label Position column is not used when the Shape is set to
            // Label.  The Color column determines the text and outline colors, and
            // the Label Fill Color column determnines the fill color.  The Size
            // column determines the font size, which in turn determines the box
            // size.

            iTableRow = 4;

            SetTableCellValue(vertexTable, iTableRow, VertexColumnName,
                "Alternative Label 2");

            SetTableCellValue(vertexTable, iTableRow, ColorColumnName, "Black");
            SetTableCellValue(vertexTable, iTableRow, ShapeColumnName, "Label");
            SetTableCellValue(vertexTable, iTableRow, SizeColumnName, 3);
            SetTableCellValue(vertexTable, iTableRow, OpacityColumnName, 100);
            SetTableCellValue(vertexTable, iTableRow, VisibilityColumnName, "Show");

            // Note that for the Label shape, the text can be multiline.

            SetTableCellValue(vertexTable, iTableRow, LabelColumnName,
                "Alternative\nLabel 2");

            SetTableCellValue(vertexTable, iTableRow, LabelFillColorColumnName,
                "White Smoke");

            SetTableCellValue(vertexTable, iTableRow, PolarRColumnName, 80.0);
            SetTableCellValue(vertexTable, iTableRow, PolarAngleColumnName, 150.0);
        }

        //*************************************************************************
        //  Method: SetLayoutAndReadWorkbook()
        //
        /// <summary>
        /// Sets the layout to Polar Absolute and forces NodeXL to read the
        /// workbook into the graph once the Excel application is made visible.
        /// </summary>
        ///
        /// <param name="perWorkbookSettingsTable">
        /// The per-workbook settings table in the NodeXL workbook.
        /// </param>
        //*************************************************************************

        private static void
        SetLayoutAndReadWorkbook
        (
            ListObject oPerWorkbookSettingsTable
        )
        {
            Debug.Assert(oPerWorkbookSettingsTable != null);

            // The table consists of name/value pairs, with the names in the
            // "Per-Workbook Setting" column and the values in the "Value" column.
            // Set the "Auto Layout on Open" value to "PolarAbsolute", with no
            // space.
            //
            // When the Excel application is made visible, NodeXL will read this
            // table, and if the "Auto Layout on Open" value isn't empty, it will
            // set the layout to the specified value and read the workbook.

            Range oNameColumnData;
            Object[,] aoNameColumnDataValues;

            if (!ExcelUtil.TryGetTableColumnDataAndValues(
                    oPerWorkbookSettingsTable, "Per-Workbook Setting",
                    out oNameColumnData, out aoNameColumnDataValues))
            {
                OnUnexpectedCondition(Properties.Resources.PerWorkbookTableMissing);
            }

            Int32 iRows = oNameColumnData.Rows.Count;

            for (Int32 i = 1; i <= iRows; i++)
            {
                String sName;

                if (
                    ExcelUtil.TryGetNonEmptyStringFromCell(aoNameColumnDataValues,
                        i, 1, out sName)
                    &&
                    sName == "Auto Layout on Open"
                    )
                {
                    // IMPORTANT: There is no space in "PolarAbsolute".  It's an
                    // Enum value.

                    SetTableCellValue(oPerWorkbookSettingsTable, i - 1, "Value",
                        "PolarAbsolute");

                    return;
                }
            }

            OnUnexpectedCondition(Properties.Resources.AutolayoutOnOpenMissing);
        }

        //*************************************************************************
        //  Method: TryGetTemplatePath()
        //
        /// <summary>
        /// Attempts to get the full path to the NodeXL template file.
        /// </summary>
        ///
        /// <param name="application">
        /// The Excel application.
        /// </param>
        ///
        /// <param name="templatePath">
        /// Where the path to the template file gets stored regardless of the
        /// return value.
        /// </param>
        ///
        /// <remarks>
        /// true if the template file exists.
        /// </remarks>
        //*************************************************************************
        private static Boolean
        TryGetTemplatePath
        (
            Microsoft.Office.Interop.Excel.Application application,
            out String templatePath
        )
        {
            string programFilesFolder;
            templatePath = null;

            if (IntPtr.Size == 4)
            {
                // We are running within a 32-bit .Net environment.
                programFilesFolder = Environment.GetFolderPath(
                    Environment.SpecialFolder.ProgramFiles);
            }
            else
            {
                // We are running on a 64-bit .Net environment and 
                // NodeXL templates are in the 32bit sub-system directory.
                programFilesFolder = System.Environment.GetEnvironmentVariable(
                    "ProgramFiles(x86)");
            }

            if (string.IsNullOrWhiteSpace(programFilesFolder))
            {
                programFilesFolder = string.Empty;
            }

            templatePath = Path.Combine(programFilesFolder, @"Microsoft Research\Microsoft NodeXL Excel Template\NodeXLGraph.xltx");
            if (File.Exists(templatePath))
                return true;

            // Check for updated install location
            templatePath = Path.Combine(programFilesFolder, @"Social Media Research Foundation\NodeXL Excel Template\NodeXLGraph.xltx");
            if (File.Exists(templatePath))
                return true;

            templatePath = Path.Combine(application.TemplatesPath, TemplateName);
            if (File.Exists(templatePath))
                return true;

            return false;
        }

        //*************************************************************************
        //  Method: SetTableCellValue()
        //
        /// <summary>
        /// Sets the value of one cell in a table.
        /// </summary>
        ///
        /// <param name="table">
        /// The table with the cell that needs to be set.
        /// </param>
        ///
        /// <param name="zeroBasedTableRow">
        /// Zero-based index of the data row.
        /// </param>
        ///
        /// <param name="columnName">
        /// Name of the table column.
        /// </param>
        ///
        /// <param name="cellValue">
        /// Value to set.
        /// </param>
        //*************************************************************************

        private static void
        SetTableCellValue
        (
            ListObject table,
            Int32 zeroBasedTableRow,
            String columnName,
            Object cellValue
        )
        {
            Debug.Assert(table != null);
            Debug.Assert(zeroBasedTableRow >= 0);
            Debug.Assert(!String.IsNullOrEmpty(columnName));

            Range oTableColumnData;

            if (!ExcelUtil.TryGetTableColumnData(table, columnName,
                out oTableColumnData))
            {
                OnUnexpectedCondition(Properties.Resources.NodeXLColumnMissing);
            }

            Range oCell = (Range)oTableColumnData.Cells[zeroBasedTableRow + 1, 1];

            oCell.set_Value(Missing.Value, cellValue);
        }

        //*************************************************************************
        //  Method: RadiusWpfToVertexSize()
        //
        /// <summary>
        /// Converts a vertex radius from WPF units to the units used in the NodeXL
        /// workbook.
        /// </summary>
        ///
        /// <param name="radiusWpf">
        /// Vertex radius, in WPF units.
        /// </param>
        ///
        /// <returns>
        /// The value to insert in the Size column to force a vertex to have a
        /// radius of radiusWpf WPF units.
        /// </returns>
        ///
        /// <remarks>
        /// The Size column in the vertex worksheet uses an arbitrary scale of 1 to
        /// 100.  This method calculates the Size to use to force a vertex to have
        /// a specified radius in WPF units.
        /// </remarks>
        //*************************************************************************

        private static Double
        RadiusWpfToVertexSize
        (
            Double radiusWpf
        )
        {
            Debug.Assert(radiusWpf >= 0);

            // A Size of 1 corresponds to 0.1 WPF, and a Size of 100 corresponds to
            // 549.0 WPF.

            Double dX1 = 1.0;
            Double dX2 = 100.0;
            Double dY1 = 0.1;
            Double dY2 = 549.0;

            Double dY = radiusWpf;

            // Given Y, calculate X:

            Double dX = dX1 + (dY - dY1) * (dX2 - dX1) / (dY2 - dY1);

            return (dX);
        }

        //*************************************************************************
        //  Method: OnUnexpectedCondition()
        //
        /// <summary>
        /// Attempts to get the full path to the application's template file.
        /// </summary>
        ///
        /// <param name="application">
        /// The Excel application.
        /// </param>
        ///
        /// <param name="templatePath">
        /// Where the path to the template file gets stored regardless of the
        /// return value.
        /// </param>
        ///
        /// <remarks>
        /// true if the template file exists.
        /// </remarks>
        //*************************************************************************

        private static void
        OnUnexpectedCondition
        (
            String errorMessage
        )
        {
            throw new InvalidOperationException(errorMessage);
        }


        //*************************************************************************
        //  Constants
        //*************************************************************************

        private const String TemplateName = "NodeXLGraph.xltx";

        private const String VertexColumnName = "Vertex";
        private const String ColorColumnName = "Color";
        private const String ShapeColumnName = "Shape";
        private const String SizeColumnName = "Size";
        private const String OpacityColumnName = "Opacity";
        private const String VisibilityColumnName = "Visibility";
        private const String LabelColumnName = "Label";
        private const String LabelFillColorColumnName = "Label Fill Color";
        private const String LabelPositionColumnName = "Label Position";
        private const String PolarRColumnName = "Polar R";
        private const String PolarAngleColumnName = "Polar Angle";
    }
}
