using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bio;

namespace Bio.Variant
{
    /// <summary>
    /// Top level class for holding variant information.  
    /// This is implemented in two sub classes, SNPVariant and IndelVariant.
    /// </summary>
    public abstract class Variant : IComparable<Variant>, IEquatable<Variant>
    {
        /// <summary>
        /// The name of the reference sequence the variant was called from.
        /// </summary>
        public string RefName;

        /// <summary>
        /// SNP, indel, or complex.
        /// </summary>
        public VariantType Type { get; protected set; }

        /// <summary>
        /// 0-based start position of variant.  
        /// <code>
        /// For Indels, this is the left most position 
        /// BEFORE the event. e.g.
        /// <code>
        /// AAATTTAAA   -> is a deletion of length 3 starting at position 2.
        /// AAA---AAA
        /// 
        /// A--TTAAA -> is an insertion of length 2 starting at position 0.
        /// ACCTTAAA
        /// 
        /// ACCTTAAA  -> is a deletion at position 3.
        /// ACC-TAAA
        /// </code>
        /// 
        /// </summary>
        public int StartPosition { get; set; }

        /// <summary>
        /// O-based end index (same as start for SNPs).
        /// A SNP is of length 1, a deletion of length 2 is 2, etc.
        /// </summary>
        public int Length { get; protected set; }

        /// <summary>
        /// The position in the alignment where this variant ends, exclusive.
        /// For a SNP, this is the position AFTER the SNP.  Same for indels.
        /// 
        /// <code>
        /// AACCTT -> End Position = 3
        /// AAGCTT
        /// </code>
        /// 
        /// </summary>
        public int EndPosition
        {
            get { return StartPosition + Length; }
        }

        /// <summary>
        /// Is the variant at the very start or end of an alignment? 
        /// (that is it was called based on the first or last base seen on 
        /// either sequence in the alignment.)
        /// These can have certain pathologies so we take note and keep an eye on them.
        /// They should almost always be excluded by the alignment algorithm clipping at the ends.
        /// </summary>
        public bool AtEndOfAlignment { get; protected set; }

        /// <summary>
        /// The QV value for this call, if it exists. If not, this is set to 0.
        /// 
        /// For deletion mutations, the QV value is obtained from the next query
        /// base in the alignment.
        /// </summary>
        /// <value>The QV value.</value>
        public byte QV {get; set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="Bio.Variant.Variant"/> class.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="atAlignmentEnd">If set to <c>true</c> at alignment end.</param>
        public Variant(int position,  bool atAlignmentEnd = false)
        {
            StartPosition = position;
            AtEndOfAlignment = atAlignmentEnd;
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Bio.Variant.Variant"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode ()
        {
            //TODO: This is a little unsafe as these fields are mutable...
            var h1 = RefName != null ? RefName.GetHashCode () : 0;
            var h2 = StartPosition.GetHashCode ();
            var h3 = EndPosition.GetHashCode ();
            return h1 ^ h2 ^ h3;
        }
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Bio.Variant.Variant"/>.
        /// 
        /// Two variants are "equal" if they have the same start and end positions and are of the same type. 
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="Bio.Variant.Variant"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Bio.Variant.Variant"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals (object obj)
        {
            var res = obj as Variant;
            if (res != null)
                return this.Equals (res);
            return false;
        }
        #region IComparable implementation

        /// <Docs>To be added.</Docs>
        /// <para>Returns the sort order (Reference Name, Start Position) of the current variant compared to the specified object.</para>
        /// <summary>
        /// 
        /// </summary>
        /// <returns>The to.</returns>
        /// <param name="other">Other.</param>
        public int CompareTo (Variant other)
        {
            var nameComp = String.CompareOrdinal(RefName,other.RefName);
            if (nameComp == 0) {
                return StartPosition.CompareTo (other.StartPosition);
            }
            return nameComp;
        }

        #endregion

        #region IEquatable implementation
        /// <summary>
        /// Determines whether the specified <see cref="Bio.Variant.Variant"/> is equal to the current <see cref="Bio.Variant.Variant"/>.
        /// 
        /// Two variants are equal if they have the same reference, start/end and type.
        /// </summary>
        /// <param name="other">The <see cref="Bio.Variant.Variant"/> to compare with the current <see cref="Bio.Variant.Variant"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="Bio.Variant.Variant"/> is equal to the current
        /// <see cref="Bio.Variant.Variant"/>; otherwise, <c>false</c>.</returns>
        bool IEquatable<Variant>.Equals (Variant other)
        {
            var otherComp = CompareTo (other);
            if (otherComp == 0) {
                return this.StartPosition == other.StartPosition && this.Type == other.Type && this.EndPosition == other.EndPosition;
            }
            return false;
        }

        #endregion
    }

    /// <summary>
    /// The type of variant, either SNP, INDEL or Complex.
    /// </summary>
    public enum VariantType
    {
        /// <summary>
        /// A SNP variant.
        /// </summary>
        SNP,
        /// <summary>
        /// And Indel variant.
        /// </summary>
        INDEL,
        /// <summary>
        /// A Complex variant.  Not currently used but a place holder for more complex scenarios in the future.
        /// </summary>
        Complex
    }
}
