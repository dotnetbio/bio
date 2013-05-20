using System;
using Bio.Algorithms.Alignment.Legacy;
using Bio.Registration;
using Bio.Algorithms.Alignment;

namespace Bio.Test.AddIns
{
    /// <summary>
    /// Add-in for the Negative test:
    /// the add-in that cannot be constructed by the Registration API and results in exception thrown.
    /// </summary>
    [RegistrableAttribute(true)] 
    public class AlignerNoDefaultConstructor : NeedlemanWunschAligner
    {
        /// <summary>
        /// Private default constructor
        /// </summary>
        private AlignerNoDefaultConstructor()
        {
        }

        /// <summary>
        /// Public constructor with parameter;
        /// not accessible via default Activator.CreateInstance
        /// </summary>
        /// <param name="gapCost"></param>
        public AlignerNoDefaultConstructor(int gapCost)
        {
            this.GapOpenCost = gapCost;
        }
    }
}
