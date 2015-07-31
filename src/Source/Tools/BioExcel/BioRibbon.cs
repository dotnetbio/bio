using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;

using Bio;
using Bio.Algorithms.Alignment;
using Bio.Algorithms.Assembly;
using Bio.Algorithms.MUMmer;
using Bio.Extensions;
using Bio.IO;
using Bio.IO.Bed;
using Bio.IO.FastA;
using Bio.IO.FastQ;
using Bio.IO.GenBank;
using Bio.IO.Gff;
using Bio.Web.Blast;

using BiodexExcel.Properties;
using BiodexExcel.Visualizations.Common;
using BiodexExcel.Visualizations.Common.DialogBox;

using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;
using Microsoft.Vbe.Interop;
using Microsoft.Win32;

using Tools.VennDiagram;

using Color = System.Drawing.Color;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using XlHAlign = Microsoft.Office.Interop.Excel.XlHAlign;
using XlVAlign = Microsoft.Office.Interop.Excel.XlVAlign;

namespace BiodexExcel
{
    /// <summary>
    /// BioRibbon represents the .NET Bio tab in the Excel main ribbon. This class is the heart
    /// of the workbench and controls the entire workflow of the Ribbon.
    /// </summary>
    public partial class BioRibbon
    {
        #region -- Private Members --

        /// <summary>
        /// Maximum width of any column when importing a file.
        /// </summary>
        private const int MaximumColumnWidth = 50;

        /// <summary>
        /// Dictionary key for genbank metadata
        /// </summary>
        private const string GenbankMetadataKey = "GenBank";

        /// <summary>
        /// Dictionary key for getting offset values
        /// </summary>
        private const string StartOffsetString = "StartOffsets";

        /// <summary>
        /// Dictionary key for getting offset values
        /// </summary>
        private const string EndOffsetString = "EndOffsets";

        /// <summary>
        /// URL for .NET Bio homepage
        /// </summary>
        private const string MBFHomePage = "http://bio.codeplex.com";

        /// <summary>
        /// Required version of NodeXL for venn tool
        /// </summary>
        private const String NodeXLTemplateRequiredVersion = "1.0.1.108";

        /// <summary>
        /// Stores the number of english alphabets.
        /// </summary>
        private const int NumberOfAlphabets = 26;

        /// <summary>
        /// Stores the max length of a worksheet name.
        /// </summary>
        private const int MaxWorksheetNameLength = 30;

        /// <summary>
        /// Maximum number of columns supported by excel.
        /// </summary>
        private const int MaxExcelColumns = 15999;

        /// <summary>
        /// Indicates the zoom level for all the worksheets.
        /// </summary>
        private const int ZoomLevel = 70;

        /// <summary>
        /// Stores the value of the default color.
        /// </summary>
        private const double DefaultColorValue = 16777215.0;

        /// <summary>
        /// Key to get access to the cells context menu.
        /// </summary>
        private const string Cell = "Cell";

        /// <summary>
        /// Name of the macro which will draw a chart of frequencies of alphabets in the sequence.
        /// </summary>
        private const string DisplayChartMacroName = "DisplayChart";

        /// <summary>
        /// Maximum number of times object model call should be retried.
        /// (Retry 15 times in span of 10 minutes)
        /// </summary>
        private const int MaxRetryCount = 15;

        /// <summary>
        /// Base interval time between object model call retries
        /// </summary>
        private const int RetryInterval = 5000; // 1 Seconds

        /// <summary>
        /// Error code when object model call fails (VBA_E_IGNORE)
        /// </summary>
        private const int VbaIgnoreErrorCode = -2146777998;

        /// <summary>
        /// Indicates the maximum number of sequence characters that will be displayed in a single line.
        /// </summary>
        private static int maxNumberOfCharacters = 80;

        /// <summary>
        /// List of headers which will be displayed in a Sequence range sheet.
        /// </summary>
        private static readonly List<string> rangeHeaders = new List<string>
                                                            {
                                                                Resources.CHROM_ID,
                                                                Resources.CHROM_START,
                                                                Resources.CHROM_END,
                                                                Resources.BED_NAME,
                                                                Resources.BED_SCORE,
                                                                Resources.BED_STRAND,
                                                                Resources.BED_THICK_START,
                                                                Resources.BED_THICK_END,
                                                                Resources.BED_ITEM_RGB,
                                                                Resources.BED_BLOCK_COUNT,
                                                                Resources.BED_BLOCK_SIZES,
                                                                Resources.BED_BLOCK_STARTS
                                                            };

        /// <summary>
        /// List of headers which will be displayed in a BLAST sheet.
        /// </summary>
        private static readonly string[] blastHeaders =
        {
            Resources.QUERY_ID, Resources.SUBJECT_ID, Resources.IDENTITY,
            Resources.ALIGNMENT, Resources.LENGTH, Resources.QSTART,
            Resources.QEND, Resources.SSTART, Resources.SEND,
            Resources.EVALUE, Resources.SCORE, Resources.GAPS
        };

        /// <summary>
        /// Lists the name of the first 40 columns.
        /// </summary>
        private static readonly string[] columnAZ =
        {
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
            "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
        };

        /// <summary>
        /// Default background color for cells containing "A".
        /// </summary>
        private static readonly Color defaultAColor = Color.Gold;

        /// <summary>
        /// Default background color for cells containing "G".
        /// </summary>
        private static readonly Color defaultGColor = Color.Green;

        /// <summary>
        /// Default background color for cells containing "C".
        /// </summary>
        private static readonly Color defaultCColor = Color.Crimson;

        /// <summary>
        /// Default background color for cells containing "T".
        /// </summary>
        private static readonly Color defaultTColor = Color.Teal;

        /// <summary>
        /// Default background color for cells containing "U".
        /// </summary>
        private static readonly Color defaultUColor = Color.Teal;

        /// <summary>
        /// Stores the list of all file-names that have been imported into excel sheet.
        /// </summary>
        private readonly List<string> fileNames = new List<string>();

        /// <summary>
        /// Indicates a value whether all sequence sheet should be re-aligned or not.
        /// </summary>
        private bool alignAllSequenceSheet = false;

        /// <summary>
        /// References the background thread which will performs sequence alignment
        /// in the background.
        /// </summary>
        private BackgroundWorker alignerThread;

        /// <summary>
        /// References the background thread which will performs sequence assembly
        /// in the background.
        /// </summary>
        private BackgroundWorker assemblerThread;

        /// <summary>
        /// Stores information regarding the enabled state of
        /// Cancel button in Alignment (btnCancelAlign)
        /// </summary>
        private bool cancelAlignButtonState;

        /// <summary>
        /// Stores information regarding the enabled state of
        /// Cancel button in Assembly
        /// </summary>
        private bool cancelAssemblyButtonState;

        /// <summary>
        /// Stores information regarding the enabled state of
        /// Cancel button in Blast Search (btnCancelSearch)
        /// </summary>
        private bool cancelSearchButtonState;

        /// <summary>
        /// Stores mapping of a particular molecule name and the colors used.
        /// </summary>
        private Dictionary<byte, Color> colorMap = new Dictionary<byte, Color>();

        /// <summary>
        /// Sheet number to append to a alignment result sheet name
        /// </summary>
        private int currentAlignResultSheetNumber = 1;

        /// <summary>
        /// Number of times the BLAST service has been run in this instance. This number
        /// is appended to the name of the sheet.
        /// </summary>
        private int currentBlastSheetNumber = 1;

        /// <summary>
        /// Maintains a count of the number of times sequences were assembled in this instance.
        /// This number is appended to the name of the sheet.
        /// </summary>
        private int currentConsensusSheetNumber = 1;

        /// <summary>
        /// Maintains a count of the number of times sequence files are imported in this instance.
        /// This number is appended to the name of the sheet.
        /// </summary>
        private int currentFileNumber = 1;

        /// <summary>
        /// Maintains a count of the number of times range-sequence files are merged in this instance.
        /// This number is appended to the name of the sheet.
        /// </summary>
        private int currentMergeSheetNumber = 1;

        /// <summary>
        /// Maintains a count of the number of times intersect operation were performed in this instance.
        /// This number is appended to the name of the sheet.
        /// </summary>
        private int intersectSheetNumber = 1;

        /// <summary>
        /// Flag to indicate if NodeXL is installed or not.
        /// </summary>
        private bool isNodeXLInstalled;

        /// <summary>
        /// Identifier of the blast job submitted.
        /// </summary>
        private string requestIdentifier = string.Empty;

        /// <summary>
        /// Holds the number of sequences we import onto a single worksheet.
        /// </summary>
        private int sequencesPerWorksheet = 1;

        /// <summary>
        /// Maintains a count of the number of times subtract operation were performed in this instance.
        /// This number is appended to the name of the sheet.
        /// </summary>
        private int subtractSheetNumber = 1;

        /// <summary>
        /// Stores the default background color of the cells.
        /// </summary>
        private Color transparentColor;

        /// <summary>
        /// Name of the web service currently running.
        /// </summary>
        private string webserviceName;

        /// <summary>
        /// Key containing installation path of .NET Bio
        /// </summary>
        public static string MBFInstallationPath
        {
            get
            {
                //typical path is Program Files\Microsoft Biology Initiative\Microsoft Biology Framework
                Assembly assembly = Assembly.GetEntryAssembly();

                if (assembly != null)
                {
                    return Path.GetDirectoryName(assembly.Location);
                }

                string codeBase = Assembly.GetCallingAssembly().CodeBase;
                var uri = new Uri(codeBase);

                // BioExcel specific
                if (codeBase.Contains("exce..vsto"))
                {
                    //look into [HKEY_CURRENT_USER\Software\Microsoft\Office\Excel\Addins\BioExcel]
                    RegistryKey regKeyAppRoot =
                        Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Office\Excel\Addins\BioExcel");
                    uri = new Uri(regKeyAppRoot.GetValue("Manifest").ToString());
                }
                return Uri.UnescapeDataString(Path.GetDirectoryName(uri.AbsolutePath));
            }
        }

        #endregion -- Private Members --

        /// <summary>
        /// Initializes a new instance of the BioRibbon class.
        /// </summary>
        public BioRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            this.InitializeComponent();

            // Create a dummy 1st group -- this is to correct a bug
            // related to Excel add-ins where the first group disappears when
            // you start Excel with a file (vs. just starting a blank worksheet).
            RibbonGroup dummyGroup = this.Factory.CreateRibbonGroup();
            dummyGroup.Label = "Dummy";
            dummyGroup.Visible = false;

            this.tabBio.Groups.Insert(0, dummyGroup);

            this.BuildRibbonTabs();

            this.btnCancelAlign.Click += this.OnCancelAlignClicked;
            this.btnCancelAssemble.Click += this.OnCancelAssembleClicked;
            this.btnCancelSearch.Click += this.OnCancelSearchClicked;
            this.btnMaxColumn.Click += this.OnSizeChangedClicked;
            this.btnRunChartMacro.Click += this.RunMacro;
            this.btnAbout.Click += this.OnAboutClick;
            this.btnMerge.Click += this.OnMergeRangeSequence;
            this.btnIntersect.Click += this.OnIntersectClick;
            this.btnSubtract.Click += this.OnSubtractClick;
            this.btnConfigureColor.Click += this.OnConfigureColorClick;
            this.btnConfigureImport.Click += this.OnConfigureImportOptions;
            this.btnVennDiagram.Click += this.OnVennDiagramClick;
            this.btnHomePage.Click += this.OnHomePageClick;
            this.btnAssemble.Click += this.OnAssembleClick;

            this.SetScreenTips();
        }

        #region -- Private Static Methods --

        /// <summary>
        /// This method writes a given value onto a given range on a excel sheet.
        /// </summary>
        /// <param name="range">Range instance.</param>
        /// <param name="value">Value to be written to that range.</param>
        private static void WriteRangeValue(Range range, string value)
        {
            range.set_Value(XlRangeValueDataType.xlRangeValueDefault, value);
        }

        /// <summary>
        /// This method generates the column name string when given the column number
        /// as input.
        /// </summary>
        /// <param name="number">Column number</param>
        /// <returns>Column name</returns>
        private static string GetColumnString(long number)
        {
            var value = new StringBuilder();

            while (number > 0)
            {
                long mod = number % NumberOfAlphabets;
                if (mod == 0)
                {
                    value.Append("Z");
                    number = number / NumberOfAlphabets;
                    number--;
                }
                else
                {
                    value.Insert(0, columnAZ[mod - 1]);
                    number = number / NumberOfAlphabets;
                }
            }

            return value.ToString();
        }

        /// <summary>
        /// This method displays a single row of NCBI\EBI result data.
        /// </summary>
        /// <param name="currentsheet">The worksheet instance.</param>
        /// <param name="rowNumber">The row number from where the BLAST result rendering has to begin</param>
        /// <param name="values">The result values.</param>
        private static void DisplayValues(Worksheet currentsheet, int rowNumber, params string[] values)
        {
            int columnNumber = 2;

            foreach (string value in values)
            {
                Range range = currentsheet.get_Range(GetColumnString(columnNumber) + rowNumber, Type.Missing);
                WriteRangeValue(range, value);
                columnNumber++;
            }
        }

        /// <summary>
        /// This method runs assembly on the list of sequences passed.
        /// Additionally the user is allowed to select the
        /// alignment algorithm.
        /// </summary>
        /// <param name="input">Input for the assembly process.</param>
        /// <param name="worker">The Assembly parser thread</param>
        /// <returns>IDeNovoAssembly instance.</returns>
        private static IDeNovoAssembly RunAssembly(AssemblyInputEventArgs input, BackgroundWorker worker)
        {
            double mergeThreshold = input.MergeThreshold;
            List<ISequence> sequence = input.Sequences.ToList();

            var assemble = new OverlapDeNovoAssembler();
            assemble.OverlapAlgorithm = input.Aligner;

            // Special casing for SW alignment.
            if (assemble.OverlapAlgorithm is SmithWatermanAligner)
            {
                // If we set the Threshold value lesser than the Max score, then the result will be “JUNK”.
                // So setting the threshold value to 25 approximately supports sequence length of 15,0000.
                mergeThreshold = 25;
            }

            assemble.MergeThreshold = mergeThreshold;
            assemble.OverlapAlgorithm.SimilarityMatrix = input.AlignerInput.SimilarityMatrix;

            assemble.OverlapAlgorithm.GapOpenCost = input.AlignerInput.GapCost;
            assemble.OverlapAlgorithm.GapExtensionCost = input.AlignerInput.GapExtensionCost;

            assemble.ConsensusResolver = new SimpleConsensusResolver(input.ConsensusThreshold);
            assemble.AssumeStandardOrientation = false;

            AssignAlignerParameter(assemble.OverlapAlgorithm, input.AlignerInput);

            IDeNovoAssembly assemblyOutput = assemble.Assemble(sequence);

            if (worker != null && worker.CancellationPending)
            {
                return null;
            }

            return assemblyOutput;
        }

        /// <summary>
        /// Assign the aligner specific parameters
        /// </summary>
        /// <param name="sequenceAligner">Sequence Aligner object</param>
        /// <param name="alignerInput">Aligner Input object</param>
        private static void AssignAlignerParameter(ISequenceAligner sequenceAligner, AlignerInputEventArgs alignerInput)
        {
            if (sequenceAligner is NucmerPairwiseAligner)
            {
                var nucmer = sequenceAligner as NucmerPairwiseAligner;

                nucmer.LengthOfMUM = alignerInput.LengthOfMUM;
                nucmer.FixedSeparation = alignerInput.FixedSeparation;
                nucmer.MaximumSeparation = alignerInput.MaximumSeparation;
                nucmer.MinimumScore = alignerInput.MinimumScore;
                nucmer.SeparationFactor = alignerInput.SeparationFactor;
                nucmer.BreakLength = alignerInput.BreakLength;
            }
            else if (sequenceAligner is MUMmerAligner)
            {
                var mummer = sequenceAligner as MUMmerAligner;

                mummer.LengthOfMUM = alignerInput.LengthOfMUM;
            }
        }

        /// <summary>
        /// This method checks if the BLAST search has yielded
        /// any results or not.
        /// </summary>
        /// <param name="blastResults">List of BlastResult objects.</param>
        /// <returns>
        /// true: if the search has results, false otherwise.
        /// </returns>
        private static bool BlastHasResults(List<BlastResult> blastResults)
        {
            bool resultsFound = false;

            foreach (BlastResult result in blastResults)
            {
                foreach (BlastSearchRecord record in result.Records)
                {
                    if (null != record.Hits && 0 < record.Hits.Count)
                    {
                        foreach (Hit hit in record.Hits)
                        {
                            if (null != hit.Hsps && 0 < hit.Hsps.Count)
                            {
                                foreach (Hsp hsp in hit.Hsps)
                                {
                                    resultsFound = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return resultsFound;
        }

        /// <summary>
        /// Extracts the metadata in a SequenceRange object.
        /// </summary>
        /// <param name="range">Range object whose metadata has to be extracted.</param>
        /// <param name="key">Key for the metadata.</param>
        /// <returns>Value of the metadata.</returns>
        private static object ExtractRangeMetadata(ISequenceRange range, string key)
        {
            object metadataValue = null;

            if (range != null && range.Metadata != null && !string.IsNullOrEmpty(key) && range.Metadata.ContainsKey(key))
            {
                metadataValue = range.Metadata[key];
            }

            return metadataValue;
        }

        /// <summary>
        /// Gets a list of worksheets when given a list of worksheet names.
        /// </summary>
        /// <param name="sheetNames">List of sheet names</param>
        /// <returns>List of reference to worksheets with given names.</returns>
        private static List<Worksheet> GetWorksheetList(List<string> sheetNames)
        {
            Workbook workBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Sheets sheets = workBook.Worksheets;

            var worksheetList = new List<Worksheet>();
            foreach (string sheetName in sheetNames)
            {
                worksheetList.Add(GetSheetReference(sheetName));
            }

            return worksheetList;
        }

        /// <summary>
        /// Gets a reference to worksheet which has the same name as the sheet name.
        /// </summary>
        /// <param name="sheetName">Name of the worksheet.</param>
        /// <returns>Reference to worksheet.</returns>
        private static Worksheet GetSheetReference(string sheetName)
        {
            Workbook workBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Sheets sheets = workBook.Worksheets;
            Worksheet finalSheet = null;
            for (int i = 1; i <= sheets.Count; i++)
            {
                var sheet = sheets[i] as Worksheet;
                if (sheet != null && sheet.Name.Equals(sheetName))
                {
                    finalSheet = sheet;
                    break;
                }
            }

            return finalSheet;
        }

        #endregion --Private Static Methods--

        #region -- Private Methods --

        private readonly Regex regexAddress = new Regex(
            @"(?<Sheet>^.[^!]*)!\$(?<Column>.[^$]*)\$(?<Row>.[^$]*)$",
            RegexOptions.IgnoreCase);

        private readonly Regex regexRangeAddress =
            new Regex(
                @"(?<Sheet>^.[^!]*)!\$(?<Column>.[^$]*)\$(?<Row>.[^$]*):\$(?<Column1>.[^$]*)\$(?<Row1>.[^$]*)$",
                RegexOptions.IgnoreCase);

        /// <summary>
        /// Navigate to .NET Bio homepage using default browser.
        /// </summary>
        private void OnHomePageClick(object sender, RibbonControlEventArgs e)
        {
            Process.Start(MBFHomePage);
        }

        /// <summary>
        /// Set tool tips for UI items.
        /// </summary>
        private void SetScreenTips()
        {
            this.btnAbout.ScreenTip = Resources.MENU_ABOUT_TIP;
            this.btnAbout.SuperTip = Resources.MENU_ABOUT_SUPERTIP;
            this.btnConfigureColor.SuperTip = Resources.MENU_CONF_COLOR_SUPERTIP;
            this.btnMaxColumn.SuperTip = Resources.MENU_CONF_COLWRAP_SUPERTIP;
            this.splitConfiguration.ScreenTip = Resources.MENU_CONF_TIP;
            this.splitConfiguration.SuperTip = Resources.MENU_CONF_SUPERTIP;
            this.splitOperate.ScreenTip = Resources.MENU_BED_TIP;
            this.splitOperate.SuperTip = Resources.MENU_BED_SUPERTIP;
            this.splitChart.ScreenTip = Resources.MENU_CHART_TIP;
            this.splitChart.SuperTip = Resources.MENU_CHART_SUPERTIP;
            this.btnCancelSearch.ScreenTip = Resources.MENU_CANCELBLAST_TIP;
            this.btnCancelAlign.ScreenTip = Resources.MENU_CANCELALIGN_TIP;
            this.splitAligners.ScreenTip = Resources.MENU_ALIGNERS_TIP;
            this.splitAligners.SuperTip = Resources.MENU_ALIGNERS_SUPERTIP;
            this.btnAssemble.ScreenTip = Resources.MENU_ASSEMBLERS_TIP;
            this.btnAssemble.SuperTip = Resources.MENU_ASSEMBLERS_SUPERTIP;
            this.splitWebService.ScreenTip = Resources.MENU_BLAST_TIP;
            this.splitImport.ScreenTip = Resources.MENU_IMPORT_TIP;
            this.splitWebService.SuperTip = Resources.MENU_BLAST_SUPERTIP;
            this.splitExport.ScreenTip = Resources.MENU_EXPORT_TIP;
            this.splitExport.SuperTip = Resources.MENU_EXPORT_SUPERTIP;
            this.splitImport.SuperTip = Resources.MENU_IMPORT_SUPERTIP;
            this.btnVennDiagram.ScreenTip = Resources.MENU_VENN_TIP;
            this.btnVennDiagram.SuperTip = Resources.MENU_VENN_SUPERTIP;

            this.btnMerge.Description = Resources.MENU_MERGE_DESC;
            this.btnMerge.ScreenTip = Resources.SCREEN_TIP_MERGE;
            this.btnSubtract.Description = Resources.MENU_SUBTRACT_DESC;
            this.btnSubtract.ScreenTip = Resources.SCREEN_TIP_SUBTRACT;
            this.btnIntersect.Description = Resources.MENU_INTERSECT_DESC;
            this.btnIntersect.ScreenTip = Resources.SCREEN_TIP_INTERSECT;
        }

        /// <summary>
        /// This method is called when the entire ribbon is loaded.
        /// At this point of time all our UI elements will be properly
        /// initialized.
        /// </summary>
        /// <param name="sender">.NET Bio Ribbon.</param>
        /// <param name="e">Event data.</param>
        private void OnRibbonLoad(object sender, RibbonUIEventArgs e)
        {
            this.BuildColorScheme();
            this.CheckForNodeXL();
            this.EnableAllControls();

            // Adds a event handler every time a new sheet is selected.
            Globals.ThisAddIn.Application.WorkbookActivate += this.OnWorkBookOpen;
            Globals.ThisAddIn.Application.SheetChange += SequenceCache.OnSheetDataChanged;
            Globals.ThisAddIn.Application.WorkbookDeactivate += this.OnWorkbookDeactivate;
        }

        /// <summary>
        /// Get the location of nodeXL template file
        /// </summary>
        /// <returns>Path to nodeXL template file</returns>
        private string GetNodeXLTemplatePath()
        {
            string programFilesFolder;

            if (IntPtr.Size == 4)
            {
                // NodeXL is running within 32-bit Excel.
                programFilesFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            }
            else
            {
                // NodeXL is running within 64-bit Excel.
                programFilesFolder = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            if (string.IsNullOrWhiteSpace(programFilesFolder))
            {
                programFilesFolder = string.Empty;
            }

            string templatePath = Path.Combine(
                programFilesFolder,
                @"Microsoft Research\Microsoft NodeXL Excel Template\NodeXLGraph.xltx");
            if (File.Exists(templatePath))
            {
                return templatePath;
            }

            templatePath = Path.Combine(Globals.ThisAddIn.Application.TemplatesPath, "NodeXLGraph.xltx");
            if (File.Exists(templatePath))
            {
                return templatePath;
            }

            return string.Empty;
        }

        /// <summary>
        /// Check if NodeXL is installed and set appropriate flags and messages.
        /// </summary>
        private void CheckForNodeXL()
        {
            if (File.Exists(this.GetNodeXLTemplatePath())) // check if it exists at all
            {
                this.isNodeXLInstalled = true;
            }
            else
            {
                this.btnVennDiagram.SuperTip = Resources.MENU_VENN_NodeXLmissing;
            }
        }

        /// <summary>
        /// This method builds a dictionary of all the molecule types supported by .NET Bio
        /// and associates a default color to each of the molecule types.
        /// </summary>
        private void BuildColorScheme()
        {
            System.Windows.Media.Color transparentShade = Colors.Transparent;
            this.transparentColor = Color.FromArgb(
                transparentShade.A,
                transparentShade.R,
                transparentShade.G,
                transparentShade.B);
            foreach (byte nucleotide in DnaAlphabet.Instance)
            {
                if (!this.colorMap.ContainsKey(nucleotide))
                {
                    this.colorMap.Add(nucleotide, this.transparentColor);
                }
            }

            foreach (byte nucleotide in RnaAlphabet.Instance)
            {
                if (!this.colorMap.ContainsKey(nucleotide))
                {
                    this.colorMap.Add(nucleotide, this.transparentColor);
                }
            }

            foreach (byte acid in ProteinAlphabet.Instance)
            {
                if (!this.colorMap.ContainsKey(acid))
                {
                    this.colorMap.Add(acid, this.transparentColor);
                }
            }

            byte alphabet = DnaAlphabet.Instance.A;
            if (this.colorMap.ContainsKey(alphabet))
            {
                this.colorMap[alphabet] = defaultAColor;
                //this.colorMap[(byte)'a'] = defaultAColor;
            }

            alphabet = DnaAlphabet.Instance.G;
            if (this.colorMap.ContainsKey(alphabet))
            {
                this.colorMap[alphabet] = defaultGColor;
            }

            alphabet = DnaAlphabet.Instance.T;
            if (this.colorMap.ContainsKey(alphabet))
            {
                this.colorMap[alphabet] = defaultTColor;
            }

            alphabet = RnaAlphabet.Instance.U;
            if (this.colorMap.ContainsKey(alphabet))
            {
                this.colorMap[alphabet] = defaultUColor;
            }

            alphabet = DnaAlphabet.Instance.C;
            if (this.colorMap.ContainsKey(alphabet))
            {
                this.colorMap[alphabet] = defaultCColor;
            }
        }

        /// <summary>
        /// This method pops up a dialog for the user to change import
        /// options.
        /// </summary>
        /// <param name="sender">Configure import options button</param>
        /// <param name="e">Event data</param>
        private void OnConfigureImportOptions(object sender, RibbonControlEventArgs e)
        {
            this.ResetStatus();
            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlWait;

            var configuration = new ImportConfiguration { SequencesPerWorksheet = this.sequencesPerWorksheet };
            var helper = new WindowInteropHelper(configuration);
            helper.Owner = (IntPtr)Globals.ThisAddIn.Application.Hwnd;
            configuration.Activated += this.OnWPFWindowActivated;
            if (configuration.ShowDialog() == true)
            {
                this.sequencesPerWorksheet = configuration.SequencesPerWorksheet;
            }
        }

        /// <summary>
        /// This method pops up a dialog for the user which enables him
        /// to select the color scheme for molecule types.
        /// </summary>
        /// <param name="sender">Configure color button.</param>
        /// <param name="e">Event data.</param>
        private void OnConfigureColorClick(object sender, RibbonControlEventArgs e)
        {
            this.ResetStatus();
            var sheet = Globals.ThisAddIn.Application.ActiveSheet as Worksheet;

            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlWait;
            var configuration = new ColorConfiguration(Globals.ThisAddIn.Application, this.colorMap);
            var helper = new WindowInteropHelper(configuration);
            helper.Owner = (IntPtr)Globals.ThisAddIn.Application.Hwnd;
            configuration.Activated += this.OnWPFWindowActivated;
            if (configuration.Show())
            {
                this.colorMap = configuration.ColorMap;

                foreach (Range selectionArea in (Globals.ThisAddIn.Application.Selection as Range).Areas)
                {
                    this.FillBackGroundColor(selectionArea);
                }
            }
        }

        /// <summary>
        /// This method Intersect the intervals of 2 queries
        /// </summary>
        /// <param name="sender">btnIntersect instance.</param>
        /// <param name="e">Event data</param>
        private void OnIntersectClick(object sender, RibbonControlEventArgs e)
        {
            this.ResetStatus();
            var inputs = new InputSelection();
            inputs.MinimumSequenceCount = 2;
            inputs.SequenceLabels = new[]
                                    {
                                        Resources.InputSelection_SequenceLabel_BED1,
                                        Resources.InputSelection_SequenceLabel_BED2,
                                        Resources.Export_BED_SequenceRangeString
                                    }; // Set labels for the sequences
            inputs.GetInputSequenceRanges(this.DoBEDIntersect, true, true, true);
        }

        /// <summary>
        /// Callback method from input selection model which will actually do the selected operation.
        /// </summary>
        /// <param name="e">Event Argument</param>
        private void DoBEDIntersect(InputSequenceRangeSelectionEventArg e)
        {
            this.ScreenUpdate(false);

            Workbook currentWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            var intersectOutputType = IntersectOutputType.OverlappingIntervals;
            var result = new List<SequenceRangeGrouping>();

            if ((bool)e.Parameter[InputSelection.OVERLAP])
            {
                intersectOutputType = IntersectOutputType.OverlappingPiecesOfIntervals;
            }

            for (int i = 1; i < e.Sequences.Count; i++)
            {
                if (0 == result.Count)
                {
                    result.Add(
                        e.Sequences[i - 1].Intersect(
                            e.Sequences[i],
                            (long)e.Parameter[InputSelection.MINIMUMOVERLAP],
                            intersectOutputType,
                            true));
                }
                else
                {
                    result.Add(
                        result[result.Count - 1].Intersect(
                            e.Sequences[i],
                            (long)e.Parameter[InputSelection.MINIMUMOVERLAP],
                            intersectOutputType,
                            true));
                }
            }

            string sheetName = Resources.INTERSECT_SHEET
                               + this.intersectSheetNumber.ToString(CultureInfo.CurrentCulture);
            this.intersectSheetNumber++;
            this.WriteSequenceRange(currentWorkbook, sheetName, result[result.Count - 1], e.Data, true, false);

            this.ScreenUpdate(true);
        }

        /// <summary>
        /// This method Subtracts two query regions.
        /// </summary>
        /// <param name="sender">btnSubtract instance.</param>
        /// <param name="e">Event data</param>
        private void OnSubtractClick(object sender, RibbonControlEventArgs e)
        {
            this.ResetStatus();

            var inputs = new InputSelection();
            inputs.MinimumSequenceCount = 2;
            inputs.MaximumSequenceCount = 2;
            inputs.SequenceLabels = new[]
                                    {
                                        Resources.InputSelection_SequenceLabel_BED1,
                                        Resources.InputSelection_SequenceLabel_BED2,
                                        Resources.Export_BED_SequenceRangeString
                                    };
            inputs.GetInputSequenceRanges(this.DoBEDSubtract, false, true, true);
        }

        /// <summary>
        /// Callback method from input selection model which will actually do the selected operation.
        /// </summary>
        /// <param name="e">Event Argument</param>
        private void DoBEDSubtract(InputSequenceRangeSelectionEventArg e)
        {
            this.ScreenUpdate(false);

            Workbook currentWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            var subtractOutputType = SubtractOutputType.NonOverlappingPiecesOfIntervals;
            SequenceRangeGrouping result = null;

            if ((bool)e.Parameter[InputSelection.OVERLAP])
            {
                subtractOutputType = SubtractOutputType.IntervalsWithNoOverlap;
            }

            result = e.Sequences[0].Subtract(
                e.Sequences[1],
                (long)e.Parameter[InputSelection.MINIMUMOVERLAP],
                subtractOutputType,
                true);

            string sheetName = Resources.SUBTRACT_SHEET + this.subtractSheetNumber.ToString(CultureInfo.CurrentCulture);
            this.subtractSheetNumber++;
            this.WriteSequenceRange(currentWorkbook, sheetName, result, e.Data, true, false);

            this.ScreenUpdate(true);
        }

        /// <summary>
        /// Clear cached list of sheet name and load the new names.
        /// </summary>
        /// <param name="book">Workbook which contains a bunch of sheets.</param>
        private void LoadSheetNames(Workbook book)
        {
            this.fileNames.Clear();

            foreach (Worksheet sheet in book.Sheets)
            {
                this.fileNames.Add(sheet.Name);
            }
        }

        /// <summary>
        /// OnMergeRangeSequence Merges the overlapping intervals of a query.
        /// </summary>
        /// <param name="sender">btnMerge Instance.</param>
        /// <param name="e">Event data</param>
        private void OnMergeRangeSequence(object sender, RibbonControlEventArgs e)
        {
            this.ResetStatus();

            var inputs = new InputSelection();
            inputs.MinimumSequenceCount = 1;
            inputs.SequenceLabels = new[]
                                    {
                                        Resources.InputSelection_SequenceLabel_BED1,
                                        Resources.Export_BED_SequenceRangeString
                                    };
            inputs.GetInputSequenceRanges(this.DoBEDMerge, false, false, true);
        }

        /// <summary>
        /// Callback method from input selection model which will actually do the selected operation.
        /// </summary>
        /// <param name="e">Event Argument</param>
        private void DoBEDMerge(InputSequenceRangeSelectionEventArg e)
        {
            this.ScreenUpdate(false);

            Workbook currentWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;

            if (e.Sequences.Count > 1)
            {
                SequenceRangeGrouping referenceGrouping = e.Sequences[0];
                    // First item is mentioned as reference sequence when calling the dialog.
                for (int i = 1; i < e.Sequences.Count; i++)
                {
                    SequenceRangeGrouping rangeGrouping = e.Sequences[i];
                    referenceGrouping = referenceGrouping.MergeOverlaps(
                        rangeGrouping,
                        (long)e.Parameter[InputSelection.MINIMUMOVERLAP],
                        true);
                }

                string sheetName = Resources.MERGED_SHEET
                                   + this.currentMergeSheetNumber.ToString(CultureInfo.CurrentCulture);
                this.currentMergeSheetNumber++;
                this.WriteSequenceRange(currentWorkbook, sheetName, referenceGrouping, e.Data, false, true);
            }
            else
            {
                SequenceRangeGrouping mergedOverlap = e.Sequences[0];

                if (mergedOverlap != null)
                {
                    string sheetName = Resources.MERGED_SHEET
                                       + this.currentMergeSheetNumber.ToString(CultureInfo.CurrentCulture);
                    this.currentMergeSheetNumber++;
                    mergedOverlap = mergedOverlap.MergeOverlaps((long)e.Parameter[InputSelection.MINIMUMOVERLAP], true);
                    this.WriteSequenceRange(currentWorkbook, sheetName, mergedOverlap, e.Data, false, true);
                }
            }

            this.ScreenUpdate(true);
        }

        /// <summary>
        /// Formats and writes the query region (Output of Merge/Subtract/Intersect) operations
        /// </summary>
        /// <param name="resultWorkbook">
        /// Workbook to which Range has to be written
        /// </param>
        /// <param name="resultSheetname">New worksheet name</param>
        /// <param name="resultGroup">Output group</param>
        /// <param name="groupsData">
        /// Complete input groups information
        /// Contains individual Group, sheet and addresses of ISequenceRange
        /// </param>
        private void WriteSequenceRange(
            Workbook resultWorkbook,
            string resultSheetname,
            SequenceRangeGrouping resultGroup,
            Dictionary<SequenceRangeGrouping, GroupData> groupsData,
            bool showMetadata,
            bool showBasePairCount)
        {
            if (resultGroup.GroupIDs.Count() > 0)
            {
                int baseRowIndex = 2, baseColumnIndex = 2;
                int dataRowIndex = 0, dataColumnIndex = 0;
                int totalColumnCount = 0;
                object[,] values;
                List<string> hyperlinks = null;
                List<ISequenceRange> resultSequenceRanges = null;
                Dictionary<SequenceRangeGrouping, Dictionary<string, int>> groupSheetIndices = null;
                Dictionary<int, Tuple<SequenceRangeGrouping, bool, List<ISequenceRange>>> columnData = null;
                Dictionary<ISequenceRange, string> rangedata = null;
                Dictionary<SequenceRangeGrouping, SequenceRangeGrouping> allSheetData = null;
                Dictionary<SequenceRangeGrouping, int> allSheetCount = null;
                SequenceRangeGrouping groupToMerge = null;
                SequenceRangeGrouping referenceGroup = null;
                SequenceRangeGrouping queryGroup = null;
                SequenceRangeGrouping sheetGroup = null;
                int sheetCount = 0;
                Range activeRange = null;

                var resultWorksheet =
                    resultWorkbook.Worksheets.Add(
                        Type.Missing,
                        resultWorkbook.Worksheets.get_Item(resultWorkbook.Worksheets.Count),
                        Type.Missing,
                        Type.Missing) as Worksheet;
                ((_Worksheet)resultWorksheet).Activate();
                Globals.ThisAddIn.Application.ActiveWindow.Zoom = ZoomLevel;

                resultWorksheet.Name = resultSheetname;
                activeRange = resultWorksheet.get_Range(GetColumnString(baseColumnIndex) + baseRowIndex, Type.Missing);

                rangedata = groupsData.Values.Select(gd => gd.Metadata) // Get the Metadata
                    .SelectMany(sd => sd.Values).ToList() // Get the Dictionary
                    .SelectMany(rd => rd).ToList().ToDictionary(k => k.Key, v => v.Value); // Convert to dictionary

                groupSheetIndices = new Dictionary<SequenceRangeGrouping, Dictionary<string, int>>();
                baseRowIndex = this.WriteSequenceRangeHeader(
                    resultWorksheet,
                    groupSheetIndices,
                    groupsData,
                    baseRowIndex,
                    baseColumnIndex,
                    ref totalColumnCount,
                    showMetadata,
                    showBasePairCount);
                totalColumnCount -= (baseColumnIndex - 1);

                foreach (string resultGroupKey in resultGroup.GroupIDs)
                {
                    resultSequenceRanges = resultGroup.GetGroup(resultGroupKey);
                    dataRowIndex = 0;
                    values = new object[resultSequenceRanges.Count, totalColumnCount];
                    activeRange = resultWorksheet.get_Range(
                        GetColumnString(baseColumnIndex) + baseRowIndex,
                        Missing.Value);
                    activeRange = activeRange.get_Resize(resultSequenceRanges.Count, totalColumnCount);

                    foreach (ISequenceRange resultSequenceRange in resultSequenceRanges)
                    {
                        referenceGroup = null;
                        queryGroup = null;
                        dataColumnIndex = 0;
                        allSheetData = new Dictionary<SequenceRangeGrouping, SequenceRangeGrouping>();
                        allSheetCount = new Dictionary<SequenceRangeGrouping, int>();

                        values[dataRowIndex, dataColumnIndex] = resultSequenceRange.ID;
                        dataColumnIndex++;
                        values[dataRowIndex, dataColumnIndex] = resultSequenceRange.Start;
                        dataColumnIndex++;
                        values[dataRowIndex, dataColumnIndex] = resultSequenceRange.End;
                        dataColumnIndex++;

                        if (showMetadata)
                        {
                            for (int index = 3; index < rangeHeaders.Count; index++)
                            {
                                values[dataRowIndex, dataColumnIndex] = ExtractRangeMetadata(
                                    resultSequenceRange,
                                    rangeHeaders[index]);
                                dataColumnIndex++;
                            }
                        }

                        columnData = PrepareSequenceRowRange(
                            groupsData,
                            groupSheetIndices,
                            rangedata,
                            resultSequenceRange);

                        foreach (var columnGroup in columnData)
                        {
                            if (showBasePairCount)
                            {
                                // Get the parent ranges for Group's range in a column
                                groupToMerge = new SequenceRangeGrouping(columnGroup.Value.Item3);
                                if (1 < columnGroup.Value.Item3.Count)
                                {
                                    groupToMerge = groupToMerge.MergeOverlaps(0, false);
                                }

                                // Render data for Group's range in a column
                                values[dataRowIndex, columnGroup.Key] =
                                    groupToMerge.GroupRanges.Sum(sr => sr.End - sr.Start);
                                values[dataRowIndex, columnGroup.Key + 1] = columnGroup.Value.Item3.Count;
                            }
                            else
                            {
                                values[dataRowIndex, columnGroup.Key] = columnGroup.Value.Item3.Count;
                            }

                            // Let the hyperlink added
                            hyperlinks = new List<string>();
                            foreach (ISequenceRange range in columnGroup.Value.Item3)
                            {
                                hyperlinks.AddRange(rangedata[range].Split(','));
                            }

                            this.ShowHyperlink(
                                hyperlinks,
                                activeRange,
                                columnGroup.Key,
                                dataRowIndex,
                                showBasePairCount);

                            if (showBasePairCount)
                            {
                                // Calculate data for all group
                                if (allSheetData.TryGetValue(columnGroup.Value.Item1, out sheetGroup))
                                {
                                    allSheetData[columnGroup.Value.Item1] = sheetGroup.MergeOverlaps(
                                        groupToMerge,
                                        0,
                                        false);
                                }
                                else
                                {
                                    allSheetData[columnGroup.Value.Item1] = groupToMerge;
                                }

                                // Build up reference & query groups (later get common range using this)
                                if (columnGroup.Value.Item2)
                                {
                                    if (null == referenceGroup)
                                    {
                                        referenceGroup = groupToMerge;
                                    }
                                    else
                                    {
                                        referenceGroup = referenceGroup.MergeOverlaps(groupToMerge, 0, false);
                                    }
                                }
                                else
                                {
                                    if (null == queryGroup)
                                    {
                                        queryGroup = groupToMerge;
                                    }
                                    else
                                    {
                                        queryGroup = queryGroup.MergeOverlaps(groupToMerge, 0, false);
                                    }
                                }
                            }

                            // Calculate range count for all group
                            if (allSheetCount.TryGetValue(columnGroup.Value.Item1, out sheetCount))
                            {
                                allSheetCount[columnGroup.Value.Item1] += columnGroup.Value.Item3.Count;
                            }
                            else
                            {
                                allSheetCount[columnGroup.Value.Item1] = columnGroup.Value.Item3.Count;
                            }
                        }

                        // Render all columns in SequenceRangeGrouping
                        foreach (var allData in allSheetCount)
                        {
                            dataColumnIndex = groupSheetIndices[allData.Key].Values.Min() - (showBasePairCount ? 2 : 1);
                            if (showBasePairCount)
                            {
                                values[dataRowIndex, dataColumnIndex] =
                                    allSheetData[allData.Key].GroupRanges.Sum(sr => sr.End - sr.Start);
                                dataColumnIndex++;
                            }
                            values[dataRowIndex, dataColumnIndex] = allData.Value;
                        }

                        if (showBasePairCount)
                        {
                            // Render common column in SequenceRangeGrouping
                            if (null != referenceGroup && null != queryGroup)
                            {
                                referenceGroup = referenceGroup.Intersect(
                                    queryGroup,
                                    0,
                                    IntersectOutputType.OverlappingPiecesOfIntervals,
                                    false);
                                values[dataRowIndex, totalColumnCount - 1] =
                                    referenceGroup.GroupRanges.Sum(sr => sr.End - sr.Start);
                            }
                            else
                            {
                                values[dataRowIndex, totalColumnCount - 1] = 0;
                            }
                        }

                        dataRowIndex++;
                    }

                    activeRange.set_Value(Missing.Value, values);

                    baseRowIndex += dataRowIndex;
                }

                resultWorksheet.Columns.AutoFit();
                this.NormalizeColumWidths(resultWorksheet.UsedRange);
                this.EnableAllControls();
            }
            else
            {
                MessageBox.Show(
                    Resources.NO_RESULT,
                    Resources.CAPTION,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// There are range that can be merged (row-wise)
        /// </summary>
        /// <param name="hyperlinkList">Hyperlink list</param>
        /// <param name="activeRange">Active Range Object</param>
        /// <param name="columnIndex">Column index in Active range</param>
        /// <param name="dataRowIndex">Data row index in Active range</param>
        /// <param name="showBasePairCount">Show base pair count</param>
        private void ShowHyperlink(
            List<string> hyperlinkList,
            Range activeRange,
            int columnIndex,
            int dataRowIndex,
            bool showBasePairCount)
        {
            // make sure the address are arranged in order
            hyperlinkList.Sort();

            string rangeAddress = string.Empty, lastAddress = string.Empty;
            string currentSheet = string.Empty, lastSheet = string.Empty;
            string currentColumn = string.Empty, currentColumn1 = string.Empty;
            string lastColumn = string.Empty, lastColumn1 = string.Empty;
            int currentRow = 0, currentRow1 = 0;
            int lastRow = 0, lastRow1 = 0;
            int adjustmentRowCount = 0;
            Match currentMatch = null, lastMatch = null;

            foreach (string hyperlink in hyperlinkList)
            {
                if (string.IsNullOrEmpty(lastAddress))
                {
                    lastAddress = hyperlink;
                    continue;
                }

                // Check if the lastAddress and hyperlink are adjacent (differ by one row)
                // They must be in same format 
                // (<Sheet>!$<Column>$<Row)
                currentMatch = this.regexAddress.Match(hyperlink);
                if (currentMatch.Success)
                {
                    currentSheet = currentMatch.Groups["Sheet"].Value;
                    currentColumn = currentMatch.Groups["Column"].Value;
                    currentColumn1 = currentColumn;
                    int.TryParse(currentMatch.Groups["Row"].Value, out currentRow);
                    currentRow1 = currentRow;
                }
                else
                {
                    // (<Sheet>!$<Column>$<Row>:$<Column1>$<Row1>)
                    currentMatch = this.regexRangeAddress.Match(hyperlink);
                    if (currentMatch.Success)
                    {
                        currentSheet = currentMatch.Groups["Sheet"].Value;
                        currentColumn = currentMatch.Groups["Column"].Value;
                        currentColumn1 = currentMatch.Groups["Column1"].Value;
                        int.TryParse(currentMatch.Groups["Row"].Value, out currentRow);
                        int.TryParse(currentMatch.Groups["Row1"].Value, out currentRow1);
                    }
                    else
                    {
                        currentSheet = currentColumn = currentColumn1 = string.Empty;
                        currentRow = currentRow1 = -1;
                    }
                }

                lastMatch = this.regexAddress.Match(lastAddress);
                if (lastMatch.Success)
                {
                    lastSheet = lastMatch.Groups["Sheet"].Value;
                    lastColumn = lastMatch.Groups["Column"].Value;
                    lastColumn1 = lastColumn;
                    int.TryParse(lastMatch.Groups["Row"].Value, out lastRow);
                    lastRow1 = lastRow;
                }
                else
                {
                    lastMatch = this.regexRangeAddress.Match(lastAddress);
                    if (lastMatch.Success)
                    {
                        lastSheet = lastMatch.Groups["Sheet"].Value;
                        lastColumn = lastMatch.Groups["Column"].Value;
                        lastColumn1 = lastMatch.Groups["Column1"].Value;
                        int.TryParse(lastMatch.Groups["Row"].Value, out lastRow);
                        int.TryParse(lastMatch.Groups["Row1"].Value, out lastRow1);
                    }
                    else
                    {
                        lastSheet = lastColumn = lastColumn1 = string.Empty;
                        lastRow = lastRow1 = -1;
                    }
                }

                if (0 == string.Compare(currentSheet, lastSheet, true)
                    && 0 == string.Compare(currentColumn, lastColumn, true)
                    && 0 == string.Compare(currentColumn1, lastColumn1, true)
                    && currentRow1 == lastRow + adjustmentRowCount + 1)
                {
                    lastAddress = string.Concat(
                        currentSheet,
                        "!$",
                        lastColumn,
                        "$",
                        lastRow,
                        ":$",
                        currentColumn1,
                        "$",
                        currentRow1);
                    adjustmentRowCount++;
                    continue;
                }

                if (string.IsNullOrEmpty(rangeAddress))
                {
                    rangeAddress = this.HandleSpecialChars(lastAddress);
                }
                else
                {
                    rangeAddress = string.Concat(rangeAddress, ",", this.HandleSpecialChars(lastAddress));
                }

                lastAddress = hyperlink;
                adjustmentRowCount = 0;
            }

            if (string.IsNullOrEmpty(rangeAddress))
            {
                rangeAddress = this.HandleSpecialChars(lastAddress);
            }
            else
            {
                rangeAddress = string.Concat(rangeAddress, ",", this.HandleSpecialChars(lastAddress));
            }

            // Add hyperlink
            activeRange.Hyperlinks.Add(
                activeRange.get_Range(
                    GetColumnString(columnIndex + (showBasePairCount ? 2 : 1)) + (dataRowIndex + 1),
                    Missing.Value),
                string.Empty,
                rangeAddress,
                Missing.Value,
                Missing.Value);
        }

        /// <summary>
        /// Convert a range address into proper format by handling special chars
        /// </summary>
        /// <param name="rangeAddress">Source range address</param>
        /// <returns>Converted range address</returns>
        private string HandleSpecialChars(string rangeAddress)
        {
            int sheetNameSplitterIndex = rangeAddress.LastIndexOf('!');
            string sheetName = rangeAddress.Substring(0, sheetNameSplitterIndex).Replace("'", "''");
            rangeAddress = string.Format("'{0}'!{1}", sheetName, rangeAddress.Substring(sheetNameSplitterIndex + 1));
            return rangeAddress;
        }

        /// <summary>
        /// Prepares a row of SequenceRange for writing.
        /// </summary>
        /// <param name="groupsData">
        /// Complete input groups information
        /// Contains individual Group, sheet and addresses of ISequenceRange
        /// </param>
        /// <param name="groupSheetIndices">
        /// Complete indices.
        /// Contains individual column of each sheet of each Group
        /// </param>
        /// <param name="rangedata">Sequence and address list</param>
        /// <param name="resultSequenceRange">Query region that has to be prepared</param>
        /// <returns>Prepared data ready for output</returns>
        private static Dictionary<int, Tuple<SequenceRangeGrouping, bool, List<ISequenceRange>>> PrepareSequenceRowRange
            (
            Dictionary<SequenceRangeGrouping, GroupData> groupsData,
            Dictionary<SequenceRangeGrouping, Dictionary<string, int>> groupSheetIndices,
            Dictionary<ISequenceRange, string> rangedata,
            ISequenceRange resultSequenceRange)
        {
            Dictionary<int, Tuple<SequenceRangeGrouping, bool, List<ISequenceRange>>> columnData = null;

            columnData = new Dictionary<int, Tuple<SequenceRangeGrouping, bool, List<ISequenceRange>>>();
            foreach (ISequenceRange parentRange in resultSequenceRange.ParentSeqRanges)
            {
                PrepareSequenceRangeRow(groupsData, groupSheetIndices, rangedata, columnData, parentRange);
            }

            return columnData;
        }

        /// <summary>
        /// Prepares a row of SequenceRange for writing.
        /// </summary>
        /// <param name="groupsData">
        /// Complete input groups information
        /// Contains individual Group, sheet and addresses of ISequenceRange
        /// </param>
        /// <param name="groupSheetIndices">
        /// Complete indices.
        /// Contains individual column of each sheet of each Group
        /// </param>
        /// <param name="rangedata">Sequence and address list</param>
        /// <param name="columnData">Data ready for output</param>
        /// <param name="parentRange">Query region that has to be prepared</param>
        private static void PrepareSequenceRangeRow(
            Dictionary<SequenceRangeGrouping, GroupData> groupsData,
            Dictionary<SequenceRangeGrouping, Dictionary<string, int>> groupSheetIndices,
            Dictionary<ISequenceRange, string> rangedata,
            Dictionary<int, Tuple<SequenceRangeGrouping, bool, List<ISequenceRange>>> columnData,
            ISequenceRange parentRange)
        {
            IEnumerable<SequenceRangeGrouping> grp = groupsData.Keys.Where(s => s.GroupRanges.Contains(parentRange));
            if (0 == grp.Count())
            {
                foreach (ISequenceRange grandParent in parentRange.ParentSeqRanges)
                {
                    PrepareSequenceRangeRow(groupsData, groupSheetIndices, rangedata, columnData, grandParent);
                }

                return;
            }

            Tuple<SequenceRangeGrouping, bool, List<ISequenceRange>> parentType = null;
                // Where the parent is ref / query
            List<ISequenceRange> parentRanges = null;
            SequenceRangeGrouping group = null;
            List<SequenceRangeGrouping> inputGroups = groupsData.Keys.ToList();

            // Regular expression to read the sheet name from address
            var regexSheetname = new Regex(@"(?<Sheetname>^.[^!]*)", RegexOptions.IgnoreCase);
            Match matchSheetname = null;
            Dictionary<string, int> sheetIndices = null;
            string sheetName = string.Empty;
            int sheetIndex;

            group = grp.First();
            if (groupSheetIndices.TryGetValue(group, out sheetIndices))
            {
                matchSheetname = regexSheetname.Match(rangedata[parentRange]);
                if (matchSheetname.Success)
                {
                    sheetName = matchSheetname.Groups["Sheetname"].Value;
                }

                if (sheetIndices.TryGetValue(sheetName, out sheetIndex))
                {
                    if (columnData.TryGetValue(sheetIndex, out parentType))
                    {
                        parentRanges = parentType.Item3;
                    }
                    else
                    {
                        parentRanges = new List<ISequenceRange>();
                        parentType = new Tuple<SequenceRangeGrouping, bool, List<ISequenceRange>>(
                            group,
                            inputGroups.IndexOf(group) == 0,
                            parentRanges);
                        columnData.Add(sheetIndex, parentType);
                    }

                    parentRanges.Add(parentRange);
                }
            }
        }

        /// <summary>
        /// Build the header for Merge operations on BED inputs
        /// </summary>
        /// <param name="currentSheet">Current worksheet object</param>
        /// <param name="groupSheetIndices">
        /// Complete indices.
        /// Contains individual column of each sheet of each Group
        /// </param>
        /// <param name="groupsData">
        /// Complete input groups information
        /// Contains individual Group, sheet and addresses of ISequenceRange
        /// </param>
        /// <param name="baseRowIndex">Base row index</param>
        /// <param name="baseColumnIndex"></param>
        /// <returns>Current index of sheet row</returns>
        private int WriteSequenceRangeHeader(
            Worksheet currentSheet,
            Dictionary<SequenceRangeGrouping, Dictionary<string, int>> groupSheetIndices,
            Dictionary<SequenceRangeGrouping, GroupData> groupsData,
            int baseRowIndex,
            int baseColumnIndex,
            ref int sheetColumnIndex,
            bool showMetadata,
            bool showBasePairCount)
        {
            int sheetRowIndex = baseRowIndex + 2;
            Dictionary<string, int> sheetIndices = null;
            sheetColumnIndex = baseColumnIndex;
            Range activeRange = currentSheet.get_Range(GetColumnString(sheetColumnIndex) + sheetRowIndex, Type.Missing);

            activeRange.Cells.Font.Bold = true;
            activeRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            WriteRangeValue(activeRange, Resources.CHROM_ID);
            sheetColumnIndex++;

            activeRange = activeRange.Next;
            activeRange.Cells.Font.Bold = true;
            activeRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            WriteRangeValue(activeRange, Resources.CHROM_START);
            sheetColumnIndex++;

            activeRange = activeRange.Next;
            activeRange.Cells.Font.Bold = true;
            activeRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            WriteRangeValue(activeRange, Resources.CHROM_END);
            sheetColumnIndex++;

            if (showMetadata)
            {
                for (int index = 3; index < rangeHeaders.Count; index++)
                {
                    activeRange = activeRange.Next;
                    activeRange.Cells.Font.Bold = true;
                    activeRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    WriteRangeValue(activeRange, rangeHeaders[index]);
                    sheetColumnIndex++;
                }
            }

            sheetRowIndex -= 2;
            foreach (var groupData in groupsData)
            {
                activeRange = currentSheet.get_Range(GetColumnString(sheetColumnIndex) + sheetRowIndex, Type.Missing);
                activeRange = activeRange.get_Resize(
                    1,
                    (groupData.Value.Metadata.Count * (showBasePairCount ? 2 : 1)) + (showBasePairCount ? 2 : 1));
                activeRange.Merge();
                activeRange.Cells.Font.Bold = true;
                activeRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                WriteRangeValue(activeRange, groupData.Value.Name);

                WriteSequeceRangeHeader(
                    currentSheet,
                    activeRange,
                    Resources.ALL_SHEETS,
                    ref sheetRowIndex,
                    ref sheetColumnIndex,
                    showBasePairCount);

                foreach (string sheetname in groupData.Value.Metadata.Keys)
                {
                    if (!groupSheetIndices.TryGetValue(groupData.Key, out sheetIndices))
                    {
                        sheetIndices = new Dictionary<string, int>();
                        groupSheetIndices[groupData.Key] = sheetIndices;
                    }

                    sheetIndices[sheetname] = sheetColumnIndex - baseColumnIndex;
                    WriteSequeceRangeHeader(
                        currentSheet,
                        activeRange,
                        sheetname,
                        ref sheetRowIndex,
                        ref sheetColumnIndex,
                        showBasePairCount);
                }
            }

            if (showBasePairCount)
            {
                activeRange = currentSheet.get_Range(GetColumnString(sheetColumnIndex) + sheetRowIndex, Type.Missing);
                activeRange.Cells.Font.Bold = true;
                activeRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                WriteRangeValue(activeRange, Resources.COMMON);

                sheetRowIndex++;
                activeRange = currentSheet.get_Range(GetColumnString(sheetColumnIndex) + sheetRowIndex, Type.Missing);
                activeRange.Cells.Font.Bold = true;
                activeRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                WriteRangeValue(activeRange, Resources.BASE_PAIR_COUNT);
                sheetRowIndex += 2;
            }
            else
            {
                sheetRowIndex += 3;
            }

            return sheetRowIndex;
        }

        /// <summary>
        /// Write the column header for a sheet
        /// </summary>
        /// <param name="currentSheet">Current worksheet object</param>
        /// <param name="currentRange">Current range object</param>
        /// <param name="sequenceGroupName">Sequence group Name</param>
        /// <param name="sheetRowIndex">Sheet row index</param>
        /// <param name="sheetColumnIndex">Sheet column index</param>
        private static void WriteSequeceRangeHeader(
            Worksheet currentSheet,
            Range currentRange,
            string sequenceGroupName,
            ref int sheetRowIndex,
            ref int sheetColumnIndex,
            bool showBasePairCount)
        {
            sheetRowIndex++;
            currentRange = currentSheet.get_Range(GetColumnString(sheetColumnIndex) + sheetRowIndex, Type.Missing);
            currentRange = currentRange.get_Resize(1, showBasePairCount ? 2 : 1);
            currentRange.Merge();
            currentRange.Cells.Font.Bold = true;
            currentRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            WriteRangeValue(currentRange, sequenceGroupName);
            sheetRowIndex++;

            if (showBasePairCount)
            {
                currentRange = currentSheet.get_Range(GetColumnString(sheetColumnIndex) + sheetRowIndex, Type.Missing);
                currentRange.Cells.Font.Bold = true;
                currentRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                WriteRangeValue(currentRange, Resources.BASE_PAIR_COUNT);
                sheetColumnIndex++;
            }

            currentRange = currentSheet.get_Range(GetColumnString(sheetColumnIndex) + sheetRowIndex, Type.Missing);
            currentRange.Cells.Font.Bold = true;
            currentRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            WriteRangeValue(currentRange, Resources.NUMBER_OF_RANGES);
            sheetColumnIndex++;

            sheetRowIndex -= 2;
        }

        /// <summary>
        /// Limits column widts of given range to a specific width
        /// </summary>
        /// <param name="range">Range of which the column size should be limited</param>
        private void NormalizeColumWidths(Range range)
        {
            foreach (Range column in range.Columns)
            {
                if (column.EntireColumn.ColumnWidth > MaximumColumnWidth)
                {
                    column.ColumnWidth = MaximumColumnWidth;
                }
            }
        }

        /// <summary>
        /// Displays About dialog.
        /// </summary>
        /// <param name="sender">About button.</param>
        /// <param name="e">Event data</param>
        private void OnAboutClick(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlWait;
            this.ResetStatus();
            var dialog = new AboutDialog();
            dialog.Activated += this.OnWPFWindowActivated;
            var helper = new WindowInteropHelper(dialog);
            helper.Owner = (IntPtr)Globals.ThisAddIn.Application.Hwnd;
            dialog.ShowDialog();
        }

        /// <summary>
        /// Create a Venn diagram out of two/three SequenceRangeGrouping objects.
        /// This method gets the user input for creating the diagram
        /// </summary>
        private void OnVennDiagramClick(object sender, RibbonControlEventArgs e)
        {
            var inputs = new InputSelection();
            inputs.MinimumSequenceCount = 2;
            inputs.MaximumSequenceCount = 3;
            inputs.PromptForSequenceName = false;
            inputs.SequenceLabels = new[]
                                    {
                                        Resources.InputSelection_SequenceLabel_Venn1,
                                        Resources.InputSelection_SequenceLabel_Venn2,
                                        Resources.InputSelection_SequenceLabel_Venn3
                                    };
            inputs.GetInputSequenceRanges(this.DoDrawVenn, false, false, false);
        }

        /// <summary>
        /// Call venn diagram module to create the diagran and display it using NodeXL
        /// </summary>
        private void DoDrawVenn(InputSequenceRangeSelectionEventArg e)
        {
            // Call VennToNodeXL which will do the calculations and add the NodeXL workbook to the application object passed.
            if (e.Sequences.Count == 2)
            {
                VennToNodeXL.CreateNodeXLVennDiagramWorkbookFromSequenceRangeGroupings(
                    Globals.ThisAddIn.Application,
                    e.Sequences[0],
                    e.Sequences[1]);
            }
            else // if sequence count is not 2, its 3. InputSequenceDialog guarentees it as we set min=2 and max=3.
            {
                VennToNodeXL.CreateNodeXLVennDiagramWorkbookFromSequenceRangeGroupings(
                    Globals.ThisAddIn.Application,
                    e.Sequences[0],
                    e.Sequences[1],
                    e.Sequences[2]);
            }
        }

        /// <summary>
        /// Change the wait cursor to normal one once the window is loaded.
        /// </summary>
        private void OnWPFWindowActivated(object sender, EventArgs e)
        {
            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlDefault;
        }

        /// <summary>
        /// Gets a valid file name according to the excel rules for naming a worksheet.
        /// </summary>
        /// <param name="fileName">The name of the file being imported</param>
        /// <returns>A valid name for worksheets</returns>
        private string GetValidFileNames(string fileName)
        {
            string file = fileName, validFileName;
            var sb = new StringBuilder();

            int index = 0;

            // Removes all characters apart from letters or digits.
            for (; index < file.Length; index++)
            {
                if (char.IsLetterOrDigit(file[index]) || file[index] == '_')
                {
                    sb.Append(file[index]);
                }
            }

            if (sb.Length == 0)
            {
                sb =
                    new StringBuilder(
                        Resources.SHEET_NAME + this.currentFileNumber.ToString(CultureInfo.CurrentCulture));

                validFileName = sb.ToString();
                this.fileNames.Add(validFileName);
                return validFileName;
            }

            // Ensures that the file name is not greater than "MaxWorksheetNameLength" characters.
            int maxLength = sb.Length > MaxWorksheetNameLength ? MaxWorksheetNameLength : sb.Length;

            if (this.fileNames.Contains(sb.ToString(0, maxLength)))
            {
                sb = new StringBuilder(sb.ToString(0, maxLength));
                int count = 0;

                // If this file name is already present, start adding indexes to it.
                // For e.g if file name is abcd, then on first import the new name will be abcd1,
                // then on further imports it will become abcd2,abcd3,abcd4.....
                while (this.fileNames.Contains(sb.ToString()))
                {
                    if (count != 0)
                    {
                        sb.Remove(sb.Length - 1, 1);
                    }

                    count++;
                    sb.Append(count);
                }

                // If it has exceeded "MaxWorksheetNameLength" characters then start removing the second
                // character. 
                if (sb.Length > MaxWorksheetNameLength)
                {
                    while (this.fileNames.Contains(sb.ToString()) || sb.Length > MaxWorksheetNameLength)
                    {
                        if (sb.Length > 3)
                        {
                            sb.Remove(2, 1);
                        }
                        else
                        {
                            break;
                        }
                    }

                    // If it still doesnt have a unique name, then the new name is Seq_1 or Seq_2.......
                    if (this.fileNames.Contains(sb.ToString()))
                    {
                        sb =
                            new StringBuilder(
                                Resources.SHEET_NAME + this.currentFileNumber.ToString(CultureInfo.CurrentCulture));
                        this.currentFileNumber++;
                    }
                }

                this.fileNames.Add(sb.ToString());
                return sb.ToString();
            }

            validFileName = sb.ToString(0, maxLength);
            this.fileNames.Add(validFileName);
            return validFileName;
        }

        /// <summary>
        /// This method populates the BioRibbon tab with UI controls.
        /// </summary>
        private void BuildRibbonTabs()
        {
            this.AddParsersDropDowns();
            this.AddFormattersDropDowns();
            this.AddAlignersDropDown();
            //this.AddWebServiceDropDowns();
        }

        /// <summary>
        /// This method retrieves all the supported aligners in the framework and
        /// populates the Align drop down.
        /// </summary>
        private void AddAlignersDropDown()
        {
            foreach (ISequenceAligner aligner in SequenceAligners.All)
            {
                RibbonButton button = this.Factory.CreateRibbonButton();
                button.ControlSize = RibbonControlSize.RibbonControlSizeLarge;
                button.Image = Resources.FromExcel;
                button.Tag = aligner;
                button.Click += this.OnAlignmentButtonClicked;
                button.Label = string.Format(aligner.Name);
                button.Description = string.Format(aligner.Description);
                if (aligner is IPairwiseSequenceAligner)
                {
                    button.ScreenTip = string.Format(Resources.SCREEN_TIP_PAIRWISEALIGNERS, button.Label);
                }
                else
                {
                    button.ScreenTip = string.Format(Resources.SCREEN_TIP_ALIGNERS, button.Label);
                }
                this.splitAligners.Items.Add(button);
            }
        }

        /// <summary>
        /// This method retrieves all the supported formatters in the framework and
        /// populates the formatters drop down.
        /// </summary>
        private void AddFormattersDropDowns()
        {
            foreach (ISequenceFormatter formatter in SequenceFormatters.All)
            {
                RibbonButton button = this.Factory.CreateRibbonButton();
                button.ControlSize = RibbonControlSize.RibbonControlSizeLarge;
                button.Image = Resources.FromText;
                button.Label = string.Format(formatter.Name.ToUpper(CultureInfo.CurrentCulture));
                button.ShowImage = true;
                button.ScreenTip = string.Format(Resources.SCREEN_TIP_EXPORT_MENU, button.Label);
                button.Tag = formatter;
                button.Click += this.OnExportClick;
                this.splitExport.Items.Add(button);
            }

            foreach (ISequenceRangeFormatter formatter in SequenceRangeFormatters.All)
            {
                RibbonButton button = this.Factory.CreateRibbonButton();
                button.ControlSize = RibbonControlSize.RibbonControlSizeLarge;
                button.Image = Resources.FromText;
                button.Label = string.Format(formatter.Name.ToUpper(CultureInfo.CurrentCulture));
                button.ShowImage = true;
                button.ScreenTip = string.Format(Resources.SCREEN_TIP_EXPORT_MENU, button.Label);
                button.Tag = formatter;
                button.Click += this.OnExportClick;
                this.splitExport.Items.Add(button);
            }
        }

        /// <summary>
        /// This method retrieves all the supported parsers in the framework and
        /// populates the parsers drop down.
        /// </summary>
        private void AddParsersDropDowns()
        {
            foreach (ISequenceParser parser in SequenceParsers.All)
            {
                RibbonButton button = this.Factory.CreateRibbonButton();
                button.ControlSize = RibbonControlSize.RibbonControlSizeLarge;
                button.Image = Resources.FromText;
                button.Click += this.ReadSequenceFiles;
                button.Label = string.Format(parser.Name.ToUpper(CultureInfo.CurrentCulture));
                button.ShowImage = true;
                button.Tag = parser;
                button.ScreenTip = string.Format(Resources.SCREEN_TIP_IMPORT_MENU, button.Label);
                this.splitImport.Items.Add(button);
            }

            foreach (ISequenceRangeParser parser in SequenceRangeParsers.All)
            {
                RibbonButton button = this.Factory.CreateRibbonButton();
                button.ControlSize = RibbonControlSize.RibbonControlSizeLarge;
                button.Image = Resources.FromText;
                button.Click += this.ReadSequenceFiles;
                button.Label = string.Format(parser.Name.ToUpper(CultureInfo.CurrentCulture));
                button.ShowImage = true;
                button.Tag = parser;
                button.ScreenTip = string.Format(Resources.SCREEN_TIP_IMPORT_MENU, button.Label);
                this.splitImport.Items.Add(button);
            }
        }

        /// <summary>
        /// This method retrieves all the supported Web-Service in the framework and
        /// populates the Web-Services drop down.
        /// </summary>
        private void AddWebServiceDropDowns()
        {
            //foreach (IBlastServiceHandler blastService in
            //    WebServices.All.Where(service => service is IBlastServiceHandler))
            //{
            //    RibbonButton button = this.Factory.CreateRibbonButton();
            //    button.ControlSize = RibbonControlSize.RibbonControlSizeLarge;
            //    button.Image = Resources.FromWeb;
            //    button.Click += this.OnExecuteBlastSearch;
            //    button.Label = string.Format(Resources.SERVICE_LABEL, blastService.Name);
            //    button.Tag = blastService.Name;
            //    button.Description = string.Format(Resources.SERVICE_DESC, blastService.Name);
            //    button.ShowImage = true;
            //    button.ScreenTip = string.Format(Resources.SCREEN_TIP_BLAST_SEARCH, blastService.Name);
            //    this.splitWebService.Items.Add(button);
            //}
        }

        /// <summary>
        /// This method runs the DisplayChart macro which draws a chart of alphabet frequencies.
        /// </summary>
        /// <param name="sender">btnRunChartMacro instance.</param>
        /// <param name="e">Event data.</param>
        private void RunMacro(object sender, RibbonControlEventArgs e)
        {
            this.ResetStatus();
            vbext_ProcKind procType;
            string macroName = string.Empty;

            // Get the list of all Macros defined in this workbook.
            try
            {
                foreach (VBComponent vbc in Globals.ThisAddIn.Application.ActiveWorkbook.VBProject.VBComponents)
                {
                    CodeModule codeModule = vbc.CodeModule;
                    int currentLine = codeModule.CountOfDeclarationLines + 1;
                    while (currentLine < codeModule.CountOfLines)
                    {
                        string procName = codeModule.get_ProcOfLine(currentLine, out procType);

                        // Check if the name matches the Display Chart macro name.
                        if (procName.Contains(DisplayChartMacroName))
                        {
                            // If it does use this particular macro.
                            macroName = procName;
                            break;
                        }

                        currentLine += codeModule.get_ProcCountLines(procName, procType);
                    }
                }
            }
            catch
            {
                macroName = string.Empty;
            }

            try
            {
                // If macro not found, then raise an error.
                if (!string.IsNullOrEmpty(macroName))
                {
                    // If found, run the macro.
                    Globals.ThisAddIn.Application.Run(
                        macroName,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value);
                }
                else
                {
                    MessageBox.Show(
                        Resources.MACRO_MISSING,
                        Resources.CAPTION,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// This method is called when the user wishes to configure the max number of sequence
        /// characters per line.
        /// </summary>
        /// <param name="sender">btnMaxColumn instance.</param>
        /// <param name="e">Event data.</param>
        private void OnSizeChangedClicked(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlWait;
            this.ResetStatus();
            var dialog = new MaxColumnsDialog(maxNumberOfCharacters, this.alignAllSequenceSheet);
            var helper = new WindowInteropHelper(dialog);
            helper.Owner = (IntPtr)Globals.ThisAddIn.Application.Hwnd;
            dialog.Activated += this.OnWPFWindowActivated;
            if (dialog.Show())
            {
                maxNumberOfCharacters = dialog.MaxNumber;
            }
        }

        /// <summary>
        /// This method is called when the user wants to cancel a
        /// ongoing web service search.
        /// </summary>
        /// <param name="sender">Cancel search button</param>
        /// <param name="e">Event data</param>
        private void OnCancelSearchClicked(object sender, RibbonControlEventArgs e)
        {
            IBlastWebHandler blastServiceHandler = this.GetWebServiceInstance(this.webserviceName);
            //try
            //{
            //    blastServiceHandler.CancelRequest(this.requestIdentifier);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(
            //        String.Format(Resources.BLAST_CANCEL_FAILED, ex.Message),
            //        Resources.CAPTION,
            //        MessageBoxButtons.OK,
            //        MessageBoxIcon.Error);
            //}

            this.btnCancelSearch.Enabled = this.cancelSearchButtonState = false;
            this.ChangeStatusBar(string.Format(Resources.WEBSERVICE_STATUS_BAR, Resources.CANCELLED));
        }

        /// <summary>
        /// This method is called when the user wants to cancel a
        /// ongoing sequence alignment.
        /// </summary>
        /// <param name="sender">Cancel align button</param>
        /// <param name="e">Event data</param>
        private void OnCancelAlignClicked(object sender, RibbonControlEventArgs e)
        {
            if (this.alignerThread != null && this.alignerThread.IsBusy)
            {
                this.alignerThread.CancelAsync();
            }

            this.btnCancelAlign.Enabled = this.cancelAlignButtonState = false;
            this.ChangeStatusBar(string.Format(Resources.ALIGNMENT_STATUS_BAR, Resources.CANCELLED));
        }

        /// <summary>
        /// This method is called when the user wants to cancel a
        /// ongoing sequence assembly
        /// </summary>
        /// <param name="sender">Cancel align button</param>
        /// <param name="e">Event data</param>
        private void OnCancelAssembleClicked(object sender, RibbonControlEventArgs e)
        {
            if (this.assemblerThread != null && this.assemblerThread.IsBusy)
            {
                this.assemblerThread.CancelAsync();
            }

            this.btnCancelAssemble.Enabled = this.cancelAssemblyButtonState = false;
            this.ChangeStatusBar(string.Format(Resources.ASSEMBLER_STATUS_BAR, Resources.CANCELLED));
        }

        /// <summary>
        /// This method is called when the user wants to start a alignment operation.
        /// This method extracts the sequences present in the selected excel sheets
        /// and runs alignment on them.
        /// </summary>
        /// <param name="sender">Align button</param>
        /// <param name="e">Event data</param>
        private void OnAlignmentButtonClicked(object sender, RibbonControlEventArgs e)
        {
            this.ResetStatus();
            var aligner = (sender as RibbonButton).Tag as ISequenceAligner;

            var inputs = new InputSelection();
            inputs.MinimumSequenceCount = 2;
            if (aligner is IPairwiseSequenceAligner)
            {
                inputs.MaximumSequenceCount = 2;
            }

            inputs.GetInputSequences(this.DoAlignment, false, aligner);
        }

        /// <summary>
        /// Start an alignment on selected sequences
        /// </summary>
        /// <param name="selectedSequences">List of sequences selected</param>
        /// <param name="args">Any other arguments</param>
        private void DoAlignment(List<ISequence> selectedSequences, params object[] args)
        {
            this.btnCancelAlign.Enabled = true;
            this.cancelAlignButtonState = true;

            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlWait;

            var alignerDialog = new AssemblyInputDialog(
                true,
                selectedSequences[0].Alphabet,
                args[0] as ISequenceAligner);
            var assemblyInputHelper = new WindowInteropHelper(alignerDialog);
            assemblyInputHelper.Owner = (IntPtr)Globals.ThisAddIn.Application.Hwnd;
            alignerDialog.Activated += this.OnWPFWindowActivated;
            if (alignerDialog.Show())
            {
                AlignerInputEventArgs alignerInput = alignerDialog.GetAlignmentInput();
                if (alignerInput != null) // If fetching parameters were successful
                {
                    alignerInput.Sequences = selectedSequences;
                    alignerInput.Aligner = alignerDialog.Aligner;

                    this.ChangeStatusBar(string.Format(Resources.ALIGNMENT_STATUS_BAR, Resources.ALIGNING));

                    this.alignerThread = new BackgroundWorker();
                    this.alignerThread.WorkerSupportsCancellation = true;
                    this.alignerThread.DoWork += this.OnRunAlignerAlgorithm;
                    this.alignerThread.RunWorkerCompleted += this.OnAlignerCompleted;
                    this.alignerThread.RunWorkerAsync(alignerInput);
                }
            }
        }

        /// <summary>
        /// This method runs a specific alignment algorithm against a selected set of sequence(s).
        /// This is called by a BackgroundWorker thread.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">BackgroundWorker event args</param>
        private void OnRunAlignerAlgorithm(object sender, DoWorkEventArgs e)
        {
            var alignerInput = e.Argument as AlignerInputEventArgs;

            AssignAlignerParameter(alignerInput.Aligner, alignerInput);
            alignerInput.Aligner.GapOpenCost = alignerInput.GapCost;
            alignerInput.Aligner.GapExtensionCost = alignerInput.GapExtensionCost;

            if (alignerInput.SimilarityMatrix != null)
            {
                alignerInput.Aligner.SimilarityMatrix = alignerInput.SimilarityMatrix;
            }

            try
            {
                IList<ISequenceAlignment> alignedResult = alignerInput.Aligner.Align(alignerInput.Sequences);
                if (this.alignerThread.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                if (alignedResult.Count > 0)
                {
                    e.Result = Tuple.Create(alignedResult, alignerInput);
                }
                else
                {
                    e.Cancel = true;
                    MessageBox.Show(
                        Resources.NO_RESULT,
                        Resources.CAPTION,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    this.ResetStatus();
                }
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                MessageBox.Show(ex.Message, Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ChangeStatusBar(string.Format(Resources.ALIGNMENT_STATUS_BAR, Resources.ERROR));
            }
            finally
            {
                this.btnCancelAlign.Enabled = false;
                this.cancelAlignButtonState = false;
            }
        }

        /// <summary>
        /// Display the alignment result on sheet
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Results of alignment</param>
        private void OnAlignerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                return;
            }

            try
            {
                var results = e.Result as Tuple<IList<ISequenceAlignment>, AlignerInputEventArgs>;
                if (results.Item2.Aligner is IMultipleSequenceAligner)
                {
                    this.BuildMultipleAlignmentResultView(results);
                }
                else
                {
                    this.BuildAlignmentResultView(results);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.ChangeStatusBar(string.Format(Resources.ALIGNMENT_STATUS_BAR, Resources.DONE));
            }
        }

        /// <summary>
        /// This method displays the output of a pair-wise alignment or alignment of many sequences.
        /// </summary>
        /// <param name="results">Result of the alignment process.</param>
        private void BuildAlignmentResultView(Tuple<IList<ISequenceAlignment>, AlignerInputEventArgs> results)
        {
            IList<ISequenceAlignment> alignedResult = results.Item1;
            AlignerInputEventArgs alignerInput = results.Item2;

            if (alignedResult.Count == 0 || alignedResult[0].AlignedSequences.Count == 0)
            {
                MessageBox.Show(
                    Resources.NO_RESULT,
                    Resources.CAPTION,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            Workbook activeWorkBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            var activesheet = (Worksheet)Globals.ThisAddIn.Application.ActiveSheet;
            var currentsheet =
                (Worksheet)activeWorkBook.Worksheets.Add(Type.Missing, activesheet, Type.Missing, Type.Missing);

            ((_Worksheet)currentsheet).Activate();
            Globals.ThisAddIn.Application.ActiveWindow.Zoom = ZoomLevel;
            currentsheet.Name =
                this.GetValidFileNames(
                    Resources.Alignment_AlignedSequencesHeading
                    + this.currentAlignResultSheetNumber.ToString(CultureInfo.CurrentCulture));
            this.currentAlignResultSheetNumber++;

            int rowNumber = 1;

            Range header = currentsheet.get_Range("A" + rowNumber, Type.Missing);
            WriteRangeValue(header, Resources.ID);
            header.WrapText = true;

            header = currentsheet.get_Range("B" + rowNumber, Type.Missing);
            WriteRangeValue(header, Resources.Alignment_StartOffsetString);
            header.WrapText = true;

            header = currentsheet.get_Range("C" + rowNumber, Type.Missing);
            WriteRangeValue(header, Resources.Alignment_EndOffsetString);
            header.WrapText = true;
            currentsheet.Rows[rowNumber].Font.Bold = true;

            rowNumber++;

            foreach (ISequenceAlignment sequenceAlignment in alignedResult)
            {
                const int startingColumn = 4;

                foreach (IAlignedSequence alignedSequence in sequenceAlignment.AlignedSequences)
                {
                    int alignResultNumber = 1, sequenceNumber = 1;

                    // Get max length of all sequences selected
                    long numberofCharacters = 1;
                    long maxSequenceLength = alignedSequence.Sequences.Max(s => s.Count);

                    // calculate number of alphabets in one cell
                    if (maxSequenceLength > MaxExcelColumns)
                    {
                        numberofCharacters = maxSequenceLength % MaxExcelColumns == 0
                                                 ? maxSequenceLength / MaxExcelColumns
                                                 : 1 + (maxSequenceLength / MaxExcelColumns);
                    }

                    // write to sheet
                    int currentSequenceIndex = 0;
                    List<long> startOffsets = null;
                    List<long> endOffsets = null;

                    if (alignedSequence.Metadata.ContainsKey(StartOffsetString))
                    {
                        startOffsets = alignedSequence.Metadata[StartOffsetString] as List<long>;
                    }

                    if (startOffsets == null)
                    {
                        startOffsets = new List<long>();
                    }

                    if (alignedSequence.Metadata.ContainsKey(EndOffsetString))
                    {
                        endOffsets = alignedSequence.Metadata[EndOffsetString] as List<long>;
                    }

                    if (endOffsets == null)
                    {
                        endOffsets = new List<long>();
                    }

                    foreach (ISequence currSeq in alignedSequence.Sequences)
                    {
                        // Write the ID
                        header = currentsheet.get_Range("A" + rowNumber, Type.Missing);

                        string id = null;
                        if (!string.IsNullOrEmpty(currSeq.ID))
                        {
                            id = currSeq.ID;
                        }
                        else
                        {
                            id = alignerInput.Sequences[sequenceNumber - 1].ID;
                        }

                        if (string.IsNullOrEmpty(id))
                        {
                            id = Resources.Alignment_AlignedSequencesHeading + "_" + (currentSequenceIndex + 1);
                        }

                        // Attempt to link back to original value
                        object seqAddress;
                        if ((currSeq.Metadata.TryGetValue(Resources.EXCEL_CELL_LINK, out seqAddress)
                             && seqAddress != null)
                            || (alignerInput.Sequences[sequenceNumber - 1].Metadata.TryGetValue(
                                Resources.EXCEL_CELL_LINK,
                                out seqAddress) && seqAddress != null))
                        {
                            currentsheet.Hyperlinks.Add(
                                header,
                                string.Empty,
                                seqAddress,
                                "Jump to Original Sequence",
                                id);
                        }
                        else
                        {
                            WriteRangeValue(header, id);
                        }

                        // Write the start offset for this sequence
                        long startOffset = -1;
                        if (startOffsets.Count > currentSequenceIndex)
                        {
                            startOffset = startOffsets[currentSequenceIndex] + 1;
                            header = currentsheet.get_Range("B" + rowNumber, Type.Missing);
                            WriteRangeValue(header, startOffset.ToString());
                        }

                        // Write the end offset for this sequence
                        long endOffset = -1;
                        if (endOffsets.Count > currentSequenceIndex)
                        {
                            endOffset = endOffsets[currentSequenceIndex] + 1;
                            header = currentsheet.get_Range("C" + rowNumber, Type.Missing);
                            WriteRangeValue(header, endOffset.ToString());
                        }

                        // Build the data
                        var rangeData =
                            new string[1, maxSequenceLength > MaxExcelColumns ? MaxExcelColumns : maxSequenceLength];
                        int columnIndex = 0;

                        for (long i = 0; i < currSeq.Count; i += numberofCharacters, columnIndex++)
                        {
                            ISequence tempSeq = currSeq.GetSubSequence(i, numberofCharacters);
                            rangeData[0, columnIndex] = tempSeq.ConvertToString();
                        }

                        // dump to sheet
                        Range currentRange = currentsheet.get_Range(
                            GetColumnString(startingColumn) + rowNumber,
                            Type.Missing);
                        if (columnIndex > 1)
                        {
                            currentRange = currentRange.get_Resize(1, columnIndex);
                            currentRange.set_Value(Missing.Value, rangeData);
                            currentRange.Columns.AutoFit();
                            this.FillBackGroundColor(currentRange);

                            string rangeName = "AlignedSeq_" + alignResultNumber + "_" + currSeq.ID + "_"
                                               + sequenceNumber;
                            this.CreateNamedRange(
                                currentsheet,
                                currentRange,
                                rangeName,
                                startingColumn,
                                rowNumber,
                                columnIndex,
                                1);
                        }

                        rowNumber++;
                        sequenceNumber++;

                        currentSequenceIndex++;
                    }

                    rowNumber++; // blank line between alignments
                    alignResultNumber++;
                }
            }
            currentsheet.Columns.AutoFit();
        }

        /// <summary>
        /// This method displays the output of a multi-sequence alignment process.
        /// </summary>
        /// <param name="results">Result of the alignment process.</param>
        private void BuildMultipleAlignmentResultView(Tuple<IList<ISequenceAlignment>, AlignerInputEventArgs> results)
        {
            IList<ISequenceAlignment> alignedResult = results.Item1;
            AlignerInputEventArgs alignerInput = results.Item2;

            if (alignedResult.Count == 0 || alignedResult[0].AlignedSequences.Count == 0)
            {
                MessageBox.Show(
                    Resources.NO_RESULT,
                    Resources.CAPTION,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            Workbook activeWorkBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            var activesheet = (Worksheet)Globals.ThisAddIn.Application.ActiveSheet;
            var currentsheet =
                (Worksheet)activeWorkBook.Worksheets.Add(Type.Missing, activesheet, Type.Missing, Type.Missing);

            ((_Worksheet)currentsheet).Activate();
            Globals.ThisAddIn.Application.ActiveWindow.Zoom = ZoomLevel;
            currentsheet.Name =
                this.GetValidFileNames(
                    Resources.Alignment_AlignedSequencesHeading
                    + this.currentAlignResultSheetNumber.ToString(CultureInfo.CurrentCulture));
            this.currentAlignResultSheetNumber++;

            int rowNumber = 1;

            Range header = currentsheet.get_Range("A" + rowNumber, Type.Missing);
            WriteRangeValue(header, Resources.ID);
            header.WrapText = true;
            header.Font.Bold = true;

            rowNumber++;

            foreach (ISequenceAlignment sequenceAlignment in alignedResult)
            {
                const int startingColumn = 2;

                foreach (IAlignedSequence alignedSequence in sequenceAlignment.AlignedSequences)
                {
                    int alignResultNumber = 1, sequenceNumber = 1;

                    // Get max length of all sequences selected
                    long numberofCharacters = 1;
                    long maxSequenceLength = alignedSequence.Sequences.Max(s => s.Count);

                    // calculate number of alphabets in one cell
                    if (maxSequenceLength > MaxExcelColumns)
                    {
                        numberofCharacters = maxSequenceLength % MaxExcelColumns == 0
                                                 ? maxSequenceLength / MaxExcelColumns
                                                 : 1 + (maxSequenceLength / MaxExcelColumns);
                    }

                    // write to sheet
                    int currentSequenceIndex = 0;

                    foreach (ISequence currSeq in alignedSequence.Sequences)
                    {
                        // Write the ID
                        header = currentsheet.get_Range("A" + rowNumber, Type.Missing);

                        string id = null;
                        if (!string.IsNullOrEmpty(currSeq.ID))
                        {
                            id = currSeq.ID;
                        }
                        else
                        {
                            id = alignerInput.Sequences[sequenceNumber - 1].ID;
                        }

                        if (string.IsNullOrEmpty(id))
                        {
                            id = Resources.Alignment_AlignedSequencesHeading + "_" + (currentSequenceIndex + 1);
                        }

                        // Attempt to link back to original value
                        object seqAddress;
                        if ((currSeq.Metadata.TryGetValue(Resources.EXCEL_CELL_LINK, out seqAddress)
                             && seqAddress != null)
                            || (alignerInput.Sequences[sequenceNumber - 1].Metadata.TryGetValue(
                                Resources.EXCEL_CELL_LINK,
                                out seqAddress) && seqAddress != null))
                        {
                            currentsheet.Hyperlinks.Add(
                                header,
                                string.Empty,
                                seqAddress,
                                "Jump to Original Sequence",
                                id);
                        }
                        else
                        {
                            WriteRangeValue(header, id);
                        }

                        // Build the data
                        var rangeData =
                            new string[1, maxSequenceLength > MaxExcelColumns ? MaxExcelColumns : maxSequenceLength];

                        int columnIndex = 0;

                        for (long i = 0; i < currSeq.Count; i += numberofCharacters, columnIndex++)
                        {
                            ISequence tempSeq = currSeq.GetSubSequence(i, numberofCharacters);
                            rangeData[0, columnIndex] = tempSeq.ConvertToString();
                        }

                        // dump to sheet
                        Range currentRange = currentsheet.get_Range(
                            GetColumnString(startingColumn) + rowNumber,
                            Type.Missing);
                        if (columnIndex > 1)
                        {
                            currentRange = currentRange.get_Resize(1, columnIndex);
                            currentRange.set_Value(Missing.Value, rangeData);
                            currentRange.Columns.AutoFit();
                            this.FillBackGroundColor(currentRange);

                            string rangeName = "AlignedSeq_" + alignResultNumber + "_" + currSeq.ID + "_"
                                               + sequenceNumber;
                            this.CreateNamedRange(
                                currentsheet,
                                currentRange,
                                rangeName,
                                startingColumn,
                                rowNumber,
                                columnIndex,
                                1);
                        }

                        rowNumber++;
                        sequenceNumber++;

                        currentSequenceIndex++;
                    }

                    rowNumber += 2;
                    alignResultNumber++;
                }
            }
            currentsheet.Columns.AutoFit();
        }

        /// <summary>
        /// Create a named range with the specified parameters
        /// </summary>
        /// <param name="currentsheet">Sheet in which name range has to be created</param>
        /// <param name="currentRange">Range to be named</param>
        /// <param name="rangeName">Name to be given</param>
        /// <param name="startingCol">Starting column</param>
        /// <param name="startingRow">Starting row</param>
        /// <param name="cols">Total number of colums</param>
        /// <param name="rows">Total number of rows</param>
        private void CreateNamedRange(
            Worksheet currentsheet,
            Range currentRange,
            string rangeName,
            int startingCol,
            int startingRow,
            int cols,
            int rows)
        {
            var formulaBuilder = new StringBuilder();
            formulaBuilder.Append("=");
            formulaBuilder.Append(currentsheet.Name);
            formulaBuilder.Append("!$");
            formulaBuilder.Append(GetColumnString(startingCol) + "$" + startingRow);
            formulaBuilder.Append(":$");
            formulaBuilder.Append(GetColumnString(startingCol + (cols - 1)) + "$" + (startingRow + (rows - 1)));
            string sequenceFormula = formulaBuilder.ToString();

            currentsheet.Names.Add(
                this.GetValidFileNames(rangeName),
                sequenceFormula,
                true,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing);
        }

        /// <summary>
        /// This method is called when the user wants to start an assembler operation.
        /// </summary>
        private void OnAssembleClick(object sender, RibbonControlEventArgs e)
        {
            this.ResetStatus();

            var inputs = new InputSelection();
            inputs.MinimumSequenceCount = 2;
            inputs.GetInputSequences(this.DoAssembly, false);
        }

        /// <summary>
        /// Callback method from input selection model which will actually do the selected operation
        /// </summary>
        /// <param name="selectedSequences">List of sequences depending on the user selections made</param>
        /// <param name="args">Any arguments passed when calling the selection model</param>
        private void DoAssembly(List<ISequence> selectedSequences, params object[] args)
        {
            // Verify none of the sequences are protein - we cannot assembly these because we 
            // cannot align them (no complements).
            if (selectedSequences.Any(s => s.Alphabet is ProteinAlphabet))
            {
                MessageBox.Show(
                    Resources.PROTEIN_NOT_SUPPORTED,
                    Resources.CAPTION,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlWait;
            var assemblerDialog = new AssemblyInputDialog(false, selectedSequences[0].Alphabet);
            var assemblyInputHelper = new WindowInteropHelper(assemblerDialog);
            assemblyInputHelper.Owner = (IntPtr)Globals.ThisAddIn.Application.Hwnd;
            assemblerDialog.Activated += this.OnWPFWindowActivated;
            if (assemblerDialog.Show())
            {
                var eventArgs = new AssemblyInputEventArgs(selectedSequences, assemblerDialog.Aligner);
                eventArgs.ConsensusThreshold = assemblerDialog.ConsensusThreshold;
                eventArgs.MatchScore = assemblerDialog.MatchScore;
                eventArgs.MergeThreshold = assemblerDialog.MergeThreshold;
                eventArgs.MismatchScore = assemblerDialog.MisMatchScore;

                eventArgs.AlignerInput = assemblerDialog.GetAlignmentInput();

                if (eventArgs.AlignerInput != null) // If fetching parameters were successful
                {
                    this.OnRunAssemblerAlgorithm(eventArgs);
                }
            }
        }

        /// <summary>
        /// This method is called when the user wants to use NCBI\EBI databases
        /// for a search operation. This methos pops-up a UI where the user
        /// can configure the parameters required to run NCBI\EBI.
        /// </summary>
        /// <param name="sender">ExecuteBlast button.</param>
        /// <param name="e">Event data</param>
        private void OnExecuteBlastSearch(object sender, RibbonControlEventArgs e)
        {
            this.ResetStatus();
            this.webserviceName = (sender as RibbonButton).Tag as string;

            var inputs = new InputSelection();
            inputs.SequenceLabels = new[] { Resources.InputSelection_SequenceLabel_Blast };
            inputs.MinimumSequenceCount = 1;
            inputs.GetInputSequences(this.OnExecuteSearch, false);
        }

        /// <summary>
        /// This method is called when the user wants to import a Range-Sequence file
        /// into excel. The user chooses a particular Sequence file which will
        /// be parsed by parsers available in our framework and is then imported
        /// into a excel file.
        /// </summary>
        /// <param name="parser">SequenceRangeParser instance.</param>
        /// <param name="fileName">Name of the file</param>
        private void ReadRangeSequence(ISequenceRangeParser parser, string fileName)
        {
            SequenceRangeGrouping rangeGroup = parser.ParseRangeGrouping(fileName);
            List<ISequenceRange> sequences = rangeGroup.Flatten();
            if (sequences == null || sequences.Count == 0)
            {
                string strMessage = string.Format(Resources.PARSE_ERROR, Path.GetFileName(fileName), string.Empty);
                strMessage = strMessage.TrimEnd(" :".ToCharArray());
                MessageBox.Show(strMessage, Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Workbook workBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            var workSheet =
                workBook.Worksheets.Add(
                    Type.Missing,
                    workBook.Worksheets.get_Item(workBook.Worksheets.Count),
                    Type.Missing,
                    Type.Missing) as Worksheet;
            ((_Worksheet)workSheet).Activate();
            Globals.ThisAddIn.Application.ActiveWindow.Zoom = ZoomLevel;

            string validName = this.GetValidFileNames(Path.GetFileNameWithoutExtension(fileName));
            workSheet.Name = validName;

            int rowNumber = 2;
            int initialRowNumber = rowNumber;
            int columnNumber = 2;
            int maxColumnNumber = 13;

            foreach (string header in rangeHeaders)
            {
                Range range = workSheet.get_Range(GetColumnString(columnNumber) + rowNumber, Type.Missing);
                WriteRangeValue(range, header);
                columnNumber++;
            }

            rowNumber++;
            columnNumber = 2;

            Range activeSelectedRange = workSheet.get_Range(GetColumnString(columnNumber) + rowNumber, Missing.Value);
            activeSelectedRange = activeSelectedRange.get_Resize(sequences.Count, 12);
            var values = new object[sequences.Count, 12];

            for (int i = 0; i < sequences.Count; i++)
            {
                ISequenceRange range = sequences[i];
                values[i, 0] = range.ID;
                values[i, 1] = range.Start;
                values[i, 2] = range.End;
                values[i, 3] = ExtractRangeMetadata(range, Resources.BED_NAME);
                values[i, 4] = ExtractRangeMetadata(range, Resources.BED_SCORE);

                object value = ExtractRangeMetadata(range, Resources.BED_STRAND);
                if (value != null)
                {
                    values[i, 5] = value.ToString();
                }

                values[i, 6] = ExtractRangeMetadata(range, Resources.BED_THICK_START);
                values[i, 7] = ExtractRangeMetadata(range, Resources.BED_THICK_END);
                values[i, 8] = ExtractRangeMetadata(range, Resources.BED_ITEM_RGB);
                values[i, 9] = ExtractRangeMetadata(range, Resources.BED_BLOCK_COUNT);
                var strValue = ExtractRangeMetadata(range, Resources.BED_BLOCK_SIZES) as string;

                // As excel is not handling more than 4000 chars in a single cell.
                if (strValue != null && strValue.Length > 4000)
                {
                    strValue = strValue.Substring(0, 4000);
                }

                values[i, 10] = strValue;

                strValue = ExtractRangeMetadata(range, Resources.BED_BLOCK_STARTS) as string;

                // As excel is not handling more than 4000 chars in a single cell.
                if (strValue != null && strValue.Length > 4000)
                {
                    strValue = strValue.Substring(0, 4000);
                }

                values[i, 11] = strValue;

                rowNumber++;
            }

            activeSelectedRange.set_Value(Missing.Value, values);

            workSheet.Columns.AutoFit();
            this.NormalizeColumWidths(workSheet.UsedRange);
            this.EnableAllControls();

            var sb = new StringBuilder();
            sb.Append("='");
            sb.Append(workSheet.Name);
            sb.Append("'!");
            sb.Append("$");
            sb.Append(GetColumnString(columnNumber) + "$" + initialRowNumber);
            sb.Append(":$");
            sb.Append(GetColumnString(maxColumnNumber) + "$" + (rowNumber - 1).ToString(CultureInfo.CurrentCulture));
            string formula = sb.ToString();

            Name dataWithHeader = workSheet.Names.Add(
                Resources.SEQUENCERANGEDATA_PRESELECTION + workSheet.Name,
                formula,
                true,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing,
                Type.Missing);
            dataWithHeader.RefersToRange.Select();
            //added default from UI as auto detect and ignore space
            SequenceCache.Add(activeSelectedRange, rangeGroup, "_Auto Detect_NB");
        }

        /// <summary>
        /// This method is called when the user wants to import a Sequence file
        /// or a Query region file into excel.
        /// </summary>
        /// <param name="sender">Import button.</param>
        /// <param name="e">Event data</param>
        private void ReadSequenceFiles(object sender, RibbonControlEventArgs e)
        {
            var parser = (sender as RibbonButton).Tag as IParser;
            if (parser == null)
            {
                return;
            }

            this.ResetStatus();
            var openFileDialog1 = new OpenFileDialog
                                  {
                                      Multiselect = true,
                                      Filter =
                                          string.Format(
                                              "{0} ({1})|{1}|All Files (*.*)|*.*",
                                              parser.Name,
                                              parser.SupportedFileTypes.Replace(".", "*.")
                                          .Replace(',', ';'))
                                  };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                int currentRow = 1;
                foreach (string fileName in openFileDialog1.FileNames)
                {
                    try
                    {
                        this.ScreenUpdate(false);

                        var sequenceParser = parser as ISequenceParser;
                        if (sequenceParser != null)
                        {
                            try
                            {
                                sequenceParser.Open(fileName);
                                this.ReadSequences(sequenceParser, fileName, ref currentRow);
                            }
                            finally
                            {
                                sequenceParser.Close();
                            }
                        }
                        else
                        {
                            var rangeParser = parser as ISequenceRangeParser;
                            this.ReadRangeSequence(rangeParser, fileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            string.Format(Resources.PARSE_ERROR, Path.GetFileName(fileName), ex.Message),
                            Resources.CAPTION,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    finally
                    {
                        this.ScreenUpdate(true);
                    }
                }
            }
        }

        /// <summary>
        /// This method is called when the user wants to import a Sequence file
        /// into excel. The user chooses a particular Sequence file which will
        /// be parsed by parsers available in our framework and is then imported
        /// into a excel file.
        /// </summary>
        /// <param name="parser">SequenceParser instance.</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="currentRow">Current row of insertion</param>
        private void ReadSequences(ISequenceParser parser, string fileName, ref int currentRow)
        {
            this.ScreenUpdate(false);

            if (this.sequencesPerWorksheet == 1)
            {
                this.ImportSequencesOnePerSheet(parser, fileName);
            }
            else if (this.sequencesPerWorksheet <= 0)
            {
                this.ImportSequencesAllInOneSheet(parser, fileName, ref currentRow);
            }
            else if (this.sequencesPerWorksheet > 0)
            {
                this.ImportSequencesAcrossSheets(parser, fileName, ref currentRow);
            }

            this.ScreenUpdate(true);
        }

        /// <summary>
        /// This method breaks the sequences across multiple worksheets.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="fileName"></param>
        /// <param name="currentRow"></param>
        private void ImportSequencesAcrossSheets(ISequenceParser parser, string fileName, ref int currentRow)
        {
            Workbook workBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            int sequenceCount = 0;
            Worksheet worksheet = null;

            Globals.ThisAddIn.Application.EnableEvents = false;

            try
            {
                foreach (ISequence sequence in parser.Parse())
                {
                    if (worksheet == null || sequenceCount++ >= this.sequencesPerWorksheet)
                    {
                        if (worksheet != null)
                        {
                            worksheet.Cells[1, 1].EntireColumn.AutoFit(); // Autofit first column
                        }

                        currentRow = 1;
                        sequenceCount = 1;
                        worksheet =
                            workBook.Worksheets.Add(
                                Type.Missing,
                                workBook.Worksheets.Item[workBook.Worksheets.Count],
                                Type.Missing,
                                Type.Missing) as Worksheet;
                        if (worksheet == null)
                        {
                            return;
                        }

                        // Get a name for the worksheet.
                        string validName =
                            this.GetValidFileNames(
                                string.IsNullOrEmpty(sequence.ID)
                                    ? Path.GetFileNameWithoutExtension(fileName)
                                    : sequence.ID);
                        worksheet.Name = validName;
                        ((_Worksheet)worksheet).Activate();
                        Globals.ThisAddIn.Application.ActiveWindow.Zoom = ZoomLevel;
                    }

                    // If sequence ID cannot be used as a sheet name, update the sequence DisplayID with the string used as sheet name.
                    if (string.IsNullOrEmpty(sequence.ID))
                    {
                        sequence.ID = Path.GetFileNameWithoutExtension(fileName) + "_" + sequenceCount;
                    }

                    this.WriteOneSequenceToWorksheet(parser, ref currentRow, sequence, worksheet);
                }

                if (worksheet != null)
                {
                    worksheet.Cells[1, 1].EntireColumn.AutoFit(); // Autofit first column
                }
            }
            finally
            {
                this.EnableAllControls();
                Globals.ThisAddIn.Application.EnableEvents = true;
            }
        }

        /// <summary>
        /// This method imports a set of sequences, one per worksheet.
        /// </summary>
        /// <param name="parser">SequenceParser instance.</param>
        /// <param name="fileName">Name of the file</param>
        private void ImportSequencesOnePerSheet(ISequenceParser parser, string fileName)
        {
            Workbook workBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            foreach (ISequence sequence in parser.Parse())
            {
                var worksheet =
                    workBook.Worksheets.Add(
                        Type.Missing,
                        workBook.Worksheets.Item[workBook.Worksheets.Count],
                        Type.Missing,
                        Type.Missing) as Worksheet;
                if (worksheet == null)
                {
                    return;
                }

                string validName =
                    this.GetValidFileNames(
                        string.IsNullOrEmpty(sequence.ID) ? Path.GetFileNameWithoutExtension(fileName) : sequence.ID);

                // If sequence ID cannot be used as a sheet name, update the sequence DisplayID with the string used as sheet name.
                if (string.IsNullOrEmpty(sequence.ID))
                {
                    sequence.ID = validName;
                }

                worksheet.Name = validName;
                ((_Worksheet)worksheet).Activate();
                Globals.ThisAddIn.Application.ActiveWindow.Zoom = ZoomLevel;
                Globals.ThisAddIn.Application.EnableEvents = false;

                try
                {
                    int currentRow = 1;
                    this.WriteOneSequenceToWorksheet(parser, ref currentRow, sequence, worksheet);
                    this.currentFileNumber++;
                }
                finally
                {
                    worksheet.Cells[1, 1].EntireColumn.AutoFit(); // Autofit first column
                    this.EnableAllControls();
                    Globals.ThisAddIn.Application.EnableEvents = true;
                }
            }
        }

        /// <summary>
        /// This method imports a set of sequences, one sequence per row.
        /// </summary>
        /// <param name="parser">SequenceParser instance.</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="currentRow">Next row of insertion</param>
        private void ImportSequencesAllInOneSheet(ISequenceParser parser, string fileName, ref int currentRow)
        {
            Workbook workBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            var worksheet =
                workBook.Worksheets.Add(
                    Type.Missing,
                    workBook.Worksheets.Item[workBook.Worksheets.Count],
                    Type.Missing,
                    Type.Missing) as Worksheet;
            worksheet.Name = this.GetValidFileNames(Path.GetFileNameWithoutExtension(fileName));
            ((_Worksheet)worksheet).Activate();
            Globals.ThisAddIn.Application.ActiveWindow.Zoom = ZoomLevel;
            Globals.ThisAddIn.Application.EnableEvents = false;

            try
            {
                int sequenceCount = 0;
                foreach (ISequence sequence in parser.Parse())
                {
                    sequenceCount++;
                    if (string.IsNullOrEmpty(sequence.ID))
                    {
                        sequence.ID = Path.GetFileNameWithoutExtension(fileName) + "_" + sequenceCount;
                    }
                    this.WriteOneSequenceToWorksheet(parser, ref currentRow, sequence, worksheet);
                    this.currentFileNumber++;
                }
            }
            finally
            {
                worksheet.Cells[1, 1].EntireColumn.AutoFit(); // Autofit first column
                this.EnableAllControls();
                Globals.ThisAddIn.Application.EnableEvents = true;
            }
        }

        /// <summary>
        /// This writes out a single sequence with header, data, quality scores and metadata into the worksheet.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="currentRow"></param>
        /// <param name="sequence"></param>
        /// <param name="worksheet"></param>
        private void WriteOneSequenceToWorksheet(
            ISequenceParser parser,
            ref int currentRow,
            ISequence sequence,
            Worksheet worksheet)
        {
            // Write the header (sequence id)
            if (!string.IsNullOrEmpty(sequence.ID))
            {
                string[,] formattedMetadata = ExcelImportFormatter.SequenceIDHeaderToRange(sequence);
                Range range = this.WriteToSheet(worksheet, formattedMetadata, currentRow, 1);
                if (range != null)
                {
                    range.WrapText = false;
                    currentRow += range.Rows.Count;
                }
            }

            // Write out the data
            Range dataRange;
            if (sequence.Count > 0)
            {
                currentRow = this.WriteSequence(worksheet, sequence, currentRow, out dataRange);
                if (dataRange != null)
                {
                    dataRange.Columns.AutoFit(); // Autofit columns with sequence data
                }
            }

            // Write quality values if file is FastQ
            if (sequence is QualitativeSequence)
            {
                currentRow++;
                worksheet.Cells[currentRow, 1].Value2 = Resources.Sequence_QualityScores;
                currentRow = this.WriteQualityValues(
                    sequence as QualitativeSequence,
                    worksheet,
                    currentRow,
                    3,
                    out dataRange);
                if (dataRange != null)
                {
                    dataRange.Columns.AutoFit(); // Autofit columns with quality scores
                }
            }

            // Write out the metadata 
            Range metadataRange;
            this.WriteMetadata(sequence, parser, worksheet, currentRow, out metadataRange);
            if (metadataRange != null)
            {
                currentRow += metadataRange.Rows.Count;
            }

            currentRow++; // Add space row between sequences
        }

        /// <summary>
        /// Formats the metadata depending on the sequence type and displays it.
        /// </summary>
        /// <param name="sequence">Sequence which is holding the metadata</param>
        /// <param name="parserUsed">Parser used to read data</param>
        /// <param name="worksheet">Sheet on to which the metadata should be written.</param>
        /// <param name="startingRow">Row we are on</param>
        /// <param name="metadataRange">Will have the range to which the metadata was written</param>
        private void WriteMetadata(
            ISequence sequence,
            ISequenceParser parserUsed,
            Worksheet worksheet,
            int startingRow,
            out Range metadataRange)
        {
            if (parserUsed is GenBankParser)
            {
                if (sequence.Metadata.ContainsKey(GenbankMetadataKey))
                {
                    string[,] formattedMetadata =
                        ExcelImportFormatter.GenBankMetadataToRange(
                            sequence.Metadata[GenbankMetadataKey] as GenBankMetadata);
                    metadataRange = this.WriteToSheet(worksheet, formattedMetadata, startingRow, 1);
                    if (metadataRange != null)
                    {
                        // Turn off wrapping for metadata values.
                        metadataRange.WrapText = false;
                    }
                }
                else
                {
                    metadataRange = null;
                }
            }
            else if (parserUsed is GffParser)
            {
                string[,] formattedMetadata = ExcelImportFormatter.GffMetaDataToRange(sequence);
                metadataRange = this.WriteToSheet(worksheet, formattedMetadata, startingRow, 1);
                if (metadataRange != null)
                {
                    metadataRange.Columns.VerticalAlignment = XlVAlign.xlVAlignTop;
                    metadataRange.WrapText = false;
                    // Now wrap data columns
                    metadataRange.Columns[2].AutoFit();
                    metadataRange.Columns[2].WrapText = true;
                    metadataRange.Columns[2].AutoFit();
                }
            }
            else
            {
                metadataRange = null;
                // No error on this.
                //MessageBox.Show(Resources.MetadataFormatError, Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// Write quality scores to the sheet if its a FastQ file
        /// </summary>
        /// <param name="sequence">Sequence of which the quality scores should be written</param>
        /// <param name="worksheet">Worksheet to which to write</param>
        /// <param name="startingRow">Starting row</param>
        /// <param name="startingColumn">Column we wrote to</param>
        /// <param name="dataRange">Range where quality values were written</param>
        /// <returns>Index of row after last written row</returns>
        private int WriteQualityValues(
            QualitativeSequence sequence,
            Worksheet worksheet,
            int startingRow,
            int startingColumn,
            out Range dataRange)
        {
            string[,] qualityScores = ExcelImportFormatter.FastQQualityValuesToRange(sequence, maxNumberOfCharacters);
            dataRange = this.WriteToSheet(worksheet, qualityScores, startingRow, startingColumn);

            return startingRow + qualityScores.GetLength(0) + 1;
        }

        /// <summary>
        /// Writes the given array to a worksheet
        /// </summary>
        /// <param name="worksheet">Sheet to which to write to</param>
        /// <param name="data">Data to be written</param>
        /// <param name="row">Starting row</param>
        /// <param name="col">Starting col</param>
        private Range WriteToSheet(Worksheet worksheet, string[,] data, int row, int col)
        {
            Range range = worksheet.Range[GetColumnString(col) + (row), Type.Missing];
            range = range.Resize[data.GetLength(0), data.GetLength(1)];
            range.set_Value(Missing.Value, data);

            return range;
        }

        /// <summary>
        /// This method writes a sequence to a given worksheet.
        /// </summary>
        /// <param name="worksheet">The worksheet instance</param>
        /// <param name="sequence">The sequence which has to be imported into the excel sheet.</param>
        /// <param name="initialRowNumber">Initial row number</param>
        /// <param name="sequenceDataRange">The row number from where the sequence rendering has to begin</param>
        /// <returns>Index of last row where sequence data was written</returns>
        private int WriteSequence(
            Worksheet worksheet,
            ISequence sequence,
            int initialRowNumber,
            out Range sequenceDataRange)
        {
            Range heading = worksheet.Range["A" + initialRowNumber.ToString(CultureInfo.CurrentCulture), Type.Missing];
            WriteRangeValue(heading, Resources.SEQUENCE_DATA);

            return this.WriteSequenceDataIntoSheet(
                worksheet,
                sequence,
                initialRowNumber,
                3,
                Resources.SEQUENCEDATA_PRESELECTION + this.GetValidFileNames(sequence.ID),
                out sequenceDataRange);
        }

        /// <summary>
        /// This method writes a sequence to a given worksheet.
        /// </summary>
        /// <param name="worksheet">The worksheet instance</param>
        /// <param name="sequence">The sequence which has to be imported into the excel sheet.</param>
        /// <param name="initialRowNumber">Initial row number</param>
        /// <param name="initialColumnNumber">Initial column number</param>
        /// <param name="rangeName">Name to use for range</param>
        /// <param name="sequenceDataRange">The row number from where the sequence rendering has to begin</param>
        /// <returns>Index of last row where sequence data was written</returns>
        private int WriteSequenceDataIntoSheet(
            Worksheet worksheet,
            ISequence sequence,
            int initialRowNumber,
            int initialColumnNumber,
            string rangeName,
            out Range sequenceDataRange)
        {
            int counts = 0;
            int maxColumnNumber = 0;

            string columnPos = GetColumnString(initialColumnNumber);

            int rowNumber = initialRowNumber;
            var rowCount = (int)Math.Ceiling((decimal)sequence.Count / maxNumberOfCharacters);
            long columnCount = sequence.Count > maxNumberOfCharacters ? maxNumberOfCharacters : sequence.Count;
            var rangeData = new string[rowCount, columnCount];

            // Put the data into the rows.
            while (counts < sequence.Count)
            {
                int columnNumber = initialColumnNumber - 1;
                for (int i = 0; (i < maxNumberOfCharacters) && (counts < sequence.Count); i++, counts++, columnNumber++)
                {
                    rangeData[rowNumber - initialRowNumber, i] = new string(new[] { (char)sequence[counts] });
                }

                if (columnNumber > maxColumnNumber)
                {
                    maxColumnNumber = columnNumber;
                }

                rowNumber++;
            }

            if (sequence.Count > 0)
            {
                Range range =
                    worksheet.Range[columnPos + initialRowNumber.ToString(CultureInfo.CurrentCulture), Type.Missing];
                range = range.Resize[rowCount, columnCount];
                range.set_Value(Missing.Value, rangeData);

                this.FillBackGroundColor(range);

                // Add a range name to this selected sequence.
                if (!string.IsNullOrEmpty(rangeName))
                {
                    var sb = new StringBuilder();
                    sb.Append("='");
                    sb.Append(worksheet.Name);
                    sb.Append("'!");
                    sb.Append("$" + columnPos + "$" + initialRowNumber);
                    sb.Append(":$");
                    sb.Append(
                        GetColumnString(maxColumnNumber) + "$" + (rowNumber - 1).ToString(CultureInfo.CurrentCulture));
                    string formula = sb.ToString();

                    worksheet.Names.Add(
                        rangeName,
                        formula,
                        true,
                        Type.Missing,
                        Type.Missing,
                        Type.Missing,
                        Type.Missing,
                        Type.Missing,
                        Type.Missing,
                        Type.Missing,
                        Type.Missing);
                }

                Range fullSelection =
                    worksheet.Range[
                        columnPos + initialRowNumber.ToString(CultureInfo.CurrentCulture),
                        GetColumnString(maxColumnNumber) + (rowNumber - 1).ToString(CultureInfo.CurrentCulture)];
                fullSelection.Select();
                sequenceDataRange = fullSelection;
                //added default from UI as auto detect and ignore space
                SequenceCache.Add(fullSelection, sequence, "_Auto Detect_NB");
            }
            else
            {
                sequenceDataRange = null;
            }

            return rowNumber;
        }

        /// <summary>
        /// Changes the status bar to reflect the status of the alignment and web-service.
        /// </summary>
        /// <param name="status">Status to be displayed.</param>
        private void ChangeStatusBar(string status)
        {
            bool isCompleted = false;
            int retryCount = 1;

            while (!isCompleted && retryCount <= MaxRetryCount)
            {
                try
                {
                    Globals.ThisAddIn.Application.StatusBar = status;
                    isCompleted = true;
                }
                catch (COMException ex)
                {
                    if (VbaIgnoreErrorCode == ex.ErrorCode && retryCount < MaxRetryCount)
                    {
                        Thread.Sleep(RetryInterval * retryCount);
                        retryCount++;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Enables or disables the screen update
        /// Retry the operation till it succeeds / all retries times out.
        /// </summary>
        /// <param name="isEnable">Is enabled</param>
        private void ScreenUpdate(bool isEnable)
        {
            bool isCompleted = false;
            int retryCount = 1;

            while (!isCompleted && retryCount <= MaxRetryCount)
            {
                try
                {
                    Globals.ThisAddIn.Application.ScreenUpdating = isEnable;
                    isCompleted = true;
                }
                catch (COMException ex)
                {
                    if (VbaIgnoreErrorCode == ex.ErrorCode && retryCount < MaxRetryCount)
                    {
                        Thread.Sleep(RetryInterval * retryCount);
                        retryCount++;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the instance of IWebService depending on the service name.
        /// </summary>
        /// <param name="webserviceName">Name of the webservice</param>
        /// <returns>Instance of the web service</returns>
        private IBlastWebHandler GetWebServiceInstance(string webserviceName)
        {
            //foreach (IBlastServiceHandler blastService in
            //    WebServices.All.Where(service => service is IBlastServiceHandler))
            //{
            //    if (blastService.Name.Equals(this.webserviceName))
            //    {
            //        return blastService;
            //    }
            //}

            return null;
        }

        /// <summary>
        /// This event is triggered when any of the buttons in ribbon is clicked.
        /// </summary>
        private void RibbonControl_Click(object sender, RibbonControlEventArgs e)
        {
            this.ResetStatus();
        }

        /// <summary>
        /// Reset the status to Ready
        /// </summary>
        private void ResetStatus()
        {
            this.ChangeStatusBar(Resources.STATUS_READY);
        }

        private void btnUserManual_Click(object sender, RibbonControlEventArgs e)
        {
            string userGuidePath = MBFInstallationPath + Resources.UserGuideRelativePath;

            if (File.Exists(userGuidePath))
            {
                Process.Start(userGuidePath);
            }
            else
            {
                MessageBox.Show(Resources.NoUserGuidePresent, Resources.CAPTION, MessageBoxButtons.OK);
            }
        }

        #region Export options

        /// <summary>
        /// Export data from sheets to a particulat sequence file format
        /// </summary>
        private void OnExportClick(object sender, RibbonControlEventArgs e)
        {
            var formatter = ((sender as RibbonButton).Tag as ISequenceFormatter);
            if (formatter is FastAFormatter || formatter is FastQFormatter || formatter is GenBankFormatter
                || formatter is GffFormatter)
            {
                var sequenceSelection = new InputSelection();

                if (formatter is GenBankFormatter)
                {
                    sequenceSelection.MaximumSequenceCount = 1;
                    sequenceSelection.MinimumSequenceCount = 1;
                }

                sequenceSelection.GetSequencesForExport(this.DoExportSequence, formatter);
            }
            else
            {
                // as its not a ISequenceFormatter try to cast it to ISequenceRangeFormatter
                var rangeformatter = ((sender as RibbonButton).Tag as ISequenceRangeFormatter);

                if (rangeformatter is ISequenceRangeFormatter)
                {
                    var sequenceSelection = new InputSelection();
                    sequenceSelection.SequenceLabels = new[] { Resources.Export_BED_SequenceRangeString };
                    sequenceSelection.MaximumSequenceCount = 1;
                    sequenceSelection.PromptForSequenceName = false;
                    sequenceSelection.GetInputSequenceRanges(
                        this.DoExportRangeSequence,
                        false,
                        false,
                        false,
                        rangeformatter);
                }
            }
        }

        /// <summary>
        /// Method which will export a parsed BED file
        /// </summary>
        /// <param name="e">Event Arguments</param>
        private void DoExportRangeSequence(InputSequenceRangeSelectionEventArg e)
        {
            Stream sequenceWriter = null;
            //todo: Below line used hard coded BEDFormatter, have to get it from args later on
            ISequenceRangeFormatter formatter = new BedFormatter();
            var saveDialog = new SaveFileDialog { Filter = "BED Files|*.BED" };

            try
            {
                if (e.Sequences[0].GroupIDs.Any())
                {
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Get a textwriter to the file which will append text to it.
                        sequenceWriter = File.OpenWrite(saveDialog.FileName);
                        formatter.Format(sequenceWriter, e.Sequences[0]);
                    }
                }
                else
                {
                    MessageBox.Show(
                        Resources.EMPTY_SEQUENCE_RANGE,
                        Resources.CAPTION,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sequenceWriter != null)
                {
                    sequenceWriter.Close();
                }
            }
        }

        /// <summary>
        /// Method which will export the parsed data to the appropriate file format
        /// </summary>
        /// <param name="sequences"></param>
        /// <param name="args"></param>
        private void DoExportSequence(List<ISequence> sequences, params object[] args)
        {
            bool openedFormatter = false;
            var formatter = args[0] as ISequenceFormatter;
            var saveDialog = new SaveFileDialog();

            if (formatter is FastAFormatter)
            {
                saveDialog.Filter = "FastA Files|*.FastA";
            }
            else if (formatter is FastQFormatter)
            {
                saveDialog.Filter = "FastQ Files|*.FastQ";
            }
            else if (formatter is GenBankFormatter)
            {
                saveDialog.Filter = "GenBank Files|*.gbk";
            }
            else if (formatter is GffFormatter)
            {
                saveDialog.Filter = "Gff Files|*.gff";
            }
            try
            {
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Remove file is exists
                    if (File.Exists(saveDialog.FileName))
                    {
                        File.Delete(saveDialog.FileName);
                    }

                    // Get a textwriter to the file which will append text to it.
                    formatter.Open(saveDialog.FileName);
                    openedFormatter = true;

                    // Check the formatter chosen, Loop through if there are multiple sequences selected and append to the file
                    if (formatter is FastAFormatter || formatter is FastQFormatter || formatter is GenBankFormatter)
                    {
                        foreach (ISequence currentSequence in sequences)
                        {
                            formatter.Format(currentSequence);
                        }
                    }
                    else if (formatter is GffFormatter)
                    {
                        var gffFormatter = formatter as GffFormatter;
                        if (gffFormatter != null)
                        {
                            gffFormatter.ShouldWriteSequenceData = true;
                        }

                        foreach (ISequence currentSequence in sequences)
                        {
                            formatter.Format(currentSequence);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (openedFormatter)
                {
                    formatter.Close();
                }
            }
        }

        #endregion

        #region -- Web Service --

        /// <summary>
        /// This method is called when the user prompts for a sequence search on
        /// the NCBI database. This method creates a type of web-service object
        /// and passes parameters to it. And waits for the result.
        /// </summary>
        /// <param name="sequences">IAssembler instance</param>
        /// <param name="args">Event data.</param>
        private void OnExecuteSearch(List<ISequence> sequences, params object[] args)
        {
            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlWait;
            var dialog = new BlastDialog(this.webserviceName);
            var helper = new WindowInteropHelper(dialog);
            helper.Owner = (IntPtr)Globals.ThisAddIn.Application.Hwnd;
            dialog.Activated += this.OnWPFWindowActivated;
            dialog.ShowDialog();

            if (dialog.WebServiceInputArgs != null)
            {
                WebServiceInputEventArgs e = dialog.WebServiceInputArgs;

                this.btnCancelSearch.Enabled = this.cancelSearchButtonState = true;
                this.requestIdentifier = string.Empty;

                if (e != null && !string.IsNullOrEmpty(this.webserviceName))
                {
                    IBlastWebHandler blastServiceHandler = this.GetWebServiceInstance(this.webserviceName);

                    //blastServiceHandler.Configuration = e.Configuration;

                    //// Make sure if the event handler was already added, it is removed
                    //// otherwise the handler will be invoked multiple times when the
                    //// event is raised.
                    //blastServiceHandler.RequestCompleted -=
                    //    new EventHandler<BlastRequestCompletedEventArgs>(this.OnBlastRequestCompleted);

                    //blastServiceHandler.RequestCompleted +=
                    //    new EventHandler<BlastRequestCompletedEventArgs>(this.OnBlastRequestCompleted);
                    //try
                    //{
                    //    if (WebServices.BioHPCBlast != null && WebServices.BioHPCBlast.Name.Equals(this.webserviceName))
                    //    {
                    //        this.requestIdentifier = blastServiceHandler.SubmitRequest(sequences, e.ServiceParameters);
                    //    }
                    //    else
                    //    {
                    //        this.requestIdentifier = blastServiceHandler.SubmitRequest(
                    //            sequences[0],
                    //            e.ServiceParameters);
                    //    }

                    //    this.ChangeStatusBar(string.Format(Resources.WEBSERVICE_STATUS_BAR, Resources.SEARCHING));
                    //}
                    //catch (Exception ex)
                    //{
                    //    this.btnCancelSearch.Enabled = this.cancelSearchButtonState = false;
                    //    MessageBox.Show(ex.Message, Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //}
                }
            }
        }

        /// <summary>
        /// This event is fired when the search on NCBI database is completed.
        /// This event asks IWebServicePresneter to display BLAST outputs.
        /// </summary>
        /// <param name="sender">BackgroundWorker instance.</param>
        /// <param name="e">Event data.</param>
        private void OnSearchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.btnCancelSearch.Enabled = this.cancelSearchButtonState = false;
            if (e.Cancelled)
            {
                return;
            }

            this.ScreenUpdate(false);

            var blastResults = e.Result as List<BlastResult>;

            if (blastResults != null)
            {
                if (BlastHasResults(blastResults))
                {
                    Workbook activeWorkBook = Globals.ThisAddIn.Application.ActiveWorkbook;
                    var activesheet = (Worksheet)Globals.ThisAddIn.Application.ActiveSheet;
                    var currentsheet =
                        (Worksheet)activeWorkBook.Worksheets.Add(Type.Missing, activesheet, Type.Missing, Type.Missing);

                    currentsheet.Name = Resources.BLAST_RESULTS + this.currentBlastSheetNumber;
                    this.currentBlastSheetNumber++;

                    int rowNumber = 2;
                    int columnNumber = 2;

                    foreach (string header in blastHeaders)
                    {
                        Range range = currentsheet.get_Range(GetColumnString(columnNumber) + rowNumber, Type.Missing);
                        range.Cells.Font.Bold = true;
                        WriteRangeValue(range, header);
                        columnNumber++;
                    }

                    rowNumber++;

                    foreach (BlastResult result in blastResults)
                    {
                        foreach (BlastSearchRecord record in result.Records)
                        {
                            if (null != record.Hits && 0 < record.Hits.Count)
                            {
                                foreach (Hit hit in record.Hits)
                                {
                                    if (null != hit.Hsps && 0 < hit.Hsps.Count)
                                    {
                                        foreach (Hsp hsp in hit.Hsps)
                                        {
                                            DisplayValues(
                                                currentsheet,
                                                rowNumber,
                                                record.IterationQueryId,
                                                hit.Id,
                                                hsp.IdentitiesCount.ToString(CultureInfo.CurrentCulture),
                                                hsp.AlignmentLength.ToString(CultureInfo.CurrentCulture),
                                                hit.Length.ToString(CultureInfo.CurrentCulture),
                                                hsp.QueryStart.ToString(),
                                                hsp.QueryEnd.ToString(),
                                                hsp.HitStart.ToString(CultureInfo.CurrentCulture),
                                                hsp.HitEnd.ToString(),
                                                hsp.EValue.ToString(CultureInfo.CurrentCulture),
                                                hsp.BitScore.ToString(CultureInfo.CurrentCulture));
                                            rowNumber++;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    currentsheet.Columns.AutoFit();
                    this.EnableAllControls();
                }
                else
                {
                    MessageBox.Show(
                        Resources.BLAST_NO_RESULT,
                        Resources.CAPTION,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                this.ChangeStatusBar(string.Format(Resources.WEBSERVICE_STATUS_BAR, Resources.DONE));
            }
            else if (e.Result is string)
            {
                this.ChangeStatusBar(string.Format(Resources.WEBSERVICE_STATUS_BAR, Resources.DONE));
                MessageBox.Show(e.Result.ToString(), Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.ScreenUpdate(true);
        }

        #endregion

        #region -- Assembly --

        /// <summary>
        /// This event is fired when user wants to assemble the sequences.
        /// This event will be raised by IAssembler. The controller class
        /// instantiates algorithm implementation to perform assembly.
        /// </summary>
        /// <param name="e">Assembly input arguments.</param>
        private void OnRunAssemblerAlgorithm(AssemblyInputEventArgs e)
        {
            if (e.Sequences != null && e.Aligner != null)
            {
                this.btnCancelAssemble.Enabled = this.cancelAssemblyButtonState = true;
                this.ChangeStatusBar(string.Format(Resources.ASSEMBLER_STATUS_BAR, Resources.ASSEMBLING));
                this.assemblerThread = new BackgroundWorker();
                this.assemblerThread.WorkerSupportsCancellation = true;
                this.assemblerThread.DoWork += this.OnAssembleStarted;
                this.assemblerThread.RunWorkerCompleted += this.OnAssemblerCompleted;
                this.assemblerThread.RunWorkerAsync(e);
            }
        }

        /// <summary>
        /// This event is fired when the assembling the sequence is completed.
        /// This event asks IAssembler to display consensus view.
        /// </summary>
        /// <param name="sender">BackgroundWorker instance.</param>
        /// <param name="e">Event data.</param>
        private void OnAssemblerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                return;
            }

            var assemblerResult = e.Result as IDeNovoAssembly;
            if (assemblerResult != null)
            {
                this.BuildConsensusView(assemblerResult);
            }

            this.btnCancelAssemble.Enabled = this.cancelAssemblyButtonState = false;
            this.ChangeStatusBar(string.Format(Resources.ASSEMBLER_STATUS_BAR, Resources.DONE));

            // This is an error scenario, display it to the users.
            var errorMessage = e.Result as string;
            if (errorMessage != null)
            {
                this.ChangeStatusBar(string.Format(Resources.ASSEMBLER_STATUS_BAR, Resources.ERROR));
                MessageBox.Show(errorMessage, Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// This method displays the output of a assembly process.
        /// </summary>
        /// <param name="assemblerResult">Result of the assembly process.</param>
        private void BuildConsensusView(IDeNovoAssembly assemblerResult)
        {
            var overlapAssemblerResult = assemblerResult as IOverlapDeNovoAssembly;
            if (overlapAssemblerResult != null)
            {
                this.ScreenUpdate(false);
                Workbook activeWorkBook = Globals.ThisAddIn.Application.ActiveWorkbook;
                var activesheet = (Worksheet)Globals.ThisAddIn.Application.ActiveSheet;
                var currentsheet =
                    (Worksheet)activeWorkBook.Worksheets.Add(Type.Missing, activesheet, Type.Missing, Type.Missing);
                string[,] rangeData;
                int rowNumber = 1;
                int contigNumber = 1;
                int rowCount, rowIndex, columnIndex;

                ((_Worksheet)currentsheet).Activate();
                Globals.ThisAddIn.Application.ActiveWindow.Zoom = ZoomLevel;

                currentsheet.Name =
                    this.GetValidFileNames(
                        "ConsensusView" + this.currentConsensusSheetNumber.ToString(CultureInfo.CurrentCulture));
                this.currentConsensusSheetNumber++;
                foreach (Contig contig in overlapAssemblerResult.Contigs)
                {
                    // Write Header
                    Range header = currentsheet.get_Range(
                        "A" + rowNumber.ToString(CultureInfo.CurrentCulture),
                        Type.Missing);
                    WriteRangeValue(header, "Contig" + contigNumber.ToString(CultureInfo.CurrentCulture));

                    ISequence contigSequence = contig.Consensus;
                    Range currentRange = currentsheet.get_Range(
                        "B" + rowNumber.ToString(CultureInfo.CurrentCulture),
                        Type.Missing);

                    long numberofCharacters = 1;
                    if (contigSequence.Count > MaxExcelColumns)
                    {
                        if (contigSequence.Count % MaxExcelColumns == 0)
                        {
                            numberofCharacters = contigSequence.Count / MaxExcelColumns;
                        }
                        else
                        {
                            numberofCharacters = contigSequence.Count / MaxExcelColumns;
                            numberofCharacters++;
                        }
                    }

                    int columnCount = 1;

                    rowCount = (int)Math.Ceiling(contigSequence.Count / (decimal)MaxExcelColumns);
                    rowIndex = 0;
                    columnIndex = 0;
                    rangeData =
                        new string[rowCount,
                            contigSequence.Count > MaxExcelColumns ? MaxExcelColumns : contigSequence.Count];

                    for (long i = 0; i < contigSequence.Count; i += numberofCharacters)
                    {
                        if (MaxExcelColumns == columnIndex)
                        {
                            columnIndex = 0;
                            rowIndex++;
                        }

                        ISequence tempSeq = contigSequence.GetSubSequence(i, numberofCharacters);

                        string subsequence = tempSeq.ConvertToString();
                        rangeData[rowIndex, columnIndex] = subsequence;

                        columnIndex++;
                        columnCount++;
                    }

                    var formulaBuilder = new StringBuilder();
                    string formula = string.Empty;
                    string name = string.Empty;
                    if (columnCount > 1)
                    {
                        currentRange = currentRange.get_Resize(1, columnCount - 1);
                        currentRange.set_Value(Missing.Value, rangeData);
                        this.FillBackGroundColor(currentRange);
                        formulaBuilder.Append("=");
                        formulaBuilder.Append(currentsheet.Name);
                        formulaBuilder.Append("!");
                        formulaBuilder.Append("$B$" + rowNumber);
                        formulaBuilder.Append(":$");
                        formulaBuilder.Append(
                            GetColumnString(columnCount) + "$" + rowNumber.ToString(CultureInfo.CurrentCulture));
                        formula = formulaBuilder.ToString();
                        name = Resources.CONTIG + contigNumber.ToString(CultureInfo.CurrentCulture);

                        currentsheet.Names.Add(
                            this.GetValidFileNames(name),
                            formula,
                            true,
                            Type.Missing,
                            Type.Missing,
                            Type.Missing,
                            Type.Missing,
                            Type.Missing,
                            Type.Missing,
                            Type.Missing,
                            Type.Missing);
                    }

                    rowNumber++;

                    int sequenceNumber = 1;
                    foreach (Contig.AssembledSequence assembled in contig.Sequences)
                    {
                        int initialRowNumber = rowNumber;
                        columnCount = 1;

                        ISequence assembledSequence = assembled.Sequence;

                        // Write Header
                        Range sequenceHeader =
                            currentsheet.get_Range("A" + rowNumber.ToString(CultureInfo.CurrentCulture), Type.Missing);
                        WriteRangeValue(sequenceHeader, assembledSequence.ID);

                        if (assembled.IsComplemented && assembled.IsReversed)
                        {
                            assembledSequence = assembled.Sequence.GetReverseComplementedSequence();
                            sequenceHeader.Cells.AddComment(Resources.SEQUENCE_REVERSECOMPLEMENT);
                        }
                        else if (assembled.IsReversed)
                        {
                            assembledSequence = assembled.Sequence.GetReversedSequence();
                            sequenceHeader.Cells.AddComment(Resources.SEQUENCE_REVERSE);
                        }
                        else if (assembled.IsComplemented)
                        {
                            assembledSequence = assembled.Sequence.GetComplementedSequence();
                            sequenceHeader.Cells.AddComment(Resources.SEQUENCE_COMPLEMENT);
                        }

                        long startingColumn = assembled.Position / numberofCharacters;
                        startingColumn++;
                        currentRange =
                            currentsheet.get_Range(
                                GetColumnString(startingColumn + 1) + rowNumber.ToString(CultureInfo.CurrentCulture),
                                Type.Missing);

                        long startingIndex = 0;

                        if (numberofCharacters > 1)
                        {
                            long cellStartIndex = (startingColumn - 1) * numberofCharacters;
                            long endingIndex = cellStartIndex + numberofCharacters - 1;
                            long startextractCharacters = endingIndex - assembled.Position + 1;
                            long numberOfSpaces = Math.Abs(assembled.Position - cellStartIndex);

                            string firstcell =
                                assembledSequence.GetSubSequence(0, startextractCharacters).ConvertToString();
                            var sb = new StringBuilder();

                            for (int i = 1; i <= numberOfSpaces; i++)
                            {
                                sb.Append(" ");
                            }

                            sb.Append(firstcell);
                            WriteRangeValue(currentRange, sb.ToString());
                            startingIndex = startextractCharacters;
                            currentRange = currentRange.Next;
                        }

                        rowCount = (int)Math.Ceiling(assembledSequence.Count / (decimal)MaxExcelColumns);
                        rowIndex = 0;
                        columnIndex = 0;
                        rangeData =
                            new string[rowCount,
                                assembledSequence.Count > MaxExcelColumns ? MaxExcelColumns : assembledSequence.Count];

                        for (long i = startingIndex; i < assembledSequence.Count; i += numberofCharacters)
                        {
                            if (MaxExcelColumns == columnIndex)
                            {
                                columnIndex = 0;
                                rowIndex++;
                            }

                            ISequence tempSeq = assembledSequence.GetSubSequence(i, numberofCharacters);

                            string derivedSequence = tempSeq.ConvertToString();
                            rangeData[rowIndex, columnIndex] = derivedSequence;

                            columnIndex++;
                            columnCount++;
                        }

                        if (columnCount > 1)
                        {
                            currentRange = currentRange.get_Resize(1, columnCount - 1);
                            currentRange.set_Value(Missing.Value, rangeData);
                            currentRange.Columns.AutoFit();
                            this.FillBackGroundColor(currentRange);

                            formulaBuilder = new StringBuilder();
                            formulaBuilder.Append("=");
                            formulaBuilder.Append(currentsheet.Name);
                            formulaBuilder.Append("!$");
                            formulaBuilder.Append(GetColumnString(startingColumn + 1) + "$" + initialRowNumber);
                            formulaBuilder.Append(":$");
                            formulaBuilder.Append(
                                GetColumnString(startingColumn + columnCount - 1) + "$"
                                + rowNumber.ToString(CultureInfo.CurrentCulture));
                            string sequenceFormula = formulaBuilder.ToString();
                            name = Resources.CONTIG + contigNumber + "_" + assembledSequence.ID
                                   + sequenceNumber.ToString(CultureInfo.CurrentCulture);
                            currentsheet.Names.Add(
                                this.GetValidFileNames(name),
                                sequenceFormula,
                                true,
                                Type.Missing,
                                Type.Missing,
                                Type.Missing,
                                Type.Missing,
                                Type.Missing,
                                Type.Missing,
                                Type.Missing,
                                Type.Missing);
                        }

                        rowNumber++;
                        sequenceNumber++;
                    }

                    contigNumber++;
                    rowNumber++;
                }

                int unmerged = 1;
                foreach (ISequence sequence in overlapAssemblerResult.UnmergedSequences)
                {
                    // Write Header
                    Range sequenceHeader = currentsheet.get_Range(
                        "A" + rowNumber.ToString(CultureInfo.CurrentCulture),
                        Type.Missing);
                    WriteRangeValue(sequenceHeader, "Unmerged Sequence_" + sequence.ID);

                    long numberofCharacters = 1;
                    if (sequence.Count > MaxExcelColumns)
                    {
                        if (sequence.Count % MaxExcelColumns == 0)
                        {
                            numberofCharacters = sequence.Count / MaxExcelColumns;
                        }
                        else
                        {
                            numberofCharacters = sequence.Count / MaxExcelColumns;
                            numberofCharacters++;
                        }
                    }

                    Range currentRange = currentsheet.get_Range(
                        "B" + rowNumber.ToString(CultureInfo.CurrentCulture),
                        Type.Missing);

                    int columnCount = 1;
                    rowCount = (int)Math.Ceiling(sequence.Count / (decimal)MaxExcelColumns);
                    rowIndex = 0;
                    columnIndex = 0;
                    rangeData =
                        new string[rowCount, sequence.Count > MaxExcelColumns ? MaxExcelColumns : sequence.Count];

                    for (long i = 0; i < sequence.Count; i += numberofCharacters)
                    {
                        if (MaxExcelColumns == columnIndex)
                        {
                            columnIndex = 0;
                            rowIndex++;
                        }

                        ISequence tempSeq = sequence.GetSubSequence(i, numberofCharacters);

                        string subsequence = tempSeq.ConvertToString();
                        rangeData[rowIndex, columnIndex] = subsequence;

                        columnIndex++;
                        columnCount++;
                    }

                    if (columnCount > 1)
                    {
                        currentRange = currentRange.get_Resize(1, columnCount - 1);
                        currentRange.set_Value(Missing.Value, rangeData);

                        this.FillBackGroundColor(currentRange);

                        var formulaBuilder = new StringBuilder();
                        formulaBuilder.Append("=");
                        formulaBuilder.Append(currentsheet.Name);
                        formulaBuilder.Append("!");
                        formulaBuilder.Append("$B$" + rowNumber);
                        formulaBuilder.Append(":$");
                        formulaBuilder.Append(
                            GetColumnString(columnCount) + "$" + rowNumber.ToString(CultureInfo.CurrentCulture));
                        string formula = formulaBuilder.ToString();
                        string name = Resources.UNMERGED_SEQUENCE + unmerged.ToString(CultureInfo.CurrentCulture);
                        currentsheet.Names.Add(
                            this.GetValidFileNames(name),
                            formula,
                            true,
                            Type.Missing,
                            Type.Missing,
                            Type.Missing,
                            Type.Missing,
                            Type.Missing,
                            Type.Missing,
                            Type.Missing,
                            Type.Missing);
                    }

                    rowNumber++;
                    unmerged++;
                }

                currentsheet.Columns.AutoFit();
                this.EnableAllControls();
                this.ScreenUpdate(true);
            }
        }

        /// <summary>
        /// This method fills the background color of a particular range depending
        /// on the content within the range.
        /// </summary>
        /// <param name="currentRange">Range whose background color has to be filled.</param>
        private void FillBackGroundColor(Range currentRange)
        {
            this.ScreenUpdate(false);
            if (currentRange.FormatConditions.Count > 0)
            {
                currentRange.FormatConditions.Delete();
            }

            foreach (var color in this.colorMap)
            {
                if (color.Value == null) // skip if no color specified
                {
                    continue;
                }
                if (color.Value.A == 0) // skip if transparent
                {
                    continue;
                }

                Range keyExists = currentRange.Find(
                    new string(new[] { (char)color.Key }),
                    Missing.Value,
                    Missing.Value,
                    XlLookAt.xlWhole,
                    Missing.Value,
                    XlSearchDirection.xlNext,
                    false,
                    Missing.Value,
                    Missing.Value);

                if (null != keyExists)
                {
                    var cond =
                        (FormatCondition)
                        currentRange.FormatConditions.Add(
                            XlFormatConditionType.xlCellValue,
                            XlFormatConditionOperator.xlEqual,
                            new string(new[] { (char)color.Key }),
                            Missing.Value,
                            Missing.Value,
                            Missing.Value,
                            Missing.Value,
                            Missing.Value);

                    cond.Interior.Color = ColorTranslator.ToWin32(color.Value);
                }
            }

            this.ScreenUpdate(true);
        }

        /// <summary>
        /// This event is fired by assemblerThread when the thread is invoked.
        /// This event assembles a collection of ISequences.
        /// </summary>
        /// <param name="sender">BackgroundWorker instance.</param>
        /// <param name="e">Event data.</param>
        private void OnAssembleStarted(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            try
            {
                if (worker != null)
                {
                    var assemblerInput = e.Argument as AssemblyInputEventArgs;
                    if (assemblerInput != null)
                    {
                        IDeNovoAssembly assemblerResult = RunAssembly(assemblerInput, worker);
                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }

                        e.Result = assemblerResult;
                    }
                }
            }
            catch (Exception ex)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                e.Result = ex.Message;
            }
        }

        #endregion

        #region -- Enable-Disable Controls --

        /// <summary>
        /// This method enables the given list of controls.
        /// </summary>
        private void EnableAllControls()
        {
            var groups = new List<RibbonGroup>();

            groups.Add(this.groupImportExport);
            groups.Add(this.groupAlignment);
            groups.Add(this.groupWebService);
            groups.Add(this.groupCharts);
            groups.Add(this.grpGenomicInterval);
            groups.Add(this.groupConfig);
            groups.Add(this.groupAssembly);
            groups.Add(this.groupNodeXL);

            foreach (RibbonGroup group in groups)
            {
                foreach (RibbonControl control in group.Items)
                {
                    control.Enabled = true;
                }
            }

            this.btnVennDiagram.Enabled = this.isNodeXLInstalled;
            this.btnCancelSearch.Enabled = this.cancelSearchButtonState;
            this.btnCancelAlign.Enabled = this.cancelAlignButtonState;
            this.btnCancelAssemble.Enabled = this.cancelAssemblyButtonState;
        }

        /// <summary>
        /// This method disables all controls in the ribbon including generic controls.
        /// </summary>
        private void DisableAllControls()
        {
            var groups = new List<RibbonGroup>();

            groups.Add(this.groupImportExport);
            groups.Add(this.groupAlignment);
            groups.Add(this.groupWebService);
            groups.Add(this.groupCharts);
            groups.Add(this.grpGenomicInterval);
            groups.Add(this.groupConfig);
            groups.Add(this.groupAssembly);
            groups.Add(this.groupNodeXL);

            foreach (RibbonGroup group in groups)
            {
                foreach (RibbonControl control in group.Items)
                {
                    control.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Raised when a workbook switches to another one or when its closed.
        /// </summary>
        /// <param name="Wb">workbook being deactivated</param>
        private void OnWorkbookDeactivate(Workbook Wb)
        {
            if (Globals.ThisAddIn.Application.Workbooks.Count == 1) // If count is one and is deactivating, it is closing!
            {
                this.DisableAllControls();
            }
        }

        /// <summary>
        /// This event is fired when a workbook is activated.
        /// this event enables all controls in the Generic group.
        /// </summary>
        /// <param name="workBook">Workbook being opened.</param>
        private void OnWorkBookOpen(Workbook workBook)
        {
            if (!SequenceCache.IsInitialized)
            {
                SequenceCache.Initialize();
            }

            this.EnableAllControls();
            this.LoadSheetNames(Globals.ThisAddIn.Application.ActiveWorkbook);
        }

        #endregion -- Enable-Disable groups --

        #endregion -- Private Methods --
    }
}