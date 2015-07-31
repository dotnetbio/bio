using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Bio.IO.AppliedBiosystems;
using Bio.IO.AppliedBiosystems.DataParsers;
using NUnit.Framework;

namespace Bio.Tests.Framework.IO.AppliedBiosystems
{
    /// <summary>
    /// Validate abi parsing.
    /// </summary>
    [TestFixture]
    public class AbiTests
    {
        private static void ValidateDataItems(AB_Root xmlData, IParserContext rawData)
        {
            var validator = new Ab1DataValidator(xmlData.Items.OfType<AB_RootData>().First().Tag);
            rawData.DataItems.ForEach(item => item.Accept(validator));
        }

        private static void ValidateHeader(AB_Root xmlData, IParserContext rawData)
        {
            AB_RootHeader xmlHeader = xmlData.Items.OfType<AB_RootHeader>().First();

            Assert.AreEqual(xmlHeader.Version, rawData.Header.Version.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(int.Parse(xmlHeader.Directory_Elements, CultureInfo.InvariantCulture), rawData.Header.DirectoryEntries.Count);
            Assert.AreEqual(xmlHeader.ByteOrder, rawData.Header.FileSignature);
            // The xml converter messes up the header dir information and reverses the tag name (BUGS!)
            Assert.AreEqual(xmlHeader.Directory_Tag_Name,
                            new string(rawData.Header.DirectoryEntryDefinition.TagName.Reverse().ToArray()));
            Assert.AreEqual(xmlHeader.Directory_Tag_Number,
                            rawData.Header.DirectoryEntryDefinition.TagNumber.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(xmlHeader.Directory_Elements,
                            rawData.Header.DirectoryEntryDefinition.ElementCount.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Verifies the parser can read abi file.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestParser()
        {
            Ab1Examples.GetRawData(Ab1Examples.Ab1SampleBinaryFileName);
        }

        /// <summary>
        /// The xml document representing the binary ab1 file was generated using the applied biosystems data file
        /// converter.  We user this to verify that our binary parsing is accurate.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateBinaryParserVsXml()
        {
            IParserContext rawData = Ab1Examples.GetRawData(Ab1Examples.Ab1SampleBinaryFileName);
            AB_Root xmlData = Ab1Examples.GetXmlData(Ab1Examples.Ab1SampleXmlFileName);

            ValidateHeader(xmlData, rawData);
            ValidateDataItems(xmlData, rawData);
        }

        /// <summary>
        /// Test converting abi context to sequence.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestAb1ToSequence()
        {
            var parser = new Ab1Parser();
            using (parser.Open(Ab1Examples.Ab1SampleBinaryFileName))
            {
                ISequence sequence = parser.Parse().First();
                Trace.WriteLine(sequence);
            }
        }
    }
}
