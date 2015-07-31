/****************************************************************************
 * FastaP2TestCases.cs
 * 
 *   This file contains the Fasta - Parsers and Formatters Priority Two test cases.
 * 
***************************************************************************/

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Bio.IO.FastA;
using Bio.Util.Logging;

#if (SILVERLIGHT == false)
    namespace Bio.TestAutomation.IO.FastA
#else
    namespace Bio.Silverlight.TestAutomation.IO.FastA
#endif
{
    /// <summary>
    /// FASTA Priority Two parser and formatter test cases implementation.
    /// </summary>
    [TestFixture]
    public class FastAP2TestCases
    {

        /// <summary>
        /// Invalidate close method; should be allowed now.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void FastAFormatterInvalidateClose()
        {
            FastAFormatter formatter = new FastAFormatter();
            formatter.Close();
        }

        /// <summary>
        /// Invalidate FormatString method
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void FastAFormatterInvalidateFormatString()
        {
            ISequence iSeq = null;
            try
            {
                new FastAFormatter().FormatString(iSeq);
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.Write("Fasta P2 : ArgumentNullException caught successfully. " + ex.Message);
            }
        }
    }
}
