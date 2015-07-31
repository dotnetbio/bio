/****************************************************************************
 * SpecialFunctions.cs
 * 
 * This file contains the SpecialFunctions BVT test cases.
 * 
******************************************************************************/
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Util;
using System.Reflection;
using System.Globalization;
using Bio.TestAutomation.Util;

namespace Bio.TestAutomation.Util
{
    /// <summary>
    /// Bvt Test Cases for SpecialFunctions class
    /// </summary>
    [TestClass]
    public class SpecialFunctionsBvtTestCases
    {
        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #endregion Global Variables

        #region BVT Test Cases

        
        /// <summary>
        /// Validates SpecialFunctions.GetEntryOrCallingAssembly
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetEntryOrCallingAssembly()
        {
            string assemblyname = utilityObj.xmlUtil.GetTextValue(Constants.CallingAssemblyName,
                Constants.CallingAssemblyFullNode);
            Assembly assembly =  SpecialFunctions.GetEntryOrCallingAssembly();
            Assert.IsTrue(assembly.FullName.Contains(assemblyname));
        }

        /// <summary>
        /// Validates SpecialFunctions.VersionToDate
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateVersionToDate()
        {
            Assembly assembly = SpecialFunctions.GetEntryOrCallingAssembly();
            Version version = new Version(1, 0, 0, 0);
            DateTime versionDate = SpecialFunctions.VersionToDate(version);
            Assert.IsNotNull(versionDate);
        }

        /// <summary>
        /// Validates SpecialFunctions.CopyDirectory
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateCopyDirectory()
        {
            string testDir = Path.GetTempPath() + "dir1\\";
            string testDirNew = Path.GetTempPath() +"dir2\\";
            string testFileName = Path.GetTempFileName().Split('\\').Last();
            string testFileName2 = Path.GetTempFileName().Split('\\').Last();
            Directory.CreateDirectory(testDir);
            using (File.Create(testDir + "\\" + testFileName)) { }
            using (File.Create(testDir + "\\" + testFileName2)) { }
            SpecialFunctions.CopyDirectory(testDir, testDirNew, true, true);
            Assert.IsTrue(File.Exists(testDirNew + "\\" + testFileName));
            Assert.IsTrue(File.Exists(testDirNew + "\\" + testFileName2));
            Directory.Delete(testDir, true);
            Directory.Delete(testDirNew, true);
        }

        /// <summary>
        /// Validates SpecialFunctions.DateProgramWasCompiled
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateDateProgramWasCompiled()
        {
            DateTime date = SpecialFunctions.DateProgramWasCompiled();
            Assert.IsNotNull(date);
        }

        /// <summary>
        /// Validates SpecialFunctions.DivideWork
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateDivideWork()
        {
            RangeCollection rangeColl = new RangeCollection(1, 500);
            long[] arr1 = rangeColl.ToArray();
            long pieceCount = 2;
            long batchCount = 2;
            RangeCollection skipListOrNull = new RangeCollection(1, 3);
            IEnumerable<KeyValuePair<long, long>> keyValuePair = SpecialFunctions.DivideWork<long>(arr1, rangeColl, pieceCount, batchCount, skipListOrNull);
            Assert.AreEqual(248, keyValuePair.Count());
        }
        #endregion BVT Test Cases
    }
}
