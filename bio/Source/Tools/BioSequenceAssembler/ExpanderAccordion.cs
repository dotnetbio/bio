using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SequenceAssembler
{
    /// <summary>
    /// Accordion control for expanders
    /// Assumes that all children of this panel are Expanders, and the expand direction is vertical
    /// </summary>
    public class ExpanderAccordion : Panel
    {
        /// <summary>
        /// Expander added recently to the children collection. Used to link with the thumb control
        /// </summary>
        private Expander lastExpanderAdded;

        /// <summary>
        /// Attached property for controling layout
        /// </summary>
        public static readonly DependencyProperty UserDefinedSizeProperty =
            DependencyProperty.RegisterAttached("UserDefinedSize", 
            typeof(double?), typeof(ExpanderAccordion), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange));

        /// <summary>
        /// Attached property for controling layout
        /// </summary>
        public static readonly DependencyProperty AllocatedSizeProperty =
            DependencyProperty.RegisterAttached("AllocatedSize",
            typeof(double), typeof(ExpanderAccordion),
            new FrameworkPropertyMetadata(default(double)));

        /// <summary>
        /// Initializes a new instance of the ExpanderAccordion class.
        /// </summary>
        public ExpanderAccordion() : base()
        {
            this.SizeChanged += new SizeChangedEventHandler(OnSizeChanged);            
        }

        /// <summary>
        /// Fired anytime the size of this control changes
        /// </summary>
        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.HeightChanged) // do something only if height has changed.
            {
                foreach (UIElement child in this.Children)
                {
                    child.SetValue(UserDefinedSizeProperty, null);
                }
            }
        }

        /// <summary>
        /// Overridden method which is called while a new child is being added to the collection.
        /// Used to create thumb controls for each child, which is used to drag and resise the child.
        /// </summary>
        /// <param name="visualAdded">Item which got added</param>
        /// <param name="visualRemoved">Item which got removed</param>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            VerticalDragThumb thumb = null;

            Expander expander = visualRemoved as Expander;
            if (expander != null)
            {
                expander.Collapsed -= new RoutedEventHandler(OnChildExpanderCollapsed);
            }
            
            expander = visualAdded as Expander;
            if (expander != null)
            {
                expander.Collapsed += new RoutedEventHandler(OnChildExpanderCollapsed);

                if (lastExpanderAdded != null)
                {
                    // Create a thumb control and associate with this child
                    thumb = new VerticalDragThumb();
                    thumb.DragDelta += new DragDeltaEventHandler(OnThumbDragged);
                    this.Children.Insert(this.Children.Count - 1, thumb);
                    thumb.ExpanderAbove = lastExpanderAdded;
                    thumb.ExpanderBelow = expander;

                    base.OnVisualChildrenChanged(thumb, null);
                }

                lastExpanderAdded = expander;
            }

            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        /// <summary>
        /// Find the nearest expander based on the parameters supplied.
        /// </summary>
        /// <param name="currentElement">UIElement from where to start searching</param>
        /// <param name="searchUpwards">True to search upwards in the children collection</param>
        /// <param name="onlyGetOpenExpanders">True to consider open expanders only</param>
        /// <returns>Expander instance if one is found matching the parameters</returns>
        private Expander GetNextExpander(UIElement currentElement, bool searchUpwards = false, bool onlyGetOpenExpanders = true)
        {
            int currentIndex = this.Children.IndexOf(currentElement);
            currentIndex += searchUpwards ? -1 : 1; // go up or down based on the searchUpwards parameter

            int searchStopIndex = searchUpwards ? -1 : this.Children.Count; // stop searching for an expander when at this index, either start or end of the list.

            while (currentIndex != searchStopIndex)
            {
                if (this.Children[currentIndex] is Expander && (this.Children[currentIndex] as Expander).IsEnabled)
                {
                    if (onlyGetOpenExpanders == false) // if user is searching for closed expanders too return this expander
                    {
                        return this.Children[currentIndex] as Expander;
                    }
                    else if((this.Children[currentIndex] as Expander).IsExpanded) // if user wants only open expanders, return here
                    {
                        return this.Children[currentIndex] as Expander;
                    }
                }

                currentIndex += searchUpwards ? -1 : 1; // go up or down based on the searchUpwards parameter
            }

            return null;
        }

        /// <summary>
        /// Fired when user drags a thumb control to resise an expander
        /// </summary>
        private void OnThumbDragged(object sender, DragDeltaEventArgs e)
        {
            VerticalDragThumb thumb = sender as VerticalDragThumb;
            Expander nextOpenExpanderAbove = GetNextExpander(thumb, true);
            Expander nextOpenExpanderBelow = GetNextExpander(thumb, false);

            bool movingUp = e.VerticalChange < 0;

            // Check for moves which are not possible... or open or close expanders as needed to make the move...
            if (e.VerticalChange == 0)
            {
                return;
            }

            if (movingUp && ((thumb.ExpanderAbove.IsExpanded == false && nextOpenExpanderAbove == null) || GetNextExpander(thumb, false,false) == null))
            {
                return;
            }

            if (!movingUp && ((thumb.ExpanderBelow.IsExpanded == false && nextOpenExpanderBelow == null) || GetNextExpander(thumb, false,false) == null))
            {
                return;
            }

            if (movingUp && thumb.ExpanderBelow != null && thumb.ExpanderBelow.IsExpanded == false)
            {
                thumb.ExpanderBelow.IsExpanded = true;
                nextOpenExpanderBelow = thumb.ExpanderBelow;
            }

            if (!movingUp && nextOpenExpanderAbove == null)
            {
                thumb.ExpanderAbove.IsExpanded = true;
                nextOpenExpanderAbove = thumb.ExpanderAbove;
            }

            // Handle the expander above
            if (nextOpenExpanderAbove != null)
            {
                double currentHeight = (double)nextOpenExpanderAbove.GetValue(AllocatedSizeProperty);
                double newHeight = currentHeight + e.VerticalChange;

                if (newHeight > nextOpenExpanderAbove.MinHeight)
                {
                    if (nextOpenExpanderAbove.IsExpanded == false)
                    {
                        nextOpenExpanderAbove.IsExpanded = true;
                    }
                    nextOpenExpanderAbove.SetValue(UserDefinedSizeProperty, newHeight);
                }
                else if (newHeight <= nextOpenExpanderAbove.MinHeight && nextOpenExpanderAbove.IsExpanded == true)
                {
                    nextOpenExpanderAbove.IsExpanded = false;
                    nextOpenExpanderAbove.SetValue(UserDefinedSizeProperty, null);
                }
            }

            // Handle the expander below
            if (nextOpenExpanderBelow != null)
            {
                double currentHeight = (double)nextOpenExpanderBelow.GetValue(AllocatedSizeProperty);
                double newHeight = currentHeight - e.VerticalChange;

                if (newHeight > nextOpenExpanderBelow.MinHeight)
                {
                    if (nextOpenExpanderBelow.IsExpanded == false)
                    {
                        nextOpenExpanderBelow.IsExpanded = true;
                    }
                    nextOpenExpanderBelow.SetValue(UserDefinedSizeProperty, newHeight);
                }
                else if (newHeight <= nextOpenExpanderBelow.MinHeight && nextOpenExpanderBelow.IsExpanded == true)
                {
                    nextOpenExpanderBelow.IsExpanded = false;
                    nextOpenExpanderBelow.SetValue(UserDefinedSizeProperty, null);
                }

                if (thumb.ExpanderBelow != null && thumb.ExpanderBelow == lastExpanderAdded)
                {
                    nextOpenExpanderBelow.SetValue(UserDefinedSizeProperty, null);
                }
            }
        }

        /// <summary>
        /// Fired whenever an expander is collapsed
        /// </summary>
        private void OnChildExpanderCollapsed(object sender, RoutedEventArgs e)
        {
            Expander collapsedExpander = sender as Expander;
            Expander expanderChild = null;
            Expander onlyOpenExpander = null; // holds an expander instance if it is the only open expander left
            bool foundOpenExpander = false;

            foreach (UIElement child in this.Children)
            {
                 expanderChild = child as Expander;

                if (expanderChild != null && expanderChild.IsExpanded == true)
                {
                    onlyOpenExpander = expanderChild;
                    if(foundOpenExpander)
                    {
                        onlyOpenExpander = null;
                    }
                    foundOpenExpander = true;
                }
            }

            // If no open expanders after closing this one, find the expander just below or just above and open it.
            if (!foundOpenExpander)
            {
                Expander expanderToAutoOpen;
                expanderToAutoOpen = GetNextExpander(collapsedExpander, false, false);

                if (expanderToAutoOpen == null)
                {
                    expanderToAutoOpen = GetNextExpander(collapsedExpander, true, false);
                }

                if (expanderToAutoOpen != null)
                {
                    expanderToAutoOpen.IsExpanded = true;
                }
                else
                {
                    collapsedExpander.IsExpanded = true;
                }
            }

            // if only one expander is left, reset its height to use default available full space.
            if (onlyOpenExpander != null)
            {
                onlyOpenExpander.SetValue(UserDefinedSizeProperty, null);
            }

            // Reset the dragged height
            collapsedExpander.SetValue(UserDefinedSizeProperty, null);
        }

        /// <summary>
        /// Overriding the measure pass
        /// </summary>
        /// <param name="constraint">Available space for this control</param>
        /// <returns>Space needed to render the contents of this control</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            double desiredHeight = 0;
            double desiredWidth = 0;

            // Call measure on all children and find the desired size of all...
            foreach (UIElement child in this.Children)
            {
                if (child as Expander != null || child as VerticalDragThumb != null)
                {
                    if (child as Expander != null)
                        (child as Expander).Height = double.NaN;

                    child.Measure(constraint);
                    desiredHeight += child.DesiredSize.Height;
                    desiredWidth = child.DesiredSize.Width > desiredWidth ? child.DesiredSize.Width : desiredWidth;
                }
            }

            return new Size(
                double.IsPositiveInfinity(constraint.Width) ? desiredWidth : constraint.Width, 
                double.IsPositiveInfinity(constraint.Height) ? desiredHeight : constraint.Height
                );
        }

        /// <summary>
        /// Arrange the children in the panel using the calculations from measure pass.
        /// </summary>
        /// <param name="finalSize">Total area available to arrange the controls</param>
        /// <returns>Total used area</returns>
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            double closedExpandersTotalHeight = 0; // Total minimum height required by closed expanders and expanders with user defined height.
            double openExpandersTotalHeight = 0; // Total size asked by all open expanders

            foreach (UIElement child in this.Children)
            {
                Expander childExpander = child as Expander;

                if (childExpander != null)
                {
                    // Get a height if user has resized
                    double? userHeight = (double?)childExpander.GetValue(UserDefinedSizeProperty);

                    if (userHeight != null)
                    {
                        closedExpandersTotalHeight += (double)userHeight;
                    }
                    else if (childExpander.IsExpanded)
                    {
                        openExpandersTotalHeight += childExpander.DesiredSize.Height;
                    }
                    else
                    {
                        closedExpandersTotalHeight += childExpander.DesiredSize.Height;
                    }
                }
                else if (child as VerticalDragThumb != null)
                {
                    closedExpandersTotalHeight += child.DesiredSize.Height;
                }
            }

            // Start arranging

            double usedHeight = 0;
            double usedWidth = 0;
            double openSpace = finalSize.Height - closedExpandersTotalHeight;
            double openHeightDistributionFactor = openSpace / openExpandersTotalHeight; // Multiplication factor for distributing available free space
            double currentChildHeight = 0;

            foreach (UIElement child in this.Children)
            {
                Expander childExpander = child as Expander;

                if (childExpander != null)
                {
                    double? userHeight = (double?)childExpander.GetValue(UserDefinedSizeProperty);

                    // Find height to allocate
                    if (userHeight != null)
                    {
                        currentChildHeight = (double)userHeight;
                    }
                    else if (childExpander.IsExpanded)
                    {
                        currentChildHeight = child.DesiredSize.Height * openHeightDistributionFactor;
                    }
                    else
                    {
                        currentChildHeight = childExpander.DesiredSize.Height;
                    }

                    // Set height property so that its contents wont draw beyond this height
                    childExpander.Height = currentChildHeight - (childExpander.Margin.Top + childExpander.Margin.Bottom + childExpander.Padding.Top + childExpander.Padding.Bottom);
                    childExpander.Arrange(
                        new Rect(0,
                                usedHeight,
                                finalSize.Width,
                                currentChildHeight
                                ));

                    childExpander.SetValue(AllocatedSizeProperty, currentChildHeight);
                }
                else if (child as VerticalDragThumb != null)
                {
                    currentChildHeight = child.DesiredSize.Height;
                    child.Arrange(new Rect(0, usedHeight, finalSize.Width, child.DesiredSize.Height));
                }

                usedHeight += currentChildHeight;
                usedWidth = child.DesiredSize.Width > usedWidth ? child.DesiredSize.Width : usedWidth;
            }

            return new Size(finalSize.Width, usedHeight);
        }
    }

    /// <summary>
    /// A Thumb class modified to link with two expanders adjacent to it.
    /// </summary>
    public class VerticalDragThumb : Thumb
    {
        public Expander ExpanderAbove { get; set; }
        public Expander ExpanderBelow { get; set; }

        /// <summary>
        /// Initializes a new instance of the VerticalDragThumb class.
        /// </summary>
        public VerticalDragThumb() : base()
        {
            this.Cursor = System.Windows.Input.Cursors.SizeNS;
        }
    }
}
