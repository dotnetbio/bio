namespace SequenceAssembler
{
    #region -- Using Directives --

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Bio;
    using Bio.Algorithms.Assembly;
    using Bio.Algorithms.Alignment;

    #endregion -- Using Directives --

    /// <summary>
    /// Interaction logic for ConsensusCustomView.xaml.
    /// ConsensusCustomView would contain the Consensus custom view logic.
    /// It contains the top view with Graphical representation
    /// and the bottom view with Textual Sequence representation. It would have populate 
    /// </summary>
    public partial class ConsensusCustomView : UserControl
    {
        #region -- Private Members --

        /// <summary>
        /// Stores reference to the Sequences currently being displayed.
        /// </summary>
        private IList<ISequence> selectedSequences;

        /// <summary>
        /// Stores reference to the Contig currently being displayed.
        /// </summary>
        private Contig selectedContig;

        /// <summary>
        /// Stores reference to the IAlignedSequence currently being displayed.
        /// </summary>
        private IAlignedSequence selectedAlignment;

        /// <summary>
        /// Stores the start value of the range slider.
        /// </summary>
        private int rangeStart;

        /// <summary>
        /// Stores the end value of the range slider.
        /// </summary>
        private int rangeEnd;

        /// <summary>
        /// Dictionary key for getting offset values
        /// </summary>
        private const string StartOffsetString = "StartOffsets";

        /// <summary>
        /// Dictionary key for getting offset values
        /// </summary>
        private const string EndOffsetString = "EndOffsets";

        #endregion -- Private Members --

        #region -- Public Events --
        
        /// <summary>
        /// Event to inform the Sequence has been removed
        /// </summary>
        public event EventHandler<RemoveSequenceEventArgs> RemoveSequence;

        #endregion

        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the ConsensusCustomView class.
        /// </summary>
        public ConsensusCustomView()
        {
            this.InitializeComponent();
            this.customView.Visibility = Visibility.Hidden;
            this.ghostText.Visibility = Visibility.Visible;
            this.rangeSlider.RangeSelectionChanged += new EventHandler<RangeSelectionChangedEventArgs>(this.OnRangeChanged);
            this.topViewLines.SizeChanged += new SizeChangedEventHandler(this.OnConsensusCustomViewSizeChanged);
            this.sequencesBottomView.SequenceRemoving += new EventHandler<RemoveSequenceEventArgs>(OnSequencesBottomViewSequenceRemoving);
            this.ReadColorScheme();
            this.ApplyCurrentColor();

            this.LayoutUpdated += new EventHandler(OnConsensusCustomViewLayoutUpdated);
        }

        #endregion -- Constructor --

        #region -- Properties --

        /// <summary>
        /// Gets or Sets if an item can be removed from the view by the user
        /// </summary>
        public bool AllowRemove
        {
            get
            {
                return this.sequencesBottomView.AllowRemove;
            }
            set
            {
                this.sequencesBottomView.AllowRemove = value;
            }
        }

        /// <summary>
        /// Gets or Sets the highlighted sequence
        /// </summary>
        public ISequence HighlightedSequence
        {
            get
            {
                return sequencesBottomView.HighlightedSequence;
            }
            set
            {
                sequencesBottomView.HighlightedSequence = value;
                topViewLines.HighlightedSequence = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the legend bar is visible.
        /// </summary>
        public bool IsLegendVisible
        {
            get
            {
                return legendRow.Height.Value > 0;
            }
            set
            {
                legendRow.Height = value ? GridLength.Auto : new GridLength(0);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the top view containing the sequence lines visible or not.
        /// </summary>
        public bool IsTopViewVisible
        {
            get
            {
                return topViewRow.Height.Value > 0;
            }
            set
            {
                topViewRow.Height = value ? new GridLength(.45, GridUnitType.Star) : new GridLength(0);
                consensusSequenceSplitter.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                funnelLeftLine.Visibility = System.Windows.Visibility.Hidden;
                funnelRightLine.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        /// <summary>
        /// Gets or sets the Selected Sequences on the sequences tree
        /// </summary>
        public IList<ISequence> SelectedSequences
        {
            get
            {
                return this.selectedSequences;
            }

            set
            {
                this.selectedSequences = value;
                this.selectedContig = null;
                this.selectedAlignment = null;
                if (this.SelectedSequences != null)
                {
                    this.sequencesBottomView.SetDataSource(this.SelectedSequences);
                    PlotTopView();
                    rangeSlider.SetRange(sequencesBottomView.TotalColumns);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Selected Contig on the consensus tree
        /// </summary>
        public Contig SelectedContig
        {
            get
            {
                return this.selectedContig;
            }

            set
            {
                this.selectedContig = value;
                this.selectedAlignment = null;
                this.selectedSequences = null;
                if (this.selectedContig != null)
                {
                    this.sequencesBottomView.IsMetadataVisible = false;
                    this.sequencesBottomView.SetDataSource(this.SelectedContig);
                    PlotTopView();
                    rangeSlider.SetRange(sequencesBottomView.TotalColumns);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Selected Contig on the consensus tree
        /// </summary>
        public IAlignedSequence SelectedAlignment
        {
            get
            {
                return this.selectedAlignment;
            }

            set
            {
                this.selectedAlignment = value;
                this.selectedContig = null;
                this.selectedSequences = null;
                if (this.selectedAlignment != null)
                {
                    this.sequencesBottomView.IsMetadataVisible = false;
                    this.sequencesBottomView.SetDataSource(this.selectedAlignment);
                    PlotTopView();
                    rangeSlider.SetRange(sequencesBottomView.TotalColumns);
                }
            }
        }

        /// <summary>
        /// Zoom / Font size of the sequence viewer
        /// Minimum value is 2
        /// </summary>
        public int SequenceViewZoomFactor
        {
            get { return (int)this.sequencesBottomView.FontSize; }
            set
            {
                if (value < 2)
                {
                    this.sequencesBottomView.FontSize = 2;
                }
                else
                {
                    this.sequencesBottomView.FontSize = value;
                }
            }
        }

        /// <summary>
        /// Gets or Sets the text to be displayed when there is no seuqnce to display
        /// </summary>
        public string IdleText
        {
            get { return (string)GetValue(IdleTextProperty); }
            set { SetValue(IdleTextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IdleText.
        /// </summary>
        public static readonly DependencyProperty IdleTextProperty =
            DependencyProperty.Register("IdleText", typeof(String), typeof(ConsensusCustomView), new FrameworkPropertyMetadata("Idle text goes here.."));
        
        #endregion -- Properties --

        #region -- Public Members --

        /// <summary>
        /// Clears the top view of the Consensus view
        /// </summary>
        public void ClearTopView()
        {
            if (this.selectedSequences != null && this.selectedSequences.Count == 0)
            {
                this.selectedSequences = null;
            }

            this.topViewLines.Clear();
            this.ghostText.Visibility = Visibility.Visible;
            this.customView.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// This method would plot the consensus contigs and sequences 
        /// on the top view of the custom view
        /// </summary>
        public void PlotTopView()
        {
            if (this.selectedContig != null || this.selectedSequences != null || this.selectedAlignment != null)
            {
                // remove the ghost text and show the custom view
                this.ghostText.Visibility = Visibility.Collapsed;
                this.customView.Visibility = Visibility.Visible;
            }

            if (IsTopViewVisible == false)
            {
                // No need of further processing if its hiden
                return;
            }

            if (this.SelectedContig != null) // If there is a contig selected
            {
                this.consensusSequence.Visibility = Visibility.Visible;
                this.consensusLine.Visibility = Visibility.Visible;

                // Set start and end of consensus line
                consensusLine.X1 = (this.consensusSequence.ActualWidth * sequencesBottomView.BasePaddingLeft) / this.sequencesBottomView.TotalColumns;
                consensusLine.X2 = ((this.consensusSequence.ActualWidth * (this.SelectedContig.Consensus.Count + sequencesBottomView.BasePaddingLeft)) / this.sequencesBottomView.TotalColumns);

                topViewLines.SetDataSource(this.SelectedContig, this.sequencesBottomView.TotalColumns, sequencesBottomView.BasePaddingLeft);
            }
            else if (this.SelectedSequences != null) // If there are sequences
            {
                this.consensusLine.Visibility = Visibility.Hidden;
                topViewLines.SetDataSource(this.SelectedSequences, this.sequencesBottomView.TotalColumns);
            }
            else if (this.SelectedAlignment != null) // if alignment selected
            {
                this.consensusSequence.Visibility = System.Windows.Visibility.Visible;
                this.consensusLine.Visibility = Visibility.Hidden;

                topViewLines.SetDataSource(this.SelectedAlignment, this.sequencesBottomView.TotalColumns);
            }
            else
            {
                consensusLine.Visibility = Visibility.Collapsed;
                this.ClearTopView();
            }
        }

        #endregion

        #region -- Private Methods --

        /// <summary>
        /// Updates the inverted funnel every time there is a layout change
        /// </summary>
        private void OnConsensusCustomViewLayoutUpdated(object sender, EventArgs e)
        {
            if (IsTopViewVisible == false)
                return;

            try
            {
                Point[] rangeSliderPoints = this.rangeSlider.rangeSliderBase.GetSliderPositionRelative(customView);

                funnelLeftLine.Visibility = System.Windows.Visibility.Visible;
                funnelRightLine.Visibility = System.Windows.Visibility.Visible;

                funnelLeftLine.X2 = rangeSliderPoints[0].X;
                funnelLeftLine.Y2 = rangeSliderPoints[1].Y;
                funnelRightLine.X2 = rangeSliderPoints[1].X;
                funnelRightLine.Y2 = rangeSliderPoints[1].Y;

                if (sequencesBottomView.RightMostSequence != null)
                {
                    double lastSequencePoint = sequencesBottomView.RightMostSequence.sequenceItemsBorder.TransformToAncestor(customView)
                                .Transform(new Point(0, 0)).X + sequencesBottomView.RightMostSequence.sequenceItemsBorder.ActualWidth;
                    Point bottomViewPoint = sequencesBottomView.TransformToAncestor(customView)
                                .Transform(new Point(0, 0));

                    funnelLeftLine.X1 = bottomViewPoint.X;
                    funnelLeftLine.Y1 = bottomViewPoint.Y - consensusSequenceSplitter.Height;

                    funnelRightLine.X1 = lastSequencePoint;
                    funnelRightLine.Y1 = bottomViewPoint.Y - consensusSequenceSplitter.Height;
                }
            }
            catch
            {
                // Incase of error hide the lines
                funnelLeftLine.Visibility = System.Windows.Visibility.Hidden;
                funnelRightLine.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        /// <summary>
        /// This method gets the default color for the alphabets
        /// in case the config entry for the alphabet is missing or
        /// is corrupt. 
        /// </summary>
        /// <param name="consensusAlphabet">
        /// Indicates whether the alphabet is consensus alphabet
        /// or sequence alphabet.
        /// </param>
        /// <returns>
        /// Returns default color depending on whether the
        /// alphabet is a consenus alphabet or a sequence alphabet
        /// </returns>
        private static Brush GetDefaultColor(bool consensusAlphabet)
        {
            if (consensusAlphabet)
            {
                return new SolidColorBrush(Colors.Blue);
            }
            else
            {
                return new SolidColorBrush(Colors.Green);
            }
        }

        /// <summary>
        /// This method attempts to convert the specified string to a System.Windows.Media.Color. 
        /// If not successful this method will return default colors.
        /// </summary>
        /// <param name="consensusAlphabet">
        /// Indicates whether the alphabet is consensus alphabet
        /// or sequence alphabet.
        /// </param>
        /// <param name="colorString">String which holds a color name</param>
        /// <returns>Instance of System.Windows.Media.Color.</returns>
        private Brush GetColorFromString(bool consensusAlphabet, string colorString)
        {
            Color color;
            if (SequenceAssembly.colorLookUpTable.ContainsKey(colorString))
            {
                color = SequenceAssembly.colorLookUpTable[colorString];
                return new SolidColorBrush(color);
            }

            try
            {
                color = (Color)ColorConverter.ConvertFromString(colorString);
                SequenceAssembly.colorLookUpTable.Add(colorString, color);
                return new SolidColorBrush(color);
            }
            catch (Exception)
            {
                return GetDefaultColor(consensusAlphabet);
            }
        }

        /// <summary>
        /// This event is fired when the size of the window is changed.
        /// </summary>
        /// <param name="sender">Custom View</param>
        /// <param name="e">Event data</param>
        private void OnConsensusCustomViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.rangeSlider.Height = this.consensusGrid.ActualHeight;
        }

        /// <summary>
        /// The event would raise an event to inform that sequence has been removed, 
        /// would change the style of the associated treeview item
        /// </summary>
        /// <param name="sender">Framework Element</param>
        /// <param name="e">Routed event args</param>
        void OnSequencesBottomViewSequenceRemoving(object sender, RemoveSequenceEventArgs e)
        {
            if (this.RemoveSequence != null)
            {
                this.RemoveSequence(this, e);
            }

            if (this.selectedSequences == null || this.selectedSequences.Count == 0)
            {
                this.ClearTopView();
            }
        }

        /// <summary>
        /// This method is called when the range in the range slider changes.
        /// </summary>
        /// <param name="sender">Range slider.</param>
        /// <param name="e">Event data.</param>
        private void OnRangeChanged(object sender, RangeSelectionChangedEventArgs e)
        {
            this.rangeStart = (int)e.NewRangeStart;
            this.rangeEnd = (int)e.NewRangeStop;
            
            this.sequencesBottomView.DisplayStartIndex = rangeStart;
            this.sequencePanelHorizontalScrollBar.Value = rangeStart;
        }

        /// <summary>
        /// This method changes the color scheme of the current sequences view
        /// displayed.
        /// </summary>
        public void ApplyCurrentColor()
        {
            if (SequenceAssembly.chosenColorScheme != null && SequenceAssembly.chosenColorScheme.ColorMapping != null)
            {
                Dictionary<char, Brush> colorMapping = new Dictionary<char, Brush>();

                // Create color mapping to sent it to sequence viewer panel
                foreach (var colorKey in SequenceAssembly.chosenColorScheme.ColorMapping.Keys)
                {
                    if (colorKey.ToString() == "Default")
                    {
                        colorMapping.Add('*', GetColorFromString(true, SequenceAssembly.chosenColorScheme.ColorMapping[colorKey].ToString()));
                    }
                    else
                    {
                        colorMapping.Add(colorKey.ToString()[0], GetColorFromString(true, SequenceAssembly.chosenColorScheme.ColorMapping[colorKey].ToString()));
                    }
                }

                sequencesBottomView.ColorMap = colorMapping;
            }
            else
            {
                sequencesBottomView.ColorMap = null;
            }
        }

        /// <summary>
        /// Depending on the color scheme chosen by the user, this method reads the appropriate
        /// section from the config file which has color scheming info.
        /// </summary>
        private void ReadColorScheme()
        {
            SequenceAssembly.colorSchemeInfo = ConfigurationManager.GetSection("Colors") as List<ColorSchemeInfo>;

            if (SequenceAssembly.colorSchemeInfo != null && SequenceAssembly.colorSchemeInfo.Count > 0)
            {
                SequenceAssembly.chosenColorScheme = SequenceAssembly.colorSchemeInfo[0];
            }
        }

        /// <summary>
        /// Update the sequence view when the user scrolls the horizontal scroll bar.
        /// </summary>
        /// <param name="sender">Caller of this event</param>
        /// <param name="e">Property changed event args</param>
        private void OnSequencePanelHorizontalScrollBarValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sequencesBottomView.DisplayStartIndex = (int)e.NewValue;
            this.rangeSlider.RangeStartSelected = sequencesBottomView.DisplayStartIndex;
        }

        /// <summary>
        /// Updates scroll bar values when there is a change in the layout
        /// </summary>
        private void UpdateSequencesViewScrollBars()
        {
            sequencePanelVerticalScrollBar.Maximum = sequencesBottomView.TotalRows;
            sequencePanelHorizontalScrollBar.Maximum = Math.Max(sequencesBottomView.TotalColumns - sequencesBottomView.MaxVisibleCharCount, 0);

            // Shift one page on large change
            sequencePanelVerticalScrollBar.LargeChange = this.sequencesBottomView.MaxVisibleItemCount;
            sequencePanelHorizontalScrollBar.LargeChange = this.sequencesBottomView.MaxVisibleCharCount;

            // -1 because many times the last char gets cut off coz of float rounding issues.
            if (this.sequencesBottomView.MaxVisibleCharCount > 0)
                rangeSlider.RangeWidth = Math.Max(this.sequencesBottomView.MaxVisibleCharCount - 1, 0);
        }

        private void OnSequencesBottomViewLayoutUpdated(object sender, EventArgs e)
        {
            UpdateSequencesViewScrollBars();
        }

        /// <summary>
        /// Updates scroll bar values when there is a change in the layout
        /// </summary>
        private void UpdateTopViewScrollBars()
        {
            topViewVerticalScrollBar.Maximum = topViewLines.TotalRows;
            topViewVerticalScrollBar.LargeChange = topViewLines.MaxVisibleItemCount;
        }

        private void OnTopViewLayoutUpdated(object sender, EventArgs e)
        {
            UpdateTopViewScrollBars();
        }

        #endregion -- Private Methods --
    }
}
