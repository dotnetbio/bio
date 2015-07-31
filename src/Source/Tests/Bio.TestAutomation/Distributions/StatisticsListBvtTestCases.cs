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
    /// Bvt test cases for StatisticsList Class
    /// </summary>
    [TestClass]
    public class StatisticsListBvtTestCases
    {
        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static StatisticsListBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region StatisticsList Bvt Test Cases

        /// <summary>
        /// validates the Add method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateStatisticsListAdd()
        {
            StatisticsList statList = new StatisticsList();
            GaussianStatistics gaussStat = GaussianStatistics.GetInstance(2.33, 1.256, 7);
            DiscreteStatistics discreteStat = DiscreteStatistics.GetInstance(1);
            ContinuousStatistics contStat = ContinuousStatistics.GetInstance(2.333);
            MissingStatistics missingStat = MissingStatistics.GetInstance;
            BooleanStatistics boolStat = BooleanStatistics.GetInstance(true);
            statList.Add(gaussStat);
            statList.Add(discreteStat);
            statList.Add(contStat);
            statList.Add(missingStat);
            statList.Add(boolStat);
            Assert.AreEqual(5, statList.Count);
            SufficientStatistics result = null;
            foreach (SufficientStatistics stat in statList)
            {
                Assert.IsTrue(SufficientStatistics.TryParse(stat.ToString(), out result));
            }
        }

        /// <summary>
        /// validates the GetMissingInstance property
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateStatisticsListGetMissingInstance()
        {
            StatisticsList statList = StatisticsList.GetMissingInstance;
            Assert.IsTrue(statList.ElementAt(0).IsMissing());
        }

        /// <summary>
        /// validates the GetInstance method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateStatisticsListGetInstance()
        {
            GaussianStatistics gaussStat = GaussianStatistics.GetInstance(2.33, 1.256, 7);
            DiscreteStatistics discreteStat = DiscreteStatistics.GetInstance(1);
            StatisticsList statList = StatisticsList.GetInstance(gaussStat);
            statList.Add(discreteStat);
            SufficientStatistics result = null;
            foreach (SufficientStatistics stat in statList)
            {
                Assert.IsTrue(SufficientStatistics.TryParse(stat.ToString(), out result));
            }
        }

        /// <summary>
        /// validates the RemoveRange and Remove methods
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateStatisticsListRemoveRangeAndRemove()
        {
            StatisticsList statList = new StatisticsList();
            GaussianStatistics gaussStat = GaussianStatistics.GetInstance(2.33, 1.256, 7);
            DiscreteStatistics discreteStat = DiscreteStatistics.GetInstance(1);
            ContinuousStatistics contStat = ContinuousStatistics.GetInstance(2.333);
            MissingStatistics missingStat = MissingStatistics.GetInstance;
            BooleanStatistics boolStat = BooleanStatistics.GetInstance(true);
            statList.Add(gaussStat);
            statList.Add(discreteStat);
            statList.Add(contStat);
            statList.Add(missingStat);
            statList.Add(boolStat);
            Assert.AreEqual(5, statList.Count);
            statList.RemoveRange(3, 2);
            Assert.AreEqual(3, statList.Count);
            statList.Remove(2);
            Assert.AreEqual(2, statList.Count);
        }


        /// <summary>
        /// validates the SubSequence method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateStatisticsListSubSequence()
        {
            StatisticsList statList = new StatisticsList();
            GaussianStatistics gaussStat = GaussianStatistics.GetInstance(2.33, 1.256, 7);
            DiscreteStatistics discreteStat = DiscreteStatistics.GetInstance(1);
            ContinuousStatistics contStat = ContinuousStatistics.GetInstance(2.333);
            MissingStatistics missingStat = MissingStatistics.GetInstance;
            BooleanStatistics boolStat = BooleanStatistics.GetInstance(true);
            statList.Add(gaussStat);
            statList.Add(discreteStat);
            statList.Add(contStat);
            statList.Add(missingStat);
            statList.Add(boolStat);
            Assert.AreEqual(5, statList.Count);
            SufficientStatistics result = null;
            StatisticsList statListSubSequence = (StatisticsList)statList.SubSequence(0, 2);
            Assert.IsTrue(GaussianStatistics.TryParse(statListSubSequence.ElementAt(0).ToString(), out result));
            Assert.IsTrue(DiscreteStatistics.TryParse(statListSubSequence.ElementAt(1).ToString(), out result));
        }

        /// <summary>
        /// validates the TryParse method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateStatisticsListTryParse()
        {
            StatisticsList statList = new StatisticsList();
            GaussianStatistics gaussStat = GaussianStatistics.GetInstance(2.33, 1.256, 7);
            DiscreteStatistics discreteStat = DiscreteStatistics.GetInstance(1);
            ContinuousStatistics contStat = ContinuousStatistics.GetInstance(2.333);
            MissingStatistics missingStat = MissingStatistics.GetInstance;
            BooleanStatistics boolStat = BooleanStatistics.GetInstance(true);
            statList.Add(gaussStat);
            statList.Add(discreteStat);
            statList.Add(contStat);
            statList.Add(missingStat);
            statList.Add(boolStat);
            Assert.AreEqual(5, statList.Count);
            SufficientStatistics result = null;
            StatisticsList statListSubSequence = (StatisticsList)statList.SubSequence(0, 2);
            Assert.IsTrue(StatisticsList.TryParse(statList.ToString(), out result));
            Assert.IsTrue(StatisticsList.TryParse(statListSubSequence.ToString(), out result));
        }

        /// <summary>
        /// validates the ToString method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateStatisticsListToString()
        {
            StatisticsList statList = new StatisticsList();
            GaussianStatistics gaussStat = GaussianStatistics.GetInstance(2.33, 1.256, 7);
            DiscreteStatistics discreteStat = DiscreteStatistics.GetInstance(1);
            MissingStatistics missingStat = MissingStatistics.GetInstance;
            statList.Add(gaussStat);
            statList.Add(discreteStat);
            statList.Add(missingStat);
            string statString = statList.ToString();
            Assert.AreEqual(gaussStat.ToString() + ";" + discreteStat.ToString() + ";" + missingStat.ToString(), statString);
        }

        /// <summary>
        /// validates the Clone method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateStatisticsListClone()
        {
            StatisticsList statList = new StatisticsList();
            GaussianStatistics gaussStat = GaussianStatistics.GetInstance(2.33, 1.256, 7);
            DiscreteStatistics discreteStat = DiscreteStatistics.GetInstance(1);
            MissingStatistics missingStat = MissingStatistics.GetInstance;
            statList.Add(gaussStat);
            statList.Add(discreteStat);
            statList.Add(missingStat);
            StatisticsList clone = (StatisticsList)statList.Clone();
            Assert.AreEqual(statList, clone);
        }

        /// <summary>
        /// validates the Equals method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateStatisticsListEquals()
        {
            StatisticsList statList = new StatisticsList();
            GaussianStatistics gaussStat = GaussianStatistics.GetInstance(2.33, 1.256, 7);
            DiscreteStatistics discreteStat = DiscreteStatistics.GetInstance(1);
            MissingStatistics missingStat = MissingStatistics.GetInstance;
            statList.Add(gaussStat);
            statList.Add(discreteStat);
            statList.Add(missingStat);
            StatisticsList clone = (StatisticsList)statList.Clone();
            Assert.IsTrue(statList.Equals(clone));
        }

        /// <summary>
        /// Validates the AsGaussianStatistics() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateStatisticsListAsGaussianStatistics()
        {
            StatisticsList statList = new StatisticsList();
            GaussianStatistics gaussStat = GaussianStatistics.GetInstance(2.33, 1.256, 7);
            DiscreteStatistics discreteStat = DiscreteStatistics.GetInstance(1);
            MissingStatistics missingStat = MissingStatistics.GetInstance;
            statList.Add(gaussStat);
            statList.Add(discreteStat);
            statList.Add(missingStat);
            GaussianStatistics gStat = statList.AsGaussianStatistics();
            Assert.AreEqual("Missing", gStat.ToString());
        }

        /// <summary>
        /// Validates the AsContinuousStatistics() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateStatisticsListAsContinuousStatistics()
        {
            StatisticsList statList = new StatisticsList();
            GaussianStatistics gaussStat = GaussianStatistics.GetInstance(2.33, 1.256, 7);
            DiscreteStatistics discreteStat = DiscreteStatistics.GetInstance(1);
            MissingStatistics missingStat = MissingStatistics.GetInstance;
            statList.Add(gaussStat);
            statList.Add(discreteStat);
            statList.Add(missingStat);
            ContinuousStatistics cStat = statList.AsContinuousStatistics();
            Assert.IsTrue(cStat.IsMissing());
        }

        /// <summary>
        /// Validates the AsDiscreteStatistics() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateStatisticsListAsDiscreteStatistics()
        {
            StatisticsList statList = new StatisticsList();
            GaussianStatistics gaussStat = GaussianStatistics.GetInstance(2.33, 1.256, 7);
            DiscreteStatistics discreteStat = DiscreteStatistics.GetInstance(1);
            MissingStatistics missingStat = MissingStatistics.GetInstance;
            statList.Add(gaussStat);
            statList.Add(discreteStat);
            statList.Add(missingStat);
            DiscreteStatistics dStat = statList.AsDiscreteStatistics();
            Assert.AreEqual(statList, dStat.AsBooleanStatistics());
        }

        /// <summary>
        /// Validates the AsBooleanStatistics() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateStatisticsListAsBooleanStatistics()
        {
            StatisticsList statList = new StatisticsList();
            GaussianStatistics gaussStat = GaussianStatistics.GetInstance(2.33, 1.256, 7);
            DiscreteStatistics discreteStat = DiscreteStatistics.GetInstance(1);
            MissingStatistics missingStat = MissingStatistics.GetInstance;
            statList.Add(gaussStat);
            statList.Add(discreteStat);
            statList.Add(missingStat);
            BooleanStatistics dStat = statList.AsBooleanStatistics();
            Assert.AreEqual(statList, dStat.AsStatisticsList());
        }

        /// <summary>
        /// Validates the GetHashCode() method
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        [Ignore]
        public void ValidateStatisticsListGetHashCode()
        {
            StatisticsList statList = new StatisticsList();
            GaussianStatistics gaussStat = GaussianStatistics.GetInstance(2.33, 1.256, 7);
            DiscreteStatistics discreteStat = DiscreteStatistics.GetInstance(1);
            MissingStatistics missingStat = MissingStatistics.GetInstance;
            statList.Add(gaussStat);
            statList.Add(discreteStat);
            statList.Add(missingStat);
            Assert.AreEqual(-1032035030, statList.GetHashCode());
        }

        #endregion StatisticsList Bvt Test Cases
    }
}
