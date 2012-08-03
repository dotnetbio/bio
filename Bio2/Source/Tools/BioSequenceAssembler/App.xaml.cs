namespace SequenceAssembler
{
    #region -- Using directives --
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Threading;
    using SequenceAssembler.Properties;
    #endregion -- Using directives --

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// This event will gets file names from the command line and sets in to application properties.
        /// </summary>
        /// <param name="e">Startup event args.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(this.UnhandledException);
            if (e.Args != null && e.Args.Count() > 0)
            {
                // Set the property this will be accessed in OnLoad of SequenceAssembly class.
                this.Properties["FilesToLoad"] = e.Args;
            }

            base.OnStartup(e);
        }

        /// <summary>
        /// Method to handle unhandled exception.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event argument.</param>
        private void UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            DisplayException(e.Exception);
            e.Handled = true;
        }

        /// <summary>
        /// Display Exception Messages, if inner exception found then displays the inner exception.
        /// </summary>
        /// <param name="ex">The Exception.</param>
        private static void DisplayException(Exception ex)
        {
            if (ex.InnerException == null || string.IsNullOrEmpty(ex.InnerException.Message))
            {
                MessageDialogBox.Show(ex.Message, Resource.CAPTION,
                                                MessageDialogButton.OK);
            }
            else
            {
                DisplayException(ex.InnerException);
            }
        }
    }
}
