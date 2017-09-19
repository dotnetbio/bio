namespace BiodexExcel.Visualizations.Common
{
    #region --Using Directive --

    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    #endregion --Using Directive --
    
    /// <summary>
    /// ColumnMap usercontrol presents the user an option to map a
    /// column header to a given column.
    /// </summary>
    public partial class ColumnMap : UserControl
    {
        #region -- Private Members --

        /// <summary>
        /// Stores the number of english alphabets.
        /// </summary>
        private const int NumberOfAlphabets = 26;

        /// <summary>
        /// Tab key stroke
        /// </summary>
        private const string TabKeyStroke = "{TAB}";

        /// <summary>
        /// Lists the name of the first 40 columns.
        /// </summary>
        private static string[] columnAZ = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        #endregion -- Private Members --

        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the ColumnMap class.
        /// </summary>
        /// <param name="columnNumbers">List of column numbers.</param>
        /// <param name="columnHeader">Header name.</param>
        public ColumnMap(List<int> columnNumbers, string columnHeader)
        {
            this.InitializeComponent();
            this.txtColumnHeader.Text = columnHeader;
            this.ColumnHeader = columnHeader;
            this.InitializeComboBox(columnNumbers);
            this.cmbColumnNumbers.SelectionChanged += new SelectionChangedEventHandler(this.OnSelectionChanged);
            this.cmbColumnNumbers.KeyDown += new System.Windows.Input.KeyEventHandler(this.OnKeyDown);
        }

        #endregion -- Constructor --

        #region -- Public Properties --

        /// <summary>
        /// Gets the column-number that the user has mapped against a given header.
        /// </summary>
        public int? ColumnNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the column header.
        /// </summary>
        public string ColumnHeader
        {
            get;
            private set;
        }

        #endregion -- Public Properties --

        #region -- Private Methods --

        /// <summary>
        /// This method generates the column name string when given the column number
        /// as input.
        /// </summary>
        /// <param name="number">Column number</param>
        /// <returns>Column name</returns>
        private static string GetColumnString(int number)
        {
            StringBuilder value = new StringBuilder();

            while (number > 0)
            {
                int mod = number % NumberOfAlphabets;
                if (mod == 0)
                {
                    value.Append("Z");
                    number = number / NumberOfAlphabets;
                    number--;
                }
                else
                {
                    value.Insert(0, columnAZ[mod - 1]);
                    number = number / NumberOfAlphabets;
                }
            }

            return value.ToString();
        }

        /// <summary>
        /// This method sets the column number for a given header
        /// when the user updates it.
        /// </summary>
        /// <param name="sender">cmbColumnNumbers instance.</param>
        /// <param name="e">Event data.</param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = this.cmbColumnNumbers.SelectedItem as ComboBoxItem;

            if (item != null)
            {
                this.ColumnNumber = item.Tag as int?;
            }
        }

        /// <summary>
        /// Custom handling of Tab Keystroke to select the current item and move to next control
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">Event argument</param>
        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (e.Key == Key.Tab && cb.IsDropDownOpen)
            {
                ComboBoxItem item = FocusManager.GetFocusedElement(Window.GetWindow(this)) as ComboBoxItem;
                cb.SelectedItem = item;
                cb.IsDropDownOpen = false;
                e.Handled = true;
                System.Windows.Forms.SendKeys.Send(TabKeyStroke);
            }
        }

        /// <summary>
        /// This method builds a combo box with a list
        /// of column numbers that the user can map to a given header.
        /// </summary>
        /// <param name="columnNumbers">List of column numbers.</param>
        private void InitializeComboBox(List<int> columnNumbers)
        {
            foreach (int columnNumber in columnNumbers)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = GetColumnString(columnNumber);
                item.Tag = columnNumber;
                this.cmbColumnNumbers.Items.Add(item);
            }
        }
        
        #endregion -- Private Methods --
    }
}
