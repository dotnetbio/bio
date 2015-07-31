using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Distributions.Converters;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using Bio.Distributions;
using Bio.Util;

namespace Bio.TestAutomation.Distributions.Converters
{
    /// <summary>
    /// Bvt Test Cases for Converters
    /// </summary>
    [TestClass]
    public class ConvertersBvtTestCases
    {
        /// <summary>
        /// Validates StringToDoubleConverter
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateStringToDoubleConverter()
        {
            StringToDoubleConverter stdconverter = new StringToDoubleConverter();
            string str = stdconverter.ConvertBackward(1.234);
            Assert.AreEqual("1.234", str);
            double doubleVal = stdconverter.ConvertForward("1.234");
            Assert.AreEqual(1.234d, doubleVal);
        }

        /// <summary>
        /// Validates CharToSufficientStatisticsConverter
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateCharToSufficientStatisticsConverter()
        {
            CharToSufficientStatisticsConverter ctssConverter = new CharToSufficientStatisticsConverter();
            DiscreteStatistics discreteStat = DiscreteStatistics.GetInstance(1);
            char convertedChar = ctssConverter.ConvertBackward(discreteStat);
            Assert.AreEqual('1', convertedChar);
            BooleanStatistics convertedBooleanStat = ctssConverter.ConvertForward('1') as BooleanStatistics;
            Assert.AreEqual("1", convertedBooleanStat.ToString());
        }

        /// <summary>
        /// Validates IntToSufficientStatisticsConverter
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateIntToSufficientStatisticsConverter()
        {
            IntToSufficientStatisticsConverter itssConverter = new IntToSufficientStatisticsConverter();
            DiscreteStatistics discreteStat = DiscreteStatistics.GetInstance(1);
            int convertedInt = itssConverter.ConvertBackward(discreteStat);
            Assert.AreEqual(1, convertedInt);
            DiscreteStatistics convertedDiscreteStat = itssConverter.ConvertForward('1') as DiscreteStatistics;
            Assert.IsNotNull(convertedDiscreteStat);
        }

        /// <summary>
        /// Validates ValueConverters static members
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateValueConverters()
        {
            ValueConverter<string, double> vcStrDouble = ValueConverters.StringToDoubleConverter;
            ValueConverter<int, SufficientStatistics> vcIntSS = ValueConverters.IntToSufficientStatistics;
            ValueConverter<SufficientStatistics, int> vcSSInt = ValueConverters.SufficientStatisticsToInt;
            ValueConverter<char, SufficientStatistics> vcCharSS = ValueConverters.CharToSufficientStatistics;
            ValueConverter<SufficientStatistics, char> vcSSChar = ValueConverters.SufficientStatisticsToChar;
            Assert.IsNotNull(vcStrDouble);
            Assert.IsNotNull(vcIntSS);
            Assert.IsNotNull(vcSSInt);
            Assert.IsNotNull(vcCharSS);
            Assert.IsNotNull(vcSSChar);
        }

    }
}
