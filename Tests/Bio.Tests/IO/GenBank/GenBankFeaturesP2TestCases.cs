/****************************************************************************
 * GenBankFeaturesP2TestCases.cs
 * 
 *   This file contains the GenBank Features P2 test cases.
 * 
***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Bio.IO;
using Bio.IO.GenBank;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;
using System.Linq;
using NUnit.Framework;
using System.Runtime.Serialization;
using Bio;

#if (SILVERLIGHT == false)
    namespace Bio.TestAutomation.IO.GenBank
#else
    namespace Bio.Silverlight.TestAutomation.IO.GenBank
#endif
{
    /// <summary>
    /// GenBank Features P2 test case implementation.
    /// </summary>
    [TestFixture]
    public class GenBankFeaturesP2TestCases
    {

        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\GenBankFeaturesTestConfig.xml");

        #endregion Global Variables

      #region GenBankFeature P2 TestCases

        /// <summary>
        /// InValidate subsequence from GenBank sequence with location using 
        /// compliment operator with sub location.
        /// Input : GenBank sequence,location.
        /// Output : Validation of expected sub sequence.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateSubSequenceWithComplimentOperator()
        {
            // Get Values from XML node.
            string sequence = utilityObj.xmlUtil.GetTextValue(
                Constants.InvalidLocationWithComplementOperatorNode,
                Constants.ExpectedSequence);
            string location = utilityObj.xmlUtil.GetTextValue(
                Constants.InvalidLocationWithComplementOperatorNode,
                Constants.Location);
            string alphabet = utilityObj.xmlUtil.GetTextValue(
                Constants.InvalidLocationWithComplementOperatorNode,
                Constants.AlphabetNameNode);

            // Create a sequence object.
            ISequence seqObj = new Sequence(Utility.GetAlphabet(alphabet),
                sequence);

            // Build a location.
            ILocationBuilder locBuilder = new LocationBuilder();
            ILocation loc = locBuilder.GetLocation(location);
            LocationResolver locResolver = new LocationResolver();

            // Get sequence using location of the sequence with operator.
            try
            {
                loc.GetSubSequence(seqObj);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                LogExceptionMessage();
            }

            // Validate sub sequence exception for an invalid sequence.
            try
            {
                locResolver.GetSubSequence(loc, null);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                LogExceptionMessage();
            }

            // Validate GetSubSequence method with null location.
            try
            {
                locResolver.GetSubSequence(null, seqObj);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                LogExceptionMessage();
            }

            // Validate sub sequence exception for an invalid sequence.
            try
            {
                locResolver.GetSubSequence(loc, null, null);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                LogExceptionMessage();
            }

            // Validate GetSubSequence method with null location.
            try
            {
                locResolver.GetSubSequence(null, seqObj, null);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                LogExceptionMessage();
            }

            // Validate GetSubSequence method with null location.
            try
            {
                loc.GetSubSequence(null);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                       "GenBankFeatures P2 : Validate the exception successfully"));
            }

        }

        /// <summary>
        /// InValidate subsequence from GenBank sequence with location 
        /// using invalid accession.
        /// Input : GenBank sequence,location.
        /// Output : Validation of expected sub sequence.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateSubSequenceWithInvalidAccessionID()
        {
            // Get Values from XML node.
            string sequence = utilityObj.xmlUtil.GetTextValue(
                Constants.LocationWithDotOperatorNode,
                Constants.ExpectedSequence);
            string location = utilityObj.xmlUtil.GetTextValue(
                Constants.LocationWithDotOperatorNode,
                Constants.Location);
            string alphabet = utilityObj.xmlUtil.GetTextValue(
                Constants.LocationWithDotOperatorNode,
                Constants.AlphabetNameNode);

            // Create a sequence object.
            ISequence seqObj = new Sequence(Utility.GetAlphabet(alphabet),
                sequence);

            // Build a location.
            ILocationBuilder locBuilder = new LocationBuilder();
            ILocation loc = locBuilder.GetLocation(location);
            loc.Accession = "Invalid";

            // Get sequence using location of the sequence with operator.
            try
            {
                loc.GetSubSequence(seqObj);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "GenBankFeatures P2 : Validate the exception successfully"));
            }

            Dictionary<string, ISequence> refSeq = null;

            // Validate GetSubSequence method with null location.
            try
            {
                loc.GetSubSequence(null, refSeq);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "GenBankFeatures P2 : Validate the exception successfully"));
            }

        }

        /// <summary>
        /// InValidate GenBank location EndData.
        /// Input : GenBank sequence,location.
        /// Output : Validation of location end data exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateGenBankLocationEndData()
        {
            // Get Values from XML node.
            string position = utilityObj.xmlUtil.GetTextValue(
                Constants.LocationWithEndDataNode, Constants.Position);

            // Build a location.
            LocationResolver locResolver = new LocationResolver();

            // Validate whether mentioned end data is present in the location
            // or not.
            try
            {
                locResolver.IsInEnd(null, Int32.Parse(position, (IFormatProvider)null));
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "GenBankFeatures P2 : Validate the exception successfully"));
            }

            // Validate IsInStart method exception.
            try
            {
                locResolver.IsInStart(null, Int32.Parse(position, (IFormatProvider)null));
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                LogExceptionMessage();
            }

            // Validate IsInRange method exception.
            try
            {
                locResolver.IsInRange(null, Int32.Parse(position, (IFormatProvider)null));
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                LogExceptionMessage();
            }

        }

        /// <summary>
        /// InValidate GenBank location GetStart and GetEnd..
        /// Input : GenBank sequence,location.
        /// Output : Validation of location end data.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateGenBankLocationGetStart()
        {
            // Build a location.
            LocationResolver locResolver = new LocationResolver();

            // Validate GetStart method exception.
            try
            {
                locResolver.GetStart(null);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                LogExceptionMessage();
            }

            // Validate GetEnd method exception.
            try
            {
                locResolver.GetEnd(null);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                LogExceptionMessage();
            }
        }

        /// <summary>
        /// InValidate GenBank location with invalid start location data.
        /// Input :Invalid location data.
        /// Output : Validation of location end data.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateGenBankLocationIsInStart()
        {
            string startData = ".";
            InValidateLocationStartData(startData);

            // Set Invalid start data Validate GetStart method exception.
            startData = "..";
            InValidateLocationStartData(startData);

            // Validate an exception for invalid start data with "^" operator.
            startData = "^";
            InValidateLocationStartData(startData);

            // Set Invalid start data Validate GetStart method exception.
            startData = "^^";
            InValidateLocationStartData(startData);

            // Validate an exception for invalid start data with "<" operator.
            startData = "<";
            InValidateLocationStartData(startData);

            // Set Invalid start data Validate GetStart method exception.
            startData = "<<";
            InValidateLocationStartData(startData);

            // Validate an exception for invalid start data with ">" operator.
            startData = ">";
            InValidateLocationStartData(startData);

            // Set Invalid start data Validate GetStart method exception.
            startData = ">>";
            InValidateLocationStartData(startData);
        }

        /// <summary>
        /// InValidate GenBank location with invalid End location data.
        /// Input :Invalid location data.
        /// Output : Validation of location end data.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateGenBankLocationEndDataWithInvalidValues()
        {
            // Invalidate location with end data ".";
            string endData = ".";
            InValidateLocationEndData(endData);

            // Set Invalid end data Validate GetStart method exception.
            endData = "..";
            InValidateLocationEndData(endData);

            // Validate an exception for invalid end data with "^" operator.
            endData = "^";
            InValidateLocationEndData(endData);

            // Set Invalid end data Validate GetStart method exception.
            endData = "^^";
            InValidateLocationEndData(endData);

            // Validate an exception for invalid end data with "<" operator.
            endData = "<";
            InValidateLocationEndData(endData);

            // Set Invalid end data Validate GetStart method exception.
            endData = "<<";
            InValidateLocationEndData(endData);

            // Validate an exception for invalid end data with ">" operator.
            endData = ">";
            InValidateLocationEndData(endData);

            // Set Invalid end data Validate GetStart method exception.
            endData = ">>";
            InValidateLocationEndData(endData);
        }

        /// <summary>
        /// InValidate GenBank GetLocation.
        /// Input : GenBank File
        /// Output : Validation of GenBank GetLocation.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateGenBankLocations()
        {
            // Build a location with invalid values
            ILocationBuilder locBuilder = new LocationBuilder();
            ILocation loc = null;
            try
            {
                loc = locBuilder.GetLocation(null);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                Assert.IsNull(loc);
                ApplicationLog.WriteLine(
                    "GenBankFeatures P2 : Validated the expected exception");
            }

            try
            {
                loc = locBuilder.GetLocation("Invalid");
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                Assert.IsNull(loc);
                ApplicationLog.WriteLine(
                    "GenBankFeatures P2 : Validated the expected exception");
            }
        }

        /// <summary>
        /// InValidate leaf location of GenBank locations.
        /// Input : GenBank File
        /// Output : Validation of GenBank leaf locations.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateGenBankLocationPositions()
        {
            // Get Values from XML node.
            string location = utilityObj.xmlUtil.GetTextValue(
                Constants.LocationWithEndDataUsingOperatorNode, Constants.Location);
            string expectedEndData = utilityObj.xmlUtil.GetTextValue(
                Constants.LocationWithEndDataUsingOperatorNode, Constants.EndData);
            string expectedStartData = utilityObj.xmlUtil.GetTextValue(
                Constants.LocationWithEndDataUsingOperatorNode, Constants.StartData);
            string position = utilityObj.xmlUtil.GetTextValue(
                Constants.LocationWithEndDataUsingOperatorNode, Constants.Position);

            // Build a location.
            ILocationBuilder locBuilder = new LocationBuilder();
            Location loc = (Location)locBuilder.GetLocation(location);
            loc.Resolver = null;
            loc.EndData = expectedEndData;
            loc.StartData = expectedStartData;

            // Validate whether mentioned end data is present in the location
            // or not.
            try
            {
                loc.IsInEnd(Int32.Parse(position, (IFormatProvider)null));
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                // Log to VSTest GUI.
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "GenBankFeatures P2 : Expected exception is verified"));
            }

            try
            {
                loc.IsInStart(Int32.Parse(position, (IFormatProvider)null));
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                // Log to VSTest GUI.
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "GenBankFeatures P2 : Expected exception is verified"));
            }

            try
            {
                loc.IsInRange(Int32.Parse(position, (IFormatProvider)null));
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                // Log to VSTest GUI.
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "GenBankFeatures P2 : Expected exception is verified"));
            }


        }

        #endregion GenBankFeature P2 TestCases

        #region Helper Methods

        /// <summary>
        /// Log VSTest GUI exception message.
        /// </summary>
        private static void LogExceptionMessage()
        {
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "GenBankFeatures P2 : Validate the exception successfully"));
        }

        /// <summary>
        /// InValidate GenBank location with invalid End location data.
        /// <param name="endData">End data used for different test cases.</param>
        /// </summary>
        private static void InValidateLocationEndData(string endData)
        {
            // Build a location.
            LocationResolver locResolver = new LocationResolver();
            string location = "123.125";
            ILocationBuilder locBuilder = new LocationBuilder();
            ILocation loc = locBuilder.GetLocation(location);

            // Set Invalid end data Validate GetStart method exception.
            loc.EndData = endData;
            try
            {
                locResolver.IsInEnd(loc, 124);
                Assert.Fail();
            }
            catch (IndexOutOfRangeException)
            {
                LogExceptionMessage();
            }
            catch (FormatException)
            {
                LogExceptionMessage();
            }
        }

        /// <summary>
        /// InValidate GenBank location with invalid Start location data.
        /// <param name="endData">Start data used for different test cases.</param>
        /// </summary>
        private static void InValidateLocationStartData(string startData)
        {
            // Build a location.
            LocationResolver locResolver = new LocationResolver();
            string location = "123.125";
            ILocationBuilder locBuilder = new LocationBuilder();
            ILocation loc = locBuilder.GetLocation(location);

            // Set Invalid end data Validate GetStart method exception.
            loc.StartData = startData;
            try
            {
                locResolver.IsInStart(loc, 124);
                Assert.Fail();
            }
            catch (IndexOutOfRangeException)
            {
                LogExceptionMessage();
            }
            catch (FormatException)
            {
                LogExceptionMessage();
            }
        }

        #endregion Helper Methods
    }
}
