using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Bio;



namespace Bio.Variant
{
    /// <summary>
    /// A class to store information about a SNP variant.
    /// </summary>
    public class SNPVariant : Variant
    {
        /// <summary>
        /// The BP in the reference at this position.
        /// </summary>
        public readonly char RefBP;

        /// <summary>
        /// The variant present at this position.
        /// </summary>
        public readonly char  AltBP;

        /// <summary>
        /// Create a new SNP in the given reference at the given position.
        /// </summary>
        /// <param name="position">0-based position on reference.</param>
        /// <param name="altAllele">The variant present (A, C, G, T)</param>
        /// <param name="refBP">The Reference sequence.</param>
        public SNPVariant(int position, char altAllele, char refBP, bool atEnd = false) :
        base (position, atEnd)
        {
            AltBP = altAllele;
            Type = VariantType.SNP;
            RefBP = (char) refBP;
            Length = 1;
        }
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Bio.Variant.SNPVariant"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Bio.Variant.SNPVariant"/>.</returns>
        public override string ToString ()
        {
            return RefBP + "->" + AltBP + " @ " + StartPosition;
        }
    }
}
