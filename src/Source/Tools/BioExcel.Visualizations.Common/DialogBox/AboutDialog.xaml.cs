namespace BiodexExcel.Visualizations.Common
{
    #region -- Using Directives --

    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using System;

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
        private static string bioDll = "Bio.Core";

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

            // Assign the version
            txtVersionNumber.Text = string.Format(" {0}, using Bio.Core.dll {1}", GetDllVersion("BioExcel"), GetDllVersion(bioDll));
        }

        /// <summary>
        /// Gets the version of Bio.dll and displays that as the version of 
        /// the excel workbench.
        /// </summary>
        private string GetDllVersion(string dllName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly referencedAssembly in assemblies)
            {
                AssemblyName assemblyName = referencedAssembly.GetName();

                if (assemblyName.Name.Equals(dllName))
                {
                    return assemblyName.Version.ToString();
                }
            }
            return "(n/a)";
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
