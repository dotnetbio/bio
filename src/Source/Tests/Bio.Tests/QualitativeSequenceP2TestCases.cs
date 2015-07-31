using System;
using System.Globalization;
using System.Linq;
using System.Text;

using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests
{
    /// <summary>
    /// Test Automation code for Bio Qualitative Sequence P2 level validations.
    /// </summary>
    [TestFixture]
    public class QualitativeSequenceP2TestCases
    {
        /// <summary>
        /// Qualitative Sequence method Parameters which are used for different test cases 
        /// based on which the test cases are executed.
        /// </summary>
        enum QualitativeSequenceParameters
        {
            FormatType,
            ByteArray,
        };

        /// <summary>
        /// Qualitative sequence format type parameters.
        /// </summary>
        enum QualitativeSeqFormatTypePam
        {
            SangerToIllumina,
            SangerToSolexa,
            SolexaToSanger,
            SolexaToIllumina,
            IlluminaToSanger,
            IlluminaToSolexa,
        };

        readonly Utility utilityObj = new Utility(@"TestUtils\QualitativeTestsConfig.xml");

        # region Qualitative Sequence P2 TestCases

        /// <summary>
        /// Invalidate Qualsequence with null alphabet
        /// Input Data : Null Sequence.
        /// Output Data : Validation of Exception by passing null value.
        /// </summary>
        [Test]
        [Category("Priority2")]
        [Ignore("Different error message on mono versus .NET")]
        public void InvalidateQualSequenceWithNullValue()
        {
            // Get values from xml.
            string expectedErrorMessage = this.utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleDnaSangerNode, Constants.AlphabetNullExceptionNode);
            string actualError = string.Empty;
            string updatedActualError = string.Empty;
            QualitativeSequence qualSeq = null;

            //create Qualitative sequence by passing null value.
            try
            {
                qualSeq = new QualitativeSequence(null, FastQFormatType.Sanger,
                    (byte[])null, (byte[])null);
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                actualError = ex.Message;

                // Validate an expected exception.
                updatedActualError = actualError.Replace("\r", "").Replace("\n", "");
                Assert.AreEqual(expectedErrorMessage.ToLower(CultureInfo.CurrentCulture),
                    updatedActualError.ToLower(CultureInfo.CurrentCulture));
                Assert.IsNull(qualSeq);
            }

            // Log to VSTest GUI.
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Qualitative Sequence P2: Qualitative Sequence Null exception was validated successfully {0}",
                updatedActualError));
        }

        /// <summary>
        /// Invalidate Qualsequence with empty sequence.
        /// Input Data : Empty Sequence.
        /// Output Data : Validation of Exception by passing empty sequence.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateQualSequenceWithEmptySequence()
        {
            QualitativeSequence qualSeq = new QualitativeSequence(
                Alphabets.DNA, FastQFormatType.Sanger, "", "");

            Assert.IsNotNull(qualSeq);

            // Log to VSTest GUI.
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Qualitative Sequence P2: Qualitative empty Sequence was validated successfully {0}",
                qualSeq));
        }

        /// <summary>
        /// QualSequence  of FastQ Format "Sanger" with invalid score.
        /// Input Data : Valid Dna sanger Sequence, Invalid quality score.
        /// Output Data : Validate Exception by passing invalid quality score.
        /// </summary>
        [Test]
        [Category("Priority2")]
        [Ignore("Different error on windows/linux")]
        public void InvalidateSangerQualSequenceWithInvalidQualScore()
        {
            this.InValidateQualSequence(Constants.SimpleDnaSangerNode,
                Constants.InvalidQualityScore, QualitativeSequenceParameters.FormatType);
        }

        /// <summary>
        /// QualSequence of FastQ Format "Illumina" with invalid score.
        /// Input Data : Valid Dna Illumina Sequence, Invalid quality score.
        /// Output Data : Validate Exception by passing invalid quality score.
        /// </summary>
        [Test]
        [Category("Priority2")]
        [Ignore("Different error on windows/linux")]
        public void InvalidateIlluminaQualSequenceWithInvalidQualScore()
        {
            this.InValidateQualSequence(Constants.SimpleDnaIlluminaNode,
                Constants.InvalidQualityScore, QualitativeSequenceParameters.FormatType);
        }

        /// <summary>
        /// QualSequence of FastQ Format "Solexa" with invalid score.
        /// Input Data : Valid Dna Solexa Sequence, Invalid quality score.
        /// Output Data : Validate Exception by passing invalid quality score.
        /// </summary>
        [Test]
        [Category("Priority2")]
        [Ignore("Different error message on mono versus .NET")]
        public void InvalidateSolexaQualSequenceWithInvalidQualScore()
        {
            this.InValidateQualSequence(Constants.SimpleDnaSolexaNode,
                Constants.InvalidQualityScore, QualitativeSequenceParameters.FormatType);
        }

        /// <summary>
        /// QualSequence  of FastQ Format "Sanger" with few invalid score
        /// in byte array.
        /// Input Data : Valid Dna sanger Sequence, Invalid quality score.
        /// Output Data : Validate Exception by passing invalid quality score.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSangerQualSequenceWithInvalidByteArrayScore()
        {
            this.InValidateQualSequence(Constants.SimpleDnaSangerNode,
                Constants.InvalidByteQualScore, QualitativeSequenceParameters.ByteArray);
        }

        /// <summary>
        /// QualSequence  of FastQ Format "Solexa" with few invalid score
        /// in byte array.
        /// Input Data : Valid Dna Solexa Sequence, Invalid quality score.
        /// Output Data : Validate Exception by passing invalid quality score.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateSolexaQualSequenceWithInvalidByteArrayScore()
        {
            this.InValidateQualSequence(Constants.SimpleDnaSolexaNode,
               Constants.InvalidByteQualScore, QualitativeSequenceParameters.ByteArray);
        }

        /// <summary>
        /// QualSequence  of FastQ Format "Illumina" with few invalid score
        /// in byte array.
        /// Input Data : Valid Dna Illumina Sequence, Invalid quality score.
        /// Output Data : Validate Exception by passing invalid quality score.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateIlluminaQualSequenceWithInvalidByteArrayScore()
        {
            this.InValidateQualSequence(Constants.SimpleDnaIlluminaNode,
                Constants.InvalidByteQualScore, QualitativeSequenceParameters.ByteArray);
        }

        /// <summary>
        /// Invalidate Qualsequence with invalid characters in the sequence.
        /// Input Data : Null Sequence.
        /// Output Data : Validation of Exception by passing seq with invalid characters.
        /// </summary>
        [Test]
        [Category("Priority2")]
        [Ignore("Different error message on linux/windows")]
        public void InvalidateQualSequenceWithInvalidChars()
        {
            // Get values from xml.
            string expectedErrorMessage = this.utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleDnaSangerNode, Constants.InvalidAlphabetErrorMessage);
            string actualError = string.Empty;
            string updatedActualError = string.Empty;
            QualitativeSequence qualSeq = null;

            //Try creating Qualitative sequence by passing invalid seq chars.
            try
            {
                qualSeq = new QualitativeSequence(Alphabets.DNA, FastQFormatType.Sanger, "AGTZ",
                    Utility.GetDefaultEncodedQualityScores(FastQFormatType.Sanger,4));
                Assert.Fail();
            }
            catch (ArgumentException ex)
            {
                actualError = ex.Message;
                // Validate an expected exception.
                updatedActualError = actualError.Replace("\r", "").Replace("\n", "");
                Assert.AreEqual(expectedErrorMessage.ToLower(CultureInfo.CurrentCulture),
                    updatedActualError.ToLower(CultureInfo.CurrentCulture));
                Assert.IsNull(qualSeq);
            }

            // Log to VSTest GUI.
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Qualitative Sequence P2: Qualitative Sequence Null exception was validated successfully {0}",
                updatedActualError));
        }


        /// <summary>
        /// Validate Reverse()of Dna Qualitative Sequence.
        /// Input Data : Qual Sequence.
        /// Output Data : Reverse of an Qualitative Sequence.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateReverseOfDnaQualSeq()
        {
            // Get Values from xml node.
            string reversedSeq = this.utilityObj.xmlUtil.GetTextValue(
               Constants.SimpleDnaSangerNode, Constants.ReverseQualSeq);

            // Create a Dna Sanger Qualitative Sequence.
            QualitativeSequence createdQualSeq =
                this.CreateQualitativeSequence(Constants.SimpleDnaSangerNode);

            // Validate an Reverse of Qual Sequence.
            ISequence reverseQual = createdQualSeq.GetReversedSequence();
            Assert.AreEqual(reversedSeq, new string(reverseQual.Select(a => (char)a).ToArray()));

            // Log to VSTest GUI.
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Qualitative Sequence P2: Reverse of Qualitative Sequence {0}",
                reverseQual));
        }

        /// <summary>
        /// Validate Reverse()of Rna Qualitative Sequence.
        /// Input Data : Qual Sequence.
        /// Output Data : Reverse of an Rna Qualitative Sequence.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateReverseOfRnaQualSeq()
        {
            // Get Values from xml node.
            string reversedSeq = this.utilityObj.xmlUtil.GetTextValue(
               Constants.SimpleRnaSangerNode, Constants.ReverseQualSeq);

            // Create a Dna Sanger Qualitative Sequence.
            QualitativeSequence createdQualSeq =
                this.CreateQualitativeSequence(Constants.SimpleRnaSangerNode);

            // Validate an Reverse of Qual Sequence.
            ISequence reverseQual = createdQualSeq.GetReversedSequence();
            Assert.AreEqual(reversedSeq, new string(reverseQual.Select(a => (char)a).ToArray()));

            // Log to VSTest GUI.
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Qualitative Sequence P2: Reverse of Qualitative Sequence {0}",
                reverseQual));
        }

        /// <summary>
        /// Validate Reverse()of Protein Qualitative Sequence.
        /// Input Data : Qual Sequence.
        /// Output Data : Reverse of an Protein Qualitative Sequence.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateReverseOfProteinQualSeq()
        {
            // Get Values from xml node.
            string reversedSeq = this.utilityObj.xmlUtil.GetTextValue(
               Constants.SimpleProteinSangerNode, Constants.ReverseQualSeq);

            // Create a Dna Sanger Qualitative Sequence.
            QualitativeSequence createdQualSeq =
                this.CreateQualitativeSequence(Constants.SimpleProteinSangerNode);

            // Validate an Reverse of Protein Qual Sequence.
            ISequence reverseQual = createdQualSeq.GetReversedSequence();
            Assert.AreEqual(reversedSeq, new string(reverseQual.Select(a => (char)a).ToArray()));

            // Log to VSTest GUI.
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Qualitative Sequence P2: Reverse of Qualitative Sequence {0}",
                reverseQual));
        }

        /// <summary>
        /// Validate Complement of Dna Qualitative Sequence.
        /// Input Data : Qual Sequence.
        /// Output Data : Complement of an Qualitative Sequence.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateComplementOfDnaQualSeq()
        {
            // Get Values from xml node.
            string complementSeq = this.utilityObj.xmlUtil.GetTextValue(
               Constants.SimpleDnaSangerNode, Constants.ComplementQualSeqNode);

            // Create a Dna Sanger Qualitative Sequence.
            QualitativeSequence createdQualSeq =
                this.CreateQualitativeSequence(Constants.SimpleDnaSangerNode);

            // Validate Reverse of Qual Sequence.
            ISequence complementQual = createdQualSeq.GetComplementedSequence();
            Assert.AreEqual(complementSeq, new string(complementQual.Select(a => (char)a).ToArray()));

            // Log to VSTest GUI.
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Qualitative Sequence P2: complement Qual of Qualitative Sequence {0}",
                complementQual));
        }

        /// <summary>
        /// Validate Reverse()of Rna Qualitative Sequence.
        /// Input Data : Qual Sequence.
        /// Output Data : Reverse of an Rna Qualitative Sequence.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateComplementOfRnaQualSeq()
        {
            // Get Values from xml node.
            string complementSeq = this.utilityObj.xmlUtil.GetTextValue(
               Constants.SimpleRnaSangerNode, Constants.ComplementQualSeqNode);

            // Create a Dna Sanger Qualitative Sequence.
            QualitativeSequence createdQualSeq =
                this.CreateQualitativeSequence(Constants.SimpleRnaSangerNode);

            // Validate complement of Qual Sequence.
            ISequence complementQual = createdQualSeq.GetComplementedSequence();
            Assert.AreEqual(complementSeq, new string(complementQual.Select(a => (char)a).ToArray()));

            // Log to VSTest GUI.
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Qualitative Sequence P2: complement Qual of Qualitative Sequence {0}",
                complementQual));
        }

        /// <summary>
        /// Validate Exception when try complement Protein Qual sequence..
        /// Input Data : Qual Sequence.
        /// Output Data : Exception while getting Protein complement for Qual Sequence..
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InValidateComplementOfProteinQualSeq()
        {
            // Get values from xml.
            string expectedErrorMessage = this.utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleProteinSangerNode, Constants.ComplementException);
            string actualError = string.Empty;
            ISequence seq = null;

            // Create a Dna Sanger Qualitative Sequence.
            QualitativeSequence createdQualSeq = this.CreateQualitativeSequence(
                Constants.SimpleProteinSangerNode);

            // Try getting commplement of Protein sequences.
            try
            {
                seq = createdQualSeq.GetComplementedSequence();
                Assert.Fail();
            }
            catch (NotSupportedException ex)
            {
                actualError = ex.Message;
                // Validate an expected exception.
                Assert.AreEqual(expectedErrorMessage.ToLower(CultureInfo.CurrentCulture),
                    actualError.ToLower(CultureInfo.CurrentCulture));
            }

            Assert.IsNull(seq);
            // Log to VSTest GUI.
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Qualitative Sequence P2: Reverse of Qualitative Sequence {0}",
                actualError));
        }

        /// <summary>
        /// Validate creation of Qualitative Sequence for Medium size less than 100KB Dna Sequence
        /// with Solexa FastQFormat and specified score.
        /// Input Data : Dna Alphabet,Dna Sequence,"Solexa" FastQFormat.
        /// and ByteArray score
        /// Output Data : Validation of Created Qualitative sequence.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateSolexaFormatTypeMediumSizeDnaQualitativeSequence()
        {
            this.GeneralQualitativeSequence(Constants.MediumSizeDNASolexaNode,
                QualitativeSequenceParameters.ByteArray);
        }

        /// <summary>
        /// Validate creation of Qualitative Sequence for Medium size less than 100KB Dna Sequence
        /// with Solexa FastQFormat and specified score.
        /// Input Data : Dna Alphabet,Dna Sequence,"Sanger" FastQFormat, NCBI4NA Encoding.
        /// and ByteArray score
        /// Output Data : Validation of Created Qualitative sequence.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateSangerFormatTypeMediumSizeDnaQualitativeSequence()
        {
            this.GeneralQualitativeSequence(Constants.MediumSizeDNASangerNode,
                QualitativeSequenceParameters.ByteArray);
        }

        /// <summary>
        /// Validate creation of Qualitative Sequence for Medium size less than 100KB Dna Sequence
        /// with Illumina FastQFormat and specified score.
        /// Input Data : Dna Alphabet,Dna Sequence,"Illumina" FastQFormat, NCBI4NA Encoding.
        /// and ByteArray score
        /// Output Data : Validation of Created Qualitative sequence.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateIlluminaFormatTypeMediumSizeDnaQualitativeSequence()
        {
            this.GeneralQualitativeSequence(Constants.MediumSizeDNAIlluminaNode,
                QualitativeSequenceParameters.ByteArray);
        }

        /// <summary>
        /// Validate creation of Qualitative Sequence for Large size greater than 
        /// 100KB Dna Sequence with Solexa FastQFormat and specified score.
        /// Input Data : Dna Alphabet,Dna Sequence,"Solexa" FastQFormat.
        /// and ByteArray score
        /// Output Data : Validation of Created Qualitative sequence.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateSolexaFormatTypeLargeSizeDnaQualitativeSequence()
        {
            this.GeneralQualitativeSequence(Constants.LargeSizeDNASolexaNode,
                QualitativeSequenceParameters.ByteArray);
        }

        /// <summary>
        /// Validate creation of Qualitative Sequence for Large size greater than 
        /// 100KB Dna Sequence with Illumina FastQFormat and specified score.
        /// Input Data : Dna Alphabet,Dna Sequence,"Illumina" FastQFormat.
        /// and ByteArray score
        /// Output Data : Validation of Created Qualitative sequence.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateIlluminaFormatTypeLargeSizeDnaQualitativeSequence()
        {
            this.GeneralQualitativeSequence(Constants.LargeSizeDNAIlluminaNode,
                QualitativeSequenceParameters.ByteArray);
        }

        /// <summary>
        /// Validate creation of Qualitative Sequence for Large size greater than 
        /// 100KB Dna Sequence with Sanger FastQFormat and specified score.
        /// Input Data : Dna Alphabet,Dna Sequence,"Sanger" FastQFormat.
        /// and ByteArray score
        /// Output Data : Validation of Created Qualitative sequence.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateSangerFormatTypeLargeSizeDnaQualitativeSequence()
        {
            this.GeneralQualitativeSequence(Constants.LargeSizeDNASangerNode,
                QualitativeSequenceParameters.ByteArray);
        }

        /// <summary>
        /// Validate creation of Qualitative Sequence for Very Large size greater 
        /// than 200KB Dna Sequence with Illumina FastQFormat and specified score.
        /// Input Data : Dna Alphabet,Dna Sequence,"Illumina" FastQFormat.
        /// and ByteArray score
        /// Output Data : Validation of Created Qualitative sequence.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateIlluminaFormatTypeVeryLargeSizeDnaQualitativeSequence()
        {
            this.GeneralQualitativeSequence(Constants.VeryLargeSizeDNAIlluminaNode,
                QualitativeSequenceParameters.ByteArray);
        }

        /// <summary>
        /// Validate creation of Qualitative Sequence for Very Large size greater 
        /// than 200KB Dna Sequence with Sanger FastQFormat and specified score.
        /// Input Data : Dna Alphabet,Dna Sequence,"Sanger" FastQFormat.
        /// and ByteArray score
        /// Output Data : Validation of Created Qualitative sequence.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateSangerFormatTypeVeryLargeSizeDnaQualitativeSequence()
        {
            this.GeneralQualitativeSequence(Constants.VeryLargeSizeDNASangerNode,
                QualitativeSequenceParameters.ByteArray);
        }

        /// <summary>
        /// Validate creation of Qualitative Sequence for Very Large size greater 
        /// than 200KB Dna Sequence with Solexa FastQFormat and specified score.
        /// Input Data : Dna Alphabet,Dna Sequence,"Solexa" FastQFormat.
        /// and ByteArray score
        /// Output Data : Validation of Created Qualitative sequence.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void ValidateSolexaFormatTypeVeryLargeSizeDnaQualitativeSequence()
        {
            this.GeneralQualitativeSequence(Constants.VeryLargeSizeDNASolexaNode,
                QualitativeSequenceParameters.ByteArray);
        }

        /// <summary>
        /// Invalidate convert from Sanger to Illumnia with invalid input values.
        /// Input Data : Invalid quality scores.
        /// Output Data : Validation of an expected exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateConvertSangerToIllumina()
        {
            this.InvalidateFormatTypeConvertion(
                QualitativeSeqFormatTypePam.SangerToIllumina);
        }

        /// <summary>
        /// Invalidate convert from Sanger to Solexa with invalid input values.
        /// Input Data : Invalid quality scores.
        /// Output Data : Validation of an expected exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateConvertSangerToSolexa()
        {
            this.InvalidateFormatTypeConvertion(
                QualitativeSeqFormatTypePam.SangerToSolexa);
        }

        /// <summary>
        /// Invalidate convert from Solexa to Illumnia with invalid input values.
        /// Input Data : Invalid quality scores.
        /// Output Data : Validation of an expected exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateConvertSolexaToIllumina()
        {
            this.InvalidateFormatTypeConvertion(
                QualitativeSeqFormatTypePam.SolexaToIllumina);
        }

        /// <summary>
        /// Invalidate convert from Solexa to Sanger with invalid input values.
        /// Input Data : Invalid quality scores.
        /// Output Data : Validation of an expected exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateConvertSolexaToSanger()
        {
            this.InvalidateFormatTypeConvertion(
                QualitativeSeqFormatTypePam.SolexaToSanger);
        }

        /// <summary>
        /// Invalidate convert from Illumina to Sanger with invalid input values.
        /// Input Data : Invalid quality scores.
        /// Output Data : Validation of an expected exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateConvertIlluminaToSanger()
        {
            this.InvalidateFormatTypeConvertion(
                QualitativeSeqFormatTypePam.IlluminaToSanger);
        }

        /// <summary>
        /// Invalidate convert from Illumina to Solexa with invalid input values.
        /// Input Data : Invalid quality scores.
        /// Output Data : Validation of an expected exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateConvertIlluminaToSolexa()
        {
            this.InvalidateFormatTypeConvertion(
                QualitativeSeqFormatTypePam.IlluminaToSolexa);
        }

        /// <summary>
        /// Invalidate Qualitative constructor.
        /// Input Data : Invalid input values.
        /// Output Data : Validation of an expected exception.
        /// </summary>
        [Test]
        [Category("Priority2")]
        public void InvalidateQualitativeSeqCtor()
        {
            // Get Input values from xml config file.
            string inputSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.QualitativeSequenceInsertSeqItemNode,
                Constants.inputSequenceNode);
            string QualScoreError = this.utilityObj.xmlUtil.GetTextValue(
                Constants.QualitativeSequenceInsertSeqItemNode,
                Constants.NullExceptionError);

            string actualError = null;

            QualitativeSequence seq = null;
            // Create a qual sequence.
            try
            {
                seq = new QualitativeSequence(Alphabets.DNA,
                     FastQFormatType.Sanger, inputSequence, null);
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                actualError = ex.Message;
               
            }
            finally
            {
                if (seq != null)
                    ((IDisposable)seq).Dispose();
            }

           
        }


        #endregion QualitativeSequence P2 TestCases

        #region Supporting Methods

        /// <summary>
        /// General method to invalidate creation of Qualitative sequence
        /// with invalid qual score..
        /// <param name="nodeName">Name of the Format type xml node.</param>
        /// <param name="errorMessage">Error message xml node name.</param>
        /// <param name="qualPam">Qualitative sequence constructor paramter.</param>
        /// </summary>
        void InValidateQualSequence(string nodeName, string errorMessage,
            QualitativeSequenceParameters qualPam)
        {
            // Get values from xml.
            FastQFormatType expectedFormatType = Utility.GetFastQFormatType(
                this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FastQFormatType));
            string expectedErrorMessage = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, errorMessage).Replace("FORMATTYPE", expectedFormatType.ToString());
            string inputSequence = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.inputSequenceNode);
            string actualError = string.Empty;
            string updatedActualError = string.Empty;
            QualitativeSequence qualSeq = null;
            byte[] scoreArray = { 65, 64, 66, 68, 69, 67, 65, 65, 65, 65, 65,
                                   200, 3, 65, 65, 65, 65, 65, 65, 65, 65, 65, 65, 65, 65, 4 };
            switch (qualPam)
            {
                case QualitativeSequenceParameters.FormatType:
                    //Try to create Qualitative sequence by invalid Quality score
                    try
                    {
                        qualSeq = new QualitativeSequence(Alphabets.DNA, expectedFormatType,
                          Encoding.UTF8.GetBytes(inputSequence), (byte[])null);
                        Assert.Fail();
                    }
                    catch (ArgumentException ex)
                    {
                        actualError = ex.Message;
                        // Validate an expected exception.
                        updatedActualError = actualError.Replace("\r", "").Replace("\n", "");
                        Assert.AreEqual(expectedErrorMessage.ToLower(CultureInfo.CurrentCulture),
                            updatedActualError.ToLower(CultureInfo.CurrentCulture));
                    }
                    break;
                case QualitativeSequenceParameters.ByteArray:
                    //Try to create Qualitative sequence by invalid Quality score
                    try
                    {
                        qualSeq = new QualitativeSequence(Alphabets.DNA, expectedFormatType,
                            Encoding.UTF8.GetBytes(inputSequence), scoreArray);
                        Assert.Fail();
                    }
                    catch (ArgumentException ex)
                    {
                        actualError = ex.Message;
                        // Validate an expected exception.
                        updatedActualError = actualError.Replace("\r", "").Replace("\n", "");
                        Assert.AreEqual(expectedErrorMessage.ToLower(CultureInfo.CurrentCulture),
                            updatedActualError.ToLower(CultureInfo.CurrentCulture));
                    }
                    break;
                default:
                    break;
            }

            // Log to VSTest GUI.
            Assert.IsNull(qualSeq);
            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Qualitative Sequence P2: Qualitative Sequence Invalid score exception was validated successfully {0}",
                updatedActualError));
        }

        /// <summary>
        /// General method to create a Qualitative sequence.
        /// <param name="nodeName">xml node name of diferent FastQ format.</param>
        /// </summary>
        private QualitativeSequence CreateQualitativeSequence(string nodeName)
        {
            // Gets the actual sequence and the alphabet from the Xml
            IAlphabet alphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlphabetNameNode));
            FastQFormatType expectedFormatType = Utility.GetFastQFormatType(
                this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FastQFormatType));
            QualitativeSequence createdQualitativeSequence = null;
            string inputSequence = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.inputSequenceNode);
            string inputQuality = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.InputByteArrayNode);
            byte[] byteArray = Encoding.UTF8.GetBytes(inputQuality);

            // Create a Qualitative Sequence.
            createdQualitativeSequence = new QualitativeSequence(
                alphabet, expectedFormatType, Encoding.UTF8.GetBytes(inputSequence), byteArray);

            return createdQualitativeSequence;
        }

        /// <summary>
        /// General method to validate creation of Qualitative sequence.
        /// <param name="nodeName">xml node name.</param>
        /// <param name="parameters">Different Qualitative Sequence parameters.</param>
        /// </summary>
        void GeneralQualitativeSequence(
            string nodeName, QualitativeSequenceParameters parameters)
        {
            // Gets the actual sequence and the alphabet from the Xml
            IAlphabet alphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlphabetNameNode));
            FastQFormatType expectedFormatType = Utility.GetFastQFormatType(
                this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FastQFormatType));
            QualitativeSequence createdQualitativeSequence = null;
            string inputSequence = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.inputSequenceNode);
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.ExpectedSequenceNode);
            string expectedSequenceCount = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.QSequenceCount);
            string inputQuality = this.utilityObj.xmlUtil.GetTextValue(
                 nodeName, Constants.InputByteArrayNode);
            byte[] byteArray = Encoding.UTF8.GetBytes(inputQuality);
            int index = 0;

            // Create and validate Qualitative Sequence.
            switch (parameters)
            {
                case QualitativeSequenceParameters.ByteArray:
                    createdQualitativeSequence = new QualitativeSequence(alphabet, expectedFormatType,
                      Encoding.UTF8.GetBytes(inputSequence), byteArray);

                    // Validate score
                    foreach (byte qualScore in createdQualitativeSequence.GetEncodedQualityScores())
                    {
                        Assert.AreEqual(qualScore, Convert.ToInt32(byteArray[index], (IFormatProvider)null));
                        index++;
                    }
                    break;
                default:
                    break;
            }

            // Validate createdSequence qualitative sequence.
            Assert.IsNotNull(createdQualitativeSequence);
            Assert.AreEqual(createdQualitativeSequence.Alphabet, alphabet);
            Assert.AreEqual(new string(createdQualitativeSequence.Select(a => (char)a).ToArray()), expectedSequence);
            Assert.AreEqual(createdQualitativeSequence.Count.ToString((IFormatProvider)null), expectedSequenceCount);
            Assert.AreEqual(createdQualitativeSequence.FormatType, expectedFormatType);

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Qualitative Sequence P2:Qualitative Sequence {0} is as expected.",
                createdQualitativeSequence.ToString()));

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Qualitative Sequence P2:Qualitative Sequence Score {0} is as expected.",
                createdQualitativeSequence.GetEncodedQualityScores().ToString()));

            ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                "Qualitative Sequence P2:Qualitative format type {0} is as expected.",
                createdQualitativeSequence.FormatType));
        }

        /// <summary>
        /// Invalidate convert from one FastQ format type to other.
        /// <param name="formatTypePam">Type of the format want to be converted</param>       
        /// </summary>
        void InvalidateFormatTypeConvertion(
            QualitativeSeqFormatTypePam formatTypePam)
        {
            // Invalidate Qualitative sequence format type convertion.
            switch (formatTypePam)
            {
                case QualitativeSeqFormatTypePam.SangerToIllumina:
                this.ConvertTypeToType(FastQFormatType.Sanger, FastQFormatType.Illumina_v1_3);
                    break;
                case QualitativeSeqFormatTypePam.SangerToSolexa:
                this.ConvertTypeToType(FastQFormatType.Sanger, FastQFormatType.Solexa_Illumina_v1_0);
                    break;
                case QualitativeSeqFormatTypePam.IlluminaToSanger:
                    this.ConvertTypeToType(FastQFormatType.Illumina_v1_3, FastQFormatType.Sanger);
                    break;
                case QualitativeSeqFormatTypePam.IlluminaToSolexa:
                    this.ConvertTypeToType(FastQFormatType.Illumina_v1_3, FastQFormatType.Solexa_Illumina_v1_0);
                    break;
                case QualitativeSeqFormatTypePam.SolexaToIllumina:
                    this.ConvertTypeToType(FastQFormatType.Solexa_Illumina_v1_0, FastQFormatType.Illumina_v1_3);
                    break;
                case QualitativeSeqFormatTypePam.SolexaToSanger:
                this.ConvertTypeToType(FastQFormatType.Solexa_Illumina_v1_0, FastQFormatType.Sanger);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Invalidate convert from Illumina to Sanger format type.
        /// </summary>
        void ConvertTypeToType(FastQFormatType type1, FastQFormatType type2)
        {

            int[] scoreArray = { -12, 24 };
            int qualScore = -12;
            string actualError = null;
            Assert.Throws<ArgumentNullException> ( () =>
                QualitativeSequence.ConvertEncodedQualityScore(type1, type2, null));
         
            Assert.Throws<ArgumentOutOfRangeException> ( () =>
                QualitativeSequence.ConvertQualityScores(type1, type2, scoreArray)
            );

            // Validate an expected error message for invalid qual scores.
            Assert.Throws<ArgumentOutOfRangeException> ( () =>
                QualitativeSequence.ConvertQualityScore(type1, type2, qualScore)
            );
        }

        # endregion Supporting Methods
    }
}
