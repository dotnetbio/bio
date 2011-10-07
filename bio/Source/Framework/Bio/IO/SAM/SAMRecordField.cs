using System;
using System.Collections.Generic;

namespace Bio.IO.SAM
{
    /// <summary>
    /// This class holds SAM record fields.
    /// Record fields are present in the SAM header.
    /// This class can hold one header line of the SAM header.
    /// For example, consider the following header line.
    /// @SQ	SN:chr20	LN:62435964
    /// In this example SQ is the Type code.
    /// SN:chr20  and LN:62435964 are SAMRecordFieldTags.
    /// </summary>
    [Serializable]
    public class SAMRecordField
    {
        #region Constructors
        /// <summary>
        /// Creates SAMRecordField instance.
        /// </summary>
        public SAMRecordField()
        {
            Tags = new List<SAMRecordFieldTag>();
        }

        /// <summary>
        /// Creates SAMRecordField with the specified type code.
        /// </summary>
        /// <param name="typecode">Type code.</param>
        public SAMRecordField(string typecode)
            : this()
        {
            Typecode = typecode;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Record field type code.
        /// for example. HD, SQ.
        /// </summary>
        public string Typecode { get; set; }

        /// <summary>
        /// List of SAM RecordFieldTags.
        /// </summary>
        public IList<SAMRecordFieldTag> Tags { get; private set; }
        #endregion
    }
}
