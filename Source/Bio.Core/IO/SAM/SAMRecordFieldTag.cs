namespace Bio.IO.SAM
{
    /// <summary>
    /// This class holds the tag in the header lines.
    /// For example, consider the following header line.
    /// @HD	VN:1.0
    /// In this example VN:1.0 is the SAMRecordFieldTag.
    /// Where VN is stored in Tag property and 1.0 is stored 
    /// in the value property of this class.
    /// </summary>
    public class SAMRecordFieldTag
    {
        /// <summary>
        /// Creates new SAMRecordFieldTag instance.
        /// </summary>
        /// <param name="tag">Record field tag.</param>
        /// <param name="value">Record field value.</param>
        public SAMRecordFieldTag(string tag, string value)
        {
            Tag = tag;
            Value = value;
        }

        /// <summary>
        /// Record field tag.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Record field tag.
        /// </summary>
        public string Value { get; set; }
    }
}
