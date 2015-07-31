using System;
using System.IO;
using Bio.Util;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.TestAutomation.Util
{
    /// <summary>
    /// FileAccessComparer Bvt Test cases
    /// </summary>
    [TestClass]
    public class FileUtilsBvtTestCases
    {
        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region Bvt Test cases

        /// <summary>
        /// Validates OpenTextStripComments method of the FileUtils class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateOpenTextStripCommentsForFileName()
        {
            string filename = Path.GetTempFileName();
            StreamReader commentedStreamReader = null;

            // Create temp files
            using (File.Create(filename)) { }
            using (commentedStreamReader = FileUtils.OpenTextStripComments(filename))
            {
                Assert.IsNotNull(commentedStreamReader, "FileUtils BVT: FileUtils OpenTextStripComments Not validated successfully");
                ApplicationLog.WriteLine("FileUtils BVT: FileUtils OpenTextStripComments Validated successfully");
            }

            // Delete temp files
            File.Delete(filename);
        }

        /// <summary>
        /// Validates OpenTextStripComments method of the FileUtils class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateOpenTextStripCommentsForFileInfo()
        {
            string filename = Path.GetTempFileName();
            StreamReader commentedStreamReader = null;

            // Create temp files
            using (File.Create(filename)) { }
            FileInfo fileinfo = new FileInfo(filename);

            using (commentedStreamReader = FileUtils.OpenTextStripComments(fileinfo))
            {
                Assert.IsNotNull(commentedStreamReader, "FileUtils BVT: FileUtils OpenTextStripComments Not validated successfully");
                ApplicationLog.WriteLine("FileUtils BVT: FileUtils OpenTextStripComments Validated successfully");
            }

            // Delete temp files
            File.Delete(filename);
        }

        /// <summary>
        /// Validates StripComments method of the FileUtils class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateStripCommentsForStream()
        {
            string filename = Path.GetTempFileName();
            StreamReader commentedStreamReader = null;

            // Create temp files
            using (FileStream stream = File.Create(filename))
            {
                using (commentedStreamReader = FileUtils.StripComments(stream))
                {
                    Assert.IsNotNull(commentedStreamReader, "FileUtils BVT: FileUtils OpenTextStripComments Not validated successfully");
                    ApplicationLog.WriteLine("FileUtils BVT: FileUtils OpenTextStripComments Validated successfully");
                }
            }

            // Delete temp files
            File.Delete(filename);
        }

        /// <summary>
        /// Validates ReadLine method of the FileUtils class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateReadLineForFileInfo()
        {
            string filename = Path.GetTempFileName();
            string fileContent = null;

            // Create temp files
            using (File.Create(filename)) { }

            FileInfo fileinfo = new FileInfo(filename);
            fileContent = FileUtils.ReadLine(fileinfo);
            Assert.IsNull(fileContent, "FileUtils BVT: FileUtils ReadLine Not validated successfully");
            ApplicationLog.WriteLine("FileUtils BVT: FileUtils ReadLine Validated successfully");

            File.Delete(filename);
        }

        /// <summary>
        /// Validates ReadLine method of the FileUtils class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateReadLineForFileName()
        {
            string filename = Path.GetTempFileName();
            string fileContent = null;

            // Create temp files
            using (File.Create(filename)) { }
            fileContent = FileUtils.ReadLine(filename);

            Assert.IsNull(fileContent, "FileUtils BVT: FileUtils ReadLine Not validated successfully");
            ApplicationLog.WriteLine("FileUtils BVT: FileUtils ReadLine Validated successfully");

            File.Delete(filename);
        }

        /// <summary>
        /// Validates ReadEachLine method of the FileUtils class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateReadEachLineForFileName()
        {
            string filename = Path.GetTempFileName();
            int count = 0;
            bool result = true;

            // Create a string array that consists of three lines.
            string[] lines = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilTraceNode,
                Constants.SequenceTypeNode).Split(';');

            // Create temp files
            using (File.Create(filename)) { }
            File.WriteAllLines(filename, lines);

            foreach (string item in FileUtils.ReadEachLine(filename))
            {
                if (item != lines[count++])
                {
                    result = false;
                    break;
                }
            }

            Assert.IsTrue(result, "FileUtils BVT: FileUtils ReadEachLine Not validated successfully");
            ApplicationLog.WriteLine("FileUtils BVT: FileUtils ReadEachLine Validated successfully");
            File.Delete(filename);
        }

        /// <summary>
        /// Validates ReadEachLine method of the FileUtils class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateReadEachLineForFileInfo()
        {
            string filename = Path.GetTempFileName();
            int count = 0;
            bool result = true;

            // Create a string array that consists of three lines.
            string[] lines = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilTraceNode,
                Constants.SequenceTypeNode).Split(';');

            // Create temp files
            using (File.Create(filename)) { }
            File.WriteAllLines(filename, lines);
            FileInfo fileinfo = new FileInfo(filename);

            foreach (string item in FileUtils.ReadEachLine(fileinfo))
            {
                if (item != lines[count++])
                {
                    result = false;
                    break;
                }
            }

            Assert.IsTrue(result, "FileUtils BVT: FileUtils ReadEachLine Not validated successfully");
            ApplicationLog.WriteLine("FileUtils BVT: FileUtils ReadEachLine Validated successfully");
            File.Delete(filename);
        }

        /// <summary>
        /// Validates ReadEachLine method of the FileUtils class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateReadEachLineForTextReader()
        {
            string filename = Path.GetTempFileName();
            int count = 0;
            bool result = true;

            string[] lines = utilityObj.xmlUtil.GetTextValue(
                Constants.UtilTraceNode,
                Constants.SequenceTypeNode).Split(';');

            // Create temp files
            using (File.Create(filename)) { }
            File.WriteAllLines(filename, lines);
            using (TextReader reader = File.OpenText(filename))
            {
                foreach (string item in FileUtils.ReadEachLine(reader))
                {
                    if (item != lines[count++])
                    {
                        result = false;
                        break;
                    }
                }
            }

            Assert.IsTrue(result, "FileUtils BVT: FileUtils ReadEachLine Not validated successfully");
            ApplicationLog.WriteLine("FileUtils BVT: FileUtils ReadEachLine Validated successfully");
            File.Delete(filename);
        }

        #endregion Bvt Test cases
    }
}

