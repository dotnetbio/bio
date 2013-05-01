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
using System.IO;
using System.Linq;
using System.Reflection;
using Bio.Algorithms.Alignment;
using Bio.Algorithms.Assembly;
using Bio.IO;
using Bio.Registration;
using Bio.SimilarityMatrices;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.TestAutomation.Registration
{
    /// <summary>
    ///     Registration BVT Test case implementation.
    /// </summary>
    [TestClass]
    public class RegistrationBvtTestCases
    {
        #region Constants

        private const string AddInsFolder = "\\Add-ins";
        private const string BioTestAutomationDll = "\\Bio.TestAutomation.dll";

        #endregion Constants

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
        ///     Validates Registered Aligners.
        ///     Input : Register Two Aligners.
        ///     Validation : Validate the Aligners Registered.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void RegisterAddinsRegisterAligner()
        {
            CreateAddinsFolder();
            IList<ISequenceAligner> finalValue = new List<ISequenceAligner>();
            finalValue.Add(new TestAutomationSequenceAligner());
            finalValue.Add(new TestAutomationPairwiseSequenceAligner());

            // Gets the registered Aligners
            IList<ISequenceAligner> registeredAligners = GetClasses<ISequenceAligner>(true);
            RegisterAlignGeneralTestCases(registeredAligners, finalValue);
            DeleteAddinsFolder();
        }

        /// <summary>
        ///     Validates Registered Assemblies.
        ///     Input : Register One Assembly.
        ///     Validation : Validate the Assembly Registered.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void RegisterAddinsRegisterAssembly()
        {
            CreateAddinsFolder();
            IList<IDeNovoAssembler> finalValue = new List<IDeNovoAssembler>();
            finalValue.Add(new TestAutomationSequenceAssembler());

            // Gets the registered Assemblers
            IList<IDeNovoAssembler> registeredAssemblers = GetClasses<IDeNovoAssembler>(true);
            if (null != registeredAssemblers && registeredAssemblers.Count > 0)
            {
                foreach (IDeNovoAssembler assembler in finalValue)
                {
                    string name = string.Empty;
                    string description = string.Empty;

                    registeredAssemblers.FirstOrDefault(IA => string.Compare(name = IA.Name,
                                                                             assembler.Name,
                                                                             StringComparison.OrdinalIgnoreCase) == 0);
                    registeredAssemblers.FirstOrDefault(IA => string.Compare(description = IA.Description,
                                                                             assembler.Description,
                                                                             StringComparison.OrdinalIgnoreCase) == 0);

                    // Validates the Name and Description
                    Assert.AreEqual(assembler.Name, name);
                    Assert.AreEqual(assembler.Description, description);
                    ApplicationLog.WriteLine(
                        string.Format(null, @"Successfully validated the Registered components for Assembly '{0}'.",
                                      name));
                }
            }
            else
            {
                ApplicationLog.WriteLine("No Components to Register.");
                Assert.Fail();
            }
            DeleteAddinsFolder();
        }

        /// <summary>
        /// Validates Registered Alphabets.
        /// Input : Register One Alphabet.
        /// Validation : Validate the Alphabet Registered.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        [Ignore]
        public void RegisterAddinsRegisterAlphabet()
        {
            CreateAddinsFolder();
            IList<IAlphabet> finalValue = new List<IAlphabet>();
            finalValue.Add(new TestAutomationAlphabet());

            // Gets the registered Alphabets
            IList<IAlphabet> registeredAlphabets = Alphabets.All.ToList();
            if (registeredAlphabets.Count > 0)
            {
                foreach (IAlphabet alphabet in finalValue)
                {
                    string name = string.Empty;

                    registeredAlphabets.FirstOrDefault(ia => string.Compare(name = ia.Name, alphabet.Name, StringComparison.OrdinalIgnoreCase) == 0);

                    // Validates the Name
                    Assert.AreEqual(alphabet.Name, name);
                    ApplicationLog.WriteLine(
                        string.Format((IFormatProvider)null, @"Successfully validated the Registered components for Alphabet '{0}'.",
                        name));
                }
            }
            else
            {
                ApplicationLog.WriteLine("No Components to Register.");
                Assert.Fail();
            }
            DeleteAddinsFolder();
        }

        /// <summary>
        ///     Validates Registered Formatter.
        ///     Input : Register One Formatter.
        ///     Validation : Validate the Formatter Registered.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void RegisterAddinsRegisterFormatter()
        {
            CreateAddinsFolder();
            IList<IFormatter> finalValue = new List<IFormatter>();
            finalValue.Add(new TestAutomationSequenceFormatter());

            // Gets the registered formatters
            IList<IFormatter> registeredFormatters = GetClasses<IFormatter>(true);
            if (null != registeredFormatters && registeredFormatters.Count > 0)
            {
                foreach (IFormatter formatter in finalValue)
                {
                    string name = string.Empty;
                    string description = string.Empty;

                    registeredFormatters.FirstOrDefault(IA => string.Compare(name = IA.Name,
                                                                             formatter.Name,
                                                                             StringComparison.OrdinalIgnoreCase) == 0);
                    registeredFormatters.FirstOrDefault(IA => string.Compare(description = IA.Description,
                                                                             formatter.Description,
                                                                             StringComparison.OrdinalIgnoreCase) == 0);

                    // Validates the Name and Description
                    Assert.AreEqual(formatter.Name, name);
                    Assert.AreEqual(formatter.Description, description);
                    ApplicationLog.WriteLine(
                        string.Format(null, @"Successfully validated the Registered components for Formatter '{0}'.",
                                      name));
                }
            }
            else
            {
                ApplicationLog.WriteLine("No Components to Register.");
                Assert.Fail();
            }
            DeleteAddinsFolder();
        }

        /// <summary>
        ///     Validates Registered Parsers.
        ///     Input : Register One Parser.
        ///     Validation : Validate the Parser Registered.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void RegisterAddinsRegisterParser()
        {
            CreateAddinsFolder();
            IList<IParser> finalValue = new List<IParser>();
            finalValue.Add(new TestAutomationSequenceParser());

            IList<IParser> registeredParsers = GetClasses<IParser>(true);
            if (null != registeredParsers && registeredParsers.Count > 0)
            {
                foreach (IParser parser in finalValue)
                {
                    string name = string.Empty;
                    string description = string.Empty;

                    registeredParsers.FirstOrDefault(IA => string.Compare(name = IA.Name,
                                                                          parser.Name,
                                                                          StringComparison.OrdinalIgnoreCase) == 0);
                    registeredParsers.FirstOrDefault(IA => string.Compare(description = IA.Description,
                                                                          parser.Description,
                                                                          StringComparison.OrdinalIgnoreCase) == 0);

                    // Validates the Name and Description
                    Assert.AreEqual(parser.Name, name);
                    Assert.AreEqual(parser.Description, description);
                    ApplicationLog.WriteLine(
                        string.Format(null, @"Successfully validated the Registered components for Parser '{0}'.",
                                      name));
                }
            }
            else
            {
                ApplicationLog.WriteLine("No Components to Register.");
                Assert.Fail();
            }
            DeleteAddinsFolder();
        }

        /// <summary>
        ///     Validates Registered Instances.
        ///     Input : Register Two Aligners.
        ///     Validation : Validate the Instances Registered.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void RegisterAddinsGetInstances()
        {
            CreateAddinsFolder();
            IList<ISequenceAligner> finalValue = new List<ISequenceAligner>();
            finalValue.Add(new TestAutomationSequenceAligner());
            finalValue.Add(new TestAutomationPairwiseSequenceAligner());

            // Gets the registered Instances for the path passed
            string assemblyPath = string.Concat(RegisteredAddIn.AddinFolderPath,
                                                BioTestAutomationDll);
            IList<ISequenceAligner> registeredAligners =
                RegisteredAddIn.GetInstancesFromAssembly<ISequenceAligner>(assemblyPath);

            RegisterAlignGeneralTestCases(registeredAligners, finalValue);
            DeleteAddinsFolder();
        }

        /// <summary>
        ///     Validates Registered Instances.
        ///     Input : Register Two Aligners.
        ///     Validation : Validate the Instances Registered.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void RegisterAddinsGetInstancesFilter()
        {
            CreateAddinsFolder();
            IList<ISequenceAligner> finalValue = new List<ISequenceAligner>();
            finalValue.Add(new TestAutomationSequenceAligner());
            finalValue.Add(new TestAutomationPairwiseSequenceAligner());

            // Gets the registered Instances for the path passed and the filter
            IList<ISequenceAligner> registeredAligners =
                RegisteredAddIn.GetInstancesFromAssemblyPath<ISequenceAligner>(
                    RegisteredAddIn.AddinFolderPath, "*.dll");

            RegisterAlignGeneralTestCases(registeredAligners, finalValue);
            DeleteAddinsFolder();
        }

        /// <summary>
        ///     Validates Registered Instances.
        ///     Input : Register Two Aligners.
        ///     Validation : Validate the Instances Registered.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void RegisterAddinsGetInstancesExecutingAssembly()
        {
            CreateAddinsFolder();
            IList<ISequenceAligner> finalValue = new List<ISequenceAligner>();
            finalValue.Add(new TestAutomationSequenceAligner());
            finalValue.Add(new TestAutomationPairwiseSequenceAligner());

            // Gets the registered Instances for the path passed
            IList<ISequenceAligner> registeredAligners =
                RegisteredAddIn.GetInstancesFromExecutingAssembly<ISequenceAligner>();

            if (0 == registeredAligners.Count)
            {
                ApplicationLog.WriteLine("Referring from the Bio.dll, hence validation is not required.");
            }
            DeleteAddinsFolder();
        }

        /// <summary>
        ///     Validates the properties
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void RegisterAddinsAllProperties()
        {
            CreateAddinsFolder();
            // Validate the property values if exists
            Assert.IsTrue(!string.IsNullOrEmpty(RegisteredAddIn.AddinFolderPath));
            Assert.IsTrue(string.IsNullOrEmpty(RegisteredAddIn.CoreFolderPath));
            ApplicationLog.WriteLine(
                string.Format(null, "Successfully validate the property AddInFolderPath with value '{0}'",
                              RegisteredAddIn.AddinFolderPath));
            ApplicationLog.WriteLine("Successfully validate the property CoreFolderPath");
            DeleteAddinsFolder();
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
        public sealed class TestAutomationSequenceFormatter : IFormatter
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
        }

        /// <summary>
        ///     Creating new parser class which is extended from ISequenceParser.
        ///     Also registered for auto-plugin by the registration attribute as true
        /// </summary>
        [Registrable(true)]
        public sealed class TestAutomationSequenceParser : IParser
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
                get { throw new NotImplementedException(); }
            }

            #endregion
        }

        #endregion Registration Components

        #region Supported Methods

        /// <summary>
        ///     General Register Aligner test case validation goes here.
        /// </summary>
        /// <param name="registeredAligners">Registered Aligners</param>
        /// <param name="finalValue">Expected Registered Aligners</param>
        private static void RegisterAlignGeneralTestCases(IList<ISequenceAligner> registeredAligners,
                                                          IList<ISequenceAligner> finalValue)
        {
            if (null != registeredAligners && registeredAligners.Count > 0)
            {
                foreach (ISequenceAligner aligner in finalValue)
                {
                    string name = string.Empty;
                    string description = string.Empty;

                    registeredAligners.FirstOrDefault(IA => string.Compare(name = IA.Name,
                                                                           aligner.Name,
                                                                           StringComparison.OrdinalIgnoreCase) == 0);
                    registeredAligners.FirstOrDefault(IA => string.Compare(
                        description = IA.Description, aligner.Description,
                        StringComparison.OrdinalIgnoreCase) == 0);

                    // Validates the Name and Description
                    Assert.AreEqual(aligner.Name, name);
                    Assert.AreEqual(aligner.Description, description);
                    ApplicationLog.WriteLine(
                        string.Format(null, @"Successfully validated the Registered Instances for '{0}'.",
                                      name));
                }
            }
            else
            {
                ApplicationLog.WriteLine("No Components to Register.");
                Assert.Fail();
            }
        }

        /// <summary>
        ///     Creates the Add-ins folder
        /// </summary>
        private static void CreateAddinsFolder()
        {
            // Gets the Add-ins folder name
            var uri = new Uri(Assembly.GetCallingAssembly().CodeBase);
            string addInsFolderPath = Uri.UnescapeDataString(string.Concat(
                Path.GetDirectoryName(uri.AbsolutePath),
                AddInsFolder));

            if (!Directory.Exists(addInsFolderPath))
                // Creates the Add-ins folder
                Directory.CreateDirectory(addInsFolderPath);

            // If TestAutomation file already exists, don't replace
            if (!File.Exists(string.Concat(addInsFolderPath, BioTestAutomationDll)))
            {
                // Copies the Bio.TestAutomation.dll to Add-ins folder
                File.Copy(Uri.UnescapeDataString(uri.AbsolutePath),
                          string.Concat(addInsFolderPath, BioTestAutomationDll), true);
            }
        }

        /// <summary>
        ///     Deletes the Add-ins folder if exists
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static void DeleteAddinsFolder()
        {
            var uri = new Uri(Assembly.GetCallingAssembly().CodeBase);
            string addInsFolderPath = Uri.UnescapeDataString(string.Concat(
                Path.GetDirectoryName(uri.AbsolutePath),
                AddInsFolder));

            try
            {
                // If the Add-ins folder exists delete the same
                if (Directory.Exists(addInsFolderPath))
                    Directory.Delete(addInsFolderPath, true);
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Gets all registered specified classes in core folder and addins (optional) folders
        /// </summary>
        /// <param name="includeAddinFolder">include add-ins folder or not</param>
        /// <returns>List of registered classes</returns>
        private static IList<T> GetClasses<T>(bool includeAddinFolder)
            where T : class
        {
            IList<T> registeredAligners = new List<T>();

            if (includeAddinFolder)
            {
                IList<T> addInAligners;
                if (null != RegisteredAddIn.AddinFolderPath)
                {
                    addInAligners =
                        RegisteredAddIn.GetInstancesFromAssemblyPath<T>(RegisteredAddIn.AddinFolderPath,
                                                                        RegisteredAddIn.DLLFilter);
                    if (null != addInAligners && addInAligners.Count > 0)
                    {
                        foreach (T aligner in addInAligners)
                        {
                            if (aligner != null)
                            {
                                registeredAligners.Add(aligner);
                            }
                        }
                    }
                }
            }

            return registeredAligners;
        }

        #endregion Supported Methods
    }
}