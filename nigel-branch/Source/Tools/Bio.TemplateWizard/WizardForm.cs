using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System;

namespace Bio.TemplateWizard
{
    /// <summary>
    /// Main form of the wizard which hosts any wizard screens
    /// </summary>
    public partial class WizardForm : Form
    {
        /// <summary>
        /// Set of all available wizard screens.
        /// Holds one instance of each screen and same instance will be used throughout the application.
        /// </summary>
        private Dictionary<WizardScreenType, IWizardScreen> activeWizardScreens = new Dictionary<WizardScreenType, IWizardScreen>();

        /// <summary>
        /// Index / WizardScreenType enum of currently loaded screen
        /// </summary>
        private WizardScreenType currentScreen;

        /// <summary>
        /// Indicates if the user cancelled the wizard or not.
        /// </summary>
        public bool Submitted { get; set; }

        public string BioAssemblyPath
        {
            get
            {
                return (activeWizardScreens[WizardScreenType.Operations] as OperationsScreen).BioAssemblyPath;
            }
        }

        /// <summary>
        /// List of operations selected by user.
        /// </summary>
        public ReadOnlyCollection<string> SnippetTags 
        { 
            get 
            { 
                return (activeWizardScreens[WizardScreenType.Operations] as OperationsScreen).SnippetTags; 
            } 
        }

        /// <summary>
        /// Initializes a new instance of the WizardForm class.
        /// </summary>
        public WizardForm()
        {
            InitializeComponent();
            // Load all wizard screens
            activeWizardScreens.Add(WizardScreenType.Welcome, new WelcomeScreen());
            activeWizardScreens.Add(WizardScreenType.Operations, new OperationsScreen());

            // Display first screen
            LoadScreen(currentScreen);
        }

        /// <summary>
        /// Displays a given screen in the content area of the wizard
        /// </summary>
        /// <param name="screenType">WizardScreenType enum of screen to be displayed</param>
        private void LoadScreen(WizardScreenType screenType)
        {
            // Do not proceed if there was a validation failiure in current screen
            if (!activeWizardScreens[currentScreen].ValidateScreen())
            {
                return;
            }

            // Check if specified screen type is present
            if (!activeWizardScreens.ContainsKey(screenType))
            {
                throw new InvalidOperationException("Specified wizard screen was not found!");
            }

            // Clear existing content
            wizardScreenPanel.Controls.Clear();
            IWizardScreen screenToLoad = activeWizardScreens[screenType];
            wizardScreenPanel.Controls.Add(screenToLoad as UserControl);
            wizardScreenPanel.Controls[0].Dock = DockStyle.Fill;

            // Set the headings
            mainHeader.Text = screenToLoad.MainHeader;
            subHeader.Text = screenToLoad.SubHeader;

            // If showing the last screen, disable next and activate finish.
            if ((int)screenType == activeWizardScreens.Count - 1)
            {
                navigateNext.Enabled = false;
                navigateFinish.Enabled = true;
            }
            else
            {
                navigateNext.Enabled = true;
                navigateFinish.Enabled = false;
            }

            // Enable back button only if its not the first screen being displayed now
            navigatePrevious.Enabled = ((int)screenType != 0);
        }

        /// <summary>
        /// Advance to next screen
        /// </summary>
        private void OnNextClick(object sender, EventArgs e)
        {
            LoadScreen(currentScreen + 1);
            currentScreen++;
        }

        /// <summary>
        /// Go back to previous screen
        /// </summary>
        private void OnPreviousClick(object sender, EventArgs e)
        {
            LoadScreen(currentScreen - 1);
            currentScreen--;
        }

        /// <summary>
        /// Cancel the wizard
        /// </summary>
        private void OnCancelClick(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Close the wizard and return the settings chosen by user.
        /// </summary>
        private void OnFinishClick(object sender, EventArgs e)
        {
            // Validate current screen
            if (!activeWizardScreens[currentScreen].ValidateScreen())
            {
                return;
            }

            Submitted = true;
            Close();
        }
    }

    /// <summary>
    /// Holds the list of available wizard screen types.
    /// Also maintains the order of display of screens.
    /// </summary>
    public enum WizardScreenType
    {
        Welcome = 0,
        Operations
    }
}
