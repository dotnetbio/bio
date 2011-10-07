namespace SequenceAssembler
{
    #region -- Using Directive --

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;

    #endregion  -- Using Directive --

    /// <summary>
    /// RangeSelectionChangedEventArgs describes the custom event Args
    /// for the for the Range slider RangeSelectionChanged event.
    /// </summary>
    public class RangeSelectionChangedEventArgs : RoutedEventArgs
    {
        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the RangeSelectionChangedEventArgs class.
        /// </summary>
        /// <param name="newRangeStart">The new range start set</param>
        /// <param name="newRangeStop">The new range stop set</param>
        internal RangeSelectionChangedEventArgs(long newRangeStart, long newRangeStop)
        {
            this.NewRangeStart = newRangeStart;
            this.NewRangeStop = newRangeStop;
        }

        /// <summary>
        /// Initializes a new instance of the RangeSelectionChangedEventArgs class.
        /// </summary>
        /// <param name="slider">The slider to get the info from</param>
        internal RangeSelectionChangedEventArgs(RangeSliderBase slider)
            : this(slider.RangeStartSelected, slider.RangeStopSelected)
        {
        }

        #endregion -- Constructor --

        #region -- Properties --

        /// <summary>
        /// Gets or sets the new range start selected in the range slider
        /// </summary>
        public long NewRangeStart
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the new range stop selected in the range slider
        /// </summary>
        public long NewRangeStop
        {
            get;
            set;
        }

        #endregion -- Properties --
    }
}
