namespace SequenceAssembler
{
    #region -- Using Directive --

    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using Bio;
    using Bio.Algorithms.Alignment;
    using Bio.Algorithms.Alignment.MultipleSequenceAlignment;
    using Bio.Algorithms.MUMmer;
    using Bio.SimilarityMatrices;
    using SequenceAssembler.Properties;

    #endregion -- Using Directive --

    /// <summary>
    /// AssemblerDialog class will provide a pop-up to the user, which will be allow
    /// the user to configure input parameters to the Assembly process.
    /// </summary>
    public partial class AlignerDialog : Window
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

        #endregion -- Private Members --

        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the AssemblerDialog class.
        /// </summary>
        /// <param name="alignmentAttributes">Alignment parameters</param>
        public AlignerDialog(string defaultSM)
        {
            InitializeComponent();
            this.defaultSM = defaultSM;

            // Add aligners to the drop down
            foreach (ISequenceAligner aligner in SequenceAligners.All)
            {
                cmbAlgorithms.Items.Add(aligner.Name);
            }
            cmbAlgorithms.Items.Add("PAMSAM");

            this.cmbAlgorithms.SelectedIndex = 0;

            if (this.cmbAlgorithms.Items.Count > 0)
            {
                IAlignmentAttributes alignmentAttributes = this.GetAlignmentAttribute(this.cmbAlgorithms.SelectedItem.ToString());
                this.LoadAlignmentArguments(alignmentAttributes);
            }

            this.cmbAlgorithms.SelectionChanged += new SelectionChangedEventHandler(cmbAlgorithms_SelectionChanged);

            this.Owner = Application.Current.MainWindow;
            this.btnSubmit.Click += new RoutedEventHandler(this.OnSubmitButtonClicked);
            this.btnCancel.Click += new RoutedEventHandler(this.OnCancelClicked);
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

        #endregion -- Constructor --

        #region -- Public Properties --

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

        public bool GetPamsamInput(
                StackPanel stkPanel,
                AlignerInputEventArgs alignerInput, IAlphabet alphabet)
        {
            TextBox textBox;
            int intValue;

            try
            {
                foreach (UIElement uiElement in stkPanel.Children)
                {
                    if (uiElement is TextBox)
                    {
                        textBox = uiElement as TextBox;

                        switch (textBox.Tag.ToString())
                        {
                            case PamsamAlignmentAttributes.KmerLength:
                                if (int.TryParse(textBox.Text.Trim(), out intValue))
                                {
                                    alignerInput.KmerLength = intValue;
                                }
                                else
                                {
                                    throw new ArgumentException(PamsamAlignmentAttributes.KmerLength);
                                }
                                break;

                            case PamsamAlignmentAttributes.GapOpenPenalty:
                                if (int.TryParse(textBox.Text.Trim(), out intValue))
                                {
                                    alignerInput.GapOpenPenalty = intValue;
                                }
                                else
                                {
                                    throw new ArgumentException(PamsamAlignmentAttributes.GapOpenPenalty);
                                }
                                break;

                            case PamsamAlignmentAttributes.GapExtendPenalty:
                                if (int.TryParse(textBox.Text.Trim(), out intValue))
                                {
                                    alignerInput.GapExtendPenalty = intValue;
                                }
                                else
                                {
                                    throw new ArgumentException(PamsamAlignmentAttributes.GapExtendPenalty);
                                }
                                break;
                            case PamsamAlignmentAttributes.NumberOfPartitions:
                                if (int.TryParse(textBox.Text.Trim(), out intValue))
                                {
                                    alignerInput.NumberOfPartitions = intValue;
                                }
                                else
                                {
                                    throw new ArgumentException(PamsamAlignmentAttributes.NumberOfPartitions);
                                }
                                break;
                            case PamsamAlignmentAttributes.DegreeOfParallelism:
                                if (int.TryParse(textBox.Text.Trim(), out intValue))
                                {
                                    alignerInput.DegreeOfParallelism = intValue;
                                }
                                else
                                {
                                    throw new ArgumentException(PamsamAlignmentAttributes.DegreeOfParallelism);
                                }
                                break;
                        }
                    }
                    else if (uiElement is ComboBox)
                    {
                        ComboBox comboBox = uiElement as ComboBox;

                        switch (comboBox.Tag.ToString())
                        {
                            case PamsamAlignmentAttributes.SimilarityMatrix:
                                if (null != comboBox.SelectedValue)
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

                            case PamsamAlignmentAttributes.DistanceFunctionType:
                                if (!Enum.TryParse<DistanceFunctionTypes>(comboBox.Text, out alignerInput.DistanceFunctionName))
                                {
                                    throw new ArgumentException(PamsamAlignmentAttributes.DistanceFunctionType);
                                }
                                break;
                            case PamsamAlignmentAttributes.UpdateDistanceMethodsType:
                                if (!Enum.TryParse<UpdateDistanceMethodsTypes>(comboBox.Text, out alignerInput.UpdateDistanceMethodsType))
                                {
                                    throw new ArgumentException(PamsamAlignmentAttributes.UpdateDistanceMethodsType);
                                }
                                break;
                            case PamsamAlignmentAttributes.ProfileAlignerName:
                                if (!Enum.TryParse<ProfileAlignerNames>(comboBox.Text, out alignerInput.ProfileAlignerName))
                                {
                                    throw new ArgumentException(PamsamAlignmentAttributes.ProfileAlignerName);
                                }
                                break;
                            case PamsamAlignmentAttributes.ProfileScoreFunctionName:
                                if (!Enum.TryParse<ProfileScoreFunctionNames>(comboBox.Text, out alignerInput.ProfileScoreFunctionName))
                                {
                                    throw new ArgumentException(PamsamAlignmentAttributes.ProfileScoreFunctionName);
                                }
                                break;
                        }
                    }
                }
            }
            catch (ArgumentException ae)
            {
                MessageDialogBox.Show(  Resource.INVALID_TEXT
                                        + ae.Message
                                        + Resource.VALUE_TEXT,
                                        Properties.Resource.CAPTION,
                                        MessageDialogButton.OK);

                return false;
            }

            return true;
        }


        /// <summary>
        /// Get the aligner input paramater from the controls in stack panel
        /// </summary>
        /// <param name="stkPanel">Stack panel objec</param>
        /// <param name="assemblyInput">aligner input object</param>
        /// <param name="alphabet"> Alphabet of the Selected sequences.</param>
        /// <returns>Are parameters valid</returns>
        public bool GetAlignmentInput(
                StackPanel stkPanel,
                AlignerInputEventArgs alignerInput, IAlphabet alphabet)
        {
            TextBox textBox;
            int intValue;
            float floatValue;

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
            AlignerInputEventArgs alignerInput = new AlignerInputEventArgs();

            valid = valid && GetAlignmentInput(stkAlingerParam, alignerInput, null);

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
                MessageDialogBox.Show(Properties.Resource.INVALID_TEXT + Application.Current.Resources[MatchScoreKey] + Properties.Resource.VALUE_TEXT, Properties.Resource.CAPTION, MessageDialogButton.OK);
                return false;
            }

            if (!ParseValue(this.txtMisMatchScore.Text, out this.misMatchScore))
            {
                MessageDialogBox.Show(Properties.Resource.INVALID_TEXT + Application.Current.Resources[MisMatchScoreKey] + Properties.Resource.VALUE_TEXT, Properties.Resource.CAPTION, MessageDialogButton.OK);
                return false;
            }

            return true;
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
            block.Margin = new Thickness(0, 5, 0, 0);
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
            block.Margin = new Thickness(0, 5, 0, 0);
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
            else if (sequenceAligner is PAMSAMMultipleSequenceAligner)
            {
                alignmentAttributes = new PamsamAlignmentAttributes();
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
                    return aligner;
                }
            }

            if (algorithmName == Properties.Resource.PAMSAM)
            {
                return new PAMSAMMultipleSequenceAligner();
            }

            return null;
        }

        #endregion -- Private Methods --
    }
}
