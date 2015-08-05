using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bio;

namespace Bio.Variant
{
    /// <summary>
    /// The two types of indels, insertions or deletions.
    /// </summary>
    [Flags]
    public enum IndelType : byte { 
        /// <summary>
        /// An insertion relative to the reference.
        /// </summary>
        Insertion, 
        /// <summary>
        /// A deletion relative to the reference.
        /// </summary>
        Deletion}


    /// <summary>
    /// An Indel Variant.
    /// </summary>
    public class IndelVariant : Variant
    {
        /// <summary>
        /// Insertion or Deletion
        /// </summary>
        public readonly IndelType InsertionOrDeletion;

        /// <summary>
        /// These are the bases in the insertion (or what was deleted).
        /// </summary>
        public readonly string InsertedOrDeletedBases;

        /// <summary>
        /// True if the homopolymer length of the reference is > 1.
        /// <code>
        /// e.g. ACC -> TRUE
        ///      A-C
        /// </code>
        /// </summary>
        /// <value><c>true</c> if in homo polymer; otherwise, <c>false</c>.</value>
        public bool InHomopolymer {
            get {
               
                return HomopolymerLengthInReference > 1;
            }
        }

        ///
        /// At the position of the insertion or deletion, what is the length of the nearest homopolymer run in the reference.
        /// For insertions, the HP length is defined by the next base in the reference.
        /// 
        /// <code>
        /// For example, the insertion below occurs at an HP of char 'C' and length 3.
        /// 
        /// ref:   AT-CCCTG
        /// query: ATCCCCTG
        /// 
        /// For multiple insertions or a long deletion, only the HP of the first position is counted,
        /// so the example below would be a 2 bp long 'C'
        /// 
        /// 
        /// ref:   ATCCTGCATA
        /// query: AT-----ATA
        /// 
        /// 
        /// </code>
        ///
        public int HomopolymerLengthInReference {
            get;
            set;
        }

        /// <summary>
        /// A, C, G or T the basepair of the homopolymer.
        /// </summary>
        /// <value>The homopolymer base.</value>
        public char HomopolymerBase {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bio.Variant.IndelVariant"/> class.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="length">Length.</param>
        /// <param name="bases">Bases.</param>
        /// <param name="insertionOrDeletion">Insertion or deletion.</param>
        /// <param name="hpBase">Hp base.</param>
        /// <param name="hpLengthInReference">Hp length in reference.</param>
        /// <param name="atAlignmentEnd">If set to <c>true</c> at alignment end.</param>
        public IndelVariant(int position, int length, string bases, IndelType insertionOrDeletion, char hpBase, int hpLengthInReference, bool atAlignmentEnd = false) 
            : base (position, atAlignmentEnd)
        {
            Type = VariantType.INDEL;
            InsertionOrDeletion = insertionOrDeletion;
            Length = length;
            InsertedOrDeletedBases = bases;
            HomopolymerBase = hpBase;
            HomopolymerLengthInReference = hpLengthInReference;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Bio.Variant.IndelVariant"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Bio.Variant.IndelVariant"/>.</returns>
        public override string ToString ()
        {
            var insert = this.InsertionOrDeletion == IndelType.Deletion ? "Deletion" : "Insertion";
            return string.Format ("[IndelVariant: InHomopolymer={0}, HomopolymerLengthInReference={1}, HomopolymerBase={2}, Type={3}]", InHomopolymer, HomopolymerLengthInReference, HomopolymerBase, insert);
        }
    }
}