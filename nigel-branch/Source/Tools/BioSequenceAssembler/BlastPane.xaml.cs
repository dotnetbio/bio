namespace SequenceAssembler
{
    #region -- Using directives --

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using Bio.Web.Blast;

    #endregion -- Using directives --

    /// <summary>
    /// Interaction logic for BlastPane.xaml. Blast pane will display the 
    /// results of BLAST Webservice.Blast pane will display both
    /// the textual output and visual output (using Silvergene control)
    /// for a particular BLAST result.
    /// </summary>
    public partial class BlastPane : UserControl, IWebServicePresenter
    {
        #region -- Private Members --

        /// <summary>
        /// Holds an instance of BlastResult. This instance is used to 
        /// display Blast header information.
        /// </summary>
        private BlastResult blastResult;

        /// <summary>
        /// Describes  list of the blast result collator
        /// </summary>
        private IList<BlastResultCollator> blastResultCollator;

        /// <summary>
        /// GridViewColumn instance which is already sorted
        /// </summary>
        private GridViewColumnHeader lastSortedHeader;

        /// <summary>
        /// Gets or Sets the current column sort direction
        /// </summary>
        public ListSortDirection ColumnSortDirection
        {
            get { return (ListSortDirection)GetValue(ColumnSortDirectionProperty); }
            set { SetValue(ColumnSortDirectionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ColumnSortDirection.
        /// </summary>
        public static readonly DependencyProperty ColumnSortDirectionProperty =
            DependencyProperty.RegisterAttached("ColumnSortDirection", typeof(ListSortDirection), typeof(BlastPane), new UIPropertyMetadata(ListSortDirection.Ascending));

        /// <summary>
        /// Gets or Sets if this is a sorted column
        /// </summary>
        public bool IsSortedColumn
        {
            get { return (bool)GetValue(IsSortedColumnProperty); }
            set { SetValue(IsSortedColumnProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsSorted.
        /// </summary>
        public static readonly DependencyProperty IsSortedColumnProperty =
            DependencyProperty.RegisterAttached("IsSortedColumn", typeof(bool), typeof(BlastPane), new UIPropertyMetadata(false));

        #endregion -- Private Members --

        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the BlastPane class.
        /// </summary>
        public BlastPane()
        {
            InitializeComponent();
            this.blastResultCollator = new List<BlastResultCollator>();
            this.silverMapControl.BlastSerializer = new BlastXmlSerializer();
            this.lstSingleLineReport.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.OnListViewDoubleClick);
            this.lstSingleLineReport.PreviewKeyUp += new KeyEventHandler(this.OnListViewKeyUp);
            this.btnBlastHeader.Click += new RoutedEventHandler(this.OnBlastHeaderClicked);
            this.webServiceReportTab.SelectionChanged += new SelectionChangedEventHandler(this.OnWebServiceReportTabSelectionChanged);
        }
        
        #endregion -- Constructor --

        #region -- Public Methods --

        /// <summary>
        /// This method displays BLAST hits on the UI.
        /// </summary>
        /// <param name="results">BLAST hits</param>
        public void DisplayWebServiceOutput(IList<BlastResult> results, IBlastServiceHandler blastService, string databaseName)
        {
            if (results != null && results.Count >= 1)
            {
                this.RenderSingleLineReport(results, blastService, databaseName);
                this.blastResult = results[0];

                this.txtDate.Text = DateTime.Now.Date.ToString(CultureInfo.CurrentCulture);
                this.txtDataBaseName.Text = this.blastResult.Metadata.Database;
                this.txtVersion.Text = this.blastResult.Metadata.Version;                
            }
        }

        #endregion -- Public Methods --

        #region -- Private Static Methods --

        /// <summary>
        /// This method renders the multiple line report for BLAST hits.
        /// </summary>
        /// <param name="blastResult">BLAST hits</param>
        private static void RenderPairwiseSequenceAlignment(BlastResultCollator blastResult)
        {
            PairwiseSequenceAlignment alignment = new PairwiseSequenceAlignment(blastResult);
            alignment.ShowDialog();
        }
        #endregion

        #region -- Private Methods --
        
        /// <summary>
        /// This event is fired when The silvermap tab is selected
        /// </summary>
        /// <param name="sender">Tab control</param>
        /// <param name="e">Event data</param>
        private void OnWebServiceReportTabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.webServiceReportTab.SelectedIndex == 1 && this.blastResultCollator.Count == 0)
            {
                this.silverMapControl.InvokeSilverMap(this.blastResultCollator);
            }
        }

        /// <summary>
        /// This method renders the single line report for BLAST hits.
        /// </summary>
        /// <param name="results">BLAST hits</param>
        private void RenderSingleLineReport(IList<BlastResult> results, IBlastServiceHandler blastService, string databaseName)
        {
            this.lstSingleLineReport.ItemsSource = null;
            this.lstSingleLineReport.Items.Clear();
            this.blastResultCollator.Clear();
            this.Focus();
            foreach (BlastResult result in results)
            {
                foreach (BlastSearchRecord record in result.Records)
                {
                    if (null != record.Hits
                            && 0 < record.Hits.Count)
                    {
                        foreach (Hit hit in record.Hits)
                        {
                            if (null != hit.Hsps
                                    && 0 < hit.Hsps.Count)
                            {
                                foreach (Hsp hsp in hit.Hsps)
                                {
                                    BlastResultCollator blast = new BlastResultCollator();
                                    blast.Uri = BlastHitUrlResolver.ResolveUrl(hit.Id, blastService, databaseName);
                                    blast.Alignment = hsp.AlignmentLength;
                                    blast.Bit = hsp.BitScore;
                                    blast.EValue = hsp.EValue;
                                    blast.Identity = hsp.IdentitiesCount;
                                    blast.Length = hit.Length;
                                    blast.QEnd = hsp.QueryEnd;
                                    blast.QStart = hsp.QueryStart;
                                    blast.QueryId = record.IterationQueryId;
                                    blast.SEnd = hsp.HitEnd;
                                    blast.SStart = hsp.HitStart;
                                    blast.SubjectId = hit.Id;
                                    blast.Positives = hsp.PositivesCount;
                                    blast.QueryString = hsp.QuerySequence;
                                    blast.SubjectString = hsp.HitSequence;
                                    blast.Accession = hit.Accession;
                                    blast.Description = hit.Def;
                                    blast.Gaps = hsp.Gaps;
                                    this.blastResultCollator.Add(blast);
                                }
                            }
                        }
                    }
                }
            }
            
            this.lstSingleLineReport.ItemsSource = this.blastResultCollator;

            // Invoke the Silvermap
            if (this.blastResultCollator.Count > 0 || this.webServiceReportTab.SelectedIndex == 1)
            {
                this.silverMapControl.InvokeSilverMap(this.blastResultCollator);
            }
        }       

        /// <summary>
        /// This method listens to the double click on the List view
        /// and if the a ListView item has been clicked, then it displays
        /// the Pairwise sequence alignment report.
        /// </summary>
        /// <param name="sender">ListView instance</param>
        /// <param name="e"> Event data.</param>
        private void OnListViewDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is ListViewItem))   
            {       
                dep = VisualTreeHelper.GetParent(dep);   
            }

            if (dep == null)
            {
                return;
            }

            BlastResultCollator item = this.lstSingleLineReport.ItemContainerGenerator.ItemFromContainer(dep) as BlastResultCollator;
            if (item != null)
            {
                BlastPane.RenderPairwiseSequenceAlignment(item);
            }
        }

        /// <summary>
        /// This method is invoked when "View Blast Header" button is clicked.
        /// </summary>
        /// <param name="sender">btnBlastHeader button.</param>
        /// <param name="e">Event data,</param>
        private void OnBlastHeaderClicked(object sender, RoutedEventArgs e)
        {
            BlastHeader header = new BlastHeader(this.blastResult.Metadata);
            header.ShowDialog();
        }

        /// <summary>
        /// This method listens to the key down on the ListViewItem and if thekey pressed is the "Enter Key"
        /// , then it displays the Pairwise sequence alignment report.
        /// </summary>
        /// <param name="sender">ListView instance.</param>
        /// <param name="e">Event Data.</param>
        private void OnListViewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DependencyObject dep = (DependencyObject)e.OriginalSource;
                if (dep != null)
                {
                    BlastResultCollator item = this.lstSingleLineReport.ItemContainerGenerator.ItemFromContainer(dep) as BlastResultCollator;
                    if (item != null)
                    {
                        BlastPane.RenderPairwiseSequenceAlignment(item);
                    }
                }
            }
        } 

        /// <summary>
        /// Raised when user clicks a column header in the blast results
        /// </summary>
        /// <param name="sender">Column which was clicked</param>
        /// <param name="e">event args</param>
        private void OnSingleLineReportHeaderClick(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection sortDirection = 0;
            
            if (headerClicked != null)
            {
                if (headerClicked == lastSortedHeader) // if the clicked header is already sorted
                {
                    // reverse the sort
                    if ((ListSortDirection)headerClicked.GetValue(ColumnSortDirectionProperty) == ListSortDirection.Descending)
                    {
                        sortDirection = ListSortDirection.Ascending;
                    }
                    else
                    {
                        sortDirection = ListSortDirection.Descending;
                    }
                }
                else
                {
                    sortDirection = ListSortDirection.Ascending;
                }

                // Get the property to sort on, by taking the binding path from the column
                string bindingPath = (headerClicked.Column.DisplayMemberBinding as Binding).Path.Path as string;

                DoSort(this.lstSingleLineReport.ItemsSource, bindingPath, sortDirection);

                // Set apropriate attached properties to show the sort indicator in UI
                if (lastSortedHeader != null)
                {
                    lastSortedHeader.SetValue(IsSortedColumnProperty, false);
                }
                lastSortedHeader = headerClicked;
                headerClicked.SetValue(IsSortedColumnProperty, true);
                headerClicked.SetValue(ColumnSortDirectionProperty, sortDirection);
            }
        }

        /// <summary>
        /// Creates a sort description to sort the given IEnumerable
        /// </summary>
        /// <param name="itemsSource">IEnumerable to sort</param>
        /// <param name="bindingPath">Propert to sort on</param>
        /// <param name="sortDirection">Sort direction</param>
        private void DoSort(System.Collections.IEnumerable itemsSource, string bindingPath, ListSortDirection sortDirection)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(itemsSource);
            collectionView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(bindingPath, sortDirection);
            collectionView.SortDescriptions.Add(sd);
            collectionView.Refresh();
        }

        /// <summary>
        /// Fired when user clicks the subject id hyperlink
        /// </summary>
        /// <param name="e">Parameter containing url to navigate to</param>
        private void OnSubjectIDRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        #endregion -- Private Methods --
    }
}
