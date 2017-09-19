using System.IO;
using System.Xml.Serialization;
using Bio.IO.AppliedBiosystems;
using Bio.IO.AppliedBiosystems.DataParsers;

namespace Bio.Tests.Framework.IO.AppliedBiosystems
{
    /// <summary>
    /// These examples are used to validate the .abi parser.  This used the Applied Biosystems tool for converting abi to xml: 
    /// http://www.appliedbiosystems.com/absite/us/en/home/support/software-community/tools-for-accessing-files.html
    /// 
    /// The main caveat with this is it is very slow and requires a two step process to import into code (abi -> xml -> code).
    /// </summary>
    public static class Ab1Examples
    {
        /// <summary>
        /// Sample folder for files.
        /// </summary>
        public const string SampleFolder = "TestUtils\\AppliedBiosystems\\";

        /// <summary>
        /// Sample ab1 file, this is public from Applied Biosystems sample data set.
        /// </summary>
        public const string Ab1SampleBinaryFileName = SampleFolder + "A_2004-08-24_18-23-44_043630_A02_017_015.ab1";

        /// <summary>
        /// Sample file converted to xml.
        /// </summary>
        public const string Ab1SampleXmlFileName = SampleFolder + "A_2004-08-24_18-23-44_043630_A02_017_015.ab1.xml";

        /// <summary>
        /// Helper method to read an abi file and load its context.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static IParserContext GetRawData(string fileName)
        {
            using (Stream stream = new FileInfo(fileName).OpenRead())
            {
                var reader = new BinaryReader(stream);
                return Ab1Parser.Parse(reader);
            }
        }

        /// <summary>
        /// Helper method to deserialize an xml file representing a converted abi file to xml.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static AB_Root GetXmlData(string fileName)
        {
            using (Stream stream = new FileInfo(fileName).OpenRead())
            {
                return (AB_Root)new XmlSerializer(typeof(AB_Root)).Deserialize(stream);
            }
        }
    }
}
