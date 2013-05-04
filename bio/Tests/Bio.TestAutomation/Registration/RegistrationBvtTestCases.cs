/****************************************************************************
 * RegistrationBvtTestCases.cs
 * 
 *   This file contains the BVT test cases for validation the
 *   registration process in Bio
 * 
***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bio.Algorithms.Alignment;
using Bio.Algorithms.Assembly;
using Bio.IO;
using Bio.Registration;
using Bio.SimilarityMatrices;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Bio.TestAutomation.Registration
{
    /// <summary>
    ///     Registration BVT Test case implementation.
    /// </summary>
    [TestClass]
    public class RegistrationBvtTestCases
    {
        #region Constructor

        /// <summary>
        ///     Static constructor to open log and make other settings needed for test
        /// </summary>
        static RegistrationBvtTestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region Register Addins BVT Test cases

        /// <summary>
        /// Validates Registered Alphabets.
        /// Input : Register One Alphabet.
        /// Validation : Validate the Alphabet Registered.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestAddinsRegisterAlphabet()
        {
            Assert.IsNotNull(Alphabets.All.FirstOrDefault(a => a is TestAutomationAlphabet));
        }

        /// <summary>
        ///     Validates Registered Assemblers.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestAddinsRegisterAssembler()
        {
            var assemblers = RegisteredAddIn.GetInstancesFromAssembly<IDeNovoAssembler>(
                Assembly.GetExecutingAssembly().CodeBase.Substring(8));

            Assert.AreEqual(1, assemblers.Count);
            Assert.AreEqual(typeof(TestAutomationSequenceAssembler), assemblers[0].GetType());
        }

        /// <summary>
        ///     Validates Registered Formatter.
        ///     Input : Register One Formatter.
        ///     Validation : Validate the Formatter Registered.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestAddinsRegisterFormatter()
        {
            Assert.IsNotNull(SequenceFormatters.All.FirstOrDefault(sf => sf is TestAutomationSequenceFormatter));
        }

        /// <summary>
        ///     Validates Registered Parsers.
        ///     Input : Register One Parser.
        ///     Validation : Validate the Parser Registered.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestAddinsRegisterParser()
        {
            Assert.IsNotNull(SequenceParsers.All.FirstOrDefault(sp => sp is TestAutomationSequenceParser));
        }

        /// <summary>
        ///     Validates Registered Instances.
        ///     Input : Register Two Aligners.
        ///     Validation : Validate the Instances Registered.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestAddinsSequenceAligners()
        {
            List<ISequenceAligner> expectedAligners = new List<ISequenceAligner>
            {
                new TestAutomationSequenceAligner(),
                new TestAutomationPairwiseSequenceAligner()
            };

            // Gets the registered Instances for the path passed
            IList<ISequenceAligner> actualAligners = RegisteredAddIn.GetInstancesFromAssembly<ISequenceAligner>(
                typeof(TestAutomationSequenceAligner).Assembly.FullName);
            CompareAlignerElements(expectedAligners, actualAligners);
        }

        #endregion Register Addins BVT Test cases

        #region Registration Components

        /// <summary>
        ///     Creating new aligner class which is extended from ISequenceAligner.
        ///     Also registered for auto-plugin by the registration attribute as true
        /// </summary>
        [Registrable(true)]
        public sealed class TestAutomationSequenceAligner : ISequenceAligner
        {
            #region ISequenceAligner members

            string ISequenceAligner.Name
            {
                get { return "TestAutomation SequenceAligner"; }
            }

            string ISequenceAligner.Description
            {
                get { return "TestAutomation SequenceAligner Description"; }
            }

            IConsensusResolver ISequenceAligner.ConsensusResolver
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            SimilarityMatrix ISequenceAligner.SimilarityMatrix
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            int ISequenceAligner.GapOpenCost
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            int ISequenceAligner.GapExtensionCost
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            IList<ISequenceAlignment> ISequenceAligner.AlignSimple(IEnumerable<ISequence> inputSequences)
            {
                throw new NotImplementedException();
            }

            IList<ISequenceAlignment> ISequenceAligner.Align(IEnumerable<ISequence> inputSequences)
            {
                throw new NotImplementedException();
            }

            #endregion ISequenceAligner members
        }

        /// <summary>
        ///     Creating new pairwise aligner class which is extended from IPairwiseSequenceAligner.
        ///     Also registered for auto-plugin by the registration attribute as true
        /// </summary>
        [Registrable(true)]
        public sealed class TestAutomationPairwiseSequenceAligner : IPairwiseSequenceAligner
        {
            #region IPairwiseSequenceAligner Members

            /// <summary>
            ///     Similarity Matrix
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
            public SimilarityMatrix SimilarityMatrix
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            /// <summary>
            ///     Gap open Cost
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
            public int GapOpenCost
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            /// <summary>
            ///     Gap extension cost
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
            public int GapExtensionCost
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            /// <summary>
            ///     Align Simple
            /// </summary>
            /// <param name="sequence1">Sequence 1</param>
            /// <param name="sequence2">Sequence 2</param>
            /// <returns>Not Implemented exception</returns>
            public IList<IPairwiseSequenceAlignment> AlignSimple(ISequence sequence1, ISequence sequence2)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            ///     Align
            /// </summary>
            /// <param name="sequence1">Sequence 1</param>
            /// <param name="sequence2">Sequence 2</param>
            /// <returns>Not Implemented exception</returns>
            public IList<IPairwiseSequenceAlignment> Align(ISequence sequence1, ISequence sequence2)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region ISequenceAligner Members

            /// <summary>
            ///     Name of the aligner
            /// </summary>
            public string Name
            {
                get { return "TestAutomation Pairwise SequenceAligner"; }
            }

            /// <summary>
            ///     Name of the description
            /// </summary>
            public string Description
            {
                get { return "TestAutomation Pairwise SequenceAligner Description"; }
            }

            /// <summary>
            ///     Consensus Resolver
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
            IConsensusResolver ISequenceAligner.ConsensusResolver
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            /// <summary>
            ///     Align Simple
            /// </summary>
            /// <param name="inputSequences">Input Sequences</param>
            /// <returns>Not Implemented exception</returns>
            IList<ISequenceAlignment> ISequenceAligner.AlignSimple(IEnumerable<ISequence> inputSequences)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            ///     Align
            /// </summary>
            /// <param name="inputSequences">Input Sequences</param>
            /// <returns>Not Implemented exception</returns>
            IList<ISequenceAlignment> ISequenceAligner.Align(IEnumerable<ISequence> inputSequences)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        /// <summary>
        ///     Creating new assembler class which is extended from IDeNovoAssembler.
        ///     Also registered for auto-plugin by the registration attribute as true
        /// </summary>
        [Registrable(true)]
        public sealed class TestAutomationSequenceAssembler : IDeNovoAssembler
        {
            #region IDeNovoAssembler Members

            string IDeNovoAssembler.Name
            {
                get { return "TestAutomation SequenceAssembler"; }
            }

            string IDeNovoAssembler.Description
            {
                get { return "TestAutomation SequenceAssembler Description"; }
            }

            IDeNovoAssembly IDeNovoAssembler.Assemble(IEnumerable<ISequence> inputSequences)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        /// <summary>
        ///     Creating new alphabet class which is extended from IAlphabet.
        ///     Also registered for auto-plugin by the registration attribute as true
        /// </summary>
        [Registrable(true)]
        public sealed class TestAutomationAlphabet : IAlphabet
        {
            #region IAlphabet Members

            string IAlphabet.Name
            {
                get { return "TestAutomation Alphabet"; }
            }

            bool IAlphabet.HasGaps
            {
                get { throw new NotImplementedException(); }
            }

            bool IAlphabet.HasAmbiguity
            {
                get { throw new NotImplementedException(); }
            }

            bool IAlphabet.HasTerminations
            {
                get { throw new NotImplementedException(); }
            }

            /// <summary>
            ///     Gets a value indicating whether this alphabet supports complement or not.
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
            public bool IsComplementSupported
            {
                get { throw new NotImplementedException(); }
            }

            /// <summary>
            ///     Gets the count of symbols present in this alphabet.
            ///     This incldues basic symbols, gaps, terminations and ambiguous symbols present in this alphabet.
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
            public int Count
            {
                get { throw new NotImplementedException(); }
            }

            /// <summary>
            ///     Provides array like access to the symbols in this alphabet.
            /// </summary>
            /// <param name="index">Index of symbol present in this alphabet.</param>
            /// <returns>A byte which represents the symbol.</returns>
            [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
            public byte this[int index]
            {
                get { throw new NotImplementedException(); }
            }

            #endregion

            /// <summary>
            ///     Gets the ambigious characters present in alphabet.
            /// </summary>
            public HashSet<byte> GetAmbiguousSymbols()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            ///     Gets the consensus symbol present in alphabet.
            /// </summary>
            public byte GetConsensusSymbol(HashSet<byte> symbols)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            ///     Maps A to A  and a to A
            ///     that is key will contain unique values.
            ///     This will be used in the IsValidSymbol method to address Scenarios like a == A, G == g etc.
            /// </summary>
            public byte[] GetSymbolValueMap()
            {
                throw new NotImplementedException();
            }

            public bool TryGetComplementSymbol(byte symbol, out byte complementSymbol)
            {
                throw new NotImplementedException();
            }

            public bool TryGetDefaultGapSymbol(out byte defaultGapSymbol)
            {
                throw new NotImplementedException();
            }

            public bool TryGetDefaultTerminationSymbol(out byte defaultTerminationSymbol)
            {
                throw new NotImplementedException();
            }

            public bool TryGetGapSymbols(out HashSet<byte> gapSymbols)
            {
                throw new NotImplementedException();
            }

            public bool TryGetTerminationSymbols(out HashSet<byte> terminationSymbols)
            {
                throw new NotImplementedException();
            }

            public HashSet<byte> GetValidSymbols()
            {
                throw new NotImplementedException();
            }

            public bool TryGetAmbiguousSymbol(HashSet<byte> symbols, out byte ambiguousSymbol)
            {
                throw new NotImplementedException();
            }

            public bool TryGetBasicSymbols(byte ambiguousSymbol, out HashSet<byte> basicSymbols)
            {
                throw new NotImplementedException();
            }

            public bool CompareSymbols(byte x, byte y)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator<byte> IEnumerable<byte>.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public bool ValidateSequence(byte[] symbols, long offset, long length)
            {
                throw new NotImplementedException();
            }

            public bool CheckIsAmbiguous(byte item)
            {
                throw new NotImplementedException();
            }

            public bool CheckIsGap(byte item)
            {
                throw new NotImplementedException();
            }

            public bool TryGetComplementSymbol(byte[] symbols, out byte[] complementSymbols)
            {
                throw new NotImplementedException();
            }

            public string GetFriendlyName(byte item)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     Creating new formatter class which is extended from ISequenceFormatter.
        ///     Also registered for auto-plugin by the registration attribute as true
        /// </summary>
        [Registrable(true)]
        public sealed class TestAutomationSequenceFormatter : ISequenceFormatter
        {
            #region ISequenceFormatter Members

            /// <summary>
            /// </summary>
            public string Name
            {
                get { return "TestAutomation SequenceFormatter"; }
            }

            /// <summary>
            /// </summary>
            public string Description
            {
                get { return "TestAutomation SequenceFormatter Description"; }
            }

            /// <summary>
            /// </summary>
            [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
            public string SupportedFileTypes
            {
                get { throw new NotImplementedException(); }
            }

            #endregion

            public void Open(string filename)
            {
                throw new NotImplementedException();
            }

            public void Open(System.IO.StreamWriter outStream)
            {
                throw new NotImplementedException();
            }

            public void Write(ISequence sequence)
            {
                throw new NotImplementedException();
            }

            public void Close()
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     Creating new parser class which is extended from ISequenceParser.
        ///     Also registered for auto-plugin by the registration attribute as true
        /// </summary>
        [Registrable(true)]
        public sealed class TestAutomationSequenceParser : ISequenceParser
        {
            #region ISequenceParser Members

            string IParser.Name
            {
                get { return "TestAutomation SequenceParser"; }
            }

            string IParser.Description
            {
                get { return "TestAutomation SequenceParser Description"; }
            }

            string IParser.SupportedFileTypes
            {
                get { return ".unknown"; }
            }

            #endregion

            public void Open(string dataSource)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<ISequence> Parse()
            {
                throw new NotImplementedException();
            }

            public IEnumerable<ISequence> Parse(System.IO.StreamReader reader)
            {
                throw new NotImplementedException();
            }

            public void Close()
            {
                throw new NotImplementedException();
            }

            public IAlphabet Alphabet
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        #endregion Registration Components

        #region Supported Methods

        /// <summary>
        ///     General Register Aligner test case validation goes here.
        /// </summary>
        /// <param name="expectedAligners"></param>
        /// <param name="actualAligners"></param>
        private static void CompareAlignerElements(IList<ISequenceAligner> expectedAligners, IList<ISequenceAligner> actualAligners)
        {
            string[] expected = expectedAligners.Select(a => a.Name + ":" + a.Description).ToArray();
            string[] actual = actualAligners.Select(a => a.Name + ":" + a.Description).ToArray();

            Console.WriteLine("Expected: {0}", string.Join(",", expectedAligners.Select(a => a.Name)));
            Console.WriteLine("Actual: {0}", string.Join(",", actualAligners.Select(a => a.Name)));

            Assert.AreEqual(expected.Length, actual.Length, "Different number of aligners found.");
            CollectionAssert.AreEquivalent(expected, actual, "Aligners did not match");
        }

        #endregion Supported Methods
    }
}