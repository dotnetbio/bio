using System;
using System.Collections.Generic;
using System.Linq;
using Bio.Algorithms.MUMmer;
using Bio.Registration;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// SequenceAligners class is an abstraction class which provides instances
    /// and lists of all Aligners currently supported by Bio. 
    /// </summary>
    public static class SequenceAligners
    {
        /// <summary>
        /// Initializes static members of the SequenceAligners class.
        /// Static constructor
        /// </summary>
        static SequenceAligners()
        {
            NUCmer = new NucmerPairwiseAligner();
            MUMmer = new MUMmerAligner();
            NeedlemanWunsch = new NeedlemanWunschAligner();
            SmithWaterman = new SmithWatermanAligner();

            var knownAligners = new List<ISequenceAligner> { SmithWaterman, NeedlemanWunsch, MUMmer, NUCmer };

            // Get the registered aligners
            IEnumerable<ISequenceAligner> registeredAligners = GetAligners();
            if (null != registeredAligners)
            {
                knownAligners.AddRange(registeredAligners
                    .Where(aligner => aligner != null 
                        && knownAligners.All(sa => string.Compare(sa.Name, aligner.Name, StringComparison.OrdinalIgnoreCase) != 0)));
            }

            All = knownAligners;
        }

        /// <summary>
        /// Gets an instance of SmithWatermanAligner class which implements
        /// the SmithWaterman algorithm for partial alignment
        /// </summary>
        public static SmithWatermanAligner SmithWaterman { get; private set; }

        /// <summary>
        /// Gets an instance of NeedlemanWunschAligner class which implements
        /// the NeedlemanWunsch algorithm for global alignment.
        /// </summary>
        public static NeedlemanWunschAligner NeedlemanWunsch { get; private set; }

        /// <summary>
        /// Gets an instance of MUMmer3 class which implements
        /// the MUMmer algorithm for partial alignment
        /// </summary>
        public static MUMmerAligner MUMmer { get; private set; }

        /// <summary>
        /// Gets an instance of NUCmer3 class which implements
        /// the NUCmer algorithm for alignment
        /// </summary>
        public static NucmerPairwiseAligner NUCmer { get; private set; }

        /// <summary>
        /// Gets the list of all aligners which is supported by the framework.
        /// </summary>
        public static List<ISequenceAligner> All { get; private set; }

        /// <summary>
        /// Gets all registered aligners in core folder and addins (optional) folders
        /// </summary>
        /// <returns>List of registered aligners</returns>
        private static IEnumerable<ISequenceAligner> GetAligners()
        {
            var implementations = BioRegistrationService.LocateRegisteredParts<ISequenceAligner>();
            var registeredAligners = new List<ISequenceAligner>();

            foreach (var impl in implementations)
            {
                try
                {
                    ISequenceAligner aligner = Activator.CreateInstance(impl) as ISequenceAligner;
                    if (aligner != null)
                        registeredAligners.Add(aligner);
                }
                catch
                {
                    // Cannot create - no default ctor?
                }
            }

            return registeredAligners;

        }
    }
}
