using System.Collections.Generic;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// Interface to resolve the start and end positions of a location.
    /// Classes which implements this interface should resolve any ambiguity in 
    /// the start and end positions of a location.
    /// Please refer LocationResolver for default implementation of this interface.
    /// </summary>
    public interface ILocationResolver
    {
        /// <summary>
        /// Returns start position of the specified location.
        /// </summary>
        /// <param name="location">Location instance.</param>
        int GetStart(ILocation location);

        /// <summary>
        /// Returns end position of the specified location.
        /// </summary>
        /// <param name="location">Location instance.</param>
        int GetEnd(ILocation location);

        /// <summary>
        /// Returns a sequence which contains bases from the specified sequences as specified by the location.
        /// If a feature location and the sequence in which the feature is present is 
        /// specified then the output sequence will contain the bases related to the feature.
        /// </summary>
        /// <param name="location">Location instance.</param>
        /// <param name="sequence">Sequence from which the sub sequence has to be returned.</param>
        ISequence GetSubSequence(ILocation location, ISequence sequence);

        /// <summary>
        /// Returns a sequence which contains bases from the specified sequences as specified by the location.
        /// If the location contains accession then the sequence from the referredSequences which matches the 
        /// accession of the location will be considered.
        /// 
        /// For example, 
        /// If a location is "join(100..200, J00089.1:10..50, J00090.2:30..40)"
        /// bases from 100 to 200 will be taken from the sequence parameter and referredSequences will
        /// be searched for the J00089.1 and J00090.2 accession if found then those sequences will be considered 
        /// for constructing the output sequence.
        /// If the referred sequence is not found in the referredSequences then an exception will occur.
        /// </summary>
        /// <param name="location">Location instance.</param>
        /// <param name="sequence">Sequence instance from which the sub sequence has to be returned.</param>
        /// <param name="referredSequences">A dictionary containing Accession numbers as keys and Sequences as values, this will be used when
        /// the location or sub-locations contains accession.</param>
        ISequence GetSubSequence(ILocation location, ISequence sequence, Dictionary<string, ISequence> referredSequences);

        /// <summary>
        /// Return true if the specified position is within the start position.
        /// For example,
        /// if the start data of a location is "23.40", this method will 
        /// return true for the position values ranging from 23 to 40.
        /// </summary>
        /// <param name="location">Location instance.</param>
        /// <param name="position">Position to be verified.</param>
        /// <returns>Returns true if the specified position is with in the start position else returns false.</returns>
        bool IsInStart(ILocation location, int position);

        /// <summary>
        /// Return true if the specified position is within the end position.
        /// For example,
        /// if the end data of a location is "23.40", this method will 
        /// return true for the position values ranging from 23 to 40.
        /// </summary>
        /// <param name="location">Location instance.</param>
        /// <param name="position">Position to be verified.</param>
        /// <returns>Returns true if the specified P\position is with in the end position else returns false.</returns>
        bool IsInEnd(ILocation location, int position);

        /// <summary>
        /// Returns true if the specified position is with in the start and end positions.
        /// </summary>
        /// <param name="location">Location instance.</param>
        /// <param name="position">Position to be verified.</param>
        /// <returns>Returns true if the specified position is with in the start and end positions else returns false.</returns>
        bool IsInRange(ILocation location, int position);

        /// <summary>
        /// Creates a new ILocationResolver that is a copy of the current ILocationResolver.
        /// </summary>
        /// <returns>A new ILocationResolver that is a copy of this ILocationResolver.</returns>
        ILocationResolver Clone();
    }
}
