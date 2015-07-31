/****************************************************************************
 * OutputFile.cs
 * 
 * This file contains the OutputFile BVT test cases.
 * 
******************************************************************************/

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Algorithms.Assembly.Padena;
using Bio.Util;
using Bio.IO.GenBank;
using Bio.Util.ArgumentParser;
using Bio.Util.Logging;
using System.IO;

namespace Bio.TestAutomation.BioUtil.ArgumentParser
{
    /// <summary>
    /// Bvt test cases for the OutputFile class
    /// </summary>
    [TestClass]
    public class OutputFileBvtTestCases
    {
        #region Bvt Test cases

        /// <summary>
        /// Validates FinalizeParse method of the OutputFile class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateFinalizeParse()
        {
            OutputFile outputFile = new OutputFile();
            string filename = Path.GetTempFileName();

            // Create temp files
            using (File.Create(filename)) { }

            try
            {
                outputFile.FullName = filename;
                outputFile.FinalizeParse();
                ApplicationLog.WriteLine("OutputFile BVT: OutputFile FinalizeParse Validated successfully");
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "OutputFile BVT: OutputFile FinalizeParse Validated successfully:", ex.Message));
                Assert.Fail("OutputFile BVT: OutputFile FinalizeParse Not validated successfully");
            }
        }

        /// <summary>
        /// Validates Operator FileInfo of the OutputFile class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateOperatorFileInfo()
        {
            OutputFile outputFile = new OutputFile();

            string filename = Path.GetTempFileName();
            using (File.Create(filename)) { };
            FileInfo fileinfo = new FileInfo(filename);

            outputFile = fileinfo;

            try
            {
                fileinfo = outputFile;
                Assert.IsNotNull(fileinfo, "OutputFile BVT: OutputFile Operator FileInfo Validated successfully");
                outputFile = null;
                fileinfo = outputFile;
                Assert.IsNull(fileinfo, "OutputFile BVT: OutputFile Operator FileInfo Validated successfully");
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "OutputFile BVT: OutputFile FileInfo not validated successfully:", ex.Message));
                Assert.Fail("OutputFile BVT: OutputFile FileInfo not validated successfully");
            }
        }

        /// <summary>
        /// Validates Operator OutputFile of the OutputFile class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateOperatorOutputFile()
        {
            OutputFile outputFile = null;
            string filename = Path.GetTempFileName();
            using (File.Create(filename)) { };
            FileInfo fileinfo = new FileInfo(filename); 

            try
            {
                outputFile = fileinfo;
                Assert.IsNotNull(outputFile, "OutputFile BVT: OutputFile Operator OutputFile Validated successfully");
                fileinfo = null;
                outputFile = fileinfo;
                Assert.IsNull(outputFile, "OutputFile BVT: OutputFile Operator OutputFile Validated successfully");
            }
            catch (Exception ex)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "OutputFile BVT: OutputFile OutputFile not validated successfully:", ex.Message));
                Assert.Fail("OutputFile BVT: OutputFile OutputFile not validated successfully");
            }
        }

        #endregion Bvt Test cases
    }
}
