namespace BiodexExcel.Visualizations.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using Bio;

    /// <summary>
    /// Event Argument for SequenceRangeGroup Input Selection
    /// </summary>
    public class InputSequenceRangeSelectionEventArg
    {
        /// <summary>
        /// Address data of each SequenceRangeGrouping
        /// </summary>
        private Dictionary<SequenceRangeGrouping, GroupData> _data = null;

        /// <summary>
        /// List of all parsed sequences
        /// </summary>
        private List<SequenceRangeGrouping> _allSequences = null;

        /// <summary>
        /// Parameter to the Event
        /// </summary>
        private Dictionary<string, object> _parameter = null;

        /// <summary>
        /// Callback argument to the event
        /// </summary>
        private object[] _argsForCallback = null;

        /// <summary>
        /// Initializes a new instance of the InputSequenceRangeSelectionEventArg class
        /// </summary>
        /// <param name="data">Data of event</param>
        /// <param name="sequenceList">List of all parsed sequences</param>
        /// <param name="parameter">Paramters of event</param>
        /// <param name="argsForCallback">Callback arguments of event</param>
        public InputSequenceRangeSelectionEventArg(
                Dictionary<SequenceRangeGrouping, GroupData> data,
                List<SequenceRangeGrouping> sequenceList,
                Dictionary<string, object> parameter,
                object[] argsForCallback)
        {
            _data = data;
            _allSequences = sequenceList;
            _parameter = parameter;
            _argsForCallback = argsForCallback;
        }

        /// <summary>
        /// Gets the list of SequenceRangeGrouping
        /// </summary>
        public IList<SequenceRangeGrouping> Sequences
        {
            get { return _allSequences; }
        }

        /// <summary>
        /// Gets address data of each SequenceRangeGrouping
        /// </summary>
        public Dictionary<SequenceRangeGrouping, GroupData> Data
        {
            get { return _data; }
        }

        /// <summary>
        /// Gets the list of Parameters
        /// </summary>
        public Dictionary<string, object> Parameter
        {
            get { return _parameter; }
        }

        /// <summary>
        /// Gets the Callback arguments
        /// </summary>
        public object[] ArgsForCallback
        {
            get { return _argsForCallback; }
        }
    }
}
