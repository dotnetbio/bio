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
    /// Bvt Test Cases For BooleanStatistics Class
    /// </summary>
    [TestClass]
    public class BooleanStatisticsBvtTestCases
    {
        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static BooleanStatisticsBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region BooleanStatistics Bvt Test Cases

        /// <summary>
        /// validates the GetMissingInstance property
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateBooleanStatisticsGetMissingInstance()
        {
            BooleanStatistics boolStat = BooleanStatistics.GetMissingInstance;
            Assert.IsTrue(boolStat.IsMissing());
        }

        /// <summary>
        /// validates the Equals method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateBooleanStatisticsEquals()
        {
            SufficientStatistics suffStat = BooleanStatistics.GetInstance(false);
            BooleanStatistics boolStat = BooleanStatistics.GetMissingInstance;
            Assert.IsFalse(boolStat.Equals(suffStat));

            suffStat = (SufficientStatistics)boolStat;
            Assert.IsTrue(boolStat.Equals(suffStat));

            boolStat = BooleanStatistics.GetInstance(true);
            suffStat = (SufficientStatistics)boolStat;
            Assert.IsTrue(boolStat.Equals(suffStat));

            boolStat = BooleanStatistics.GetInstance(false);
            suffStat = (SufficientStatistics)boolStat;
            Assert.IsTrue(boolStat.Equals(suffStat));

            object suffStatObj = (object)boolStat;
            Assert.IsTrue(boolStat.Equals(suffStat));
        }

        /// <summary>
        /// validates the TryParse method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateBooleanStatisticsTryParse()
        {
            SufficientStatistics result = null;
            BooleanStatistics boolStat = BooleanStatistics.GetInstance(true);
            Assert.IsTrue(BooleanStatistics.TryParse("true", out result));
            Assert.IsTrue(boolStat.Equals(result));
        }

        /// <summary>
        /// validates the constants of BooleanStatistics class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateBooleanStatisticsConstants()
        {
            Assert.AreEqual(-1, BooleanStatistics.Missing);
            Assert.AreEqual(0, BooleanStatistics.False);
            Assert.AreEqual(1, BooleanStatistics.True);
        }

        /// <summary>
        /// validates the ToString method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateBooleanStatisticsToString()
        {
            BooleanStatistics boolStat1 = BooleanStatistics.GetInstance(true);
            BooleanStatistics boolStat2 = BooleanStatistics.GetMissingInstance;
            Assert.AreEqual("1", boolStat1.ToString());
            Assert.AreEqual("-1", boolStat2.ToString());
        }

        /// <summary>
        /// Validates the AsStatisticsList() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateBooleanStatisticsAsStatisticsList()
        {
            BooleanStatistics boolStat1 = BooleanStatistics.GetInstance(true);
            StatisticsList sList = boolStat1.AsStatisticsList();
            Assert.AreEqual(1, sList.Count);
        }

        /// <summary>
        /// Validates the AsGaussianStatistics() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateBooleanStatisticsAsGaussianStatistics()
        {
            BooleanStatistics boolStat1 = BooleanStatistics.GetInstance(true);
            GaussianStatistics gStat = boolStat1.AsGaussianStatistics();
            Assert.AreEqual(boolStat1, gStat.AsBooleanStatistics());
        }

        /// <summary>
        /// Validates the AsContinuousStatistics() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateBooleanStatisticsAsContinuousStatistics()
        {
            BooleanStatistics boolStat1 = BooleanStatistics.GetInstance(true);
            ContinuousStatistics cStat = boolStat1.AsContinuousStatistics();
            Assert.AreEqual(1, cStat.Value);
        }

        /// <summary>
        /// Validates the AsDiscreteStatistics() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateBooleanStatisticsAsDiscreteStatistics()
        {
            BooleanStatistics boolStat1 = BooleanStatistics.GetInstance(true);
            DiscreteStatistics dStat = boolStat1.AsDiscreteStatistics();
            Assert.AreEqual(boolStat1, dStat.AsBooleanStatistics());
        }

        /// <summary>
        /// Validates the GetHashCode() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        [Ignore]
        public void ValidateBooleanStatisticsGetHashCode()
        {
            // This is a stupid test.  It likely changes on each release of the .NET framework since 
            // we are using an integer as our value here.  So it broke for .NET 4.5, I am simply removing
            // it because it doesn't test anything anyway.
            BooleanStatistics boolStat1 = BooleanStatistics.GetInstance(true);
            Assert.AreEqual(372029325, boolStat1.GetHashCode());
        }

        /// <summary>
        /// Validates the ConvertToBooleanStatistics() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateBooleanStatisticsConvertToBooleanStatistics()
        {
            BooleanStatistics boolStat1 = BooleanStatistics.GetInstance(true);
            SufficientStatistics suffStats = boolStat1;
            Dictionary<string, SufficientStatistics> testSuffStats = new Dictionary<string, SufficientStatistics>
                                                                         {
                                                                             { "Test", suffStats }
                                                                         };

            Dictionary<string, BooleanStatistics> boolStats = BooleanStatistics.ConvertToBooleanStatistics(testSuffStats);
            Assert.AreEqual(boolStat1, boolStats["Test"]);
        }

        /// <summary>
        /// Validates the BooleanStatistics(Discrete) method
        /// </summary>
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateBooleanStatisticsExplicitDiscrete()
        {
            DiscreteStatistics discreteStat2 = DiscreteStatistics.GetInstance(5);
            BooleanStatistics boolStat2 = (BooleanStatistics)discreteStat2;
            Assert.AreEqual(5, (int)(DiscreteStatistics)boolStat2);

            DiscreteStatistics discreteStat = DiscreteStatistics.GetMissingInstance;
            BooleanStatistics boolStat1 = (BooleanStatistics)discreteStat;
            Assert.AreEqual(discreteStat, (DiscreteStatistics)boolStat1);

            DiscreteStatistics discreteStat3 = null;
            try
            {
                BooleanStatistics boolStat3 = (BooleanStatistics)discreteStat3;
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                ApplicationLog.WriteLine("BooleanStatistics : Successfully validated the BooleanStatistics(Discrete)");
            }
        }

        /// <summary>
        /// Validates the int(BooleanStatistics) method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateBooleanStatisticsExplicitInt()
        {
            DiscreteStatistics discreteStat2 = DiscreteStatistics.GetInstance(5);
            BooleanStatistics boolStat2 = (BooleanStatistics)discreteStat2;
            Assert.AreEqual(5, (int)boolStat2);
            Assert.IsFalse((bool)boolStat2);
            try
            {

                DiscreteStatistics discreteStat = DiscreteStatistics.GetMissingInstance;
                BooleanStatistics boolStat1 = (BooleanStatistics)discreteStat;
                Assert.IsFalse((bool)boolStat1);
                int temp = (int)boolStat1;
            }
            catch (InvalidCastException)
            {
                ApplicationLog.WriteLine("BooleanStatistics : Successfully validated the int(0)");
            }

            DiscreteStatistics discreteStat3 = null;
            try
            {
                BooleanStatistics boolStat3 = (BooleanStatistics)discreteStat3;
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                ApplicationLog.WriteLine("BooleanStatistics : Successfully validated the int(booleanstatistics)");
            }
        }

        /// <summary>
        /// Validates the BooleanStatistics(MissingStatistics) method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateBooleanStatisticsExplicitMissingStatistics()
        {
            MissingStatistics missStat = MissingStatistics.GetInstance;
            BooleanStatistics boolStat2 = (BooleanStatistics)missStat;
            Assert.IsTrue(boolStat2.IsMissing());

            MissingStatistics missStat2 = null;
            Assert.IsNull((BooleanStatistics)missStat2);
        }

        #endregion BooleanStatistics Bvt Test Cases
    }
}
