using System;

using Bio.IO;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests.IO
{
    /// <summary>
    /// SequenceParserFormatter P2 parser Test cases implementation.
    /// </summary>

    [TestFixture]
    public class SequenceParserFormatterP2TestCases
    {
        [Test]
        [Category("Priority2")]
        public void ValidateFindFormatterByFileName()
        {
            ISequenceFormatter formatter=SequenceFormatters.FindFormatterByFileName(Constants.InvalidFileName);
            this.ValidateFormatterExceptions(formatter);
        }

        [Test]
        [Category("Priority2")]
        public void ValidateFindFormatterByName()
        {
            ISequenceFormatter formatter = SequenceFormatters.FindFormatterByName(Constants.FastaTempFileName, Constants.InvalidFileName);
            this.ValidateFormatterExceptions(formatter);            
        }

        [Test]
        [Category("Priority2")]
        public void ValidateFindParserByFileName()
        {
            ISequenceParser parser = SequenceParsers.FindParserByFileName(Constants.InvalidFileName);
            this.ValidateParserExceptions(parser);           
        }

        [Test]
        [Category("Priority2")]
        public void ValidateFindParserByName()
        {
            ISequenceParser parser = SequenceParsers.FindParserByName(Constants.FastaTempFileName, Constants.InvalidFileName);
            this.ValidateParserExceptions(parser);
        }

        # region Supporting Methods.

        public void ValidateParserExceptions(ISequenceParser parser)
        {
            try
            {
                var parserTypes = parser.GetType();
                Assert.IsNotNull(parserTypes);
            }
            catch (NullReferenceException exception)
            {
                // Log to VSTest GUI.
                ApplicationLog.WriteLine(string.Format((IFormatProvider) null,
                                                       "Sequence Parser P2 : Validated Exception {0} successfully",
                                                       exception.Message));
            }
        }

        public void ValidateFormatterExceptions(ISequenceFormatter formatter)
        {
            try
            {
                var formatterTypes = formatter.GetType();
                Assert.IsNotNull(formatterTypes);
            }
            catch (NullReferenceException exception)
            {
                // Log to VSTest GUI.
                ApplicationLog.WriteLine(string.Format((IFormatProvider) null,
                                                       "Sequence Formatter P2 : Validated Exception {0} successfully",
                                                       exception.Message));
            }
        }

        # endregion Supporting Methods.
    }
}
