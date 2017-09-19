namespace BiodexExcel.Visualizations.Common
{
    using System.Collections.Generic;
    using Bio;

    /// <summary>
    /// Holds the data about SequenceRangeGrouping
    /// </summary>
    public class GroupData
    {
        /// <summary>
        /// Metadata for SequenceRangeGroup
        /// </summary>
        private Dictionary<string, Dictionary<ISequenceRange, string>> _metadata = null;

        /// <summary>
        /// Initializes a new instance of the GroupData class
        /// </summary>
        /// <param name="group">SequenceRangeGroup object</param>
        /// <param name="name">Name of SequenceRangeGroup</param>
        /// <param name="metadata">Metadata for SequenceRangeGroup</param>
        public GroupData(
                SequenceRangeGrouping group,
                string name,
                Dictionary<string, Dictionary<ISequenceRange, string>> metadata)
        {
            Group = group;
            Name = name;
            _metadata = metadata;
        }

        /// <summary>
        /// Gets or sets SequenceRangeGroup
        /// </summary>
        public SequenceRangeGrouping Group { get; set; }

        /// <summary>
        /// Gets or sets the name of SequenceRangeGroup
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the Metadata for SequenceRangeGroup
        /// Key: Sheet name
        /// Value:
        ///  Key: SequenceRange
        ///  Value: Address
        /// </summary>
        public Dictionary<string, Dictionary<ISequenceRange, string>> Metadata
        {
            get { return _metadata; }
        }
    }
}
