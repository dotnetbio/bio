/****************************************************************************
 * Helper.cs
 * 
 * This file contains the Helper BVT test cases.
 * 
******************************************************************************/

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Bio.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Algorithms.Assembly.Padena;
using Bio.Util;
using Bio.IO.GenBank;

namespace Bio.TestAutomation.Util
{
    /// <summary>
    /// Bvt test cases for the Helper class
    /// </summary>
    [TestClass]
    public class HelperBvtTestCases
    {

        #region Bvt Test cases


        /// <summary>
        /// Validates Helper.GetReverseComplement
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        //TODO: This is really a PADENA Test, needs to be renamed.
        public void ValidateGetReverseComplement()
        {
            const int kmerLength = 6;
            const int dangleThreshold = 3;
            const int redundantThreshold = 7;

            using (ParallelDeNovoAssembler assembler = new ParallelDeNovoAssembler())
            {
                assembler.KmerLength = kmerLength;
                assembler.DanglingLinksThreshold = dangleThreshold;
                assembler.RedundantPathLengthThreshold = redundantThreshold;

                assembler.ScaffoldRedundancy = 0;
                assembler.Depth = 3;
                CloneLibrary.Instance.AddLibrary("abc", 5, 20);

                PadenaAssembly result = (PadenaAssembly)assembler.Assemble(GetReadsForScaffolds(), true);

               

                var expectedContigs = new List<string>
                {
                   "TTTTTT",
                   "TTAGCGCG",
                   "CGCGCCGCGC",
                   "CGCGCG",
                   "GCGCGC",
                   "TTTTTA",
                   "TTTTAGC",
                   "TTTTAA",
                   "TTTAAA",
                   "ATGCCTCCTATCTTAGC",
                };
                
                Assert.AreEqual(10, result.ContigSequences.Count());

                foreach (ISequence contig in result.ContigSequences)
                {
                    string contigSeq = contig.ConvertToString();
                    Assert.IsTrue(
                        expectedContigs.Contains(contigSeq) ||
                        expectedContigs.Contains(contigSeq.GetReverseComplement(new char[contigSeq.Length])), 
                        "Found unknown contig " + contigSeq);
                }

                Assert.AreEqual(8, result.Scaffolds.Count());
                var expectedScaffolds = new List<string>
                {
                    "ATGCCTCCTATCTTAGCGCGC",
                    "CGCGCG",
                    "CGCGCCGCGC",
                    "TTTTTA",
                    "TTTTTT",
                    "TTTTAGC",
                    "TTTTAA",
                    "TTTAAA",
                };

                foreach (ISequence scaffold in result.Scaffolds)
                {
                    string scaffoldSeq = scaffold.ConvertToString();
                    Assert.IsTrue(
                        expectedScaffolds.Contains(scaffoldSeq) ||
                        expectedScaffolds.Contains(scaffoldSeq.GetReverseComplement(new char[scaffoldSeq.Length])), 
                        "Found unknown scaffold " + scaffoldSeq);
                }
            }
        }

        /// <summary>
        /// Validates Helper.GetSequenceRange
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetSequenceRange()
        {
            const int kmerLength = 6;
            const int dangleThreshold = 3;
            const int redundantThreshold = 7;

            using (ParallelDeNovoAssembler assembler = new ParallelDeNovoAssembler())
            {
                assembler.KmerLength = kmerLength;
                assembler.DanglingLinksThreshold = dangleThreshold;
                assembler.RedundantPathLengthThreshold = redundantThreshold;

                assembler.ScaffoldRedundancy = 0;
                assembler.Depth = 3;
                CloneLibrary.Instance.AddLibrary("abc", (float)5, (float)20);

                PadenaAssembly result = (PadenaAssembly)assembler.Assemble(GetReadsForScaffolds(), true);
                ISequence sequence = result.ContigSequences[0];
                ISequence seqRange = Helper.GetSequenceRange(sequence, 2, 3);
                Assert.AreEqual(3, seqRange.Count);

                string sequenceStr = new string(sequence.Select(a => (char)a).ToArray());
                string seqRangeStr = new string(seqRange.Select(a => (char)a).ToArray());
                Assert.IsTrue(sequenceStr.Contains(seqRangeStr));

            }
        }

        /// <summary>
        /// Validates Helper.ConvertSequenceToString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateConvertSequenceToString()
        {
            ISequence sequence = new Sequence(Alphabets.DNA, "ATGCCTC");
            string sequenceStr = Helper.ConvertSequenceToString(sequence);
            Assert.AreEqual("ATGCCTC", sequenceStr);
        }

        /// <summary>
        /// Validates Helper.SplitSequence
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateSplitSequence()
        {
            Sequence sequence = new Sequence(Alphabets.DNA, "ATGCCTCATGCC");
            List<ISequence> sequenceList = Helper.SplitSequence(sequence, 3, 3);
            Assert.AreEqual(12, sequenceList.Count);

            foreach (ISequence seq in sequenceList)
            {
                Assert.IsTrue("ATGCCTCATGCC".Contains(Helper.ConvertSequenceToString(seq)));
            }
        }

        /// <summary>
        /// Validates Helper.GetHexString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetHexString()
        {
            byte[] sequence = "ATGCCTCATGCC".Select(a => (byte)a).ToArray();
            string hexString = Helper.GetHexString(sequence, 0, 11);
            Assert.IsNotNull(hexString);
        }

        /// <summary>
        /// Validates Helper.IsBAM
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateIsBAM()
        {
            string filename = "bamfile.bam";
            bool isBam = Helper.IsBAM(filename);
            Assert.IsTrue(isBam);

        }

        /// <summary>
        /// Validates Helper.GetStrandType by passing string
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetStrandTypeByString()
        {
            string SingleStrand = "ss-";
            string DoubleStrand = "ds-";
            string MixedStrand = "ms-";
            string noneStrand = "";
            Assert.AreEqual(SequenceStrandType.Single, Helper.GetStrandType(SingleStrand));
            Assert.AreEqual(SequenceStrandType.Double, Helper.GetStrandType(DoubleStrand));
            Assert.AreEqual(SequenceStrandType.Mixed, Helper.GetStrandType(MixedStrand));
            Assert.AreEqual(SequenceStrandType.None, Helper.GetStrandType(noneStrand));
        }

        /// <summary>
        /// Validates Helper.GetStrandType by passing strand type
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetStrandTypeByStrandType()
        {
            string SingleStrand = "ss-";
            string DoubleStrand = "ds-";
            string MixedStrand = "ms-";
            string noneStrand = "";
            Assert.AreEqual(SingleStrand, Helper.GetStrandType(SequenceStrandType.Single));
            Assert.AreEqual(DoubleStrand, Helper.GetStrandType(SequenceStrandType.Double));
            Assert.AreEqual(MixedStrand, Helper.GetStrandType(SequenceStrandType.Mixed));
            Assert.AreEqual(noneStrand, Helper.GetStrandType(SequenceStrandType.None));
        }

        /// <summary>
        /// Validates Helper.GetStrandTopology
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetStrandTopology()
        {
            string LinearStrandTopology = "linear";
            string CircularStrandTopology = "circular";
            string noneStrandTopology = "";
            Assert.AreEqual(SequenceStrandTopology.Linear, Helper.GetStrandTopology(LinearStrandTopology));
            Assert.AreEqual(SequenceStrandTopology.Circular, Helper.GetStrandTopology(CircularStrandTopology));
            Assert.AreEqual(SequenceStrandTopology.None, Helper.GetStrandTopology(noneStrandTopology));
        }

        /// <summary>
        /// Validates Helper.CreateTabString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateCreateTabString()
        {
            object[] strings = { "str1", "str2", "str3" };
            string tabbedString = Helper.CreateTabString(strings);
            string expectedoutput = strings[0] + "\t" + strings[1] + "\t" + strings[2];
            Assert.AreEqual(expectedoutput, tabbedString);
        }


        /// <summary>
        /// Validates Helper.CreateDelimitedString
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateCreateDelimitedString()
        {
            object[] strings = { "str1", "str2", "str3" };
            string delimitedString = Helper.CreateDelimitedString("|", strings);
            string expectedOutput = strings[0] + "|" + strings[1] + "|" + strings[2];
            Assert.AreEqual(expectedOutput, delimitedString);
        }

        /// <summary>
        /// Validates Helper.GetLittleEndianByteArray - Int16
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetLittleEndianByteArrayInt16()
        {
            Int16 value = 6;
            byte[] byteArray = Helper.GetLittleEndianByteArray(value);
            Assert.AreEqual((byte)(value & 0x00FF), byteArray[0]);
            Assert.AreEqual((byte)((value & 0xFF00) >> 8), byteArray[1]);
        }

        /// <summary>
        /// Validates Helper.GetLittleEndianByteArray - UInt16
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetLittleEndianByteArrayUInt16()
        {
            UInt16 value = 6;
            byte[] byteArray = Helper.GetLittleEndianByteArray(value);
            Assert.AreEqual((byte)(value & 0x00FF), byteArray[0]);
            Assert.AreEqual((byte)((value & 0xFF00) >> 8), byteArray[1]);
        }

        /// <summary>
        /// Validates Helper.GetLittleEndianByteArray - int
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetLittleEndianByteArrayint()
        {
            int value = 6;
            byte[] byteArray = Helper.GetLittleEndianByteArray(value);
            Assert.AreEqual((byte)(value & 0x00FF), byteArray[0]);
            Assert.AreEqual((byte)((value & 0xFF00) >> 8), byteArray[1]);
            Assert.AreEqual((byte)((value & 0x00FF0000) >> 16), byteArray[2]);
            Assert.AreEqual((byte)((value & 0xFF000000) >> 24), byteArray[3]);
        }

        /// <summary>
        /// Validates Helper.GetLittleEndianByteArray - uint
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetLittleEndianByteArrayuint()
        {
            uint value = 6;
            byte[] byteArray = Helper.GetLittleEndianByteArray(value);
            Assert.AreEqual((byte)(value & 0x00FF), byteArray[0]);
            Assert.AreEqual((byte)((value & 0xFF00) >> 8), byteArray[1]);
            Assert.AreEqual((byte)((value & 0x00FF0000) >> 16), byteArray[2]);
            Assert.AreEqual((byte)((value & 0xFF000000) >> 24), byteArray[3]);
        }

        /// <summary>
        /// Validates Helper.GetLittleEndianByteArray - float
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGetLittleEndianByteArrayFloat()
        {
            float value = 1.23F;
            byte[] byteArray = Helper.GetLittleEndianByteArray(value);
            Assert.AreEqual(BitConverter.GetBytes(value).ToString(), byteArray.ToString());
        }

        #endregion Bvt Test cases

        #region Helper methods

        /// <summary>
        /// Gets reads required for scaffolds.
        /// </summary>
        private static List<ISequence> GetReadsForScaffolds()
        {
            List<ISequence> sequences = new List<ISequence>();
            Sequence seq = new Sequence(Alphabets.DNA, "ATGCCTC");
            seq.ID = ">10.x1:abc";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "CCTCCTAT");
            seq.ID = "1";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TCCTATC");
            seq.ID = "2";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TGCCTCCT");
            seq.ID = "3";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "ATCTTAGC");
            seq.ID = "4";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "CTATCTTAG");
            seq.ID = "5";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "CTTAGCG");
            seq.ID = "6";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "GCCTCCTAT");
            seq.ID = ">8.x1:abc";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TAGCGCGCTA");
            seq.ID = ">8.y1:abc";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "AGCGCGC");
            seq.ID = ">9.x1:abc";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TTTTTT");
            seq.ID = "7";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TTTTTAAA");
            seq.ID = "8";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TAAAAA");
            seq.ID = "9";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TTTTAG");
            seq.ID = "10";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "TTTAGC");
            seq.ID = "11";
            sequences.Add(seq);
            seq = new Sequence(Alphabets.DNA, "GCGCGCCGCGCG");
            seq.ID = "12";
            sequences.Add(seq);
            return sequences;
        }
        #endregion Helper methods
    }
}
