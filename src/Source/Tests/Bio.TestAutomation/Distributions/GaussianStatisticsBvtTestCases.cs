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
    /// Bvt Test Cases for GaussianStatistics Class
    /// </summary>
    [TestClass]
    public class GaussianStatisticsBvtTestCases
    {
        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static GaussianStatisticsBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region GaussianStatistics.cs Bvt Test Cases

        /// <summary>
        /// validates the GetMissingInstance property
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGaussianStatisticsGetMissingInstance()
        {
            GaussianStatistics gaussStat = GaussianStatistics.GetMissingInstance;
            Assert.IsTrue(gaussStat.IsMissing());
        }

        /// <summary>
        /// validates the GetHashCode method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGaussianStatisticsGetHashCode()
        {
            GaussianStatistics gaussStat = GaussianStatistics.GetMissingInstance;
            Assert.AreEqual(gaussStat.GetHashCode(), MissingStatistics.GetInstance.GetHashCode());

            GaussianStatistics gaussStat1 = GaussianStatistics.GetInstance(1.12, 0.65, 7);
            Assert.AreEqual(gaussStat1.GetHashCode(), gaussStat1.ToString().GetHashCode());
        }

        /// <summary>
        /// validates the Equals method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGaussianStatisticsEquals()
        {
            SufficientStatistics suffStat = null;
            GaussianStatistics gaussStat = GaussianStatistics.GetMissingInstance;
            Assert.IsFalse(gaussStat.Equals(suffStat));

            suffStat = (SufficientStatistics)gaussStat;
            Assert.IsTrue(gaussStat.Equals(suffStat));

            gaussStat = GaussianStatistics.GetInstance(1.1, 2.2, 3);
            suffStat = (SufficientStatistics)gaussStat;
            Assert.IsTrue(gaussStat.Equals(suffStat));

            object suffStatObj = (object)gaussStat;
            Assert.IsTrue(gaussStat.Equals(suffStat));
        }

        /// <summary>
        /// validates the TryParse method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGaussianStatisticsTryParse()
        {
            SufficientStatistics result = null;
            GaussianStatistics gaussStat = GaussianStatistics.GetInstance(1.1, 2.3, 5);
            Assert.IsTrue(GaussianStatistics.TryParse("1.1,2.3,5", out result));
            Assert.IsTrue(gaussStat.Equals(result));
        }

        /// <summary>
        /// validates the properties of GaussianStatistics class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGaussianStatisticsProperties()
        {
            GaussianStatistics gaussStat = GaussianStatistics.GetInstance(1.1, 2.3, 5);
            Assert.AreEqual(1.1, gaussStat.Mean);
            Assert.AreEqual(2.3, gaussStat.Variance);
            Assert.AreEqual(5, gaussStat.SampleSize);
        }

        /// <summary>
        /// validates the Add method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGaussianStatisticsAdd()
        {
            GaussianStatistics gaussStat1 = GaussianStatistics.GetInstance(1.1, 2.3, 5);
            GaussianStatistics gaussStat2 = GaussianStatistics.GetInstance(2.1, 3.3, 10);
            GaussianStatistics gaussStatSum = GaussianStatistics.Add(gaussStat1, gaussStat2);
            Assert.AreEqual(1.7666666666666666, gaussStatSum.Mean);
            Assert.AreEqual(15, gaussStatSum.SampleSize);
            Assert.AreEqual(3.1888888888888887, gaussStatSum.Variance);
            Assert.AreEqual(94.649999999999991, gaussStatSum.SumOfSquares);
        }

        /// <summary>
        /// validates the ToString method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGaussianStatisticsToString()
        {
            GaussianStatistics gaussStat1 = GaussianStatistics.GetInstance(1.1, 2.3, 5);
            GaussianStatistics gaussStat2 = GaussianStatistics.GetMissingInstance;
            Assert.AreEqual("1.1,2.3,5", gaussStat1.ToString());
            Assert.AreEqual("Missing", gaussStat2.ToString());
        }

        /// <summary>
        /// Validates the AsStatisticsList() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGaussianStatisticsAsStatisticsList()
        {
            GaussianStatistics boolStat1 = GaussianStatistics.GetInstance(5, 2, 2);
            StatisticsList sList = boolStat1.AsStatisticsList();
            Assert.AreEqual(1, sList.Count);
        }

        /// <summary>
        /// Validates the AsBooleanStatistics() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGaussianStatisticsAsBooleanStatistics()
        {
            GaussianStatistics boolStat1 = GaussianStatistics.GetInstance(0, 2, 2);
            BooleanStatistics gStat = boolStat1.AsBooleanStatistics();
            Assert.AreEqual(0, gStat.AsGaussianStatistics().Mean);
        }

        /// <summary>
        /// Validates the AsContinuousStatistics() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGaussianStatisticsAsContinuousStatistics()
        {
            GaussianStatistics boolStat1 = GaussianStatistics.GetInstance(5, 2, 2);
            ContinuousStatistics cStat = boolStat1.AsContinuousStatistics();
            Assert.AreEqual(5, cStat.AsGaussianStatistics().Mean);
        }

        /// <summary>
        /// Validates the AsDiscreteStatistics() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGaussianStatisticsAsDiscreteStatistics()
        {
            IEnumerable<double> obj = new List<double>() { 4, 2, 3 };
            GaussianStatistics boolStat1 = GaussianStatistics.GetInstance(obj);
            DiscreteStatistics dStat = boolStat1.AsDiscreteStatistics();
            Assert.AreEqual(3, dStat.AsGaussianStatistics().Mean);
        }

        /// <summary>
        /// Validates the BooleanStatistics(MissingStatistics) method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGaussianStatisticsExplicitMissingStatistics()
        {
            MissingStatistics missStat = MissingStatistics.GetInstance;
            GaussianStatistics boolStat2 = (GaussianStatistics)missStat;
            Assert.AreEqual(missStat, boolStat2);

            MissingStatistics missStat2 = null;
            Assert.IsNull((GaussianStatistics)missStat2);
        }

        #endregion GaussianStatistics.cs Bvt Test Cases
    }
}
