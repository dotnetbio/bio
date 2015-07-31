using System;
using System.Text;

using Bio.Extensions;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests
{
    /// <summary>
    /// Test Automation code for Bio Qualitative sequence validations.
    /// </summary>
    [TestFixture]
    public class QualitativeSequenceP1TestCases
    {
        #region Enums

        /// <summary>
        /// Qualitative Sequence method Parameters which are used for different test cases 
        /// based on which the test cases are executed.
        /// </summary>
        enum QualitativeSequenceParameters
        {
            Score,
            ByteArray,
            IndexOf,
            IndexOfNonGap,
            IndexOfNonGapWithParam,
            LastIndexOf,
            LastIndexOfWithPam,
            DefaultScoreWithAlphabets,
            DefaultScoreWithSequence,
            MaxDefaultScore,
            MinDefaultScore,
        };

        #endregion Enums

        readonly Utility utilityObj = new Utility(@"TestUtils\QualitativeTestsConfig.xml");

        #region Qualitative P1 TestCases

        /// <summary>
        /// Validate creation of Qualitative Sequence for Rna Sequence
        /// with Sanger FastQFormat and specified score.
        /// Input Data : Rna Alphabet,Rna Sequence,"Sanger" FastQFormat.
        /// and Score "120" 
        /// Output Data : Validation of Created Qualitative sequence.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateSangerFormatTypeRnaQualitativeSequenceWithScore()
        {
            this.GeneralQualitativeSequence(Constants.SimpleRnaSangerNode,
                QualitativeSequenceParameters.Score);
        }

        /// <summary>
        /// Validate creation of Qualitative Sequence for Rna Sequence
        /// with Solexa FastQFormat and specified score.
        /// Input Data : Rna Alphabet,Rna Sequence,"Solexa" FastQFormat.
        /// and Score "104" 
        /// Output Data : Validation of Created Qualitative sequence.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateSolexaFormatTypeRnaQualitativeSequenceWithScore()
        {
            this.GeneralQualitativeSequence(Constants.SimpleRnaSolexaNode,
                QualitativeSequenceParameters.Score);
        }

        /// <summary>
        /// Validate creation of Qualitative Sequence for Rna Sequence
        /// with Illumina FastQFormat and specified score.
        /// Input Data : Rna Alphabet,Rna Sequence,"Illumina" FastQFormat.
        /// and Score "104" 
        /// Output Data : Validation of Created Qualitative sequence.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateIlluminaFormatTypeRnaQualitativeSequenceWithScore()
        {
            this.GeneralQualitativeSequence(Constants.SimpleRnaIlluminaNode,
                QualitativeSequenceParameters.Score);
        }

        /// <summary>
        /// Validate creation of Qualitative Sequence for Protein Sequence
        /// with Sanger FastQFormat and specified score.
        /// Input Data : Protein Alphabet,Protein Sequence,"Sanger" FastQFormat.
        /// and Score "120" 
        /// Output Data : Validation of Created Qualitative sequence.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateSangerFormatTypeProteinQualitativeSequenceWithScore()
        {
            this.GeneralQualitativeSequence(Constants.SimpleProteinSangerNode,
                QualitativeSequenceParameters.Score);
        }

        /// <summary>
        /// Validate creation of Qualitative Sequence for Protein Sequence
        /// with Solexa FastQFormat and specified score.
        /// Input Data : Protein Alphabet,Protein Sequence,"Solexa" FastQFormat.
        /// and Score "104" 
        /// Output Data : Validation of Created Qualitative sequence.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateSolexaFormatTypeProteinQualitativeSequenceWithScore()
        {
            this.GeneralQualitativeSequence(Constants.SimpleProteinSolexaNode,
                QualitativeSequenceParameters.Score);
        }

        /// <summary>
        /// Validate creation of Qualitative Sequence for Protein Sequence
        /// with Illumina FastQFormat and specified score.
        /// Input Data : Protein Alphabet,Protein Sequence,"Illumina" FastQFormat.
        /// and Score "104" 
        /// Output Data : Validation of Created Qualitative sequence.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateIlluminaFormatTypeProteinQualitativeSequenceWithScore()
        {
            this.GeneralQualitativeSequence(Constants.SimpleProteinIlluminaNode,
                QualitativeSequenceParameters.Score);
        }

        /// <summary>
        /// Validate creation of Qualitative Sequence for Dna Sequence
        /// with Illumina FastQFormat and Byte array.
        /// Input Data : Dna Alphabet,Dna Sequence,"Illumina" FastQFormat.
        /// Output Data : Validation of Created Qualitative sequence with score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateIlluminaFormatTypeDnaQualitativeSequenceWithByteArray()
        {
            this.GeneralQualitativeSequence(Constants.SimpleDNAIlluminaByteArrayNode,
                QualitativeSequenceParameters.ByteArray);
        }

        /// <summary>
        /// Validate IndexOf Qualitative Sequence Items.
        /// Input Data : Dna Sequence and score.
        /// Output Data : Validate qualitative sequence item indices.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateQualitativeSeqItemIndexes()
        {
            this.ValidateGeneralQualitativeSeqItemIndices(
                Constants.SimpleDnaSangerNode, QualitativeSequenceParameters.IndexOf);
        }

        /// <summary>
        /// Validate IndexOf Non Gap characters present in Qualitative Sequence Items.
        /// Input Data : Dna Sequence and score.
        /// Output Data : Validate IndexOf Non Gap characters present in Qualitative Sequence Items.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateQualitativeSeqItemIndexOfNonGapChars()
        {
            this.ValidateGeneralQualitativeSeqItemIndices(
                Constants.SimpleDnaSangerNode, QualitativeSequenceParameters.IndexOfNonGap);
        }

        /// <summary>
        /// Validate IndexOf Non Gap characters present in Qualitative Sequence
        /// Items by passing Sequence Item position to IndexOfNonGap() method.
        /// Input Data : Dna Sequence and score.
        /// Output Data : Validate IndexOf Non Gap characters present in Qualitative Sequence Items.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateQualitativeSeqItemIndexOfNonGapCharsUsingPam()
        {
            this.ValidateGeneralQualitativeSeqItemIndices(
                Constants.SimpleDnaSangerNode, QualitativeSequenceParameters.IndexOfNonGapWithParam);
        }

        /// <summary>
        /// Validate Last IndexOf Non Gap characters present in Qualitative Sequence Items.
        /// Input Data : Dna Sequence and score.
        /// Output Data : Validate Last IndexOf Non Gap characters present in Qualitative Sequence Items.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateQualitativeSeqItemLastIndexOfNonGapChars()
        {
            this.ValidateGeneralQualitativeSeqItemIndices(
                Constants.SimpleDnaSangerNode, QualitativeSequenceParameters.LastIndexOf);
        }

        /// <summary>
        /// Validate LastIndexOf Non Gap characters present in Qualitative Sequence
        /// Items by passing Sequence Item position to LastIndexOfNonGap() method.
        /// Input Data : Dna Sequence and score.
        /// Output Data : Validate IndexOf Non Gap characters present in Qualitative Sequence Items.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateQualitativeSeqItemLastIndexOfNonGapCharsUsingPam()
        {
            this.ValidateGeneralQualitativeSeqItemIndices(
                Constants.SimpleDnaSangerNode, QualitativeSequenceParameters.LastIndexOfWithPam);
        }

        /// <summary>
        /// Validate default score for Dna solexa FastQ sequence.
        /// Input Data :Dna Alphabet,Solexa FastQ format.
        /// Output Data : Validate FastQ Sanger format type default score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateDefaultQualScoreForDnaSolexa()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleDnaSolexaNode, QualitativeSequenceParameters.DefaultScoreWithAlphabets);
        }

        /// <summary>
        /// Validate FastQ Sanger format type default score.
        /// Input Data :Dna Alphabet, Sanger FastQ format.
        /// Output Data : Validate FastQ Sanger format type default score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateDefaultQualScoreForDnaSanger()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleDnaSangerNode, QualitativeSequenceParameters.DefaultScoreWithAlphabets);
        }

        /// <summary>
        /// Validate FastQ Sanger format type default score.
        /// Input Data :Protein Alphabet, Sanger FastQ format.
        /// Output Data : Validate FastQ Sanger format type default score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateDefaultQualScoreForProteinSanger()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleProteinSangerNode, QualitativeSequenceParameters.DefaultScoreWithAlphabets);
        }

        /// <summary>
        /// Validate FastQ Illumina format type default score.
        /// Input Data :Dna Alphabet, Illumina FastQ format.
        /// Output Data : Validate FastQ Illumina format type default score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateDefaultQualScoreForDnaIllumina()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleDnaIlluminaNode, QualitativeSequenceParameters.DefaultScoreWithAlphabets);
        }

        /// <summary>
        /// Validate FastQ Illumina format type default score.
        /// Input Data :Rna Alphabet, Illumina FastQ format.
        /// Output Data : Validate FastQ Illumina format type default score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateDefaultQualScoreForRnaIllumina()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleRnaIlluminaNode, QualitativeSequenceParameters.DefaultScoreWithAlphabets);
        }

        /// <summary>
        /// Validate FastQ Illumina format type default score.
        /// Input Data :Protein Alphabet, Illumina FastQ format.
        /// Output Data : Validate FastQ Illumina format type default score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateDefaultQualScoreForProteinIllumina()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleProteinIlluminaNode, QualitativeSequenceParameters.DefaultScoreWithAlphabets);
        }

        /// <summary>
        /// Validate FastQ Solexa format type default score.
        /// Input Data :Protein Sequence, Solexa FastQ format.
        /// Output Data : Validate FastQ Solexa format type default score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateDefaultQualScoreForProteinSequenceSolexa()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleProteinSolexaNode, QualitativeSequenceParameters.DefaultScoreWithSequence);
        }

        /// <summary>
        /// Validate FastQ Solexa format type default score.
        /// Input Data :Dna Sequence, Solexa FastQ format.
        /// Output Data : Validate FastQ Solexa format type default score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateDefaultQualScoreForDnaSequenceSolexa()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleDnaSolexaNode, QualitativeSequenceParameters.DefaultScoreWithSequence);
        }

        /// <summary>
        /// Validate FastQ Solexa format type default score.
        /// Input Data :Rna Sequence, Solexa FastQ format.
        /// Output Data : Validate FastQ Solexa format type default score.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateDefaultQualScoreForRnaSequenceSolexa()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleRnaSolexaNode, QualitativeSequenceParameters.DefaultScoreWithSequence);
        }

        /// <summary>
        /// Validate Maximum score for Dna Sanger FastQ.
        /// Input Data :Dna Sequence, Sanger FastQ format.
        /// Output Data : Validate Maximum score for Dna Sanger FastQ.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateMaxQualScoreForDnaSanger()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleDnaSangerNode, QualitativeSequenceParameters.MaxDefaultScore);
        }

        /// <summary>
        /// Validate Maximum score for Rna Sanger FastQ.
        /// Input Data :Rna Sequence, Sanger FastQ format.
        /// Output Data : Validate Maximum score for Rna Sanger FastQ.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateMaxQualScoreForRnaSanger()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleRnaSangerNode, QualitativeSequenceParameters.MaxDefaultScore);
        }

        /// <summary>
        /// Validate Maximum score for Protein Sanger FastQ.
        /// Input Data :Protein Sequence,Sanger FastQ format.
        /// Output Data : Validate Maximum score for Protein Sanger FastQ.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateMaxQualScoreForProteinSanger()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleProteinSangerNode, QualitativeSequenceParameters.MaxDefaultScore);
        }

        /// <summary>
        /// Validate Maximum score for Dna Illumina FastQ.
        /// Input Data :Dna Sequence,Illumina FastQ format.
        /// Output Data : Validate Maximum score for Dna Illumina FastQ.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateMaxQualScoreForDnaIllumina()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleDnaIlluminaNode, QualitativeSequenceParameters.MaxDefaultScore);
        }

        /// <summary>
        /// Validate Maximum score for Rna Illumina FastQ.
        /// Input Data :Rna Sequence,Illumina FastQ format.
        /// Output Data : Validate Maximum score for Rna Illumina FastQ.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateMaxQualScoreForRnaIllumina()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleRnaIlluminaNode, QualitativeSequenceParameters.MaxDefaultScore);
        }

        /// <summary>
        /// Validate Maximum score for Protein Illumina FastQ.
        /// Input Data :Protein Sequence,Illumina FastQ format.
        /// Output Data : Validate Maximum score for Protein Illumina FastQ.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateMaxQualScoreForProteinIllumina()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleProteinIlluminaNode, QualitativeSequenceParameters.MaxDefaultScore);
        }

        /// <summary>
        /// Validate Maximum score for Dna Solexa FastQ.
        /// Input Data :Dna Sequence,Solexa FastQ format.
        /// Output Data : Validate Maximum score for Dna Solexa FastQ.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateMaxQualScoreForDnaSolexa()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleDnaSolexaNode, QualitativeSequenceParameters.MaxDefaultScore);
        }

        /// <summary>
        /// Validate Maximum score for Rna Solexa FastQ.
        /// Input Data :Rna Sequence,Solexa FastQ format.
        /// Output Data : Validate Maximum score for Rna Solexa FastQ.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateMaxQualScoreForRnaSolexa()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleRnaSolexaNode, QualitativeSequenceParameters.MaxDefaultScore);
        }

        /// <summary>
        /// Validate Maximum score for Protein Solexa FastQ.
        /// Input Data :Protein Sequence,Solexa FastQ format.
        /// Output Data : Validate Maximum score for Protein Solexa FastQ.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateMaxQualScoreForProteinSolexa()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleProteinSolexaNode, QualitativeSequenceParameters.MaxDefaultScore);
        }

        /// <summary>
        /// Validate Minimum score for Dna Sanger FastQ.
        /// Input Data :Dna Sequence, Sanger FastQ format.
        /// Output Data : Validate Minimum score for Dna Sanger FastQ.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateMinimumQualScoreForDnaSanger()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleDnaSangerNode, QualitativeSequenceParameters.MinDefaultScore);
        }

        /// <summary>
        /// Validate Minimum score for Rna Sanger FastQ.
        /// Input Data :Rna Sequence, Sanger FastQ format.
        /// Output Data : Validate Minimum score for Rna Sanger FastQ.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateMinimumQualScoreForRnaSanger()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleRnaSangerNode, QualitativeSequenceParameters.MinDefaultScore);
        }

        /// <summary>
        /// Validate Minimum score for Protein Sanger FastQ.
        /// Input Data :Protein Sequence,Sanger FastQ format.
        /// Output Data : Validate Minimum score for Protein Sanger FastQ.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateMinimumQualScoreForProteinSanger()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleProteinSangerNode, QualitativeSequenceParameters.MinDefaultScore);
        }

        /// <summary>
        /// Validate Minimum score for Dna Illumina FastQ.
        /// Input Data :Dna Sequence,Illumina FastQ format.
        /// Output Data : Validate Minimum score for Dna Illumina FastQ.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateMinimumQualScoreForDnaIllumina()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleDnaIlluminaNode, QualitativeSequenceParameters.MinDefaultScore);
        }

        /// <summary>
        /// Validate Minimum score for Rna Illumina FastQ.
        /// Input Data :Rna Sequence,Illumina FastQ format.
        /// Output Data : Validate Minimum score for Rna Illumina FastQ.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateMinimumQualScoreForRnaIllumina()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleRnaIlluminaNode, QualitativeSequenceParameters.MinDefaultScore);
        }

        /// <summary>
        /// Validate Minimum score for Protein Illumina FastQ.
        /// Input Data :Protein Sequence,Illumina FastQ format.
        /// Output Data : Validate Minimum score for Protein Illumina FastQ.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateMinimumQualScoreForProteinIllumina()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleProteinIlluminaNode, QualitativeSequenceParameters.MinDefaultScore);
        }

        /// <summary>
        /// Validate Minimum score for Dna Solexa FastQ.
        /// Input Data :Dna Sequence,Solexa FastQ format.
        /// Output Data : Validate Minimum score for Dna Solexa FastQ.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateMinimumQualScoreForDnaSolexa()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleDnaSolexaNode, QualitativeSequenceParameters.MinDefaultScore);
        }

        /// <summary>
        /// Validate Minimum score for Rna Solexa FastQ.
        /// Input Data :Rna Sequence,Solexa FastQ format.
        /// Output Data : Validate Minimum score for Rna Solexa FastQ.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateMinimumQualScoreForRnaSolexa()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleRnaSolexaNode, QualitativeSequenceParameters.MinDefaultScore);
        }

        /// <summary>
        /// Validate Minimum score for Protein Solexa FastQ.
        /// Input Data :Protein Sequence,Solexa FastQ format.
        /// Output Data : Validate Minimum score for Protein Solexa FastQ.
        /// </summary>
        [Test]
        [Category("Priority1")]
        public void ValidateMinimumQualScoreForProteinSolexa()
        {
            this.ValidateFastQDefaultScores(
                Constants.SimpleProteinSolexaNode, QualitativeSequenceParameters.MinDefaultScore);
        }

        #endregion Qualitative P1 TestCases

        #region Supporting Methods

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
            string inputScoreforIUPAC = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.MaxScoreNode);
            string inputQuality = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.InputByteArrayNode);
            byte[] byteArray = Encoding.UTF8.GetBytes(inputQuality);
            int index = 0;

            // Create and validate Qualitative Sequence.
            switch (parameters)
            {
                case QualitativeSequenceParameters.Score:
                    createdQualitativeSequence = new QualitativeSequence(alphabet, expectedFormatType,
                        inputSequence, Utility.GetDefaultEncodedQualityScores(expectedFormatType, inputSequence.Length));
                    // Validate score
                    foreach (byte qualScore in createdQualitativeSequence.GetEncodedQualityScores())
                    {
                        Assert.AreEqual(qualScore, Convert.ToInt32(inputScoreforIUPAC, (IFormatProvider)null));
                    }
                    break;
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
            Assert.AreEqual(createdQualitativeSequence.ConvertToString(), expectedSequence);
            Assert.AreEqual(createdQualitativeSequence.Count.ToString((IFormatProvider)null), expectedSequenceCount);
            ApplicationLog.WriteLine(string.Format(null, "Qualitative Sequence P1:Qualitative Sequence {0} is as expected.", createdQualitativeSequence));
            ApplicationLog.WriteLine(string.Format(null, "Qualitative Sequence P1:Qualitative Sequence Score {0} is as expected.", createdQualitativeSequence.GetEncodedQualityScores()));
            Assert.AreEqual(createdQualitativeSequence.FormatType, expectedFormatType);
            ApplicationLog.WriteLine(string.Format(null, "Qualitative Sequence P1:Qualitative format type {0} is as expected.", createdQualitativeSequence.FormatType));
        }

        /// <summary>
        /// General method to validate Index of Qualitative Sequence Items.
        /// <param name="nodeName">xml node name.</param>
        /// <param name="indexParam">Different Qualitative Sequence parameters.</param>
        /// </summary>
        void ValidateGeneralQualitativeSeqItemIndices(string nodeName, QualitativeSequenceParameters indexParam)
        {
            // Gets the actual sequence and the alphabet from the Xml
            IAlphabet alphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(
               nodeName, Constants.AlphabetNameNode));
            FastQFormatType expectedFormatType = Utility.GetFastQFormatType(
                this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FastQFormatType));
            QualitativeSequence createdQualitativeSequence = null;
            string inputSequence = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.inputSequenceNode);
            string expectedFirstItemIdex = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.FirstItemIndex);
            string expectedLastItemIdex = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.LastItemIndex);
            string expectedGapIndex = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.IndexOfGap);
            long lastItemIndex;
            long index;

            // Create a qualitative Sequence.
            createdQualitativeSequence = new QualitativeSequence(
                alphabet, expectedFormatType, inputSequence,
                 Utility.GetDefaultEncodedQualityScores(expectedFormatType, inputSequence.Length));

            // Get a Index of qualitative sequence items
            switch (indexParam)
            {
                case QualitativeSequenceParameters.IndexOfNonGap:
                    index = createdQualitativeSequence.IndexOfNonGap();

                    // Validate Qualitative sequence item indices.
                    Assert.AreEqual(index, Convert.ToInt32(expectedFirstItemIdex, (IFormatProvider)null));
                    break;
                case QualitativeSequenceParameters.IndexOfNonGapWithParam:
                    index = createdQualitativeSequence.IndexOfNonGap(5);

                    // Validate Qualitative sequence item indices.
                    Assert.AreEqual(index, Convert.ToInt32(expectedGapIndex, (IFormatProvider)null));
                    break;
                case QualitativeSequenceParameters.LastIndexOf:
                    lastItemIndex = createdQualitativeSequence.LastIndexOfNonGap();

                    // Validate Qualitative sequence item indices.
                    Assert.AreEqual(lastItemIndex, Convert.ToInt32(expectedLastItemIdex, (IFormatProvider)null));
                    break;
                case QualitativeSequenceParameters.LastIndexOfWithPam:
                    lastItemIndex = createdQualitativeSequence.LastIndexOfNonGap(5);

                    // Validate Qualitative sequence item indices.
                    Assert.AreEqual(lastItemIndex, Convert.ToInt32(expectedGapIndex, (IFormatProvider)null));
                    break;
                default:
                    break;
            }

            // Logs to the VSTest GUI window
            ApplicationLog.WriteLine("Qualitative Sequence P1 : Qualitative SequenceItems indices validation completed successfully.");
        }

        /// <summary>
        /// General method to validate default score for different FastQ 
        /// format with different sequence.
        /// <param name="nodeName">xml node name.</param>
        /// <param name="parameters">Different Qualitative Score method parameter.</param>
        /// </summary>
        void ValidateFastQDefaultScores(string nodeName, QualitativeSequenceParameters parameters)
        {
            // Gets the actual sequence and the alphabet from the Xml
            IAlphabet alphabet = Utility.GetAlphabet(this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.AlphabetNameNode));
            FastQFormatType expectedFormatType = Utility.GetFastQFormatType(
                this.utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FastQFormatType));
            string inputSequence = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.inputSequenceNode);
            string expectedMaxScore = this.utilityObj.xmlUtil.GetTextValue(
                nodeName, Constants.DefualtMaxScore);
            string expectedMinScore = this.utilityObj.xmlUtil.GetTextValue(
                 nodeName, Constants.DefaultMinScore);

            QualitativeSequence createdQualitativeSequence = null;
            string qualityScoresString = Utility.GetDefaultEncodedQualityScores(expectedFormatType, inputSequence.Length);
            byte[] expectedMaxScores = Utility.GetEncodedQualityScores((byte)int.Parse(expectedMaxScore, null as IFormatProvider), inputSequence.Length);
            byte[] expectedMinScores = Utility.GetEncodedQualityScores((byte)int.Parse(expectedMinScore, null as IFormatProvider), inputSequence.Length);
            int i = 0;
            switch (parameters)
            {
                case QualitativeSequenceParameters.DefaultScoreWithAlphabets:
                    createdQualitativeSequence = new QualitativeSequence(
                        alphabet, expectedFormatType, inputSequence,
                         qualityScoresString);

                    // Validate default score.
                    i = 0;
                    foreach (byte qualitativeScore in createdQualitativeSequence.GetEncodedQualityScores())
                    {
                        Assert.AreEqual(qualitativeScore,
                            (byte)(qualityScoresString[i]));
                        i++;
                    }

                    // Log VSTest GUI.
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "Qualitative Sequence P1:Qualitative Sequence Default score {0} is as expected.",
                        qualityScoresString[0]));
                    break;
                case QualitativeSequenceParameters.DefaultScoreWithSequence:
                    createdQualitativeSequence = new QualitativeSequence(alphabet,
                        expectedFormatType, inputSequence,
                        qualityScoresString);

                    i = 0;
                    // Validate default score.
                    foreach (byte qualitativeScore in createdQualitativeSequence.GetEncodedQualityScores())
                    {
                        Assert.AreEqual(qualitativeScore,
                            (byte)(qualityScoresString[i]));
                        i++;
                    }

                    // Log VSTest GUI.
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "Qualitative Sequence P1:Qualitative Sequence Default score {0} is as expected.",
                        qualityScoresString[0]));
                    break;
                case QualitativeSequenceParameters.MaxDefaultScore:
                    createdQualitativeSequence = new QualitativeSequence(
                        alphabet, expectedFormatType, Encoding.UTF8.GetBytes(inputSequence),
                        expectedMaxScores);
                    i = 0;
                    // Validate default maximum score.
                    foreach (byte qualitativeScore in createdQualitativeSequence.GetEncodedQualityScores())
                    {
                        Assert.AreEqual(qualitativeScore,
                            expectedMaxScores[i]);
                        i++;
                    }

                    // Log VSTest GUI.
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "Qualitative Sequence P1:Qualitative Sequence Maximum score {0} is as expected.",
                        QualitativeSequence.GetMaxEncodedQualScore(expectedFormatType)));
                    break;
                case QualitativeSequenceParameters.MinDefaultScore:
                    createdQualitativeSequence = new QualitativeSequence(
                        alphabet, expectedFormatType, Encoding.UTF8.GetBytes(inputSequence),
                       expectedMinScores);

                    i = 0;
                    // Validate default minimum score.
                    foreach (byte qualitativeScore in createdQualitativeSequence.GetEncodedQualityScores())
                    {
                        Assert.AreEqual(qualitativeScore,
                            expectedMinScores[i]);
                        i++;
                    }

                    // Log VSTest GUI.
                    ApplicationLog.WriteLine(string.Format(null, "Qualitative Sequence P1:Qualitative Sequence Minimum score {0} is as expected.",
                        QualitativeSequence.GetMinEncodedQualScore(expectedFormatType)));
                    break;
                default:
                    break;
            }
        }

        #endregion Supporting Methods
    }
}
