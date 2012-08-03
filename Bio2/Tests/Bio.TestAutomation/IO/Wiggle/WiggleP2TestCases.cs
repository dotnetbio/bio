/****************************************************************************
 * WiggleP2TestCases.cs
 * 
 *   This file contains the Wiggle - Parsers and Formatters P2 test cases.
 * 
***************************************************************************/

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

using Bio.IO.Wiggle;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

namespace Bio.TestAutomation.IO.Wiggle
{
    /// <summary>
    /// Wiggle P2 test cases implementation.
    /// </summary>
    [TestClass]
    public class WiggleP2TestCases
    {
        #region Global Variables.

        Utility utilityObj = new Utility(@"TestUtils\WiggleTestConfig.xml");

        #endregion Global Variables.

        # region Enum

        /// <summary>
        /// Enum to determine Exception type
        /// </summary>
        public enum ExceptionType
        {
            NullParser,
            NullFormatter,
            NullAnnotationWithData,
            NullAnnotationWithChromosomeName,            
        };

        #endregion Enum

        # region Test cases

        /// <summary>
        /// Invalidate Parser by passing null to Parse method.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InValidateWiggleParser()
        {
            ValidateArgumentNullException(ExceptionType.NullParser);            
        }

        /// <summary>
        /// Invalidate Formatter by passing null to Write method.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InValidateWiggleFormatter()
        {
            ValidateArgumentNullException(ExceptionType.NullFormatter);            
        }

        /// <summary>
        /// Invalidate Annotations by passing null Data.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InValidateWiggleAnnotation()
        {
            ValidateArgumentNullException(ExceptionType.NullAnnotationWithData);
        }

        /// <summary>
        /// Invalidate Annotations by passing null as Chromosome Name..
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InValidateWiggleAnnotationWithoutChromosomeName()
        {
            ValidateArgumentNullException(ExceptionType.NullAnnotationWithChromosomeName);
        }

        /// <summary>
        /// Invalidate Wiggle Parser by passing Invalid Track.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InValidateWiggleParserWithInvalidTrack()
        {
            ValidateFormatException(Constants.InvalidTrackNode);
        }

        /// <summary>
        /// Invalidate Wiggle Parser by passing Invalid AutoScale.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InValidateWiggleParserWithInvalidAutoScale()
        {
            ValidateFormatException(Constants.InvalidAutoScaleNode);
        }

        # endregion Test cases

        # region Supporting methods

        /// <summary>
        /// Validate Exceptions from Wiggle Parser and Formatter for General test cases.
        /// </summary>
        /// <param name="exceptionType"></param>
        public void ValidateArgumentNullException(ExceptionType exceptionType)
        {
            // Gets the filepath.
            String filePath = utilityObj.xmlUtil.GetTextValue(Constants.
                              SimpleWiggleWithFixedStepNodeName, Constants.FilePathNode);
            Assert.IsTrue(File.Exists(filePath));
            WiggleAnnotation annotation =null;

            try
            {
                switch (exceptionType)
                {
                    case ExceptionType.NullFormatter:
                        WiggleFormatter formatter = new WiggleFormatter(filePath);
                        formatter.Write(null);                        
                        break;
                    case ExceptionType.NullParser:
                        WiggleParser parser = new WiggleParser();
                        string valueForParse = null;
                        parser.Parse(valueForParse);
                        break;
                    case ExceptionType.NullAnnotationWithData:
                        annotation = new WiggleAnnotation(null, Constants.StringByteArray);
                        break;
                    case ExceptionType.NullAnnotationWithChromosomeName:
                        float[] data = new float[2] { 1.0F, 2.0F };
                        annotation = new WiggleAnnotation(data, null,0,0);
                        break;
                }
            }
            catch (ArgumentNullException exception)
            {
                ApplicationLog.WriteLine(
                    "Wiggle P2 test cases : Successfully validated the exception:", exception.Message);
                Console.WriteLine(
                    "Wiggle P2 test cases : Successfully validated the exception:", exception.Message);
            }
        }

        /// <summary>
        /// Validate Exceptions from Wiggle Parser with invalid metadata.
        /// </summary>
        /// <param name="nodeName">Node containing the invalid file name.</param>
        public void ValidateFormatException(string nodeName)
        {
            // Gets the filepath.
            String filePath = utilityObj.xmlUtil.GetTextValue(Constants.
                              InValidFileNamesNode, nodeName);
            Assert.IsTrue(File.Exists(filePath));

            WiggleParser parser=new WiggleParser();

            try
            {
                parser.Parse(filePath);
            }
            catch (FormatException exception)
            {
                ApplicationLog.WriteLine(
                    "Wiggle P2 test cases : Successfully validated Format exception:", exception.Message);
                Console.WriteLine(
                    "Wiggle P2 test cases : Successfully validated Format exception:", exception.Message);
            }
        }

        # endregion Supporting methods
    }
}
