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
    /// Bvt Test Cases for SufficientStatistics Class
    /// </summary>
    [TestClass]
    public class SufficientStatisticsBvtTestCases
    {
        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static SufficientStatisticsBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region SufficientStatistics Bvt Test Cases

        /// <summary>
        /// validates the CompareTo method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSufficientStatisticsCompareTo()
        {
            SufficientStatistics suffStat = SufficientStatistics.Parse("1");
            SufficientStatistics suffStat2 = SufficientStatistics.Parse("2");
            Assert.AreEqual(-1, suffStat.CompareTo(suffStat2));
        }

        /// <summary>
        /// validates the TryParse method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSufficientStatisticsTryParse()
        {
            SufficientStatistics result = null;
            SufficientStatistics continousStat = SufficientStatistics.Parse("1");
            Assert.IsTrue(SufficientStatistics.TryParse("1.25658", out result));
            Assert.IsFalse(continousStat.Equals(result));
        }

        /// <summary>
        /// Validates the GetHashCode() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        [Ignore]
        public void ValidateSufficientStatisticsGetHashCode()
        {
            // Not used anymore - changes with each .NET release.
            SufficientStatistics boolStat1 = SufficientStatistics.Parse("1");
            Assert.AreEqual(372029325, boolStat1.GetHashCode());
        }

        #endregion SufficientStatistics Bvt Test Cases
    }
}
