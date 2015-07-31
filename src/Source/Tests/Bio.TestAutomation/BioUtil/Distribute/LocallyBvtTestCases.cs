using System;
using Bio.Util.Distribute;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Util;

namespace Bio.TestAutomation.BioUtil.Distribute
{
    /// <summary>
    /// Locally Bvt Test cases
    /// </summary>
    [TestClass]
    public class LocallyBvtTestCases
    {
        #region Bvt Test cases
        /// <summary>
        /// Validates Distribute method of the Locally class
        /// </summary>
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateDistribute()
        {
            Locally locally = new Locally();
            IDistributable idistributable = new CommandApp();

            try
            {
                locally.Cleanup = true;
                locally.TaskCount = 10;
                locally.Tasks = new RangeCollection(1, 500); 
                locally.Distribute(idistributable);
                ApplicationLog.WriteLine("Locally BVT: Locally Distribute Validated successfully");
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "Locally BVT: Locally Distribute not validated successfully:", ex.Message));
                Assert.Fail("Locally BVT: Locally Distribute Not validated successfully");
            }
        }

        /// <summary>
        /// Validates FinalizeParse method of the Locally class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFinalizeParse()
        {
            Locally locally = new Locally();

            try
            {
                locally.Cleanup = true; 
                locally.FinalizeParse();
                ApplicationLog.WriteLine("Locally BVT: Locally FinalizeParse Validated successfully");
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "Locally BVT: Locally FinalizeParse Not validated successfully:", ex.Message));
                Assert.Fail("Locally BVT: Locally FinalizeParse Not validated successfully");
            }
        }

        #endregion Bvt Test cases
    }
}
