using System.Linq;

namespace BiodexExcel.Visualizations.Common
{
    #region -- Using Directive --

    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using Bio;
    using Bio.Algorithms.Alignment;
    using Bio.Algorithms.MUMmer;
    using Bio.SimilarityMatrices;


    #endregion -- Using Directive --

    /// <summary>
    /// AssemblerDialog class will provide a pop-up to the user, which will be allow
    /// the user to configure input parameters to the Assembly process.
    /// </summary>
    public partial class AssemblyInputDialog : Window
    {
        #region -- Private Members --

        /// <summary>
        /// Flag to say if the operation is alignment or assembly
        /// </summary>
        private bool isAlignment;

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
        /// Alphabet used on the sequence to be aligned or assembled
        /// </summary>
        private IAlphabet sequenceAlphabet;

        #endregion -- Private Members --

        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the AssemblyInputDialog class.
        /// </summary>
        /// <param name="IsAlignment">Flags if the operation is alignment or assembly</param>
        /// <param name="sequenceAlphabet">Alphabet of the selected sequences</param>
        public AssemblyInputDialog(bool IsAlignment, IAlphabet sequenceAlphabet, ISequenceAligner selectedAligner = null)
        {
            this.isAlignment = IsAlignment;
            this.sequenceAlphabet = sequenceAlphabet;
            InitializeComponent();

            if (isAlignment)
            {
                thresholdsPanel.Visibility = Visibility.Hidden;
                alignerPanel.Visibility = Visibility.Collapsed;
                headingBlock.Text = Resources["AssemblyInputDialog_AlignInputParameters"].ToString();
            }

            // Add aligners to the drop down
            foreach (ISequenceAligner aligner in SequenceAligners.All.OrderBy(sa => sa.Name))
            {
                if (!IsAlignment)
                {
                    // If assembly, load only pairwise aligners
                    if (!(aligner is IPairwiseSequenceAligner))
                    {
                        continue;
                    }
                }

                alignerDropDown.Items.Add(aligner.Name);
            }

            // Select Smith-Waterman by default.
            if (selectedAligner == null)
            {
                selectedAligner =
                    SequenceAligners.All.FirstOrDefault(
                        sa => string.Compare(sa.Name, "Smith-Waterman", StringComparison.OrdinalIgnoreCase) == 0);
            }

            // Ensure aligner is in our list.
            if (selectedAligner != null && alignerDropDown.Items.Contains(selectedAligner.Name))
            {
                alignerDropDown.Text = selectedAligner.Name;
            }
            // If not, select the first algorithm present.
            else
            {
                alignerDropDown.SelectedIndex = 0;
            }

            // Load our parameters.
            LoadAlignmentArguments(alignerDropDown.Text);

            this.btnSubmit.Click += new RoutedEventHandler(this.OnSubmitButtonClicked);
            this.btnCancel.Click += new RoutedEventHandler(this.OnCancelClicked);
            this.alignerDropDown.SelectionChanged += new SelectionChangedEventHandler(OnAlignerChanged);
            this.btnSubmit.Focus();
        }

        /// <summary>
        /// Load the parameters for the new aligner selected.
        /// </summary>
        void OnAlignerChanged(object sender, SelectionChangedEventArgs e)
        {
            this.LoadAlignmentArguments(e.AddedItems[0].ToString());
        }

        /// <summary>
        /// Gets the list of alignment argument based on the algorithm selected
        /// </summary>
        /// <param name="algoName">Name of the algorithm used</param>
        /// <returns>Alignment arguments</returns>
        private IAlignmentAttributes GetAlignmentAttribute(string algoName)
        {
            IAlignmentAttributes alignmentAttributes = null;

            ISequenceAligner sequenceAligner = ChooseAlgorithm(algoName);
            this.Aligner = sequenceAligner;

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
        /// <param name="algorithmName">Nmae of the algorithm.</param>
        /// <returns>Class which instantiates the algorithm.</returns>
        private static ISequenceAligner ChooseAlgorithm(string algorithmName)
        {
            foreach (ISequenceAligner aligner in SequenceAligners.All)
            {
                if (aligner.Name.Equals(algorithmName))
                {
                    ISequenceAligner pairWise = aligner as ISequenceAligner;
                    if (pairWise != null)
                    {
                        return pairWise;
                    }
                }
            }

            return null;
        }

        #endregion -- Constructor --

        #region -- Public Properties --

        /// <summary>
        /// Gets the Match score.
        /// </summary>
        public int MatchScore
        {
            get { return this.matchScore; }
        }

        /// <summary>
        /// Gets the Mismatch score.
        /// </summary>
        public int MisMatchScore
        {
            get { return this.misMatchScore; }
        }

        /// <summary>
        /// Gets the Merge threshold.
        /// </summary>
        public double MergeThreshold
        {
            get { return this.mergeThreshold; }
        }

        /// <summary>
        /// Gets the consensus threshold.
        /// </summary>
        public double ConsensusThreshold
        {
            get { return this.consensusThreshold; }
        }

        /// <summary>
        /// Gets or sets the selected aligner
        /// </summary>
        public ISequenceAligner Aligner { get; set; }

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
        /// <param name="assemblyInput">aligner input object</param>
        /// <returns>Are parameters valid</returns>
        public AlignerInputEventArgs GetAlignmentInput()
        {
            StackPanel stkPanel = this.stkAlingerParam;
            AlignerInputEventArgs alignerInput = new AlignerInputEventArgs();
            TextBox textBox;
            int intValue;
            float floatValue;

            alignerInput.Aligner = this.Aligner;

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
                                MessageBox.Show(
                                        Properties.Resources.INVALID_TEXT
                                            + PairwiseAlignmentAttributes.GapOpenCost
                                            + Properties.Resources.VALUE_TEXT,
                                        Properties.Resources.CAPTION,
                                        MessageBoxButton.OK, MessageBoxImage.Error);

                                return null;
                            }

                            break;

                        case PairwiseAlignmentAttributes.GapExtensionCost:
                            if (int.TryParse(textBox.Text.Trim(), out intValue))
                            {
                                alignerInput.GapExtensionCost = intValue;
                            }
                            else
                            {
                                MessageBox.Show(
                                        Properties.Resources.INVALID_TEXT
                                            + PairwiseAlignmentAttributes.GapExtensionCost
                                            + Properties.Resources.VALUE_TEXT,
                                        Properties.Resources.CAPTION,
                                        MessageBoxButton.OK, MessageBoxImage.Error);

                                return null;
                            }

                            break;

                        case MUMmerAttributes.LengthOfMUM:
                            if (int.TryParse(textBox.Text.Trim(), out intValue))
                            {
                                alignerInput.LengthOfMUM = intValue;
                            }
                            else
                            {
                                MessageBox.Show(
                                        Properties.Resources.INVALID_TEXT
                                            + NUCmerAttributes.LengthOfMUM
                                            + Properties.Resources.VALUE_TEXT,
                                        Properties.Resources.CAPTION,
                                        MessageBoxButton.OK, MessageBoxImage.Error);

                                return null;
                            }

                            break;

                        case NUCmerAttributes.FixedSeparation:
                            if (int.TryParse(textBox.Text.Trim(), out intValue))
                            {
                                alignerInput.FixedSeparation = intValue;
                            }
                            else
                            {
                                MessageBox.Show(
                                        Properties.Resources.INVALID_TEXT
                                            + NUCmerAttributes.FixedSeparation
                                            + Properties.Resources.VALUE_TEXT,
                                        Properties.Resources.CAPTION,
                                        MessageBoxButton.OK, MessageBoxImage.Error);

                                return null;
                            }

                            break;

                        case NUCmerAttributes.MaximumSeparation:
                            if (int.TryParse(textBox.Text.Trim(), out intValue))
                            {
                                alignerInput.MaximumSeparation = intValue;
                            }
                            else
                            {
                                MessageBox.Show(
                                        Properties.Resources.INVALID_TEXT
                                            + NUCmerAttributes.MaximumSeparation
                                            + Properties.Resources.VALUE_TEXT,
                                        Properties.Resources.CAPTION,
                                        MessageBoxButton.OK, MessageBoxImage.Error);

                                return null;
                            }

                            break;

                        case NUCmerAttributes.MinimumScore:
                            if (int.TryParse(textBox.Text.Trim(), out intValue))
                            {
                                alignerInput.MinimumScore = intValue;
                            }
                            else
                            {
                                MessageBox.Show(
                                        Properties.Resources.INVALID_TEXT
                                            + NUCmerAttributes.MinimumScore
                                            + Properties.Resources.VALUE_TEXT,
                                        Properties.Resources.CAPTION,
                                        MessageBoxButton.OK, MessageBoxImage.Error);

                                return null;
                            }

                            break;

                        case NUCmerAttributes.SeparationFactor:
                            if (float.TryParse(textBox.Text.Trim(), out floatValue))
                            {
                                alignerInput.SeparationFactor = floatValue;
                            }
                            else
                            {
                                MessageBox.Show(
                                        Properties.Resources.INVALID_TEXT
                                            + NUCmerAttributes.SeparationFactor
                                            + Properties.Resources.VALUE_TEXT,
                                        Properties.Resources.CAPTION,
                                        MessageBoxButton.OK, MessageBoxImage.Error);

                                return null;
                            }

                            break;

                        case NUCmerAttributes.BreakLength:
                            if (int.TryParse(textBox.Text.Trim(), out intValue))
                            {
                                alignerInput.BreakLength = intValue;
                            }
                            else
                            {
                                MessageBox.Show(
                                        Properties.Resources.INVALID_TEXT
                                            + NUCmerAttributes.BreakLength
                                            + Properties.Resources.VALUE_TEXT,
                                        Properties.Resources.CAPTION,
                                        MessageBoxButton.OK, MessageBoxImage.Error);

                                return null;
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

                            if (comboBox.SelectedIndex > 0) // user selected a SM other than 'DiagonalSM'
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
                                    MessageBox.Show(
                                            Properties.Resources.INVALID_TEXT + PairwiseAlignmentAttributes.SimilarityMatrix + Properties.Resources.VALUE_TEXT,
                                            Properties.Resources.CAPTION,
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Error);

                                    return null;
                                }
                            }
                            else
                            {
                                // If DSM is selected
                                alignerInput.SimilarityMatrix = new DiagonalSimilarityMatrix(this.MatchScore, this.MisMatchScore);
                            }

                            break;

                        default:
                            break;
                    }
                }
            }

            return alignerInput;
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
            if (!ParseValue(this.txtMatchScore.Text, out this.matchScore))
            {
                MessageBox.Show(
                        Properties.Resources.INVALID_TEXT + Properties.Resources.MATCH_SCORE + Properties.Resources.VALUE_TEXT,
                        Properties.Resources.CAPTION,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                return false;
            }

            if (!ParseValue(this.txtMergeThreshold.Text, out this.mergeThreshold))
            {
                MessageBox.Show(
                        Properties.Resources.INVALID_TEXT + Properties.Resources.MERGE_THRESHOLD + Properties.Resources.VALUE_TEXT,
                        Properties.Resources.CAPTION,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                return false;
            }

            if (!ParseValue(this.txtMisMatchScore.Text, out this.misMatchScore))
            {
                MessageBox.Show(
                        Properties.Resources.INVALID_TEXT + Properties.Resources.MISMATCH_SCORE + Properties.Resources.VALUE_TEXT,
                        Properties.Resources.CAPTION,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                return false;
            }

            if (!ParseValue(this.txtConsensusThreshold.Text, out this.consensusThreshold) || this.consensusThreshold < 0 || this.consensusThreshold > 100)
            {
                MessageBox.Show(
                        Properties.Resources.INVALID_TEXT + Properties.Resources.CONSENSUS_THRESHOLD + Properties.Resources.VALUE_TEXT,
                        Properties.Resources.CAPTION,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Load the list of arguments as appropriate control on the UI.
        /// </summary>
        /// <param name="alignmentAttributes">List of Alignment parameters</param>
        private void LoadAlignmentArguments(string algoName)
        {
            IAlignmentAttributes alignmentAttributes = this.GetAlignmentAttribute(algoName);
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

                this.InvalidateVisual();
            }
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
            StringListValidator validator = alignmentAttribute.Validator as StringListValidator;
            combo.ItemsSource = validator.ValidValues;
            combo.SelectedIndex = validator.ValidValues.IndexOf(alignmentAttribute.DefaultValue);

            if (alignmentAttribute.Name == "Similarity Matrix")
            {
                combo.SelectedIndex = validator.ValidValues.IndexOf(GetDefaultSM(this.sequenceAlphabet));
            }

            parentPanel.Children.Add(block);
            parentPanel.Children.Add(combo);
        }

        /// <summary>
        /// Gets a default similarity matrix for assemblying any given sequence
        /// </summary>
        /// <param name="sequence">Sequence used to identify the molecule type and get the SM</param>
        /// <returns>Similarity matrix name</returns>
        private string GetDefaultSM(IAlphabet sequenceAlphabet)
        {
            if (sequenceAlphabet == Alphabets.DNA)
            {
                return SimilarityMatrix.StandardSimilarityMatrix.AmbiguousDna.ToString();
            }
            else if (sequenceAlphabet == Alphabets.RNA)
            {
                return SimilarityMatrix.StandardSimilarityMatrix.AmbiguousRna.ToString();
            }
            else if (sequenceAlphabet == Alphabets.Protein)
            {
                return SimilarityMatrix.StandardSimilarityMatrix.Blosum50.ToString();
            }
            else
            {
                return SimilarityMatrix.StandardSimilarityMatrix.AmbiguousDna.ToString();
            }
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

            parentPanel.Children.Add(block);
            parentPanel.Children.Add(box);
        }

        #endregion -- Private Methods --
    }
}
