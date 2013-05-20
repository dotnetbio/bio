namespace SequenceAssembler
{
    #region -- Using directives --

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Bio;
    using Bio.Algorithms.Alignment;
    using Bio.Algorithms.Assembly;
    using Bio.IO.GenBank;
    using SequenceAssembler.Properties;
    using Bio.IO;


    #endregion -- Using directives --

    /// <summary>
    /// Interaction logic for AssemblerPane.xaml. Assembler pane will host
    /// sequence view and consensus view. Assembler pane will display
    /// Sequence and Consensus tree view along with selected sequences,assembly report etc.
    /// </summary>
    public partial class AssemblerPane : UserControl, IAssembler
    {
        #region -- Private members --

        /// <summary>
        /// Last opened blast dialog, to show it again in case of an error
        /// </summary>
        private Window lastBlastDialog;

        /// <summary>
        /// Background brush for treeview item which is already added to the workspace
        /// </summary>
        private Brush addedSequenceItemBackgroundBrush;

        /// <summary>
        /// Describes the selected treeview item (Framework Element).
        /// </summary>
        private TreeViewItem selectedTreeViewItem;

        /// <summary>
        /// Describes the Selected Sequences
        /// </summary>
        private VirtualList<ISequence> selectedSequences = new VirtualList<ISequence>();

        /// <summary>
        /// Describes the supported file types.
        /// </summary>
        private Collection<string> fileTypes;

        /// <summary>
        /// Describes the List of the extensions without the '.' character
        /// </summary>
        private Collection<string[]> extensionsList;

        /// <summary>
        /// Describes the Title of the format type.
        /// </summary>
        private Collection<string> fileTypeHeaders;

        /// <summary>
        /// Indicates whether the consensus view has already been built or not.
        /// </summary>
        private bool alignerOrAssemblyResultPresent;

        /// <summary>
        /// Describes the current contig sequence being editted.
        /// </summary>
        private MenuItem editItem;

        /// <summary>
        /// Describes the selected sequence
        /// </summary>
        private ISequence selectedSequence;

        /// <summary>
        /// Indicates whether contig selection has changed or not.
        /// </summary>
        private bool contigChanged;

        /// <summary>
        /// Holds a reference to EditSequenceDialog.
        /// </summary>
        private EditSequenceDialog dialog;

        /// <summary>
        /// Available pairwise Aligners.
        /// </summary>
        private IEnumerable<string> algorithms;

        #endregion

        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the AssemblerPane class.
        /// </summary>
        public AssemblerPane()
        {
            InitializeComponent();
            this.SetTabIndex();
            addedSequenceItemBackgroundBrush = this.FindResource("AddedTreeViewItemBrush") as Brush;
            this.btnAssemble.Click += this.OnAssembleClick;
            this.btnAlign.Click += this.OnAlignClick;
            this.sequenceViewer.Drop += new DragEventHandler(this.OnSequenceTreeItemDrop);
            this.btnCancelAssembly.Click += new RoutedEventHandler(this.OnCancelAssemblyClick);
            this.consensusTree.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(this.OnConsensusTreeSelectedItemChanged);
            this.sequenceTree.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(OnSequenceTreeSelectionChanged);
            this.sequenceTree.GotFocus += new RoutedEventHandler(sequenceTree_GotFocus);
            this.consensusTree.GotFocus += new RoutedEventHandler(consensusTree_GotFocus);
            this.executeServiceBtn.Click += new RoutedEventHandler(this.OnExecuteServiceBtnClick);
            this.cancelServiceBtn.Click += new RoutedEventHandler(this.OnCancelServiceBtnClick);
            this.customSequenceView.MouseWheel += new MouseWheelEventHandler(OnCustomSequenceView_MouseWheel);
            this.consensusCustomView.MouseWheel += new MouseWheelEventHandler(OnConsensusCustomView_MouseWheel);

            // Initiates the custom sequence view
            this.customSequenceView.AllowRemove = true;
            this.customSequenceView.RemoveSequence += new System.EventHandler<RemoveSequenceEventArgs>(this.OnSequenceViewerRemoveSequence);

            this.executeServiceBtn.IsEnabled = false;
            this.customViewTab.IsSelected = true;
        }

        #endregion

        #region -- Public Event --

        /// <summary>
        /// This event is fired when user wants to assemble the sequences.
        /// This event will be raised by IAssembler. The controller class
        /// instantiates algorithm implementation to perform assembly.
        /// </summary>
        public event EventHandler<AssemblyInputEventArgs> RunAssemblerAlgorithm;

        /// <summary>
        /// This event is fired when user wants to align the sequences.
        /// This event will be raised by IAssembler. The controller class
        /// instantiates algorithm implementation to perform alignment.
        /// </summary>
        public event EventHandler<AlignerInputEventArgs> RunAlignerAlgorithm;

        /// <summary>
        /// This event is fired when user wants to save the sequences.
        /// This event will be raised by IAssembler. The controller class
        /// instantiates fromatters to save the sequence.
        /// </summary>
        public event EventHandler<SaveSequenceInputEventArgs> SaveSequence;

        /// <summary>
        /// This event is fired when user wants to search for relevant
        /// sequences in NCBI database, using NCBI blast webservice.
        /// This event will be raised by IAssembler. The controller class
        /// instantiates the actual webservice implementation to perform search.
        /// </summary>
        public event EventHandler<WebServiceInputEventArgs> ExecuteSearchStarted;

        /// <summary>
        /// This event is fired when user cancels the search for relevant
        /// sequences in NCBI database, using NCBI blast webservice.
        /// This event will be raised by IAssembler. The controller class
        /// cancels the actual webservice implementation search. 
        /// </summary>
        public event EventHandler<WebServiceInputEventArgs> CancelSearch;

        /// <summary>
        /// This event is fired when user wants to assemble the sequences.
        /// This event will be raised by IAssembler. The controller class
        /// instantiates algorithm implementation to perform assembly.
        /// </summary>
        public event EventHandler ImportCompleted;

        /// <summary>
        /// This event is fired when user wants to cancel the assemble of 
        /// the sequences.This event will be raised by IAssembler.
        /// </summary>
        public event EventHandler CancelAssembly;

        /// <summary>
        /// Event to Show Dialog which would bring 
        /// the Graying effect to the Dialog
        /// </summary>
        public event EventHandler PopupOpened;

        /// <summary>
        /// Event to close the Dialog which would close 
        /// the Gray effect for the dialog.
        /// </summary>
        public event EventHandler PopupClosed;

        /// <summary>
        /// Event to inform the controller about the file 
        /// has been unloaded.
        /// </summary>
        public event EventHandler<FileUnloadedEventArgs> FileUnloaded;

        /// <summary>
        /// Event to inform the controller about the Save Item to be 
        /// enabled or disabled
        /// </summary>
        public event EventHandler UpdateSaveItemStatus;

        #endregion

        #region -- Public Properties --

        /// <summary>
        /// Gets a value indicating whether the 
        /// item should be enabled or not.
        /// </summary>
        public bool SaveItemEnabled
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the selected consensus sequence
        /// </summary>
        public ISequence SelectedSequence
        {
            get
            {
                return this.selectedSequence;
            }
        }

        /// <summary>
        /// Gets the Selected Sequences
        /// </summary>
        public VirtualList<ISequence> SelectedSequences
        {
            get
            {
                return this.selectedSequences;
            }
        }

        #endregion

        #region -- IAssembler Members --
        /// <summary>
        /// This method closes the progress bar and shows 
        /// the webservice button bar
        /// </summary>
        /// <param name="success">Completed successfully or not</param>
        public void SearchCompleted(bool success)
        {
            if (success)
            {
                this.lastBlastDialog.Close();

                this.blastResultsExpander.IsExpanded = true;
                this.blastResultsExpander.IsEnabled = true;
                this.consensusViewExpander.IsExpanded = false;
                this.sequenceViewExpander.IsExpanded = false;
            }
            else
            {
                this.stkWebService.Visibility = Visibility.Visible;
                this.stkWebProgressBar.Visibility = Visibility.Collapsed;

                this.lastBlastDialog.ShowDialog();
            }

            this.stkWebService.Visibility = Visibility.Visible;
            this.stkWebProgressBar.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// This method will build the sequence tree view.
        /// </summary>
        /// <param name="sequences">List of ISequences to build the tree view.</param>
        public void DisplaySequenceTreeView(IEnumerable<ParsedFileInfo> sequences)
        {
            MouseButtonEventHandler itemClick = new MouseButtonEventHandler(this.OnItemMouseUp);
            MouseEventHandler itemMouseEnter = new MouseEventHandler(OnSequenceTreeItemMouseEnter);

            foreach (ParsedFileInfo info in sequences)
            {
                TreeViewItem fileItem = new TreeViewItem();
                fileItem.MouseMove += new System.Windows.Input.MouseEventHandler(this.OnSequenceTreeItemMouseMove);
                fileItem.DragOver += new DragEventHandler(this.OnSequenceTreeItemDragOver);

                int sequenceIndex = 0;
                FileInfo fileInfo = new FileInfo(info.FileName);
                fileItem.Header = fileInfo.Name;
                fileItem.Tag = info;

                foreach (ISequence currentSeq in info.Sequence)
                {
                    TreeViewItem item = new TreeViewItem();
                    if (string.IsNullOrEmpty(currentSeq.ID))
                    {
                        SetSequenceDisplayID(currentSeq, Resource.SEQUENCE_TEXT_SV + (sequenceIndex + 1).ToString(CultureInfo.CurrentCulture));
                    }

                    item.Header = currentSeq.ID;
                    item.Tag = sequenceIndex;
                    item.ToolTip = info.FileName;
                    item.MouseUp += itemClick;
                    item.MouseEnter += itemMouseEnter;
                    item.Resources.Add(SystemColors.ControlBrushKey, Brushes.LightBlue);

                    sequenceIndex++;
                    fileItem.Items.Add(item);
                    AddSequenceToWorkspace(item);
                }

                fileItem.ContextMenu = this.CreateSequenceContextMenu(fileItem);

                this.sequenceTree.Items.Add(fileItem);

                fileItem.IsExpanded = true;
            }

            // Expand the sequences expander control and collapse others
            if (sequences.Count() > 0)
            {
                this.sequenceViewExpander.IsExpanded = true;
                this.consensusViewExpander.IsExpanded = false;
                this.blastResultsExpander.IsExpanded = false;
            }

            // Hide the file load dialog
            if (this.ImportCompleted != null)
            {
                this.ImportCompleted(this, null);
            }

            RefreshSequencesCustomView();
        }

        /// <summary>
        /// Sets display id of a given ISeuqence object by trying to convert it to its concrete class
        /// </summary>
        /// <param name="sequence">Sequence object of which display id has to be set</param>
        /// <param name="ID">Id to be set on the sequence</param>
        private void SetSequenceDisplayID(ISequence sequence, string ID)
        {
            if (sequence is Sequence)
            {
                (sequence as Sequence).ID = ID;
            }
            else if (sequence is QualitativeSequence)
            {
                (sequence as QualitativeSequence).ID = ID;
            }
        }

        /// <summary>
        /// Raised when user clicks save as option on the context menu of a parent treeview item in sequences treeview
        /// </summary>
        /// <param name="sender">Treeview item</param>
        /// <param name="e">Event args</param>
        private void OnSaveSequencesClick(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = (e.OriginalSource as MenuItem).Tag as TreeViewItem;
            if (selectedItem.Tag is ParsedFileInfo)
            {
                SaveSelectedSequences(selectedItem);
            }
            else if (selectedItem.Tag is int)
            {
                ISequence sequence = GetSequenceFromTreeViewItem(selectedItem);
                if (sequence != null)
                {
                    this.SaveSequences(new Collection<ISequence> { sequence });
                }
            }
        }

        /// <summary>
        /// Save all selected sequences in sequences treeview
        /// </summary>
        /// <param name="parentItem">Treeview item of which child sequences has to be saved.</param>
        private void SaveSelectedSequences(TreeViewItem parentItem)
        {
            Collection<ISequence> sequencesToSave = new Collection<ISequence>();
            IList<ISequence> allSequences = (parentItem.Tag as ParsedFileInfo).Sequence;

            foreach (TreeViewItem sequenceItem in parentItem.Items)
            {
                if (sequenceItem.Background != null && sequenceItem.Background != Brushes.Transparent && sequenceItem.Tag is int)
                {
                    sequencesToSave.Add(allSequences[(int)sequenceItem.Tag]);
                }
            }

            if (sequencesToSave.Count > 0)
            {
                this.SaveSequences(sequencesToSave);
            }
            else
            {
                MessageDialogBox.Show(Properties.Resource.NoSequencesSelected, Properties.Resource.CAPTION, MessageDialogButton.OK);
            }
        }

        /// <summary>
        /// Extract the source list and sequence index from the given TreeViewItem and add it to the workspace
        /// </summary>
        /// <param name="item">Treeview item to get sequence from</param>
        private void AddSequenceToWorkspace(TreeViewItem item)
        {
            item.Background = addedSequenceItemBackgroundBrush;

            IList<ISequence> sourceList = ((item.Parent as TreeViewItem).Tag as ParsedFileInfo).Sequence;

            this.selectedSequences.Add(sourceList, (int)item.Tag);
        }

        void OnSequenceTreeItemMouseEnter(object sender, MouseEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null && item.ContextMenu == null && item.Parent is TreeViewItem)
            {
                item.ToolTip = this.CreateToolTip(item.ToolTip.ToString(), GetSequenceFromTreeViewItem(item), false);
                item.ContextMenu = this.CreateSequenceContextMenu(item);
            }
        }

        /// <summary>
        /// This method will display the list of available alignment
        /// algorithms to the user.
        /// </summary>
        /// <param name="algorithms">List of alignment algorithms.</param>
        public void InitializeAlignmentAlgorithms(IEnumerable<string> algorithms)
        {
            this.algorithms = algorithms;
        }

        /// <summary>
        /// Initializes the supported files types, which would be used to 
        /// create filters for the save dialog.
        /// </summary>
        /// <param name="types">Supported File Types</param>
        public void InitializeSupportedFileTypes(Collection<string> types)
        {
            this.fileTypes = types;
            this.extensionsList = new Collection<string[]>();
            this.fileTypeHeaders = new Collection<string>();
            this.ExtractExtensions();
        }

        /// <summary>
        /// This method will display the list of available webservices
        /// to the user.
        /// </summary>
        /// <param name="webServiceNames">List of webservice names.</param>
        public void InitializeSupportedWebServices(IEnumerable<string> webServiceNames)
        {
            if (webServiceNames.Count() > 0)
            {

                foreach (string webservice in webServiceNames)
                {
                    if (!string.IsNullOrEmpty(webservice))
                    {
                        cmbWebServices.Items.Add(webservice);
                    }
                }

                cmbWebServices.Text = Properties.Resource.DefaultWebService;
            }
        }

        /// <summary> 
        /// This method will build consensus view UI.
        /// </summary>               
        /// <param name="assemblyOutput">Contains output of the assembly process.</param>        
        public void BuildConsensusView(AssemblyOutput assemblyOutput)
        {
            bool assembled = false;

            this.assembleProgress.Visibility = Visibility.Collapsed;
            this.assemblePane.Visibility = Visibility.Visible;

            treeViewConsensus.Header = "Contigs";
            consensusTreeViewCaption.Text = Properties.Resource.ContigsTreeView;
            consensusViewExpander.Header = Properties.Resource.Contigs;

            if (assemblyOutput != null)
            {
                IDeNovoAssembly assembly = assemblyOutput.SequenceAssembly;
                this.ClearOlderConsensus();
                this.consensusCustomView.ClearTopView();
                this.alignerOrAssemblyResultPresent = true;
                this.stkAssemblyReport.Visibility = Visibility.Visible;

                if (assemblyOutput.AssemblerUsed == AssemblerType.PaDeNA)
                {
                    try
                    {
                        AddContigsToTree(assemblyOutput.Contigs);
                    }
                    catch (TimeoutException)
                    {
                        MessageDialogBox.Show(Properties.Resource.TreeViewLoadTimeOut, Properties.Resource.CAPTION, MessageDialogButton.OK);
                        ClearOlderConsensus();
                        this.consensusViewExpander.IsExpanded = false;
                        this.consensusViewExpander.IsEnabled = false;
                        return;
                    }

                    assembled = true;
                }
                else
                {
                    int sequenceNumber = 1;

                    // checks whether the assembling was successful or not
                    if (assembly != null)
                    {
                        IOverlapDeNovoAssembly overlapAssembly = assembly as IOverlapDeNovoAssembly;
                        if (overlapAssembly != null)
                        {
                            // create the treeview for the contigs
                            try
                            {
                                sequenceNumber = AddContigsToTree(overlapAssembly.Contigs);
                            }
                            catch (TimeoutException)
                            {
                                MessageDialogBox.Show(Properties.Resource.TreeViewLoadTimeOut, Properties.Resource.CAPTION, MessageDialogButton.OK);
                                ClearOlderConsensus();
                                this.consensusViewExpander.IsExpanded = false;
                                this.consensusViewExpander.IsEnabled = false;
                                return;
                            }

                            if (overlapAssembly.UnmergedSequences.Count > 0)
                            {
                                TreeViewItem unassembledItem = new TreeViewItem();
                                unassembledItem.ItemContainerStyle = this.FindResource("ConsensusTreeViewItem") as Style;
                                unassembledItem.Header = Properties.Resource.UNASSEMBLED_TEXT;

                                foreach (ISequence sequence in overlapAssembly.UnmergedSequences)
                                {
                                    TreeViewItem sequenceItem = new TreeViewItem();
                                    string displayId = string.Empty;

                                    if (!string.IsNullOrEmpty(sequence.ID))
                                    {
                                        displayId = sequence.ID;
                                    }

                                    sequenceItem.Header = string.Format(Properties.Resource.SEQUENCE_TEXT_CV, sequenceNumber.ToString(CultureInfo.CurrentCulture), displayId);
                                    unassembledItem.Items.Add(sequenceItem);

                                    // creates the tooltip for the sequence item of the contig
                                    sequenceItem.ToolTip = this.CreateToolTip(string.Empty, sequence, true);
                                    sequenceNumber++;

                                    // creates the context menu for the sequences.
                                    sequenceItem.Tag = sequence;
                                    sequenceItem.ContextMenu = this.CreateContextMenu(sequenceItem);
                                }

                                unassembledItem.Tag = overlapAssembly.UnmergedSequences;
                                treeViewConsensus.Items.Add(unassembledItem);
                            }

                            assembled = true;
                        }
                    }
                }

                if (assembled)
                {
                    this.consensusViewExpander.IsExpanded = true;
                    this.consensusViewExpander.IsEnabled = true;
                    this.sequenceViewExpander.IsExpanded = false;
                    this.blastResultsExpander.IsExpanded = false;

                    this.txtInputSequence.Text = assemblyOutput.NoOfSequence.ToString(CultureInfo.CurrentCulture);
                    this.txtAlignmentAlgorithm.Text = assemblyOutput.AlgorithmNameUsed;
                    this.txtTotalTime.Text = assemblyOutput.EndTime.Subtract(assemblyOutput.StartTime).ToString();
                    this.txtStartTime.Text = assemblyOutput.StartTime.ToString(CultureInfo.CurrentCulture);
                    this.txtEndTime.Text = assemblyOutput.EndTime.ToString(CultureInfo.CurrentCulture);
                    this.numberOfContigsLabel.Text = Properties.Resource.NoOfContigs;
                    this.txtContigs.Text = assemblyOutput.NoOfContigs.ToString(CultureInfo.CurrentCulture);
                    this.txtUnAssembledCountGrid.Visibility = System.Windows.Visibility.Visible;
                    this.txtLengthGrid.Visibility = System.Windows.Visibility.Visible;
                    this.txtUnAssembled.Text = assemblyOutput.NoOfUnassembledSequence.ToString(CultureInfo.CurrentCulture);
                    this.txtLength.Text = assemblyOutput.TotalLength.ToString(CultureInfo.CurrentCulture);
                }
            }
        }

        /// <summary> 
        /// This method will build alignment view UI.
        /// </summary>               
        /// <param name="alignmentOutput">Contains output of the alignment process.</param>        
        public void BuildAlignmentView(AlignmentOutput alignmentOutput)
        {
            this.assembleProgress.Visibility = Visibility.Collapsed;
            this.assemblePane.Visibility = Visibility.Visible;

            treeViewConsensus.Header = Properties.Resource.Alignments;
            consensusTreeViewCaption.Text = Properties.Resource.AlignmentsTreeView;
            consensusViewExpander.Header = Properties.Resource.Alignments;

            if (alignmentOutput != null)
            {
                this.ClearOlderConsensus();
                this.consensusCustomView.ClearTopView();
                this.alignerOrAssemblyResultPresent = true;
                this.stkAssemblyReport.Visibility = Visibility.Visible;

                // checks whether the alignment was succesful or not
                if (alignmentOutput.AlignerResult != null)
                {
                    // create the treeview
                    AddAlignmentsToTree(alignmentOutput.AlignerResult);
                }

                this.consensusViewExpander.IsExpanded = true;
                this.consensusViewExpander.IsEnabled = true;
                this.sequenceViewExpander.IsExpanded = false;
                this.blastResultsExpander.IsExpanded = false;

                this.txtInputSequence.Text = alignmentOutput.InputSequenceCount.ToString(CultureInfo.CurrentCulture);
                this.txtAlignmentAlgorithm.Text = alignmentOutput.AlignerName;
                this.txtTotalTime.Text = alignmentOutput.EndTime.Subtract(alignmentOutput.StartTime).ToString();
                this.txtStartTime.Text = alignmentOutput.StartTime.ToString(CultureInfo.CurrentCulture);
                this.txtEndTime.Text = alignmentOutput.EndTime.ToString(CultureInfo.CurrentCulture);
                this.numberOfContigsLabel.Text = Properties.Resource.NoOfAlignments;
                this.txtContigs.Text = alignmentOutput.AlignerResult.Sum(n => n.AlignedSequences.Count).ToString(CultureInfo.CurrentCulture);
                this.txtUnAssembledCountGrid.Visibility = System.Windows.Visibility.Collapsed;
                this.txtLengthGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Adds the given list of ISequenceAlignments to the left side tree view
        /// </summary>
        /// <param name="alignments">List of Alignments to add</param>
        private void AddAlignmentsToTree(IList<ISequenceAlignment> alignments)
        {
            treeViewConsensus.Items.Clear();
            int currentAlignmentNumber = 1;

            foreach (ISequenceAlignment currentAlignment in alignments)
            {
                foreach (IAlignedSequence alignedSequence in currentAlignment.AlignedSequences)
                {
                    TreeViewItem alignmentItem = new TreeViewItem();

                    alignmentItem.ItemContainerStyle = this.FindResource("ConsensusTreeViewItem") as Style;
                    alignmentItem.Header = Properties.Resource.Alignment + " " + currentAlignmentNumber.ToString(CultureInfo.CurrentCulture);
                    alignmentItem.Tag = alignedSequence;

                    int sequenceTotalCount = alignedSequence.Sequences.Count;

                    for (int i = 0; i < sequenceTotalCount; i++)
                    {
                        ISequence currentSequence = alignedSequence.Sequences[i];

                        TreeViewItem sequenceItem = new TreeViewItem();

                        if (!string.IsNullOrWhiteSpace(currentSequence.ID))
                        {
                            sequenceItem.Header = currentSequence.ID;
                        }
                        else
                        {
                            sequenceItem.Header = string.Format(Properties.Resource.SEQUENCE_TEXT_SV + " " + (i + 1).ToString(CultureInfo.CurrentCulture));
                        }

                        sequenceItem.Tag = currentSequence;
                        alignmentItem.Items.Add(sequenceItem);

                        // creates the tooltip for the alignment
                        sequenceItem.ToolTip = this.CreateToolTip(string.Empty, currentSequence, true);

                        // creates the context menu for the sequences.
                        sequenceItem.ContextMenu = this.CreateContextMenu(sequenceItem);
                    }

                    treeViewConsensus.Items.Add(alignmentItem);
                    currentAlignmentNumber++;
                }
            }

            if (treeViewConsensus.Items.Count > 0)
            {
                (treeViewConsensus.Items[0] as TreeViewItem).IsSelected = true;
            }
        }

        /// <summary>
        /// Adds the given list of contigs to the left side tree view
        /// </summary>
        /// <param name="contigs">List of contigs to add</param>
        /// <returns>Number of sequences</returns>
        private int AddContigsToTree(IEnumerable<Contig> contigs)
        {
            int currentContigNumber = 1;
            int sequenceNumber = 1;
            DateTime startTime = DateTime.Now;

            foreach (Contig cont in contigs)
            {
                TreeViewItem contigItem = new TreeViewItem();
                contigItem.ItemContainerStyle = this.FindResource("ConsensusTreeViewItem") as Style;
                contigItem.Header = Properties.Resource.CONTIG_TEXT + currentContigNumber.ToString(CultureInfo.CurrentCulture);

                // creates the context menu for the contig 
                ContextMenu menu = new ContextMenu();
                MenuItem saveContig = new MenuItem();
                saveContig.Header = Properties.Resource.SAVE_TEXT;
                menu.Items.Add(saveContig);
                saveContig.Tag = cont;
                saveContig.Click += new RoutedEventHandler(this.OnSaveContigClicked);
                contigItem.ContextMenu = menu;

                foreach (Contig.AssembledSequence sequence in cont.Sequences.OrderBy(s => s.Position))
                {
                    ISequence assembledSequence = sequence.Sequence;
                    TreeViewItem sequenceItem = new TreeViewItem();
                    string displayId = string.Empty;

                    if (!string.IsNullOrEmpty(assembledSequence.ID))
                    {
                        displayId = assembledSequence.ID;
                    }

                    sequenceItem.Header = string.Format(Properties.Resource.SEQUENCE_TEXT_CV, sequenceNumber.ToString(CultureInfo.CurrentCulture), displayId);
                    sequenceItem.Tag = assembledSequence;
                    contigItem.Items.Add(sequenceItem);

                    // creates the tooltip for the sequence item of the contig
                    sequenceItem.ToolTip = this.CreateToolTip(string.Empty, assembledSequence, true);
                    sequenceNumber++;

                    // creates the context menu for the sequences.
                    sequenceItem.ContextMenu = this.CreateContextMenu(sequenceItem);

                    // Throw an exception if this runs for more than 50 seconds.
                    if ((DateTime.Now - startTime).Seconds > 50)
                    {
                        throw new TimeoutException();
                    }
                }

                contigItem.Tag = cont;
                treeViewConsensus.Items.Add(contigItem);

                // Selects the first Contig.
                if (treeViewConsensus.Items.Count == 1)
                {
                    contigItem.IsSelected = true;
                }

                currentContigNumber++;
            }

            return sequenceNumber;
        }

        /// <summary>
        /// This method will be called when there is a
        /// error during assembly process. This gives a chance
        /// to IAssembly pane to cleanup.
        /// </summary>
        public void Cleanup()
        {
            // Hide the animation.
            this.assembleProgress.Visibility = Visibility.Collapsed;

            // Display assemble button again.
            this.assemblePane.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// This method would save Sequences to the file depending on what is selected in the treeview
        /// </summary>
        public void SaveWorkspace()
        {
            if (this.selectedSequences.Count > 0)
            {
                this.SaveSequences(this.selectedSequences);
            }
        }

        /// <summary>
        /// Save the given collection of sequences
        /// </summary>
        /// <param name="sequences">collection of sequences to save</param>
        private void SaveSequences(ICollection<ISequence> sequences)
        {
            System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();
            FileDialogFilterBuilder filter = this.CreateFormatFilter();
            filter.AddAllFileTypes(Resource.ALL_FILE_FORMAT);

            // Set the Filter for the File dialog.
            saveDialog.Filter = filter.ToFilterString();

            // Set "FastA" as Default Parameter
            saveDialog.FilterIndex = 0;

            if (System.Windows.Forms.DialogResult.OK == saveDialog.ShowDialog())
            {
                string filePath = saveDialog.FileName;
                SaveSequenceInputEventArgs saveArgs = new SaveSequenceInputEventArgs(sequences, filePath);
                if (this.SaveSequence != null)
                {
                    this.SaveSequence.Invoke(this, saveArgs);
                }
            }
        }

        #endregion

        #region -- Private Methods --

        /// <summary>
        /// This event fires on load of Custom view. It would plot 
        /// the sequences on the top view of the custom view 
        /// </summary>
        /// <param name="sender">Consensus custom view</param>
        /// <param name="e">Event Data</param>
        private void OnCustomViewLoaded(object sender, RoutedEventArgs e)
        {
            if (this.customViewTab.IsSelected && this.contigChanged == true)
            {
                if (this.consensusCustomView.SelectedContig != null)
                {
                    this.consensusCustomView.ClearTopView();
                    this.consensusCustomView.PlotTopView();
                    this.contigChanged = false;
                }
            }
        }

        /// <summary>
        /// This event is fired on selecting an Item on the consensus tree view.
        /// </summary>
        /// <param name="sender">Consensus Tree View</param>
        /// <param name="e">Event Data</param>
        private void OnConsensusTreeSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem item;
            if (sender is TreeViewItem)
            {
                item = sender as TreeViewItem;
            }
            else
            {
                item = (sender as TreeView).SelectedItem as TreeViewItem;
            }
            this.selectedTreeViewItem = item;

            // Depending upon the selection of the sequence the 
            // selected sequence has to be updated and enable/disable 
            // the execute search button
            if (item.Tag is ISequence)
            {
                this.selectedSequence = item.Tag as ISequence;
                this.executeServiceBtn.IsEnabled = true;
                consensusCustomView.HighlightedSequence = this.selectedSequence;
            }
            else if (item.Tag is Contig)
            {
                this.selectedSequence = (item.Tag as Contig).Consensus;
                this.executeServiceBtn.IsEnabled = true;
                consensusCustomView.HighlightedSequence = null;
            }
            else if (item.Tag is IAlignedSequence)
            {
                this.executeServiceBtn.IsEnabled = false;
                consensusCustomView.HighlightedSequence = null;
            }
            else
            {
                this.executeServiceBtn.IsEnabled = false;
                consensusCustomView.HighlightedSequence = null;
            }

            // Whether the the item is contig or not the custom view is displayed
            if (item.Tag is Contig)
            {
                if (this.consensusCustomView.SelectedContig != item.Tag as Contig)
                {
                    this.consensusCustomView.SelectedContig = item.Tag as Contig;
                    this.contigChanged = true;
                    this.consensusCustomView.ClearTopView();
                    if (this.customViewTab.IsSelected && this.consensusCustomView.IsLoaded)
                    {
                        this.consensusCustomView.PlotTopView();
                    }
                }
            }
            else if (item.Tag != null && (item.Parent as TreeViewItem).Tag is Contig)
            {
                if (this.consensusCustomView.SelectedContig != (item.Parent as TreeViewItem).Tag as Contig)
                {
                    this.consensusCustomView.SelectedContig = (item.Parent as TreeViewItem).Tag as Contig;
                    this.contigChanged = true;
                    this.consensusCustomView.ClearTopView();
                    if (this.customViewTab.IsSelected && this.consensusCustomView.IsLoaded)
                    {
                        this.consensusCustomView.PlotTopView();
                    }
                }
            }
            else if (item.Tag is IAlignedSequence)
            {
                if (this.consensusCustomView.SelectedAlignment != item.Tag as IAlignedSequence)
                {
                    this.consensusCustomView.SelectedAlignment = item.Tag as IAlignedSequence;
                    this.contigChanged = true;
                    this.consensusCustomView.ClearTopView();
                    if (this.customViewTab.IsSelected && this.consensusCustomView.IsLoaded)
                    {
                        this.consensusCustomView.PlotTopView();
                    }
                }
            }
            else if (item.Tag != null && ((item.Parent as TreeViewItem).Tag is IAlignedSequence))
            {
                if (this.consensusCustomView.SelectedAlignment != (item.Parent as TreeViewItem).Tag as IAlignedSequence)
                {
                    this.consensusCustomView.SelectedAlignment = (item.Parent as TreeViewItem).Tag as IAlignedSequence;
                    this.contigChanged = true;
                    this.consensusCustomView.ClearTopView();
                    if (this.customViewTab.IsSelected && this.consensusCustomView.IsLoaded)
                    {
                        this.consensusCustomView.PlotTopView();
                    }
                }
            }
            else if (item.Tag is IList<ISequence>)
            {
                if (this.consensusCustomView.SelectedSequences != item.Tag as IList<ISequence>)
                {
                    this.consensusCustomView.SelectedSequences = item.Tag as IList<ISequence>;
                    this.contigChanged = true;
                    this.consensusCustomView.ClearTopView();
                    if (this.customViewTab.IsSelected && this.consensusCustomView.IsLoaded)
                    {
                        this.consensusCustomView.PlotTopView();
                    }
                }
            }
            else if (item.Tag != null && (item.Parent as TreeViewItem).Tag is IList<ISequence>)
            {
                if (this.consensusCustomView.SelectedSequences != (item.Parent as TreeViewItem).Tag as IList<ISequence>)
                {
                    this.consensusCustomView.SelectedSequences = (item.Parent as TreeViewItem).Tag as IList<ISequence>;
                    this.contigChanged = true;
                    this.consensusCustomView.ClearTopView();
                    if (this.customViewTab.IsSelected && this.consensusCustomView.IsLoaded)
                    {
                        this.consensusCustomView.PlotTopView();
                    }
                }
            }
            else if (item.Tag == null)
            {
                this.consensusCustomView.SelectedContig = null;
                this.contigChanged = true;
                this.consensusCustomView.ClearTopView();
            }

            if (this.consensusCustomView.SelectedSequences != null || this.consensusCustomView.SelectedContig != null || this.consensusCustomView.SelectedAlignment != null)
            {
                this.consensusViewZoomPanel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.consensusViewZoomPanel.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        /// <summary>
        /// This method sets Tab index for the children controls.
        /// </summary>
        private void SetTabIndex()
        {
            this.sequenceViewExpander.TabIndex = 0;
            this.btnAssemble.TabIndex = 1;
            this.btnAlign.TabIndex = 2;
            this.consensusViewExpander.TabIndex = 3;
            this.treeViewConsensus.TabIndex = 4;
            this.assemblyResultTab.TabIndex = 5;
            this.blastResultsExpander.TabIndex = 6;
        }

        /// <summary>
        /// This method would create context menu for the Sequence Tree view.
        /// </summary>
        /// <param name="item">Tree view item for which the context menu is being created</param>
        /// <returns>Context menu for the item</returns>
        private ContextMenu CreateSequenceContextMenu(TreeViewItem item)
        {
            ContextMenu sequenceMenu = new ContextMenu();

            // The unload option
            MenuItem unloadSequence = new MenuItem();
            unloadSequence.Header = Resource.UNLOAD_TEXT;
            unloadSequence.Tag = item;
            sequenceMenu.Items.Add(unloadSequence);
            unloadSequence.Click += new RoutedEventHandler(this.OnUnloadSequence);

            MenuItem saveSequences = new MenuItem();
            saveSequences.Header = Resource.SAVE_TEXT;
            saveSequences.Tag = item;
            sequenceMenu.Items.Add(saveSequences);
            saveSequences.Click += new RoutedEventHandler(OnSaveSequencesClick);

            return sequenceMenu;
        }

        /// <summary>
        /// This event is fired when a file or sequence is 
        /// unloaded from the Sequence Treeview
        /// </summary>
        /// <param name="sender">Menu Item Instance</param>
        /// <param name="e">Event Data</param>
        private void OnUnloadSequence(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            TreeViewItem item = menuItem.Tag as TreeViewItem;

            UnloadSequence(item);
        }

        /// <summary>
        /// Unloads the sequence associated with the tree view item being passed
        /// </summary>
        /// <param name="item">TreeViewItem associated with the sequence to unload</param>
        private void UnloadSequence(TreeViewItem item)
        {
            if (item != null)
            {
                // if the tree item is a File
                if (item.HasItems)
                {
                    foreach (TreeViewItem childItem in item.Items)
                    {
                        this.RemoveSequenceFromViewer(GetSequenceFromTreeViewItem(childItem));
                    }

                    // Remove the file info from the collection.
                    this.RaiseFileUnLoad(item);
                    (item.Parent as ItemsControl).Items.Remove(item);
                }
                else
                {
                    this.RemoveSequenceTreeItem(item);
                }
            }
        }

        /// <summary>
        /// This method would remove the Sequence from the Sequences tree
        /// </summary>
        /// <param name="item">Tree view item</param>
        private void RemoveSequenceTreeItem(TreeViewItem item)
        {
            ItemsControl parent = item.Parent as ItemsControl;

            this.RemoveSequenceFromViewer(GetSequenceFromTreeViewItem(item));

            parent.Items.Remove(item);

            if (parent.Items.Count == 0 && parent is TreeViewItem)
            {
                this.RaiseFileUnLoad(parent as TreeViewItem);
                (parent.Parent as ItemsControl).Items.Remove(parent);
            }
        }

        /// <summary>
        /// This mehtod would raise an event to inform controller 
        /// that file unload has happened and remove the give info from info collection
        /// </summary>
        /// <param name="item">Tree View Item</param>
        private void RaiseFileUnLoad(TreeViewItem item)
        {
            ParsedFileInfo parsedInfo = item.Tag as ParsedFileInfo;
            if (parsedInfo != null)
            {
                FileInfo info = new FileInfo(parsedInfo.FileName);
                if (info != null)
                {
                    FileUnloadedEventArgs args = new FileUnloadedEventArgs(info);
                    if (this.FileUnloaded != null)
                    {
                        this.FileUnloaded(item, args);
                    }
                }
            }
        }

        /// <summary>
        /// This method would remove the unloaded selected sequence   
        /// from the viewer.
        /// </summary>
        /// <param name="item">Tree view item</param>
        private void RemoveSequenceFromViewer(ISequence sequence)
        {
            if (this.selectedSequences.Contains(sequence))
            {
                this.selectedSequences.Remove(sequence);

                this.customSequenceView.SelectedSequences = this.SelectedSequences;
                this.customSequenceView.ClearTopView();
                this.customSequenceView.PlotTopView();

                if (this.SelectedSequences.Count != 0)
                {
                    this.sequenceViewZoomPanel.Visibility = Visibility.Visible;
                    this.SaveItemEnabled = true;
                }
                else
                {
                    this.sequenceViewZoomPanel.Visibility = Visibility.Hidden;
                    this.SaveItemEnabled = false;
                }

                // if number of sequences is less that one, diable the assemble button.
                if (this.SelectedSequences.Count <= 1)
                {
                    this.btnAssemble.IsEnabled = false;
                    this.btnAlign.IsEnabled = false;
                }

                if (this.UpdateSaveItemStatus != null)
                {
                    this.UpdateSaveItemStatus(null, null);
                }
            }
        }

        /// <summary>
        /// This method would create the custom tooltip for the treeview 
        /// item(consensus and sequence tree view).
        /// </summary>
        /// <param name="fileName">the Filename associated with the sequence</param>
        /// <param name="sequence">the sequence associated with the tooltip</param>
        /// <param name="consensusView">
        /// bool representing the whether the tooltip is 
        /// for sequence or consensus tree view
        /// </param>
        /// <returns>The custom ToolTip</returns>
        private ToolTip CreateToolTip(string fileName, ISequence sequence, bool consensusView)
        {
            // initiate the custom tooltip control
            TreeViewTooltip toolTipControl = new TreeViewTooltip();
            ToolTip toolTip = new ToolTip();

            // assign the associated values of the sequence
            toolTipControl.DisplayId = sequence.ID;
            toolTipControl.Length = sequence.Count;
            toolTipControl.AlphabetType = sequence.Alphabet;
            string description = string.Empty;

            // if the chosen file is of genbank type 
            // then extract the description
            if (sequence.Metadata.Keys.Contains("GenBank"))
            {
                GenBankMetadata metadata = sequence.Metadata["GenBank"] as GenBankMetadata;
                if (metadata != null)
                {
                    description = metadata.Definition;
                }
            }
            else
            {
                if (sequence.Metadata.Count > 0 && sequence.Metadata.Keys.Contains("definition") && sequence.Metadata["definition"] != null)
                {
                    description = sequence.Metadata["definition"].ToString();
                }
            }

            toolTipControl.Description = description;

            // add only if consensus view
            if (!consensusView)
            {
                toolTipControl.FileName = fileName;
            }

            toolTip.Style = this.FindResource("TreeItemToolTip") as Style;
            toolTip.Content = toolTipControl;

            return toolTip;
        }

        /// <summary>
        /// Clears the previous consenses view.
        /// </summary>
        private void ClearOlderConsensus()
        {
            this.treeViewConsensus.Items.Clear();

            // Remove Text data from here.
            this.txtInputSequence.Text = string.Empty;
            this.txtAlignmentAlgorithm.Text = string.Empty;
            this.txtStartTime.Text = string.Empty;
            this.txtEndTime.Text = string.Empty;
            this.txtContigs.Text = string.Empty;
            this.txtUnAssembled.Text = string.Empty;
            this.txtLength.Text = string.Empty;
        }

        /// <summary>
        /// This event is fired when the tree item is double clicked,
        /// It would add the selectde sequence to the sequence 
        /// view as first child.
        /// </summary>
        /// <param name="sender">Sequence Tree view item</param>
        /// <param name="e">Mouse event data</param>
        private void OnItemMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                TreeViewItem item = sender as TreeViewItem;

                ISequence sequenceItem = GetSequenceFromTreeViewItem(item);

                if (sequenceItem != null)
                {
                    int itemIndex = this.selectedSequences.IndexOf(sequenceItem);
                    if (itemIndex == -1) // if not already added
                    {
                        AddSequenceToWorkspace(item);
                        RefreshSequencesCustomView();
                    }
                    else
                    {
                        item.Background = Brushes.Transparent;
                        this.selectedSequences.RemoveAt(itemIndex);
                        RefreshSequencesCustomView();
                    }
                }
            }
        }

        /// <summary>
        /// This method would create the context menu for the sequence 
        /// in the consensus tree view
        /// </summary>
        /// <param name="item">Associated TreItem</param>
        /// <returns>The context menu for the sequence</returns>
        private ContextMenu CreateContextMenu(TreeViewItem item)
        {
            ContextMenu sequenceMenu = new ContextMenu();

            // The save option
            MenuItem saveSequence = new MenuItem();
            saveSequence.Header = Resource.SAVE_TEXT;
            saveSequence.Tag = item;
            sequenceMenu.Items.Add(saveSequence);
            saveSequence.Click += new RoutedEventHandler(this.OnSaveContigClicked);

            // The Edit option
            MenuItem ediSequence = new MenuItem();
            ediSequence.Header = Resource.EDIT_TEXT;
            ediSequence.Tag = item;
            sequenceMenu.Items.Add(ediSequence);
            ediSequence.Click += new RoutedEventHandler(this.OnEditSequenceClick);
            return sequenceMenu;
        }

        /// <summary>
        /// This event is fired on click of the edit option 
        /// in the context menu, this would launch the edit dialog, which 
        /// would enable the user to edit the given sequence and save it.
        /// </summary>
        /// <param name="sender">Edit menu item</param>
        /// <param name="e">Event data</param>
        private void OnEditSequenceClick(object sender, RoutedEventArgs e)
        {
            this.editItem = sender as MenuItem;

            TreeViewItem treeItem = this.editItem.Tag as TreeViewItem;
            ISequence sequence = GetSequenceFromTreeViewItem(treeItem);

            this.dialog = new EditSequenceDialog(sequence);
            this.dialog.SaveEditedSequence += new EventHandler<EditSequenceEventArgs>(this.OnDialogSaveEditedSequence);
            this.dialog.CancelEditDialog += new EventHandler(this.OnDialogCancelEditDialog);
            if (sequence != null)
            {
                this.dialog.SequenceString = new string(sequence.Select(a => (char)a).ToArray());
                if (this.PopupOpened != null)
                {
                    this.PopupOpened(sender, e);
                }

                this.dialog.ShowDialog();
            }
        }

        /// <summary>
        /// This event is fired when the cancel button on edit dialog is clicked, 
        /// this would discard all the changes and would close the dialog and 
        /// would inform the controller
        /// </summary>
        /// <param name="sender">Cancel Button</param>
        /// <param name="e">Event Data</param>
        private void OnDialogCancelEditDialog(object sender, EventArgs e)
        {
            if (this.PopupClosed != null)
            {
                this.PopupClosed(sender, e);
            }
        }

        /// <summary>
        /// This event is fired on save button click, It passes the edited sequence 
        /// to the controller and closes the dialog.
        /// </summary>
        /// <param name="sender">Save button</param>
        /// <param name="e">Edit sequence event args</param>
        private void OnDialogSaveEditedSequence(object sender, EditSequenceEventArgs e)
        {
            try
            {
                ISequence sequence = new Sequence(e.Sequence.Alphabet, e.SequenceString);
                this.dialog.Close();
                this.UpdateEditedSeqItem(sequence);
                if (this.PopupClosed != null)
                {
                    this.PopupClosed(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageDialogBox.Show(ex.Message, Properties.Resource.CAPTION, MessageDialogButton.OK);
            }
        }

        /// <summary>
        /// Updates the tree item with the edited sequence.
        /// </summary>
        /// <param name="sequence">Edited sequence</param>
        private void UpdateEditedSeqItem(ISequence sequence)
        {
            TreeViewItem item = this.editItem.Tag as TreeViewItem;
            item.Tag = sequence;
            item.ToolTip = this.CreateToolTip(null, sequence, true);
            item.ContextMenu = this.CreateContextMenu(item);
        }

        /// <summary>
        /// This event will be fired when the user clicks on the assemble button.
        /// Assembler will raise an event to controller asking it to perform
        /// assembly on the selected sequences. The assembler will pass the
        /// selected sequences and selected alignment algorithm.
        /// </summary>
        /// <param name="sender">Assemble button.</param>
        /// <param name="e">Event data</param>
        private void OnAssembleClick(object sender, RoutedEventArgs e)
        {
            if (this.alignerOrAssemblyResultPresent == true)
            {
                if (MessageDialogResult.No == MessageDialogBox.Show(Properties.Resource.REASSEMBLY_WARNING, Properties.Resource.CAPTION, MessageDialogButton.YesNo))
                {
                    return;
                }
            }

            AssemblerDialog dialog = new AssemblerDialog(algorithms, GetDefaultSM(this.selectedSequences[0]), this.selectedSequences);

            if (dialog.Show())
            {
                AssemblyInputEventArgs input = new AssemblyInputEventArgs(this.SelectedSequences, dialog.SelectedAlgo);

                if (dialog.AssemblerSelected == AssemblerType.PaDeNA)
                {
                    dialog.GetPaDeNAInput(input);
                }
                else
                {
                    input.ConsensusThreshold = dialog.ConsensusThreshold;
                    input.MatchScore = dialog.MatchScore;
                    input.MergeThreshold = dialog.MergeThreshold;
                    input.MismatchScore = dialog.MisMatchScore;
                    AlignerInputEventArgs alignerInput = new AlignerInputEventArgs();
                    input.AlignerInput = alignerInput;
                    dialog.GetAlignmentInput(dialog.stkAlingerParam, input, this.SelectedSequences[0].Alphabet);
                }

                if (this.RunAssemblerAlgorithm != null)
                {
                    assemblyInProgressText.Text = Properties.Resource.AssemblingSequences;
                    this.assembleProgress.Visibility = Visibility.Visible;
                    this.assemblePane.Visibility = Visibility.Collapsed;
                    this.RunAssemblerAlgorithm(this, input);
                }
            }
        }

        /// <summary>
        /// Gets a default similarity matrix for assemblying any given sequence
        /// </summary>
        /// <param name="sequence">Sequence used to identify the molecule type and get the SM</param>
        /// <returns>Similarity matrix name</returns>
        private string GetDefaultSM(ISequence sequence)
        {
            if (sequence.Alphabet == Alphabets.DNA)
            {
                return Bio.SimilarityMatrices.SimilarityMatrix.StandardSimilarityMatrix.AmbiguousDna.ToString();
            }
            else if (sequence.Alphabet == Alphabets.RNA)
            {
                return Bio.SimilarityMatrices.SimilarityMatrix.StandardSimilarityMatrix.AmbiguousRna.ToString();
            }
            else if (sequence.Alphabet == Alphabets.Protein)
            {
                return Bio.SimilarityMatrices.SimilarityMatrix.StandardSimilarityMatrix.Blosum50.ToString();
            }
            else
            {
                return Bio.SimilarityMatrices.SimilarityMatrix.StandardSimilarityMatrix.AmbiguousDna.ToString();
            }
        }

        /// <summary>
        /// This event will be fired when the user clicks on the align button.
        /// This will raise an event to controller asking it to perform
        /// alignment on the selected sequences. This will pass the
        /// selected sequences and selected alignment algorithm.
        /// </summary>
        /// <param name="sender">Align button.</param>
        /// <param name="e">Event data</param>
        private void OnAlignClick(object sender, RoutedEventArgs e)
        {
            if (this.alignerOrAssemblyResultPresent == true)
            {
                if (MessageDialogResult.No == MessageDialogBox.Show(Properties.Resource.REASSEMBLY_WARNING, Properties.Resource.CAPTION, MessageDialogButton.YesNo))
                {
                    return;
                }
            }

            AlignerDialog dialog = new AlignerDialog(GetDefaultSM(this.selectedSequences[0]));
            if (dialog.Show())
            {
                AlignerInputEventArgs alignerInput = new AlignerInputEventArgs();
                alignerInput.SelectedAlgorithm = dialog.SelectedAlgo;
                alignerInput.SequencesToAlign = this.SelectedSequences;

                if (dialog.SelectedAlgo == Properties.Resource.PAMSAM)
                {
                    dialog.GetPamsamInput(dialog.stkAlingerParam, alignerInput, this.SelectedSequences[0].Alphabet);
                }
                else
                {
                    dialog.GetAlignmentInput(dialog.stkAlingerParam, alignerInput, this.SelectedSequences[0].Alphabet);
                }

                if (this.RunAlignerAlgorithm != null)
                {
                    assemblyInProgressText.Text = Properties.Resource.AligningSequences;
                    this.assembleProgress.Visibility = Visibility.Visible;
                    this.assemblePane.Visibility = Visibility.Collapsed;
                    this.RunAlignerAlgorithm(this, alignerInput);
                }
            }
        }

        /// <summary>
        /// Extracts the extensions from the supported file types
        /// It creates the array of the extensions without the '.' 
        /// character from the comma separated string
        /// </summary>
        private void ExtractExtensions()
        {
            const int SourceIndex = 1;
            const int DestinationIndex = 0;
            foreach (string type in this.fileTypes)
            {
                //// Split the comma separated values into the array.
                string[] filterTypes = type.Split(',');

                //// Add the Format type header in the headers list.
                this.fileTypeHeaders.Add(filterTypes[0]);
                string[] filters = new string[filterTypes.Length - 1];

                //// remove the Format type header from the array of the extensions 
                Array.Copy(filterTypes, SourceIndex, filters, DestinationIndex, filterTypes.Length - 1);
                int i = 0;

                //// replace the '.' character
                foreach (string filter in filters)
                {
                    filters[i] = filter.Replace(".", string.Empty);
                    i++;
                }

                //// create the extension list.
                this.extensionsList.Add(filters);
            }
        }

        /// <summary>
        /// This event is fired when the user wishes to save
        /// a particular contig or a sequence. If user selects
        /// to save a contig then all the sequences which
        /// are under the contig will be saved.
        /// </summary>
        /// <param name="sender">Menu Item.</param>
        /// <param name="e">Event data.</param>
        private void OnSaveContigClicked(object sender, RoutedEventArgs e)
        {
            Collection<ISequence> sequences = new Collection<ISequence>();
            MenuItem item = e.OriginalSource as MenuItem;
            if (item != null)
            {
                Contig contig = item.Tag as Contig;
                if (contig != null)
                {
                    sequences.Add(contig.Consensus);
                }
                else
                {
                    Sequence seq = (item.Tag as TreeViewItem).Tag as Sequence;
                    if (seq != null)
                    {
                        sequences.Add(seq);
                    }
                }

                this.SaveSequences(sequences);
            }
        }

        /// <summary>
        /// This creates the filter expression for the given extensions
        /// for eg:|description |*.ext1;*.ext2|
        /// </summary>
        /// <returns>the filter string expression for the given exensions</returns>
        private FileDialogFilterBuilder CreateFormatFilter()
        {
            FileDialogFilterBuilder filter = new FileDialogFilterBuilder();

            //// Creates the Filters for the Given File Extensions
            foreach (string[] filters in this.extensionsList)
            {
                FilterInfo fileInfo = new FilterInfo(this.fileTypeHeaders[this.extensionsList.IndexOf(filters)]);
                fileInfo.SetExtensions(filters);
                filter.Info.Add(fileInfo);
            }

            return filter;
        }

        /// <summary>
        /// This event will be fired when the user clicks on the Cancel button.
        /// Assembler will raise an event to controller asking it to cancel the
        /// assembly process on the selected sequences.
        /// </summary>        
        /// <param name="sender">Cancel button</param>
        /// <param name="e">Event Data</param>
        private void OnCancelAssemblyClick(object sender, RoutedEventArgs e)
        {
            this.Cleanup();

            if (this.CancelAssembly != null)
            {
                this.CancelAssembly(sender, e);
            }
        }

        /// <summary>
        /// The method would validate the drop location, whether the dropped 
        /// location is the sequence viewer or not.
        /// </summary>
        /// <param name="location">Mouse pointer location</param>
        /// <returns>bool-whether the drop was successful or not.</returns>
        private bool ValidateDropLocation(Point location)
        {
            HitTestResult hitTestResults = VisualTreeHelper.HitTest(this, location);

            if (hitTestResults.VisualHit is Rectangle || hitTestResults.VisualHit is Border
                || hitTestResults.VisualHit is ScrollViewer || hitTestResults.VisualHit is TextBlock)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The event handler would provide the drag 
        /// effect for the dragging action
        /// </summary>
        /// <param name="sender">Framework Element</param>
        /// <param name="e">Drag event args</param>
        private void OnSequenceTreeItemDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ISequence)))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        /// <summary>
        /// Gets a Isequence instance from a treeview item by first looking at its Tag for a sequence
        /// if not found, look at its parents tag and get the sequence
        /// </summary>
        /// <param name="sourceItem">TreeViewItem from which the sequence has to be extracted</param>
        /// <returns>ISequence if successful, else null</returns>
        private ISequence GetSequenceFromTreeViewItem(TreeViewItem sourceItem)
        {
            if (sourceItem.Tag is ISequence)
            {
                return sourceItem.Tag as ISequence;
            }
            else if (sourceItem.Parent is TreeViewItem && sourceItem.Tag is int && (sourceItem.Parent as TreeViewItem).Tag is ParsedFileInfo)
            {
                return ((sourceItem.Parent as TreeViewItem).Tag as ParsedFileInfo).Sequence[(int)sourceItem.Tag];
            }

            return null;
        }

        /// <summary>
        /// The event handler would handle the drop of the item
        /// and set the IsChecked property of the treeview item being dragged
        /// </summary>
        /// <param name="sender">Framework Element</param>
        /// <param name="e">Drag event args</param>
        private void OnSequenceTreeItemDrop(object sender, DragEventArgs e)
        {
            // check the drop was valid or not.
            if (this.ValidateDropLocation(e.GetPosition(this)))
            {
                TreeViewItem draggedData = e.Data.GetData(typeof(TreeViewItem)) as TreeViewItem;
                ISequence sequenceItem = GetSequenceFromTreeViewItem(draggedData);

                if (sequenceItem != null && draggedData.Parent is TreeViewItem && (draggedData.Parent as TreeViewItem).Tag is ParsedFileInfo)
                {
                    if (this.selectedSequences.Contains(sequenceItem) == false)
                    {
                        AddSequenceToWorkspace(draggedData);
                        RefreshSequencesCustomView();
                    }
                }
            }
        }

        /// <summary>
        /// This would create a sequence item on the viewer.
        /// </summary>
        private void RefreshSequencesCustomView()
        {
            this.customSequenceView.SelectedSequences = this.selectedSequences;

            this.customSequenceView.ClearTopView();
            this.customSequenceView.PlotTopView();

            if (this.SelectedSequences.Count != 0)
            {
                this.sequenceViewZoomPanel.Visibility = Visibility.Visible;
                this.SaveItemEnabled = true;
            }
            else
            {
                this.sequenceViewZoomPanel.Visibility = Visibility.Hidden;
                this.SaveItemEnabled = false;
            }

            if (this.SelectedSequences.Count > 1)
            {
                this.btnAssemble.IsEnabled = true;
                this.btnAlign.IsEnabled = true;
            }
            else
            {
                this.btnAssemble.IsEnabled = false;
                this.btnAlign.IsEnabled = false;
            }

            if (this.UpdateSaveItemStatus != null)
            {
                this.UpdateSaveItemStatus(null, null);
            }
        }

        /// <summary>
        /// This event handler would remove the selcted sequence from 
        /// the UI sequence viewer and the selected sequence collection
        /// </summary>
        /// <param name="sender">Framework Element</param>
        /// <param name="e">Remove Sequence event args</param>
        private void OnSequenceViewerRemoveSequence(object sender, RemoveSequenceEventArgs e)
        {
            RemoveSequenceFromViewer(e.Sequence);

            foreach (TreeViewItem fileItem in sequenceTree.Items)
            {
                foreach (TreeViewItem treeItem in fileItem.Items)
                {
                    ISequence sequence = GetSequenceFromTreeViewItem(treeItem);
                    if (sequence == e.Sequence)
                    {
                        treeItem.Background = Brushes.Transparent;
                        RefreshSequencesCustomView();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// The event handler initiates the dragging if the mouse left button is in 
        /// pressed state and no other element is being dragged. Attaches the drag events to the 
        /// treeview with the selected value.
        /// </summary>
        /// <param name="sender">Framework Element</param>
        /// <param name="e">Mouse event args</param>
        private void OnSequenceTreeItemMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && this.sequenceTree.SelectedValue != null)
            {
                TreeViewItem draggedItemData = this.sequenceTree.SelectedValue as TreeViewItem;
                if (draggedItemData != null)
                {
                    // Initaites the drag and drop.
                    DragDrop.DoDragDrop(this.sequenceTree, draggedItemData, DragDropEffects.Move);
                }
            }
        }

        /// <summary>
        /// This event is fired on cancelling search service.
        /// </summary>
        /// <param name="sender">Cancel button</param>
        /// <param name="e">Event data</param>
        private void OnCancelServiceBtnClick(object sender, RoutedEventArgs e)
        {
            if (this.CancelSearch != null)
            {
                string webserviceName = this.cmbWebServices.SelectedItem as string;
                this.CancelSearch(sender, new WebServiceInputEventArgs(null, webserviceName, null));
            }
        }

        /// <summary>
        /// This event is fired on click of execute service button,
        /// It would load the pop up with the available service.
        /// </summary>
        /// <param name="sender">Execute button</param>
        /// <param name="e">Event data</param>
        private void OnExecuteServiceBtnClick(object sender, RoutedEventArgs e)
        {
            string webserviceName = this.cmbWebServices.SelectedItem as string;
            BlastDialog dialog = new BlastDialog(webserviceName);
            dialog.Owner = Application.Current.MainWindow;
            dialog.ClosePopup += new EventHandler(this.OnBlastdialogClosePopup);
            dialog.ExecuteSearch += new EventHandler<WebServiceInputEventArgs>(this.OnDialogExecuteSearch);
            if (this.PopupOpened != null)
            {
                this.PopupOpened(sender, e);
            }

            lastBlastDialog = dialog;
            dialog.ShowDialog();
        }

        /// <summary>
        /// This event would be fired on submitting the parameters 
        /// from the blast pane. 
        /// </summary>
        /// <param name="sender">The Blast pane submit button</param>
        /// <param name="e">Web service input arguments</param>
        private void OnDialogExecuteSearch(object sender, WebServiceInputEventArgs e)
        {
            if (this.ExecuteSearchStarted != null)
            {
                this.stkWebService.Visibility = Visibility.Collapsed;
                this.stkWebProgressBar.Visibility = Visibility.Visible;

                string webserviceName = this.cmbWebServices.SelectedItem as string;

                (sender as BlastDialog).Hide();

                this.ExecuteSearchStarted.Invoke(
                        this,
                        new WebServiceInputEventArgs(e.ServiceParameters, webserviceName, e.Configuration));
            }
        }

        /// <summary>
        /// This event is fired on cancelling the parameters 
        /// from the blast pane
        /// </summary>
        /// <param name="sender">the blast pane cancel button</param>
        /// <param name="e">Event Data</param>
        private void OnBlastdialogClosePopup(object sender, EventArgs e)
        {
            if (this.PopupClosed != null)
            {
                this.PopupClosed(sender, e);
            }

            (sender as BlastDialog).Close();
        }

        /// <summary>
        /// Controls the zoom factor of the sequences view
        /// </summary>
        /// <param name="sender">Zoom slider control</param>
        /// <param name="e">Property changed event args</param>
        private void OnSequenceViewZoomSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            customSequenceView.SequenceViewZoomFactor = (int)e.NewValue;
        }

        /// <summary>
        /// Controls the zoom factor of the consensus view
        /// </summary>
        /// <param name="sender">Zoom slider control</param>
        /// <param name="e">Property changed event args</param>
        private void OnConsensusViewZoomSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            consensusCustomView.SequenceViewZoomFactor = (int)e.NewValue;
        }

        /// <summary>
        /// Fired when the selected item in sequence tree changes
        /// </summary>
        private void OnSequenceTreeSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.selectedTreeViewItem = sender as TreeViewItem;
            if (this.selectedTreeViewItem == null)
            {
                this.selectedTreeViewItem = e.NewValue as TreeViewItem;
            }

            if (this.selectedTreeViewItem != null)
            {
                this.selectedSequence = GetSequenceFromTreeViewItem(this.selectedTreeViewItem);
                customSequenceView.HighlightedSequence = this.selectedSequence;
            }
        }

        /// <summary>
        /// Fired when sequence tree gets the focus
        /// </summary>
        private void sequenceTree_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.sequenceTree.SelectedItem != null)
            {
                this.OnSequenceTreeSelectionChanged(this.sequenceTree.SelectedItem, null);
            }
        }

        /// <summary>
        /// Fired when consensus tree gets the focus
        /// </summary>
        private void consensusTree_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.consensusTree.SelectedItem != null)
            {
                this.OnConsensusTreeSelectedItemChanged(this.consensusTree.SelectedItem, null);
            }
        }

        /// <summary>
        /// Fired when mouse wheel is rotated in consensus view.
        /// </summary>
        private void OnConsensusCustomView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (this.consensusViewZoomSlider.Maximum > this.consensusViewZoomSlider.Value)
                {
                    this.consensusViewZoomSlider.Value++;
                }
            }
            else
            {
                if (this.consensusViewZoomSlider.Minimum < this.consensusViewZoomSlider.Value)
                {
                    this.consensusViewZoomSlider.Value--;
                }
            }
        }

        /// <summary>
        /// Fired when mouse wheel is rotated in sequence view.
        /// </summary>
        private void OnCustomSequenceView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (this.sequenceViewZoomSlider.Maximum > this.sequenceViewZoomSlider.Value)
                {
                    this.sequenceViewZoomSlider.Value++;
                }
            }
            else
            {
                if (this.sequenceViewZoomSlider.Minimum < this.sequenceViewZoomSlider.Value)
                {
                    this.sequenceViewZoomSlider.Value--;
                }
            }
        }

        #endregion
    }
}
