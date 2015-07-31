using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using Bio.Distributions;

namespace Bio.TestAutomation.Distributions
{
    /// <summary>
    /// Bvt Test Cases Fir MissingStatistics Class
    /// </summary>
    [TestClass]
    public class MissingStatisticsBvtTestCases
    {
        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static MissingStatisticsBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region MissingStatistics Bvt Test Cases

        /// <summary>
        /// validates the GetInstance property
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMissingStatisticsGetInstance()
        {
            MissingStatistics missingStat = MissingStatistics.GetInstance;
            Assert.IsTrue(missingStat.IsMissing());
        }

        /// <summary>
        /// validates the GetHashCode method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMissingStatisticsGetHashCode()
        {
            MissingStatistics missingStat = MissingStatistics.GetInstance;
            Assert.AreEqual(missingStat.GetHashCode(), MissingStatistics.GetInstance.GetHashCode());
        }

        /// <summary>
        /// validates the Equals method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMissingStatisticsEquals()
        {
            SufficientStatistics suffStat = null;
            MissingStatistics missingStat = MissingStatistics.GetInstance;
            Assert.IsFalse(missingStat.Equals(suffStat));

            suffStat = (SufficientStatistics)missingStat;
            Assert.IsTrue(missingStat.Equals(suffStat));

            missingStat = MissingStatistics.GetInstance;
            suffStat = (SufficientStatistics)missingStat;
            Assert.IsTrue(missingStat.Equals(suffStat));

            object suffStatObj = (object)missingStat;
            Assert.IsTrue(missingStat.Equals(suffStat));
        }

        /// <summary>
        /// validates the TryParse method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMissingStatisticsTryParse()
        {
            SufficientStatistics result = null;
            MissingStatistics missingStat = MissingStatistics.GetInstance;
            Assert.IsTrue(MissingStatistics.TryParse("?", out result));
            Assert.IsTrue(missingStat.Equals(result));
            Assert.IsTrue(MissingStatistics.TryParse("missing", out result));
            Assert.IsTrue(missingStat.Equals(result));
        }

        /// <summary>
        /// validates properties of the MissingStatistics class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMissingStatisticsConstants()
        {
            char missingChar = MissingStatistics.MissingChar;
            Assert.AreEqual('?', missingChar);
        }

        /// <summary>
        /// validates the ToString method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMissingStatisticsToString()
        {
            MissingStatistics missingStat1 = MissingStatistics.GetInstance;
            Assert.AreEqual("?", missingStat1.ToString());
        }

        /// <summary>
        /// Validates the AsStatisticsList() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMissingStatisticsAsStatisticsList()
        {
            MissingStatistics boolStat1 = MissingStatistics.GetInstance;
            StatisticsList sList = boolStat1.AsStatisticsList();
            Assert.AreEqual(1, sList.Count);
        }

        /// <summary>
        /// Validates the AsGaussianStatistics() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMissingStatisticsAsGaussianStatistics()
        {
            MissingStatistics boolStat1 = MissingStatistics.GetInstance;
            GaussianStatistics gStat = boolStat1.AsGaussianStatistics();
            Assert.AreEqual("Missing", gStat.ToString());
        }

        /// <summary>
        /// Validates the AsContinuousStatistics() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMissingStatisticsAsContinuousStatistics()
        {
            MissingStatistics boolStat1 = MissingStatistics.GetInstance;
            ContinuousStatistics cStat = boolStat1.AsContinuousStatistics();
            Assert.IsTrue(cStat.IsMissing());
        }

        /// <summary>
        /// Validates the AsDiscreteStatistics() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateMissingStatisticsAsDiscreteStatistics()
        {
            MissingStatistics boolStat1 = MissingStatistics.GetInstance;
            DiscreteStatistics dStat = boolStat1.AsDiscreteStatistics();
            Assert.AreEqual(boolStat1, dStat.AsBooleanStatistics());
        }

        #endregion MissingStatistics Bvt Test Cases
    }
}
