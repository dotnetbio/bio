namespace Bio.IO.SAM
{
    /// <summary>
    /// Holds the reference sequence information.
    /// </summary>
    public class ReferenceSequenceInfo
    {
        /// <summary>
        /// Gets or sets the name of the reference sequence.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the length of the reference sequence.
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// Initializes a new instance of ReferenceSequenceInfo class.
        /// </summary>
        public ReferenceSequenceInfo() { }

        /// <summary>
        /// Initializes a new instance of ReferenceSequenceInfo class with specified name and length.
        /// </summary>
        public ReferenceSequenceInfo(string name, long length)
        {
            this.Name = name;
            this.Length = length;
        }
    }
}
