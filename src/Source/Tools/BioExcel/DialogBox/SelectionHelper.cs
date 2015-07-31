namespace BiodexExcel
{
    using System;
    using System.Windows.Forms;
    using Microsoft.Office.Interop.Excel;
    using System.Collections.Generic;

    /// <summary>
    /// Method to call back once the selection is complete and the user clicks the ok button
    /// </summary>
    /// <param name="selectionAddress">Address of the selected range</param>
    /// <param name="submitSelected">Indicates if the user submitted the form or cancelled it</param>
    public delegate void SelectionCompleted(String selectionAddress, bool submitSelected);

    /// <summary>
    /// Interaction logic for SelectionHelper form
    /// </summary>
    public partial class SelectionHelper : Form
    {
        private bool submitSelected;
        private string currentSheetName, lastsheetName;
        private int lastManualComma;

        /// <summary>
        /// Event raised when the user clicks the 'Ok' button.
        /// </summary>
        public event SelectionCompleted SelectionComplete;

        /// <summary>
        /// Address of the selected range.
        /// </summary>
        public string SelectedAddress
        {
            get { return selectionText.Text; }
            set { selectionText.Text = value; }
        }

        /// <summary>
        /// Creates a new instance of the SelectionHelper class
        /// </summary>
        /// <param name="selectionCompletedCallback">Method to call back once selection is done.</param>
        public SelectionHelper(SelectionCompleted selectionCompletedCallback)
        {
            InitializeComponent();
            SelectionComplete += selectionCompletedCallback;
            currentSheetName = (Globals.ThisAddIn.Application.ActiveSheet as Worksheet).Name;

            Globals.ThisAddIn.Application.ShowSelectionFloaties = true;
            Globals.ThisAddIn.Application.SheetActivate += new AppEvents_SheetActivateEventHandler(Application_SheetActivate);
            Globals.ThisAddIn.Application.SheetSelectionChange += new Microsoft.Office.Interop.Excel.AppEvents_SheetSelectionChangeEventHandler(Application_SheetSelectionChange);
        }

        /// <summary>
        /// Manipulate the address like:
        /// If user enters a comma manually, address till that point is considered as fixed and wont be manipulated
        /// When user changes the active sheet, any sheet references which is in the non-fixed part of the address will be replaced with current sheet reference
        /// </summary>
        /// <param name="activatedSheet">current worksheet</param>
        void Application_SheetActivate(object activatedSheet)
        {
            Worksheet activeSheet = activatedSheet as Worksheet;
            lastsheetName = currentSheetName;
            currentSheetName = activeSheet.Name;

            string address = selectionText.Text;

            if (address.Length > 0 && currentSheetName != lastsheetName) // check if activate fired on same sheet
            {
                if (address[address.Length - 1] != ',' && address.Length > lastManualComma) // if last char is not a comma
                {
                    string savedAddress = address.Substring(0, lastManualComma); // address till the point where user entered a comma manually

                    // get address part after the manual comma
                    if (address[lastManualComma] == ',' && lastManualComma != 0)
                    {
                        address = address.Substring(lastManualComma + 1);
                    }
                    else
                    {
                        address = address.Substring(lastManualComma);
                    }

                    int lastComma = address.LastIndexOf(',', address.Length - 1);

                    while (lastComma != -1)
                    {
                        string lastSheetInAddress;
                        int sheetNameEndIndex = address.LastIndexOf('!');
                        if (sheetNameEndIndex != -1)
                        {
                            lastSheetInAddress = address.Substring(lastComma + 1, sheetNameEndIndex - (lastComma + 1)); // last sheet name in the address
                            if (string.IsNullOrEmpty(lastSheetInAddress))
                            {
                                break;
                            }

                            // replace sheetname with current sheetname if no comma is added before navigating to new sheet
                            address = address.Substring(0, lastComma) + "," + currentSheetName + address.Substring(lastComma + 1 + lastsheetName.Length);
                            lastComma = address.LastIndexOf(',', lastComma - 1);
                        }
                    }
                    if (lastComma == -1) // check first item in address
                    {
                        int sheetNameEndIndex;
                        if (address.Contains(","))
                        {
                            sheetNameEndIndex = address.LastIndexOf('!', address.IndexOf(','));
                        }
                        else
                        {
                            sheetNameEndIndex = address.LastIndexOf('!');
                        }

                        if (sheetNameEndIndex != -1)
                        {
                            string lastSheetInAddress = address.Substring(0, sheetNameEndIndex);
                            address = currentSheetName + address.Substring(lastSheetInAddress.Length);
                        }
                    }

                    // join the saved address and the manipulated address and write back
                    selectionText.Text = savedAddress + ((savedAddress.Length > 0) ? "," : "") + address;

                    // try to show a selection on the manipulated regions so that user comes to know what just happened.
                    try
                    {
                        Range unionOfSelection = null;

                        foreach (Range rangeToSelect in InputSelection.GetRanges(address))
                        {
                            if (unionOfSelection == null)
                            {
                                unionOfSelection = rangeToSelect;
                            }
                            else
                            {
                                unionOfSelection = Globals.ThisAddIn.Application.Union(unionOfSelection, rangeToSelect);
                            }
                        }

                        Globals.ThisAddIn.Application.EnableEvents = false;
                        unionOfSelection.Select();
                    }
                    catch
                    {
                        // Ignore if we cannot parse the text as it might be tampered manually by user. 
                    }
                    finally
                    {
                        Globals.ThisAddIn.Application.EnableEvents = true;
                    }
                }
            }
            else
            {
                // If nothing is selected, auto detect any active selection on the activated sheet.
                selectionText.Text = InputSelection.GetRangeAddress(Globals.ThisAddIn.Application.Selection);
            }
        }

        /// <summary>
        /// Raised when user makes any change in the selection of any sheet.
        /// </summary>
        /// <param name="changedSheet">Sheet which raised the event</param>
        /// <param name="changedRange">Range which got changed</param>
        void Application_SheetSelectionChange(object changedSheet, Microsoft.Office.Interop.Excel.Range changedRange)
        {
            string address = selectionText.Text;
            selectionText.Text = selectionText.Text.Substring(0, lastManualComma);
          
            Range selection = Globals.ThisAddIn.Application.Selection as Range;
            foreach (Range r in selection.Areas)
            {
                // Append the new selection
                if (!string.IsNullOrWhiteSpace(selectionText.Text) && selectionText.Text[selectionText.TextLength - 1] != ',')
                    selectionText.Text += ",";

                selectionText.Text += r.Worksheet.Name + "!" + r.Address.Replace("$", "");
            }

            this.Activate();
            this.selectionText.Focus();
            this.selectionText.SelectionStart = this.selectionText.TextLength;
        }

        /// <summary>
        /// Click event for Ok button
        /// </summary>
        private void okButton_Click(object sender, EventArgs e)
        {
            submitSelected = true;
            Close();
        }

        /// <summary>
        /// This method gets called when the form is being closed and raises the SelectionComplete event if user has not cancelled the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectionHelperClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            Globals.ThisAddIn.Application.SheetSelectionChange -= Application_SheetSelectionChange;
            Globals.ThisAddIn.Application.SheetActivate -= Application_SheetActivate;

            if (SelectionComplete != null)
                SelectionComplete(selectionText.Text.Trim(), submitSelected);
        }

        /// <summary>
        /// Set the lastManualComma when user presses ','
        /// Once comma is pressed, address till that point is considered as saved. 
        /// Any further selection/Sheet changes must not affect the address till that point.
        /// </summary>
        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ',')
                lastManualComma = selectionText.TextLength;
        }

        /// <summary>
        /// Handle updating of lastManualComma if the user changes the address manually by typing into it or by deleting part of it
        /// lastManualComma will be set to the total length of the changed text as the user has changed it manually and he knows what he is doing!
        /// </summary>
        private void OnChange(object sender, EventArgs e)
        {
            if (selectionText.TextLength < lastManualComma)
                lastManualComma = selectionText.TextLength;
        }

        /// <summary>
        /// Set the focus to the address box once the form is activated.
        /// </summary>
        private void SelectionHelper_Activated(object sender, EventArgs e)
        {
            selectionText.Focus();
        }

        private void OnWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                submitSelected = false;
                e.SuppressKeyPress = true;
                e.Handled = true;
                Close();
            }
        }
    }
}
