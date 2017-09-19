namespace BiodexExcel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using BiodexExcel.Properties;
    using BiodexExcel.Visualizations.Common;
    using Bio;
    using Bio.IO;
    using Bio.IO.GenBank;
    using Bio.IO.Gff;
    using Bio.Util;
    using Microsoft.Office.Interop.Excel;

    /// <summary>
    /// This class exposes functionality of making custom user selections parsing it and returning the results, 
    /// thereby hiding the implementation of UI and logic related to custom selections and parsing.
    /// </summary>
    public class InputSelection
    {
        /// <summary>
        /// Parameter: Overlapping Base Pairs
        /// </summary>
        public const string OVERLAP = "Overlap";

        /// <summary>
        /// Parameter: Minimum Overlap
        /// </summary>
        public const string MINIMUMOVERLAP = "MinimumOverlap";

        /// <summary>
        /// List of headers which will be displayed in a Sequence range sheet.
        /// </summary>
        private static List<string> rangeHeaders = new List<string>() { 
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
                Resources.BED_BLOCK_STARTS };

        /// <summary>
        /// List of headers which are must
        /// </summary>
        private static List<string> requiredHeaders = new List<string>() { 
                Resources.CHROM_ID, 
                Resources.CHROM_START, 
                Resources.CHROM_END };

        private int minSeqCount = 1;

        /// <summary>
        /// Minimum number of sequences to be selected by the user
        /// </summary>
        public int MinimumSequenceCount
        {
            get { return minSeqCount; }
            set { minSeqCount = value; }
        }

        private int maxSeqCount = Int32.MaxValue;

        /// <summary>
        /// Maximum number of sequences user can select
        /// </summary>
        public int MaximumSequenceCount
        {
            get { return maxSeqCount; }
            set { maxSeqCount = value; }
        }

        /// <summary>
        /// Flag to indicate if there should be a option to input the sequence name in the input selection window.
        /// </summary>
        public bool PromptForSequenceName { get; set; }

        private ISelectionDialog callerDialog;
        private NativeWindow nativeExcelWindow;
        private event SequenceSelectionComplete inputSequenceSelectionComplete;
        private event InputSequenceRangeSelectionComplete inputSequenceRangeSelectionComplete; // for BED
        private object[] argsForCallback;

        /// <summary>
        /// Array of strings, which will be used as the label for sequence address box in the input selection dialog. 
        /// Dialog will use the default string if this is null.
        /// </summary>
        public string[] SequenceLabels { get; set; }

        /// <summary>
        /// Creates the instance of input selection class and initializes it
        /// </summary>
        public InputSelection()
        {
            PromptForSequenceName = true;
        }

        /// <summary>
        /// This method will initiate a custom user selection by showing the input 
        /// selection dialog, then parse the selections and return the sequence object.
        /// This method is particularly for BED files.
        /// </summary>
        /// <param name="callbackAfterComplete">
        /// Method to call after the selection and parsing process is complete.
        /// </param>
        /// <param name="intersectOperation">Is this an Intersect Operation</param>
        /// <param name="isOverlappingIntervalVisible">Is Overlapping interval visible</param>
        /// <param name="isMinimumOverlapVisible">Is Minimum overlap visible</param>
        /// <param name="argsForCallback">Callback arguments</param>
        public void GetInputSequenceRanges(
                InputSequenceRangeSelectionComplete callbackAfterComplete,
                bool intersectOperation,
                bool isOverlappingIntervalVisible,
                bool isMinimumOverlapVisible,
                params object[] argsForCallback)
        {
            inputSequenceRangeSelectionComplete = callbackAfterComplete;
            this.argsForCallback = argsForCallback;

            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlWait;
            InputSelectionDialog dialog = new InputSelectionDialog(
                    ShowSelectionHelper,
                    minSeqCount,
                    maxSeqCount,
                    intersectOperation,
                    isOverlappingIntervalVisible,
                    isMinimumOverlapVisible,
                    SequenceLabels);
            dialog.IsSequenceRangeSelection = true;
            dialog.IsSequenceNameVisible = this.PromptForSequenceName;
            dialog.Activated += new EventHandler(OnWPFWindowActivated);
            dialog.Initialize();
            dialog.SelectedItem.SequenceAddress = GetRangeAddress(Globals.ThisAddIn.Application.Selection);
            System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(dialog);
            helper.Owner = (IntPtr)Globals.ThisAddIn.Application.Hwnd;
            dialog.InputSelectionDialogSubmitting += new SequenceSelectionDialogSubmit(OnInputSequenceRangeDialogSubmit);
            dialog.ShowDialog(); // this call will exit when selection window is shown as this dialog will be hidden.
        }

        /// <summary>
        /// Method called when the user clicks Ok button on InputSelectionDialog.
        /// Takes care of parsing the selections and returning the result to the user.
        /// In case there was an error parsing, it will show the input selection dialog again with the sequence highlighted.
        /// </summary>
        /// <param name="dialog">InputSequenceDialog object which raised this event</param>
        private void OnInputSequenceRangeDialogSubmit(ISelectionDialog dialog)
        {
            InputSelectionDialog selectionDialog = dialog as InputSelectionDialog;
            GroupData cachedData = null;

            // maps sheet to its column-mapping
            Dictionary<string, Dictionary<int, string>> columnMappedSheets =
                    new Dictionary<string, Dictionary<int, string>>();

            // Goes in the cache and is the output of this method as well.
            Dictionary<SequenceRangeGrouping, GroupData> groupsData =
                    new Dictionary<SequenceRangeGrouping, GroupData>();
            List<SequenceRangeGrouping> parsedSequences = new List<SequenceRangeGrouping>();

            SequenceRangeGrouping sequenceRangeGroup = null;
            Dictionary<string, Dictionary<ISequenceRange, string>> sheetData = null;
            Dictionary<ISequenceRange, string> rangeData = null;
            List<ISequenceRange> sequenceRanges = null;

            List<Range> rangesInCurrentSequenceItem;

            // Regular expression to read the sheet name from address
            Regex regexSheetname = new Regex(@"(?<Sheetname>^.[^!]*)", RegexOptions.IgnoreCase);
            Match matchSheetname = null;
            string sheetName = string.Empty;

            try
            {
                foreach (InputSequenceItem currentSequenceItem in selectionDialog.GetSequences())
                {
                    try
                    {
                        rangesInCurrentSequenceItem = GetRanges(currentSequenceItem.SequenceAddress);
                        // get from cache
                        cachedData = SequenceCache.TryGetSequence(rangesInCurrentSequenceItem, selectionDialog.InputParamsAsKey) as GroupData;
                        if (cachedData != null)
                        {
                            // got from cache
                            cachedData.Name = currentSequenceItem.SequenceName; // Set ID

                            if (currentSequenceItem.IsUseMetadataSelected)
                            {
                                parsedSequences.Insert(0, cachedData.Group);
                            }
                            else
                            {
                                parsedSequences.Add(cachedData.Group);
                            }

                            if (!groupsData.ContainsKey(cachedData.Group))
                            {
                                groupsData.Add(cachedData.Group, cachedData);
                            }
                        }
                        else
                        {
                            // parse it as its not in cache
                            sheetData = new Dictionary<string, Dictionary<ISequenceRange, string>>();
                            sequenceRanges = new List<ISequenceRange>();
                            foreach (Range currentRange in rangesInCurrentSequenceItem)
                            {
                                bool firstRowIsHeader = false;

                                // See if the sheet in which this range is, has a column mapping
                                if (!columnMappedSheets.ContainsKey(GetMappingKey(currentRange)))
                                {
                                    (currentRange.Worksheet as _Worksheet).Activate();
                                    currentRange.Select();
                                    Dictionary<int, string> mapping = GetMappingForRange(currentRange, out firstRowIsHeader);
                                    if (mapping == null)
                                    {
                                        // Could not get a proper mapping. So redirect to previous window.
                                        selectionDialog.ShowDialog();
                                        return;
                                    }

                                    if (firstRowIsHeader)
                                    {
                                        UpdateColumnHeaders(currentRange, mapping);
                                    }

                                    columnMappedSheets.Add(GetMappingKey(currentRange), mapping);
                                }

                                // If range has a header, remove first row from it before sending it for parsing.
                                Range rangeToParse;
                                if (firstRowIsHeader)
                                {
                                    if (currentRange.Rows.Count == 1) // only one row which is marked as header, throw error
                                    {
                                        throw new InvalidOperationException(Resources.SelectionModel_ParsingFailed);
                                    }

                                    rangeToParse = currentRange.get_Offset(1, 0);
                                    rangeToParse = rangeToParse.get_Resize(currentRange.Rows.Count - 1, currentRange.Columns.Count);
                                }
                                else
                                {
                                    rangeToParse = currentRange;
                                }

                                Dictionary<ISequenceRange, string> srCollection =
                                        ExcelSelectionParser.RangeToSequenceRange(
                                            rangeToParse,
                                            columnMappedSheets[GetMappingKey(currentRange)]);

                                foreach (KeyValuePair<ISequenceRange, string> sr in srCollection)
                                {
                                    matchSheetname = regexSheetname.Match(sr.Value);
                                    if (matchSheetname.Success)
                                    {
                                        sheetName = matchSheetname.Groups["Sheetname"].Value;
                                        if (sheetData.TryGetValue(sheetName, out rangeData))
                                        {
                                            rangeData.Add(sr.Key, sr.Value);
                                        }
                                        else
                                        {
                                            rangeData = new Dictionary<ISequenceRange, string>();
                                            sheetData.Add(sheetName, rangeData);
                                            rangeData.Add(sr.Key, sr.Value);
                                        }

                                        sequenceRanges.Add(sr.Key);
                                    }
                                }
                            }

                            sequenceRangeGroup = new SequenceRangeGrouping(sequenceRanges);
                            cachedData = new GroupData(sequenceRangeGroup,
                                    currentSequenceItem.SequenceName,
                                    sheetData);
                            SequenceCache.Add(rangesInCurrentSequenceItem, cachedData, selectionDialog.InputParamsAsKey);

                            if (currentSequenceItem.IsUseMetadataSelected)
                            {
                                parsedSequences.Insert(0, cachedData.Group);
                            }
                            else
                            {
                                parsedSequences.Add(cachedData.Group);
                            }

                            groupsData.Add(cachedData.Group, cachedData);
                        }
                    }
                    catch
                    {
                        // Set error status on the current item and re-throw the error
                        currentSequenceItem.SetErrorStatus(true);
                        throw;
                    }
                }

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add(InputSelection.OVERLAP, selectionDialog.OverlappingBasePairs);
                parameters.Add(InputSelection.MINIMUMOVERLAP, selectionDialog.MinOverLap);

                // On successful completion of parsing...
                if (inputSequenceRangeSelectionComplete != null)
                {
                    InputSequenceRangeSelectionEventArg eventArg =
                            new InputSequenceRangeSelectionEventArg(groupsData,
                                    parsedSequences,
                                    parameters,
                                    argsForCallback);
                    inputSequenceRangeSelectionComplete(eventArg);
                }

                selectionDialog.InputSelectionDialogSubmitting -= OnInputSequenceDialogSubmit;
                selectionDialog.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                selectionDialog.ShowDialog();
            }
        }


        /// <summary>
        /// This method will initiate a custom user selection by showing the input 
        /// selection dialog, then parse the selections and return the sequence object.
        /// This method is for any sequence data other than BED.
        /// </summary>
        /// <param name="callbackAfterComplete">
        /// Method to call after the selection and parsing process is complete.
        /// </param>
        /// <param name="intersectOperation">Is this an Intersect Operation</param>
        /// <param name="argsForCallback">Callback arguments</param>
        public void GetInputSequences(
                SequenceSelectionComplete callbackAfterComplete,
                bool intersectOperation,
                params object[] argsForCallback)
        {
            this.PromptForSequenceName = true;
            this.inputSequenceSelectionComplete = callbackAfterComplete;
            this.argsForCallback = argsForCallback;

            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlWait;
            InputSelectionDialog selectionDialog = new InputSelectionDialog(
                    ShowSelectionHelper,
                    minSeqCount,
                    maxSeqCount,
                    intersectOperation,
                    false,
                    false,
                    SequenceLabels);
            selectionDialog.Activated += new EventHandler(OnWPFWindowActivated);
            selectionDialog.IsSequenceNameVisible = this.PromptForSequenceName;
            selectionDialog.Initialize();
            selectionDialog.SelectedItem.SequenceAddress = GetRangeAddress(Globals.ThisAddIn.Application.Selection);
            System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(selectionDialog);
            helper.Owner = (IntPtr)Globals.ThisAddIn.Application.Hwnd;
            selectionDialog.InputSelectionDialogSubmitting += new SequenceSelectionDialogSubmit(OnInputSequenceDialogSubmit);
            selectionDialog.ShowDialog(); // this call will exit when selection window is shown as this dialog will be hidden.
        }

        /// <summary>
        /// Method called when the user clicks Ok button on InputSelectionDialog.
        /// Takes care of parsing the selections and returning the result to the user.
        /// In case there was an error parsing, it will show the input selection dialog again with the sequence highlighted.
        /// </summary>
        /// <param name="dialog">InputSequenceDialog object which raised this event</param>
        private void OnInputSequenceDialogSubmit(ISelectionDialog dialog)
        {
            InputSelectionDialog selectionDialog = dialog as InputSelectionDialog;
            List<ISequence> parsedSequences = new List<ISequence>();
            List<Range> rangesInCurrentSequenceItem;
            List<InputSequenceItem> sequenceItems = selectionDialog.GetSequences();

            try
            {
                foreach (InputSequenceItem currentSequenceItem in sequenceItems)
                {
                    try
                    {
                        rangesInCurrentSequenceItem = GetRanges(currentSequenceItem.SequenceAddress);
                        if (rangesInCurrentSequenceItem.Count > 0)
                        {
                            ISequence sequenceForCurrentItem;
                            sequenceForCurrentItem = SequenceCache.TryGetSequence(rangesInCurrentSequenceItem, selectionDialog.InputParamsAsKey) as ISequence; // get from cache

                            string id = currentSequenceItem.SequenceName;
                            if (string.IsNullOrEmpty(id))
                            {
                                id = currentSequenceItem.SequenceAddress;
                                int pos = id.IndexOf('!');
                                if (pos > 0)
                                    id = id.Substring(0, pos);
                            }

                            if (sequenceForCurrentItem == null) // if not in cache
                            {
                                sequenceForCurrentItem = ExcelSelectionParser.RangeToSequence(rangesInCurrentSequenceItem, selectionDialog.TreatBlankCellsAsGaps, selectionDialog.MoleculeType, id);
                                sequenceForCurrentItem.Metadata[Resources.EXCEL_CELL_LINK] = currentSequenceItem.SequenceAddress;
                                SequenceCache.Add(rangesInCurrentSequenceItem, sequenceForCurrentItem, selectionDialog.InputParamsAsKey);
                            }
                            else
                            {
                                // Set the ID
                                sequenceForCurrentItem = SetSequenceID(sequenceForCurrentItem, id);
                                sequenceForCurrentItem.Metadata[Resources.EXCEL_CELL_LINK] = currentSequenceItem.SequenceAddress;
                            }

                            parsedSequences.Add(sequenceForCurrentItem);
                        }
                        else
                        {
                            currentSequenceItem.SetErrorStatus(false);
                        }
                    }
                    catch
                    {
                        // Set error status on item and re-throw
                        currentSequenceItem.SetErrorStatus(true);
                        throw;
                    }
                }

                // On successful parsing...
                if (inputSequenceSelectionComplete != null)
                    inputSequenceSelectionComplete(parsedSequences, this.argsForCallback);
                selectionDialog.InputSelectionDialogSubmitting -= OnInputSequenceDialogSubmit;
                selectionDialog.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                selectionDialog.ShowDialog();
            }
        }

        /// <summary>
        /// Takes in any sequence as an object, try to identify what sequence it is and set the ID provided
        /// </summary>
        /// <param name="targetSequence">Sequence object</param>
        /// <param name="sequenceID">ID to be set</param>
        private ISequence SetSequenceID(ISequence targetSequence, string sequenceID)
        {
            if (targetSequence is Sequence)
            {
                //DerivedSequence derivedSeq = new DerivedSequence(targetSequence, false, false);
                targetSequence.ID = sequenceID;
                return targetSequence;
            }
            else
            {
                return targetSequence;
            }
        }

        /// <summary>
        /// This method will initiate a custom user selection by showing the export 
        /// selection dialog, then parse the selections and return the sequence object.
        /// This method is for any sequence data other than BED.
        /// </summary>
        /// <param name="callbackAfterComplete">
        /// Method to call after the selection and parsing process is complete.
        /// </param>
        /// <param name="argsForCallback">Callback arguments</param>
        public void GetSequencesForExport(
                SequenceSelectionComplete callbackAfterComplete,
                params object[] argsForCallback)
        {
            this.inputSequenceSelectionComplete = callbackAfterComplete;
            this.argsForCallback = argsForCallback;

            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlWait;
            ExportSelectionDialog selectionDialog = new ExportSelectionDialog(ShowSelectionHelper, maxSeqCount);
            selectionDialog.Activated += new EventHandler(OnWPFWindowActivated);
            selectionDialog.IsSequenceNameVisible = true;

            if (argsForCallback[0] is Bio.IO.FastA.FastAFormatter)
            {
                selectionDialog.IsMetadataVisible = false;
            }
            else if (argsForCallback[0] is Bio.IO.FastQ.FastQFormatter)
            {
                selectionDialog.IsQualityScoresVisible = true;
            }
            else
            {
                selectionDialog.IsMetadataVisible = true;
            }

            selectionDialog.Initialize();

            selectionDialog.SelectedItem.SequenceAddress = GetRangeAddress(Globals.ThisAddIn.Application.Selection);
            System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(selectionDialog);
            helper.Owner = (IntPtr)Globals.ThisAddIn.Application.Hwnd;
            selectionDialog.InputSelectionDialogSubmitting += new SequenceSelectionDialogSubmit(OnExportSequenceDialogSubmit);
            selectionDialog.ShowDialog(); // this call will exit when selection window is shown as this dialog will be hidden.
        }

        /// <summary>
        /// Method called when the user clicks Ok button on InputSelectionDialog.
        /// Takes care of parsing the selections and returning the result to the user.
        /// In case there was an error parsing, it will show the input selection dialog again with the sequence highlighted.
        /// </summary>
        /// <param name="selectionDialog">InputSequenceDialog object which raised this event</param>
        private void OnExportSequenceDialogSubmit(ISelectionDialog dialog)
        {
            ExportSelectionDialog selectionDialog = dialog as ExportSelectionDialog;
            List<ISequence> parsedSequences = new List<ISequence>();
            List<Range> rangesInCurrentSequenceItem;
            List<InputSequenceItem> sequenceItems = selectionDialog.GetSequences();
            ISequenceFormatter formatterUsed = argsForCallback[0] as ISequenceFormatter;

            try
            {
                foreach (InputSequenceItem currentSequenceItem in sequenceItems)
                {
                    try
                    {
                        ISequence sequenceForCurrentItem = null;

                        // Parse sequence
                        if (formatterUsed is GffFormatter && string.IsNullOrWhiteSpace(currentSequenceItem.SequenceAddress))
                        {
                            sequenceForCurrentItem = new Sequence(Alphabets.DNA, "");
                        }
                        else
                        {
                            rangesInCurrentSequenceItem = GetRanges(currentSequenceItem.SequenceAddress);

                            if (rangesInCurrentSequenceItem.Count > 0)
                            {
                                // get from cache with default UI options.
                                sequenceForCurrentItem = SequenceCache.TryGetSequence(rangesInCurrentSequenceItem, selectionDialog.InputParamsAsKey) as ISequence;
                                if (sequenceForCurrentItem == null) // if not in cache
                                {
                                    sequenceForCurrentItem = ExcelSelectionParser.RangeToSequence(rangesInCurrentSequenceItem, selectionDialog.TreatBlankCellsAsGaps, selectionDialog.MoleculeType, currentSequenceItem.SequenceName);
                                    //added default from UI as auto detect and ignore space
                                    SequenceCache.Add(rangesInCurrentSequenceItem, sequenceForCurrentItem, selectionDialog.InputParamsAsKey);
                                }
                                else
                                {
                                    // Set the ID
                                    sequenceForCurrentItem = SetSequenceID(sequenceForCurrentItem, currentSequenceItem.SequenceName);
                                }
                            }
                            else
                            {
                                currentSequenceItem.SetErrorStatus(false);
                            }
                        }
                        //Parse metadata
                        if (formatterUsed is Bio.IO.FastQ.FastQFormatter)
                        {
                            rangesInCurrentSequenceItem = GetRanges(currentSequenceItem.MetadataAddress);
                            if (rangesInCurrentSequenceItem.Count > 0 && sequenceForCurrentItem != null)
                            {
                                sequenceForCurrentItem = ExcelSelectionParser.RangeToQualitativeSequence(rangesInCurrentSequenceItem, sequenceForCurrentItem);
                            }
                        }
                        else if (formatterUsed is GenBankFormatter)
                        {
                            rangesInCurrentSequenceItem = GetRanges(currentSequenceItem.MetadataAddress);
                            if (rangesInCurrentSequenceItem.Count > 0 && sequenceForCurrentItem != null)
                            {
                                try
                                {
                                    GenBankMetadata metadata = ExcelSelectionParser.RangeToGenBankMetadata(rangesInCurrentSequenceItem);
                                    sequenceForCurrentItem.Metadata[Helper.GenBankMetadataKey] = metadata;
                                    if (string.IsNullOrEmpty(sequenceForCurrentItem.ID))
                                    {
                                        // Set the ID
                                        sequenceForCurrentItem = SetSequenceID(sequenceForCurrentItem, metadata.Locus.Name);
                                    }
                                }
                                catch
                                {
                                    throw new Exception(Properties.Resources.GenbankMetadataParseError);
                                }
                            }
                        }
                        else if (formatterUsed is GffFormatter)
                        {
                            rangesInCurrentSequenceItem = GetRanges(currentSequenceItem.MetadataAddress);
                            if (rangesInCurrentSequenceItem.Count > 0 && sequenceForCurrentItem != null)
                            {
                                ExcelSelectionParser.RangeToGffMetadata(sequenceForCurrentItem, rangesInCurrentSequenceItem);
                            }
                        }

                        // Add the parsed sequence to the list of parsed sequences
                        parsedSequences.Add(sequenceForCurrentItem);
                    }
                    catch
                    {
                        // Set error status on item and re-throw
                        currentSequenceItem.SetErrorStatus(true);
                        throw;
                    }
                }

                // On successful parsing...
                if (inputSequenceSelectionComplete != null)
                    inputSequenceSelectionComplete(parsedSequences, this.argsForCallback);
                selectionDialog.InputSelectionDialogSubmitting -= OnInputSequenceDialogSubmit;
                selectionDialog.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                selectionDialog.ShowDialog();
            }
        }

        /// <summary>
        /// Method to change the wait cursor to normal once the window is loaded.
        /// </summary>
        private void OnWPFWindowActivated(object sender, EventArgs e)
        {
            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlDefault;
        }

        /// <summary>
        /// Returns a string to be used as a key for caching column mapping of BED files.
        /// </summary>
        /// <param name="range">Range for which the mapping should be returned</param>
        /// <returns>String to be used as a key for storing column mapping information</returns>
        private string GetMappingKey(Range range)
        {
            string[] split = range.Address.Replace("$", "").Split(':');
            if (split.Length == 2) // If range address is in format Sheet!Ax:Bx
            {
                return range.Worksheet.Name +
                    split[0][0] + // Column name of cell where range starts
                    split[1][0]; // Column name of cell where range ends
            }
            else if (split.Length == 1) // If range address is in format Sheet!Ax
            {
                return range.Worksheet.Name +
                    split[0][0]; // Column name of the cell
            }
            else
            {
                throw new FormatException(Resources.SelectionModel_ParsingFailed);
            }
        }

        /// <summary>
        /// Invokes the BED wizard and creates a mapping which will be used for parsing the BED data.
        /// </summary>
        /// <param name="selectedRange">Range for which mapping is required</param>
        /// <returns>Mapping in the format of Dictionary(column number,BED header)</returns>
        private Dictionary<int, string> GetMappingForRange(Range selectedRange, out bool firstRowIsHeader)
        {
            Dictionary<int, string> columnheaderMapping;
            List<int> columnNumbers = new List<int>(); // List of columns for the user to map to a header

            columnheaderMapping = TryGetColumnHeaders(selectedRange);
            if (columnheaderMapping != null)
            {
                // Headers are present
                firstRowIsHeader = true;
                return columnheaderMapping;
            }
            else
            {
                columnheaderMapping = new Dictionary<int, string>();

                //Headers not present in selection, show the wizard
                foreach (Range currentRange in selectedRange.Columns) // load all columns in the selected range
                {
                    if (currentRange.EntireColumn.Hidden == false) // load only if its not hidden
                    {
                        columnNumbers.Add(currentRange.Column);
                    }
                }

                Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlWait;
                BedWizardDialog bedWizard = new BedWizardDialog(selectedRange.Worksheet.Name, columnNumbers, rangeHeaders, requiredHeaders);
                bedWizard.Activated += new EventHandler(OnWPFWindowActivated);
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(bedWizard);
                helper.Owner = (IntPtr)Globals.ThisAddIn.Application.Hwnd;
                if (bedWizard.Show())
                {
                    try
                    {
                        foreach (var map in bedWizard.Mapping)
                        {
                            columnheaderMapping.Add(map.Value, map.Key); // swap the mapping as further operations are expecting this format.
                        }
                    }
                    catch (ArgumentException)
                    {
                        throw new ArgumentException(Properties.Resources.SelectionModel_BED_ColumnMappedMoreThanOnce);
                    }

                    firstRowIsHeader = bedWizard.IsHeaderPresent;
                    return columnheaderMapping;
                }
                else
                {
                    firstRowIsHeader = false;
                    return null;
                }
            }
        }

        /// <summary>
        /// This method checks if all headers are present
        /// and not repeated and they have been spelled correctly.
        /// </summary>
        /// <param name="selectedRange">Range on which to search for Headers</param>
        /// <returns>true if all headers are fine and false if absent.</returns>
        private Dictionary<int, string> TryGetColumnHeaders(Range selectedRange)
        {
            Dictionary<int, string> headers = new Dictionary<int, string>();
            foreach (Range column in selectedRange.Columns)
            {
                if (column.EntireColumn.Hidden == false) // skip if hidden
                {
                    headers.Add(column.Column, column.Cells[1, 1].Value2 as string != null ? (column.Cells[1, 1].Value2 as string).Trim() : "");
                }
            }

            // Check if there are duplicates in headers
            if (headers.Values.Distinct().Count() != headers.Count)
            {
                return null;
            }

            // Check if all existing headers are spelled correctly
            foreach (var header in headers)
            {
                if (!rangeHeaders.Contains(header.Value))
                {
                    return null;
                }
            }

            // Check for required headers
            foreach (string requiredHeader in requiredHeaders)
            {
                if (!headers.Values.Contains(requiredHeader))
                    return null;
            }

            return headers;
        }

        /// <summary>
        /// This method updates the column header to match the .NET Bio headings for displaying
        /// sequence range sheet.
        /// </summary>
        /// <param name="targetRange">Range which points to starting cell of the header.</param>
        /// <param name="columnheaderMapping">Mapping of column v\s header.</param>
        private void UpdateColumnHeaders(Range targetRange, Dictionary<int, string> columnheaderMapping)
        {
            foreach (var currentMapping in columnheaderMapping)
            {
                Range header = targetRange.Worksheet.Cells[targetRange.Row, currentMapping.Key];
                header.Value2 = currentMapping.Value;
            }
        }

        /// <summary>
        /// Gets individual range objects from a range address which may have multiple ranges
        /// </summary>
        /// <param name="rangeString">Range address to be parsed</param>
        /// <returns>List of Range objects</returns>
        public static List<Range> GetRanges(string rangeString)
        {
            Worksheet tmpWorksheet; // Point to a sheet temporarily while creating ranges
            List<Range> identifiedRanges = new List<Range>();
            string[] sheetReferences = rangeString.Split(','); // split up of multiple range selections in multiple sheets

            foreach (string sheetref in sheetReferences)
            {
                int lastExclamationIndex = sheetref.LastIndexOf('!');

                // See if exclamation is present or not
                if (lastExclamationIndex == -1)
                {
                    throw new FormatException(Resources.InputSelection_InvalidRange);
                }
                else
                {
                    string sheetName = sheetref.Substring(0, lastExclamationIndex).Trim('\'');
                    string[] rangeCells = sheetref.Substring(lastExclamationIndex + 1).Split(new char[] { '!', ':' });

                    // make sure there was a sheet reference and a range part which has a start cell and a end cell in the string ex sheet1 ! A1 : B2
                    if (rangeCells.Length == 1 || rangeCells.Length == 2)
                    {
                        // Get the range and add to ranges list
                        try
                        {
                            tmpWorksheet = (Globals.ThisAddIn.Application.Worksheets[sheetName] as Worksheet);
                            identifiedRanges.Add(tmpWorksheet.get_Range(rangeCells[0], rangeCells.Length == 2 ? rangeCells[1] : Type.Missing));
                        }
                        catch
                        {
                            // Failed to get the specified range from sheet for some reason
                            throw new FormatException(Resources.InputSelection_InvalidRange);
                        }
                    }
                    else
                    {
                        throw new FormatException(Resources.InputSelection_InvalidRange);
                    }
                }
            }

            return identifiedRanges;
        }

        /// <summary>
        /// This method displays the selection helper window where user can make selection on excel using mouse.
        /// </summary>
        /// <param name="selectionDialog">InputSelectionDialog instance which fired this call</param>
        private void ShowSelectionHelper(ISelectionDialog selectionDialog)
        {
            callerDialog = selectionDialog;
            SelectionHelper selectionHelperWindow = new SelectionHelper(OnSelectionComplete);
            selectionHelperWindow.SelectedAddress = selectionDialog.SelectedItem.FocusedTextBox.Text;

            // hook helper window as excel window's child
            nativeExcelWindow = new NativeWindow();
            IntPtr excelWindow = new IntPtr(Globals.ThisAddIn.Application.Hwnd);
            nativeExcelWindow.AssignHandle(excelWindow);
            selectionHelperWindow.Show(nativeExcelWindow);
            selectionHelperWindow.Focus();
        }

        /// <summary>
        /// Method to complete user selection using selection helper and display the caller window (InputSelectionDialog)
        /// </summary>
        /// <param name="selectedAddress">Selected address by user</param>
        /// <param name="submitSelected">Value indicating if the user cancelled the window or Ok</param>
        private void OnSelectionComplete(string selectedAddress, bool submitSelected)
        {
            nativeExcelWindow.ReleaseHandle();
            if (submitSelected)
            {
                callerDialog.SelectedItem.FocusedTextBox.Text = selectedAddress;
                callerDialog.SelectedItem.FocusedTextBox.Focus();
            }
            callerDialog.ShowDialog();
        }

        /// <summary>
        /// Gives out the address as a string for a particular range object.
        /// </summary>
        /// <param name="target">Range object</param>
        /// <returns>Address of the given range</returns>
        public static string GetRangeAddress(Range target)
        {
            StringBuilder rangeAddress = new StringBuilder();
            foreach (Range currentRange in target.Areas)
            {
                rangeAddress.Append(currentRange.Worksheet.Name);
                rangeAddress.Append("!");
                rangeAddress.Append(currentRange.Address.Replace("$", ""));
                rangeAddress.Append(",");

            }
            rangeAddress.Remove(rangeAddress.Length - 1, 1);
            return rangeAddress.ToString();
        }
    }
}
