namespace BiodexExcel.Visualizations.Common
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// OfficeColorPicker component which resembles Office 2007 color picker drop down.
    /// </summary>
    public partial class OfficeColorPicker : ComboBox
    {
        /// <summary>
        /// DP for current color
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Brush), typeof(OfficeColorPicker),
                new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// DP for Text property
        /// </summary>
        public new static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(String), typeof(OfficeColorPicker),
                new FrameworkPropertyMetadata("?", FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Initializes a new instance of the OfficeColorPicker class.
        /// </summary>
        public OfficeColorPicker()
        {
            this.InitializeComponent();

            // As child element holds the color picker template, create one child and set it as current.
            this.AddChild(" "); 
            this.SelectedIndex = 0; 
        }

        /// <summary>
        /// Gets or sets currently selected color
        /// </summary>
        public Color SelectedColor
        {
            get { return ((SolidColorBrush)GetValue(SelectedColorProperty)).Color; }
            set { SetValue(SelectedColorProperty, new SolidColorBrush(value)); }
        }

        /// <summary>
        /// Gets or sets label to be displayed on the component
        /// </summary>
        public new string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Set the selected color based on the color on which user clicked
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">Event arguments</param>
        private void OnColorSelect(object sender, RoutedEventArgs e)
        {
            this.SelectedColor = ((sender as Button).Background as SolidColorBrush).Color;
            this.IsDropDownOpen = false;
        }

        /// <summary>
        /// Set color to transparent on the selected item
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">Event arguments</param>
        private void OnClearColor(object sender, RoutedEventArgs e)
        {
            this.SelectedColor = Colors.Transparent;
            this.IsDropDownOpen = false;
        }

        /// <summary>
        /// Show the color dialog for choosing from more colors
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">Event arguments</param>
        private void OnShowColorDialog(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog moreColorsDialog = new System.Windows.Forms.ColorDialog();
            if (moreColorsDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.SelectedColor = Color.FromArgb(moreColorsDialog.Color.A, moreColorsDialog.Color.R, moreColorsDialog.Color.G, moreColorsDialog.Color.B);
            }
        }
    }
}
