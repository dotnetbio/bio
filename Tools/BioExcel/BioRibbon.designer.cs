
using Microsoft.Office.Tools.Ribbon;

namespace BiodexExcel
{
    partial class BioRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabBio = this.Factory.CreateRibbonTab();
            this.groupImportExport = this.Factory.CreateRibbonGroup();
            this.splitImport = this.Factory.CreateRibbonSplitButton();
            this.splitExport = this.Factory.CreateRibbonSplitButton();
            this.groupAlignment = this.Factory.CreateRibbonGroup();
            this.splitAligners = this.Factory.CreateRibbonSplitButton();
            this.btnCancelAlign = this.Factory.CreateRibbonButton();
            this.groupAssembly = this.Factory.CreateRibbonGroup();
            this.btnAssemble = this.Factory.CreateRibbonButton();
            this.btnCancelAssemble = this.Factory.CreateRibbonButton();
            this.groupWebService = this.Factory.CreateRibbonGroup();
            this.splitWebService = this.Factory.CreateRibbonSplitButton();
            this.btnCancelSearch = this.Factory.CreateRibbonButton();
            this.groupCharts = this.Factory.CreateRibbonGroup();
            this.splitChart = this.Factory.CreateRibbonSplitButton();
            this.btnRunChartMacro = this.Factory.CreateRibbonButton();
            this.grpGenomicInterval = this.Factory.CreateRibbonGroup();
            this.splitOperate = this.Factory.CreateRibbonSplitButton();
            this.btnMerge = this.Factory.CreateRibbonButton();
            this.btnIntersect = this.Factory.CreateRibbonButton();
            this.btnSubtract = this.Factory.CreateRibbonButton();
            this.groupNodeXL = this.Factory.CreateRibbonGroup();
            this.btnVennDiagram = this.Factory.CreateRibbonButton();
            this.groupConfig = this.Factory.CreateRibbonGroup();
            this.splitConfiguration = this.Factory.CreateRibbonSplitButton();
            this.btnMaxColumn = this.Factory.CreateRibbonButton();
            this.btnConfigureColor = this.Factory.CreateRibbonButton();
            this.btnConfigureImport = this.Factory.CreateRibbonButton();
            this.groupAbout = this.Factory.CreateRibbonGroup();
            this.btnHomePage = this.Factory.CreateRibbonButton();
            this.btnUserManual = this.Factory.CreateRibbonButton();
            this.btnAbout = this.Factory.CreateRibbonButton();
            this.tabBio.SuspendLayout();
            this.groupImportExport.SuspendLayout();
            this.groupAlignment.SuspendLayout();
            this.groupAssembly.SuspendLayout();
            this.groupWebService.SuspendLayout();
            this.groupCharts.SuspendLayout();
            this.grpGenomicInterval.SuspendLayout();
            this.groupNodeXL.SuspendLayout();
            this.groupConfig.SuspendLayout();
            this.groupAbout.SuspendLayout();
            // 
            // tabBio
            // 
            this.tabBio.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tabBio.Groups.Add(this.groupImportExport);
            this.tabBio.Groups.Add(this.groupAlignment);
            this.tabBio.Groups.Add(this.groupAssembly);
            this.tabBio.Groups.Add(this.groupWebService);
            this.tabBio.Groups.Add(this.groupCharts);
            this.tabBio.Groups.Add(this.grpGenomicInterval);
            this.tabBio.Groups.Add(this.groupNodeXL);
            this.tabBio.Groups.Add(this.groupConfig);
            this.tabBio.Groups.Add(this.groupAbout);
            this.tabBio.KeyTip = "B";
            this.tabBio.Label = ".NET Bio";
            this.tabBio.Name = "tabBio";
            // 
            // groupImportExport
            // 
            this.groupImportExport.Items.Add(this.splitImport);
            this.groupImportExport.Items.Add(this.splitExport);
            this.groupImportExport.Label = "Sequence Data";
            this.groupImportExport.Name = "groupImportExport";
            // 
            // splitImport
            // 
            this.splitImport.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.splitImport.Image = global::BiodexExcel.Properties.Resources.FromText;
            this.splitImport.ItemSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.splitImport.KeyTip = "I";
            this.splitImport.Label = "Import From";
            this.splitImport.Name = "splitImport";
            this.splitImport.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.RibbonControl_Click);
            // 
            // splitExport
            // 
            this.splitExport.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.splitExport.Image = global::BiodexExcel.Properties.Resources.FromText;
            this.splitExport.ItemSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.splitExport.KeyTip = "E";
            this.splitExport.Label = "Export To";
            this.splitExport.Name = "splitExport";
            this.splitExport.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.RibbonControl_Click);
            // 
            // groupAlignment
            // 
            this.groupAlignment.Items.Add(this.splitAligners);
            this.groupAlignment.Items.Add(this.btnCancelAlign);
            this.groupAlignment.Label = "Sequence Alignment";
            this.groupAlignment.Name = "groupAlignment";
            // 
            // splitAligners
            // 
            this.splitAligners.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.splitAligners.Image = global::BiodexExcel.Properties.Resources.FromText;
            this.splitAligners.ItemSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.splitAligners.KeyTip = "A";
            this.splitAligners.Label = "Select Aligner";
            this.splitAligners.Name = "splitAligners";
            // 
            // btnCancelAlign
            // 
            this.btnCancelAlign.KeyTip = "X";
            this.btnCancelAlign.Label = "Cancel";
            this.btnCancelAlign.Name = "btnCancelAlign";
            // 
            // groupAssembly
            // 
            this.groupAssembly.Items.Add(this.btnAssemble);
            this.groupAssembly.Items.Add(this.btnCancelAssemble);
            this.groupAssembly.Label = "Sequence Assembly";
            this.groupAssembly.Name = "groupAssembly";
            // 
            // btnAssemble
            // 
            this.btnAssemble.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnAssemble.Image = global::BiodexExcel.Properties.Resources.FromText;
            this.btnAssemble.KeyTip = "S";
            this.btnAssemble.Label = "Assemble";
            this.btnAssemble.Name = "btnAssemble";
            this.btnAssemble.ShowImage = true;
            // 
            // btnCancelAssemble
            // 
            this.btnCancelAssemble.KeyTip = "X";
            this.btnCancelAssemble.Label = "Cancel";
            this.btnCancelAssemble.Name = "btnCancelAssemble";
            // 
            // groupWebService
            // 
            this.groupWebService.Items.Add(this.splitWebService);
            this.groupWebService.Items.Add(this.btnCancelSearch);
            this.groupWebService.Label = "Web service";
            this.groupWebService.Name = "groupWebService";
            // 
            // splitWebService
            // 
            this.splitWebService.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.splitWebService.Image = global::BiodexExcel.Properties.Resources.FromWeb;
            this.splitWebService.ItemSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.splitWebService.KeyTip = "B";
            this.splitWebService.Label = "Select BLAST service";
            this.splitWebService.Name = "splitWebService";
            this.splitWebService.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.RibbonControl_Click);
            // 
            // btnCancelSearch
            // 
            this.btnCancelSearch.KeyTip = "X";
            this.btnCancelSearch.Label = "Cancel";
            this.btnCancelSearch.Name = "btnCancelSearch";
            // 
            // groupCharts
            // 
            this.groupCharts.Items.Add(this.splitChart);
            this.groupCharts.Label = "Charting";
            this.groupCharts.Name = "groupCharts";
            // 
            // splitChart
            // 
            this.splitChart.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.splitChart.Image = global::BiodexExcel.Properties.Resources.Charts;
            this.splitChart.Items.Add(this.btnRunChartMacro);
            this.splitChart.ItemSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.splitChart.KeyTip = "R";
            this.splitChart.Label = "Charts";
            this.splitChart.Name = "splitChart";
            this.splitChart.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.RibbonControl_Click);
            // 
            // btnRunChartMacro
            // 
            this.btnRunChartMacro.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnRunChartMacro.Image = global::BiodexExcel.Properties.Resources.Clustered_Chart;
            this.btnRunChartMacro.Label = "DNA Sequence Distribution Table";
            this.btnRunChartMacro.Name = "btnRunChartMacro";
            this.btnRunChartMacro.ShowImage = true;
            // 
            // grpGenomicInterval
            // 
            this.grpGenomicInterval.Items.Add(this.splitOperate);
            this.grpGenomicInterval.Label = "Genomic Intervals";
            this.grpGenomicInterval.Name = "grpGenomicInterval";
            // 
            // splitOperate
            // 
            this.splitOperate.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.splitOperate.Image = global::BiodexExcel.Properties.Resources.FromExcel;
            this.splitOperate.Items.Add(this.btnMerge);
            this.splitOperate.Items.Add(this.btnIntersect);
            this.splitOperate.Items.Add(this.btnSubtract);
            this.splitOperate.ItemSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.splitOperate.KeyTip = "G";
            this.splitOperate.Label = "Operate on Genomic Intervals";
            this.splitOperate.Name = "splitOperate";
            this.splitOperate.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.RibbonControl_Click);
            // 
            // btnMerge
            // 
            this.btnMerge.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnMerge.Image = global::BiodexExcel.Properties.Resources.FromExcel;
            this.btnMerge.Label = "Merge";
            this.btnMerge.Name = "btnMerge";
            this.btnMerge.ShowImage = true;
            // 
            // btnIntersect
            // 
            this.btnIntersect.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnIntersect.Image = global::BiodexExcel.Properties.Resources.FromExcel;
            this.btnIntersect.Label = "Intersect";
            this.btnIntersect.Name = "btnIntersect";
            this.btnIntersect.ShowImage = true;
            // 
            // btnSubtract
            // 
            this.btnSubtract.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnSubtract.Image = global::BiodexExcel.Properties.Resources.FromExcel;
            this.btnSubtract.Label = "Subtract";
            this.btnSubtract.Name = "btnSubtract";
            this.btnSubtract.ShowImage = true;
            // 
            // groupNodeXL
            // 
            this.groupNodeXL.Items.Add(this.btnVennDiagram);
            this.groupNodeXL.Label = "NodeXL";
            this.groupNodeXL.Name = "groupNodeXL";
            // 
            // btnVennDiagram
            // 
            this.btnVennDiagram.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnVennDiagram.Image = global::BiodexExcel.Properties.Resources.VennIcon;
            this.btnVennDiagram.KeyTip = "V";
            this.btnVennDiagram.Label = "Venn Diagram";
            this.btnVennDiagram.Name = "btnVennDiagram";
            this.btnVennDiagram.ShowImage = true;
            // 
            // groupConfig
            // 
            this.groupConfig.Items.Add(this.splitConfiguration);
            this.groupConfig.Label = "Configuration";
            this.groupConfig.Name = "groupConfig";
            // 
            // splitConfiguration
            // 
            this.splitConfiguration.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.splitConfiguration.Image = global::BiodexExcel.Properties.Resources.FromExcel;
            this.splitConfiguration.Items.Add(this.btnMaxColumn);
            this.splitConfiguration.Items.Add(this.btnConfigureColor);
            this.splitConfiguration.Items.Add(this.btnConfigureImport);
            this.splitConfiguration.ItemSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.splitConfiguration.KeyTip = "C";
            this.splitConfiguration.Label = "Configure";
            this.splitConfiguration.Name = "splitConfiguration";
            this.splitConfiguration.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.RibbonControl_Click);
            // 
            // btnMaxColumn
            // 
            this.btnMaxColumn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnMaxColumn.Description = "Change Sequence data wraparound column width (1 to 16000)";
            this.btnMaxColumn.Image = global::BiodexExcel.Properties.Resources.FromExcel;
            this.btnMaxColumn.Label = "Sequence Wrap-Around Column";
            this.btnMaxColumn.Name = "btnMaxColumn";
            this.btnMaxColumn.ShowImage = true;
            // 
            // btnConfigureColor
            // 
            this.btnConfigureColor.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnConfigureColor.Description = "Change color scheme for DNA,RNA and Protein molecules";
            this.btnConfigureColor.Image = global::BiodexExcel.Properties.Resources.FromExcel;
            this.btnConfigureColor.Label = "Change color scheme for molecules";
            this.btnConfigureColor.Name = "btnConfigureColor";
            this.btnConfigureColor.ShowImage = true;
            // 
            // btnConfigureImport
            // 
            this.btnConfigureImport.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnConfigureImport.Description = "Change how sequences are imported into spreadsheet";
            this.btnConfigureImport.Image = global::BiodexExcel.Properties.Resources.FromExcel;
            this.btnConfigureImport.Label = "Change import options";
            this.btnConfigureImport.Name = "btnConfigureImport";
            this.btnConfigureImport.ShowImage = true;
            // 
            // groupAbout
            // 
            this.groupAbout.Items.Add(this.btnHomePage);
            this.groupAbout.Items.Add(this.btnUserManual);
            this.groupAbout.Items.Add(this.btnAbout);
            this.groupAbout.Label = "Help";
            this.groupAbout.Name = "groupAbout";
            // 
            // btnHomePage
            // 
            this.btnHomePage.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnHomePage.Image = global::BiodexExcel.Properties.Resources.about;
            this.btnHomePage.KeyTip = "O";
            this.btnHomePage.Label = "Home Page";
            this.btnHomePage.Name = "btnHomePage";
            this.btnHomePage.ShowImage = true;
            // 
            // btnUserManual
            // 
            this.btnUserManual.Image = global::BiodexExcel.Properties.Resources.help;
            this.btnUserManual.KeyTip = "H";
            this.btnUserManual.Label = "Help";
            this.btnUserManual.Name = "btnUserManual";
            this.btnUserManual.ShowImage = true;
            this.btnUserManual.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnUserManual_Click);
            // 
            // btnAbout
            // 
            this.btnAbout.Image = global::BiodexExcel.Properties.Resources.info;
            this.btnAbout.KeyTip = "U";
            this.btnAbout.Label = "About .NET Bio";
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.ShowImage = true;
            this.btnAbout.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.RibbonControl_Click);
            // 
            // BioRibbon
            // 
            this.Name = "BioRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tabBio);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.OnRibbonLoad);
            this.tabBio.ResumeLayout(false);
            this.tabBio.PerformLayout();
            this.groupImportExport.ResumeLayout(false);
            this.groupImportExport.PerformLayout();
            this.groupAlignment.ResumeLayout(false);
            this.groupAlignment.PerformLayout();
            this.groupAssembly.ResumeLayout(false);
            this.groupAssembly.PerformLayout();
            this.groupWebService.ResumeLayout(false);
            this.groupWebService.PerformLayout();
            this.groupCharts.ResumeLayout(false);
            this.groupCharts.PerformLayout();
            this.grpGenomicInterval.ResumeLayout(false);
            this.grpGenomicInterval.PerformLayout();
            this.groupNodeXL.ResumeLayout(false);
            this.groupNodeXL.PerformLayout();
            this.groupConfig.ResumeLayout(false);
            this.groupConfig.PerformLayout();
            this.groupAbout.ResumeLayout(false);
            this.groupAbout.PerformLayout();
        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonSplitButton splitAligners;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupAssembly;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnAssemble;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnCancelAssemble;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupImportExport;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnHomePage;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnUserManual;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupNodeXL;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnVennDiagram;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSubtract;
        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabBio;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupWebService;
        internal Microsoft.Office.Tools.Ribbon.RibbonSplitButton splitImport;
        internal Microsoft.Office.Tools.Ribbon.RibbonSplitButton splitExport;
        internal Microsoft.Office.Tools.Ribbon.RibbonSplitButton splitWebService;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupAlignment;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnCancelSearch;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnCancelAlign;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupConfig;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupAbout;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnAbout;
        internal Microsoft.Office.Tools.Ribbon.RibbonSplitButton splitConfiguration;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnMaxColumn;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupCharts;
        internal Microsoft.Office.Tools.Ribbon.RibbonSplitButton splitChart;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnRunChartMacro;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpGenomicInterval;
        internal Microsoft.Office.Tools.Ribbon.RibbonSplitButton splitOperate;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnMerge;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnIntersect;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnConfigureColor;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnConfigureImport;
    }

    partial class ThisRibbonCollection
    {
        internal BioRibbon Ribbon1
        {
            get { return this.GetRibbon<BioRibbon>(); }
        }
    }
}
