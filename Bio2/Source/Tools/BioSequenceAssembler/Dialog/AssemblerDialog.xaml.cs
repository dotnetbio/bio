namespace SequenceAssembler
{
    #region -- Using Directive --

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Bio;
    using Bio.Algorithms.Alignment;
    using Bio.Algorithms.Assembly.Padena;
    using Bio.Algorithms.MUMmer;
    using Bio.SimilarityMatrices;
    using SequenceAssembler.Properties;

    #endregion -- Using Directive --

    /// <summary>
    /// AssemblerDialog class will provide a pop-up to the user, which will be allow
    /// the user to configure input parameters to the Assembly process.
    /// </summary>
    public partial class AssemblerDialog : Window
    {
        #region -- Private Members --

        /// <summary>
        /// Similarity matrix to be selected by default
        /// </summary>
        private string defaultSM;

        /// <summary>
        /// Holds the key to Match Score string in resource dictionary.
        /// </summary>
        private const string MatchScoreKey = "AssemblerDialog_MatchScore";

        /// <summary>
        /// Holds the key to MisMatch Score string in resource dictionary.
        /// </summary>
        private const string MisMatchScoreKey = "AssemblerDialog_MismatchScore";

        /// <summary>
        /// Holds the key to Gap cost string in resource dictionary.
        /// </summary>
        private const string GapCostKey = "AssemblerDialog_GapCost";

        /// <summary>
        /// Holds the key to Merge Threshold string in resource dictionary.
        /// </summary>
        private const string MergeThresholdKey = "AssemblerDialog_MergeThreshold";

        /// <summary>
        /// Holds the key to Consensus Threshold string in resource dictionary.
        /// </summary>
        private const string ConsensusThresholdKey = "AssemblerDialog_ConsensusThreshold";

        /// <summary>
        /// Indicates whether the use pressed submit button or not.
        /// </summary>
        private bool submit;

        /// <summary>
        /// Holds the match score.
        /// </summary>
        private int matchScore;

        /// <summary>
        /// Holds the Mismatch score.
        /// </summary>
        private int misMatchScore;

        /// <summary>
        /// Holds the Merge threshold.
        /// </summary>
        private double mergeThreshold;

        /// <summary>
        /// Holds the consensus threshold.
        /// </summary>
        private double consensusThreshold;

        /// <summary>
        /// Gets or sets the kmer length
        /// </summary>
        private int kmerLength;

        /// <summary>
        /// Gets or sets the length threshold for redundant paths purger.
        /// </summary>
        private int redundantThreshold;

        /// <summary>
        /// Gets or sets the threshold length for dangling link purger.
        /// </summary>
        private int dangleThreshold;

        /// <summary>
        /// Gets or sets value of redundancy for building scaffolds.
        /// </summary>
        private int scaffoldRedundancy;

        /// <summary>
        /// Gets or sets the Depth for graph traversal in scaffold builder step.
        /// </summary>
        private int depth;

        /// <summary>
        /// Gets or sets the ersoion threshold. 
        /// </summary>
        private double erosionThreshold;

        /// <summary>
        /// Gets or sets threashold for low coverage contig removal. 
        /// </summary>
        private double lowCoverageContigRemovalThreshold;

        /// <summary>
        /// Gets or sets mean of clone library used.
        /// </summary>
        private double cloneLibraryMean;

        /// <summary>
        /// Gets or sets SD of clone library used.
        /// </summary>
        private double cloneLibrarySd;

        #endregion -- Private Members --

        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the AssemblerDialog class.
        /// </summary>
        /// <param name="algorithms">Algorithms</param>
        /// <param name="defaultSM">Default similarity matrix</param>
        /// <param name="sequences">Sequences to assemble</param>
        public AssemblerDialog(IEnumerable<string> algorithms, string defaultSM, IList<ISequence> sequences)
        {
            InitializeComponent();
            this.defaultSM = defaultSM;

            this.InitializeAlignmentAlgorithms(algorithms);
            if (this.cmbAlgorithms.Items.Count > 0)
            {
                IAlignmentAttributes alignmentAttributes = this.GetAlignmentAttribute(this.cmbAlgorithms.SelectedItem.ToString());
                this.LoadAlignmentArguments(alignmentAttributes);
            }

            this.cmbAlgorithms.SelectionChanged += new SelectionChangedEventHandler(cmbAlgorithms_SelectionChanged);

            this.Owner = Application.Current.MainWindow;

            this.simpleSequenceAssemblerOptionButton.Checked += new RoutedEventHandler(OnAssemblerSelectionChanged);
            this.padenaOptionButton.Checked += new RoutedEventHandler(OnAssemblerSelectionChanged);
            this.btnSubmit.Click += new RoutedEventHandler(this.OnSubmitButtonClicked);
            this.btnCancel.Click += new RoutedEventHandler(this.OnCancelClicked);

            // Get default values for padena
            int estimatedKmerLength = ParallelDeNovoAssembler.EstimateKmerLength(sequences);
            txtKmerLength.Text = estimatedKmerLength.ToString();
            txtDangleThreshold.Text = (estimatedKmerLength + 1).ToString();
            txtRedundantThreshold.Text = (3 * (estimatedKmerLength + 1)).ToString();
            this.cmbLibraryNames.SelectionChanged += new SelectionChangedEventHandler(OnLibraryNames_SelectionChanged);
            this.InitializeLibraryNames();
            this.btnSubmit.Focus();
        }

        /// <summary>
        /// Selection change event of Aligner combo.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Selection Changed EventArgs.</param>
        private void cmbAlgorithms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string algo = string.Empty;
            if (e.AddedItems.Count > 0)
            {
                algo = e.AddedItems[0].ToString();
            }

            IAlignmentAttributes alignmentAttributes = this.GetAlignmentAttribute(algo);
            this.LoadAlignmentArguments(alignmentAttributes);
        }

        /// <summary>
        /// Selection change event of library combo.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Selection Chnaged EventArgs.</param>
        private void OnLibraryNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!e.AddedItems[0].ToString().Equals("Add New Library"))
            {
                CloneLibraryInformation information = CloneLibrary.Instance.GetLibraryInformation(e.AddedItems[0].ToString());
                this.txtLibraryMean.Text = information.MeanLengthOfInsert.ToString();
                this.txtLibraryName.Text = information.LibraryName.ToString();
                this.txtLibraryStandardDeviation.Text = information.StandardDeviationOfInsert.ToString();
            }
            else
            {
                this.txtLibraryMean.Text = string.Empty;
                this.txtLibraryName.Text = string.Empty;
                this.txtLibraryStandardDeviation.Text = string.Empty;
            }
        }

        #endregion -- Constructor --

        #region -- Public Properties --

        /// <summary>
        /// Gets if padena is selected as the assembler
        /// </summary>
        public AssemblerType AssemblerSelected
        {
            get
            {
                return padenaOptionButton.IsChecked == true ? AssemblerType.PaDeNA : AssemblerType.SimpleSequenceAssembler;
            }
        }

        /// <summary>
        /// Gets the Match score.
        /// </summary>
        public int MatchScore
        {
            get
            {
                return this.matchScore;
            }
        }

        /// <summary>
        /// Gets the Mismatch score.
        /// </summary>
        public int MisMatchScore
        {
            get
            {
                return this.misMatchScore;
            }
        }

        /// <summary>
        /// Gets the Merge threshold.
        /// </summary>
        public double MergeThreshold
        {
            get
            {
                return this.mergeThreshold;
            }
        }

        /// <summary>
        /// Gets the consensus threshold.
        /// </summary>
        public double ConsensusThreshold
        {
            get
            {
                return this.consensusThreshold;
            }
        }

        /// <summary>
        /// Selected Algorithm.
        /// </summary>
        public string SelectedAlgo
        {
            get
            {
                if (this.cmbAlgorithms.SelectedItem != null)
                {
                    return this.cmbAlgorithms.SelectedItem.ToString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets whether erosion is enabled.
        /// </summary>
        public bool IsErosionEnabled
        {
            get
            {
                return this.chkErosion.IsChecked == true;
            }
        }

        /// <summary>
        /// Gets whether low coverage contig removal is enabled.
        /// </summary>
        public bool IsLowCoverageContigRemovalEnabled
        {
            get
            {
                return this.chkLowCoverageRemoval.IsChecked == true;
            }
        }

        /// <summary>
        /// Gets the value of erosion threshold.
        /// </summary>
        public double ErosionThreshold
        {
            get
            {
                return this.erosionThreshold;
            }
        }

        /// <summary>
        /// Gets the value of low coverage contig removal.
        /// </summary>
        public double LowCoverageContigRemovalThreshold
        {
            get
            {
                return this.lowCoverageContigRemovalThreshold;
            }
        }

        #endregion -- Public Properties --

        #region -- Public Methods --

        /// <summary>
        /// This method displays the color dialog and waits for the user to choose the color
        /// scheme and returns the chosen color scheme back to the listener.
        /// </summary>
        /// <returns>The color scheme chosen by the user.</returns>
        public new bool Show()
        {
            this.ShowDialog();
            return this.submit;
        }

        /// <summary>
        /// Get the aligner input paramater from the controls in stack panel
        /// </summary>
        /// <param name="stkPanel">Stack panel object</param>
        /// <param name="assemblyInput">aligner input object</param>
        /// <param name="alphabet"> Alphabet of the Selected sequences.</param>
        /// <returns>Are parameters valid</returns>
        public bool GetAlignmentInput(
                StackPanel stkPanel,
                AssemblyInputEventArgs assemblyInput, IAlphabet alphabet)
        {
            AlignerInputEventArgs alignerInput;
            TextBox textBox;
            int intValue;
            float floatValue;

            alignerInput = assemblyInput.AlignerInput;

            foreach (UIElement uiElement in stkPanel.Children)
            {
                if (uiElement is TextBox)
                {
                    textBox = uiElement as TextBox;

                    switch (textBox.Tag.ToString())
                    {
                        case PairwiseAlignmentAttributes.GapOpenCost:
                            if (int.TryParse(textBox.Text.Trim(), out intValue))
                            {
                                alignerInput.GapCost = intValue;
                            }
                            else
                            {
                                MessageDialogBox.Show(
                                        Resource.INVALID_TEXT
                                            + PairwiseAlignmentAttributes.GapOpenCost
                                            + Resource.VALUE_TEXT,
                                        Properties.Resource.CAPTION,
                                        MessageDialogButton.OK);

                                return false;
                            }

                            break;

                        case PairwiseAlignmentAttributes.GapExtensionCost:
                            if (int.TryParse(textBox.Text.Trim(), out intValue))
                            {
                                alignerInput.GapExtensionCost = intValue;
                            }
                            else
                            {
                                MessageDialogBox.Show(
                                        Resource.INVALID_TEXT
                                            + PairwiseAlignmentAttributes.GapExtensionCost
                                            + Resource.VALUE_TEXT,
                                        Properties.Resource.CAPTION,
                                        MessageDialogButton.OK);

                                return false;
                            }

                            break;

                        case MUMmerAttributes.LengthOfMUM:
                            if (int.TryParse(textBox.Text.Trim(), out intValue))
                            {
                                alignerInput.LengthOfMUM = intValue;
                            }
                            else
                            {
                                MessageDialogBox.Show(
                                        Resource.INVALID_TEXT
                                            + NUCmerAttributes.LengthOfMUM
                                            + Resource.VALUE_TEXT,
                                        Properties.Resource.CAPTION,
                                        MessageDialogButton.OK);

                                return false;
                            }

                            break;

                        case NUCmerAttributes.FixedSeparation:
                            if (int.TryParse(textBox.Text.Trim(), out intValue))
                            {
                                alignerInput.FixedSeparation = intValue;
                            }
                            else
                            {
                                MessageDialogBox.Show(
                                        Resource.INVALID_TEXT
                                            + NUCmerAttributes.FixedSeparation
                                            + Resource.VALUE_TEXT,
                                        Properties.Resource.CAPTION,
                                        MessageDialogButton.OK);

                                return false;
                            }

                            break;

                        case NUCmerAttributes.MaximumSeparation:
                            if (int.TryParse(textBox.Text.Trim(), out intValue))
                            {
                                alignerInput.MaximumSeparation = intValue;
                            }
                            else
                            {
                                MessageDialogBox.Show(
                                        Resource.INVALID_TEXT
                                            + NUCmerAttributes.MaximumSeparation
                                            + Resource.VALUE_TEXT,
                                        Properties.Resource.CAPTION,
                                        MessageDialogButton.OK);

                                return false;
                            }

                            break;

                        case NUCmerAttributes.MinimumScore:
                            if (int.TryParse(textBox.Text.Trim(), out intValue))
                            {
                                alignerInput.MinimumScore = intValue;
                            }
                            else
                            {
                                MessageDialogBox.Show(
                                        Resource.INVALID_TEXT
                                            + NUCmerAttributes.MinimumScore
                                            + Resource.VALUE_TEXT,
                                        Properties.Resource.CAPTION,
                                        MessageDialogButton.OK);

                                return false;
                            }

                            break;

                        case NUCmerAttributes.SeparationFactor:
                            if (float.TryParse(textBox.Text.Trim(), out floatValue))
                            {
                                alignerInput.SeparationFactor = floatValue;
                            }
                            else
                            {
                                MessageDialogBox.Show(
                                        Resource.INVALID_TEXT
                                            + NUCmerAttributes.SeparationFactor
                                            + Resource.VALUE_TEXT,
                                        Properties.Resource.CAPTION,
                                        MessageDialogButton.OK);

                                return false;
                            }

                            break;

                        case NUCmerAttributes.BreakLength:
                            if (int.TryParse(textBox.Text.Trim(), out intValue))
                            {
                                alignerInput.BreakLength = intValue;
                            }
                            else
                            {
                                MessageDialogBox.Show(
                                        Resource.INVALID_TEXT
                                            + NUCmerAttributes.BreakLength
                                            + Resource.VALUE_TEXT,
                                        Properties.Resource.CAPTION,
                                        MessageDialogButton.OK);

                                return false;
                            }

                            break;

                        default:
                            break;
                    }
                }
                else if (uiElement is ComboBox)
                {
                    ComboBox comboBox = uiElement as ComboBox;

                    switch (comboBox.Tag.ToString())
                    {
                        case PairwiseAlignmentAttributes.SimilarityMatrix:
                            if (comboBox.SelectedValue != null && comboBox.SelectedIndex == 0) // DSM
                            {
                                int matchScore = 0;
                                int missmatchScore = 0;

                                int.TryParse(txtMatchScore.Text, out matchScore);
                                int.TryParse(txtMisMatchScore.Text, out missmatchScore);

                                alignerInput.SimilarityMatrix = new DiagonalSimilarityMatrix(matchScore, missmatchScore);
                            }

                            if (null != comboBox.SelectedValue && comboBox.SelectedIndex > 0)
                            {
                                string similarityMatrixOption = comboBox.SelectedValue.ToString();

                                if (Enum.IsDefined(
                                    typeof(SimilarityMatrix.StandardSimilarityMatrix),
                                    similarityMatrixOption))
                                {
                                    SimilarityMatrix.StandardSimilarityMatrix matrix =
                                        (SimilarityMatrix.StandardSimilarityMatrix)Enum.Parse(
                                            typeof(SimilarityMatrix.StandardSimilarityMatrix),
                                            similarityMatrixOption,
                                            true);
                                    alignerInput.SimilarityMatrix = new SimilarityMatrix(matrix);
                                }
                                else
                                {
                                    MessageDialogBox.Show(
                                            Resource.INVALID_TEXT
                                                + PairwiseAlignmentAttributes.SimilarityMatrix
                                                + Resource.VALUE_TEXT,
                                            Properties.Resource.CAPTION,
                                            MessageDialogButton.OK);

                                    return false;
                                }
                            }

                            break;

                        default:
                            break;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// This method will display the list of available alignment
        /// algorithms to the user.
        /// </summary>
        /// <param name="algorithms">List of alignment algorithms.</param>
        public void InitializeAlignmentAlgorithms(IEnumerable<string> algorithms)
        {
            if (algorithms != null)
            {
                int index = 0, count = 0;
                
                foreach (string algorithm in algorithms)
                {
                    // See if Smith Waterman is present, if so use that as our default
                    // alignment algorithm.
                    if (string.Compare("Smith-Waterman", algorithm, StringComparison.OrdinalIgnoreCase) == 0)
                        index = count;
                    count++;
                    cmbAlgorithms.Items.Add(algorithm);
                }

                if (cmbAlgorithms.Items.Count > 0)
                    cmbAlgorithms.SelectedIndex = index;
            }
        }

        /// <summary>
        /// This method will display the list of available libraries to user. 
        /// </summary>
        public void InitializeLibraryNames()
        {
            if (CloneLibrary.Instance.GetLibraries.Count > 0)
            {
                foreach (CloneLibraryInformation information in CloneLibrary.Instance.GetLibraries)
                {
                    cmbLibraryNames.Items.Add(information.LibraryName);
                }

                cmbLibraryNames.Items.Add("Add New Library");
            }

            cmbLibraryNames.SelectedIndex = 0;
        }

        /// <summary>
        /// Populate input parameters for PaDeNA
        /// </summary>
        /// <param name="input">AssemblyInputEventArgs object to which the parameters should be populated</param>
        public void GetPaDeNAInput(AssemblyInputEventArgs input)
        {
            input.AssemblerUsed = AssemblerType.PaDeNA;

            input.KmerLength = this.kmerLength;
            input.DanglingLinksThreshold = this.dangleThreshold;
            input.RedundantPathLengthThreshold = this.redundantThreshold;
            input.ErosionEnabled = this.IsErosionEnabled;

            if (this.IsErosionEnabled)
            {
                input.ErosionThreshold = this.erosionThreshold;
            }

            input.LowCoverageContigRemovalEnabled = this.IsLowCoverageContigRemovalEnabled;
            if (this.IsLowCoverageContigRemovalEnabled)
            {
                input.LowCoverageContigRemovalThreshold = this.lowCoverageContigRemovalThreshold;
            }

            if (scaffoldGenerationParameters.IsChecked == true)
            {
                input.GenerateScaffolds = true;
                input.ScaffoldRedundancy = this.scaffoldRedundancy;
                input.Depth = this.depth;
            }
            else
            {
                input.GenerateScaffolds = false;
            }
        }

        #endregion -- Public Methods --

        #region -- Private Methods --

        /// <summary>
        /// Parses a string value and converts it into
        /// double value.
        /// </summary>
        /// <param name="value">Value to be parsed.</param>
        /// <param name="result">The result of the parse operation.</param>
        /// <returns>Indicates whether the parsing was succesful or not.</returns>
        private static bool ParseValue(string value, out double result)
        {
            if (double.TryParse(value, out result))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses a string value and converts it into
        /// int value.
        /// </summary>
        /// <param name="value">Value to be parsed.</param>
        /// <param name="result">The result of the parse operation.</param>
        /// <returns>Indicates whether the parsing was succesful or not.</returns>
        private static bool ParseValue(string value, out int result)
        {
            if (int.TryParse(value, out result))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets called when Cancel button is clicked.
        /// </summary>
        /// <param name="sender">Submit button.</param>
        /// <param name="e">Event data.</param>
        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            this.submit = false;
            this.Close();
        }

        /// <summary>
        /// Gets called when submit button is clicked.
        /// </summary>
        /// <param name="sender">Submit button.</param>
        /// <param name="e">Event data.</param>
        private void OnSubmitButtonClicked(object sender, RoutedEventArgs e)
        {
            bool valid = this.ParseValues();

            // Validation required. Try to read the arguments and validate the input
            AssemblyInputEventArgs assemblerInput = new AssemblyInputEventArgs(null, null);

            if (valid && simpleSequenceAssemblerOptionButton.IsChecked == true)
            {
                assemblerInput.AlignerInput = new AlignerInputEventArgs();
                valid = GetAlignmentInput(stkAlingerParam, assemblerInput, null);
            }

            if (valid)
            {
                this.submit = true;
                this.Close();
            }
        }

        /// <summary>
        /// This method parses all the Textboxes and checks if the user
        /// has entered correct values.
        /// </summary>
        /// <returns>True in case parsing is successful, false if not.</returns>
        private bool ParseValues()
        {
            if (simpleSequenceAssemblerOptionButton.IsChecked == true)
            {
                if (!ParseValue(this.txtMatchScore.Text, out this.matchScore))
                {
                    MessageDialogBox.Show(Properties.Resource.INVALID_TEXT + Application.Current.Resources[MatchScoreKey] + Properties.Resource.VALUE_TEXT, Properties.Resource.CAPTION, MessageDialogButton.OK);
                    return false;
                }

                if (!ParseValue(this.txtMergeThreshold.Text, out this.mergeThreshold))
                {
                    MessageDialogBox.Show(Properties.Resource.INVALID_TEXT + Application.Current.Resources[MergeThresholdKey] + Properties.Resource.VALUE_TEXT, Properties.Resource.CAPTION, MessageDialogButton.OK);
                    return false;
                }

                if (!ParseValue(this.txtMisMatchScore.Text, out this.misMatchScore))
                {
                    MessageDialogBox.Show(Properties.Resource.INVALID_TEXT + Application.Current.Resources[MisMatchScoreKey] + Properties.Resource.VALUE_TEXT, Properties.Resource.CAPTION, MessageDialogButton.OK);
                    return false;
                }

                if (!ParseValue(this.txtConsensusThreshold.Text, out this.consensusThreshold))
                {
                    MessageDialogBox.Show(Properties.Resource.INVALID_TEXT + Application.Current.Resources[ConsensusThresholdKey] + Properties.Resource.VALUE_TEXT, Properties.Resource.CAPTION, MessageDialogButton.OK);
                    return false;
                }

                return true;
            }
            else
            {
                if (!ParseValue(this.txtKmerLength.Text, out this.kmerLength) || kmerLength < 0)
                {
                    MessageDialogBox.Show(Properties.Resource.INVALID_TEXT + Application.Current.Resources["AssemblerDialog_KmerLength"] + Properties.Resource.VALUE_TEXT, Properties.Resource.CAPTION, MessageDialogButton.OK);
                    return false;
                }

                if (!ParseValue(this.txtRedundantThreshold.Text, out this.redundantThreshold) || redundantThreshold < 0)
                {
                    MessageDialogBox.Show(Properties.Resource.INVALID_TEXT + Application.Current.Resources["AssemblerDialog_RedundantThreshold"] + Properties.Resource.VALUE_TEXT, Properties.Resource.CAPTION, MessageDialogButton.OK);
                    return false;
                }

                if (!ParseValue(this.txtDangleThreshold.Text, out this.dangleThreshold) || dangleThreshold < 0)
                {
                    MessageDialogBox.Show(Properties.Resource.INVALID_TEXT + Application.Current.Resources["AssemblerDialog_DangleThreshold"] + Properties.Resource.VALUE_TEXT, Properties.Resource.CAPTION, MessageDialogButton.OK);
                    return false;
                }

                if (chkErosion.IsChecked == true)
                {
                    if (!ParseValue(txtErosionThreshold.Text, out erosionThreshold) || (erosionThreshold != -1 && erosionThreshold < 0))
                    {
                        MessageDialogBox.Show(Properties.Resource.INVALID_TEXT + Application.Current.Resources["AssemblerDialog_Erosion"] + Properties.Resource.VALUE_TEXT, Properties.Resource.CAPTION, MessageDialogButton.OK);
                        return false;
                    }
                }

                if (chkLowCoverageRemoval.IsChecked == true)
                {
                    if (!ParseValue(txtLowCoverageRemovalThreshold.Text, out lowCoverageContigRemovalThreshold) || (lowCoverageContigRemovalThreshold != -1 && lowCoverageContigRemovalThreshold < 0))
                    {
                        MessageDialogBox.Show(Properties.Resource.INVALID_TEXT + Application.Current.Resources["AssemblerDialog_LowCoverageContigRemoval"] + Properties.Resource.VALUE_TEXT, Properties.Resource.CAPTION, MessageDialogButton.OK);
                        return false;
                    }
                }

                if (scaffoldGenerationParameters.IsChecked == true)
                {
                    if (!ParseValue(this.txtScaffoldRedundancy.Text, out this.scaffoldRedundancy) || scaffoldRedundancy < 0)
                    {
                        MessageDialogBox.Show(Properties.Resource.INVALID_TEXT + Application.Current.Resources["AssemblerDialog_ScaffoldRedundancy"] + Properties.Resource.VALUE_TEXT, Properties.Resource.CAPTION, MessageDialogButton.OK);
                        return false;
                    }

                    if (!ParseValue(this.txtDepth.Text, out this.depth) || depth < 0)
                    {
                        MessageDialogBox.Show(Properties.Resource.INVALID_TEXT + Application.Current.Resources["AssemblerDialog_ContigGraphTraversalDepth"] + Properties.Resource.VALUE_TEXT, Properties.Resource.CAPTION, MessageDialogButton.OK);
                        return false;
                    }

                    if (!ParseValue(this.txtLibraryMean.Text, out cloneLibraryMean))
                    {
                        MessageDialogBox.Show(Properties.Resource.INVALID_TEXT + Application.Current.Resources["AssemblerDialog_LibraryMean"] + Properties.Resource.VALUE_TEXT, Properties.Resource.CAPTION, MessageDialogButton.OK);
                        return false;
                    }

                    if (!ParseValue(this.txtLibraryStandardDeviation.Text, out cloneLibrarySd) || cloneLibrarySd < 0)
                    {
                        MessageDialogBox.Show(Properties.Resource.INVALID_TEXT + Application.Current.Resources["AssemblerDialog_LibrarySd"] + Properties.Resource.VALUE_TEXT, Properties.Resource.CAPTION, MessageDialogButton.OK);
                        return false;
                    }

                    if (string.IsNullOrEmpty(this.txtLibraryName.Text))
                    {
                        MessageDialogBox.Show(Properties.Resource.INVALID_TEXT + Application.Current.Resources["AssemblerDialog_LibraryNames"] + Properties.Resource.VALUE_TEXT, Properties.Resource.CAPTION, MessageDialogButton.OK);
                        return false;
                    }

                    CloneLibrary.Instance.AddLibrary(this.txtLibraryName.Text, (float)cloneLibraryMean, (float)cloneLibrarySd);
                }

                return true;
            }
        }

        /// <summary>
        /// Load the list of arguments as appropriate control on the UI.
        /// </summary>
        /// <param name="alignmentAttributes">List of Alignment parameters</param>
        private void LoadAlignmentArguments(IAlignmentAttributes alignmentAttributes)
        {
            stkAlingerParam.Children.Clear();
            if (null != alignmentAttributes)
            {
                foreach (KeyValuePair<string, AlignmentInfo> attribute in alignmentAttributes.Attributes)
                {
                    if (attribute.Value.DataType.Equals(AlignmentInfo.StringListType))
                    {
                        this.CreateComboField(
                            attribute.Value,
                            stkAlingerParam,
                            attribute.Key);
                    }
                    else
                    {
                        this.CreateTextField(
                            attribute.Value,
                            stkAlingerParam,
                            attribute.Key);
                    }
                }
            }

            this.UpdateLayout();
        }

        /// <summary>
        /// This method would create combo field.
        /// </summary>
        /// <param name="alignmentAttribute">Alignment parameter</param>
        /// <param name="parentPanel">Stack panel, the field will be added to this stackpanel</param>
        /// <param name="tag">Tag of the control object</param>
        private void CreateComboField(
            AlignmentInfo alignmentAttribute,
                StackPanel parentPanel,
                string tag)
        {
            TextBlock block = new TextBlock();
            block.Margin = new Thickness(0, 10, 0, 0);
            block.TextWrapping = TextWrapping.Wrap;
            block.Text = alignmentAttribute.Name;
            block.Height = 20;

            ComboBox combo = new ComboBox();
            combo.HorizontalAlignment = HorizontalAlignment.Left;
            combo.IsSynchronizedWithCurrentItem = true;
            combo.Margin = new Thickness(0, 0, 0, 0);
            combo.Tag = tag;
            combo.Width = 180;
            combo.Height = 22;
            combo.ToolTip = alignmentAttribute.Description;

            StringListValidator validator = alignmentAttribute.Validator as StringListValidator;
            combo.ItemsSource = validator.ValidValues;
            combo.SelectedIndex = validator.ValidValues.IndexOf(alignmentAttribute.DefaultValue);

            if (alignmentAttribute.Name == "Similarity Matrix")
            {
                combo.SelectedIndex = validator.ValidValues.IndexOf(this.defaultSM);
            }

            parentPanel.Children.Add(block);
            parentPanel.Children.Add(combo);
        }

        /// <summary>
        /// This method would create text field.
        /// </summary>
        /// <param name="alignmentAttribute">Alignment parameter</param>
        /// <param name="parentPanel">Stack panel, the field will be added to this stackpanel</param>
        /// <param name="tag">Tag of the control object</param>
        private void CreateTextField(
                AlignmentInfo alignmentAttribute,
                StackPanel parentPanel,
                string tag)
        {
            TextBlock block = new TextBlock();
            block.Margin = new Thickness(0, 10, 0, 0);
            block.TextWrapping = TextWrapping.Wrap;
            block.Text = alignmentAttribute.Name;
            block.Height = 20;

            TextBox box = new TextBox();
            box.HorizontalAlignment = HorizontalAlignment.Left;
            box.Margin = new Thickness(0, 0, 0, 0);
            box.TextWrapping = TextWrapping.Wrap;
            box.Text = alignmentAttribute.DefaultValue;
            box.Tag = tag;
            box.Width = 120;
            box.Height = 20;
            box.ToolTip = alignmentAttribute.Description;

            parentPanel.Children.Add(block);
            parentPanel.Children.Add(box);
        }


        /// <summary>
        /// Gets the list of alignment argument based on the algorithm selected
        /// </summary>
        /// <returns>Alignment arguments</returns>
        private IAlignmentAttributes GetAlignmentAttribute(string algo)
        {
            IAlignmentAttributes alignmentAttributes = null;

            ISequenceAligner sequenceAligner = ChooseAlgorithm(algo);

            if (sequenceAligner is NucmerPairwiseAligner)
            {
                alignmentAttributes = new NUCmerAttributes();
            }
            else if (sequenceAligner is MUMmerAligner)
            {
                alignmentAttributes = new MUMmerAttributes();
            }
            else if ((sequenceAligner is SmithWatermanAligner)
                    || (sequenceAligner is NeedlemanWunschAligner)
                    || (sequenceAligner is PairwiseOverlapAligner))
            {
                alignmentAttributes = new PairwiseAlignmentAttributes();
            }

            return alignmentAttributes;
        }

        /// <summary>
        /// This method when passed the algorithm name instantiates
        /// the framework class which implements.
        /// </summary>
        /// <param name="algorithmName">Name of the algorithm.</param>
        /// <returns>Class which instantiates the algorithm.</returns>
        private static ISequenceAligner ChooseAlgorithm(string algorithmName)
        {
            foreach (ISequenceAligner aligner in SequenceAligners.All)
            {
                if (aligner.Name.Equals(algorithmName))
                {
                    IPairwiseSequenceAligner pairWise = aligner as IPairwiseSequenceAligner;
                    if (pairWise != null)
                    {
                        return pairWise;
                    }
                }
            }

            return null;
        }

        private void OnAssemblerSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (simpleSequenceAssemblerOptionButton.IsChecked == true)
            {
                simpleSequencePart.Visibility = System.Windows.Visibility.Visible;
                padenaPart.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                simpleSequencePart.Visibility = System.Windows.Visibility.Collapsed;
                padenaPart.Visibility = System.Windows.Visibility.Visible;
            }
        }

        #endregion -- Private Methods --
    }
}
