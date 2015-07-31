using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bio.Variant
{
    /// <summary>
    /// A pileup represents a column of a multiple sequence alignment.
    /// Each element in the pileup has an observed base and associated quality score.
    /// </summary>
    public class  PileUp
    {
        /// <summary>
        /// The reference name for the pileup.
        /// </summary>
        public string RName;
        
        /// <summary>
        /// 1-based position on the reference genome
        /// </summary>
        public int Position;

        /// <summary>
        /// Is this actually inserted after the Position and
        /// not aligned with the reference?
        /// </summary>
        public bool IsInsertion { get { return InsertionOffSet > 0; } }

        public int InsertionOffSet;


        /// <summary>
        /// A list of bases and quality values at this position.
        /// </summary>
        public List<BaseAndQuality> Bases;

        /// <summary>
        /// Create a new pile up for the given contig/chromosome at the
        /// given position.
        /// </summary>
        /// <param name="rname"></param>
        /// <param name="position"></param>
        public PileUp(string rname, int position, int insertionOffset = 0)
        {
            this.RName = rname;
            this.Position = position;
            Bases = new List<BaseAndQuality>(DEFAULT_LIST_SIZE);
            this.InsertionOffSet = insertionOffset;
        }

        public PileUp(string rname, BaseAndQualityAndPosition bqp)
        {
            this.RName = rname;
            this.Position = bqp.Position;
            this.Bases = new List<BaseAndQuality>(DEFAULT_LIST_SIZE);
            this.Bases.Add(bqp.BaseWithQuality);
            this.InsertionOffSet = bqp.InsertPositionPassedRefPosition;
        }
        const int DEFAULT_LIST_SIZE = 100;
    }
}
