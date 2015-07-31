namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    using System;
    using System.Collections.Generic;
    using Bio;
    using Bio.Algorithms.Alignment;
    using SM = Bio.SimilarityMatrices.SimilarityMatrix;

    /// <summary>
    /// This class implements IAlignmentAttributes interface and defines all the 
    /// parameters required to run PAMSAM algorithm.
    /// </summary>
    public class PamsamAlignmentAttributes : IAlignmentAttributes
    {
        /// <summary>
        /// List of Parameters required to run NUCmer
        /// </summary>
        public Dictionary<string, AlignmentInfo> Attributes
        {
            get { return attributes; }
        }

        /// <summary>
        /// List of Parameters required to run NUCmer
        /// </summary>
        private Dictionary<string, AlignmentInfo> attributes;

        /// <summary>
        /// KmerLength
        /// </summary>
        public const string KmerLength = "KmerLength";

        /// <summary>
        /// Distance Function Type
        /// </summary>
        public const string DistanceFunctionType = "DistanceFunctionType";

        /// <summary>
        /// Update Distance Methods Type
        /// </summary>
        public const string UpdateDistanceMethodsType = "UpdateDistanceMethodsType";

        /// <summary>
        /// Profile Aligner Name
        /// </summary>
        public const string ProfileAlignerName = "ProfileAlignerName";

        /// <summary>
        /// Profile Score Function Name
        /// </summary>
        public const string ProfileScoreFunctionName = "ProfileScoreFunctionName";

        /// <summary>
        /// Similarity Matrix
        /// </summary>
        public const string SimilarityMatrix = "Similarity Matrix";

        /// <summary>
        /// Gap Open Penalty
        /// </summary>
        public const string GapOpenPenalty = "GapOpenPenalty";

        /// <summary>
        /// Gap Extend Penalty
        /// </summary>
        public const string GapExtendPenalty = "GapExtendPenalty";

        /// <summary>
        /// Number Of Partitions
        /// </summary>
        public const string NumberOfPartitions = "NumberOfPartitions";

        /// <summary>
        /// Degree Of Parallelism
        /// </summary>
        public const string DegreeOfParallelism = "DegreeOfParallelism";

        /// <summary>
        /// Initializes a new instance of the PairwiseAlignmentAttributes class.
        /// </summary>
        public PamsamAlignmentAttributes()
        {
            AlignmentInfo alignmentAttribute;
            attributes = new Dictionary<string, AlignmentInfo>();

            alignmentAttribute = new AlignmentInfo(
                KmerLength,
                "Kmer Length",
                true,
                "3",
                AlignmentInfo.IntType,
                null);
            attributes.Add(KmerLength, alignmentAttribute);

            StringListValidator listValidator = new StringListValidator(
                Enum.GetNames(typeof(DistanceFunctionTypes))
                );
            alignmentAttribute = new AlignmentInfo(
                DistanceFunctionType,
                "Distance Function Type",
                true,
                DistanceFunctionTypes.EuclideanDistance.ToString(),
                AlignmentInfo.StringListType,
                listValidator);
            attributes.Add(DistanceFunctionType, alignmentAttribute);

            listValidator = new StringListValidator(
                Enum.GetNames(typeof(UpdateDistanceMethodsTypes))
                );
            alignmentAttribute = new AlignmentInfo(
                UpdateDistanceMethodsType,
                "Update Distance Methods Type",
                true,
                UpdateDistanceMethodsTypes.Average.ToString(),
                AlignmentInfo.StringListType,
                listValidator);
            attributes.Add(UpdateDistanceMethodsType, alignmentAttribute);

            listValidator = new StringListValidator(
                Enum.GetNames(typeof(ProfileAlignerNames))
                );
            alignmentAttribute = new AlignmentInfo(
                ProfileAlignerName,
                "Profile Aligner Name",
                true,
                ProfileAlignerNames.NeedlemanWunschProfileAligner.ToString(),
                AlignmentInfo.StringListType,
                listValidator);
            attributes.Add(ProfileAlignerName, alignmentAttribute);

            listValidator = new StringListValidator(
                Enum.GetNames(typeof(ProfileScoreFunctionNames))
                );
            alignmentAttribute = new AlignmentInfo(
                ProfileScoreFunctionName,
                "Profile Score Function Name",
                true,
                ProfileScoreFunctionNames.WeightedInnerProduct.ToString(),
                AlignmentInfo.StringListType,
                listValidator);
            attributes.Add(ProfileScoreFunctionName, alignmentAttribute);

            listValidator = new StringListValidator(
                SM.StandardSimilarityMatrix.AmbiguousDna.ToString(),
                SM.StandardSimilarityMatrix.AmbiguousRna.ToString(),
                SM.StandardSimilarityMatrix.Blosum45.ToString(),
                SM.StandardSimilarityMatrix.Blosum50.ToString(),
                SM.StandardSimilarityMatrix.Blosum62.ToString(),
                SM.StandardSimilarityMatrix.Blosum80.ToString(),
                SM.StandardSimilarityMatrix.Blosum90.ToString(),
                SM.StandardSimilarityMatrix.Pam250.ToString(),
                SM.StandardSimilarityMatrix.Pam30.ToString(),
                SM.StandardSimilarityMatrix.Pam70.ToString()                
                );

            alignmentAttribute = new AlignmentInfo(
                SimilarityMatrix,
                "Describes matrix that determines the score for any possible pair of symbols",
                true,
                SM.StandardSimilarityMatrix.AmbiguousDna.ToString(),
                AlignmentInfo.StringListType,
                listValidator);
            attributes.Add(SimilarityMatrix, alignmentAttribute);

            alignmentAttribute = new AlignmentInfo(
                GapOpenPenalty,
                "Gap Open Penalty",
                true,
                "-4",
                AlignmentInfo.IntType,
                null);
            attributes.Add(GapOpenPenalty, alignmentAttribute);

            alignmentAttribute = new AlignmentInfo(
                GapExtendPenalty,
                "Gap Extension Penalty",
                true,
                "-1",
                AlignmentInfo.IntType,
                null);
            attributes.Add(GapExtendPenalty, alignmentAttribute);

            alignmentAttribute = new AlignmentInfo(
                NumberOfPartitions,
                "Number Of Partitions",
                true,
                (Environment.ProcessorCount * 2).ToString(),
                AlignmentInfo.IntType,
                null);
            attributes.Add(NumberOfPartitions, alignmentAttribute);

            alignmentAttribute = new AlignmentInfo(
                DegreeOfParallelism,
                "Degree Of Parallelism",
                true,
                Environment.ProcessorCount.ToString(),
                AlignmentInfo.IntType,
                null);
            attributes.Add(DegreeOfParallelism, alignmentAttribute);
        }
    }
}
