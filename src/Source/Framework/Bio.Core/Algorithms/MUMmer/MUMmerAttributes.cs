using Bio.Algorithms.Alignment;

namespace Bio.Algorithms.MUMmer
{
    /// <summary>
    /// This class extends PairwiseAlignmentAttributes and adds MUMmer specific attributes
    /// required to run the MUMmer algorithm.
    /// </summary>
    public class MUMmerAttributes : PairwiseAlignmentAttributes
    {
        /// <summary>
        /// Describes the Minimal length Maximal Unique Match parameter
        /// </summary>
        public const string LengthOfMUM = "LENGTHOFMUM";

        /// <summary>
        /// Initializes a new instance of the MUMmerAttributes class.
        /// </summary>
        public MUMmerAttributes()
        {
            AlignmentInfo alignmentAttribute = new AlignmentInfo(
                    Properties.Resource.LENGTH_OF_MUM_NAME,
                    Properties.Resource.LENGTH_OF_MUM_DESCRIPTION,
                    true,
                    "20",
                    AlignmentInfo.IntType,
                    null);
            Attributes.Add(LengthOfMUM, alignmentAttribute);
        }
    }
}
