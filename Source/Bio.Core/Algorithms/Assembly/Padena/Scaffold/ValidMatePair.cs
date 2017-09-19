using System.Collections.Generic;

namespace Bio.Algorithms.Assembly.Padena.Scaffold
{
    /// <summary>
    /// Class stores information about mate pairs and 
    /// their start positions with respect to contig.
    /// </summary>
    public class ValidMatePair
    {
        #region Fields

        /// <summary>
        /// Stores information about start position of forward read in contig.
        /// </summary>
        private IList<long> forwardReadStartPosition = new List<long>();

        /// <summary>
        /// Stores information about start position of reverse read in contig.
        /// </summary>
        private IList<long> reverseReadStartPosition = new List<long>();

        /// <summary>
        ///  Stores information about start position of reverse read in 
        ///  reverse complementary sequence of contig.
        ///  The distance estimated for both cases will be used in trace path, 
        ///  based on edge orientation contig overlap graph.
        /// </summary>
        private IList<long> reverseReadReverseComplementStartPosition = new List<long>();

        /// <summary>
        /// Stores distance between contigs using forward and 
        /// reverse complementary sequence of reverse contig.
        /// </summary>
        private IList<float> distanceBetweenContigs = new List<float>();

        /// <summary>
        /// Stores standard deviation between contigs using forward and 
        /// reverse complementary sequence of reverse contig.
        /// </summary>
        private IList<float> standardDeviationBetweenContigs = new List<float>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets value of start position of forward read in contig.
        /// </summary>
        public IList<long> ForwardReadStartPosition
        {
            get { return this.forwardReadStartPosition; }
        }

        /// <summary>
        /// Gets value of start position of reverse read in contig.
        /// </summary>
        public IList<long> ReverseReadStartPosition
        {
            get { return this.reverseReadStartPosition; }
        }

        /// <summary>
        /// Gets value of start position of reverse read in 
        /// reverse complementary sequence of contig.
        /// </summary>
        public IList<long> ReverseReadReverseComplementStartPosition
        {
            get { return this.reverseReadReverseComplementStartPosition; }
        }

        /// <summary>
        /// Gets or sets Paired reads.
        /// </summary>
        public MatePair PairedRead { get; set; }

        /// <summary>
        /// Gets distance between contigs, calculated using paired read information.
        /// </summary>
        public IList<float> DistanceBetweenContigs
        {
            get { return this.distanceBetweenContigs; }
        }

        /// <summary>
        /// Gets standard Deviation between contigs, calculated using paired read information.
        /// </summary>
        public IList<float> StandardDeviation
        {
            get { return this.standardDeviationBetweenContigs; }
        }

        /// <summary>
        /// Gets or sets Weight of relationship between two contigs.
        /// </summary>
        public int Weight { get; set; }

        #endregion
    }
}
