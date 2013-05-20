using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Security;
using System;
using System.Reflection;
using System.Globalization;
using System.IO;
using SequenceAssembler.Properties;

namespace SequenceAssembler.Dialog
{
    /// <summary>
    /// Interaction logic for FileTypeAssociationDialog.xaml
    /// </summary>
    public partial class FileTypeAssociationDialog : Window
    {
        #region Private Constants

        #region Const Format strings
        /// <summary>
        /// Application file path format. 
        /// </summary>
        private const string Application_Path_format = "\"{0}\" \"%1\"";

        /// <summary>
        /// User choice reg key format (vista onwards) 
        /// </summary>
        private const string UserChoiceKeyFromat = @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\{0}\UserChoice";

        /// <summary>
        /// File description format.
        /// </summary>
        private const string FileDescrFormat = "{0} File";
        #endregion

        #region Const Registry Keys
        /// <summary>
        /// Constatnt to hold MSRSequenceAsssembler key.
        /// </summary>
        private const string MSRSequenceAsssembler = "MSRSequenceAsssembler";

        /// <summary>
        /// Constatnt to hold ContentType key.
        /// </summary>
        private const string ContentType = "Content Type";

        /// <summary>
        /// Constatnt to hold ApplicationStringValue key.
        /// </summary>
        private const string ApplicationStringValue = "application/";

        /// <summary>
        /// Constatnt to hold DefaultIcon key.
        /// </summary>
        private const string DefaultIcon = "DefaultIcon";

        /// <summary>
        /// Constatnt to hold shell\open key.
        /// </summary>
        private const string ShellOpenKey =@"shell\open";

        /// <summary>
        /// Constatnt to hold open key.
        /// </summary>
        private const string ShellOpenValue = "open";

        /// <summary>
        /// Constatnt to hold command key.
        /// </summary>
        private const string CommandKey = "command";

        /// <summary>
        /// Constatnt to hold Progid key.
        /// </summary>
        private const string Progid = "Progid";

        /// <summary>
        /// Constatnt to hold \shell\open\command key.
        /// </summary>
        private const string ShellOpenCommandKey = @"\shell\open\command";
        #endregion

        #endregion

        /// <summary>
        /// Keeps file type check boxes.
        /// </summary>
        private List<CheckBox> chkList;

        /// <summary>
        /// holds application location.
        /// </summary>
        private string applicationPath;

        /// <summary>
        /// Constructor.
        /// </summary>
        public FileTypeAssociationDialog()
        {
            InitializeComponent();

            this.applicationPath = Assembly.GetExecutingAssembly().Location;

            this.Loaded += new RoutedEventHandler(this.OnFileTypeAssociationDialogLoaded);
            this.chkSelectALL.Checked += new RoutedEventHandler(this.OnchkSelectAllChecked);
            this.chkSelectNone.Checked += new RoutedEventHandler(OnchkSelectNoneChecked);
            this.btnOk.Click += new RoutedEventHandler(OnbtnOkClicked);
            this.btnCancel.Click += new RoutedEventHandler(OnbtnCancelClicked);
        }

        /// <summary>
        /// Associates the specified extension with Sequence Assembler.
        /// </summary>
        /// <param name="extn">File extension.</param>
        private void AssociateFileTypeWithSA(string extn)
        {
            string iconPath;
            string appPath;

            iconPath = this.applicationPath;

            appPath = string.Format(CultureInfo.CurrentCulture, Application_Path_format, this.applicationPath);

            RegistryKey rkHKLM = Registry.ClassesRoot;
            string keyValue = MSRSequenceAsssembler + extn;
            string FileTypeName = extn.Substring(1);
            RegistryKey regKey;

            regKey = rkHKLM.CreateSubKey(extn);
            regKey.SetValue(string.Empty, keyValue);

            regKey = rkHKLM.CreateSubKey(extn);
            regKey.SetValue(ContentType, ApplicationStringValue + FileTypeName);

            RegistryKey MSRregKey = rkHKLM.CreateSubKey(keyValue);
            MSRregKey.SetValue(string.Empty, string.Format(CultureInfo.InvariantCulture, FileDescrFormat, FileTypeName.ToUpper()));

            regKey = MSRregKey.CreateSubKey(DefaultIcon);
            regKey.SetValue(string.Empty, iconPath);

            regKey = MSRregKey.CreateSubKey(ShellOpenKey);
            regKey.SetValue(string.Empty, ShellOpenValue);

            regKey = regKey.CreateSubKey(CommandKey);
            regKey.SetValue(string.Empty, appPath);

            regKey.Close();
            MSRregKey.Close();
            rkHKLM.Close();

            string userChoice = string.Format(CultureInfo.InvariantCulture, UserChoiceKeyFromat, extn);
            RegistryKey rKHKUC = Registry.CurrentUser;
            if (rKHKUC.OpenSubKey(userChoice) != null)
            {
                 rKHKUC.DeleteSubKey(userChoice);
            }

            rKHKUC.Close();
        }

        /// <summary>
        /// Unassociates the specified extension with Sequence Assembler.
        /// </summary>
        /// <param name="extn">File extension.</param>
        private static void UnAssociateFileTypeWithSA(string extn)
        {
            RegistryKey rkHKLM = Registry.ClassesRoot;
            string keyValue = MSRSequenceAsssembler + extn;
            if (rkHKLM.OpenSubKey(extn, true) != null)
            {
                rkHKLM.DeleteSubKey(extn);
            }

            if (rkHKLM.OpenSubKey(keyValue) != null)
            {
                rkHKLM.DeleteSubKeyTree(keyValue);
            }

            rkHKLM.Close();

            string userChoice = string.Format(CultureInfo.InvariantCulture, UserChoiceKeyFromat, extn);
            RegistryKey rKHKUC = Registry.CurrentUser;
            RegistryKey regKey = rKHKUC.OpenSubKey(userChoice);
            if (regKey != null)
            {
                string value = regKey.GetValue(Progid) as string;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    value = value.ToUpper();
                    if (value.Equals(keyValue.ToUpper()))
                    {
                        rKHKUC.DeleteSubKey(userChoice);
                    }
                }

                regKey.Close();
            }

            rKHKUC.Close();
        }

        /// <summary>
        /// checks the registry for the specified extension is already associated with Sequence Assembler or not.
        /// </summary>
        /// <param name="extn">File extension.</param>
        /// <returns>True if the file type is associated with Sequence Assembler, else returns false.</returns>
        private bool IsExtnAssociatedWithSA(string extn)
        {
            RegistryKey rkHKLM = Registry.ClassesRoot;
            string keyValue = MSRSequenceAsssembler + extn;

            // write =true to make sure that the sufficient permission is there.
            RegistryKey MSRSAKey = rkHKLM.OpenSubKey(keyValue);

            if (MSRSAKey != null)
            {
                RegistryKey regKey = rkHKLM.OpenSubKey(keyValue + ShellOpenCommandKey);
                if (regKey != null)
                {
                    string value = regKey.GetValue(string.Empty) as string;
                    string appPath = this.applicationPath;

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        value = value.ToUpper();
                        appPath = appPath.ToUpper();
                        if (value.Contains(appPath))
                        {
                            regKey.Close();
                            MSRSAKey.Close();
                            rkHKLM.Close();
                            return true;
                        }
                    }
                }

                MSRSAKey.Close();
            }

            rkHKLM.Close();
            return false;
        }


        /// <summary>
        /// This event will be fired on selecting Cancel button.
        /// </summary>
        /// <param name="sender">btnCancel button.</param>
        /// <param name="e">Event argumetns.</param>
        private void OnbtnCancelClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// This event will be fired on selecting ok button.
        /// </summary>
        /// <param name="sender">btnOk button.</param>
        /// <param name="e">Event argumetns.</param>
        private void OnbtnOkClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (CheckBox chk in this.chkList)
                {
                    string extn = chk.Content as string;

                    if (!string.IsNullOrWhiteSpace(extn))
                    {
                        bool isAssociated = IsExtnAssociatedWithSA(extn);
                        if (chk.IsChecked == true && !isAssociated)
                        {
                            AssociateFileTypeWithSA(extn);
                        }

                        if (chk.IsChecked == false && isAssociated)
                        {
                            UnAssociateFileTypeWithSA(extn);
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(Resource.OpenApplicationInAdminMode);
            }
            catch (SecurityException)
            {
                MessageBox.Show(Resource.OpenApplicationInAdminMode);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            this.Close();
        }

        /// <summary>
        /// This event will be fired on Check of chkSelectNone check box.
        /// </summary>
        /// <param name="sender">chkSelectNone check box.</param>
        /// <param name="e">Event argumetns.</param>
        private void OnchkSelectNoneChecked(object sender, RoutedEventArgs e)
        {
            this.UnselectAll();
        }

        /// <summary>
        /// This event will be fired on Check of chkSelectAll check box.
        /// </summary>
        /// <param name="sender">chkSelectALL check box.</param>
        /// <param name="e">Event argumetns.</param>
        private void OnchkSelectAllChecked(object sender, RoutedEventArgs e)
        {
            this.SelectAll();
        }

        /// <summary>
        /// This event will be fired on Load of this window.
        /// </summary>
        /// <param name="sender">An instance of this window.</param>
        /// <param name="e">Event argumetns.</param>
        private void OnFileTypeAssociationDialogLoaded(object sender, RoutedEventArgs e)
        {
            this.chkList = new List<CheckBox>();
            chkList.Add(this.chkFasta1);
            chkList.Add(this.chkFasta2);
            chkList.Add(this.chkFasta3);
            chkList.Add(this.chkFasta4);
            chkList.Add(this.chkFasta5);
            chkList.Add(this.chkFasta6);
            chkList.Add(this.chkFasta7);

            chkList.Add(this.chkFastq1);
            chkList.Add(this.chkFastq2);

            chkList.Add(this.chkGenBank1);
            chkList.Add(this.chkGenBank2);
            chkList.Add(this.chkGenBank3);

            chkList.Add(this.chkGff1);
            this.SetchkValueOnAssociation();
            this.AddUnCheckedEventToALL();
            this.AddCheckedEventToALL();
        }

        /// <summary>
        /// Adds OnAnyUnChkChecked event to Unchecked event of all file type check boxes.
        /// </summary>
        private void AddUnCheckedEventToALL()
        {
            foreach (CheckBox chk in this.chkList)
            {
                chk.Unchecked += new RoutedEventHandler(OnAnyChkUnchecked);
            }
        }

        /// <summary>
        /// Adds OnAnyChkChecked event to checked event of all file type check boxes.
        /// </summary>
        private void AddCheckedEventToALL()
        {
            foreach (CheckBox chk in this.chkList)
            {
                chk.Checked += new RoutedEventHandler(OnAnyChkChecked);
            }
        }

        /// <summary>
        /// This event will be fired on any file type check box checked.
        /// This will sets the select none check box to false.
        /// </summary>
        /// <param name="sender">Any file type check box.</param>
        /// <param name="e">Event argumetns.</param>
        private void OnAnyChkChecked(object sender, RoutedEventArgs e)
        {
            chkSelectNone.IsChecked = false;
            CheckAllOrNone();
        }

        /// <summary>
        /// Check if all check boxes are selected or unselected and checks 'all' or 'none' appropriately
        /// </summary>
        private void CheckAllOrNone()
        {
            int checkedCount = 0; // holds the number of boxes which is checked

            foreach (CheckBox chk in this.chkList)
            {
                if (chk.IsChecked == true)
                {
                    checkedCount++;
                }
            }

            if (checkedCount == 0) // None
            {
                chkSelectNone.IsChecked = true;
            }
            if (checkedCount == this.chkList.Count)
            {
                chkSelectALL.IsChecked = true;
            }
        }

        /// <summary>
        /// This event will be fired on any file type check box unchecked.
        /// This will sets the select all check box to false.
        /// </summary>
        /// <param name="sender">Any file type check box.</param>
        /// <param name="e">Event argumetns.</param>
        private void OnAnyChkUnchecked(object sender, RoutedEventArgs e)
        {
            chkSelectALL.IsChecked = false;
            CheckAllOrNone();
        }

        /// <summary>
        /// Unselects all file types check boxes
        /// </summary>
        private void UnselectAll()
        {
            foreach (CheckBox chk in this.chkList)
            {
                chk.IsChecked = false;
            }
        }

        /// <summary>
        /// Selects all file types check boxes.
        /// </summary>
        private void SelectAll()
        {
            foreach (CheckBox chk in this.chkList)
            {
                chk.IsChecked = true;
            }
        }

        /// <summary>
        /// Sets the file type check boxes value depending on whether the 
        /// extension is associated with Sequence Assembler or not.
        /// </summary>
        private void SetchkValueOnAssociation()
        {
            try
            {
                foreach (CheckBox chk in this.chkList)
                {
                    string extn = chk.Content as string;
                    if (!string.IsNullOrWhiteSpace(extn))
                    {
                        if (IsExtnAssociatedWithSA(extn))
                        {
                            chk.IsChecked = true;
                        }
                        else
                        {
                            chk.IsChecked = false;
                        }
                    }
                }
            }
            catch (SecurityException)
            {
                MessageBox.Show(Resource.OpenApplicationInAdminMode);
                this.Close();
            }
        }
    }
}
