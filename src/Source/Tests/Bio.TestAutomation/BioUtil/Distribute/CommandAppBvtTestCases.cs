using System;
using Bio.Util;
using Bio.Util.Distribute;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.TestAutomation.BioUtil.Distribute
{
    /// <summary>
    /// CommandApp Bvt Test cases
    /// </summary>
    [TestClass]
    public class CommandAppBvtTestCases
    {
        #region Bvt Test cases

        /// <summary>
        /// Validates RunTasks method of the CommandApp class
        /// </summary>
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateRunTasks()
        {
            CommandApp commandApp = new CommandApp();
            RangeCollection rangeColl = new RangeCollection(1, 500);
            try
            {
                commandApp.RunTasks(rangeColl, 10);
                ApplicationLog.WriteLine("CommandApp BVT: CommandApp RunTasks Validated successfully");
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "CommandApp BVT: CommandApp RunTasks Validated successfully: {0}", ex.Message));
                Assert.Fail("CommandApp BVT: CommandApp RunTasks Not validated successfully");
            }
        }

        /// <summary>
        /// Validates Run method of the SelfDistributable class
        /// </summary>
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateRun()
        {
            SelfDistributable selfDistributable = new CommandApp();

            try
            {
                selfDistributable.Distribute = new Locally();
                selfDistributable.Run();
                ApplicationLog.WriteLine("SelfDistributable BVT: SelfDistributable Run Validated successfully");
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "SelfDistributable BVT: SelfDistributable Run Validated successfully:", ex.Message));
                Assert.Fail("SelfDistributable BVT: SelfDistributable Run Not validated successfully");
            }
        }

        /// <summary>
        /// Validates Cancel method of the SelfDistributable class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateCancel()
        {
            SelfDistributable selfDistributable = new CommandApp();

            try
            {
                selfDistributable.Distribute = new Locally();
                selfDistributable.Cancel();
                ApplicationLog.WriteLine("SelfDistributable BVT: SelfDistributable Cancel Validated successfully");
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "SelfDistributable BVT: SelfDistributable Cancel Validated successfully:", ex.Message));
                Assert.Fail("SelfDistributable BVT: SelfDistributable Cancel Not validated successfully");
            }
        }

        /// <summary>
        /// Validates RunTasks method of the CommandApp class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateCleanup()
        {
            CommandApp commandApp = new CommandApp();
            try
            {
                commandApp.Cleanup(10);
                ApplicationLog.WriteLine("CommandApp BVT: CommandApp Cleanup Validated successfully");
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "CommandApp BVT: CommandApp Cleanup Not validated successfully:", ex.Message));
                Assert.Fail("CommandApp BVT: CommandApp Cleanup Not validated successfully");
            }
        }

        #endregion Bvt Test cases
    }
}
