namespace SequenceAssembler.Dialog
{
    using System.Windows;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// Interaction logic for AboutScreen.xaml
    /// </summary>
    public partial class AboutScreen : Window
    {
        /// <summary>
        /// Stores the name of "Bio.dll"
        /// </summary>
        private static string bioDll = "Bio";

        /// <summary>
        /// Initializes a new instance of the AboutScreen class
        /// </summary>
        public AboutScreen()
        {
            InitializeComponent();
            versionNumber.Text = GetDllVersion(bioDll);
        }

        /// <summary>
        /// Gets the version of Bio.dll and displays that as the version of 
        /// the excel workbench.
        /// </summary>
        private string GetDllVersion(string assemblyName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            AssemblyName[] referencedAssemblies = asm.GetReferencedAssemblies();
            foreach (AssemblyName referencedAssembly in referencedAssemblies)
            {
                if (referencedAssembly.Name.Equals(assemblyName))
                {
                    return referencedAssembly.Version.ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Close this window and take the user to the empty workspace
        /// </summary>
        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Fired when user click Bio website link
        /// </summary>
        private void OnBioSiteRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(Properties.Resource.MBFWebsiteAddress);
        }
    }
}
