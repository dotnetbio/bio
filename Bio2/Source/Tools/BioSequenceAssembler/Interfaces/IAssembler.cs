namespace SequenceAssembler
{
    #region -- Using Directive --

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Bio;
    using Bio.Algorithms.Assembly;
    using Bio.Algorithms.Alignment;

    #endregion -- Using Directive --

    /// <summary>
    /// IAssembler interface will be implemented by the component
    /// which will display sequence and consensus views.
    /// </summary>
    public interface IAssembler
    {        
        /// <summary>
        /// This event is fired when user wants to assemble the sequences.
        /// This event will be raised by IAssembler. The controller class
        /// instantiates algorithm implementation to perform assembly.
        /// </summary>
        event EventHandler<AssemblyInputEventArgs> RunAssemblerAlgorithm;

        /// <summary>
        /// This event is fired when user wants to align the sequences.
        /// This event will be raised by IAssembler. The controller class
        /// instantiates algorithm implementation to perform alignment.
        /// </summary>
        event EventHandler<AlignerInputEventArgs> RunAlignerAlgorithm;

        /// <summary>
        /// This event is fired when user wants to search for relevant
        /// sequences in NCBI database, using NCBI blast webservice.
        /// This event will be raised by IAssembler. The controller class
        /// instantiates the actual webservice implementation to perform search.
        /// </summary>
        event EventHandler<WebServiceInputEventArgs> ExecuteSearchStarted;

        /// <summary>
        /// This event is fired when user cancels the search for relevant
        /// sequences in NCBI database, using NCBI blast webservice.
        /// This event will be raised by IAssembler. The controller class
        /// cancels the actual webservice implementation search. 
        /// </summary>
        event EventHandler<WebServiceInputEventArgs> CancelSearch;

        /// <summary>
        /// This event is fired when user wants to save the sequences.
        /// This event will be raised by IAssembler. The controller class
        /// instantiates fromatters to save the sequence.
        /// </summary>
        event EventHandler<SaveSequenceInputEventArgs> SaveSequence;

        /// <summary>
        /// This event is fired when the import process of file is completed,
        /// This event will be fired by IAssembler. The controller class would do the 
        /// associated action. 
        /// </summary>
        event EventHandler ImportCompleted;

        /// <summary>
        /// This event is fired when user wants to cancel the assemble process of 
        /// the sequences.This event will be raised by IAssembler.
        /// </summary>
        event EventHandler CancelAssembly;

        /// <summary>
        /// Event to Show Dialog which would bring 
        /// the Graying effect to the Dialog
        /// </summary>
        event EventHandler PopupOpened;

        /// <summary>
        /// Event to close the Dialog which would close 
        /// the Gray effect for the dialog.
        /// </summary>
        event EventHandler PopupClosed;

        /// <summary>
        /// Event to remove the fileinfo from the file infos collection
        /// </summary>
        event EventHandler<FileUnloadedEventArgs> FileUnloaded;

        /// <summary>
        /// Event to inform the controller about the Save Item to be 
        /// enabled or disabled
        /// </summary>
        event EventHandler UpdateSaveItemStatus;

        /// <summary>
        /// Gets the selected consensus sequence
        /// </summary>
        ISequence SelectedSequence
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the 
        /// item should be enabled or not.
        /// </summary>
        bool SaveItemEnabled
        {
            get;         
        }

        /// <summary>
        /// This method will build the sequence tree view.
        /// </summary>
        /// <param name="sequences">List of ISequences to build the tree view.</param>
        void DisplaySequenceTreeView(IEnumerable<ParsedFileInfo> sequences);

        /// <summary>
        /// This method will display the list of avaliable alignment
        /// algorithms to the user.
        /// </summary>
        /// <param name="algorithms">List of alignment algorithms.</param>
        void InitializeAlignmentAlgorithms(IEnumerable<string> algorithms);

        /// <summary>
        /// This method will display the consensus view.
        /// </summary>
        /// <param name="assemblyOutput">Assembly output instance.</param>
        void BuildConsensusView(AssemblyOutput assemblyOutput);

        /// <summary>
        /// This method will display the alignment view.
        /// </summary>
        /// <param name="alignmentOutput">List of ISeuqenceAlignment objects</param>
        void BuildAlignmentView(AlignmentOutput alignmentOutput);

        /// <summary>
        /// Initializes the supported files types, which would be used to 
        /// create filters for the save dialog.
        /// </summary>
        /// <param name="types">Supported File Types</param>
        void InitializeSupportedFileTypes(Collection<string> types);

        /// <summary>
        /// This method will be called when there is a
        /// error during assembly process. This gives a chance
        /// to IAssembly pane to cleanup.
        /// </summary>
        void Cleanup();

        /// <summary>
        /// This method will be called by the controller, on which the IAssembler
        /// will populate its drop-down with given Web services names. These 
        /// web service names are retrived by the controller from Framework abstraction.
        /// </summary>
        /// <param name="webServiceNames">List of supported web-service names.</param>
        void InitializeSupportedWebServices(IEnumerable<string> webServiceNames);

        /// <summary>
        /// This method would remove the loading animation on completetion of blast search
        /// </summary>
        /// <param name="success">Completed successfully or not</param>
        void SearchCompleted(bool success);

        /// <summary>
        /// This method would save the Sequence to the file
        /// </summary>
        void SaveWorkspace();
    }
}
