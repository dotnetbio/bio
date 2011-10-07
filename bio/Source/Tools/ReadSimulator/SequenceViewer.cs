using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Bio.Controls
{
    public class SequenceViewer : FrameworkElement
    {
        public static readonly DependencyProperty SequenceProperty =
            DependencyProperty.Register("Sequence", typeof(ISequence), typeof(SequenceViewer),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSequenceChanged)));

        public ISequence Sequence
        {
            get { return (ISequence)GetValue(SequenceProperty); }
            set { SetValue(SequenceProperty, value); }
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            if (Sequence != null)
            {
                FormattedText seqText = new FormattedText(Sequence.ToString(), CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight, new Typeface("Courier New"), 12.0,
                    new SolidColorBrush(Colors.Black));

                // If we can't fit the sequence, we need to come up with a compressed representation
                if (seqText.Width > ActualWidth)
                {
                    double marginX = 10.0;
                    double marginY = 3.0;
                    double barMarginY = marginY + seqText.Height + 2.0;
                    double barWidth = ActualWidth - (marginX * 2.0);
                    double vertHeight = 5.0;
                    Pen seqLinePen = new Pen(new SolidColorBrush(Colors.Black), 1.0);

                    // Draw the horizontal bar
                    drawingContext.DrawLine(seqLinePen, new Point(marginX, barMarginY), new Point(ActualWidth - marginX, barMarginY));

                    // Determine the number of verticals
                    int spaceLength = 100;
                    if (Sequence.Count > 1000)
                    {
                        spaceLength = 250;
                    }
                    if (Sequence.Count > 5000)
                    {
                        spaceLength = 1000;
                    }
                    if (Sequence.Count > 10000)
                    {
                        spaceLength = 2500;
                    }
                    int vertCount = (int)(Sequence.Count / spaceLength) + 1;
                    double vertSpacing = barWidth / (double)vertCount;

                    // Draw the vertical bars and annotation
                    for (int i = 0; i <= vertCount; i++)
                    {
                        double x = (vertSpacing * (double)i) + marginX;
                        drawingContext.DrawLine(seqLinePen, new Point(x, barMarginY), new Point(x, barMarginY + vertHeight));

                        FormattedText annotation = new FormattedText((spaceLength * i).ToString(), CultureInfo.CurrentUICulture,
                            FlowDirection.LeftToRight, new Typeface("Courier New"), 10.0,
                            new SolidColorBrush(Colors.DarkGray));
                        double textX = x - (annotation.Width / (i == vertCount ? 1.0 : 2.0));
                        drawingContext.DrawText(annotation, new Point(textX, marginY));
                    }

                } // Otherwise just draw the text sequence in there
                else
                {
                    drawingContext.DrawText(seqText, new Point(10.0, 10.0));
                }
            }
            base.OnRender(drawingContext);
        }

        private static void OnSequenceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            // Force a render when the Sequence changes
            SequenceViewer viewer = (SequenceViewer)obj;
            viewer.InvalidateArrange();
            viewer.InvalidateMeasure();
            viewer.InvalidateVisual();
        }
    }
}
