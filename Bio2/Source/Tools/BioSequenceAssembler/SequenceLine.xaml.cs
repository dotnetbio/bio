using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Bio;

namespace SequenceAssembler
{
    /// <summary>
    /// Interaction logic for SequenceLine.xaml
    /// </summary>
    public partial class SequenceLine : UserControl
    {
        /// <summary>
        /// The sequence instance this control is representing
        /// </summary>
        private ISequence sequence;

        /// <summary>
        /// Indicates if the sequence being held is a reference sequence
        /// </summary>
        private bool isReferenceSequence;

        /// <summary>
        /// Gets or Sets if current sequence is a reference sequence
        /// </summary>
        public bool IsReferenceSequence
        {
            get
            {
                return isReferenceSequence;
            }
            set
            {
                isReferenceSequence = value;
                // Dont show metadata for reference sequence
                metadataRow.Height = new GridLength(0);
                sequenceItemsBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 78, 255)); // Same as used in top view
                sequenceItemsBorder.Background = Brushes.Gainsboro;
            }
        }

        #region Fall in properties from parent panel

        private SequencesPanel parentSequencePanel;

        private long DisplayStartIndex 
        { 
            get 
            { 
                return parentSequencePanel.DisplayStartIndex; 
            } 
        }

        private long DisplayEndIndex
        {
            get
            {
                return parentSequencePanel.DisplayEndIndex;
            }
        }

        private double CharWidth
        {
            get
            {
                return parentSequencePanel.CharWidth;
            }
        }

        private double CharHeight
        {
            get
            {
                return parentSequencePanel.CharHeight;
            }
        }

        private long BasePaddingLeft
        {
            get
            {
                return parentSequencePanel.BasePaddingLeft;
            }
        }

        private Dictionary<char, Brush> ColorMap
        {
            get
            {
                return parentSequencePanel.ColorMap;
            }
        }

        #endregion

        /// <summary>
        /// Holds a reference to the sequence being hold,
        /// In case this is displaying a reversed sequence or so, will need a reference to the original sequence
        /// </summary>
        public ISequence ActualSequenceInside { get; set; }

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
        /// Gets or Sets starting index of the read where it starts aligning with the reference sequence
        /// </summary>
        public long ReadAlignStartIndex { get; set; }

        /// <summary>
        /// Gets or Sets starting index of the reference sequence where this sequence starts aligning
        /// </summary>
        public long AlignStartIndex { get; set; }

        /// <summary>
        /// Gets or Sets last index of the reference sequence where this sequence aligns
        /// </summary>
        public long AlignmentLength { get; set; }

        /// <summary>
        /// Initializes a new instance of the SequenceLine class.
        /// </summary>
        /// <param name="sequence">Sequence instance this control will hold</param>
        /// <param name="parent">Parent panel for this control, Must be a SequencesPanel object</param>
        public SequenceLine(ISequence sequence, SequencesPanel parent)
        {
            InitializeComponent();

            this.sequence = sequence;
            this.parentSequencePanel = parent;

            containerGrid.RowDefinitions[0].Height = parent.IsMetadataVisible ? new GridLength(CharHeight) : new GridLength(0);
            this.containerGrid.RowDefinitions[1].Height = new GridLength(parent.CharHeight + sequenceItemsBorder.BorderThickness.Top + sequenceItemsBorder.BorderThickness.Bottom);
            AlignmentLength = sequence.Count;

            this.closeSequenceButton.Visibility = parentSequencePanel.AllowRemove == true ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Calculates all measures needed for plotting a sequence within the given space constraints
        /// Adds sequence items (TextBlocks) to this seuqnnce line depending on the available width
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            long sequencePlotStartColumn = AlignStartIndex - ReadAlignStartIndex + BasePaddingLeft;
            long sequencePlotEndColumn = sequencePlotStartColumn + sequence.Count;

            if (!(sequencePlotStartColumn < DisplayEndIndex && sequencePlotEndColumn + 1 > DisplayStartIndex))
            {
                return new Size(0, containerGrid.RowDefinitions[0].Height.Value + containerGrid.RowDefinitions[1].Height.Value);
            }

            long startCharIndex = 0, endCharIndex;
            double leftMargin = 0;

            sequenceItemsPanel.Children.Clear();
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                sequence = new Sequence(Alphabets.DNA, "ACGT");
            }

            if (DisplayStartIndex > sequencePlotStartColumn)
            {
                startCharIndex = DisplayStartIndex - sequencePlotStartColumn;
                endCharIndex = DisplayEndIndex - DisplayStartIndex + startCharIndex;

                // Keep metadata text left aligned with the sequence start point.
                metadataBlock.Margin = new Thickness((startCharIndex * CharWidth * -1), 0, 0, 0);
            }
            else
            {
                endCharIndex = DisplayEndIndex - DisplayStartIndex - (sequencePlotStartColumn - DisplayStartIndex);
                leftMargin = (sequencePlotStartColumn - DisplayStartIndex) * CharWidth;
            }

            if (endCharIndex > sequence.Count) endCharIndex = sequence.Count;

            this.Margin = new Thickness(leftMargin, 0, 0, 0);
            sequenceItemsBorder.Width = (endCharIndex - startCharIndex) * CharWidth;

            // Color code sequences if complemented , reversed or reverse complemented
            if (IsComplemented && IsReversed)
            {
                sequenceItemsBorder.BorderBrush = Brushes.Crimson;
            }
            else if (IsComplemented)
            {
                sequenceItemsBorder.BorderBrush = Brushes.LightSeaGreen;
            }
            else if (IsReversed)
            {
                sequenceItemsBorder.BorderBrush = Brushes.BurlyWood;
            }

            // If font size is less than 4, skip this so that the sequences will appear as a line.
            // Any font size below 4 is not readable so no point in displaying it.
            if (FontSize > 4)
            {
                TextBlock block = null;
                for (long pos = startCharIndex; pos < endCharIndex; pos++)
                {
                    block = new TextBlock();

                    if (pos < ReadAlignStartIndex || pos >= ReadAlignStartIndex + AlignmentLength)
                    {
                        string symbol = new string(new char[] { (char)sequence[pos] });
                        block.Text = symbol.ToLower();
                        block.Opacity = .6;
                    }
                    else
                    {
                        string symbol = new string(new char[] { (char)sequence[pos] });
                        block.Text = symbol.ToString();
                    }

                    block.Width = CharWidth;
                    block.Height = CharHeight;
                    block.TextAlignment = TextAlignment.Center;
                    block.UseLayoutRounding = true;
                    
                    Brush colorBrush;

                    if (ColorMap != null && ColorMap.TryGetValue(char.ToUpper((char)sequence[pos]), out colorBrush))
                    {
                        block.Background = colorBrush;
                    }
                    else if(ColorMap != null && ColorMap.TryGetValue('*', out colorBrush))
                    {
                        block.Background = colorBrush;
                    }

                    sequenceItemsPanel.Children.Add(block);
                }

                metadataBlock.Text = sequence.ID;
            }
            else
            {
                metadataRow.Height = new GridLength(0);
                sequenceRow.Height = new GridLength(sequenceItemsBorder.BorderThickness.Top + sequenceItemsBorder.BorderThickness.Bottom + (isReferenceSequence ? 1 : 0));
            }
            
            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Click event of the close button of this sequenceLine
        /// </summary>
        private void OnCloseSequenceButtonClick(object sender, RoutedEventArgs e)
        {
            parentSequencePanel.RemoveSequence(sequence);
        }
    }
}
