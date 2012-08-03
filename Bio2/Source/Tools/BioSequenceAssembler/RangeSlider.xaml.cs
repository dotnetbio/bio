namespace SequenceAssembler
{
    #region -- Using Directives --

    using System;
    using System.Globalization;

    #endregion -- Using Directives --

    /// <summary>
    /// RangeSlider class is a wrapper around RangeSliderBase class.
    /// RangeSlider styles RangeSliderBase and provides a wrapper around 
    /// RangeSelectionChanged event. 
    /// </summary>
    public partial class RangeSlider
    {      
        #region -- Private variables --

        /// <summary>
        /// Stores the current range start point.
        /// </summary>
        private long rangeStart;

        /// <summary>
        /// Stores the current range width
        /// </summary>
        private long rangeWidth = 1;

        #endregion -- Private Methods --

        /// <summary>
        /// Gets or Sets the range slider selceted range starting
        /// </summary>
        public long RangeStartSelected
        {
            get { return rangeStart; }
            set
            {
                rangeStart = value;
                this.rangeSliderBase.RangeStartSelected = value;
                this.rangeSliderBase.RangeStopSelected = RangeStartSelected + RangeWidth;
            }
        }

        /// <summary>
        /// Gets or Sets the Width of the RangeSlider
        /// </summary>
        public long RangeWidth
        {
            get { return rangeWidth; }
            set 
            {
                // Dont exceed the maximum width
                if (value > rangeSliderBase.RangeStop)
                {
                    rangeWidth = rangeSliderBase.RangeStop;
                }
                else
                {
                    rangeWidth = value;
                }

                if (this.rangeSliderBase.RangeStartSelected + rangeWidth > this.rangeSliderBase.RangeStop)
                {
                    this.rangeSliderBase.RangeStartSelected = this.rangeSliderBase.RangeStop - rangeWidth;
                }

                this.rangeSliderBase.RangeStopSelected = this.rangeSliderBase.RangeStartSelected + rangeWidth;
            }
        }

        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the RangeSlider class.
        /// </summary>
        public RangeSlider()
        {
            InitializeComponent();

            // Register to range changes
            this.rangeSliderBase.RangeSelectionChanged += this.RangeSliderRangeSelectionChanged;

            // Set other properties of RangeSliderBase.
            this.rangeSliderBase.RangeStart = 0;
            this.rangeSliderBase.RangeStop = 100;
            this.rangeSliderBase.RangeStartSelected = 0;
            this.rangeSliderBase.RangeStopSelected = this.rangeSliderBase.RangeStartSelected + rangeWidth;
            this.rangeSliderBase.MinRange = 1;
        }

        #endregion -- Constructor --

        #region -- Public Events --

        /// <summary>
        /// Event to subsribe to when the range in the slider changes.
        /// </summary>
        public event EventHandler<RangeSelectionChangedEventArgs> RangeSelectionChanged;

        #endregion -- Public Events --

        #region -- Public Properties --

        /// <summary>
        /// This method sets the length of the contig that we will display.
        /// This method also calculates the mid value and displays it there.
        /// </summary>
        /// <param name="maximumValue">Last index for the range slider</param>
        public void SetRange(long maximumValue)
        {
            this.txtEndRange.Text = maximumValue.ToString(CultureInfo.CurrentUICulture);
            this.txtMidRange.Text = Math.Ceiling((decimal)maximumValue / 2).ToString(CultureInfo.CurrentUICulture);

            this.rangeSliderBase.RangeStop = maximumValue;

            // Check if the selected range is below minimum
            if (this.rangeSliderBase.RangeStartSelected < 0)
            {
                this.rangeSliderBase.RangeStartSelected = Math.Max(0, maximumValue - rangeWidth);
            }
            // Check if selected range exceeds maximum
            else if (this.rangeSliderBase.RangeStartSelected > maximumValue || this.rangeSliderBase.RangeStartSelected + rangeWidth > maximumValue)
            {
                this.rangeSliderBase.RangeStartSelected = Math.Max(0, maximumValue - rangeWidth);
            }

            // Set the selected range width
            this.rangeSliderBase.RangeStopSelected = this.rangeSliderBase.RangeStartSelected + rangeWidth;
        }

        #endregion -- Public Properties --

        #region -- Private Methods --

        /// <summary>
        /// This method gets fired when the range in the Range slider has changed.
        /// </summary>
        /// <param name="sender">RangeSliderBase instance.</param>
        /// <param name="e">New range values.</param>
        private void RangeSliderRangeSelectionChanged(object sender, RangeSelectionChangedEventArgs e)
        {
            if (e.NewRangeStart >= e.NewRangeStop)
            {
                e.NewRangeStop = e.NewRangeStart + 1;
            }

            if (this.RangeSelectionChanged != null && (this.rangeStart != e.NewRangeStart || (this.rangeStart + rangeWidth) != e.NewRangeStop))
            {
                this.RangeSelectionChanged.Invoke(sender, e);
                this.rangeStart = e.NewRangeStart;
            }
        }

        #endregion -- Private Methods --
    }
}
