using System;
using System.Collections.Generic;
using System.Linq;

using Bio.Algorithms.MUMmer;
using Bio.Properties;
using Bio.Registration;

namespace Bio
{
    /// <summary>
    /// The currently supported and built-in alphabets for sequence items.
    /// </summary>
    public static class Alphabets
    {
        /// <summary>
        /// The DNA alphabet.
        /// </summary>
        public static readonly DnaAlphabet DNA = DnaAlphabet.Instance;

        /// <summary>
        /// The RNA alphabet.
        /// </summary>
        public static readonly RnaAlphabet RNA = RnaAlphabet.Instance;

        /// <summary>
        /// The protein alphabet consisting of amino acids.
        /// </summary>
        public static readonly ProteinAlphabet Protein = ProteinAlphabet.Instance;

        /// <summary>
        /// The Ambiguous DNA alphabet.
        /// </summary>
        public static readonly AmbiguousDnaAlphabet AmbiguousDNA = AmbiguousDnaAlphabet.Instance;

        /// <summary>
        /// The Ambiguous RNA alphabet.
        /// </summary>
        public static readonly AmbiguousRnaAlphabet AmbiguousRNA = AmbiguousRnaAlphabet.Instance;

        /// <summary>
        /// The Ambiguous protein alphabet consisting of amino acids.
        /// </summary>
        public static readonly AmbiguousProteinAlphabet AmbiguousProtein = AmbiguousProteinAlphabet.Instance;

        /// <summary>
        /// Mapping between an alphabet type and its corresponding base alphabet type.
        /// </summary>
        public static readonly Dictionary<IAlphabet, IAlphabet> AlphabetToBaseAlphabetMap;

        /// <summary>
        /// Mapping between an alphabet type and its corresponding ambiguous alphabet type.
        /// </summary>
        public static readonly Dictionary<IAlphabet, IAlphabet> AmbiguousAlphabetMap;

        /// <summary>
        /// List of all supported Alphabets.
        /// </summary>
        private static readonly List<IAlphabet> KnownAlphabets = new List<IAlphabet> 
        {
            DNA,
            AmbiguousDNA,
            RNA,
            AmbiguousRNA,
            Protein,
            AmbiguousProtein
        };

        /// <summary>
        /// List of alphabet instances according to their priority in auto detection
        /// Auto detection starts from top of the list.
        /// </summary>
        private static readonly List<IAlphabet> AlphabetPriorityList = new List<IAlphabet>
        {
            DnaAlphabet.Instance,
            AmbiguousDnaAlphabet.Instance,
            RnaAlphabet.Instance,
            AmbiguousRnaAlphabet.Instance,
            ProteinAlphabet.Instance,
            AmbiguousProteinAlphabet.Instance,
        };

        /// <summary>
        /// Initializes static members of the Alphabets class.
        /// </summary>
        static Alphabets()
        {
		    // get the registered alphabets.
            IEnumerable<IAlphabet> registeredAlphabets = GetAlphabets();
            if (null != registeredAlphabets)
            {
                foreach (IAlphabet alphabet in registeredAlphabets.Where(
                    alphabet => alphabet != null && KnownAlphabets.All(ra => String.Compare(ra.Name, alphabet.Name, StringComparison.OrdinalIgnoreCase) != 0)))
                {
                    KnownAlphabets.Add(alphabet);
                }
            }

            AmbiguousAlphabetMap = new Dictionary<IAlphabet, IAlphabet>();
            MapAlphabetToAmbiguousAlphabet(DnaAlphabet.Instance, AmbiguousDnaAlphabet.Instance);
            MapAlphabetToAmbiguousAlphabet(RnaAlphabet.Instance, AmbiguousRnaAlphabet.Instance);
            MapAlphabetToAmbiguousAlphabet(ProteinAlphabet.Instance, AmbiguousProteinAlphabet.Instance);
            MapAlphabetToAmbiguousAlphabet(AmbiguousDnaAlphabet.Instance, AmbiguousDnaAlphabet.Instance);
            MapAlphabetToAmbiguousAlphabet(AmbiguousRnaAlphabet.Instance, AmbiguousRnaAlphabet.Instance);

            AlphabetToBaseAlphabetMap = new Dictionary<IAlphabet, IAlphabet>();
            MapAlphabetToBaseAlphabet(AmbiguousDnaAlphabet.Instance, DnaAlphabet.Instance);
            MapAlphabetToBaseAlphabet(AmbiguousRnaAlphabet.Instance, RnaAlphabet.Instance);
            MapAlphabetToBaseAlphabet(AmbiguousProteinAlphabet.Instance, ProteinAlphabet.Instance);

		    MapAlphabetToBaseAlphabet(MummerDnaAlphabet.Instance, DnaAlphabet.Instance);
            MapAlphabetToBaseAlphabet(MummerRnaAlphabet.Instance, RnaAlphabet.Instance);
            MapAlphabetToBaseAlphabet(MummerProteinAlphabet.Instance, ProteinAlphabet.Instance);  
        }

        /// <summary>
        ///  Gets the list of all Alphabets which are supported by the framework.
        /// </summary>
        public static IReadOnlyList<IAlphabet> All
        {
            get { return KnownAlphabets; }
        }

        /// <summary>
        /// Gets the ambiguous alphabet
        /// </summary>
        /// <param name="currentAlphabet">Alphabet to validate</param>
        /// <returns></returns>
        public static IAlphabet GetAmbiguousAlphabet(IAlphabet currentAlphabet)
        {
            if (currentAlphabet == DnaAlphabet.Instance ||
                currentAlphabet == RnaAlphabet.Instance ||
                currentAlphabet == ProteinAlphabet.Instance)
            {
                return AmbiguousAlphabetMap[currentAlphabet];
            }

            return currentAlphabet;
        }

        /// <summary>
        /// Verifies if two given alphabets comes from the same base alphabet.
        /// </summary>
        /// <param name="alphabetA">First alphabet to compare.</param>
        /// <param name="alphabetB">Second alphabet to compare.</param>
        /// <returns>True if both alphabets comes from the same base class.</returns>
        public static bool CheckIsFromSameBase(IAlphabet alphabetA, IAlphabet alphabetB)
        {
            if (alphabetA == alphabetB)
                return true;

            IAlphabet innerAlphabetA = alphabetA, innerAlphabetB = alphabetB;

            if (AlphabetToBaseAlphabetMap.Keys.Contains(alphabetA))
                innerAlphabetA = AlphabetToBaseAlphabetMap[alphabetA];

            if (AlphabetToBaseAlphabetMap.Keys.Contains(alphabetB))
                innerAlphabetB = AlphabetToBaseAlphabetMap[alphabetB];

            return innerAlphabetA == innerAlphabetB;
        }

        /// <summary>
        /// This methods loops through supported alphabet types and tries to identify
        /// the best alphabet type for the given symbols.
        /// </summary>
        /// <param name="symbols">Symbols on which auto detection should be performed.</param>
        /// <param name="offset">Offset from which the auto detection should start.</param>
        /// <param name="length">Number of symbols to process from the offset position.</param>
        /// <param name="identifiedAlphabetType">In case the symbols passed are a sub set of a bigger sequence, 
        /// provide the already identified alphabet type of the sequence.</param>
        /// <returns>Returns the detected alphabet type or null if detection fails.</returns>
        public static IAlphabet AutoDetectAlphabet(byte[] symbols, long offset, long length, IAlphabet identifiedAlphabetType)
        {
            int currentPriorityIndex = 0;

            if (identifiedAlphabetType == null)
            {
                identifiedAlphabetType = AlphabetPriorityList[0];
            }

            while (identifiedAlphabetType != AlphabetPriorityList[currentPriorityIndex])
            {
                // Increment priority index and validate boundary condition
                if (++currentPriorityIndex == AlphabetPriorityList.Count)
                {
                    throw new ArgumentException(Resource.CouldNotRecognizeAlphabet, "identifiedAlphabetType");
                }
            }

            // Start validating against alphabet types according to their priority
            while (!AlphabetPriorityList[currentPriorityIndex].ValidateSequence(symbols, offset, length))
            {
                // Increment priority index and validate boundary condition
                if (++currentPriorityIndex == AlphabetPriorityList.Count)
                {
                    // Last ditch effort - look at all registered alphabets and see if any contain all the located symbols.
                    foreach (var alphabet in All)
                    {
                        // Make sure alphabet supports validation -- if not, ignore it.
                        try
                        {
                            if (alphabet.ValidateSequence(symbols, offset, length))
                                return alphabet;
                        }
                        catch (NotImplementedException)
                        {
                        }
                    }

                    // No alphabet found.
                    return null;
                }
            }

            return AlphabetPriorityList[currentPriorityIndex];
        }

        /// <summary>
        /// Maps the alphabet to its base alphabet.
        /// For example: AmbiguousDnaAlphabet to DnaAlphabet
        /// </summary>
        /// <param name="alphabet">Alphabet to map.</param>
        /// <param name="baseAlphabet">Base alphabet to map.</param>
        private static void MapAlphabetToBaseAlphabet(IAlphabet alphabet, IAlphabet baseAlphabet)
        {
            AlphabetToBaseAlphabetMap.Add(alphabet, baseAlphabet);
        }

        /// <summary>
        /// Maps the alphabet to its ambiguous alphabet.
        /// For example: DnaAlphabet to AmbiguousDnaAlphabet.
        /// </summary>
        /// <param name="alphabet">Alphabet to map.</param>
        /// <param name="ambiguousAlphabet">Ambiguous alphabet to map.</param>
        private static void MapAlphabetToAmbiguousAlphabet(IAlphabet alphabet, IAlphabet ambiguousAlphabet)
        {
            AmbiguousAlphabetMap.Add(alphabet, ambiguousAlphabet);
        }

        /// <summary>
        /// Gets all registered alphabets in core folder and addins (optional) folders.
        /// </summary>
        /// <returns>List of registered alphabets.</returns>
        private static IEnumerable<IAlphabet> GetAlphabets()
        {
            var implementations = BioRegistrationService.LocateRegisteredParts<IAlphabet>();
            var registeredAlphabets = new List<IAlphabet>();

            foreach (var impl in implementations)
            {
                try
                {
                    IAlphabet alpha = Activator.CreateInstance(impl) as IAlphabet;
                    if (alpha != null)
                        registeredAlphabets.Add(alpha);
                }
                catch
                {
                    // Cannot create - no default ctor?
                }
            }

            return registeredAlphabets;
        }
    }
}
