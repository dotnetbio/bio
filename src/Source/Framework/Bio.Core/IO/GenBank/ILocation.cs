using System;
using System.Collections.Generic;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// Interface to hold location information.
    /// </summary>
    public interface ILocation : IComparable
    {
        /// <summary>
        /// Start position of the location.
        /// Note that this is one based position.
        /// </summary>
        int LocationStart { get; }

        /// <summary>
        /// End position of the location.
        /// Note that this is one based position.
        /// </summary>
        int LocationEnd { get; }

        /// <summary>
        /// Start position data.
        /// All positions are one based.
        /// </summary>
        string StartData { get; set; }

        /// <summary>
        /// End position data.
        /// All positions are one based.
        /// </summary>
        string EndData { get; set; }

        /// <summary>
        /// Start and end positions separator.
        /// </summary>
        string Separator { get; set; }

        /// <summary>
        /// Operator like none, complement, join and order.
        /// </summary>
        LocationOperator Operator { get; set; }

        /// <summary>
        /// Sub locations.
        /// </summary>
        List<ILocation> SubLocations { get; }

        /// <summary>
        /// Accession number of referred sequence.
        /// </summary>
        string Accession { get; set; }

        /// <summary>
        /// Returns true if the specified position is within the start positions of the location.
        /// </summary>
        /// <param name="position">Position to be verified.</param>
        bool IsInStart(int position);

        /// <summary>
        /// Returns true if the specified position is within the end positions of the location.
        /// </summary>
        /// <param name="position">Position to be verified.</param>
        bool IsInEnd(int position);

        /// <summary>
        /// Returns true if the specified position is within the start and end positions of the location.
        /// </summary>
        /// <param name="position">Position to be verified.</param>
        bool IsInRange(int position);

        /// <summary>
        /// Returns a sequence which contains bases from the specified sequences as specified by this location.
        /// </summary>
        /// <param name="sequence">Sequence from which the sub sequence has to be returned.</param>
        ISequence GetSubSequence(ISequence sequence);

        /// <summary>
        /// Returns a sequence which contains bases from the specified sequences as specified by this location.
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
        /// <param name="sequence">Sequence instance from which the sub sequence has to be returned.</param>
        /// <param name="referredSequences">A dictionary containing Accession numbers as keys and Sequences as values, this will be used when
        /// the location or sub-locations contains accession.</param>
        ISequence GetSubSequence(ISequence sequence, Dictionary<string, ISequence> referredSequences);

        /// <summary>
        /// Returns the leaf locations.
        /// </summary>
        List<ILocation> GetLeafLocations();

        /// <summary>
        /// Creates a new ILocation that is a copy of the current ILocation.
        /// </summary>
        /// <returns>A new ILocation that is a copy of this ILocation.</returns>
        ILocation Clone();
    }
}
