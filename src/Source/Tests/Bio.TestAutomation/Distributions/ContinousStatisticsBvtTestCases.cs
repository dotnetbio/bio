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
    /// Bvt Test Cases for ContinousStatistics Class
    /// </summary>
    [TestClass]
    public class ContinousStatisticsBvtTestCases
    {
        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static ContinousStatisticsBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region ContinuousStatistics Bvt Test Cases

        /// <summary>
        /// validates the GetMissingInstance property
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateContinuousStatisticsGetMissingInstance()
        {
            ContinuousStatistics continousStat = ContinuousStatistics.GetMissingInstance;
            Assert.IsTrue(continousStat.IsMissing());
        }

        /// <summary>
        /// validates the Equals method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateContinuousStatisticsEquals()
        {
            SufficientStatistics suffStat = ContinuousStatistics.GetInstance(2);
            ContinuousStatistics continousStat = ContinuousStatistics.GetMissingInstance;
            Assert.IsFalse(continousStat.Equals(suffStat));

            suffStat = (SufficientStatistics)continousStat;
            Assert.IsTrue(continousStat.Equals(suffStat));

            continousStat = ContinuousStatistics.GetInstance(1.23565);
            suffStat = (SufficientStatistics)continousStat;
            Assert.IsTrue(continousStat.Equals(suffStat));

            object suffStatObj = (object)continousStat;
            Assert.IsTrue(continousStat.Equals(suffStat));
        }

        /// <summary>
        /// validates the TryParse method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateContinuousStatisticsTryParse()
        {
            SufficientStatistics result = null;
            ContinuousStatistics continousStat = ContinuousStatistics.GetInstance(1.25658);
            Assert.IsTrue(ContinuousStatistics.TryParse("1.25658", out result));
            Assert.IsTrue(continousStat.Equals(result));
        }

        /// <summary>
        /// validates properties of the ContinuousStatistics class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateContinuousStatisticsProperties()
        {
            ContinuousStatistics continousStat = ContinuousStatistics.GetInstance(1.25658);
            Assert.AreEqual(1.25658, continousStat.Value);
        }

        /// <summary>
        /// validates the ToString method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateContinuousStatisticsToString()
        {
            ContinuousStatistics continousStat1 = ContinuousStatistics.GetInstance(1);
            ContinuousStatistics continousStat2 = ContinuousStatistics.GetMissingInstance;
            Assert.AreEqual(continousStat1.Value.ToString((IFormatProvider)null), continousStat1.ToString());
            Assert.AreEqual("Missing", continousStat2.ToString());
        }

        /// <summary>
        /// Validates the AsDiscreteStatistics() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateContinuousStatisticsAsDiscreteStatistics()
        {
            ContinuousStatistics boolStat1 = ContinuousStatistics.GetInstance(5);
            DiscreteStatistics dStat = boolStat1.AsDiscreteStatistics();
            Assert.AreEqual(boolStat1, dStat.AsContinuousStatistics());
        }

        /// <summary>
        /// Validates the AsStatisticsList() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateContinuousStatisticsAsStatisticsList()
        {
            ContinuousStatistics boolStat1 = ContinuousStatistics.GetInstance(5);
            StatisticsList dStat = boolStat1.AsStatisticsList();
            Assert.AreEqual(boolStat1, dStat.AsContinuousStatistics());
        }

        /// <summary>
        /// Validates the GetHashCode() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateContinuousStatisticsGetHashCode()
        {
            ContinuousStatistics boolStat1 = ContinuousStatistics.GetInstance(5);
            Assert.AreEqual(1075052544, boolStat1.GetHashCode());
        }

        /// <summary>
        /// Validates the ContinuousStatistics(Discrete) method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateContinuousStatisticsExplicitDiscrete()
        {
            DiscreteStatistics discreteStat2 = DiscreteStatistics.GetInstance(5);
            ContinuousStatistics boolStat2 = (ContinuousStatistics)discreteStat2;
            Assert.AreEqual(5, (int)(DiscreteStatistics)boolStat2);

            DiscreteStatistics discreteStat = DiscreteStatistics.GetMissingInstance;
            ContinuousStatistics boolStat1 = (ContinuousStatistics)discreteStat;
            Assert.AreEqual(discreteStat, (DiscreteStatistics)boolStat1);

            DiscreteStatistics discreteStat3 = null;
            Assert.IsNull((ContinuousStatistics)discreteStat3);
        }

        /// <summary>
        /// Validates the int(ContinuousStatistics) method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateContinuousStatisticsExplicitInt()
        {
            DiscreteStatistics discreteStat2 = DiscreteStatistics.GetInstance(5);
            ContinuousStatistics boolStat2 = (ContinuousStatistics)discreteStat2;
            Assert.AreEqual(5, (int)boolStat2);
            Assert.AreEqual(5, (double)boolStat2);
            try
            {

                DiscreteStatistics discreteStat = DiscreteStatistics.GetMissingInstance;
                ContinuousStatistics boolStat1 = (ContinuousStatistics)discreteStat;
                int temp = (int)boolStat1;
            }
            catch (ArgumentException)
            {
                ApplicationLog.WriteLine("ContinuousStatistics : Successfully validated the int(0)");
            }

            DiscreteStatistics discreteStat3 = null;
            try
            {
                ContinuousStatistics boolStat3 = (ContinuousStatistics)discreteStat3;
                int integerCS = (int)boolStat3;
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                ApplicationLog.WriteLine("BooleanStatistics : Successfully validated the int(booleanstatistics)");
            }
        }

        /// <summary>
        /// Validates the ContinuousStatistics(MissingStatistics) method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateContinuousStatisticsExplicitMissingStatistics()
        {
            MissingStatistics missStat = MissingStatistics.GetInstance;
            ContinuousStatistics boolStat2 = (ContinuousStatistics)missStat;
            Assert.IsTrue(boolStat2.IsMissing());

            MissingStatistics missStat2 = null;
            Assert.IsNull((ContinuousStatistics)missStat2);
        }

        #endregion ContinuousStatistics Bvt Test Cases
    }
}
