using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Bio;
using Bio.Algorithms.Alignment;
using Bio.Algorithms.Assembly;

namespace SequenceAssembler
{
    /// <summary>
    /// A panel which can draw a list of sequences.
    /// Enables data virtualization by accessing only those items which can be drawn at a given time.
    /// </summary>
    public class SequencesPanel : Panel
    {
        /// <summary>
        /// Source sequence list
        /// </summary>
        private IList<ISequence> sequenceList = new List<ISequence>();

        /// <summary>
        /// Rightmost point where a sequence alphabet is plotted, used to find the right most sequence in UI
        /// </summary>
        private double lastPlotPoint = 0;

        /// <summary>
        /// Structure to hold some properties of each sequence internally
        /// </summary>
        private struct SequenceProperties
        {
            /// <summary>
            /// Position on consensus where the sequence starts aligning
            /// </summary>
            public long AlignPosition;

            /// <summary>
            /// Whether the sequence was complemented in order to find sufficient overlap.
            /// </summary>
            public bool IsComplemented;

            /// <summary>
            /// Whether the orientation of the sequence was reversed in order to find
            /// sufficient overlap.
            /// </summary>
            public bool IsReversed;

            /// <summary>
            /// Position of the Read in alignment.
            /// </summary>
            public long ReadStartAlignPosition;

            /// <summary>
            /// Length of alignment between read and contig.
            /// </summary>
            public long AlignmentLength;

            /// <summary>
            /// Is this data from an alignment or not
            /// </summary>
            public bool IsAlignment;
        }

        /// <summary>
        /// List of properties of each sequence in the source list.
        /// </summary>
        private List<SequenceProperties> sequencePropertiesList = new List<SequenceProperties>();

        /// <summary>
        /// Dictionary key for getting offset values
        /// </summary>
        private const string StartOffsetString = "StartOffsets";

        /// <summary>
        /// Dictionary key for getting offset values
        /// </summary>
        private const string EndOffsetString = "EndOffsets";

        /// <summary>
        /// Reference sequence instance if applicable
        /// </summary>
        private ISequence referenceSequence;

        /// <summary>
        /// SequenceLine instance which holds the reference sequence
        /// </summary>
        private SequenceLine referenceSequenceLine;

        /// <summary>
        /// Container for drawing scale values in the bottom view
        /// </summary>
        private Border scaleValuesContainer;

        /// <summary>
        /// List of rectanges which forms the scale grid
        /// </summary>
        private List<Line> gridLines = new List<Line>();

        /// <summary>
        /// Thickness of grid lines
        /// </summary>
        private double gridLineWidth = 1;

        /// <summary>
        /// Padding to be applied on top of the panel before starting plotting normal sequences
        /// </summary>
        private double paddingTop;

        /// <summary>
        /// List of currently displayed sequences
        /// </summary>
        private List<SequenceLine> displayedSequences = new List<SequenceLine>();

        /// <summary>
        /// Padding to be applied on left before starting plotting sequences
        /// </summary>
        public long BasePaddingLeft { get; private set; }

        /// <summary>
        /// Sequence Line object which is the right most one, used to draw inverted funnel
        /// </summary>
        public SequenceLine RightMostSequence { get; set; }

        /// <summary>
        /// Color mapping for each alphabet in the sequence
        /// </summary>
        public Dictionary<char, Brush> ColorMap
        {
            get { return (Dictionary<char, Brush>)GetValue(ColorMapProperty); }
            set { SetValue(ColorMapProperty, value); }
        }

        /// <summary>
        /// Holds a reference to the currently highlighted sequence
        /// </summary>
        private ISequence highlightedSequence;

        /// <summary>
        /// Gets or Sets the highlighted sequence
        /// </summary>
        public ISequence HighlightedSequence
        {
            get
            {
                return highlightedSequence;
            }
            set
            {
                if (highlightedSequence != value)
                {
                    highlightedSequence = value;
                    
                    // find the sequence and highlight it
                    foreach (UIElement child in this.Children)
                    {
                        SequenceLine seqLine = child as SequenceLine;

                        if (seqLine != null)
                        {
                            if (seqLine.ActualSequenceInside == highlightedSequence)
                            {
                                seqLine.SequenceItemsHighlight.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                seqLine.SequenceItemsHighlight.Visibility = System.Windows.Visibility.Hidden;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ColorMap.
        /// </summary>
        public static readonly DependencyProperty ColorMapProperty =
            DependencyProperty.Register("ColorMap", typeof(Dictionary<char, Brush>), typeof(SequencesPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
        
        /// <summary>
        /// Total number of rows in the source list
        /// </summary>
        public long TotalRows { get; private set; }

        /// <summary>
        /// Total columns (or lengthiest sequence) needed to plot all sequences properly
        /// </summary>
        public long TotalColumns { get; private set; }

        /// <summary>
        /// Number of rows visible at a given time
        /// </summary>
        public int MaxVisibleItemCount { get; private set; }

        /// <summary>
        /// Number of columns visible at a given time, excluding reference sequence
        /// </summary>
        public int MaxVisibleCharCount { get; private set; }

        /// <summary>
        /// Gets or Sets if an item can be removed from the view by the user
        /// </summary>
        public bool AllowRemove { get; set; }
        
        /// <summary>
        /// Start index from where to start displaying sequence data
        /// </summary>
        public static readonly DependencyProperty DisplayStartIndexProperty =
            DependencyProperty.Register(
            "DisplayStartIndex", typeof(int), typeof(SequencesPanel),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure));
        
        /// <summary>
        /// Gets or Sets index from where sequence starts displaying
        /// </summary>
        public int DisplayStartIndex
        {
            get { return (int)GetValue(DisplayStartIndexProperty); }
            set { SetValue(DisplayStartIndexProperty, value); }
        }

        /// <summary>
        /// Index of last item which will be displayed
        /// </summary>
        public int DisplayEndIndex { get; private set; }

        /// <summary>
        /// Index of the sequence in the source list which will be plotted on top (or start reading from).
        /// </summary>
        public static readonly DependencyProperty SourceStartIndexProperty =
            DependencyProperty.Register(
            "SourceStartIndex", typeof(int), typeof(SequencesPanel),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or Sets Index of the sequence in the source list which will be plotted on top (or start reading from).
        /// </summary>
        public int SourceStartIndex
        {
            get { return (int)GetValue(SourceStartIndexProperty); }
            set { SetValue(SourceStartIndexProperty, value); }
        }

        /// <summary>
        /// Font size of the sequence items
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(
            "FontSize", typeof(double), typeof(SequencesPanel),
            new FrameworkPropertyMetadata((double)4, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or Sets FontSize of the sequence items
        /// </summary>
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// Gets or Sets if to show metadata line or not
        /// </summary>
        public bool IsMetadataVisible { get; set; }

        /// <summary>
        /// Gets the width in pixels required for one sequence item
        /// </summary>
        public double CharWidth { get; private set; }

        /// <summary>
        /// Gets height in pixels required for one sequence item
        /// </summary>
        public double CharHeight { get; private set; }

        /// <summary>
        /// Event to handle removal of a sequence item from the view
        /// </summary>
        public event EventHandler<RemoveSequenceEventArgs> SequenceRemoving;

        /// <summary>
        /// Initializes a new instance of the SequencesPanel class.
        /// </summary>
        public SequencesPanel() : base()
        {
            IsMetadataVisible = true;
        }

        /// <summary>
        /// Used to set datasource if the control is to display a set of sequences. For example parsed output of a FastA file.
        /// </summary>
        /// <param name="sourceList">List of sequences</param>
        public void SetDataSource(IList<ISequence> sourceList)
        {
            this.sequenceList = sourceList;
            this.sequencePropertiesList = null;
            this.referenceSequence = null;
            this.referenceSequenceLine = null;

            DataSourceUpdated();
        }

        /// <summary>
        /// Used to set datasource if the control is to display the output of an assembly.
        /// </summary>
        /// <param name="contig">Contig retrieved after doing the assembly.</param>
        public void SetDataSource(Contig contig)
        {
            this.sequenceList = new List<ISequence>();
            this.sequencePropertiesList = new List<SequenceProperties>();

            this.referenceSequence = contig.Consensus;

            foreach (Contig.AssembledSequence currentSeq in contig.Sequences.OrderBy(s => s.Position))
            {
                sequenceList.Add(currentSeq.Sequence);
                sequencePropertiesList.Add(new SequenceProperties
                {
                    AlignPosition = currentSeq.Position,
                    ReadStartAlignPosition = currentSeq.ReadPosition,
                    AlignmentLength = currentSeq.Length == 0 ? currentSeq.Sequence.Count : currentSeq.Length,
                    IsComplemented = currentSeq.IsComplemented,
                    IsReversed = currentSeq.IsReversed
                });
            }

            DataSourceUpdated();
        }

        /// <summary>
        /// Used to set datasource if the control is to display the output of an assembly.
        /// </summary>
        /// <param name="alignment">Contig retrieved after doing the assembly.</param>
        public void SetDataSource(IAlignedSequence alignment)
        {
            this.referenceSequence = null;
            this.sequenceList = new List<ISequence>();
            this.sequencePropertiesList = new List<SequenceProperties>();
            List<int> startOffsets = null;
            List<int> endOffsets = null;

            if (alignment.Metadata.ContainsKey(StartOffsetString))
            {
                startOffsets = alignment.Metadata[StartOffsetString] as List<int>;
            }

            if (alignment.Metadata.ContainsKey(EndOffsetString))
            {
                endOffsets = alignment.Metadata[EndOffsetString] as List<int>;
            }

            if (startOffsets == null || startOffsets.Count != alignment.Sequences.Count) // remove offset info if the counts are not matching
            {
                startOffsets = null;
                endOffsets = null;
                this.sequencePropertiesList = null;
            }

            for (int i = 0; i < alignment.Sequences.Count; i++ )
            {
                ISequence currentSeq = alignment.Sequences[i];
                sequenceList.Add(currentSeq);

                if (startOffsets != null && endOffsets != null)
                {
                    sequencePropertiesList.Add(new SequenceProperties
                    {
                        AlignPosition = startOffsets[i],
                        ReadStartAlignPosition = 0,
                        AlignmentLength = startOffsets[i] + currentSeq.Count,
                        IsComplemented = false,
                        IsReversed = false,
                        IsAlignment = true
                    });
                }
            }

            DataSourceUpdated();
        }
        
        /// <summary>
        /// Updates many measures required for plotting sequences
        /// To be called any time the souce list is modified
        /// </summary>
        public void DataSourceUpdated()
        {
            CalculateBounds();
            InvalidateMeasure();
        }

        /// <summary>
        /// Calculates many measures required for plotting the sequences
        /// </summary>
        private void CalculateBounds()
        {
            TotalRows = sequenceList.Count;

            TotalColumns = 0;
            BasePaddingLeft = 0;

            if (sequencePropertiesList == null || (sequencePropertiesList.Count > 0 && sequencePropertiesList[0].IsAlignment == true))
            {
                for (int i = 0; i < sequenceList.Count; i++)
                {
                    if (sequenceList[i].Count > TotalColumns)
                    {
                        TotalColumns = sequenceList[i].Count;
                    }
                }
            }
            else
            {
                for (int i = 0; i < sequenceList.Count; i++)
                {
                    // Find the base padding
                    if (sequencePropertiesList[i].ReadStartAlignPosition > sequencePropertiesList[i].AlignPosition)
                    {
                        if (sequencePropertiesList[i].ReadStartAlignPosition - sequencePropertiesList[i].AlignPosition > BasePaddingLeft)
                        {
                            BasePaddingLeft = sequencePropertiesList[i].ReadStartAlignPosition - sequencePropertiesList[i].AlignPosition;
                        }
                    }

                    // Find max index horizontally
                    if (sequencePropertiesList[i].AlignPosition - sequencePropertiesList[i].ReadStartAlignPosition + sequenceList[i].Count > TotalColumns)
                    {
                        TotalColumns = sequencePropertiesList[i].AlignPosition - sequencePropertiesList[i].ReadStartAlignPosition + sequenceList[i].Count;
                    }
                }

                TotalColumns += BasePaddingLeft;
            }

        }

        /// <summary>
        /// Remove a sequence from the list
        /// </summary>
        /// <param name="sequenceToRemove">Sequence to be removed</param>
        public void RemoveSequence(ISequence sequenceToRemove)
        {
            int sequenceIndex = sequenceList.IndexOf(sequenceToRemove);
            if (sequenceIndex != -1)
            {
                if (sequencePropertiesList != null)
                {
                    sequencePropertiesList.RemoveAt(sequenceIndex);
                }

                RemoveSequenceEventArgs args = new RemoveSequenceEventArgs(sequenceToRemove);
                if (this.SequenceRemoving != null)
                {
                    this.SequenceRemoving(this, args);
                }
            }
        }

        /// <summary>
        /// Calculates different measures needed for drawing the panel
        /// </summary>
        /// <param name="availableSize">Available size for this panel</param>
        /// <returns>Total required size, always returns available size back.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (availableSize.Width <= 0 || availableSize.Height <= 0)
            {
                return new Size(0,0);
            }

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return new Size(100, 30);
            }

            if (sequenceList == null || sequenceList.Count == 0 || FontSize < 2)
            {
                Children.Clear();
                TextBlock noSequencePresentText = new TextBlock();
                if (FontSize < 2)
                {
                    noSequencePresentText.Text = Properties.Resource.SequencesPanel_FontSizeLow;
                }
                else
                {
                    noSequencePresentText.Text = Properties.Resource.SequencesPanel_NothingToDisplay;
                }
                noSequencePresentText.Foreground = Brushes.Gainsboro;
                Children.Add(noSequencePresentText);
                noSequencePresentText.Measure(availableSize);

                return noSequencePresentText.DesiredSize;
            }

            // Calculate font dimensions
            TextBlock charSizeBlock = new TextBlock {FontSize = FontSize};
            Children.Add(charSizeBlock);
            charSizeBlock.Text = "Wp";
            charSizeBlock.Measure(availableSize);
            CharHeight = (int)charSizeBlock.DesiredSize.Height; // Height
            charSizeBlock.Text = "W";
            charSizeBlock.Measure(availableSize);
            CharWidth = (int)charSizeBlock.DesiredSize.Width + 2; // Width

            Children.Clear();
            displayedSequences.Clear();
            gridLines.Clear();
            paddingTop = 0;

            // Calculate display start and end index
            DisplayEndIndex = ((int)Math.Floor(availableSize.Width / CharWidth)) + DisplayStartIndex;

            // Add grid
            Line gridLine;

            for (int i = 0; i < availableSize.Width; i += (int)CharWidth)
            {
                gridLine = new Line();
                gridLine.UseLayoutRounding = true;
                gridLine.X1 = 0;
                gridLine.X2 = 0;
                gridLine.Y1 = 0;
                gridLine.Y2 = availableSize.Height;
                gridLine.Height = availableSize.Height;
                gridLine.StrokeThickness = gridLineWidth;
                gridLine.Width = gridLineWidth;
                gridLine.Stroke = Brushes.Gainsboro;
                gridLines.Add(gridLine);
                Children.Add(gridLine);
                gridLine.Measure(availableSize);
            }

            // Add scale values and grid lines
            StackPanel scaleValuesPanel = new StackPanel();
            scaleValuesPanel.Orientation = Orientation.Horizontal;
            TextBlock scaleValue;
            int scaleValueText = DisplayStartIndex;
            for (int i = 0; i <= availableSize.Width; i += (int)(CharWidth * 10), scaleValueText += 10)
            {
                scaleValue = new TextBlock();
                scaleValue.FontSize = FontSize;
                scaleValue.Text = scaleValueText.ToString();
                scaleValue.ToolTip = scaleValueText.ToString();
                scaleValue.Width = (int)(CharWidth * 10);
                scaleValuesPanel.Children.Add(scaleValue); 
            }
            scaleValuesContainer = new Border();
            scaleValuesContainer.BorderThickness = new Thickness(0, 0, 0, 2);
            scaleValuesContainer.BorderBrush = Brushes.Gray;
            scaleValuesContainer.Child = scaleValuesPanel;
            scaleValuesContainer.Margin = new Thickness(0, 5, 0, 5);
            Children.Add(scaleValuesContainer);
            scaleValuesContainer.Measure(availableSize);
            paddingTop += scaleValuesContainer.DesiredSize.Height;

            // Add reference sequence to the panel
            if (referenceSequence != null)
            {
                referenceSequenceLine = new SequenceLine(referenceSequence, this);
                referenceSequenceLine.IsReferenceSequence = true;
                referenceSequenceLine.FontSize = FontSize;
                referenceSequenceLine.Margin = new Thickness(0, 0, 0, 10);

                Children.Add(referenceSequenceLine);
                referenceSequenceLine.Measure(availableSize);

                paddingTop += referenceSequenceLine.DesiredSize.Height;
            }

            // Calculate max number of items displayable on screen
            SequenceLine newSeq = new SequenceLine(sequenceList[0], this);
            newSeq.Measure(availableSize);

            MaxVisibleItemCount = Math.Max(0, ((int)Math.Floor((availableSize.Height - paddingTop) / newSeq.DesiredSize.Height))); // -1 else last sequence might be cut off partly coz of rounding issues
            MaxVisibleCharCount = Math.Max(0, (int)Math.Floor(availableSize.Width / CharWidth));

            // Add sequences
            for(int currentSourceIndex = SourceStartIndex;
                (currentSourceIndex < SourceStartIndex + MaxVisibleItemCount + 1) && sequenceList.Count > currentSourceIndex;
                currentSourceIndex++)
            {
                ISequence sequenceToAssign = sequenceList[currentSourceIndex];
                
                if (sequencePropertiesList != null)
                {
                    if (sequencePropertiesList[currentSourceIndex].IsComplemented && sequencePropertiesList[currentSourceIndex].IsReversed)
                    {
                        sequenceToAssign = sequenceToAssign.GetReverseComplementedSequence();
                    }
                    else if (sequencePropertiesList[currentSourceIndex].IsComplemented)
                    {
                        sequenceToAssign = sequenceToAssign.GetComplementedSequence();
                    }
                    else if (sequencePropertiesList[currentSourceIndex].IsReversed)
                    {
                        sequenceToAssign = sequenceToAssign.GetReversedSequence();
                    }
                }

                newSeq = new SequenceLine(sequenceToAssign, this);
                newSeq.ActualSequenceInside = sequenceList[currentSourceIndex]; // keep reference to the original sequence

                if (newSeq.ActualSequenceInside == highlightedSequence)
                {
                    newSeq.SequenceItemsHighlight.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    newSeq.SequenceItemsHighlight.Visibility = System.Windows.Visibility.Hidden;
                }

                newSeq.FontSize = FontSize;
                if (sequencePropertiesList != null)
                {
                    if (sequencePropertiesList[currentSourceIndex].IsAlignment)
                    {
                        newSeq.AlignStartIndex = 0;
                        newSeq.AlignmentLength = sequenceToAssign.Count;
                    }
                    else
                    {
                        newSeq.AlignStartIndex = sequencePropertiesList[currentSourceIndex].AlignPosition;
                        newSeq.AlignmentLength = sequencePropertiesList[currentSourceIndex].AlignmentLength;
                    }
                    newSeq.ReadAlignStartIndex = sequencePropertiesList[currentSourceIndex].ReadStartAlignPosition;
                    newSeq.IsComplemented = sequencePropertiesList[currentSourceIndex].IsComplemented;
                    newSeq.IsReversed = sequencePropertiesList[currentSourceIndex].IsReversed;
                }
                if (sequencePropertiesList != null && sequencePropertiesList[currentSourceIndex].IsAlignment)
                {
                    // +1 for align start as we need to display 1 based index
                    newSeq.ToolTip = sequenceToAssign.ID + Environment.NewLine + 
                                        Properties.Resource.StartOffset + " : " + (sequencePropertiesList[currentSourceIndex].AlignPosition + 1) + Environment.NewLine +
                                        Properties.Resource.EndOffset + " : " + sequencePropertiesList[currentSourceIndex].AlignmentLength;
                }
                else
                {
                    newSeq.ToolTip = sequenceToAssign.ID;
                }

                Children.Add(newSeq);
                newSeq.Measure(availableSize);
                displayedSequences.Add(newSeq);
            }

            return availableSize;
        }
        
        /// <summary>
        /// Arranges or plots all necessary items in the panel according to the measures calculated in the measure pass.
        /// </summary>
        /// <param name="finalSize">Final available size to plot this panel</param>
        /// <returns>Total size used by this panel</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (finalSize.Width <= 0 || finalSize.Height <= 0)
            {
                return new Size(0, 0);
            }

            // Override if running in designer mode
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return new Size(100, 30);
            }

            // If source is empty
            if (sequenceList == null || sequenceList.Count == 0 || FontSize < 2 )
            {
                this.Children[0].Arrange(new Rect(
                    (finalSize.Width - Children[0].DesiredSize.Width)/2, 
                    0, 
                    Children[0].DesiredSize.Width, 
                    (finalSize.Height - Children[0].DesiredSize.Height) / 2));

                return finalSize;
            }

            double currentY = 0;

            // Grid rectangles
            int currentGridStartX = 0;
            foreach(Line currLine in gridLines)
            {
                currLine.Arrange(new Rect(currentGridStartX, 0, gridLineWidth, finalSize.Height));
                currentGridStartX += (int)CharWidth;
            }

            // Scale
            if (scaleValuesContainer != null)
            {
                scaleValuesContainer.Arrange(new Rect(0, 0, finalSize.Width, scaleValuesContainer.DesiredSize.Height));
                currentY += scaleValuesContainer.DesiredSize.Height;
            }

            // Reference sequence
            if (referenceSequenceLine != null)
            {
                referenceSequenceLine.Arrange(new Rect(0, currentY, finalSize.Width, referenceSequenceLine.DesiredSize.Height));
                currentY += referenceSequenceLine.DesiredSize.Height;
            }

            // All other sequences
            lastPlotPoint = 0;
            foreach (SequenceLine child in displayedSequences)
            {
                child.Arrange(new Rect(0, currentY, finalSize.Width, child.DesiredSize.Height));
                currentY += child.DesiredSize.Height;

                // Find if this is the right most sequence line
                double thisPlotPoint = child.sequenceItemsBorder.ActualWidth + child.sequenceItemsBorder.Margin.Left;
                if (thisPlotPoint > lastPlotPoint)
                {
                    lastPlotPoint = thisPlotPoint;
                    RightMostSequence = child;
                }
            }

            return finalSize;
        }
    }
}
