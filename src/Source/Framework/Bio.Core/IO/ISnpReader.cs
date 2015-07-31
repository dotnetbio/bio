using System.Collections.Generic;

namespace Bio.IO
{
    /// <summary>
    /// Interface for exposing a collection of SnpItems as an enumerator with 
    /// ability to skip to specific chromosome number and position.
    /// </summary>
    public interface ISnpReader : IEnumerator<SnpItem>
    {
        /// <summary>
        /// This moves the cursor to the current/next SNP item containing the given 
        /// chromosome number (i.e. Current.Chromosome == chromosomeNumber), 
        /// or beyond the end of the enumerator if none exist. 
        /// This is useful when traversing sorted SNP sequences and we need to 
        /// jump to a specific chromosome number.
        /// The enumerator should be positioned at or after the first item and at or before the last item.
        /// </summary>
        /// <param name="chromosomeNumber">The chromosome number for the Snpitem to move to.</param>
        /// <returns>True if the cursor was moved to a SnpItem with the given chromosome number. 
        /// False if no subsequent SnpItems exist with given chromosome number.
        /// If returning true, the Current item will have given chromosome number.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// The enumerator is positioned before the first element of the collection or after the last element.
        /// </exception>
        bool SkipToChromosome (int chromosomeNumber);

        /// <summary>
        /// This moves the cursor to the current/next SNP item containing the given 
        /// chromosome number and position 
        /// (i.e. Current.Chromosome == chromosomeNumber and Current.Position == position), 
        /// or beyond the end of the enumerator if none exist.
        /// The enumerator should be positioned at or after the first item and at 
        /// or before the last item.
        /// NOTE: This is useful when traversing sorted SNP sequences and need to jump to a 
        /// specific chromosome number+position.
        /// </summary>
        /// <param name="chromosomeNumber">The chromosome number for the Snpitem to move to.</param>
        /// <param name="position">The position within chromosome number for the Snpitem to move to.</param>
        /// <returns>Returns true if a SnpItem with given chromosome number and position is found.
        /// If end of file is reached before this they are found, it returns false. 
        /// If returning true, the Current item will have given chromosome number and position.</returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// The enumerator is positioned before the first element of the collection or after the last element.
        /// </exception>
        bool SkipToChromosomePosition (int chromosomeNumber, int position);

        /// <summary>
        /// This moves the cursor to the current/next SNP item containing the current chromosome number
        /// and position equal to the given position
        /// (i.e. Current.Position == position and Current.Chromosome does not change), 
        /// or beyond the end of the enumerator if none exist.
        /// The enumerator should be positioned at or after the first item and at 
        /// or before the last item.
        /// NOTE: This is useful when traversing sorted SNP sequences and need to jump to a 
        /// specific chromosome position within current chromosome position.
        /// </summary>
        /// <param name="position">The position within chromosome number for the Snpitem to move to.</param>
        /// <returns>Returns true if this position is found. 
        /// False if the next chromosome number is encountered, or end of file is 
        /// reached before the position is found. If returning true, the Current item 
        /// will contain the starting chromosome number and given position.</returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// The enumerator is positioned before the first element of the collection or after the last element.
        /// </exception>
        bool SkipToChromosomePosition (int position);
    }
}
