namespace BiodexExcel.Visualizations.Common
{
    #region --Using Directive --

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    #endregion --Using Directive --

    /// <summary>
    /// ColorConfiguration class is a dialog which lets the user change the color configuration
    /// of all supported molecule types in .NET Bio. 
    /// </summary>
    public partial class ColorConfiguration : System.Windows.Window
    {
        #region -- Private Members --
        /// <summary>
        /// Current instance of the excel application. Needed to launch the color palette.
        /// </summary>
        private Microsoft.Office.Interop.Excel.Application excelApplication;

        /// <summary>
        /// List of panels which will hold the "ColorScheme" UIElement.
        /// </summary>
        private List<Panel> stackPanels;

        /// <summary>
        /// Stores mapping of molecule type and the associated color.
        /// </summary>
        private Dictionary<byte, System.Drawing.Color> colorMap;

        /// <summary>
        /// Indicates whether the dialog was cancelled by the user or not.
        /// </summary>
        private bool submitSelected;

        #endregion -- Private Members --

        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the ColorConfiguration class.
        /// </summary>
        /// <param name="excelApplication">Instance of the excel application.</param>
        /// <param name="colorMap">Existing mapping of molecule type and the associated color.</param>
        public ColorConfiguration(Microsoft.Office.Interop.Excel.Application excelApplication, Dictionary<byte, System.Drawing.Color> colorMap)
        {
            InitializeComponent();
            this.excelApplication = excelApplication;
            this.colorMap = colorMap;
            this.btnOk.Click += new RoutedEventHandler(this.OnOkClick);
            this.btnCancel.Click += new RoutedEventHandler(this.OnCancelClick);
            this.stackPanels = new List<Panel>() { this.stkColorSchemesFirst, this.stkColorSchemesSecond, this.stkColorSchemesThird, this.stkColorSchemesFourth };
            this.BuildAlphabets();

            this.btnOk.Focus();
        }

        #endregion -- Constructor --

        #region -- Public Properties --

        /// <summary>
        /// Gets the list of mapping of molecule type and the associated color.
        /// </summary>
        public Dictionary<byte, System.Drawing.Color> ColorMap
        {
            get 
            { 
                return this.colorMap; 
            }
        }

        #endregion -- Public Properties --

        #region -- Public Methods --

        /// <summary>
        /// This method displays the sheet select dialog and waits for the user to excel sheets with sequences
        /// And returns the list of excel sheets chosen.
        /// </summary>
        /// <returns>The list of excel sheets chosen.</returns>
        public new bool Show()
        {
            this.ShowDialog();
            return this.submitSelected;
        }

        #endregion -- Public Methods --

        #region -- Private static methods --

        /// <summary>
        /// This method extracts a substring from a  given strig.
        /// For eg if #A124D7 is the inpit string and 4 is the start index and 5 is the end index, "D7" is extracted.
        /// The reason this method is needed is the input to SplitBgr function contains a string which have to be parsed in reverse.
        /// So sometimes the startIndex becomes negative if the length of input string to SplitBgr is less than 6.
        /// </summary>
        /// <param name="hexadecimalValue">Input string.</param>
        /// <param name="startIndex">Start value of the index.</param>
        /// <param name="endIndex">End value of the index.</param>
        /// <returns>Substring extracted according to indices.</returns>
        private static string GetSubstring(string hexadecimalValue, int startIndex, int endIndex)
        {
            StringBuilder sb = new StringBuilder();

            while (endIndex >= 0 && endIndex >= startIndex)
            {
                sb.Insert(0, hexadecimalValue[endIndex]);
                endIndex--;
            }

            return sb.ToString();
        }

        /// <summary>
        /// This method extracts a R, G and B value in a hexadecimal string.
        /// For e.g if the string is #A124D7, the "A1" value is assigned to "b",
        /// "24" value is assigned to "g" and "D7" value us assigned to "r".
        /// </summary>
        /// <param name="hexadecimalValue">
        /// String value in a hexadecimal format whose R,G and B components have to be extracted.
        /// </param>
        /// <param name="r">Red color component.</param>
        /// <param name="g">Green color component.</param>
        /// <param name="b">Blue color component.</param>
        private static void SplitBgr(string hexadecimalValue, out byte r, out byte g, out byte b)
        {
            r = 0;
            g = 0;
            b = 0;
            int temp = 0;
            if (hexadecimalValue.Length >= 1)
            {
                string value = GetSubstring(hexadecimalValue, hexadecimalValue.Length - 2, hexadecimalValue.Length - 1);
                temp = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
                r = Convert.ToByte(temp);

                if (hexadecimalValue.Length >= 3)
                {
                    value = GetSubstring(hexadecimalValue, hexadecimalValue.Length - 4, hexadecimalValue.Length - 3);
                    temp = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
                    g = Convert.ToByte(temp);
                }

                if (hexadecimalValue.Length >= 5)
                {
                    value = GetSubstring(hexadecimalValue, hexadecimalValue.Length - 6, hexadecimalValue.Length - 5);
                    temp = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
                    b = Convert.ToByte(temp);
                }
            }
        }

        /// <summary>
        /// This method converts a double value which represents R,G and B value of a color
        /// to a System.Windows.Media color object.
        /// </summary>
        /// <param name="colorValue">Double value which represents R,G and B value of a color</param>
        /// <returns>System.Windows.Media color object corresponding to the double value.</returns>
        private static Color GetColor(double colorValue)
        {
            int colorIndex = Convert.ToInt32(colorValue);
            string hexadecimalValue = colorIndex.ToString("X");

            byte r = 0;
            byte g = 0;
            byte b = 0;

            SplitBgr(hexadecimalValue, out r, out g, out b);

            return Color.FromRgb(r, g, b);
        }

        #endregion -- Private static methods --

        #region -- Private methods --

        /// <summary>
        /// This method builds a list of alphabets, their corresponding colors(previously chosen by the users)
        /// and a button to change the existing color.
        /// </summary>
        private void BuildAlphabets()
        {
            int index = 0;
            foreach (byte symbol in this.colorMap.Keys)
            {
                ColorScheme scheme = new ColorScheme();
                scheme.MoleculeLabel = ((char)symbol).ToString();
                scheme.Symbol = symbol;
                System.Drawing.Color colorShade = this.colorMap[symbol];
                scheme.ChosenColor = Color.FromArgb(colorShade.A, colorShade.R, colorShade.G, colorShade.B);
                this.stackPanels[index].Children.Add(scheme);
                index++;
                index = index % 4;
            }
        }

        /// <summary>
        /// This event is fired when the user wants to save his changes to the color scheme.
        /// The mapping of molecule type and color is rebuilt.
        /// </summary>
        /// <param name="sender">btnOK instance.</param>
        /// <param name="e">Event data.</param>
        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            foreach (Panel panel in this.stackPanels)
            {
                foreach (UIElement element in panel.Children)
                {
                    ColorScheme scheme = element as ColorScheme;
                    if (scheme != null)
                    {
                        if (this.colorMap.ContainsKey(scheme.Symbol))
                        {
                            System.Windows.Media.Color colorShade = scheme.ChosenColor;
                            this.colorMap[scheme.Symbol] = System.Drawing.Color.FromArgb(colorShade.A, colorShade.R, colorShade.G, colorShade.B);
                        }
                    }
                }
            }

            this.submitSelected = true;
            this.Close();    
        }

        /// <summary>
        /// This event is fired when the user wants to discard his changes to the color scheme.
        /// </summary>
        /// <param name="sender">btnCancel instance.</param>
        /// <param name="e">Event data.</param>
        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.submitSelected = false;
            this.Close();    
        }

        /// <summary>
        /// Set all colors to transparent
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">Event arguments</param>
        private void OnClearAllColors(object sender, RoutedEventArgs e)
        {
            foreach (Panel panel in this.stackPanels)
            {
                foreach (UIElement element in panel.Children)
                {
                    ColorScheme scheme = element as ColorScheme;
                    if (scheme != null)
                    {
                        scheme.ChosenColor = Colors.Transparent;
                    }
                }
            }
        }

        #endregion -- Private methods --
    }
}
