using System;
using Bio.Algorithms.Translation;
using NUnit.Framework;
using Bio;

namespace Bio.Tests.Algorithms.Translation
{
    /// <summary>
    /// Test the Translation classes.
    /// </summary>
    [TestFixture]
    public class TranslationTests
    {
        /// <summary>
        /// Test the Transcription class.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestTranscription()
        {
            ISequence seq = new Sequence(Alphabets.DNA, "ATGGCG");
            ISequence transcript = Transcription.Transcribe(seq);
            Assert.IsTrue(CompareSequenceToString("AUGGCG", transcript));
            
            Assert.AreEqual(Alphabets.RNA, transcript.Alphabet);

            ISequence reverseTranscript = Transcription.ReverseTranscribe(transcript);
            Assert.IsTrue(CompareSequenceToString("ATGGCG", reverseTranscript));
            Assert.AreEqual(Alphabets.DNA, reverseTranscript.Alphabet);
        }

        /// <summary>
        /// Test the Transcription class.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestTranscriptionSmallCase()
        {
            ISequence seq = new Sequence(Alphabets.DNA, "atggcg");
            ISequence transcript = Transcription.Transcribe(seq);
            Assert.IsTrue(CompareSequenceToString("auggcg", transcript));

            Assert.AreEqual(Alphabets.RNA, transcript.Alphabet);

            ISequence reverseTranscript = Transcription.ReverseTranscribe(transcript);
            Assert.IsTrue(CompareSequenceToString("atggcg", reverseTranscript));
            Assert.AreEqual(Alphabets.DNA, reverseTranscript.Alphabet);
        }

        /// <summary>
        /// Test the Transcription class.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestTranscriptionAmbiguousData()
        {
            ISequence seq = new Sequence(Alphabets.AmbiguousDNA, "MSRWY");
            ISequence transcript = Transcription.Transcribe(seq);
            Assert.IsTrue(CompareSequenceToString("MSRWY", transcript));

            Assert.AreEqual(Alphabets.AmbiguousRNA, transcript.Alphabet);

            ISequence reverseTranscript = Transcription.ReverseTranscribe(transcript);
            Assert.IsTrue(CompareSequenceToString("MSRWY", reverseTranscript));
            Assert.AreEqual(Alphabets.AmbiguousDNA, reverseTranscript.Alphabet);
        }

        /// <summary>
        /// Test the Transcription class.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestTranscriptionInvalidData()
        {
            try
            {
                ISequence seq = new Sequence(Alphabets.AmbiguousRNA, "MSRWY");
                ISequence transcript = Transcription.Transcribe(seq);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }

            try
            {
                ISequence seq = new Sequence(Alphabets.RNA, "AUGGCG");
                ISequence transcript = Transcription.Transcribe(seq);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }

            try
            {
                ISequence seq = new Sequence(Alphabets.DNA, "ATGGCG");
                ISequence transcript = Transcription.ReverseTranscribe(seq);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }

            try
            {
                ISequence seq = new Sequence(Alphabets.AmbiguousDNA, "MSRWY");
                ISequence transcript = Transcription.ReverseTranscribe(seq);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }
        }

        /// <summary>
        /// Test the Transcription class.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestProteinTranslation()
        {
            Sequence rnaSeq = new Sequence(Alphabets.RNA, "AUGCGCCCG");
            ISequence phase1 = ProteinTranslation.Translate(rnaSeq);
            Assert.IsTrue(CompareSequenceToString("MRP", phase1));
            Assert.AreEqual(Alphabets.Protein, phase1.Alphabet);

            rnaSeq = new Sequence(Alphabets.RNA, "AUGCGCCCG");
            phase1 = ProteinTranslation.Translate(rnaSeq, 0);
            Assert.IsTrue(CompareSequenceToString("MRP", phase1));
            Assert.AreEqual(Alphabets.Protein, phase1.Alphabet);

            rnaSeq = new Sequence(Alphabets.RNA, "AUGCGCCCG");
            phase1 = ProteinTranslation.Translate(rnaSeq, 1);
            Assert.IsTrue(CompareSequenceToString("CA", phase1));
            Assert.AreEqual(Alphabets.Protein, phase1.Alphabet);

            rnaSeq = new Sequence(Alphabets.AmbiguousRNA, "NCUCCAUCUUNUUGGAACAAA");
            phase1 = ProteinTranslation.Translate(rnaSeq, 0);
            Assert.IsTrue(CompareSequenceToString("XPSXWNK", phase1));
            Assert.AreEqual(Alphabets.AmbiguousProtein, phase1.Alphabet);
        }

        /// <summary>
        /// Test the Transcription class.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestProteinTranslationInvalid()
        {
            try
            {
                Sequence rnaSeq = new Sequence(Alphabets.RNA, "AUGCGCCCG");
                ISequence phase1 = ProteinTranslation.Translate(rnaSeq, 100);
                Assert.Fail();
            }
            catch(ArgumentException)
            {
            }

            try
            {
                Sequence dnaSeq = new Sequence(Alphabets.DNA, "ATGC");
                ISequence phase1 = ProteinTranslation.Translate(dnaSeq);
                Assert.Fail();
            }
            catch(InvalidOperationException)
            {
            }

        }

        private bool CompareSequenceToString(string reference, ISequence sequence)
        {
            if ((string.IsNullOrEmpty(reference) || sequence == null) || reference.Length != sequence.Count)
            {
                return false;
            }

            for (int i = 0; i < sequence.Count; i++)
            {
                if (sequence[i] != reference[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
