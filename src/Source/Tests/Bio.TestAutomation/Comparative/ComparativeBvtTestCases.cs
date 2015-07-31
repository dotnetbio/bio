/****************************************************************************
 * ComparativeBvtTestCases.cs
 * 
 *  This file contains the Comparative Bvt test cases.
 * 
***************************************************************************/

using Bio.Algorithms.Alignment;
using Bio.Extensions;
using Bio.IO.FastA;
using Bio.Util.Logging;
using System;
using System.IO;
using Bio.Algorithms.Assembly.Comparative;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.TestAutomation.Util;
using System.Globalization;
using Bio.Util;

namespace Bio.TestAutomation.Algorithms.Assembly.Comparative
{
    /// <summary>
    /// Comparative Bvt test cases
    /// </summary>
    [TestClass]
    public class ComparativeBvtTestCases
    {
        #region Global Variables

        Utility utilityObj = new Utility(@"TestUtils\ComparativeTestData\ComparativeTestsConfig.xml");
        ASCIIEncoding encodingObj = new ASCIIEncoding();

        #endregion Global Variables

        #region Constructor

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static ComparativeBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("Bio.automation.log");
            }
        }

        #endregion Constructor

        # region ComparativeBvtTestCases

        # region Step2 test cases

        /// <summary>
        /// Validates ResolveAmbiguity().
        /// The input reference sequences and reads are present in fasta files.
        /// Input data:One line Dna sequence.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateRepeatResolutionWithDnaFile()
        {
            RepeatResolution(Constants.RepeatResolution, true);
        }

        /// <summary>
        ///  Validates ResolveAmbiguity().
        ///  The input reference sequences and reads are taken from Configuration file.
        ///  Input data:One line Dna sequence.
        /// </summary>
        /// This is now commented as the new implementation only supports file.
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateRepeatResolutionWithDnaString()
        {
            RepeatResolution(Constants.RepeatResolution, false);
        }

        /// <summary>
        ///  Validates ResolveAmbiguity().
        ///  Input: Paired Reads with zero errors and 1X coverage.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateRepeatResolutionWithZeroErrorsAndPairedReads1X()
        {
            RepeatResolution(Constants.RepeatResolutionWithPairedReadsNode, true);
        }

        /// <summary>
        ///  Validates ResolveAmbiguity().
        ///  Input: Paired Reads with Errors and 1X coverage.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateRepeatResolutionWithErrorsAndPairedReads1X()
        {
            RepeatResolution(Constants.RepeatResolutionWithErrorsInPairedReadsNode, true);
        }


        # endregion Step2 test cases

        # region Step3 test cases

        /// <summary>
        /// Validates RefineLayout().
        /// The input reference sequences and reads are present in fasta files.
        /// Input data:One line Dna sequence.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateRefineLayoutWithDnaFile()
        {
            RefineLayout(Constants.RefineLayout, true);
        }

        /// <summary>
        ///  Validates RefineLayout().
        ///  The input reference sequences and reads are taken from Configuration file.
        ///  Input data:One line Dna sequence.
        /// </summary>
        /// This is now commented as the new implementation only supports file.
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateRefineLayoutWithDnaString()
        {
            RefineLayout(Constants.RefineLayout, false);
        }

        /// <summary>
        ///  Validates RefineLayout().
        ///  Input: Paired Reads with zero errors and 1X coverage.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateRefineLayoutWithZeroErrorsAndPairedReads1X()
        {
            RefineLayout(Constants.RefineLayoutWithPairedReadsNode, true);
        }

        # endregion Step3 test cases

        # region Step4 test cases

        /// <summary>
        /// Validate GenerateConsensus().
        /// Inputs(references and reads) having one line sequences present in a file.        
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGenerateConsensusForDnaSequencesFromFile()
        {
            ValidateGenerateConsensus(Constants.SimpleFastaNodeName, true);
        }

        /// <summary>
        /// This method will validate GenerateConsensus().
        /// Inputs(references and reads) with one line sequences .        
        /// </summary>
        /// This is now commented as the new implementation only supports file.
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGenerateConsensusForDnaSequencesFromString()
        {
            ValidateGenerateConsensus(Constants.SimpleFastaNodeName, false);
        }

        /// <summary>
        ///  Validates GenerateConsensus().
        ///  Input: Paired Reads with zero errors and 1X coverage.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateGenerateConsensusWithZeroErrorsAndPairedReads1X()
        {
            ValidateGenerateConsensus(Constants.EColi500WithZeroError1XPairedReadsNode, true);
        }

        # endregion Step4 test cases

        # region Step 1-5 test cases

        # region With Errors.

        /// <summary>
        /// Validate Assemble().
        /// Inputs(references and reads) having one line sequences with reads having few Nucleotides added.        
        /// </summary>
        /// This is now commented as the new implementation only supports file.
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateAssembleWithAddedNucloetides()
        {
            ValidateComparativeAssembleMethod(Constants.FastaOneLineSequenceWithAdditionNode);
        }

        /// <summary>
        /// Validate Assemble().
        /// Inputs(references and reads) having one line sequences with reads having few Nucleotides deleted.        
        /// </summary>
        /// This is now commented as the new implementation only supports file.
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateAssembleWithDeletedNucleotide()
        {
            ValidateComparativeAssembleMethod(Constants.FastaOneLineSequenceWithDeletionNode);
        }

        /// <summary>
        /// Validate Assemble().
        /// Input: One line dna sequence and reads with .001 % errors.
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateAssembleWithOneLineSequenceWithErrors()
        {
            ValidateComparativeAssembleMethod(Constants.FastaWithOneLineSequenceNodeWithErrors);
        }


        # endregion With Errors.

        # region Without errors.

        /// <summary>
        /// Validate Assemble().
        /// Inputs(references and reads) having one line sequences present in a file.        
        /// Output: The reads are generated manually on reference file and the output of Assemble
        /// is expected to be the sequences in the reference file.
        /// Ref file: ATGCAGTA
        /// Read's file: ATGC,AGTA
        /// Expected Output: ATGCAGTA
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateAssembleWithOneLineSequence()
        {
            ValidateComparativeAssembleMethod(Constants.FastaWithOneLineSequenceNode);
        }

        /// <summary>
        /// Validate Assemble().
        /// Input: Zero error's 10x coverage, without paired reads
        /// Output: The reads are generated using Read Simulator on reference file and the output of Assemble
        /// is expected to be the sequences in the reference file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateAssembleForZeroErrors10XCoverageWithoutPairedReads()
        {
            ValidateComparativeAssembleMethod(Constants.SequenceWithZeroErrorsAnd10XCoverageNode);
        }

        /// <summary>
        /// Validate Assemble().
        /// Input: Zero error's 1x coverage, without paired reads
        /// Output: The reads are generated using Read Simulator on reference file and the output of Assemble
        /// is expected to be the sequences in the reference file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateAssembleForZeroErrors1XCoverageWithoutPairedReads()
        {
            ValidateComparativeAssembleMethod(Constants.SequenceWithZeroErrorsAnd1XCoverageNode);
        }

        /// <summary>
        /// Validate Assemble().
        /// Input: Zero error's 10x coverage, with paired reads
        /// Output: The reads are generated using SeqGen.exe on reference file and the output of Assemble
        /// is expected to be the sequences in the reference file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateAssembleForZeroErrors10XCoverageWithPairedReads()
        {
            ValidateComparativeAssembleMethod(Constants.SequenceWithZeroErrorsAnd10XCoveragePairedNode);
        }

        /// <summary>
        /// Validate Assemble().
        /// Input: Zero error's 1x coverage, with paired reads
        /// Output: The reads are generated using SeqGen.exe on reference file and the output of Assemble
        /// is expected to be the sequences in the reference file.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateAssembleForZeroErrors1XCoverageWithPairedReads()
        {
            ValidateComparativeAssembleMethod(Constants.SequenceWithZeroErrorsAnd1XCoveragePairedNode);
        }

        # endregion Without errors.

        # endregion Step 1-5 test cases

        # endregion ComparativeBvtTestCases

        # region Supporting methods

        /// <summary>
        /// Validates Assemble method .Step 1-5.        
        /// </summary>
        /// <param name="nodeName">Parent Node name in Xml</param>
        public void ValidateComparativeAssembleMethod(string nodeName)
        {
            ComparativeGenomeAssembler assemble = new ComparativeGenomeAssembler();
            List<ISequence> referenceSeqList;
            StringBuilder expectedSequence = new StringBuilder(utilityObj.xmlUtil.GetTextValue(nodeName, Constants.ExpectedSequenceNode));

            string LengthOfMUM = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MUMLengthNode);
            string kmerLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.KmerLengthNode);
            string fixedSeparation = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FixedSeparationNode);
            string minimumScore = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MinimumScoreNode);
            string separationFactor = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SeparationFactorNode);
            string maximumSeparation = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.MaximumSeparationNode);
            string breakLength = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.BreakLengthNode);

            // Gets the reference sequence from the FastA file
            string filePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode1);

            Assert.IsNotNull(filePath);
            ApplicationLog.WriteLine(string.Format(null, "Comparative BVT : Successfully validated the File Path '{0}'.", filePath));

            using (FastAParser parser = new FastAParser(filePath))
            {
                IEnumerable<ISequence> referenceList = parser.Parse();
                Assert.IsNotNull(referenceList);
                referenceSeqList = new List<ISequence>(referenceList);
            }

            // Get the reads from configuration file.
            string readFilePath = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode2);

            assemble.LengthOfMum = int.Parse(LengthOfMUM,  CultureInfo.InvariantCulture);
            assemble.KmerLength = int.Parse(kmerLength,  CultureInfo.InvariantCulture);
            assemble.ScaffoldingEnabled = true;
            assemble.FixedSeparation = int.Parse(fixedSeparation, CultureInfo.InvariantCulture);
            assemble.MinimumScore = int.Parse(minimumScore, CultureInfo.InvariantCulture);
            assemble.SeparationFactor = float.Parse(separationFactor, CultureInfo.InvariantCulture);
            assemble.MaximumSeparation = int.Parse(maximumSeparation, CultureInfo.InvariantCulture);
            assemble.BreakLength = int.Parse(breakLength, CultureInfo.InvariantCulture);

            using (var queryparser = new FastASequencePositionParser(readFilePath))
            {
                IEnumerable<ISequence> output = assemble.Assemble(referenceSeqList, queryparser);
                StringBuilder longOutput = new StringBuilder();
                foreach (string x in output.Select(seq => seq.ConvertToString()).OrderBy(c => c))
                    longOutput.Append(x);

                Assert.AreEqual(expectedSequence.ToString().ToUpperInvariant(), longOutput.ToString().ToUpperInvariant());
            }
        }

        /// <summary>
        /// This method is Step 4 in Comparative Assembly.
        /// It validates GenerateConsensus method.
        /// </summary>
        /// <param name="nodeName">Parent Node name in Xml</param>
        /// <param name="isFilePath">Sequence location.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public void ValidateGenerateConsensus(string nodeName, bool isFilePath)
        {
            IList<IEnumerable<DeltaAlignment>> deltaValues = GetDeltaAlignment(nodeName, isFilePath);
            List<DeltaAlignment> result = new List<DeltaAlignment>();
            string deltaFile = Path.GetTempFileName();
            string readsFile = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode2);

            while (deltaValues.Count > 0)
            {
                IEnumerable<DeltaAlignment> curReadDeltas = deltaValues.ElementAt(0);
                deltaValues.RemoveAt(0); // remove currently processing item from the list

                result.Add(curReadDeltas.ElementAt(0));
            }

            ComparativeGenomeAssembler.WriteDelta(result.OrderBy(a => a.FirstSequenceStart), deltaFile);
            var daCollectionObj = new DeltaAlignmentCollection(deltaFile, readsFile);

            IEnumerable<ISequence> assembledOutput =
                ConsensusGeneration.GenerateConsensus(daCollectionObj);

            int index = 0;

            //Validate consensus sequences. 
            foreach (ISequence seq in assembledOutput)
            {
                string[] expectedValue = utilityObj.xmlUtil.GetTextValues(nodeName,
                                      Constants.ExpectedSequenceNode);
                string actualOutput = new string(seq.Select(a => (char)a).ToArray());

                Assert.AreEqual(expectedValue[index].ToUpperInvariant(), actualOutput.ToUpperInvariant());
                index++;
            }
        }

        /// <summary>
        /// Aligns reads to reference genome using NUCmer.
        /// </summary>
        /// <param name="nodeName">Name of parent Node which contains the data in xml.</param>
        /// <param name="isFilePath">Represents sequence is in a file or not.</param>
        /// <returns>Delta i.e. output from NUCmer</returns>      
        IList<IEnumerable<DeltaAlignment>> GetDeltaAlignment(string nodeName, bool isFilePath)
        {
            string[] referenceSequences = null;
            string[] searchSequences = null;

            List<ISequence> referenceSeqList = new List<ISequence>();
            List<ISequence> searchSeqList = new List<ISequence>();
            IList<IEnumerable<DeltaAlignment>> results = new List<IEnumerable<DeltaAlignment>>();

            if (isFilePath)
            {
                // Gets the reference sequence from the FastA file
                string filePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.FilePathNode1);

                Assert.IsNotNull(filePath);
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "Comparative BVT : Successfully validated the File Path '{0}'.", filePath));

                using (FastASequencePositionParser parser = new FastASequencePositionParser(filePath))
                {
                    IEnumerable<ISequence> referenceList = parser.Parse();

                    foreach (ISequence seq in referenceList)
                    {
                        referenceSeqList.Add(seq);
                    }

                    // Gets the query sequence from the FastA file
                    string queryFilePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                        Constants.FilePathNode2);

                    Assert.IsNotNull(queryFilePath);
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "Comparative BVT : Successfully validated the File Path '{0}'.", queryFilePath));

                    using (FastASequencePositionParser queryParser = new FastASequencePositionParser(queryFilePath))
                    {
                        IEnumerable<ISequence> querySeqList = queryParser.Parse();

                        foreach (ISequence seq in querySeqList)
                        {
                            searchSeqList.Add(seq);
                        }
                    }
                }
            }
            else
            {
                // Gets the reference & search sequences from the configurtion file
                referenceSequences = utilityObj.xmlUtil.GetTextValues(nodeName,
                    Constants.ReferenceSequencesNode);
                searchSequences = utilityObj.xmlUtil.GetTextValues(nodeName,
                  Constants.SearchSequencesNode);

                IAlphabet seqAlphabet = Utility.GetAlphabet(utilityObj.xmlUtil.GetTextValue(nodeName,
                       Constants.AlphabetNameNode));

                for (int i = 0; i < referenceSequences.Length; i++)
                {
                    ISequence referSeq = new Sequence(seqAlphabet, encodingObj.GetBytes(referenceSequences[i]));
                    referenceSeqList.Add(referSeq);
                }

                string[] seqArray = searchSequences.ElementAt(0).Split(',');

                searchSeqList.AddRange(seqArray.Select(t => new Sequence(seqAlphabet, encodingObj.GetBytes(t))).Cast<ISequence>());
            }

            foreach (ISequence reference in referenceSeqList)
            {
                NUCmer nucmerAligner = new NUCmer((Sequence)reference);

                string fixedSeparation = utilityObj.xmlUtil.GetTextValue(nodeName,
                         Constants.FixedSeparationNode);
                string minimumScore = utilityObj.xmlUtil.GetTextValue(nodeName,
                         Constants.MinimumScoreNode);
                string separationFactor = utilityObj.xmlUtil.GetTextValue(nodeName,
                         Constants.SeparationFactorNode);
                string LengthOfMUM = utilityObj.xmlUtil.GetTextValue(nodeName,
                         Constants.MUMLengthNode);

                nucmerAligner.FixedSeparation = int.Parse(fixedSeparation);
                nucmerAligner.MinimumScore = int.Parse(minimumScore);
                nucmerAligner.SeparationFactor = int.Parse(separationFactor);
                nucmerAligner.LengthOfMUM = int.Parse(LengthOfMUM);

                foreach (ISequence querySeq in searchSeqList)
                {
                    results.Add(nucmerAligner.GetDeltaAlignments(querySeq, true));
                }
            }

            return results;
        }

        /// <summary>
        /// Converts List to Ienumerable 
        /// </summary>
        /// <param name="list">list of ienumberable</param>
        /// <returns>Delta Alignments</returns>
        IEnumerable<DeltaAlignment> CovertListToIEnumerable(IEnumerable<IEnumerable<DeltaAlignment>> list)
        {
            return list.SelectMany(iEnumDelta => iEnumDelta);
        }

        /// <summary>
        /// Validate Step 2 of Comparative assembly i.e. ResolveAmbiguity() method.
        /// Input sequence does not contain repeats.
        /// </summary>
        /// <param name="nodeName">Parent node in Xml</param>
        /// <param name="isFilePath">Represents sequence is in a file or not.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public void RepeatResolution(string nodeName, bool isFilePath)
        {
            IList<IEnumerable<DeltaAlignment>> deltaValues = GetDeltaAlignment(nodeName, isFilePath);
            string deltaFile = Path.GetTempFileName();
            string readsFile = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FilePathNode2);
            IEnumerable<DeltaAlignment> repeatDeltaValues = null;

            ComparativeGenomeAssembler.WriteDelta(this.CovertListToIEnumerable(deltaValues), deltaFile);
            DeltaAlignmentCollection daCollectionObj = new DeltaAlignmentCollection(deltaFile, readsFile);
            repeatDeltaValues = RepeatResolver.ResolveAmbiguity(daCollectionObj);

            //Compare the delta's with the expected .
            Assert.IsTrue(CompareDeltaValues(nodeName, repeatDeltaValues));
        }

        /// <summary>
        /// Validate Step 3 of Comparative assembly i.e. RefineLayout() method.
        /// <param name="nodeName">Parent node in Xml</param>
        /// <param name="isFilePath">Represents sequence is in a file or not.</param>
        /// </summary>
        public void RefineLayout(string nodeName, bool isFilePath)
        {
            List<DeltaAlignment> repeatDeltaValues = null;
            IList<IEnumerable<DeltaAlignment>> deltaValues = GetDeltaAlignment(nodeName, isFilePath);
            List<DeltaAlignment> result = new List<DeltaAlignment>();
            string deltaFile = Path.GetTempFileName();

            while (deltaValues.Count > 0)
            {
                IEnumerable<DeltaAlignment> curReadDeltas = deltaValues.ElementAt(0);
                deltaValues.RemoveAt(0); // remove currently processing item from the list

                result.Add(curReadDeltas.ElementAt(0));
            }

            List<DeltaAlignment> orderedRepeatResolvedDeltas = result.OrderBy(a => a.FirstSequenceStart).ToList();
            ComparativeGenomeAssembler.WriteDelta(orderedRepeatResolvedDeltas, deltaFile);

            string readFilePath = utilityObj.xmlUtil.GetTextValue(nodeName,
                    Constants.FilePathNode2);

            using (var deltaColl = new DeltaAlignmentCollection(deltaFile, readFilePath))
            {

                //Validate RefineLayout().
                LayoutRefiner.RefineLayout(deltaColl);
                repeatDeltaValues = orderedRepeatResolvedDeltas;
            }
            //Compare the delta's with the expected .
            Assert.IsTrue(CompareDeltaValues(nodeName, repeatDeltaValues));
        }

        /// <summary>
        /// Compares the values of Delta Alignments.
        /// </summary>
        /// <param name="nodeName">Parent node in Xml.</param>
        /// <param name="deltaValues">List of Delta Alignments which are to be compared.</param>
        /// <returns>True is the deltas are same,otherwise false.</returns>
        public bool CompareDeltaValues(string nodeName, IEnumerable<DeltaAlignment> deltaValues)
        {
            //Get the expected values from configurtion file .
            string[] firstSeqEnd = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FirstSequenceEndNode).Split(',');
            string[] firstSeqStart = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.FirstSequenceStartNode).Split(',');
            string[] deltaReferencePosition = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.DeltasNode).Split(',');
            string[] secondSeqStart = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SecondSequenceStart).Split(',');
            string[] secondSeqEnd = utilityObj.xmlUtil.GetTextValue(nodeName, Constants.SecondSequenceEndNode).Split(',');
            
            var deltas = deltaValues;
            for (int index = 0; index < deltas.Count(); index++)
            {
                if ((0 != string.Compare(firstSeqStart[index], deltas.ElementAt(index).FirstSequenceStart.ToString((IFormatProvider)null), StringComparison.CurrentCulture))
                   || (0 != string.Compare(firstSeqEnd[index], deltas.ElementAt(index).FirstSequenceEnd.ToString((IFormatProvider)null), StringComparison.CurrentCulture))
                   || (0 != string.Compare(deltaReferencePosition[index], deltas.ElementAt(index).DeltaReferencePosition.ToString((IFormatProvider)null), StringComparison.CurrentCulture))
                   || (0 != string.Compare(secondSeqStart[index], deltas.ElementAt(index).SecondSequenceStart.ToString((IFormatProvider)null), StringComparison.CurrentCulture))
                   || (0 != string.Compare(secondSeqEnd[index], deltas.ElementAt(index).SecondSequenceEnd.ToString((IFormatProvider)null), StringComparison.CurrentCulture)))
                {
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null, "Delta Alignment : There is no match at '{0}'", index.ToString((IFormatProvider)null)));
                    return false;
                }
            }

            return true;
        }

        # endregion Supporting methods
    }
}


