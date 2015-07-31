/****************************************************************************
 * ParallelOptionsScopeBvtTestCases.cs
 * 
 * This file contains the ParallelOptionsScope BVT test cases.
 * 
******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bio.TestAutomation.Util;
using Bio.Util;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Threading.Tasks;

#if (SILVERLIGHT == false)
namespace Bio.TestAutomation.Util
#else
   namespace Bio.SilverLight.TestAutomation
#endif
{
    /// <summary>
    /// Test Automation code for Bio ParallelOptionsScope and BVT level validations.
    /// </summary>
    [TestClass]
    public class ParallelOptionsScopeBvtTestCases
    {

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static ParallelOptionsScopeBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region ParallelOptionsScope Bvt TestCases

        /// <summary>
        /// Validate All properties of ParallelOptionsScope.
        /// Input Data : Valid All ParallelOptionsScope.
        /// Output Data : Validate properties.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateParallelOptionsScopeProperties()
        {
            using (ParallelOptionsScope.Create(Environment.ProcessorCount))
            {
                Assert.IsTrue(ParallelOptionsScope.Exists);
                ParallelOptions parallel = ParallelOptionsScope.Current;
                Assert.IsNotNull(parallel);
                Assert.IsNotNull(parallel.CancellationToken);
                Assert.AreEqual(Constants.SerialProcess, parallel.TaskScheduler.Id);
                Assert.AreEqual(Environment.ProcessorCount, parallel.MaxDegreeOfParallelism);
                Assert.AreEqual(1, ParallelOptionsScope.SingleThreadedOptions.MaxDegreeOfParallelism);
                Assert.AreEqual(
                    Environment.ProcessorCount,
                    ParallelOptionsScope.FullyParallelOptions.MaxDegreeOfParallelism);
            }
            ApplicationLog.WriteLine("Trace BVT: Validation of all ParallelOptionsScope properties completed successfully.");
        }

        /// <summary>
        /// Validate Create method of ParallelOptionsScope by passing tread count.
        /// Input Data : Valid tread count.
        /// Output Data : Validate ParallelOptionsScope.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateCreateForThreadCount()
        {
            using (ParallelOptionsScope parallel = ParallelOptionsScope.Create(Constants.ParallelProcess))
            {
                Assert.IsNotNull(parallel);
                Assert.AreEqual(Constants.ParallelProcess, ParallelOptionsScope.Current.MaxDegreeOfParallelism);
                Assert.IsTrue(ParallelOptionsScope.Exists);
            }
            ApplicationLog.WriteLine(string.Concat(
                  "ParallelOptionsScope BVT: Validation of Create method for thread count completed successfully."));
        }

        /// <summary>
        /// Validate Create method of ParallelOptionsScope by ParallelOptions.
        /// Input Data : Valid ParallelOptions.
        /// Output Data : Validate ParallelOptionsScope.
        /// </summary>
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateCreateForParallelOptions()
        {
            ParallelOptions parallel = new ParallelOptions();
            parallel.MaxDegreeOfParallelism = Constants.SerialProcess;

            ParallelOptionsScope scope = ParallelOptionsScope.Create(parallel);
            Assert.IsNotNull(scope);
            Assert.AreEqual(Constants.SerialProcess, ParallelOptionsScope.Current.MaxDegreeOfParallelism);
            Assert.IsTrue(ParallelOptionsScope.Exists);

            ApplicationLog.WriteLine(string.Concat(
                  "ParallelOptionsScope BVT: Validation of Create method for ParallelOptions completed successfully."));
        }

        /// <summary>
        /// Validate CreateSingleThreaded method of ParallelOptionsScope.
        /// Input Data : No Input Data required.
        /// Output Data : Validate ParallelOptionsScope.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateCreateSingleThreaded()
        {
            using (ParallelOptionsScope scope = ParallelOptionsScope.CreateSingleThreaded())
            {
                Assert.IsNotNull(scope);
                Assert.AreEqual(Constants.SerialProcess, ParallelOptionsScope.Current.MaxDegreeOfParallelism);
                Assert.IsTrue(ParallelOptionsScope.Exists);
            }
            ApplicationLog.WriteLine(string.Concat(
                  "ParallelOptionsScope BVT: Validation of CreateSingleThreaded method completed successfully."));
        }

        /// <summary>
        /// Validate CreateFullyParallel method of ParallelOptionsScope.
        /// Input Data : No Input Data required.
        /// Output Data : Validate ParallelOptionsScope.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateCreateFullyParallel()
        {
            using (ParallelOptionsScope scope = ParallelOptionsScope.CreateFullyParallel())
            {
                Assert.IsNotNull(scope);
                Assert.AreEqual(Environment.ProcessorCount, ParallelOptionsScope.Current.MaxDegreeOfParallelism);
                Assert.IsTrue(ParallelOptionsScope.Exists);
            }
            ApplicationLog.WriteLine(string.Concat(
                  "ParallelOptionsScope BVT: Validation of CreateFullyParallel method completed successfully."));
        }

        /// <summary>
        /// Validate Dispose method of ParallelOptionsScope.
        /// Input Data : No Input Data required.
        /// Output Data : Validate ParallelOptionsScope.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateCDispose()
        {
            ParallelOptionsScope scope = ParallelOptionsScope.Create(Constants.ParallelProcess);
            Assert.IsNotNull(scope);
            Assert.AreEqual(Constants.ParallelProcess, ParallelOptionsScope.Current.MaxDegreeOfParallelism);
            Assert.IsTrue(ParallelOptionsScope.Exists);

            scope.Dispose();
            Assert.IsFalse(ParallelOptionsScope.Exists);

            ApplicationLog.WriteLine(string.Concat(
                  "ParallelOptionsScope BVT: Validation of Dispose method completed successfully."));
        }

        /// <summary>
        /// Validate Suspend method of ParallelOptionsScope.
        /// Input Data : No Input Data required.
        /// Output Data : Validate ParallelOptionsScope.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSuspend()
        {
            using (ParallelOptionsScope scope = ParallelOptionsScope.Create(Constants.ParallelProcess))
            {
                Assert.IsNotNull(scope);
                Assert.AreEqual(Constants.ParallelProcess, ParallelOptionsScope.Current.MaxDegreeOfParallelism);
                Assert.IsTrue(ParallelOptionsScope.Exists);

                IDisposable disposable = ParallelOptionsScope.Suspend();
                Assert.IsFalse(ParallelOptionsScope.Exists);
                disposable.Dispose();
            }
            ApplicationLog.WriteLine(string.Concat(
                  "ParallelOptionsScope BVT: Validation of Suspend method completed successfully."));
        }

        #endregion ParallelOptionsScope Bvt TestCases
    }
}
