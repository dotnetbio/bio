namespace BiodexExcel.Visualizations.Common
{
    #region -- Using Directives --

    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Media.Imaging;

    #endregion -- Using Directives --

    /// <summary>
    /// AboutDialog class will display copyright information 
    /// about Excel Workbench.
    /// </summary>
    public partial class AboutDialog : Window
    {
        /// <summary>
        /// Stores the name of "Bio.dll"
        /// </summary>
        private static string bioDll = "Bio";

        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the AboutDialog class.
        /// </summary>
        public AboutDialog()
        {
            this.InitializeComponent();

            MemoryStream ms = new MemoryStream();
            System.Drawing.Bitmap icon = Properties.Resources.about;
            icon.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = ms;
            image.EndInit();

            this.imgAbout.Source = image;
            this.btnOk.Focus();
            this.GetDllVersion();
        }

        /// <summary>
        /// Gets the version of Bio.dll and displays that as the version of 
        /// the excel workbench.
        /// </summary>
        private void GetDllVersion()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            AssemblyName[] referencedAssemblies = asm.GetReferencedAssemblies();
            foreach (AssemblyName referencedAssembly in referencedAssemblies)
            {
                if (referencedAssembly.Name.Equals(bioDll))
                {
                    txtVersionNumber.Text = referencedAssembly.Version.ToString();
                    break;
                }
            }
        }

        #endregion -- Constructor --

        /// <summary>
        /// Open .NET Bio page in codeplex
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">Event argument</param>
        private void OnRequestNavigateToMBFSite(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Resources.CodeplexURL);
        }
    }
}
