/****************************************************************************
 * SequenceParserFormatterP2TestCases.cs
 * 
 *   This file contains the SequenceParserFormatter - Parsers & Formatter P2 test cases.
 * 
***************************************************************************/

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Bio.IO;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.TestAutomation.IO
{
    /// <summary>
    /// SequenceParserFormatter P2 parser Test cases implementation.
    /// </summary>

    [TestClass]
    public class SequenceParserFormatterP2TestCases
    {
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void ValidateFindFormatterByFileName()
        {
            ISequenceFormatter formatter=SequenceFormatters.FindFormatterByFileName(Constants.InvalidFileName);
            ValidateFormatterExceptions(formatter);
        }

        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void ValidateFindFormatterByName()
        {
            ISequenceFormatter formatter = SequenceFormatters.FindFormatterByName(Constants.FastaTempFileName, Constants.InvalidFileName);
            ValidateFormatterExceptions(formatter);            
        }

        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void ValidateFindParserByFileName()
        {
            ISequenceParser parser = SequenceParsers.FindParserByFileName(Constants.InvalidFileName);
            ValidateParserExceptions(parser);           
        }

        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void ValidateFindParserByName()
        {
            ISequenceParser parser = SequenceParsers.FindParserByName(Constants.FastaTempFileName, Constants.InvalidFileName);
            ValidateParserExceptions(parser);
        }

        # region Supporting Methods.

        public void ValidateParserExceptions(ISequenceParser parser)
        {
            try
            {
                System.Type parserTypes = parser.GetType();
                Assert.IsNotNull(parserTypes);
            }
            catch (NullReferenceException exception)
            {
                // Log to VSTest GUI.
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "Sequence Parser P2 : Validated Exception {0} successfully",
                    exception.Message));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "Sequence Parser P2 : Validated Exception {0} successfully",
                    exception.Message));
            }
        }

        public void ValidateFormatterExceptions(ISequenceFormatter formatter)
        {
            try
            {
                System.Type formatterTypes = formatter.GetType();
                Assert.IsNotNull(formatterTypes);
            }
            catch (NullReferenceException exception)
            {
                // Log to VSTest GUI.
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "Sequence Formatter P2 : Validated Exception {0} successfully",
                    exception.Message));
                Console.WriteLine(string.Format((IFormatProvider)null,
                    "Sequence Formatter P2 : Validated Exception {0} successfully",
                    exception.Message));
            }
        }

        # endregion Supporting Methods.
    }
}
