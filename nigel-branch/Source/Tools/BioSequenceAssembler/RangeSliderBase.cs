namespace SequenceAssembler
{
    #region Using Directives

    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    #endregion

    /// <summary>
    /// Delegate for the RangeSelectionChanged event
    /// </summary>
    /// <param name="sender">The object raising the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void RangeSelectionChangedEventHandler(object sender, RangeSelectionChangedEventArgs e);

    /// <summary>
    /// RangeSliderBase will provide us with a slider which is capable of
    /// choosing a range of values.
    /// </summary>
    [DefaultEvent("RangeSelectionChanged"),
    TemplatePart(Name = "PART_RangeSliderContainer", Type = typeof(StackPanel)),
    TemplatePart(Name = "PART_LeftEdge", Type = typeof(RepeatButton)),
    TemplatePart(Name = "PART_RightEdge", Type = typeof(RepeatButton)),
    TemplatePart(Name = "PART_LeftThumb", Type = typeof(Thumb)),
    TemplatePart(Name = "PART_MiddleThumb", Type = typeof(Thumb)),
    TemplatePart(Name = "PART_RightThumb", Type = typeof(Thumb))]
    public sealed class RangeSliderBase : Control
    {
        #region Public Fields 

        /// <summary>
        /// Gets or sets the min value for the range of the range slider
        /// </summary>
        public static readonly DependencyProperty RangeStartProperty =
            DependencyProperty.Register(
            "RangeStart",
            typeof(long),
            typeof(RangeSliderBase),
            new UIPropertyMetadata(
                (long)0,
                delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
                {
                    RangeSliderBase slider = (RangeSliderBase)sender;

                    // Check if the property is set internally
                    if (!slider.internalUpdate)
                    {
                        slider.ReCalculateRanges();
                        slider.ReCalculateWidths();
                    }
                }));

        /// <summary>
        /// The max value for the range of the range slider
        /// </summary>
        public static readonly DependencyProperty RangeStopProperty =
            DependencyProperty.Register(
            "RangeStop",
            typeof(long),
            typeof(RangeSliderBase),
            new UIPropertyMetadata(
                (long)1,
                delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
                {
                    RangeSliderBase slider = (RangeSliderBase)sender;

                    // Check if the property is set internally
                    if (!slider.internalUpdate)
                    {
                        slider.ReCalculateRanges();
                        slider.ReCalculateWidths();
                    }
                }));

        /// <summary>
        /// The min value of the selected range of the range slider.
        /// </summary>
        public static readonly DependencyProperty RangeStartSelectedProperty =
            DependencyProperty.Register(
            "RangeStartSelected",
            typeof(long),
            typeof(RangeSliderBase),
            new UIPropertyMetadata(
                (long)0,
                delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
                {
                    RangeSliderBase slider = (RangeSliderBase)sender;

                    // Check if the property is set internally
                    if (!slider.internalUpdate)
                    {
                        slider.ReCalculateWidths();
                        slider.OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(slider));
                    }
                }));

        /// <summary>
        /// The max value of the selected range of the range slider
        /// </summary>
        public static readonly DependencyProperty RangeStopSelectedProperty =
            DependencyProperty.Register(
            "RangeStopSelected",
            typeof(long),
            typeof(RangeSliderBase),
            new UIPropertyMetadata(
                (long)1,
                delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
                {
                    RangeSliderBase slider = (RangeSliderBase)sender;

                    // Check if the property is set internally
                    if (!slider.internalUpdate)
                    {
                        slider.ReCalculateWidths();
                        slider.OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(slider));
                    }
                }));

        /// <summary>
        /// The min range value that you can have for the range slider
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when MinRange is set less than 0</exception>
        public static readonly DependencyProperty MinRangeProperty =
            DependencyProperty.Register(
            "MinRange",
            typeof(long),
            typeof(RangeSliderBase),
            new UIPropertyMetadata(
                (long)0,
                delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
                {
                    if ((long)e.NewValue < 0)
                    {
                        throw new ArgumentOutOfRangeException("value", "value for MinRange cannot be less than 0");
                    }

                    RangeSliderBase slider = (RangeSliderBase)sender;

                    // Check if the property is set internally
                    if (!slider.internalUpdate)
                    {
                        // Set flag to signal that the properties are being set by the object itself
                        slider.internalUpdate = true;
                        slider.RangeStopSelected = Math.Max(slider.RangeStopSelected, slider.RangeStartSelected + (long)e.NewValue);
                        slider.RangeStop = Math.Max(slider.RangeStop, slider.RangeStopSelected);

                        // Set flag to signal that the properties are being set by the object itself
                        slider.internalUpdate = false;

                        slider.ReCalculateRanges();
                        slider.ReCalculateWidths();
                    }
                }));

        /// <summary>
        /// Event raised whenever the selected range is changed
        /// </summary>
        public static readonly RoutedEvent RangeSelectionChangedEvent =
            EventManager.RegisterRoutedEvent(
            "RangeSelectionChanged",
            RoutingStrategy.Bubble,
            typeof(RangeSelectionChangedEventHandler),
            typeof(RangeSliderBase));

        #endregion

        /// <summary>
        /// Used to move the selection by x ratio when click the repeat buttons
        /// </summary>
        private const double RepeatButtonMoveRatio = 0.1;

        /// <summary>
        /// Default width of splitters thumb.
        /// </summary>
        private const double DefaultSplittersThumbWidth = 10;

        #region Commands

        /// <summary>
        /// Command to move back the selection
        /// </summary>
        private static RoutedUICommand moveBack =
            new RoutedUICommand(
                "MoveBack",
                "MoveBack",
                typeof(RangeSliderBase),
                new InputGestureCollection(new InputGesture[] 
                {
                    new KeyGesture(Key.B, ModifierKeys.Control)
                }));

        /// <summary>
        /// Command to move forward the selection
        /// </summary>
        private static RoutedUICommand moveForward =
            new RoutedUICommand(
                "MoveForward",
                "MoveForward",
                typeof(RangeSliderBase),
                new InputGestureCollection(new InputGesture[] 
                {
                    new KeyGesture(Key.F, ModifierKeys.Control)
                }));

        /// <summary>
        /// Command to move all forward the selection
        /// </summary>
        private static RoutedUICommand moveAllForward =
            new RoutedUICommand(
                "MoveAllForward",
                "MoveAllForward",
                typeof(RangeSliderBase),
                new InputGestureCollection(new InputGesture[] 
                {
                    new KeyGesture(Key.F, ModifierKeys.Alt)
                }));

        /// <summary>
        /// Command to move all back the selection
        /// </summary>
        private static RoutedUICommand moveAllBack =
            new RoutedUICommand(
                "MoveAllBack",
                "MoveAllBack",
                typeof(RangeSliderBase),
                new InputGestureCollection(new InputGesture[] 
                {
                    new KeyGesture(Key.B, ModifierKeys.Alt)
                }));

        #endregion

        #region Data members

        /// <summary>
        /// Indicates whether the update is internal or external.
        /// </summary>
        private bool internalUpdate;

        /// <summary>
        /// The center thumb to move the range around
        /// </summary>
        private Thumb centerThumb;

        /// <summary>
        /// The left thumb that is used to expand the range selected
        /// </summary>
        private Thumb leftThumb;

        /// <summary>
        /// The right thumb that is used to expand the range selected
        /// </summary>
        private Thumb rightThumb;

        /// <summary>
        /// The left side of the control (movable left part)
        /// </summary>
        private RepeatButton leftButton;

        /// <summary>
        /// The right side of the control (movable right part)
        /// </summary>
        private RepeatButton rightButton;

        /// <summary>
        /// Stackpanel to store the visual elements for this control
        /// </summary>
        private StackPanel visualElementsContainer;
        
        /// <summary>
        /// Holds the movable width.
        /// </summary>
        private double movableWidth;

        /// <summary>
        /// Holds the movalble range of the slider. 
        /// </summary>
        private long movableRange;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes static members of the RangeSliderBase class.
        /// </summary>
        static RangeSliderBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(RangeSliderBase), new FrameworkPropertyMetadata(typeof(RangeSliderBase)));
        }

        /// <summary>
        /// Initializes a new instance of the RangeSliderBase class.
        /// </summary>
        public RangeSliderBase()
        {
            CommandBindings.Add(new CommandBinding(RangeSliderBase.moveBack, this.MoveBackHandler));
            CommandBindings.Add(new CommandBinding(RangeSliderBase.moveForward, this.MoveForwardHandler));
            CommandBindings.Add(new CommandBinding(RangeSliderBase.moveAllForward, this.MoveAllForwardHandler));
            CommandBindings.Add(new CommandBinding(RangeSliderBase.moveAllBack, this.MoveAllBackHandler));

            // Hook to the size change event of the range slider
            DependencyPropertyDescriptor.FromProperty(ActualWidthProperty, typeof(RangeSliderBase)).
                AddValueChanged(this, delegate { this.ReCalculateWidths(); });
        }

        #endregion

        #region Properties and events

        /// <summary>
        /// Event raised whenever the selected range is changed
        /// </summary>
        public event RangeSelectionChangedEventHandler RangeSelectionChanged
        {
            add { AddHandler(RangeSelectionChangedEvent, value); }
            remove { RemoveHandler(RangeSelectionChangedEvent, value); }
        }

        /// <summary>
        /// Gets or sets the min value for the range of the range slider
        /// </summary>
        public long RangeStart
        {
            get { return (long)GetValue(RangeStartProperty); }
            set { SetValue(RangeStartProperty, value); }
        }

        /// <summary>
        /// Gets or sets the max value for the range of the range slider
        /// </summary>
        public long RangeStop
        {
            get { return (long)GetValue(RangeStopProperty); }
            set { SetValue(RangeStopProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the min value of the selected range of the range slider
        /// </summary>
        public long RangeStartSelected
        {
            get { return (long)GetValue(RangeStartSelectedProperty); }
            set { SetValue(RangeStartSelectedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the max value of the selected range of the range slider
        /// </summary>
        public long RangeStopSelected
        {
            get { return (long)GetValue(RangeStopSelectedProperty); }
            set { SetValue(RangeStopSelectedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the min range value that you can have for the range slider
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when MinRange is set less than 0</exception>
        public long MinRange
        {
            get { return (long)GetValue(MinRangeProperty); }
            set { SetValue(MinRangeProperty, value); }
        }   

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the position of the slider relative to the given visual parent
        /// </summary>
        /// <param name="visual">Parent of which relative position has to be calculated</param>
        /// <returns>Relative postion</returns>
        public Point[] GetSliderPositionRelative(UIElement visual)
        {
            Point topLeft = default(Point), bottomRight = default(Point);

            if (this.centerThumb != null)
            {
                topLeft = this.leftThumb.TransformToAncestor(visual)
                                .Transform(new Point(0, 0));
                topLeft.X += this.leftThumb.ActualWidth / 2;
                bottomRight = this.rightThumb.TransformToAncestor(visual)
                                .Transform(new Point(0, 0));
                bottomRight.X += this.rightThumb.ActualWidth / 2;
                bottomRight.Y += this.rightThumb.ActualHeight - 22;
            }

            return new Point[] { topLeft, bottomRight };
        }

        /// <summary>
        /// Moves the current selection with x value
        /// </summary>
        /// <param name="isLeft">True if you want to move to the left</param>
        public void MoveSelection(bool isLeft)
        {
            double widthChange = RangeSliderBase.RepeatButtonMoveRatio * (this.RangeStopSelected - this.RangeStartSelected)
                * this.movableWidth / this.movableRange;

            widthChange = isLeft ? -widthChange : widthChange;
            RangeSliderBase.MoveThumb(this.leftButton, this.rightButton, widthChange);
            this.ReCalculateRangeSelected(true, true);
        }

        /// <summary>
        /// Reset the Slider to the Start/End
        /// </summary>
        /// <param name="isStart">Pass true to reset to start point</param>
        public void ResetSelection(bool isStart)
        {
            double widthChange = this.RangeStop - this.RangeStart;
            widthChange = isStart ? -widthChange : widthChange;

            RangeSliderBase.MoveThumb(this.leftButton, this.rightButton, widthChange);
            this.ReCalculateRangeSelected(true, true);
        }

        /// <summary>
        /// This method gives the ability to move the entire
        /// range selected by the users.
        /// </summary>
        /// <param name="span">
        /// The span to move the selection by.
        /// </param>
        public void MoveSelection(long span)
        {
            if (span > 0)
            {
                if (this.RangeStopSelected + span > this.RangeStop)
                {
                    span = this.RangeStop - this.RangeStopSelected;
                }
            }
            else
            {
                if (this.RangeStartSelected + span < this.RangeStart)
                {
                    span = this.RangeStart - this.RangeStartSelected;
                }
            }

            if (span != 0)
            {
                // Set flag to signal that the properties are being set by the object itself
                this.internalUpdate = true;
                this.RangeStartSelected += span;
                this.RangeStopSelected += span;
                this.ReCalculateWidths();

                // Set flag to signal that the properties are being set by the object itself
                this.internalUpdate = false;

                this.OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
            }
        }

        /// <summary>
        /// Sets the selected range in one go. If the selection is invalid, nothing happens.
        /// </summary>
        /// <param name="selectionStart">New selection start value</param>
        /// <param name="selectionStop">New selection stop value</param>
        public void SetSelectedRange(long selectionStart, long selectionStop)
        {
            long start = Math.Max(this.RangeStart, selectionStart);
            long stop = Math.Min(selectionStop, this.RangeStop);
            start = Math.Min(start, this.RangeStop - this.MinRange);
            stop = Math.Max(this.RangeStart + this.MinRange, stop);

            if (stop >= start + this.MinRange)
            {
                // Set flag to signal that the properties are being set by the object itself
                this.internalUpdate = true;
                this.RangeStartSelected = start;
                this.RangeStopSelected = stop;
                this.ReCalculateWidths();

                // Set flag to signal that the properties are being set by the object itself
                this.internalUpdate = false;

                this.OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
            }
        }

        /// <summary>
        /// Changes the selected range to the supplied range
        /// </summary>
        /// <param name="span">The span to zoom</param>
        public void ZoomToSpan(long span)
        {
            // Set flag to signal that the properties are being set by the object itself
            this.internalUpdate = true;

            // Ensure new span is within the valid range
            span = Math.Min(span, this.RangeStop - this.RangeStart);
            span = Math.Max(span, this.MinRange);

            if (span == this.RangeStopSelected - this.RangeStartSelected)
            {
                // No change
                return;
            }

            // First zoom half of it to the right
            long rightChange = (span - (this.RangeStopSelected - this.RangeStartSelected)) / 2;
            long leftChange = rightChange;

            // If we will hit the right edge, spill over the leftover change to the other side
            if (rightChange > 0 && this.RangeStopSelected + rightChange > this.RangeStop)
            {
                leftChange += rightChange - (this.RangeStop - this.RangeStopSelected);
            }

            this.RangeStopSelected = Math.Min(this.RangeStopSelected + rightChange, this.RangeStop);
            rightChange = 0;

            // If we will hit the left edge and there is space on the right, add the leftover change to the other side
            if (leftChange > 0 && this.RangeStartSelected - leftChange < this.RangeStart)
            {
                rightChange = this.RangeStart - (this.RangeStartSelected - leftChange);
            }

            this.RangeStartSelected = Math.Max(this.RangeStartSelected - leftChange, this.RangeStart);

            // Leftovers to the right
            if (rightChange > 0)
            {
                this.RangeStopSelected = Math.Min(this.RangeStopSelected + rightChange, this.RangeStop);
            }

            this.ReCalculateWidths();

            // Set flag to signal that the properties are being set by the object itself
            this.internalUpdate = false;
            this.OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
        }

        /// <summary>
        /// Overide to get the visuals from the control template
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.visualElementsContainer = this.EnforceInstance<StackPanel>("PART_RangeSliderContainer");
            this.centerThumb = this.EnforceInstance<Thumb>("PART_MiddleThumb");
            this.leftButton = this.EnforceInstance<RepeatButton>("PART_LeftEdge");
            this.rightButton = this.EnforceInstance<RepeatButton>("PART_RightEdge");
            this.leftThumb = this.EnforceInstance<Thumb>("PART_LeftThumb");
            this.rightThumb = this.EnforceInstance<Thumb>("PART_RightThumb");
            this.InitializeVisualElementsContainer();
            this.ReCalculateWidths();
        }

        #endregion

        #region logic to resize range

        /// <summary>
        /// MoveThumb resizes the left column and the right column
        /// </summary>
        /// <param name="x">Left column that has to be resized.</param>
        /// <param name="y">Right column that has to be resized.</param>
        /// <param name="horizonalChange">Chnage required along the horizontal direction.</param>
        private static void MoveThumb(FrameworkElement x, FrameworkElement y, double horizonalChange)
        {
            double change = 0;

            // Slider went left
            if (horizonalChange < 0)
            {
                change = GetChangeKeepPositive(x.Width, horizonalChange);
            }
            else if (horizonalChange > 0)
            {
                // Slider went right if(horizontal change == 0 do nothing)
                change = -GetChangeKeepPositive(y.Width, -horizonalChange);
            }

            x.Width += change;
            y.Width -= change;
        }

        /// <summary>
        /// Ensures that the new value (newValue param) is a valid value. returns false if not
        /// </summary>
        /// <param name="width">Current width value.</param>
        /// <param name="increment">Incremental value.</param>
        /// <returns>New return value.</returns>
        private static double GetChangeKeepPositive(double width, double increment)
        {
            return Math.Max(width + increment, 0) - width;
        }

        #endregion

        #region Command handlers

        /// <summary>
        /// Resets the slider to the start.
        /// </summary>
        /// <param name="sender">MoveAllBack command.</param>
        /// <param name="e">Event data.</param>
        private void MoveAllBackHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.ResetSelection(true);
        }

        /// <summary>
        /// Resets the slider to the end.
        /// </summary>
        /// <param name="sender">MoveAllForward command.</param>
        /// <param name="e">Event data.</param>
        private void MoveAllForwardHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.ResetSelection(false);
        }

        /// <summary>
        /// Moves the current selection with x value to the left.
        /// </summary>
        /// <param name="sender">MoveBack command.</param>
        /// <param name="e">Event data.</param>
        private void MoveBackHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.MoveSelection(true);
        }
                
        /// <summary>
        /// Moves the current selection with x value to the right.
        /// </summary>
        /// <param name="sender">MoveForward command.</param>
        /// <param name="e">Event data.</param>
        private void MoveForwardHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.MoveSelection(false);
        }

        #endregion

        #region event handlers for visual elements to drag the range

        /// <summary>
        /// This method moves the selection when left repeat button 
        /// has been clicked.
        /// </summary>
        /// <param name="sender">LeftButton instance.</param>
        /// <param name="e">Event data.</param>
        private void LeftButtonClick(object sender, RoutedEventArgs e)
        {
            this.MoveSelection(true);
        }
        
        /// <summary>
        /// This method moves the selection when right repeat button 
        /// has been clicked.
        /// </summary>
        /// <param name="sender">RightButton instance.</param>
        /// <param name="e">Event data.</param>
        private void RightButtonClick(object sender, RoutedEventArgs e)
        {
            this.MoveSelection(false);
        }

        /// <summary>
        /// This method drags the thumb from the middle
        /// </summary>
        /// <param name="sender">Center thumb.</param>
        /// <param name="e">Event data.</param>
        private void CenterThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            RangeSliderBase.MoveThumb(this.leftButton, this.rightButton, e.HorizontalChange);
        }

        /// <summary>
        /// This event fires when the drag operation is complete.
        /// </summary>
        /// <param name="sender">Left thumb.</param>
        /// <param name="e">Event data.</param>
        private void RightThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.ReCalculateRangeSelected(false, true);
        }

        /// <summary>
        /// This event fires when the drag operation is complete.
        /// </summary>
        /// <param name="sender">Left thumb.</param>
        /// <param name="e">Event data.</param>
        private void LeftThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.ReCalculateRangeSelected(true, false);
        }

        /// <summary>
        /// This event fires when the drag operation is completed
        /// </summary>
        /// <param name="sender">Center Thumb</param>
        /// <param name="e">Event Data</param>
        private void OnCenterThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.ReCalculateRangeSelected(true, true);
        }

        #endregion      

        #region logic to calculate the range

        /// <summary>
        /// Recalculates the movableRange. called from the RangeStop setter, RangeStart setter and MinRange setter
        /// </summary>
        private void ReCalculateRanges()
        {
            this.movableRange = this.RangeStop - this.RangeStart - this.MinRange;
        }

        /// <summary>
        /// Recalculates the movableWidth. called whenever the width of the control changes
        /// </summary>
        private void ReCalculateWidths()
        {
            if (this.leftButton != null && this.rightButton != null && this.centerThumb != null)
            {
                this.movableWidth = Math.Max(this.ActualWidth - this.rightThumb.ActualWidth - this.leftThumb.ActualWidth - this.centerThumb.MinWidth, 1);
                this.leftButton.Width = Math.Min(this.movableWidth , Math.Max(this.movableWidth * (this.RangeStartSelected - this.RangeStart) / this.movableRange, 0));
                this.rightButton.Width = Math.Min(this.movableWidth , Math.Max(this.movableWidth * (this.RangeStop - this.RangeStopSelected) / this.movableRange, 0));
                this.centerThumb.Width = Math.Max(ActualWidth - this.leftButton.Width - this.rightButton.Width - this.rightThumb.ActualWidth - this.leftThumb.ActualWidth, 0);
            }
        }

        /// <summary>
        /// Recalculates the rangeStartSelected called when the left thumb is moved and when the middle thumb is moved
        /// and also recalculates the rangeStopSelected called when the right thumb is moved and when the middle thumb is moved
        /// </summary>
        /// <param name="recalculateStart">Indicate whether to re-calculate start.</param>
        /// <param name="recalculateStop">Indicate whether to re-calculate stop.</param>
        private void ReCalculateRangeSelected(bool recalculateStart, bool recalculateStop)
        {
            // Set flag to signal that the properties are being set by the object itself
            this.internalUpdate = true;

            if (recalculateStart)
            {
                // Make sure to get exactly rangestart if thumb is at the start
                if (this.leftButton.Width == 0.0)
                {
                    this.RangeStartSelected = this.RangeStart;
                }
                else
                {
                    this.RangeStartSelected =
                        Math.Max(this.RangeStart, (long)(this.RangeStart + (this.movableRange * this.leftButton.Width / this.movableWidth)));
                }
            }

            if (recalculateStop)
            {
                // Make sure to get exactly rangestop if thumb is at the end
                if (this.rightButton.Width == 0.0)
                {
                    this.RangeStopSelected = this.RangeStop;
                }
                else
                {
                    this.RangeStopSelected =
                        Math.Min(this.RangeStop, (long)(this.RangeStop - (this.movableRange * this.rightButton.Width / this.movableWidth)));
                }
            }

            // Set flag to signal that the properties are being set by the object itself
            this.internalUpdate = false;

            if (recalculateStart || recalculateStop)
            {
                // Raise the RangeSelectionChanged event
                this.OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(this));
            }
        }
        
        #endregion             
     
        #region Helper

        /// <summary>
        /// OnRangeSelectionChanged raises the RangeSelectionChanged event
        /// </summary>
        /// <param name="e">RangeSelectionChangedEventArgs data.</param>
        private void OnRangeSelectionChanged(RangeSelectionChangedEventArgs e)
        {
            e.RoutedEvent = RangeSelectionChangedEvent;
            RaiseEvent(e);
        }

        /// <summary>
        /// Creates a new instance of the requested Framework element. 
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <param name="partName">Child name that is being requested for.</param>
        /// <returns>Instance of the framework element.</returns>
        private T EnforceInstance<T>(string partName)
            where T : FrameworkElement, new()
        {
            T element = GetTemplateChild(partName) as T;
            if (element == null)
            {
                element = new T();
            }

            return element;
        }

        /// <summary>
        /// InitializeVisualElementsContainer adds all visual element to the conatiner
        /// </summary>
        private void InitializeVisualElementsContainer()
        {
            this.visualElementsContainer.Orientation = Orientation.Horizontal;
            this.leftThumb.Width = DefaultSplittersThumbWidth;
            this.leftThumb.Tag = "left";
            this.rightThumb.Width = DefaultSplittersThumbWidth;
            this.rightThumb.Tag = "right";

            // Handle the drag delta
            this.centerThumb.DragDelta += this.CenterThumbDragDelta;
            this.leftThumb.DragCompleted += this.LeftThumbDragCompleted;
            this.rightThumb.DragCompleted += this.RightThumbDragCompleted;
            this.centerThumb.DragCompleted += this.OnCenterThumbDragCompleted;
            this.leftButton.Click += this.LeftButtonClick;
            this.rightButton.Click += this.RightButtonClick;
        }

           
 
        #endregion
    }
}
