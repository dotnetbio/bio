namespace Bio.TemplateWizard
{
    /// <summary>
    /// Interface which defines the contract for creating a new wizard screen to be used with .NET Bio Console Application wizard.
    /// </summary>
    public interface IWizardScreen
    {
        /// <summary>
        /// Main header of the wizard, when this screen is shown
        /// </summary>
        string MainHeader { get; }

        /// <summary>
        /// Sub header of the wizard, when this screen is shown
        /// </summary>
        string SubHeader { get; }

        /// <summary>
        /// This method will be called by the wizard every time the user advances from this screen.
        /// Any kind of validations and such should be done here.
        /// </summary>
        /// <returns>Return true if all validations are successful, else false.</returns>
        bool ValidateScreen();
    }
}
