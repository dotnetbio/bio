namespace Bio.IO.GenBank
{
    /// <summary>
    /// Holds start and end position of a feature in a sequence.
    /// For example: 
    /// If location of a feature is "join(1..100,J00194.1:100..202)"
    /// then we need to two LocationRange instance to hold this location.
    /// First LocationRange will be
    ///       Accession     - empty
    ///       StartPosition -1
    ///       EndPosition   - 100
    /// Second LocationRange will be
    ///       Accession     - J00194.1
    ///       StartPoistion - 100
    ///       EndPosition   1 200
    ///       
    /// Note that the GenBank feature location can be parsed using static method "GetLocationRanges" in GenBankMetadata class.
    /// For example:
    ///  GenBankMetadata.GetLocationRanges("join(1..100,J00194.1:100..202)") this will return list of LocationRanges.
    /// </summary>
    public class LocationRange
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LocationRange()
        {
            Accession = string.Empty;
        }

        /// <summary>
        /// Creates a new LocationRange instance from the specified start and end position.
        /// </summary>
        /// <param name="startPosition">Start position of the feature.</param>
        /// <param name="endPosition">End position of the feature.</param>
        public LocationRange(int startPosition, int endPosition)
            : this(string.Empty, startPosition, endPosition) { }

        /// <summary>
        /// Creates a new LocationRange instance from the specified accession, start and end position.
        /// </summary>
        /// <param name="accession">Accession of the sequence.</param>
        /// <param name="startPosition">Start position.</param>
        /// <param name="endPosition">End position.</param>
        public LocationRange(string accession, int startPosition, int endPosition)
        {
            Accession = accession;
            StartPosition = startPosition;
            EndPosition = endPosition;
        }

        /// <summary>
        /// Gets or sets the Accession.
        /// </summary>
        public string Accession { get; set; }

        /// <summary>
        /// Gets or sets the start position.
        /// </summary>
        public int StartPosition { get; set; }

        /// <summary>
        /// Gets or sets the end position.
        /// </summary>
        public int EndPosition { get; set; }
    }
}
