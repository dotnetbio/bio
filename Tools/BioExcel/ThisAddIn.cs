using System;
using System.Windows.Forms;
namespace BiodexExcel
{
    /// <summary>
    /// ThisAddIn represents the entire add-in.
    /// </summary>
    public partial class ThisAddIn
    {
        /// <summary>
        /// Fired when addin starts up.
        /// </summary>
        /// <param name="sender">ThisAddIn instance.</param>
        /// <param name="e">Event data</param>
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {            
        }

        /// <summary>
        /// Fired when addin shut down.
        /// </summary>
        /// <param name="sender">ThisAddIn instance.</param>
        /// <param name="e">Event data.</param>
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        /// <summary>
        /// This is called to get the ribbon(s) for the add-in.  We override it
        /// to avoid the reflection scan Excel will normally perform.
        /// </summary>
        /// <returns></returns>
        protected override Microsoft.Office.Tools.Ribbon.IRibbonExtension[] CreateRibbonObjects()
        {
            try
            {
                return new[] { new BioRibbon() };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create ribbon for .NET Bio: " + ex.Message);
            }

            return new Microsoft.Office.Tools.Ribbon.IRibbonExtension[0];
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
