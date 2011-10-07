using System.Windows.Forms;

namespace Bio.TemplateWizard
{
    /// <summary>
    /// This is a welcome screen of .NET Bio Console Application wizard.
    /// </summary>
    public partial class WelcomeScreen : UserControl, IWizardScreen
    {
        /// <summary>
        /// Main header of the wizard, when this screen is shown
        /// </summary>
        public string MainHeader { get { return Properties.Resources.WelcomeScreenMainHeader; } }

        /// <summary>
        /// Sub header of the wizard, when this screen is shown
        /// </summary>
        public string SubHeader { get { return string.Empty; } }

        /// <summary>
        /// Initializes a new instance of the WelcomeScreen class.
        /// </summary>
        public WelcomeScreen()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This screen has nothing to valiate, but implementing this from IWizard interface.
        /// </summary>
        /// <returns>Always true.</returns>
        public bool ValidateScreen()
        {
            return true;
        }
    }
}
