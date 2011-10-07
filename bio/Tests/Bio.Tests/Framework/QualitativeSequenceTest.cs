using Bio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests
{
    /// <summary>
    /// Test for Qualitative sequence.
    /// </summary>
    [TestClass]
    public class QualitativeSequenceTest
    {
        /// <summary>
        /// Test constructor by passing byte array as sequence data and quality scores.
        /// </summary>
        [TestMethod]
        public void TestConstructorWithByteArray()
        {
            byte[] sequenceData = new byte[6];
            sequenceData[0] = (byte)'C';
            sequenceData[1] = (byte)'A';
            sequenceData[2] = (byte)'A';
            sequenceData[3] = (byte)'G';
            sequenceData[4] = (byte)'C';
            sequenceData[5] = (byte)'T';

            byte[] qualityScores = new byte[6];
            qualityScores[0]=65;
            qualityScores[1]=65;
            qualityScores[2]=65;
            qualityScores[3]=65;
            qualityScores[4]=110;
            qualityScores[5]=125;

            string expectedSequence = "CAAGCT";
            QualitativeSequence qualitativeSequence = new QualitativeSequence(Alphabets.DNA, FastQFormatType.Illumina, sequenceData, qualityScores);
            
            string actual = "";
            foreach (byte bt in qualitativeSequence)
            {
                actual+=(char)bt;
            }
            Assert.AreEqual(expectedSequence, actual);

            Assert.AreEqual(qualitativeSequence.Alphabet, Alphabets.DNA);
            Assert.AreEqual(qualitativeSequence.Count, 6);
            // 
            // Test for indexer
            Assert.AreEqual(qualitativeSequence[0], (byte)'C');
            Assert.AreEqual(qualitativeSequence[1], (byte)'A');
            Assert.AreEqual(qualitativeSequence[2], (byte)'A');
            Assert.AreEqual(qualitativeSequence[3], (byte)'G');
            Assert.AreEqual(qualitativeSequence[4], (byte)'C');
            Assert.AreEqual(qualitativeSequence[5], (byte)'T');

            int index=0;
            foreach(byte qualityScore in qualitativeSequence.QualityScores)
            {
                Assert.AreEqual(qualityScores[index++],qualityScore);
            }
        }

        /// <summary>
        /// Test constructor by passing string as sequence data and quality scores.
        /// </summary>
        [TestMethod]
        public void TestConstructorWithString()
        {
            string expectedSequence = "CAAGCT";
            string expectedQualityScore ="AAABBC";
              byte[] qualityScores = new byte[6];
            qualityScores[0]=65;
            qualityScores[1]=65;
            qualityScores[2]=65;
            qualityScores[3]=66;
            qualityScores[4]=66;
            qualityScores[5]=67;
            QualitativeSequence qualitativeSequence = new QualitativeSequence(Alphabets.DNA, FastQFormatType.Illumina, expectedSequence, expectedQualityScore);
            
            string actual = "";
            foreach (byte bt in qualitativeSequence)
            {
                actual+=(char)bt;
            }
            Assert.AreEqual(expectedSequence, actual);

            Assert.AreEqual(qualitativeSequence.Alphabet, Alphabets.DNA);
            Assert.AreEqual(qualitativeSequence.Count, 6);
            // 
            // Test for indexer
            Assert.AreEqual(qualitativeSequence[0], (byte)'C');
            Assert.AreEqual(qualitativeSequence[1], (byte)'A');
            Assert.AreEqual(qualitativeSequence[2], (byte)'A');
            Assert.AreEqual(qualitativeSequence[3], (byte)'G');
            Assert.AreEqual(qualitativeSequence[4], (byte)'C');
            Assert.AreEqual(qualitativeSequence[5], (byte)'T');

            int index=0;
            foreach(byte qualityScore in qualitativeSequence.QualityScores)
            {
                Assert.AreEqual(qualityScores[index++],qualityScore);
            }
        }

        /// <summary>
        /// Validate GetMinQuality score.
        /// </summary>
        [TestMethod]
        public void TestGetMinQualScore()
        {
            byte b;
            b = QualitativeSequence.GetMinQualScore(FastQFormatType.Solexa);
            Assert.AreEqual<byte>((byte)59, b);

            b = QualitativeSequence.GetMinQualScore(FastQFormatType.Sanger);
            Assert.AreEqual<byte>((byte)33, b);

            b = QualitativeSequence.GetMinQualScore(FastQFormatType.Illumina);
            Assert.AreEqual<byte>((byte)64, b);
        }

        /// <summary>
        /// Validate GetMaxScore.
        /// </summary>
        [TestMethod]
        public void TestGetMaxQualScore()
        {
            byte b;
            b = QualitativeSequence.GetMaxQualScore(FastQFormatType.Solexa);
            Assert.AreEqual<byte>((byte)126, b);

            b = QualitativeSequence.GetMaxQualScore(FastQFormatType.Sanger);
            Assert.AreEqual<byte>((byte)126, b);

            b = QualitativeSequence.GetMaxQualScore(FastQFormatType.Illumina);
            Assert.AreEqual<byte>((byte)126, b);
        }

        /// <summary>
        /// Validate GetDefaultQualityScore.
        /// </summary>
        [TestMethod]
        public void TestGetDefaultQualScore()
        {
            byte b;
            b = QualitativeSequence.GetDefaultQualScore(FastQFormatType.Sanger);
            Assert.AreEqual<byte>((byte)93, b);

            b = QualitativeSequence.GetDefaultQualScore(FastQFormatType.Solexa);
            Assert.AreEqual<byte>((byte)124, b);

            b = QualitativeSequence.GetDefaultQualScore(FastQFormatType.Illumina);
            Assert.AreEqual<byte>((byte)124, b);
        }

        /// <summary>
        /// Validate ConvertFromSolexaToIllumina
        /// </summary>
        [TestMethod]
        public void TestConvertFromSolexaToIllumina()
        {
            byte[] illuminaScores;
            byte[] solexaScores = new byte[2];
            solexaScores[0] = (byte)60;
            solexaScores[1] = (byte)60;
            illuminaScores = QualitativeSequence.ConvertFromSolexaToIllumina(solexaScores);
            Assert.IsNotNull((object)illuminaScores);
            Assert.AreEqual<int>(2, illuminaScores.Length);
            Assert.AreEqual<byte>((byte)65, illuminaScores[0]);
            Assert.AreEqual<byte>((byte)65, illuminaScores[1]);
        }

        /// <summary>
        /// Validate ConvertFromSolexaToSanger.
        /// </summary>
        [TestMethod]
        public void TestConvertFromSolexaToSanger()
        {
            byte[] sangerScores;
            byte[] solexaScores = new byte[1];
            solexaScores[0] = (byte)59;
            sangerScores = QualitativeSequence.ConvertFromSolexaToSanger(solexaScores);
            Assert.IsNotNull((object)sangerScores);
            Assert.AreEqual<int>(1, sangerScores.Length);
            Assert.AreEqual<byte>((byte)33, sangerScores[0]);
        }

        /// <summary>
        /// Validate ConvertFromSangerToSolexa
        /// </summary>
        [TestMethod]
        public void TestConvertFromSangerToSolexa()
        {
            byte[] solexaScores;
            byte[] sangerScores = new byte[2];
            sangerScores[0] = (byte)34;
            sangerScores[1] = (byte)34;
            solexaScores = QualitativeSequence.ConvertFromSangerToSolexa(sangerScores);
            Assert.IsNotNull((object)solexaScores);
            Assert.AreEqual<int>(2, solexaScores.Length);
            Assert.AreEqual<byte>((byte)59, solexaScores[0]);
            Assert.AreEqual<byte>((byte)59, solexaScores[1]);
        }

        /// <summary>
        /// Validate ConvertFromSangerToIllumina
        /// </summary>
        [TestMethod]
        public void TestConvertFromSangerToIllumina()
        {
            byte[] illuminaScores;
            byte[] sangerScores = new byte[2];
            sangerScores[0] = (byte)33;
            sangerScores[1] = (byte)33;
            illuminaScores = QualitativeSequence.ConvertFromSangerToIllumina(sangerScores);
            Assert.IsNotNull((object)illuminaScores);
            Assert.AreEqual<int>(2, illuminaScores.Length);
            Assert.AreEqual<byte>((byte)64, illuminaScores[0]);
            Assert.AreEqual<byte>((byte)64, illuminaScores[1]);
        }

        /// <summary>
        /// Validate ConvertFromIlluminaToSolexa
        /// </summary>
        [TestMethod]
        public void TestConvertFromIlluminaToSolexa()
        {
            byte[] solexaScores;
            byte[] illuminaScores = new byte[2];
            illuminaScores[0] = (byte)65;
            illuminaScores[1] = (byte)65;
            solexaScores = QualitativeSequence.ConvertFromIlluminaToSolexa(illuminaScores);
            Assert.IsNotNull((object)solexaScores);
            Assert.AreEqual<int>(2, solexaScores.Length);
            Assert.AreEqual<byte>((byte)59, solexaScores[0]);
            Assert.AreEqual<byte>((byte)59, solexaScores[1]);
        }

        /// <summary>
        /// Validate - ConvertFromIlluminaToSanger
        /// </summary>
        [TestMethod]
        public void TestConvertFromIlluminaToSanger()
        {
            byte[] sangerScores;
            byte[] illuminaScores = new byte[2];
            illuminaScores[0] = (byte)64;
            illuminaScores[1] = (byte)64;
            sangerScores = QualitativeSequence.ConvertFromIlluminaToSanger(illuminaScores);
            Assert.IsNotNull((object)sangerScores);
            Assert.AreEqual<int>(2, sangerScores.Length);
            Assert.AreEqual<byte>((byte)33, sangerScores[0]);
            Assert.AreEqual<byte>((byte)33, sangerScores[1]);
        }
    }
}
