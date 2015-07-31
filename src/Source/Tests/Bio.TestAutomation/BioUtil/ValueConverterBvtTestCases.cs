/****************************************************************************
 * ValueConverter.cs
 * 
 * This file contains the ValueConverter BVT test cases.
 * 
******************************************************************************/


using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Util;
namespace Bio.TestAutomation.Util
{
    /// <summary>
    /// Bvt Test cases for ValueConverter
    /// </summary>
    [TestClass]
    public class ValueConverterBvtTestCases
    {
        /// <summary>
        /// Validates IntToChar
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateValueConverterIntToChar()
        {

            ValueConverter<int, char> intChar = ValueConverter.IntToChar;
            int x = intChar.ConvertBackward('0');
            char y = intChar.ConvertForward(1);
            Assert.AreEqual(0, x);
            Assert.AreEqual('1', y);

        }

        /// <summary>
        /// Validates CharToInt
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateValueConverterCharToInt()
        {
            ValueConverter<char, int> charInt = ValueConverter.CharToInt;
            char x = charInt.ConvertBackward(3);
            int y = charInt.ConvertForward('4');
            Assert.AreEqual('3', x);
            Assert.AreEqual(4, y);
        }

        /// <summary>
        /// Validates CharToDoubleWithLimitsConverter
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateCharToDoubleWithLimitsConverter()
        {
            int maxValue = 10;
            CharToDoubleWithLimitsConverter cdwlc = new CharToDoubleWithLimitsConverter(maxValue);
            char c = cdwlc.ConvertBackward(9);
            double d = cdwlc.ConvertForward('8');
            Assert.AreEqual('9', c);
            Assert.AreEqual(8, d);
        }

    }
}
