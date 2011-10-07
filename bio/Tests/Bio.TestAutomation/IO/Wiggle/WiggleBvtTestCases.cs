/****************************************************************************
 * WiggleBvtTestCases.cs
 * 
 *   This file contains the Wiggle - Parsers and Formatters Bvt test cases.
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
    /// Wiggle Bvt parser and formatter Test case implementation.
    /// </summary>
    [TestClass]
    public class WiggleBvtTestCases
    {
        #region Global Variables.

        Utility utilityObj = new Utility(@"TestUtils\WiggleTestConfig.xml");

        #endregion Global Variables.

        #region Enum

        /// <summary>
        /// Enum to determind Parse type
        /// </summary>
        private enum ParseType
        {
            Stream,
            Default,
        };

        /// <summary>
        /// Enum to determind Format type
        /// </summary>
        private enum FormatType
        {
            Stream,
            Default,
        };

        #endregion Enum

        # region Wiggle Parser test cases.

        /// <summary>
        /// Validate all public properties for Wiggle Parser.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateWiggleParserPublicProperties()
        {
            WiggleAnnotation annotation = null;

            // Gets the filepath  from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(Constants.
                              SimpleWiggleWithFixedStepNodeName, Constants.FilePathNode);
            Assert.IsTrue(File.Exists(filePath));

            // Logs information to the log file            
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Wiggle Parser BVT: File Exists in the Path '{0}'.", filePath));

            //Get the expected values from configuration file.
            string expectedDescription = utilityObj.xmlUtil.GetTextValue(Constants.
                        SimpleWiggleWithFixedStepNodeName, Constants.ParserDescriptionNode);
            string expectedName = utilityObj.xmlUtil.GetTextValue(Constants.
                        SimpleWiggleWithFixedStepNodeName, Constants.NameNode);
            string expectedFileType = utilityObj.xmlUtil.GetTextValue(Constants.
                        SimpleWiggleWithFixedStepNodeName, Constants.FileTypesNode);

            WiggleParser parser = new WiggleParser();

            //Parse the file
            annotation = parser.Parse(filePath);

            Assert.AreEqual(expectedDescription, parser.Description);
            Assert.AreEqual(expectedName, parser.Name);
            Assert.AreEqual(expectedFileType, parser.FileTypes);

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Wiggle Parser BVT: Validation of all public properties is successfull"));
        }

        /// <summary>
        /// Validate Wiggle parser on a small fixed step file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateWiggleParserWithFixedStep()
        {
            ValidateWiggleParser(Constants.SimpleWiggleWithFixedStepNodeName, ParseType.Default);
        }

        /// <summary>
        /// Validate Wiggle parser on a small variable step file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateWiggleParserWithVariableStep()
        {
            ValidateWiggleParser(Constants.SimpleWiggleWithVariableStepNodeName, ParseType.Default);
        }

        /// <summary>
        /// Validate Wiggle parser on a small variable step file on parsing with a Stream.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateWiggleParserWithVariableStepOnAStream()
        {
            ValidateWiggleParser(Constants.SimpleWiggleWithVariableStepNodeName, ParseType.Stream);
        }

        # endregion Wiggle Parser test cases.

        # region Wiggle Formatter test cases.

        /// <summary>
        ///  Validate Wiggle formatter on a small fixed step file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateWiggleFormatterWithFixedStep()
        {
            ValidateWiggleFormatter(Constants.SimpleWiggleWithFixedStepNodeName, FormatType.Default);
        }

        /// <summary>
        ///   Validate Wiggle formatter on a small variable step file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateWiggleFormatterWithVariableStep()
        {
            ValidateWiggleFormatter(Constants.SimpleWiggleWithVariableStepNodeName, FormatType.Default);
        }

        /// <summary>
        ///   Validate Wiggle formatter on a small variable step file with a Stream.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateWiggleFormatterWithVariableStepOnAStream()
        {
            ValidateWiggleFormatter(Constants.SimpleWiggleWithVariableStepNodeName, FormatType.Stream);
        }

        /// <summary>
        /// Validates all public properties for Wiggle formatter.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateWiggleFormatterPublicProperties()
        {
            // Gets the filepath.
            String filePath = utilityObj.xmlUtil.GetTextValue(Constants.
                              SimpleWiggleWithFixedStepNodeName, Constants.FilePathNode);
            Assert.IsTrue(File.Exists(filePath));

            //Get the expected values from configuration file.
            string expectedDescription = utilityObj.xmlUtil.GetTextValue(Constants.
                        SimpleWiggleWithFixedStepNodeName, Constants.FormatterDescriptionNode);
            string expectedName = utilityObj.xmlUtil.GetTextValue(Constants.
                        SimpleWiggleWithFixedStepNodeName, Constants.NameNode);
            string expectedFileType = utilityObj.xmlUtil.GetTextValue(Constants.
                        SimpleWiggleWithFixedStepNodeName, Constants.FileTypesNode);

            using (StreamWriter writer = new StreamWriter(Constants.WiggleTempFileName))
            {
                WiggleFormatter formatter = new WiggleFormatter(writer);
                Assert.AreEqual(expectedDescription, formatter.Description);
                Assert.AreEqual(expectedName, formatter.Name);
                Assert.AreEqual(expectedFileType, formatter.FileTypes);
                formatter.Close();
            }

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Wiggle Formatter BVT: Validation of all public properties is successfull"));
        }

        # endregion Wiggle Formatter test cases.

        # region Wiggle Annotation test cases.

        /// <summary>
        /// Validate all public properties for Annotation.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateAnnotationPublicProperties()
        {
            WiggleAnnotation annotation = null;

            // Gets the filepath  from the Xml
            string filePath = utilityObj.xmlUtil.GetTextValue(Constants.
                              SimpleWiggleWithFixedStepNodeName, Constants.FilePathNode);
            Assert.IsTrue(File.Exists(filePath));

            // Logs information to the log file            
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Wiggle Parser BVT: File Exists in the Path '{0}'.", filePath));

            string expectedbasePosition = utilityObj.xmlUtil.GetTextValue(Constants.
                        SimpleWiggleWithFixedStepNodeName, Constants.BasePositionNode);
            string expectedchromosome = utilityObj.xmlUtil.GetTextValue(Constants.
                        SimpleWiggleWithFixedStepNodeName, Constants.ChromosomeNode);
            string expectedannotationCount = utilityObj.xmlUtil.GetTextValue(Constants.
                        SimpleWiggleWithFixedStepNodeName, Constants.AnnotationCountNode);
            string expectedSpan = utilityObj.xmlUtil.GetTextValue(Constants.
                        SimpleWiggleWithFixedStepNodeName, Constants.ExpectedSpanNode);
            string expectedStep = utilityObj.xmlUtil.GetTextValue(Constants.
                        SimpleWiggleWithFixedStepNodeName, Constants.ExpectedStepNode);
            string annotationTypeNode = utilityObj.xmlUtil.GetTextValue(Constants.
                        SimpleWiggleWithFixedStepNodeName, Constants.AnnotationTypeNode);

            string[] annotationKeys = utilityObj.xmlUtil.GetTextValues(Constants.
                        SimpleWiggleWithFixedStepNodeName, Constants.AnnotationMetadataNode);

            string[] values = annotationKeys[0].Split(',');
            string[] keys = annotationKeys[1].Split(',');

            WiggleParser parser = new WiggleParser();

            //Parse the file
            annotation = parser.Parse(filePath);

            Assert.AreEqual(long.Parse(expectedbasePosition, CultureInfo.InvariantCulture)
                            , annotation.BasePosition);
            Assert.AreEqual(expectedchromosome, annotation.Chromosome);
            Assert.AreEqual(long.Parse(expectedannotationCount, CultureInfo.InvariantCulture)
                            , annotation.Count);
            Assert.AreEqual(int.Parse(expectedSpan,
                            CultureInfo.InvariantCulture), annotation.Span);
            Assert.AreEqual(int.Parse(expectedStep,
                            CultureInfo.InvariantCulture), annotation.Step);
            Assert.AreEqual(annotationTypeNode, annotation.AnnotationType.ToString());

            Dictionary<string, string> annotationMetadata = annotation.Metadata;

            //Validate Annotation Metadata.
            for (int i = 0; i < annotationMetadata.Count; i++)
            {
                Assert.AreEqual(keys[i], annotationMetadata.ElementAt(i).Key);
                Assert.AreEqual(values[i], annotationMetadata.ElementAt(i).Value);
            }

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Wiggle Parser BVT: Validation of Annotation public properties is successfull"));
        }

        /// <summary>
        /// Creates Annotation object from the scratch and validates successfull creation of Annotation object.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateWiggleAnnotation()
        {
            string[] expectedValues = utilityObj.xmlUtil.GetTextValue(Constants.
                            AnnotationValuesNode, Constants.ExpectedValuesNode).Split(',');
            string[] annotationKeys = utilityObj.xmlUtil.GetTextValue(Constants.
                        AnnotationValuesNode, Constants.ExpectedKeysNode).Split(',');

            string stepValue = utilityObj.xmlUtil.GetTextValue(Constants.
                            AnnotationValuesNode, Constants.ExpectedStepNode);
            string startValue = utilityObj.xmlUtil.GetTextValue(Constants.
                            AnnotationValuesNode, Constants.ExpectedStartNode);
            int index = 0;

            float[] data = new float[expectedValues.Length];

            for (index = 0; index < expectedValues.Length; index++)
            {
                data[index] = float.Parse(expectedValues[index], CultureInfo.InvariantCulture);
            }

            WiggleAnnotation annotation = new WiggleAnnotation(data, Constants.
                            ChromosomeNode, long.Parse(startValue, CultureInfo.InvariantCulture),
                            int.Parse(stepValue, CultureInfo.InvariantCulture));

            index = 0;

            //Validate keys and values of the parsed file.
            foreach (KeyValuePair<long, float> keyvaluePair in annotation)
            {
                Assert.AreEqual(float.Parse(annotationKeys[index],
                            CultureInfo.InvariantCulture), keyvaluePair.Key);
                Assert.AreEqual(long.Parse(expectedValues[index],
                            CultureInfo.InvariantCulture), keyvaluePair.Value);
                index++;
            }

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Wiggle Annotation BVT: Validation of Annotation creation is successfull"));
        }

        /// <summary>
        /// Validate GetValueArray() returns all the values from an annotation object.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetValueArray()
        {
            string[] expectedValues = utilityObj.xmlUtil.GetTextValue(Constants.
                            AnnotationValuesNode, Constants.ExpectedValuesNode).Split(',');
            string stepValue = utilityObj.xmlUtil.GetTextValue(Constants.
                            AnnotationValuesNode, Constants.ExpectedStepNode);
            string startValue = utilityObj.xmlUtil.GetTextValue(Constants.
                            AnnotationValuesNode, Constants.ExpectedStartNode);
            int index = 0;

            float[] data = new float[expectedValues.Length];

            for (index = 0; index < expectedValues.Length; index++)
            {
                data[index] = float.Parse(expectedValues[index], CultureInfo.InvariantCulture);
            }

            WiggleAnnotation annotation = new WiggleAnnotation(data, Constants.
                            ChromosomeNode, long.Parse(startValue, CultureInfo.InvariantCulture), int.Parse(stepValue, CultureInfo.InvariantCulture));

            //Get the values.
            float[] dataNew = annotation.GetValueArray(0, expectedValues.Length);
            index = 0;

            //Validate the values from GetValueArray.
            foreach (float value in dataNew)
            {
                Assert.AreEqual(data[index], value);
                index++;
            }
        }

        #endregion Wiggle Annotation test cases.

        # region Supporting methods.

        /// <summary>
        /// Validate Wiggle Parser for fixed and variable steps.
        /// </summary>
        /// <param name="nodeName">Nodename</param>
        /// <param name="parseType">Read using a stream/File Name.</param>
        private void ValidateWiggleParser(string nodeName, ParseType parseType)
        {
            // Gets the filepath.
            String filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            Assert.IsTrue(File.Exists(filePath));

            int index = 0;
            WiggleAnnotation annotation = null;

            // Logs information to the log file            
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Wiggle Parser BVT: File Exists in the Path '{0}'.", filePath));

            string[] expectedValues = utilityObj.xmlUtil.GetTextValue(nodeName,
                                     Constants.ExpectedValuesNode).Split(',');
            string[] expectedPoints = utilityObj.xmlUtil.GetTextValue(nodeName,
                                     Constants.ExpectedKeysNode).Split(',');

            WiggleParser parser = new WiggleParser();

            switch (parseType)
            {
                case ParseType.Stream:
                    using (StreamReader stream = new StreamReader(filePath))
                    {
                        annotation = parser.Parse(stream);
                    }
                    break;
                default:
                    annotation = parser.Parse(filePath);
                    break;
            }

            //Validate keys and values of the parsed file.
            foreach (KeyValuePair<long, float> keyvaluePair in annotation)
            {
                Assert.AreEqual(long.Parse(expectedPoints[index],
                            CultureInfo.InvariantCulture), keyvaluePair.Key);
                Assert.AreEqual(float.Parse(expectedValues[index],
                            CultureInfo.InvariantCulture), keyvaluePair.Value);
                index++;
            }

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
               "Wiggle Parser BVT: Successfully validated the parsed file"));
        }

        /// <summary>
        /// Validate Wiggle Formatter for fixed and variable steps.
        /// </summary>
        /// <param name="nodeName">Nodename</param>
        /// <param name="formatType">Write using a stream/File Name.</param>
        private void ValidateWiggleFormatter(string nodeName, FormatType formatType)
        {
            // Gets the filepath.
            String filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode);
            Assert.IsTrue(File.Exists(filePath));
            WiggleAnnotation annotation = null, annotationNew = null;

            string[] expectedValues = utilityObj.xmlUtil.GetTextValue(nodeName,
                                     Constants.ExpectedValuesNode).Split(',');
            string[] expectedPoints = utilityObj.xmlUtil.GetTextValue(nodeName,
                                     Constants.ExpectedKeysNode).Split(',');

            //Parse the file.
            WiggleParser parser = new WiggleParser();
            annotation = parser.Parse(filePath);

            WiggleFormatter formatter = null;

            //Write to a new file.                    
            switch (formatType)
            {
                case FormatType.Stream:
                    using (StreamWriter writer = new StreamWriter(Constants.WiggleTempFileName))
                    {
                        formatter = new WiggleFormatter(writer);
                        formatter.Write(annotation);
                        formatter.Close();
                    }
                    break;
                default:
                    formatter = new WiggleFormatter(Constants.WiggleTempFileName);
                    formatter.Write(annotation);
                    formatter.Close();
                    break;
            }

            //Read the new file and then compare the annotations.
            WiggleParser parserNew = new WiggleParser();
            annotationNew = parserNew.Parse(Constants.WiggleTempFileName);
            int index = 0;

            //Validate keys and values of the parsed file.
            foreach (KeyValuePair<long, float> keyvaluePair in annotationNew)
            {
                Assert.AreEqual(long.Parse(expectedPoints[index],
                            CultureInfo.InvariantCulture), keyvaluePair.Key);
                Assert.AreEqual(float.Parse(expectedValues[index],
                            CultureInfo.InvariantCulture), keyvaluePair.Value);
                index++;
            }

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
               "Wiggle Formatter BVT: Successfully validated the written file"));
        }

        # endregion Supporting methods.
    }
}
