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
    /// Bvt Test Cases for DiscreteStatistics Class
    /// </summary>
    [TestClass]
    public class DiscreteStatisticsBvtTestCases
    {
        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static DiscreteStatisticsBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region DiscreteStatistics Bvt Test Cases

        /// <summary>
        /// validates the GetMissingInstance property
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateDiscreteStatisticsGetMissingInstance()
        {
            DiscreteStatistics discreteStat = DiscreteStatistics.GetMissingInstance;
            Assert.IsTrue(discreteStat.IsMissing());
        }

        /// <summary>
        /// validates the GetHashCode method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateDiscreteStatisticsGetHashCode()
        {
            DiscreteStatistics discreteStat = DiscreteStatistics.GetMissingInstance;
            Assert.AreEqual(discreteStat.GetHashCode(), MissingStatistics.GetInstance.GetHashCode());

            DiscreteStatistics discreteStat1 = DiscreteStatistics.GetInstance(1);
            Assert.AreEqual(discreteStat1.Value, discreteStat1.GetHashCode());
        }

        /// <summary>
        /// validates the Equals method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateDiscreteStatisticsEquals()
        {
            SufficientStatistics suffStat = null;
            DiscreteStatistics discreteStat = DiscreteStatistics.GetMissingInstance;
            Assert.IsFalse(discreteStat.Equals(suffStat));

            suffStat = (SufficientStatistics)discreteStat;
            Assert.IsTrue(discreteStat.Equals(suffStat));

            discreteStat = DiscreteStatistics.GetInstance(1);
            suffStat = (SufficientStatistics)discreteStat;
            Assert.IsTrue(discreteStat.Equals(suffStat));

            object suffStatObj = (object)discreteStat;
            Assert.IsTrue(discreteStat.Equals(suffStat));
        }

        /// <summary>
        /// validates the TryParse method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateDiscreteStatisticsTryParse()
        {
            SufficientStatistics result = null;
            DiscreteStatistics discreteStat = DiscreteStatistics.GetInstance(1);
            Assert.IsTrue(DiscreteStatistics.TryParse("1", out result));
            Assert.IsTrue(discreteStat.Equals(result));
        }

        /// <summary>
        /// validates properties of the DiscreteStatistics class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateDiscreteStatistics()
        {
            DiscreteStatistics discreteStat = DiscreteStatistics.GetInstance(1);
            Assert.AreEqual(1, discreteStat.Value);
        }

        /// <summary>
        /// validates the ToString method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateDiscreteStatisticsToString()
        {
            DiscreteStatistics discreteStat1 = DiscreteStatistics.GetInstance(1);
            DiscreteStatistics discreteStat2 = DiscreteStatistics.GetMissingInstance;
            Assert.AreEqual(discreteStat1.Value.ToString((IFormatProvider)null), discreteStat1.ToString());
            Assert.AreEqual("Missing", discreteStat2.ToString());
        }

        /// <summary>
        /// Validates the AsGaussianStatistics() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateDiscreteStatisticsAsDiscreteStatistics()
        {
            DiscreteStatistics boolStat1 = DiscreteStatistics.GetInstance(5);
            GaussianStatistics dStat = boolStat1.AsGaussianStatistics();
            Assert.AreEqual(boolStat1, dStat.AsDiscreteStatistics());
        }

        /// <summary>
        /// Validates the AsStatisticsList() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateDiscreteStatisticsAsStatisticsList()
        {
            DiscreteStatistics boolStat1 = DiscreteStatistics.GetInstance(5);
            StatisticsList dStat = boolStat1.AsStatisticsList();
            Assert.AreEqual(boolStat1, dStat.AsDiscreteStatistics());
        }

        /// <summary>
        /// Validates the DiscreteStatistics(MissingStatistics) method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateContinuousStatisticsExplicitMissingStatistics()
        {
            MissingStatistics missStat = MissingStatistics.GetInstance;
            DiscreteStatistics boolStat2 = (DiscreteStatistics)missStat;
            Assert.AreEqual(-1, (int)boolStat2);

            MissingStatistics missStat2 = null;
            Assert.IsNull((DiscreteStatistics)missStat2);
        }

        #endregion DiscreteStatistics Bvt Test Cases
    }
}
