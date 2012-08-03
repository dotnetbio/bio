using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Bio.TemplateWizard
{
    /// <summary>
    /// This wizard screen will show a list of available operations to choose from.
    /// </summary>
    public partial class OperationsScreen : UserControl, IWizardScreen
    {
        /// <summary>
        /// Registry key location of MBI versions
        /// </summary>
        private const string MBIRegistryPath = @"SOFTWARE\.NET Bio\";

        /// <summary>
        /// Registry key location of Bio Installation path
        /// </summary>
        private const string BioRegistryPath = @"\Framework\";

        /// <summary>
        /// Registry key name of Bio Installation path
        /// </summary>
        private const string BioInstalltionPathKey = "InstallationPath";

        /// <summary>
        /// Main header of the wizard, when this screen is shown
        /// </summary>
        public string MainHeader { get { return Properties.Resources.OperationsMainHeader; } }

        /// <summary>
        /// Sub header of the wizard, when this screen is shown
        /// </summary>
        public string SubHeader { get { return Properties.Resources.OperationsSubHeader; } }

        /// <summary>
        /// List of selected operations
        /// </summary>
        private List<string> selectedTags = new List<string>();

        /// <summary>
        /// List of selected operations exposed as a readonly collection.
        /// </summary>
        public ReadOnlyCollection<string> SnippetTags { get; private set; }

        public string BioAssemblyPath { get; set; }

        /// <summary>
        /// Initializes a new instance of the OperationsScreen class.
        /// </summary>
        public OperationsScreen()
        {
            InitializeComponent();
            SnippetTags = selectedTags.AsReadOnly();

            // Check for Bio framework
            RegistryKey BioPathKey = Registry.LocalMachine.OpenSubKey(MBIRegistryPath);
            
            if (BioPathKey == null || BioPathKey.SubKeyCount == 0)
            {
                // Inform user, and proceed.
                MessageBox.Show(Properties.Resources.BioMissing,
                Properties.Resources.Caption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

                BioAssemblyPath = string.Empty;

                versionSelector.Enabled = false;
                versionSelectorLabel.Enabled = false;
            }
            else
            {
                // Load available version numbers to the version selector combo
                foreach (string version in BioPathKey.GetSubKeyNames())
                {
                    versionSelector.Items.Add(version);
                }
                versionSelector.SelectedIndex = 0;

                // initialize the path so that it points to the first version in the list
                BioPathKey = Registry.LocalMachine.OpenSubKey(MBIRegistryPath + versionSelector.Text + BioRegistryPath);
                if (BioPathKey != null)
                {
                    BioAssemblyPath = BioPathKey.GetValue(BioInstalltionPathKey).ToString();
                } 
            }
        }

        /// <summary>
        /// Checks for selected choices and updates the SelectedTags list.
        /// </summary>
        /// <returns>True if validation completes successfully</returns>
        public bool ValidateScreen()
        {
            selectedTags.Clear();
            foreach(Control currentControl in operationsPanel.Controls)
            {
                if ((currentControl as CheckBox).Checked)
                {
                    selectedTags.Add(currentControl.Tag as string);
                }
            }

            SnippetTags = selectedTags.AsReadOnly();

            // Set the Bio assembly path
            RegistryKey BioPathKey = Registry.LocalMachine.OpenSubKey(MBIRegistryPath + versionSelector.Text + BioRegistryPath);
            if (BioPathKey != null)
            {
                BioAssemblyPath = BioPathKey.GetValue(BioInstalltionPathKey).ToString();
            }
                        
            return true;
        }
    }
}
