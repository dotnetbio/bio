using System;
using Bio.Util.Distribute;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.TestAutomation.BioUtil.Distribute
{
    /// <summary>
    /// RaiseError Bvt Test cases
    /// </summary>
    [TestClass]
    public class RaiseErrorBvtTestCases
    {
        #region Bvt Test cases
        /// <summary>
        /// Validates Distribute method of the RaiseError class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateDistribute()
        {
            RaiseError raiseError = new RaiseError();
            IDistributable idistributable = new CommandApp();

            try
            {
                raiseError.Distribute(idistributable);
                Assert.Fail("RaiseError BVT: RaiseError Distribute Not validated successfully");
            }
            catch (NotImplementedException ex)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "RaiseError BVT: RaiseError Distribute validated successfully:", ex.Message));
            }
        }

        /// <summary>
        /// Validates FinalizeParse method of the RaiseError class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFinalizeParse()
        {
            RaiseError raiseError = new RaiseError();

            //// Todo: Not implemented in dev code, please implement after changes in the dev code. 
            raiseError.FinalizeParse(); 
        }

        #endregion Bvt Test cases
    }
}
