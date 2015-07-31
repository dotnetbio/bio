using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Util;

namespace Bio.TestAutomation.Util
{
    /// <summary>
    /// Bvt Test Cases for NamedStreamCreator
    /// </summary>
    [TestClass]
    public class NamedStreamCreatorBvtTestCases
    {
        /// <summary>
        /// Validates 
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateNamedStreamCreator()
        {
            string fileName = "temp.txt";
            NamedStreamCreator creator = new NamedStreamCreator("FileStreamName", 
                () => new FileStream(fileName, FileMode.Open, FileAccess.Read));
            Assert.AreEqual("FileStreamName", creator.Name);
            Assert.IsNotNull(creator.Creator);
        }

        /// <summary>
        /// Validates INamedStreamCreatorExtensions.ToNamedStreamCreatorFromFileName
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateToNamedStreamCreatorFromFileName()
        {
            string streamname = "StreamName";
            INamedStreamCreator creator =
                INamedStreamCreatorExtensions.ToNamedStreamCreatorFromFileName("tmp.txt", streamname);
            Assert.AreEqual(streamname, creator.Name);
            Assert.IsNotNull(creator.Creator);
        }

        /// <summary>
        /// Validates INamedStreamCreatorExtensions.ToNamedStreamCreator
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateToNamedStreamCreator()
        {
            string fileName = "tmp.txt";
            string streamname = "StreamName";
            FileInfo fileinfo = new FileInfo(fileName);
            INamedStreamCreator creator =
               INamedStreamCreatorExtensions.ToNamedStreamCreator(fileinfo, streamname);
            Assert.AreEqual(streamname, creator.Name);
            Assert.IsNotNull(creator.Creator);
        }

        /// <summary>
        /// Validates INamedStreamCreatorExtensions.ToNamedStreamCreator with manifest resource name
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateToNamedStreamCreatorWithManifestResourceName()
        {
            string fileName = "tmp.txt";
            string streamname = "StreamName";
            string resourcename = "FileStream";
            FileInfo fileinfo = new FileInfo(fileName);
            INamedStreamCreator creator =
               INamedStreamCreatorExtensions.ToNamedStreamCreator(SpecialFunctions.GetEntryOrCallingAssembly(), resourcename, streamname);
            Assert.AreEqual(streamname, creator.Name);
            Assert.IsNotNull(creator.Creator);
        }

        /// <summary>
        /// Validates INamedStreamCreatorExtensions.ToNamedStreamCreatorFromString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateToNamedStreamCreatorFromString()
        {
            string strstream = "this is a test string";
            string streamname = "StreamName";
            INamedStreamCreator creator =
               INamedStreamCreatorExtensions.ToNamedStreamCreatorFromString(strstream, streamname);
            Assert.AreEqual(streamname, creator.Name);
            Assert.IsNotNull(creator.Creator);
        }

        /// <summary>
        /// Validates INamedStreamCreatorExtensions.ReadEachUncommentedLine
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateReadEachUncommentedLine()
        {
            string fileName = Path.GetTempFileName();
            string textToWrite = string.Empty;
            Random random = new Random(1);
            for (int i = 0; i < 5; i++)
            {
                textToWrite += random.Next().ToString((IFormatProvider)null) + "\n";
            }
            using (File.Create(fileName)) { }
            File.WriteAllText(fileName, textToWrite);
            FileInfo fileinfo = new FileInfo(fileName);
            INamedStreamCreator creator = INamedStreamCreatorExtensions.ToNamedStreamCreator(fileinfo, "Stream");
            IEnumerable<string> lines =  INamedStreamCreatorExtensions.ReadEachUncommentedLine(creator);
            Assert.IsNotNull(lines);
            Assert.AreEqual(5, lines.Count());
            foreach (string line in lines)
            {
                double num;
                Assert.IsTrue(double.TryParse(line, out num));
            }
        }

        /// <summary>
        /// Validates INamedStreamCreatorExtensions.ReadEachLine
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateReadEachLine()
        {
            string fileName = Path.GetTempFileName();
            string textToWrite = string.Empty;
            Random random = new Random(1);
            for (int i = 0; i < 5; i++)
            {
                textToWrite += random.Next().ToString((IFormatProvider)null) + "\n";
            }
            using (File.Create(fileName)) { }
            File.WriteAllText(fileName, textToWrite);
            FileInfo fileinfo = new FileInfo(fileName);
            INamedStreamCreator creator = INamedStreamCreatorExtensions.ToNamedStreamCreator(fileinfo, "Stream");
            IEnumerable<string> lines = INamedStreamCreatorExtensions.ReadEachLine(creator);
            Assert.IsNotNull(lines);
            Assert.AreEqual(5, lines.Count());
            foreach (string line in lines)
            {
                double num;
                Assert.IsTrue(double.TryParse(line, out num));
            }
        }

        /// <summary>
        /// Validates INamedStreamCreatorExtensions.ReadUncommentedLine
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateReadUncommentedLine()
        {
            string fileName = Path.GetTempFileName();
            string textToWrite = string.Empty;
            Random random = new Random(1);
            for (int i = 0; i < 5; i++)
            {
                textToWrite += random.Next().ToString((IFormatProvider)null) + "\n";
            }
            using (File.Create(fileName)) { }
            File.WriteAllText(fileName, textToWrite);
            FileInfo fileinfo = new FileInfo(fileName);
            INamedStreamCreator creator = INamedStreamCreatorExtensions.ToNamedStreamCreator(fileinfo, "Stream");
            string line = INamedStreamCreatorExtensions.ReadUncommentedLine(creator);
            Assert.IsNotNull(line);
            double num;
            Assert.IsTrue(double.TryParse(line, out num));
        }

        /// <summary>
        /// Validates INamedStreamCreatorExtensions.ReadToEnd
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateReadToEnd()
        {
            string fileName = Path.GetTempFileName();
            string textToWrite = string.Empty;
            Random random = new Random(1);
            for (int i = 0; i < 5; i++)
            {
                textToWrite += random.Next().ToString((IFormatProvider)null) + "\n";
            }
            using (File.Create(fileName)) { }
            File.WriteAllText(fileName, textToWrite);
            FileInfo fileinfo = new FileInfo(fileName);
            INamedStreamCreator creator = INamedStreamCreatorExtensions.ToNamedStreamCreator(fileinfo, "Stream");
            string line = INamedStreamCreatorExtensions.ReadToEnd(creator);
            Assert.IsNotNull(line);
            Assert.AreEqual(textToWrite, line);
        }

        /// <summary>
        /// Validates INamedStreamCreatorExtensions.WriteToStream
        /// </summary>
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateWriteToStream()
        {
            string fileName = Path.GetTempFileName();
            string textToWrite = string.Empty;
            Random random = new Random(1);
            for (int i = 0; i < 5; i++)
            {
                textToWrite += random.Next().ToString((IFormatProvider)null) + "\n";
            }
            using (File.Create(fileName)) { }
            //File.WriteAllText(fileName, textToWrite);
            FileInfo fileinfo = new FileInfo(fileName);
            INamedStreamCreator creator = INamedStreamCreatorExtensions.ToNamedStreamCreator(fileinfo, "Stream");
            string fileName1 = Path.GetTempFileName();
            using (FileStream stream = new FileStream(fileName1, FileMode.Create))
            {
                INamedStreamCreatorExtensions.WriteToStream(creator, stream);
            }
            string line = INamedStreamCreatorExtensions.ReadToEnd(creator);
            Assert.IsNotNull(line);
            Assert.AreEqual(textToWrite, line);
        }

    }
}
