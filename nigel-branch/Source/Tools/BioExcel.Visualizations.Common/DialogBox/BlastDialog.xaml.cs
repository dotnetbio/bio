namespace BiodexExcel.Visualizations.Common
{
    #region -- Using Directives --
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Bio;
    using Bio.Web;
    using Bio.Web.Blast;
    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// Interaction logic for BlastInput.xaml. BlastDialogue class
    /// will provide a pop-up to the user, which will be allow
    /// the user to configure input parameters to the BLAST webservice.
    /// These parameters will later be passed to BLAST webservice.
    /// </summary>
    public partial class BlastDialog
    {
        #region -- Private members --

        /// <summary>
        /// Describes the Database service parameter
        /// </summary>
        private const string DATABASE = "Database";

        /// <summary>
        /// String for retrieval of BioHPC DNA database names
        /// </summary>
        private const string DATABASEDNA = "DatabasesDna";

        /// <summary>
        /// String for retrieval of BioHPC protein database names
        /// </summary>
        private const string DATABASEPROT = "DatabasesProt";

        /// <summary>
        /// Describes the EntrezQuery service parameter
        /// </summary>
        private const string ENTREZQUERY = "EntrezQuery";

        /// <summary>
        /// Describes the Program service parameter
        /// </summary>
        private const string PROGRAM = "Program";

        /// <summary>
        /// Describes the Service service parameter
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
        /// Describes the NcbiGI service parameter
        /// </summary>
        private const string NCBIGI = "NcbiGI";

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
        /// Array to store BioHPC DNA database names
        /// </summary>
        private static string[] dnaDatabases;

        /// <summary>
        /// Array to store BioHPC Protein database names
        /// </summary>
        private static string[] protDatabases;

        /// <summary>
        /// Indicates whether the current dialog has to show NCBI parameters or EBI parameters. 
        /// </summary>
        private bool ncbi;

        /// <summary>
        /// Indicates whether the current dialog has to show BioHPC parameters.
        /// </summary>
        private bool biohpc;

        /// <summary>
        /// Name of selected Blast service
        /// </summary>
        private string serviceName;

        /// <summary>
        /// Stores information about default databases for various web services.
        /// </summary>
        private Dictionary<string, string> defaultDatabases = new Dictionary<string, string>();

        #endregion

        #region -- Constructor --

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
            this.ncbi = WebServices.NcbiBlast.Name.Equals(webServiceName);
            this.biohpc = WebServices.BioHPCBlast != null && WebServices.BioHPCBlast.Name.Equals(webServiceName);
            if (!biohpc)
            {
                UpdateDefaultDatabase();
            }

            this.InitializeComponent();
            this.btnCancel.Click += new RoutedEventHandler(this.OnBtnCancelClick);
            this.btnSubmit.Click += new RoutedEventHandler(this.OnBtnSubmitClick);
            this.Closed += new EventHandler(this.OnBlastDialogClosed);

            this.cmbProgram.SelectionChanged += new SelectionChangedEventHandler(this.OnCmbProgramSelectionChanged);
            this.cmbProgram.Tag = PROGRAM;
            this.txtDatabase.Tag = DATABASE;
            this.chkUseBrowerProxy.IsChecked = true;

            this.cmbProgram.ItemsSource = (BlastParameters.Parameters[PROGRAM].Validator as StringListValidator).ValidValues.OrderBy(val => val).ToList();
            this.cmbProgram.SelectedIndex = this.cmbProgram.Items.IndexOf(BlastParameters.Parameters[PROGRAM].DefaultValue);

            if (this.biohpc)
            {
                dnaDatabases = null;
                protDatabases = null;
                this.firstStk.Children.Remove(this.txtDatabase);
                this.firstStk.Children.Remove(this.txtDatabaseBlock);
                chkUseBrowerProxy.Visibility = Visibility.Collapsed;
                CreateDBListField(DATABASE, this.firstStk);
                this.GetBioHPCDatabases();
            }

            this.CreateTextField(ALIGNMENTS, this.commonParamsStk);
            this.CreateTextField(EXPECT, this.commonParamsStk);

            if (this.biohpc)
            {
                this.CreateComboFromBioHPC(FILTER, this.commonParamsStk);
                this.CreateCheckBox(EMAILNOTIFY, this.firstStk);
            }
            else
            {
                this.CreateTextField(FILTER, this.secondStk);
            }

            if (this.ncbi)
            {
                this.MinHeight = 515;
                this.MinWidth = 480;
                this.CreateTextField(ENTREZQUERY, this.firstStk);
                this.cmbService.SelectionChanged += new SelectionChangedEventHandler(this.OnCmbServiceSelectionChanged);
                this.cmbService.Tag = SERVICE;
                this.CreateTextField(EXPECTLOW, this.firstStk);
                this.CreateTextField(EXPEXTHIGH, this.firstStk);
                this.CreateTextField(HITLISTSIZE, this.firstStk);
                CreateComboField(QUERYBELIEVEDEFLINE, this.firstStk);
                this.CreateTextField(QUERYFROM, this.secondStk);
                this.CreateTextField(QUERYTO, this.secondStk);
                this.CreateTextField(EFFECTIVESEARCHSPACE, this.secondStk);
                this.CreateTextField(THRESHOLD, this.secondStk);
                CreateComboField(UNGAPPEDALIGNMENT, this.secondStk);
                CreateComboField(LCASEMASK, this.secondStk);
                this.CreateTextField(WORDSIZE, this.commonParamsStk);
                this.CreateTextField(ITHRESHOLD, this.secondStk);

                // Set the default value for gap costs
                this.gapOpenTxt.Text = "5";
                this.gapExtendedTxt.Text = "2";
            }
            else
            {
                this.MinHeight = 220;
                this.MinWidth = 480;
                if (this.biohpc)
                {
                    this.CreateTextField(EMAIL, this.stkConfigurationParams);
                }
                else
                {
                    this.CreateTextField(EMAIL, this.secondStk);
                }

                this.firstStk.Children.Remove(this.cmbService);
                this.stkGap.Visibility = Visibility.Collapsed;
                this.txtService.Visibility = Visibility.Collapsed;
                this.txtGapCost.Visibility = Visibility.Collapsed;
            }

            if (WebServices.EbiBlast.Name.Equals(webServiceName))
            {
                CreateComboField(SEQUENCETYPE, this.firstStk);
            }

            this.btnSubmit.Focus();
        }

        #endregion

        #region -- Public Events --

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

        #endregion

        #region -- Public Properties --

        /// <summary>
        /// Gets or sets the Web service input arguments
        /// </summary>
        public WebServiceInputEventArgs WebServiceInputArgs { get; set; }

        #endregion

        #region -- Private Static methods --

        /// <summary>
        /// This method would create combo field for the Blast parameter dialog,
        /// It will use the fieldname (which is the parameter name) 
        /// and stackpanel to be added.
        /// </summary>
        /// <param name="fieldName">field name</param>
        /// <param name="parentPanel">Stack panel, the field will be added to this stackpanel</param>
        private static void CreateComboField(string fieldName, StackPanel parentPanel)
        {
            TextBlock block = new TextBlock();
            block.Margin = new Thickness(0, 5, 5, 0);
            block.TextWrapping = TextWrapping.Wrap;
            block.Text = fieldName;
            parentPanel.Children.Add(block);

            ComboBox combo = new ComboBox();
            combo.HorizontalAlignment = HorizontalAlignment.Left;
            combo.IsSynchronizedWithCurrentItem = true;
            combo.Margin = new Thickness(0, 5, 5, 0);
            combo.Tag = fieldName;
            combo.Width = 100;
            StringListValidator validator = BlastParameters.Parameters[fieldName].Validator as StringListValidator;
            combo.ItemsSource = validator.ValidValues;
            combo.SelectedIndex = validator.ValidValues.IndexOf(BlastParameters.Parameters[fieldName].DefaultValue);

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
        private static bool AddValidServiceParams(ref BlastParameters serviceParam, string paramName, string paramValue)
        {
            RequestParameter param = BlastParameters.Parameters[paramName];
            if (string.IsNullOrEmpty(paramValue))
            {
                return true;
            }
            else if (param.DataType == PARAMTYPEINT && param.Validator == null)
            {
                int number;

                // Validate the int and double values which doesnot have validators.
                if (!Int32.TryParse(paramValue, out number))
                {
                    MessageBox.Show(Properties.Resources.INVALID_TEXT + paramName + Properties.Resources.VALUE_TEXT, Properties.Resources.CAPTION, MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            else if (param.DataType == PARAMTYPEDOUBLE && param.Validator == null)
            {
                double number;
                if (!Double.TryParse(paramValue, out number))
                {
                    MessageBox.Show(Properties.Resources.INVALID_TEXT + paramName + Properties.Resources.VALUE_TEXT, Properties.Resources.CAPTION, MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            if (param.IsValid(paramValue))
            {
                serviceParam.Add(paramName, paramValue);
                return true;
            }
            else
            {
                MessageBox.Show(Properties.Resources.INVALID_TEXT + paramName + Properties.Resources.VALUE_TEXT, Properties.Resources.CAPTION, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// This method checks if the BlastDialog will show NCBI parameters
        /// or EBI parameters.
        /// </summary>
        /// <param name="webserviceName">Webservice name</param>
        /// <returns>Whether the webservice is NCBI or EBI.</returns>
        private static bool CheckIfNcbi(string webserviceName)
        {
            foreach (IBlastServiceHandler webservice in WebServices.All)
            {
                if (webservice.Name.Equals(webserviceName)
                        && (webservice is NCBIBlastHandler))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// This method would validate and add the params to the service 
        /// parameters for the requested search
        /// </summary>
        /// <param name="configuration">Configuration parameter</param>
        /// <param name="paramName">Param name</param>
        /// <param name="paramValue">Param value</param>
        /// <returns>whether the parameter was valid</returns>
        private static bool CheckNAddConfiguration(ConfigParameters configuration, string paramName, string paramValue)
        {
            if (URL == paramName)
            {
                if (!Uri.IsWellFormedUriString(paramValue, UriKind.Absolute))
                {
                    MessageBox.Show(Properties.Resources.INVALID_TEXT + paramName + Properties.Resources.VALUE_TEXT, Properties.Resources.CAPTION, MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                else
                {
                    configuration.Connection = new Uri(paramValue);
                }
            }
            else if (EMAIL == paramName)
            {
                configuration.EmailAddress = paramValue;
            }

            return true;
        }

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
            TextBlock block = new TextBlock();
            block.Margin = new Thickness(0, 5, 5, 0);
            block.TextWrapping = TextWrapping.Wrap;
            block.Text = fieldName;
            parentPanel.Children.Add(block);
            block.Visibility = Visibility.Collapsed;

            ListBox combo = new ListBox();
            combo.HorizontalAlignment = HorizontalAlignment.Left;
            combo.IsSynchronizedWithCurrentItem = true;
            combo.Margin = new Thickness(0, 5, 5, 0);
            combo.Tag = fieldName;
            combo.Width = 200;
            combo.MaxHeight = 100;

            // multiple database selection possible
            combo.SelectionMode = SelectionMode.Multiple;
            parentPanel.Children.Add(combo);
            combo.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region -- Private Methods --

        /// <summary>
        /// This method would create text field for the Blast parameter dialog,
        /// It will use the fieldname (which is the parameter name) 
        /// and stackpanel to be added.
        /// </summary>
        /// <param name="fieldName">field name</param>
        /// <param name="parentPanel">Stack panel, the field will be added to this stackpanel</param>
        private void CreateTextField(string fieldName, StackPanel parentPanel)
        {
            TextBlock block = new TextBlock();
            block.Margin = new Thickness(0, 5, 5, 0);
            block.TextWrapping = TextWrapping.Wrap;
            block.Text = fieldName;

            TextBox box = new TextBox();
            box.Height = 22;

            box.HorizontalAlignment = HorizontalAlignment.Left;
            box.Margin = new Thickness(0, 5, 5, 0);

            box.TextWrapping = TextWrapping.Wrap;
            box.Tag = fieldName;

            switch (fieldName)
            {
                case URL:
                    box.Text = Properties.Resources.AZURE_URI;
                    box.Width = 300;
                    break;

                case EMAIL:
                    // Emails text box requires more space
                    box.Text = BlastParameters.Parameters[fieldName].DefaultValue;
                    box.Width = 200;
                    break;

                default:
                    // fill it up with default...
                    box.Text = BlastParameters.Parameters[fieldName].DefaultValue;
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
        private bool AddServiceParams(ref BlastParameters serviceParam, StackPanel panel)
        {
            bool valid = true;

            foreach (UIElement element in panel.Children)
            {
                TextBox txtBox = element as TextBox;
                ListBox lstBox = element as ListBox;
                CheckBox chkbox = element as CheckBox;
                if (txtBox != null)
                {
                    valid = AddValidServiceParams(ref serviceParam, txtBox.Tag.ToString(), txtBox.Text);
                }
                else if (lstBox != null)
                {
                    if (lstBox.Tag.ToString() == DATABASE && this.biohpc)
                    {
                        string parvalue = String.Empty;
                        foreach (string aux in lstBox.SelectedItems)
                        {
                            parvalue += aux + "|";
                        }

                        if (!String.IsNullOrEmpty(parvalue))
                        {
                            parvalue = parvalue.Substring(0, parvalue.Length - 1);
                            valid = AddValidServiceParams(ref serviceParam, lstBox.Tag.ToString(), parvalue);
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                }
                else if (chkbox != null)
                {
                    if (chkbox.Tag.ToString() == EMAILNOTIFY && this.biohpc)
                    {
                        if ((bool)chkbox.IsChecked)
                        {
                            valid = AddValidServiceParams(ref serviceParam, chkbox.Tag.ToString(), "yes");
                        }
                    }
                }
                else
                {
                    ComboBox combo = element as ComboBox;
                    if (combo != null && combo.Visibility == Visibility.Visible)
                    {
                        valid = AddValidServiceParams(ref serviceParam, combo.Tag.ToString(), combo.SelectedValue.ToString());
                    }
                }

                if (!valid)
                {
                    break;
                }
            }

            // checks the gap cost field
            if (valid && this.ncbi)
            {
                valid = this.CheckNAddGapCostField(ref serviceParam);
            }

            return valid;
        }

        /// <summary>
        /// This method would add Configuration parameter to the given parameter,
        /// The would get all the children items from the stack panel and would read the param values 
        /// and add to the configuration parameters.
        /// </summary>
        /// <param name="configuration">Configuration Parameter object</param>
        /// <param name="panel">Stack Panel</param>
        /// <returns>Success flag</returns>
        private bool AddConfigurationParameter(ConfigParameters configuration, StackPanel panel)
        {
            bool valid = true;

            foreach (UIElement element in panel.Children)
            {
                TextBox txtBox = element as TextBox;
                if (txtBox != null)
                {
                    valid = CheckNAddConfiguration(configuration, txtBox.Tag.ToString(), txtBox.Text);

                    if (!valid)
                    {
                        break;
                    }
                }
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
        private bool CheckNAddGapCostField(ref BlastParameters serviceParam)
        {
            int number;
            if (!Int32.TryParse(this.gapOpenTxt.Text, out number) && number != 0)
            {
                MessageBox.Show(Properties.Resources.INVALID_TEXT + GAPCOSTS + Properties.Resources.VALUE_TEXT, Properties.Resources.CAPTION, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Int32.TryParse(this.gapOpenTxt.Text, out number) && number != 0)
            {
                MessageBox.Show(Properties.Resources.INVALID_TEXT + GAPCOSTS + Properties.Resources.VALUE_TEXT, Properties.Resources.CAPTION, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            serviceParam.Add(GAPCOSTS, this.gapOpenTxt.Text + " " + this.gapExtendedTxt.Text);

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
            BlastParameters serviceParam = new BlastParameters();
            bool valid = this.AddServiceParams(ref serviceParam, this.firstStk);

            if (valid)
            {
                valid = this.AddServiceParams(ref serviceParam, this.secondStk);
            }

            if (valid)
            {
                valid = this.AddServiceParams(ref serviceParam, this.thirdColumnParams);
            }

            if (valid)
            {
                valid = this.AddServiceParams(ref serviceParam, this.commonParamsStk);
            }

            if (valid && this.serviceParams.Visibility == Visibility.Visible)
            {
                valid = this.AddServiceParams(ref serviceParam, this.serviceParams);
            }

            if (valid)
            {
                ConfigParameters configParams = new ConfigParameters();
                configParams.UseBrowserProxy = (bool)this.chkUseBrowerProxy.IsChecked;
                configParams.DefaultTimeout = 1;

                valid = this.AddConfigurationParameter(configParams, this.stkConfigurationParams);

                WebServiceInputEventArgs args = new WebServiceInputEventArgs(serviceParam, this.serviceName, configParams);
                this.WebServiceInputArgs = args;
                if (this.ExecuteSearch != null && valid)
                {
                    this.ExecuteSearch.Invoke(this, args);
                    this.Close();
                }
            }

            if (valid)
            {
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
            if (!biohpc)
            {
                this.txtDatabase.Text = defaultDatabases[e.AddedItems[0].ToString()];
            }

            switch (e.AddedItems[0].ToString())
            {
                case BLASTTYPEN:
                    this.cmbService.Items.Add(MEGABLAST);
                    this.cmbService.Items.Add(PLAIN);
                    this.cmbService.SelectedIndex = 1;
                    if (this.ncbi)
                    {
                        this.CreateTextField(MATCHREWARD, this.thirdColumnParams);
                        this.CreateTextField(MISMATCHPENALTY, this.thirdColumnParams);
                    }

                    break;

                case BLASTTYPEP:
                    this.cmbService.Items.Add(PHI);
                    this.cmbService.Items.Add(PSI);
                    this.cmbService.Items.Add(PLAIN);
                    this.cmbService.SelectedIndex = 2;
                    if (this.biohpc)
                    {
                        this.CreateComboFromBioHPC(MATRIXNAME, this.thirdColumnParams);
                    }
                    else
                    {
                        this.CreateTextField(MATRIXNAME, this.thirdColumnParams);
                    }

                    if (this.ncbi)
                    {
                        this.CreateTextField(COMPOSITIONBASEDSTATS, this.thirdColumnParams);
                    }

                    break;

                case BLASTTYPEX:
                    this.cmbService.IsEnabled = false;
                    this.txtService.Visibility = Visibility.Collapsed;
                    this.cmbService.Visibility = Visibility.Collapsed;
                    if (this.ncbi)
                    {
                        this.CreateTextField(GENETICCODE, this.thirdColumnParams);
                    }

                    if (this.biohpc)
                    {
                        this.CreateComboFromBioHPC(MATRIXNAME, this.thirdColumnParams);
                    }
                    else
                    {
                        this.CreateTextField(MATRIXNAME, this.thirdColumnParams);
                    }

                    break;

                case BLASTTYPETN:
                    this.cmbService.IsEnabled = false;
                    this.txtService.Visibility = Visibility.Collapsed;
                    this.cmbService.Visibility = Visibility.Collapsed;
                    if (this.biohpc)
                    {
                        this.CreateComboFromBioHPC(MATRIXNAME, this.thirdColumnParams);
                    }
                    else
                    {
                        this.CreateTextField(MATRIXNAME, this.thirdColumnParams);
                    }

                    break;

                case BLASTTYPETX:
                    this.cmbService.IsEnabled = false;
                    this.txtService.Visibility = Visibility.Collapsed;
                    this.cmbService.Visibility = Visibility.Collapsed;
                    if (this.biohpc)
                    {
                        this.CreateComboFromBioHPC(MATRIXNAME, this.thirdColumnParams);
                    }
                    else
                    {
                        this.CreateTextField(MATRIXNAME, this.thirdColumnParams);
                    }

                    break;
            }

            // Update DB listbox with databases appropraite for the given program (BioHPC only)
            if (this.biohpc)
            {
                this.ChangeDBListbox(e.AddedItems[0].ToString());
            }
        }

        /// <summary>
        ///  Contacts the BioHPC web service to retrieve the names of the available BLAST databases
        ///  and stores them in string arrays feeding the form controls
        /// </summary>
        private void GetBioHPCDatabases()
        {
            // Look for email and password in the form controls
            string email = String.Empty;
            string password = String.Empty;
            foreach (UIElement element in secondStk.Children)
            {
                TextBox tb = element as TextBox;
                PasswordBox pb = element as PasswordBox;
                if (pb != null)
                {
                    password = pb.Password;
                }

                if (tb != null)
                {
                    if ((string)tb.Tag == EMAIL)
                    {
                        email = tb.Text;
                    }
                }
            }

            // Retrieve the database names...
            dnaDatabases = ((BioHPCBlastHandler)WebServices.BioHPCBlast).GetServiceMetadata(DATABASEDNA);
            protDatabases = ((BioHPCBlastHandler)WebServices.BioHPCBlast).GetServiceMetadata(DATABASEPROT);

            // ... and update the control
            this.ChangeDBListbox(this.cmbProgram.SelectedValue.ToString());
            btnSubmit.Visibility = Visibility.Visible;

            // Look for the database text block and listbox and make them visible
            foreach (UIElement element in firstStk.Children)
            {
                ListBox tb = element as ListBox;
                TextBlock txb = element as TextBlock;
                if (tb != null)
                {
                    if ((string)tb.Tag == DATABASE)
                    {
                        tb.Visibility = Visibility.Visible;
                    }
                }

                if (txb != null)
                {
                    if (txb.Text == DATABASE)
                    {
                        txb.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        /// <summary>
        /// Changes the contents of the database list box upon change of the selected BLAST program
        /// </summary>
        /// <param name="blastprogram">Program to run blastall with</param>
        private void ChangeDBListbox(string blastprogram)
        {
            bool isDNA = false;
            isDNA = isDNA || blastprogram.ToLower() == BLASTTYPEN;
            isDNA = isDNA || blastprogram.ToLower() == BLASTTYPETN;
            isDNA = isDNA || blastprogram.ToLower() == BLASTTYPETX;

            foreach (UIElement element in this.firstStk.Children)
            {
                ListBox lstBox = element as ListBox;
                if (lstBox != null)
                {
                    lstBox.Items.Clear();
                    if (isDNA)
                    {
                        foreach (string aux in dnaDatabases)
                        {
                            lstBox.Items.Add(aux);
                        }
                    }
                    else
                    {
                        foreach (string aux in protDatabases)
                        {
                            lstBox.Items.Add(aux);
                        }
                    }
                    if (isDNA)
                    {
                        lstBox.SelectedItem = "nt";
                    }
                    else
                    {
                        lstBox.SelectedItem = "nr";
                    }
                    lstBox.ScrollIntoView(lstBox.SelectedItem);

                }
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
            CheckBox box = new CheckBox();
            box.HorizontalAlignment = HorizontalAlignment.Left;
            box.Margin = new Thickness(0, 5, 5, 0);
            box.Tag = fieldName;
            box.Content = fieldName;
            parentPanel.Children.Add(box);
        }

        /// <summary>
        /// This method would create combo field for the Blast parameter dialog,
        /// It will use the fieldname (which is the parameter name) 
        /// and stackpanel to be added. The values for the combo box are
        /// pulled from the metadata obtained from BioHPC web service interface.
        /// </summary>
        /// <param name="fieldName">field name</param>
        /// <param name="parentPanel">Stack panel, the field will be added to this stackpanel</param>
        private void CreateComboFromBioHPC(string fieldName, StackPanel parentPanel)
        {
            TextBlock block = new TextBlock();
            block.Margin = new Thickness(0, 5, 5, 0);
            block.TextWrapping = TextWrapping.Wrap;
            block.Text = fieldName;
            parentPanel.Children.Add(block);

            ComboBox combo = new ComboBox();
            combo.HorizontalAlignment = HorizontalAlignment.Left;
            combo.IsSynchronizedWithCurrentItem = true;
            combo.Margin = new Thickness(0, 5, 5, 0);
            combo.Tag = fieldName;
            combo.Width = 100;
            BioHPCBlastHandler mysvc = new BioHPCBlastHandler();
            combo.ItemsSource = mysvc.GetServiceMetadata(fieldName);
            switch (fieldName)
            {
                case FILTER: combo.SelectedIndex = 0;
                    break;
                case MATRIXNAME: combo.SelectedIndex = 3;
                    break;
                default: combo.SelectedIndex = 0;
                    break;
            }

            parentPanel.Children.Add(combo);
        }

        /// <summary>
        /// Add default databases for web service based on service name selected.
        /// 
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
            if (WebServices.NcbiBlast.Name.Equals(serviceName))
            {
                defaultDatabases.Add("blastn", "nt");
                defaultDatabases.Add("blastp", "nr");
                defaultDatabases.Add("blastx", "nr");
                defaultDatabases.Add("tblastn", "nt");
                defaultDatabases.Add("tblastx", "nt");
            }
            else if (WebServices.EbiBlast.Name.Equals(serviceName))
            {
                defaultDatabases.Add("blastn", "em_rel");
                defaultDatabases.Add("blastp", "uniprot");
                defaultDatabases.Add("blastx", "uniprot");
                defaultDatabases.Add("tblastn", "em_rel");
                defaultDatabases.Add("tblastx", "em_rel");
            }
            else
            {
                //Program "BlastN", "Tblastn", "Tblastx" will be added later based on database availability.
                defaultDatabases.Add("BlastP", "alu.a");
                defaultDatabases.Add("BlastX", "alu.a");
            }
        }

        #endregion
    }
}
