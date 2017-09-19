using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Bio.Extensions;
using Bio.IO.FastA;
using Bio.TestAutomation.Util;
using Bio.Util.Logging;

using NUnit.Framework;

namespace Bio.Tests
{
    /// <summary>
    /// Test Automation code for Bio Sequences and BVT level validations.
    /// </summary>
    [TestFixture]
    public class SequenceBvtTestCases
    {
        #region Enum

        /// <summary>
        /// Sequence method types to validate different test cases
        /// </summary>
        private enum SequenceMethods
        {
            Complement,
            Reverse,
            ReverseComplement
        }

        #endregion

        readonly Utility utilityObj = new Utility(@"TestUtils\TestsConfig.xml");

        #region Sequence Bvt TestCases

        /// <summary>
        /// Validate a creation of DNA Sequence by passing valid Single Character sequence.
        /// Input Data : Valid DNA Sequence with single character - "A".
        /// Output Data : Validation of created DNA Sequence.
        /// </summary>
        [Category("Priority0")]
        public void ValidateSingleCharDnaSequence()
        {
            // Gets the actual sequence and the alphabet from the Xml
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleDnaAlphabetNode, Constants.AlphabetNameNode);
            string actualSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleDnaAlphabetNode, Constants.ExpectedSingleChar);

            // Logs information to the log file
            ApplicationLog.WriteLine(string.Concat(
                "Sequence BVT: Sequence is as expected."));

            Sequence createSequence = new Sequence(
                Utility.GetAlphabet(alphabetName), actualSequence);

            Assert.IsNotNull(createSequence);

            // Validate the createdSequence            
            string seqNew = new string(createSequence.Select(a => (char)a).ToArray());
            Assert.AreEqual(seqNew, actualSequence);

            ApplicationLog.WriteLine(string.Concat(
                "Sequence BVT: Sequence is as expected."));

            Assert.AreEqual(Utility.GetAlphabet(alphabetName), createSequence.Alphabet);
            ApplicationLog.WriteLine(string.Concat(
                "Sequence BVT: Sequence Alphabet is as expected."));

            ApplicationLog.WriteLine(
                "Sequence BVT: The DNA with single character Sequence is completed successfully.");
        }

        /// <summary>
        /// Validate a creation of DNA Sequence by passing valid string.
        /// Input Data: Valid DNA sequence "ACGA".
        /// Output Data : Validation of created DNA Sequence.
        /// </summary>
        [Category("Priority0")]
        public void ValidateDnaSequence()
        {

            // Gets the actual sequence and the alphabet from the Xml
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleDnaAlphabetNode, Constants.AlphabetNameNode);
            string actualSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleDnaAlphabetNode, Constants.ExpectedNormalString);

            // Logs information to the log file
            ApplicationLog.WriteLine(string.Concat(
                "Sequence BVT: Sequence ", actualSequence, " and Alphabet ", alphabetName));

            ISequence createSequence = new Sequence(Utility.GetAlphabet(alphabetName),actualSequence);
            Assert.IsNotNull(createSequence);

            string seqNew = createSequence.ConvertToString();

            // Validate the createdSequence
            Assert.AreEqual(seqNew, actualSequence);
            ApplicationLog.WriteLine("Sequence BVT: Sequence is as expected.");

            Assert.AreEqual(Utility.GetAlphabet(alphabetName), createSequence.Alphabet);
            ApplicationLog.WriteLine("Sequence BVT: Sequence Alphabet is as expected.");

            ApplicationLog.WriteLine("Sequence BVT: The DNA Sequence with string is created successfully.");
        }

        /// <summary>
        /// Validate a creation of RNA Sequence by passing valid string sequence.
        /// Input Data : Valid RNA Sequence "GAUUCAAGGGCU".
        /// Output Data : Validation of created RNA sequence.
        /// </summary>
        [Category("Priority0")]
        public void ValidateRnaSequence()
        {
            // Gets the actual sequence and the alphabet from the Xml
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleRnaAlphabetNode, Constants.AlphabetNameNode);
            string actualSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleRnaAlphabetNode, Constants.ExpectedNormalString);

            // Logs information to the log file
            ApplicationLog.WriteLine(string.Concat(
                "Sequence BVT: Sequence ", actualSequence, " and Alphabet ", alphabetName));

            Sequence createSequence =new Sequence(Utility.GetAlphabet(alphabetName),
                    actualSequence);
            Assert.IsNotNull(createSequence);

            // Validate the createdSequence
            string seqNew = new string(createSequence.Select(a => (char)a).ToArray());
            Assert.AreEqual(seqNew, actualSequence);
            ApplicationLog.WriteLine(string.Concat(
                "Sequence BVT: Sequence is as expected."));

            Assert.AreEqual(Utility.GetAlphabet(alphabetName), createSequence.Alphabet);
            ApplicationLog.WriteLine(string.Concat(
                "Sequence BVT: Sequence Alphabet is as expected."));
            ApplicationLog.WriteLine(
                "Sequence BVT: The RNA Sequence is created successfully.");
        }

        /// <summary>
        /// Validate a creation of Protein Sequence by passing valid string sequence.
        /// Input Data : Valid Protein sequece "AGTN".
        /// Output Data : Validation of created Protein sequence.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateProteinSequence()
        {
            // Gets the actual sequence and the alphabet from the Xml
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleProteinAlphabetNode, Constants.AlphabetNameNode);
            string actualSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleProteinAlphabetNode, Constants.ExpectedNormalString);

            // Logs information to the log file
            ApplicationLog.WriteLine(string.Concat(
                "Sequence BVT: Sequence ", actualSequence, " and Alphabet ", alphabetName));

            Sequence createSequence = new Sequence(
                Utility.GetAlphabet(alphabetName), actualSequence);
            Assert.IsNotNull(createSequence);

            // Validate the createdSequence
            string seqNew = new string(createSequence.Select(a => (char)a).ToArray());
            Assert.AreEqual(seqNew, actualSequence);

            Assert.AreEqual(Utility.GetAlphabet(alphabetName), createSequence.Alphabet);

            ApplicationLog.WriteLine("Sequence BVT: The Protein Sequence is created successfully.");
        }

        /// <summary>
        /// Validate a Sequence creation for a given FastaA file.
        /// Input Data : Valid FastaA file sequence.
        /// Output Data : Validation of FastaA file sequence.
        /// </summary>
        [Category("Priority0")]
        public void ValidateFastaAFileSequence()
        {
            // Gets the expected sequence from the Xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleFastaNodeName, Constants.ExpectedSequenceNode);
            string fastAFilePath = this.utilityObj.xmlUtil.GetTextValue(
                Constants.SimpleFastaNodeName, Constants.FilePathNode);
            string alphabet = this.utilityObj.xmlUtil.GetTextValue(Constants.SimpleFastaNodeName,
                Constants.AlphabetNameNode);
            Assert.IsTrue(File.Exists(fastAFilePath));

            // Logs information to the log file
            ApplicationLog.WriteLine(string.Concat(
                "Sequence BVT: The File exist in the Path ", fastAFilePath));

            IEnumerable<ISequence> sequence = null;
            FastAParser parser = new FastAParser();
            {
                // Parse a FastA file Using Parse method and convert the same to sequence.
                parser.Alphabet = Utility.GetAlphabet(alphabet);
                sequence = parser.Parse(fastAFilePath);

                Assert.IsNotNull(sequence);
                Sequence fastASequence = (Sequence)sequence.ElementAt(0);
                Assert.IsNotNull(fastASequence);

                char[] seqString = sequence.ElementAt(0).Select(a => (char)a).ToArray();
                string newSequence = new string(seqString);

                Assert.AreEqual(expectedSequence, newSequence);
                ApplicationLog.WriteLine(string.Concat(
                    "Sequence BVT: The Sequence is as expected."));

                byte[] tmpEncodedSeq = new byte[fastASequence.Count];
                (fastASequence as IEnumerable<byte>).ToArray().CopyTo(tmpEncodedSeq, 0);

                Assert.AreEqual(expectedSequence.Length, tmpEncodedSeq.Length);
                ApplicationLog.WriteLine(string.Concat(
                    "Sequence BVT: Sequence Length is as expected."));

                Assert.AreEqual(this.utilityObj.xmlUtil.GetTextValue(
                    Constants.SimpleProteinAlphabetNode, Constants.SequenceIdNode), fastASequence.ID);
                ApplicationLog.WriteLine(string.Concat(
                    "Sequence BVT: SequenceID is as expected."));


                Assert.AreEqual(fastASequence.Alphabet.Name,
                    this.utilityObj.xmlUtil.GetTextValue(Constants.SimpleFastaNodeName, Constants.AlphabetNameNode));
                ApplicationLog.WriteLine(string.Concat(
                    "Sequence BVT: Sequence Alphabet is as expected."));
            }
        }

        /// <summary>
        /// Validates GetReversedSequence method for a given Dna Sequence.
        /// Input Data: AGTACAGCTCCAGACGT
        /// Output Data : Reverse of Input Sequence.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateGetReversedSequence()
        {
            this.ValidateSequences(Constants.DnaDerivedSequenceNode, SequenceMethods.Reverse);
        }

        /// <summary>
        /// Validates GetReverseComplementedSequence method for a given Dna Sequence.
        /// Input Data: AGTACAGCTCCAGACGT
        /// Output Data : Reverse complement of Input Sequence. 
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateGetReverseComplementedSequence()
        {
            this.ValidateSequences(Constants.DnaDerivedSequenceNode, SequenceMethods.ReverseComplement);
        }

        /// <summary>
        /// Validates GetComplementedSequence method for a given Dna Sequence.
        /// Input Data: AGTACAGCTCCAGACGT
        /// Output Data : Complement of Input Dna sequence.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateGetComplementSequence()
        {
            this.ValidateSequences(Constants.DnaDerivedSequenceNode, SequenceMethods.Complement);
        }

        /// <summary>
        /// Validates Sequence Constructor for a given Dna Sequence.
        /// Input Data: AGTACAGCTCCAGACGT
        /// Output Data : Validation of created Sequence.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSequenceConstructor()
        {
            // Get input and expected values from xml
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.AlphabetNameNode);
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(
                Constants.DnaDerivedSequenceNode, Constants.ExpectedDerivedSequence);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            List<byte[]> byteArray = new List<byte[]>
            {
                 Encoding.UTF8.GetBytes(expectedSequence),                 
            };

            // Validating Constructor.
            Sequence constructorSequence = new Sequence(alphabet, byteArray[0]);
            Assert.AreEqual(expectedSequence,
                        new string(constructorSequence.Select(a => (char)a).ToArray()));
            ApplicationLog.WriteLine(string.Concat(
                    "Sequence BVT: Validation of Sequence Constructor completed successfully."));
        }

        /// <summary>
        /// Validates Enumeration in a Sequence.
        /// Input Data: AGTACAGCTCCAGACGT
        /// Output Data : Validation of enumeration.
        [Test]
        [Category("Priority0")]
        public void ValidateEnumerator()
        {
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                                 Constants.DnaDerivedSequenceNode, Constants.AlphabetNameNode);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);
            string expectedSequence = (this.utilityObj.xmlUtil.GetTextValue(
                            Constants.DnaDerivedSequenceNode, Constants.ExpectedDerivedSequence));
            Sequence sequence = new Sequence(alphabet, expectedSequence);
            //Validate Count
            Assert.AreEqual(Encoding.UTF8.GetByteCount(expectedSequence), sequence.Count);
            ApplicationLog.WriteLine(string.Concat(
                "Sequence BVT: Validation of Count operation completed successfully."));

            //Validate Enumerator.
            string sequenceString = "";
            IEnumerator<byte> enumFromSequence = sequence.GetEnumerator();
            while (enumFromSequence.MoveNext())
            {
                sequenceString += ((char)enumFromSequence.Current);
            }

            Assert.AreEqual(sequenceString, expectedSequence);
            ApplicationLog.WriteLine(string.Concat(
              "Sequence BVT: Validation of Enumerator operation completed successfully."));
        }

        /// <summary>
        /// Validate Sequence LastIndexOfNonGap().
        /// Input data : Sequence.
        /// Output Data : Validation of LastIndexOfNonGap() method.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateSequenceLastIndexOfNonGap()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(Constants.RnaDerivedSequenceNode,
                Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(Constants.RnaDerivedSequenceNode,
                Constants.AlphabetNameNode);

            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create a Sequence object.
            Sequence seqObj =
                new Sequence(alphabet, expectedSequence);

            long index = seqObj.LastIndexOfNonGap();

            Assert.AreEqual(expectedSequence.Length - 1, index);
        }

        /// <summary>
        /// Validates CopyTo
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void ValidateCopyTo()
        {
            // Get input and expected values from xml
            string expectedSequence = this.utilityObj.xmlUtil.GetTextValue(Constants.RnaDerivedSequenceNode,
                Constants.ExpectedSequence);
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(Constants.RnaDerivedSequenceNode,
                Constants.AlphabetNameNode);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);

            // Create a Sequence object.
            ISequence iseqObj =
                new Sequence(alphabet, expectedSequence);
            Sequence seqObj = new Sequence(iseqObj);
            byte[] array = new byte[expectedSequence.Length];
            seqObj.CopyTo(array, 0, expectedSequence.Length);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < expectedSequence.Length; i++)
            {
                builder.Append((char)array[i]);
            }
            string actualValue = builder.ToString();
            Assert.AreEqual(expectedSequence, actualValue);

            //check with a part of the expected seq only
            seqObj.CopyTo(array, 0, 5);
            builder = new StringBuilder();
            for (int i = 0; i < 5; i++)
            {
                builder.Append((char)array[i]);
            }
            actualValue = builder.ToString();
            Assert.AreEqual(expectedSequence.Substring(0, 5), actualValue);
        }

        #endregion Sequence Bvt TestCases
        
        #region Supporting Methods

        /// <summary>
        /// Supporting method for validating Sequence operations.
        /// Input Data: Parent node,child node and Enum. 
        /// Output Data : Validation of public methods in Sequence class.
        /// </summary>
        void ValidateSequences(string parentNode, SequenceMethods option)
        {
            string alphabetName = this.utilityObj.xmlUtil.GetTextValue(
                                 parentNode, Constants.AlphabetNameNode);
            IAlphabet alphabet = Utility.GetAlphabet(alphabetName);
            ISequence seq = null;
            string expectedValue = "";
            ISequence sequence = new Sequence(alphabet, Encoding.UTF8.GetBytes(
                                this.utilityObj.xmlUtil.GetTextValue(parentNode, 
                                Constants.ExpectedDerivedSequence)));
            switch (option)
            {
                case SequenceMethods.Reverse:
                    seq = sequence.GetReversedSequence();
                    expectedValue = this.utilityObj.xmlUtil.GetTextValue(
                    parentNode, Constants.Reverse);
                    break;
                case SequenceMethods.ReverseComplement:
                    seq = sequence.GetReverseComplementedSequence();
                    expectedValue = this.utilityObj.xmlUtil.GetTextValue(
                    parentNode, Constants.ReverseComplement);
                    break;
                case SequenceMethods.Complement:
                    seq = sequence.GetComplementedSequence();
                    expectedValue = this.utilityObj.xmlUtil.GetTextValue(
                    parentNode, Constants.Complement);
                    break;
            }

            Assert.AreEqual(expectedValue, seq.ConvertToString());
            ApplicationLog.WriteLine(string.Concat(
                    "Sequence BVT: Validation of Sequence operation ", option, " completed successfully."));

        }

        #endregion Supporting Methods
    }
}

