namespace SequenceAssembler
{
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

    /// <summary>
    /// Implementation if virtualized top view
    /// </summary>
    class SequenceLinesPanel : StackPanel
    {
        /// <summary>
        /// List of sequences to be plotted
        /// </summary>
        private IList<ISequence> sequences;

        /// <summary>
        /// Contig to be plotted
        /// </summary>
        private Contig contig;

        /// <summary>
        /// Alignment to be plotted
        /// </summary>
        private IAlignedSequence alignedSequence;

        /// <summary>
        /// Padding to be used for every sequence
        /// Used in case of a contig where a sequence starts before the consensus
        /// </summary>
        private long basePaddingLeft;

        /// <summary>
        /// Total span of the sequences, from the starting of the sequence on the left, to the ending of the sequence on the right end
        /// </summary>
        private long maxLength;

        /// <summary>
        /// Ordered list of assembled sequences in the selected contig
        /// </summary>
        private IOrderedEnumerable<Contig.AssembledSequence> contigSequences;

        /// <summary>
        /// Brush used to draw the sequence line
        /// </summary>
        SolidColorBrush sequenceBrush = new SolidColorBrush(Color.FromArgb((byte)0xFF, (byte)0x42, (byte)0xAF, (byte)0x07));

        /// <summary>
        /// Brush used to draw the sequence line
        /// </summary>
        public SolidColorBrush SequenceBrush
        {
            get { return sequenceBrush; }
            set { sequenceBrush = value; }
        }

        /// <summary>
        /// Margin to be given for each sequence line
        /// </summary>
        private Thickness lineMargin = new Thickness(0, 5, 0, 0);

        /// <summary>
        /// Margin to be given for each sequence line
        /// </summary>
        public Thickness LineMargin
        {
            get { return lineMargin; }
            set { lineMargin = value; }
        }

        /// <summary>
        /// Thickness of each sequence line
        /// </summary>
        private int sequenceLineThickness = 3;

        /// <summary>
        /// Thickness of each sequence line
        /// </summary>
        public int SequenceLineThickness
        {
            get { return sequenceLineThickness; }
            set { sequenceLineThickness = value; }
        }

        /// <summary>
        /// Index of the sequence in the source list which will be plotted on top (or start reading from).
        /// </summary>
        public static readonly DependencyProperty SourceStartIndexProperty =
            DependencyProperty.Register(
            "SourceStartIndex", typeof(int), typeof(SequenceLinesPanel),
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
        /// Total number of rows in the source list
        /// </summary>
        public int TotalRows { get; private set; }

        /// <summary>
        /// Initializes a new instance of the SequenceLinesPanel class
        /// </summary>
        public SequenceLinesPanel() : base()
        {
            // this panel is designed to work only as a vertical stack
            this.Orientation = Orientation.Vertical;
        }

        /// <summary>
        /// Set dource data for the panel
        /// This will trigger a redraw of this panel
        /// </summary>
        /// <param name="sourceList">List of seuqences to plot</param>
        /// <param name="maximumWidthInChars">Maximum span of the sequences</param>
        public void SetDataSource(IList<ISequence> sourceList, long maximumWidthInChars)
        {
            this.contig = null;
            this.alignedSequence = null;
            this.contigSequences = null;
            this.sequences = sourceList;
            this.maxLength = maximumWidthInChars;

            InvalidateMeasure();
        }

        /// <summary>
        /// Set dource data for the panel
        /// This will trigger a redraw of this panel
        /// </summary>
        /// <param name="sourceList">Contig to plot</param>
        /// <param name="maximumWidthInChars">Maximum span of the sequences</param>
        /// <param name="basePaddingLeft">Padding to be added to left of all sequences</param>
        public void SetDataSource(Contig contig, long maximumWidthInChars, long basePaddingLeft)
        {
            this.sequences = null;
            this.alignedSequence = null;
            this.contig = contig;
            this.basePaddingLeft = basePaddingLeft;
            this.maxLength = maximumWidthInChars;
            contigSequences = contig.Sequences.OrderBy(s => s.Position);

            InvalidateMeasure();
        }

        /// <summary>
        /// Set dource data for the panel
        /// This will trigger a redraw of this panel
        /// </summary>
        /// <param name="sourceList">Alignment to plot</param>
        /// <param name="maximumWidthInChars">Maximum span of the sequences</param>
        public void SetDataSource(IAlignedSequence alignedSequence, long maximumWidthInChars)
        {
            this.sequences = null;
            this.contig = null;
            this.contigSequences = null;
            this.alignedSequence = alignedSequence;
            this.maxLength = maximumWidthInChars;

            InvalidateMeasure();
        }

        /// <summary>
        /// Number of rows visible at a given time
        /// </summary>
        public int MaxVisibleItemCount { get; private set; }

        /// <summary>
        /// Line in top view which was last selected
        /// </summary>
        private Rectangle lastSelectedLine;

        /// <summary>
        /// Brush of the last selected line
        /// </summary>
        private Brush lastSelectedLineBrush;

        /// <summary>
        /// ISequence instance which should be highlighted in the view
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

                    // reset previous change
                    if (lastSelectedLine != null)
                    {
                        lastSelectedLine.Fill = lastSelectedLineBrush;
                    }

                    // find the sequence and highlight it
                    foreach (Rectangle seqLine in this.Children)
                    {
                        if (seqLine != null)
                        {
                            if (seqLine.Tag as ISequence == highlightedSequence)
                            {
                                lastSelectedLine = seqLine;
                                lastSelectedLineBrush = seqLine.Fill;
                                seqLine.Fill = Brushes.Black;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clear the view
        /// </summary>
        public void Clear()
        {
            this.sequences = null;
            this.alignedSequence = null;
            this.contig = null;

            this.Children.Clear();
        }

        /// <summary>
        /// Layout the content of the panel according to the visible area
        /// </summary>
        /// <param name="constraint">Available visible area</param>
        /// <returns>Area required to plot</returns>
        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            this.Children.Clear();

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return new Size(100, 30);
            }

            if (sequences != null)
            {
                CreateSimpleSequenceLines(constraint);
            }
            else if (contig != null)
            {
                CreateContigLines(constraint);
            }
            else if (alignedSequence != null)
            {
                CreateAlignmentLines(constraint);
            }

            return constraint;
        }

        /// <summary>
        /// Plots an alignment in the panel
        /// </summary>
        /// <param name="constraint">Available space</param>
        private void CreateAlignmentLines(Size constraint)
        {
            double desiredHeight = 0;
            int currentIndex = SourceStartIndex;

            // loop till available space is used up or reach of end of list
            while (desiredHeight < constraint.Height && currentIndex < alignedSequence.Sequences.Count)
            {
                ISequence sequence = alignedSequence.Sequences[currentIndex];

                // create the sequence line
                Rectangle sequenceLine = new Rectangle();

                // set the start point
                sequenceLine.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                sequenceLine.Width = ((constraint.Width * sequence.Count) / this.maxLength);

                sequenceLine.Fill = Brushes.LimeGreen;
                sequenceLine.Height = sequenceLineThickness;
                sequenceLine.Margin = lineMargin;
                sequenceLine.Tag = sequence;
                if (sequence == this.highlightedSequence)
                {
                    lastSelectedLine = sequenceLine;
                    lastSelectedLineBrush = sequenceLine.Fill;
                    sequenceLine.Fill = Brushes.Black;
                }
                
                this.Children.Add(sequenceLine);
                sequenceLine.Measure(constraint);

                desiredHeight += sequenceLine.DesiredSize.Height;
                currentIndex++;
            }

            this.MaxVisibleItemCount = Math.Max(0, currentIndex - SourceStartIndex);
            this.TotalRows = alignedSequence.Sequences.Count;
        }

        /// <summary>
        /// Plot a contig
        /// </summary>
        /// <param name="constraint">available space</param>
        private void CreateContigLines(Size constraint)
        {
            double desiredHeight = 0;
            int currentIndex = SourceStartIndex;

            // loop till available space is used up or reach of end of list
            while (desiredHeight < constraint.Height && currentIndex < contigSequences.Count())
            {
                Bio.Algorithms.Assembly.Contig.AssembledSequence sequence = contigSequences.ElementAt(currentIndex);
            
                // Gets the sequence position
                long startingPosition = sequence.Position - sequence.ReadPosition + this.basePaddingLeft;

                // Set the starting position for the line to be drawn
                double startingPoint = (constraint.Width * startingPosition) / this.maxLength;

                // Set the ending position for the sequence 
                double lineWidth = ((constraint.Width * sequence.Sequence.Count) / this.maxLength);

                // create the sequence line
                Rectangle sequenceLine = new Rectangle();
                sequenceLine.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                sequenceLine.Width = lineWidth;
                sequenceLine.Height = SequenceLineThickness;
                sequenceLine.Margin = new Thickness(startingPoint, this.lineMargin.Top, 0, this.lineMargin.Bottom);

                // Color code sequences if complemented , reversed or reverse complemented
                SolidColorBrush brush = Brushes.LimeGreen;
                if (sequence.IsComplemented && sequence.IsReversed)
                {
                    brush = Brushes.Crimson;
                }
                else if (sequence.IsComplemented)
                {
                    brush = Brushes.LightSeaGreen;
                }
                else if (sequence.IsReversed)
                {
                    brush = Brushes.BurlyWood;
                }

                sequenceLine.Fill = brush;
                sequenceLine.Tag = sequence.Sequence;
                if (sequence.Sequence == this.highlightedSequence)
                {
                    lastSelectedLine = sequenceLine;
                    lastSelectedLineBrush = sequenceLine.Fill;
                    sequenceLine.Fill = Brushes.Black;
                }
                this.Children.Add(sequenceLine);
                sequenceLine.Measure(constraint);

                desiredHeight += sequenceLine.DesiredSize.Height;
                currentIndex++;
            }

            this.MaxVisibleItemCount = Math.Max(0, currentIndex - SourceStartIndex);
            this.TotalRows = contig.Sequences.Count;
        }

        /// <summary>
        /// Plot a list of sequences
        /// </summary>
        /// <param name="constraint">Available space</param>
        private void CreateSimpleSequenceLines(Size constraint)
        {
            double desiredHeight = 0;
            int currentIndex = SourceStartIndex;

            // loop till available space is used up or reach of end of list
            while (desiredHeight < constraint.Height && currentIndex < sequences.Count)
            {
                ISequence currentSeq = sequences[currentIndex];

                Rectangle sequenceLine = new Rectangle();
                sequenceLine.Tag = currentSeq;
                sequenceLine.UseLayoutRounding = true;
                sequenceLine.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                sequenceLine.Height = sequenceLineThickness;
                sequenceLine.Width = ((constraint.Width * currentSeq.Count) / this.maxLength);
                
                sequenceLine.Fill = sequenceBrush;
                if (currentSeq == highlightedSequence)
                {
                    lastSelectedLine = sequenceLine;
                    lastSelectedLineBrush = sequenceLine.Fill;
                    sequenceLine.Fill = Brushes.Black;
                }

                sequenceLine.Margin = lineMargin;
                this.Children.Add(sequenceLine);
                sequenceLine.Measure(constraint);
                
                desiredHeight += sequenceLine.DesiredSize.Height;
                currentIndex++;
            }

            this.MaxVisibleItemCount = Math.Max(0, currentIndex - SourceStartIndex);
            this.TotalRows = sequences.Count;
        }
    }
}
