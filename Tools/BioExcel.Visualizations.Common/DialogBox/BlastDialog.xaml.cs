using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Bio;
using Bio.Web.Blast;

namespace BiodexExcel.Visualizations.Common
{
    /// <summary>
    /// Interaction logic for BlastInput.xaml. BlastDialogue class
    /// will provide a pop-up to the user, which will be allow
    /// the user to configure input parameters to the BLAST webservice.
    /// These parameters will later be passed to BLAST webservice.
    /// </summary>
    public partial class BlastDialog
    {
        /// <summary>
        /// Describes the Database service parameter
        /// </summary>
        private const string DATABASE = "Database";

        /// <summary>
        /// Describes the EntrezQuery service parameter
        /// </summary>
        private const string ENTREZQUERY = "EntrezQuery";

        /// <summary>
        /// Describes the Program service parameter
        /// </summary>
        private const string PROGRAM = "Program";

        /// <summary>
        /// Describes the Service parameter
        /// </summary>
        private const string SERVICE = "Service";

        /// <summary>
        /// Describes the ExpectLow service parameter
        /// </summary>
        private const string EXPECTLOW = "ExpectLow";

        /// <summary>
        /// Describes the ExpectHigh service parameter
        /// </summary>
        private const string EXPEXTHIGH = "ExpectHigh";

        /// <summary>
        /// Describes the RequestIdentifier service parameter
        /// </summary>
        private const string RID = "RequestIdentifier";

        /// <summary>
        /// Describes the HitlistSize service parameter
        /// </summary>
        private const string HITLISTSIZE = "HitlistSize";

        /// <summary>
        /// Describes the QueryBelieveDefline service parameter
        /// </summary>
        private const string QUERYBELIEVEDEFLINE = "QueryBelieveDefline";

        /// <summary>
        /// Describes the QueryFrom service parameter
        /// </summary>
        private const string QUERYFROM = "QueryFrom";

        /// <summary>
        /// Describes the QueryTo service parameter
        /// </summary>
        private const string QUERYTO = "QueryTo";

        /// <summary>
        /// Describes the EffectiveSearchSpace service parameter
        /// </summary>
        private const string EFFECTIVESEARCHSPACE = "EffectiveSearchSpace";

        /// <summary>
        /// Describes the Threshold service parameter
        /// </summary>
        private const string THRESHOLD = "Threshold";

        /// <summary>
        /// Describes the UngappedAlignment service parameter
        /// </summary>
        private const string UNGAPPEDALIGNMENT = "UngappedAlignment";

        /// <summary>
        /// Describes the Filter service parameter
        /// </summary>
        private const string FILTER = "Filter";

        /// <summary>
        /// Describes the LowercaseMask service parameter
        /// </summary>
        private const string LCASEMASK = "LowercaseMask";

        /// <summary>
        /// Describes the phi option
        /// </summary>
        private const string PHI = "phi";

        /// <summary>
        /// Describes the psi option
        /// </summary>
        private const string PSI = "psi";

        /// <summary>
        /// Describes the Alignments service parameter
        /// </summary>
        private const string ALIGNMENTS = "Alignments";

        /// <summary>
        /// Describes the Expect service parameter
        /// </summary>
        private const string EXPECT = "Expect";

        /// <summary>
        /// Describes the WordSize service parameter
        /// </summary>
        private const string WORDSIZE = "WordSize";

        /// <summary>
        /// Describes the NucleotideMismatchPenalty service parameter
        /// </summary>
        private const string MISMATCHPENALTY = "NucleotideMismatchPenalty";

        /// <summary>
        /// Describes the NucleotideMatchReward service parameter
        /// </summary>
        private const string MATCHREWARD = "NucleotideMatchReward";

        /// <summary>
        /// Describes the MatrixName service parameter
        /// </summary>
        private const string MATRIXNAME = "MatrixName";

        /// <summary>
        /// Describes the CompositionBasedStatistics service parameter
        /// </summary>
        private const string COMPOSITIONBASEDSTATS = "CompositionBasedStatistics";

        /// <summary>
        /// Describes the GeneticCode service parameter
        /// </summary>
        private const string GENETICCODE = "GeneticCode";

        /// <summary>
        /// Describes the GapCosts service parameter
        /// </summary>
        private const string GAPCOSTS = "GapCosts";

        /// <summary>
        /// Describes the PhiPattern service parameter
        /// </summary>
        private const string PHIPATTERN = "PhiPattern";

        /// <summary>
        /// Describes the Pssm option
        /// </summary>
        private const string PSSM = "Pssm";

        /// <summary>
        /// Describes the plain option
        /// </summary>
        private const string PLAIN = "plain";

        /// <summary>
        /// Describes the megablast option
        /// </summary>
        private const string MEGABLAST = "megablast";

        /// <summary>
        /// Describes the format type.
        /// </summary>
        private const string FORMATTYPE = "FormatType";

        /// <summary>
        /// Describes the IThreshold option.
        /// </summary>
        private const string ITHRESHOLD = "IThreshold";

        /// <summary>
        /// Describes the email option.
        /// </summary>
        private const string EMAIL = "Email";

        /// <summary>
        /// Describes the email notification option.
        /// </summary>
        private const string EMAILNOTIFY = "EmailNotify";

        /// <summary>
        /// Uri of the blast service.
        /// </summary>
        private const string URL = "Uri";

        /// <summary>
        /// Integer parameter type.
        /// </summary>
        private const string PARAMTYPEINT = "int";

        /// <summary>
        /// Double parameter type.
        /// </summary>
        private const string PARAMTYPEDOUBLE = "double";

        /// <summary>
        /// blastn Type
        /// </summary>
        private const string BLASTTYPEN = "blastn";

        /// <summary>
        /// blastn Type
        /// </summary>
        private const string BLASTTYPEP = "blastp";

        /// <summary>
        /// blastn Typeblastn
        /// </summary>
        private const string BLASTTYPEX = "blastx";

        /// <summary>
        /// blastn Type
        /// </summary>
        private const string BLASTTYPETN = "tblastn";

        /// <summary>
        /// blastn Type
        /// </summary>
        private const string BLASTTYPETX = "tblastx";

        /// <summary>
        /// Sequence Type parameter currently used only in Ebi Web-Service
        /// </summary>
        private const string SEQUENCETYPE = "SequenceType";

        /// <summary>
        /// Stores information about default databases for various web services.
        /// </summary>
        private readonly Dictionary<string, string> defaultDatabases = new Dictionary<string, string>();

        /// <summary>
        /// Name of selected Blast service
        /// </summary>
        private readonly string serviceName;

        /// <summary>
        /// Initializes a new instance of the BlastDialog class
        /// </summary>
        /// <param name="webServiceName">
        /// Name of the webservice for which
        /// we need this dialog will show parameters.
        /// </param>
        public BlastDialog(string webServiceName)
        {
            this.serviceName = webServiceName;
            this.UpdateDefaultDatabase();

            this.InitializeComponent();
            this.btnCancel.Click += this.OnBtnCancelClick;
            this.btnSubmit.Click += this.OnBtnSubmitClick;
            this.Closed += this.OnBlastDialogClosed;

            this.cmbProgram.SelectionChanged += this.OnCmbProgramSelectionChanged;
            this.cmbProgram.Tag = PROGRAM;
            this.txtDatabase.Tag = DATABASE;

            this.cmbProgram.ItemsSource = new[] {
                    BlastProgram.Blastn,
                    BlastProgram.Blastp,
                    BlastProgram.Blastx,
                    BlastProgram.Megablast,
                    BlastProgram.Tblastn,
                    BlastProgram.Tblastx
                }.OrderBy(val => val).ToList();
            this.cmbProgram.SelectedIndex = this.cmbProgram.Items.IndexOf(BlastProgram.Megablast);

            this.CreateTextField(ALIGNMENTS, this.commonParamsStk);
            this.CreateTextField(EXPECT, this.commonParamsStk);
            this.CreateTextField(FILTER, this.secondStk);

            this.MinHeight = 515;
            this.MinWidth = 480;
            this.CreateTextField(ENTREZQUERY, this.firstStk);
            this.cmbService.SelectionChanged += this.OnCmbServiceSelectionChanged;
            this.cmbService.Tag = SERVICE;
            this.CreateTextField(EXPECTLOW, this.firstStk);
            this.CreateTextField(EXPEXTHIGH, this.firstStk);
            this.CreateTextField(HITLISTSIZE, this.firstStk);
            //CreateComboField(QUERYBELIEVEDEFLINE, this.firstStk);
            this.CreateTextField(QUERYFROM, this.secondStk);
            this.CreateTextField(QUERYTO, this.secondStk);
            this.CreateTextField(EFFECTIVESEARCHSPACE, this.secondStk);
            this.CreateTextField(THRESHOLD, this.secondStk);
            //CreateComboField(UNGAPPEDALIGNMENT, this.secondStk);
            //CreateComboField(LCASEMASK, this.secondStk);
            this.CreateTextField(WORDSIZE, this.commonParamsStk);
            this.CreateTextField(ITHRESHOLD, this.secondStk);

            // Set the default value for gap costs
            this.gapOpenTxt.Text = "5";
            this.gapExtendedTxt.Text = "2";

            //if (WebServices.EbiBlast.Name.Equals(webServiceName))
            //{
            //    CreateComboField(SEQUENCETYPE, this.firstStk);
            //}

            this.btnSubmit.Focus();
        }

        /// <summary>
        /// Event to close the Pop up, It informs the
        /// Controller that the pop is closed and to
        /// close the Gray background.
        /// </summary>
        public event EventHandler ClosePopup;

        /// <summary>
        /// Event to close the Pop up, It informs the
        /// Controller that the pop is closed and to
        /// close the Gray background.
        /// </summary>
        public event EventHandler<WebServiceInputEventArgs> ExecuteSearch;

        /// <summary>
        /// Gets or sets the Web service input arguments
        /// </summary>
        public WebServiceInputEventArgs WebServiceInputArgs { get; set; }

        /// <summary>
        /// This method would create combo field for the Blast parameter dialog,
        /// It will use the fieldname (which is the parameter name)
        /// and stackpanel to be added.
        /// </summary>
        /// <param name="fieldName">field name</param>
        /// <param name="parentPanel">Stack panel, the field will be added to this stackpanel</param>
        /// <param name="values">Valid values</param>
        /// <param name="defaultValue">Default value</param>
        private static void CreateComboField(string fieldName, StackPanel parentPanel, string[] values, string defaultValue = "")
        {
            var block = new TextBlock {
                Margin = new Thickness(0, 5, 5, 0),
                TextWrapping = TextWrapping.Wrap,
                Text = fieldName
            };
            parentPanel.Children.Add(block);

            var combo = new ComboBox {
                HorizontalAlignment = HorizontalAlignment.Left,
                IsSynchronizedWithCurrentItem = true,
                Margin = new Thickness(0, 5, 5, 0),
                Tag = fieldName,
                Width = 100,
                ItemsSource = values,
                SelectedIndex = Array.IndexOf(values, defaultValue)
            };
            parentPanel.Children.Add(combo);
        }

        /// <summary>
        /// This method would validate and add the params to the service
        /// parameters for the requested search
        /// </summary>
        /// <param name="serviceParam">Service parameter</param>
        /// <param name="paramName">Param name</param>
        /// <param name="paramValue">Param value</param>
        /// <returns>whether the parameter was valid</returns>
        private static bool AddValidServiceParams(ref BlastRequestParameters serviceParam, string paramName, string paramValue)
        {
            return false;
        }

        ///// <summary>
        ///// This method checks if the BlastDialog will show NCBI parameters
        ///// or EBI parameters.
        ///// </summary>
        ///// <param name="webserviceName">Webservice name</param>
        ///// <returns>Whether the webservice is NCBI or EBI.</returns>
        //private static bool CheckIfNcbi(string webserviceName)
        //{
        //    foreach (IBlastServiceHandler webservice in WebServices.All)
        //    {
        //        if (webservice.Name.Equals(webserviceName) && (webservice is NCBIBlastHandler))
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        /// <summary>
        /// This method would create ListBox field for the Blast database parameter dialog,
        /// It will use the fieldname (which is the parameter name)
        /// and stackpanel to be added. The database list will be pulled (later) from
        /// the BioHPC web service...
        /// </summary>
        /// <param name="fieldName">field name</param>
        /// <param name="parentPanel">Stack panel, the field will be added to this stackpanel</param>
        private static void CreateDBListField(string fieldName, StackPanel parentPanel)
        {
            var block = new TextBlock {
                Margin = new Thickness(0, 5, 5, 0),
                TextWrapping = TextWrapping.Wrap,
                Text = fieldName
            };
            parentPanel.Children.Add(block);
            block.Visibility = Visibility.Collapsed;

            var combo = new ListBox {
                HorizontalAlignment = HorizontalAlignment.Left,
                IsSynchronizedWithCurrentItem = true,
                Margin = new Thickness(0, 5, 5, 0),
                Tag = fieldName,
                Width = 200,
                MaxHeight = 100,
                SelectionMode = SelectionMode.Multiple
            };

            // multiple database selection possible
            parentPanel.Children.Add(combo);
            combo.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// This method would create text field for the Blast parameter dialog,
        /// It will use the fieldname (which is the parameter name)
        /// and stackpanel to be added.
        /// </summary>
        /// <param name="fieldName">field name</param>
        /// <param name="parentPanel">Stack panel, the field will be added to this stackpanel</param>
        private void CreateTextField(string fieldName, StackPanel parentPanel)
        {
            var block = new TextBlock {
                Margin = new Thickness(0, 5, 5, 0),
                TextWrapping = TextWrapping.Wrap,
                Text = fieldName
            };

            var box = new TextBox {
                Height = 22,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 5, 5, 0),
                TextWrapping = TextWrapping.Wrap,
                Tag = fieldName
            };

            switch (fieldName)
            {
                case URL:
                    box.Text = "";
                    box.Width = 300;
                    break;

                case EMAIL:
                    // Emails text box requires more space
                    box.Text = "";
                    box.Width = 200;
                    break;

                default:
                    // fill it up with default...
                    box.Text = "";
                    box.Width = 100;
                    break;
            }

            // if parameter is phipattern or pssm
            if (fieldName == PHIPATTERN || fieldName == PSSM)
            {
                parentPanel.Children.Insert(0, box);
                parentPanel.Children.Insert(0, block);
            }
            else
            {
                parentPanel.Children.Add(block);
                parentPanel.Children.Add(box);
            }
        }

        /// <summary>
        /// This method would add service params to the given service param,
        /// The would get all the children items from the stack panel and would read the param values
        /// and add to the search service parameters.
        /// </summary>
        /// <param name="serviceParam">Service parameter</param>
        /// <param name="panel">Stack panel</param>
        /// <returns>returns whether the given param was added or not</returns>
        private bool AddServiceParams(ref BlastRequestParameters serviceParam, StackPanel panel)
        {
            bool valid = true;

            foreach (UIElement element in panel.Children)
            {
                var txtBox = element as TextBox;
                var lstBox = element as ListBox;
                var chkbox = element as CheckBox;
                if (txtBox != null)
                {
                    valid = AddValidServiceParams(ref serviceParam, txtBox.Tag.ToString(), txtBox.Text);
                }
                else if (lstBox != null)
                {
                }
                else if (chkbox != null)
                {
                }
                else
                {
                    var combo = element as ComboBox;
                    if (combo != null && combo.Visibility == Visibility.Visible)
                    {
                        valid = AddValidServiceParams(
                            ref serviceParam,
                            combo.Tag.ToString(),
                            combo.SelectedValue.ToString());
                    }
                }

                if (!valid)
                {
                    break;
                }
            }

            // checks the gap cost field
            if (valid)
            {
                valid = this.CheckNAddGapCostField(ref serviceParam);
            }

            return valid;
        }

        /// <summary>
        /// The event is fired when the Blast dialog
        /// is closed using the close button
        /// </summary>
        /// <param name="sender">Blast Dialog</param>
        /// <param name="e">Event data</param>
        private void OnBlastDialogClosed(object sender, EventArgs e)
        {
            if (this.ClosePopup != null)
            {
                this.ClosePopup(this, e);
            }
        }

        /// <summary>
        /// This method validates the gap cost input
        /// and on success adds the gap cost to the service parameters
        /// </summary>
        /// <param name="serviceParam">service param to add the param</param>
        /// <returns>whether the gap cost was valid and added or not</returns>
        private bool CheckNAddGapCostField(ref BlastRequestParameters serviceParam)
        {
            int number;
            if (!Int32.TryParse(this.gapOpenTxt.Text, out number) && number != 0)
            {
                MessageBox.Show(
                    Properties.Resources.INVALID_TEXT + GAPCOSTS + Properties.Resources.VALUE_TEXT,
                    Properties.Resources.CAPTION,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            if (!Int32.TryParse(this.gapOpenTxt.Text, out number) && number != 0)
            {
                MessageBox.Show(
                    Properties.Resources.INVALID_TEXT + GAPCOSTS + Properties.Resources.VALUE_TEXT,
                    Properties.Resources.CAPTION,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            serviceParam.ExtraParameters.Add(GAPCOSTS, this.gapOpenTxt.Text + " " + this.gapExtendedTxt.Text);

            return true;
        }

        /// <summary>
        /// This event is fired on click of the submit button on the dialog,
        /// this would validate the parameters accordingly and on success would initiate adding
        /// of the parameters to the given service parameters
        /// </summary>
        /// <param name="sender">submit button</param>
        /// <param name="e">Event Data</param>
        private void OnBtnSubmitClick(object sender, RoutedEventArgs e)
        {
            var serviceParam = new BlastRequestParameters();
            bool valid = this.AddServiceParams(ref serviceParam, this.firstStk);

            if (valid)
                valid = this.AddServiceParams(ref serviceParam, this.secondStk);

            if (valid)
                valid = this.AddServiceParams(ref serviceParam, this.thirdColumnParams);

            if (valid)
                valid = this.AddServiceParams(ref serviceParam, this.commonParamsStk);

            if (valid && this.serviceParams.Visibility == Visibility.Visible)
                valid = this.AddServiceParams(ref serviceParam, this.serviceParams);

            if (valid)
            {
                var args = new WebServiceInputEventArgs(this.serviceName, serviceParam);
                this.WebServiceInputArgs = args;
                if (this.ExecuteSearch != null)
                {
                    this.ExecuteSearch.Invoke(this, args);
                    this.Close();
                }
                else
                    this.Close();
            }
        }

        /// <summary>
        /// This event is fired on cancel click button,
        /// this would close the dialog and remove the gray back ground.
        /// </summary>
        /// <param name="sender">cancel button</param>
        /// <param name="e">Event data</param>
        private void OnBtnCancelClick(object sender, RoutedEventArgs e)
        {
            if (this.ClosePopup != null)
            {
                this.ClosePopup(this, e);
            }

            this.Close();
        }

        /// <summary>
        /// This method would create the parameters based on the
        /// selection of the service type for example: phi option
        /// has phipattern as added parameters
        /// </summary>
        /// <param name="sender">the service combo box</param>
        /// <param name="e">Event Data</param>
        private void OnCmbServiceSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.serviceParams.Visibility = Visibility.Visible;
            this.serviceParams.Children.Clear();
            if (this.cmbService.Items.Count > 0)
            {
                if (this.cmbService.SelectedValue.ToString() == PHI)
                {
                    this.CreateTextField(PHIPATTERN, this.serviceParams);
                }
                else if (this.cmbService.SelectedValue.ToString() == PSI)
                {
                    this.CreateTextField(PSSM, this.serviceParams);
                }
                else
                {
                    this.serviceParams.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// This method would create the parameters based on the
        /// selection of the program for example: Blastp will have phi,psi,plain as service options
        /// and blast n will have megablast n plain as service options.
        /// </summary>
        /// <param name="sender">the program combo box</param>
        /// <param name="e">Event Data</param>
        private void OnCmbProgramSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.cmbService.Items.Clear();
            this.cmbService.IsEnabled = true;
            this.cmbService.Visibility = Visibility.Visible;
            this.thirdColumnParams.Children.Clear();
            this.txtDatabase.Text = this.defaultDatabases[e.AddedItems[0].ToString()];
            switch (e.AddedItems[0].ToString())
            {
                case BLASTTYPEN:
                    this.cmbService.Items.Add(MEGABLAST);
                    this.cmbService.Items.Add(PLAIN);
                    this.cmbService.SelectedIndex = 1;
                    this.CreateTextField(MATCHREWARD, this.thirdColumnParams);
                    this.CreateTextField(MISMATCHPENALTY, this.thirdColumnParams);
                    break;

                case BLASTTYPEP:
                    this.cmbService.Items.Add(PHI);
                    this.cmbService.Items.Add(PSI);
                    this.cmbService.Items.Add(PLAIN);
                    this.cmbService.SelectedIndex = 2;
                    this.CreateTextField(MATRIXNAME, this.thirdColumnParams);
                    this.CreateTextField(COMPOSITIONBASEDSTATS, this.thirdColumnParams);
                    break;

                case BLASTTYPEX:
                    this.cmbService.IsEnabled = false;
                    this.txtService.Visibility = Visibility.Collapsed;
                    this.cmbService.Visibility = Visibility.Collapsed;
                    this.CreateTextField(GENETICCODE, this.thirdColumnParams);
                    this.CreateTextField(MATRIXNAME, this.thirdColumnParams);
                    break;

                case BLASTTYPETN:
                    this.cmbService.IsEnabled = false;
                    this.txtService.Visibility = Visibility.Collapsed;
                    this.cmbService.Visibility = Visibility.Collapsed;
                    this.CreateTextField(MATRIXNAME, this.thirdColumnParams);
                    break;

                case BLASTTYPETX:
                    this.cmbService.IsEnabled = false;
                    this.txtService.Visibility = Visibility.Collapsed;
                    this.cmbService.Visibility = Visibility.Collapsed;
                    this.CreateTextField(MATRIXNAME, this.thirdColumnParams);
                    break;
            }

        }

        /// <summary>
        /// This method creates a checkbox for the Blast parameter dialog,
        /// It uses the fieldname (which is the parameter name)
        /// and stackpanel to be added.
        /// </summary>
        /// <param name="fieldName">field name</param>
        /// <param name="parentPanel">Stack panel, the field will be added to this stackpanel</param>
        private void CreateCheckBox(string fieldName, StackPanel parentPanel)
        {
            var box = new CheckBox {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 5, 5, 0),
                Tag = fieldName,
                Content = fieldName
            };
            parentPanel.Children.Add(box);
        }

        /// <summary>
        /// Add default databases for web service based on service name selected.
        /// Repository: NCBI
        /// Service Name    Sequence Molecule Type  Database
        /// blastn	        Nucleotide              Nucleotide Collection (nt)
        /// blastp	        Protein	                Non Redundant Protein Sequences (nr)
        /// blastx	        Nucleotide	            Non Redundant Protein Sequences (nr)
        /// tblastn	        Protein	                Nucleotide Collection (nt)
        /// tblastx	        Nucleotide	            Nucleotide Collection (nt)
        /// Repository: EBI
        /// Service Name    Sequence Molecule Type  Database
        /// BLASTN	        Nucleotide	            EMBL Release(em_rel)
        /// TBLASTN	        Protein	                EMBL Release(em_rel)
        /// TBLASTX	        Nucleotide	            EMBL Release(em_rel)
        /// BLASTP	        Protein	                UniProt KnowledgeBase(uniprot)
        /// BLASTX	        Nucleotide	            UniProt KnowledgeBase(uniprot)
        /// Repository: Azure
        /// Service Name    Sequence Molecule Type  Database
        /// BlastP          Protein                 alu.a
        /// BlastX          Protein                 alu.a
        /// </summary>
        private void UpdateDefaultDatabase()
        {
            //if (WebServices.NcbiBlast.Name.Equals(this.serviceName))
            {
                this.defaultDatabases.Add("blastn", "nt");
                this.defaultDatabases.Add("blastp", "nr");
                this.defaultDatabases.Add("blastx", "nr");
                this.defaultDatabases.Add("tblastn", "nt");
                this.defaultDatabases.Add("tblastx", "nt");
            }
            //else if (WebServices.EbiBlast.Name.Equals(this.serviceName))
            //{
            //    this.defaultDatabases.Add("blastn", "em_rel");
            //    this.defaultDatabases.Add("blastp", "uniprot");
            //    this.defaultDatabases.Add("blastx", "uniprot");
            //    this.defaultDatabases.Add("tblastn", "em_rel");
            //    this.defaultDatabases.Add("tblastx", "em_rel");
            //}
            //else
            //{
            //    //Program "BlastN", "Tblastn", "Tblastx" will be added later based on database availability.
            //    this.defaultDatabases.Add("BlastP", "alu.a");
            //    this.defaultDatabases.Add("BlastX", "alu.a");
            //}
        }
    }
}