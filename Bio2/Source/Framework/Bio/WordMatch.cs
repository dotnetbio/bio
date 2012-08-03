using System;
using System.Collections.Generic;
using Bio.Algorithms.Kmer;
using System.Linq;

namespace Bio
{
    /// <summary>
    /// WordMatch stores the region of similarity between two sequences.
    /// </summary>
    public class WordMatch : IComparable, IComparable<WordMatch>, IEquatable<WordMatch>
    {
        /// <summary>
        /// Length of the match.
        /// </summary>
        private int length;

        /// <summary>
        /// Start index of the first sequence.
        /// </summary>
        private int sequence1Start;

        /// <summary>
        /// Start index of the second sequence.
        /// </summary>
        private int sequence2Start;

        /// <summary>
        /// Initializes a new instance of the WordMatch class.
        /// </summary>
        /// <param name="length">Length of the match</param>
        /// <param name="sequence1Start">Start index of the first sequence.</param>
        /// <param name="sequence2Start"> Start index of the second sequence.</param>
        public WordMatch(int length, int sequence1Start, int sequence2Start)
        {
            this.length = length;
            this.sequence1Start = sequence1Start;
            this.sequence2Start = sequence2Start;
        }

        /// <summary>
        /// Gets or sets the length of the match
        /// </summary>
        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        /// <summary>
        /// Gets or sets the start index of the first sequence.
        /// </summary>
        public int Sequence1Start
        {
            get { return sequence1Start; }
            set { sequence1Start = value; }
        }

        /// <summary>
        /// Gets or sets the start index of the second sequence.
        /// </summary>
        public int Sequence2Start
        {
            get { return sequence2Start; }
            set { sequence2Start = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this word match should be considered or not.
        /// </summary>
        public bool Deleted
        {
            get;
            set;
        }

        /// <summary>
        /// Given a list of matches, reduce it to the minimal set of best
        /// non-overlapping matches.
        /// </summary>
        /// <param name="completeList">List of matches to reduce to non-overlapping set.</param>
        /// <param name="wordLength">Wordlength entered by the user.</param>
        /// <returns>Minimal set of best non-overlapping matches.</returns>
        public static List<WordMatch> GetMinimalList(List<WordMatch> completeList, int wordLength)
        {
            List<WordMatch> minimalList = new List<WordMatch>();

            completeList.Sort();

            foreach (WordMatch wordMatch in completeList)
            {
                int deadx1 = 0;
                int deady1 = 0;
                int deadx2 = 0;
                int deady2 = 0;

                if (!wordMatch.Deleted)
                {
                    // First pos of match
                    deadx1 = wordMatch.sequence1Start;

                    // First pos of match
                    deady1 = wordMatch.sequence2Start;

                    // Last pos of match
                    deadx2 = wordMatch.sequence1Start + wordMatch.length - 1;

                    // Last pos of match
                    deady2 = wordMatch.sequence2Start + wordMatch.length - 1;

                    foreach (WordMatch innerWordMatch in completeList)
                    {
                        if (wordMatch != innerWordMatch && !innerWordMatch.Deleted)
                        {
                            // Want to remove this match if it is in the dead zone
                            bool result = WordDeadZone(innerWordMatch, deadx1, deady1, deadx2, deady2, wordLength);

                            if (result)
                            {
                                // It is in the dead zone - remove it
                                // Need to free up the match structure and remove the
                                // current node of the list                                
                                innerWordMatch.Deleted = true;
                            }
                        }
                    }
                }
            }

            foreach (WordMatch wordMatch in completeList)
            {
                if (!wordMatch.Deleted)
                {
                    minimalList.Add(wordMatch);
                }
            }

            return minimalList;
        }

        /// <summary>
        ///  Create a list of all the matches and order them by the
        /// second sequence.
        /// </summary>
        /// <param name="kmerList">List of kmer's.</param>
        /// <param name="seq2">Second sequence.</param>
        /// <param name="wordLength">Wordlength entered by the user</param>
        /// <returns>List of all the matches.</returns>
        public static List<WordMatch> BuildMatchTable(KmersOfSequence kmerList, ISequence seq2, int wordLength)
        {
            if (seq2 == null)
            {
                throw new ArgumentNullException("seq2");
            }

            int i = 0;
            int ilast = (int)seq2.Count - wordLength;
            List<WordMatch> wordCurList = new List<WordMatch>();
            List<WordMatch> hitList = new List<WordMatch>();
            bool matched = false;

            while (i < (ilast + 1))
            {
                IList<long> positions = FindCorrespondingMatch(new string(seq2.Skip(i).Take(wordLength).Select(a => (char)a).ToArray()), kmerList);

                if (positions != null)
                {
                    int kcur = 0;
                    int kcur2 = 0;
                    int knew = 0;

                    if (wordCurList.Count > 0)
                    {
                        WordMatch curmatch = wordCurList[0];
                        kcur = curmatch.sequence1Start + curmatch.length - wordLength + 1;
                        kcur2 = curmatch.sequence2Start + curmatch.length - wordLength + 1;
                    }

                    foreach (int position in positions)
                    {
                        knew = position;

                        matched = false;

                        foreach (WordMatch curmatch in wordCurList)
                        {
                            if (!curmatch.Deleted)
                            {
                                kcur = curmatch.sequence1Start + curmatch.length -
                                    wordLength + 1;
                                kcur2 = curmatch.sequence2Start + curmatch.length -
                                    wordLength + 1;

                                // When we test, we may have already incremented
                                // one of the matches - so test old and new kcur2
                                if (kcur2 != i && kcur2 != i + 1)
                                {
                                    curmatch.Deleted = true;
                                    continue;
                                }

                                if (kcur == knew && kcur2 == i)
                                {
                                    curmatch.length++;
                                    matched = true;
                                    continue;
                                }
                            }
                        }

                        if (!matched)
                        {
                            // New current match
                            WordMatch match2 = new WordMatch(wordLength, knew, i);
                            hitList.Add(match2);
                            wordCurList.Add(match2);
                        }
                    }
                }

                i++;
            }

            wordCurList.Sort();

            foreach (WordMatch curmatch in wordCurList)
            {
                curmatch.Deleted = false;
            }

            return wordCurList;
        }

        #region IComparable Members

        /// <summary>
        /// CompareTo method is used while sorting WordMatch objects. 
        /// </summary>
        /// <param name="obj">WordMatch object</param>
        /// <returns>
        /// Returns zero if the objects are equal, 
        /// Else, returns zero if the objects have the same length, sequence1start and sequence2Start 
        /// If lengths are equal, then the objects are ordered by sequence1start
        /// If lengths are equal and sequence1Start are equal, then the objects are ordered by 
        /// </returns>
        public int CompareTo(object obj)
        {
            WordMatch other = obj as WordMatch;
            if (other == null)
            {
                return -1;
            }
            else
            {
                return CompareTo(other);
            }
        }

        #endregion

        #region IComparable<WordMatch> Members

        /// <summary>
        ///  Compares two sequence matches so the result can be used in sorting.
        /// The comparison is done by size and if the size is equal, by seq1
        /// start position.  If the sequence1 start positions are equal they are
        /// sorted by sequence2 start position.
        /// </summary>
        /// <param name="other">WordMatch object</param>
        /// <returns>
        /// Returns zero if the objects have the same length, sequence1start and sequence2Start 
        /// If lengths are equal, then the objects are ordered by sequence1start
        /// If lengths are equal and sequence1Start are equal, then the objects are ordered by sequence2start
        /// </returns>
        public int CompareTo(WordMatch other)
        {
            if (other != null)
            {
                if (other.length == length)
                {
                    if (other.sequence1Start == sequence1Start)
                    {
                        if (other.sequence2Start == sequence2Start)
                        {
                            return 0;
                        }
                        else
                        {
                            return sequence2Start - other.sequence2Start;
                        }
                    }
                    else
                    {
                        return sequence1Start - other.sequence1Start;
                    }
                }
                else
                {
                    return other.length - length;
                }
            }

            return -1;
        }

        #endregion

        #region IEquatable<WordMatch> Members

        /// <summary>
        /// Checks if another WordMatch object is equal to the current
        /// object.
        /// </summary>
        /// <param name="other">WordMatch object to be compared.</param>
        /// <returns>
        /// true: if the objects are equal else false.
        /// </returns>
        public bool Equals(WordMatch other)
        {
            return length == other.length
                && sequence1Start == other.sequence1Start
                && sequence2Start == other.sequence2Start;
        }
        #endregion

        /// <summary>
        /// Determines if a match is within the region which is not overlapped by the
        /// match starting at position (deadx1, deady1) or ending at position
        /// (deadx2, deady2). If it is in this region
        /// (the 'live zone') then true is returned, else false is returned.
        /// </summary>
        /// <param name="wordMatch">Word Match object which holds the similarity of the two sequences.</param>
        /// <param name="deadx1">starting x-position of the region for which overlapped has to be checked.</param>
        /// <param name="deady1">starting y-position of the region for which overlapped has to be checked.</param>
        /// <param name="deadx2">ending x-position of the region for which overlapped has to be checked.</param>
        /// <param name="deady2">ending y-position of the region for which overlapped has to be checked.</param>
        /// <param name="wordLength">Wordlength entered by the user</param>
        /// <returns>
        /// true: if the wordMatch is in the overlapped region, else false.
        /// </returns>
        private static bool WordDeadZone(WordMatch wordMatch, int deadx1, int deady1, int deadx2, int deady2, int wordLength)
        {
            int startx;
            int starty;
            int endx;
            int endy;

            startx = wordMatch.sequence1Start;
            starty = wordMatch.sequence2Start;

            endx = wordMatch.sequence1Start + wordMatch.length - 1;
            endy = wordMatch.sequence2Start + wordMatch.length - 1;

            // Is it in the top right live zone?
            if (startx > deadx2 && starty > deady2)
            {
                return false;
            }

            // Is it in the bottom right live zone?
            if (endx < deadx1 && endy < deady1)
            {
                return false;
            }

            // Is it in the top left dead zone? 
            if (starty >= deady1 && endx <= deadx2)
            {
                return true;
            }

            // Is it in the bottom right dead zone?
            if (endy <= deady2 && startx >= deadx1)
            {
                return true;
            }

            if (endy < deady2)
            {
                if (startx - starty < deadx1 - deady1)
                {
                    // Crosses deady1
                    wordMatch.length = deady1 - starty;
                }
                else if (startx - starty > deadx1 - deady1)
                {
                    // Crosses deadx1
                    wordMatch.length = deadx1 - startx;
                }
            }
            else if (starty > deady1)
            {
                if (startx - starty < deadx1 - deady1)
                {
                    // Crosses deadx2
                    wordMatch.length = endx - deadx2;
                    wordMatch.sequence1Start = deadx2 + 1;
                    wordMatch.sequence2Start += deadx2 - startx + 1;
                }
                else if (startx - starty > deadx1 - deady1)
                {
                    // Crosses deady2
                    wordMatch.length = endy - deady2;
                    wordMatch.sequence1Start += deady2 - starty + 1;
                    wordMatch.sequence2Start = deady2 + 1;
                }
            }

            if (wordMatch.length < wordLength)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Finds the sequence in the list of IKmer and returns the list of position
        /// of the Kmers.
        /// </summary>
        /// <param name="sequence">Sequence which has to be matched in the list of IKmer.</param>
        /// <param name="kmerList">List of IKmer.</param>
        /// <returns>Returns the list of position of IKmer.</returns>
        private static IList<long> FindCorrespondingMatch(string sequence, KmersOfSequence kmerList)
        {
            IList<long> positions = null;

            foreach (KmersOfSequence.KmerPositions kmer in kmerList.Kmers)
            {
                string kmerString = new string(kmerList.KmerToSequence(kmer).Select(a => (char)a).ToArray());
                
                if (sequence.Equals(kmerString))
                {
                    positions = kmer.Positions;
                    break;
                }
            }

            return positions;
        }

        /// <summary>
        /// Overrides hash function for a particular type.
        /// </summary>
        /// <returns>hash code</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Overrides the equal method
        /// </summary>
        /// <param name="obj">Object to be checked</param>
        /// <returns>Is equals</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return base.Equals(obj);
        }

        /// <summary>
        /// Override equal operator
        /// </summary>
        /// <param name="leftHandSideObject">LHS object</param>
        /// <param name="rightHandSideObject">RHS object</param>
        /// <returns>Is LHS == RHS</returns>
        public static bool operator ==(WordMatch leftHandSideObject, WordMatch rightHandSideObject)
        {
            if (System.Object.ReferenceEquals(leftHandSideObject, rightHandSideObject))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Override not equal operator
        /// </summary>
        /// <param name="leftHandSideObject">LHS object</param>
        /// <param name="rightHandSideObject">RHS object</param>
        /// <returns>Is LHS == RHS</returns>
        public static bool operator !=(WordMatch leftHandSideObject, WordMatch rightHandSideObject)
        {
            return !(leftHandSideObject == rightHandSideObject);
        }

        /// <summary>
        /// Override less than operator
        /// </summary>
        /// <param name="leftHandSideObject">LHS object</param>
        /// <param name="rightHandSideObject">RHS object</param>
        /// <returns>Is LHS == RHS</returns>
        public static bool operator <(WordMatch leftHandSideObject, WordMatch rightHandSideObject)
        {
            if (object.ReferenceEquals(leftHandSideObject, null) || object.ReferenceEquals(rightHandSideObject, null))
            {
                return false;
            }

            return (leftHandSideObject.CompareTo(rightHandSideObject) < 0);
        }

        /// <summary>
        /// Override greater than operator
        /// </summary>
        /// <param name="leftHandSideObject">LHS object</param>
        /// <param name="rightHandSideObject">RHS object</param>
        /// <returns>Is LHS == RHS</returns>
        public static bool operator >(WordMatch leftHandSideObject, WordMatch rightHandSideObject)
        {
            if (object.ReferenceEquals(leftHandSideObject, null) || object.ReferenceEquals(rightHandSideObject, null))
            {
                return false;
            }

            return (leftHandSideObject.CompareTo(rightHandSideObject) > 0);
        }
    }
}
