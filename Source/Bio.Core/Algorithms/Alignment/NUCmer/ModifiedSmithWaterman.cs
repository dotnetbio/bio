using System;
using System.Collections.Generic;
using Bio.SimilarityMatrices;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// Implements algorithm to extend given pair of sequences in
    /// specified direction. This implementation is specific to NUCmer.
    /// </summary>
    public class ModifiedSmithWaterman
    {
        // Operation / Functions that can be performed on given pair of sequences

        /// <summary>
        /// Direction in which the method has to be implemented
        /// </summary>
        internal const int DirectionFlag = 0x1;

        /// <summary>
        /// Perform alignment
        /// </summary>
        internal const int AlignFlag = 0x2;

        /// <summary>
        /// Maximise the alignment score
        /// </summary>
        internal const int OptimalFlag = 0x8;

        /// <summary>
        /// Align till end of shortest sequence
        /// </summary>
        internal const int SeqendFlag = 0x10;

        // Types of alignment

        /// <summary>
        /// Align forward till the score or target reached
        /// </summary>
        internal const int ForwardAlignFlag = 0x1;

        /// <summary>
        /// Align backward till the score or target reached
        /// </summary>
        internal const int BackwardAlignFlag = 0x2;

        /// <summary>
        /// Force the alignment till the end irrespective of the score
        /// </summary>
        internal const int ForcedFlag = 0x4;

        /// <summary>
        /// Ignore score and align to reach the target
        /// </summary>
        internal const int ForcedForwardAlignFlag = 0x5;

        /// <summary>
        /// Maximum number of bases till the alignment can be extended
        /// </summary>
        internal const int MaximumAlignmentLength = 10000;

        /// <summary>
        /// Number of bases to be extended before stopping alignment
        /// </summary>
        internal const int DefaultBreakLength = 200;

        /// <summary>
        /// Default valid score
        /// </summary>
        private const int DefaultValidScore = 3;

        /// <summary>
        /// Default substitution score
        /// </summary>
        private const int DefaultGapExtensionScore = -7;

        /// <summary>
        /// Default gap opening score
        /// </summary>
        private const int DefaultGapOpeningScore = -10;

        /// <summary>
        /// Use this character if non alphabet is encountered in sequence
        /// </summary>
        private const char StopCharacter = 'O';

        // State of mutation sequences

        /// <summary>
        /// Represents deletion at the given base
        /// </summary>
        private const int DeleteState = 0;

        /// <summary>
        /// Represents insertioin at the given base
        /// </summary>
        private const int InsertState = 1;

        /// <summary>
        /// Represents a match at the given base
        /// </summary>
        private const int MatchState = 2;

        /// <summary>
        /// Represents a start at the given base
        /// </summary>
        private const int StartState = 3;

        /// <summary>
        /// unknown state at the given base
        /// </summary>
        private const int NoneState = 4;

        /// <summary>
        /// Initializes a new instance of the ModifiedSmithWaterman class
        /// </summary>
        internal ModifiedSmithWaterman()
        {
            ValidScore = DefaultValidScore;
            GapExtensionScore = DefaultGapExtensionScore;
            gapOpeningScore = DefaultGapOpeningScore;
            BreakLength = DefaultBreakLength;
        }

        /// <summary>
        /// Gets or sets valid score
        /// </summary>
        internal int ValidScore { get; set; }

        /// <summary>
        /// Gets or sets substitution score
        /// </summary>
        internal int GapExtensionScore { get; set; }

        /// <summary>
        /// Gets or sets diagonal score to be used to calculate scores.
        /// </summary>
        internal SimilarityMatrix SimilarityMatrix { get; set; }

        /// <summary>
        /// Gets or sets number of bases to be extended before stopping alignment
        /// </summary>
        internal int BreakLength { get; set; }

        /// <summary>
        /// Gets or sets gap opening score
        /// </summary>
        private int gapOpeningScore;

        /// <summary>
        /// Performs the function specified by the methodName on given pair of
        /// sequences, represented by the start and end indices.
        /// Find the diagonal in which the highest score is achieved
        /// 1. Find the dimension of score matrix
        /// 2. Calculate diagonals till maximum score is reached or the end 
        ///         (free the Nodes memory as when the values are no more required)
        ///     a. Create and fill the Matrix
        ///     b. Trim unrequired diagonal nodes (left)
        ///     c. Trim unrequired diagonal nodes (right)
        /// 3. Trace the path back to the highest scoring diagonal and generate the delta
        /// </summary>
        /// <param name="referenceSequence">Reference sequence</param>
        /// <param name="referenceStart">Start index of the reference sequence</param>
        /// <param name="referenceEnd">End index of the reference sequence</param>
        /// <param name="querySequence">Query sequence</param>
        /// <param name="queryStart">Start index of the query sequence</param>
        /// <param name="queryEnd">End index of the query sequence</param>
        /// <param name="deltas">List of deltas</param>
        /// <param name="methodName">Name of the method to be implemented</param>
        /// <returns>Is aligned</returns>
        internal bool ExtendSequence(
                ISequence referenceSequence,
                long referenceStart,
                ref long referenceEnd,
                ISequence querySequence,
                long queryStart,
                ref long queryEnd,
                IList<long> deltas,
                int methodName)
        {
            List<Diagonal> diagonals;
            bool isSequenceExtended;
            long referencePointer, queryPointer;
            int currentDiagonal, previousPreviousDiagonal, previousDiagonal,
                previousPreviousDiagonalIndex, previousDiagonalIndex, diagonalIndex,
                previousPreviousDiagonalSize, previousDiagonalSize, diagonalSize,
                insertScore, deleteScore, matchScore, insertValue, deleteValue, matchValue,
                conceptualDiagonal;
            int minimumScore = -1 * Int32.MaxValue;
            int score = minimumScore;
            int nonOptimalScore = minimumScore;
            int maximumDifference = ValidScore * BreakLength;
            int diagonalLength = 2;
            int left = 0;
            int right = 0;
            long targetHighScoreDiagonal = 0;
            long targetHighScoreDiagonalIndex = 0;
            int targetDiagonal = 0;
            long targetDiagonalIndex = 0;
            long row, column, lowerIndex;

            if (0 < (methodName & DirectionFlag))
            {
                referencePointer = referenceStart - 1;
                queryPointer = queryStart - 1;
                row = referenceEnd - referenceStart + 1;
                column = queryEnd - queryStart + 1;
            }
            else
            {
                referencePointer = referenceStart + 1;
                queryPointer = queryStart + 1;
                row = referenceStart - referenceEnd + 1;
                column = queryStart - queryEnd + 1;
            }

            diagonals = new List<Diagonal>();

            // Initialize first instance of diagonal
            diagonals.Add(new Diagonal());
            diagonals[0].Left = left;
            diagonals[0].Right = right++;

            diagonals[0].Nodes.Add(new Node());
            diagonals[0].Nodes[0].Scores[DeleteState].Value = minimumScore;
            diagonals[0].Nodes[0].Scores[InsertState].Value = minimumScore;
            diagonals[0].Nodes[0].Scores[MatchState].Value = 0;
            diagonals[0].Nodes[0].MaximumScore = diagonals[0].Nodes[0].Scores[MatchState];

            diagonals[0].Nodes[0].Scores[DeleteState].State = NoneState;
            diagonals[0].Nodes[0].Scores[InsertState].State = NoneState;
            diagonals[0].Nodes[0].Scores[MatchState].State = StartState;

            lowerIndex = row < column ? row : column;

            // Calculate diagonals till minimum score is reached or the end
            for (currentDiagonal = 1;
                currentDiagonal <= row + column
                    && (currentDiagonal - targetHighScoreDiagonal) <= BreakLength
                    && left <= right;
                currentDiagonal++)
            {
                if (currentDiagonal >= diagonals.Count)
                {
                    diagonals.Add(new Diagonal());
                }

                diagonals[currentDiagonal].Left = left;
                diagonals[currentDiagonal].Right = right;

                diagonalSize = right - left + 1;
                diagonals[currentDiagonal].Nodes = new List<Node>();

                // calculate the scores
                if (currentDiagonal <= row)
                {
                    insertScore = 0;
                    matchScore = -1;
                }
                else
                {
                    insertScore = 1;
                    matchScore = currentDiagonal == row + 1 ? 0 : 1;
                }

                deleteScore = insertScore - 1;

                previousDiagonal = currentDiagonal - 1;
                previousDiagonalSize = diagonals[previousDiagonal].Right - diagonals[previousDiagonal].Left + 1;
                previousDiagonalIndex = left + deleteScore;
                previousDiagonalIndex = previousDiagonalIndex - diagonals[previousDiagonal].Left;

                previousPreviousDiagonal = currentDiagonal - 2;
                if (previousPreviousDiagonal >= 0)
                {
                    previousPreviousDiagonalSize = diagonals[previousPreviousDiagonal].Right - diagonals[previousPreviousDiagonal].Left + 1;
                    previousPreviousDiagonalIndex = left + matchScore;
                    previousPreviousDiagonalIndex = previousPreviousDiagonalIndex - diagonals[previousPreviousDiagonal].Left;
                }
                else
                {
                    previousPreviousDiagonalIndex = previousPreviousDiagonalSize = 0;
                }

                if (0 < (methodName & ForcedFlag))
                {
                    score = minimumScore;
                }

                // Calculate scores
                for (conceptualDiagonal = left; conceptualDiagonal <= right; conceptualDiagonal++)
                {
                    diagonalIndex = conceptualDiagonal - diagonals[currentDiagonal].Left;

                    if (diagonalIndex >= diagonals[currentDiagonal].Nodes.Count
                        || previousDiagonalIndex >= diagonals[currentDiagonal].Nodes.Count)
                    {
                        for (int counter = diagonals[currentDiagonal].Nodes.Count;
                                counter <= diagonalIndex || counter <= previousDiagonalIndex;
                                counter++)
                        {
                            diagonals[currentDiagonal].Nodes.Add(new Node());
                        }
                    }

                    if (diagonalIndex >= diagonals[previousDiagonal].Nodes.Count
                        || previousDiagonalIndex >= diagonals[previousDiagonal].Nodes.Count)
                    {
                        for (int counter = diagonals[previousDiagonal].Nodes.Count;
                                counter <= diagonalIndex || counter <= previousDiagonalIndex;
                                counter++)
                        {
                            diagonals[previousDiagonal].Nodes.Add(new Node());
                        }
                    }

                    // Delete score
                    if (previousDiagonalIndex >= 0 && previousDiagonalIndex < previousDiagonalSize)
                    {
                        deleteValue = diagonals[previousDiagonal].Nodes[previousDiagonalIndex].Scores[DeleteState].State == NoneState
                                ? diagonals[previousDiagonal].Nodes[previousDiagonalIndex].Scores[DeleteState].Value
                                : diagonals[previousDiagonal].Nodes[previousDiagonalIndex].Scores[DeleteState].Value + GapExtensionScore;
                        insertValue = diagonals[previousDiagonal].Nodes[previousDiagonalIndex].Scores[InsertState].State == NoneState
                                ? diagonals[previousDiagonal].Nodes[previousDiagonalIndex].Scores[InsertState].Value
                                : diagonals[previousDiagonal].Nodes[previousDiagonalIndex].Scores[InsertState].Value + gapOpeningScore;
                        matchValue = diagonals[previousDiagonal].Nodes[previousDiagonalIndex].Scores[MatchState].State == NoneState
                                ? diagonals[previousDiagonal].Nodes[previousDiagonalIndex].Scores[MatchState].Value
                                : diagonals[previousDiagonal].Nodes[previousDiagonalIndex].Scores[MatchState].Value + gapOpeningScore;
                        SetScore(
                            diagonals[currentDiagonal].Nodes[diagonalIndex].Scores[DeleteState],
                            deleteValue,
                            insertValue,
                            matchValue);
                    }
                    else
                    {
                        diagonals[currentDiagonal].Nodes[diagonalIndex].Scores[DeleteState].Value = minimumScore;
                        diagonals[currentDiagonal].Nodes[diagonalIndex].Scores[DeleteState].State = NoneState;
                    }

                    previousDiagonalIndex++;

                    // Insert score
                    if (previousDiagonalIndex >= 0 && previousDiagonalIndex < previousDiagonalSize)
                    {
                        deleteValue = diagonals[previousDiagonal].Nodes[previousDiagonalIndex].Scores[DeleteState].State == NoneState
                            ? diagonals[previousDiagonal].Nodes[previousDiagonalIndex].Scores[DeleteState].Value
                            : diagonals[previousDiagonal].Nodes[previousDiagonalIndex].Scores[DeleteState].Value + gapOpeningScore;
                        insertValue = diagonals[previousDiagonal].Nodes[previousDiagonalIndex].Scores[InsertState].State == NoneState
                            ? diagonals[previousDiagonal].Nodes[previousDiagonalIndex].Scores[InsertState].Value
                            : diagonals[previousDiagonal].Nodes[previousDiagonalIndex].Scores[InsertState].Value + GapExtensionScore;
                        matchValue = diagonals[previousDiagonal].Nodes[previousDiagonalIndex].Scores[MatchState].State == NoneState
                            ? diagonals[previousDiagonal].Nodes[previousDiagonalIndex].Scores[MatchState].Value
                            : diagonals[previousDiagonal].Nodes[previousDiagonalIndex].Scores[MatchState].Value + gapOpeningScore;
                        SetScore(
                            diagonals[currentDiagonal].Nodes[diagonalIndex].Scores[InsertState],
                            deleteValue,
                            insertValue,
                            matchValue);
                    }
                    else
                    {
                        diagonals[currentDiagonal].Nodes[diagonalIndex].Scores[InsertState].Value = minimumScore;
                        diagonals[currentDiagonal].Nodes[diagonalIndex].Scores[InsertState].State = NoneState;
                    }

                    // Match score
                    if (previousPreviousDiagonalIndex >= 0 && previousPreviousDiagonalIndex < previousPreviousDiagonalSize)
                    {
                        SetScore(
                            diagonals[currentDiagonal].Nodes[diagonalIndex].Scores[MatchState],
                            diagonals[previousPreviousDiagonal].Nodes[previousPreviousDiagonalIndex].Scores[DeleteState].Value,
                            diagonals[previousPreviousDiagonal].Nodes[previousPreviousDiagonalIndex].Scores[InsertState].Value,
                            diagonals[previousPreviousDiagonal].Nodes[previousPreviousDiagonalIndex].Scores[MatchState].Value);

                        diagonals[currentDiagonal].Nodes[diagonalIndex].Scores[MatchState].Value += GetMatchScore(
                            currentDiagonal,
                            conceptualDiagonal,
                            referenceSequence,
                            referencePointer,
                            querySequence,
                            queryPointer,
                            row,
                            methodName);
                    }
                    else
                    {
                        diagonals[currentDiagonal].Nodes[diagonalIndex].Scores[MatchState].Value = minimumScore;
                        diagonals[currentDiagonal].Nodes[diagonalIndex].Scores[MatchState].State = NoneState;
                    }

                    previousPreviousDiagonalIndex++;

                    diagonals[currentDiagonal].Nodes[diagonalIndex].MaximumScore = GetMaximumScore(diagonals[currentDiagonal].Nodes[diagonalIndex].Scores);

                    // Reset the values
                    if (diagonals[currentDiagonal].Nodes[diagonalIndex].MaximumScore.Value >= score)
                    {
                        score = diagonals[currentDiagonal].Nodes[diagonalIndex].MaximumScore.Value;
                        targetHighScoreDiagonal = currentDiagonal;
                        targetHighScoreDiagonalIndex = conceptualDiagonal;
                    }
                }

                // Calculate non-optimal score
                if ((0 < (methodName & SeqendFlag)) && currentDiagonal >= lowerIndex)
                {
                    if (lowerIndex == row)
                    {
                        if (left == 0)
                        {
                            if (diagonals[currentDiagonal].Nodes[0].MaximumScore.Value >= nonOptimalScore)
                            {
                                nonOptimalScore = diagonals[currentDiagonal].Nodes[0].MaximumScore.Value;
                                targetDiagonal = currentDiagonal;
                                targetDiagonalIndex = 0;
                            }
                        }
                    }
                    else
                    {
                        if (right == column)
                        {
                            // Below cast to int is with the assumption that this will never exceed int32 limits.
                            if (diagonals[currentDiagonal].Nodes[(int)(column - diagonals[currentDiagonal].Left)].
                                 MaximumScore.Value >= nonOptimalScore)
                            {
                                // Below cast to int is with the assumption that this will never exceed int32 limits.
                                nonOptimalScore = diagonals[currentDiagonal].Nodes[(int)(column - diagonals[currentDiagonal].Left)].
                                    MaximumScore.Value;
                                targetDiagonal = currentDiagonal;
                                targetDiagonalIndex = column;
                            }
                        }
                    }
                }

                if ((0 < (methodName & AlignFlag)) && currentDiagonal > 1)
                {
                    diagonals[previousPreviousDiagonal].Nodes = null;
                }

                // Trim unrequired diagonal nodes
                for (diagonalIndex = 0; diagonalIndex < diagonalSize; diagonalIndex++)
                {
                    if (score - diagonals[currentDiagonal].Nodes[diagonalIndex].MaximumScore.Value > maximumDifference)
                    {
                        left++;
                    }
                    else
                    {
                        break;
                    }
                }

                for (diagonalIndex = diagonalSize - 1; diagonalIndex >= 0; diagonalIndex--)
                {
                    if (score - diagonals[currentDiagonal].Nodes[diagonalIndex].MaximumScore.Value > maximumDifference)
                    {
                        right--;
                    }
                    else
                    {
                        break;
                    }
                }

                //-- Grow new diagonal and reset boundaries
                if (currentDiagonal < row && currentDiagonal < column)
                {
                    diagonalLength++;
                    right++;
                }
                else if (currentDiagonal >= row && currentDiagonal >= column)
                {
                    diagonalLength--;
                    left--;
                }
                else if (currentDiagonal >= row)
                {
                    left--;
                }
                else
                {
                    right++;
                }

                if (left < 0)
                {
                    left = 0;
                }

                if (right >= diagonalLength)
                {
                    right = diagonalLength - 1;
                }
            }

            currentDiagonal--;

            // traceback score
            isSequenceExtended = false;
            if (currentDiagonal == row + column)
            {
                if ((0 < (~methodName & OptimalFlag)) || (0 < (methodName & SeqendFlag)))
                {
                    isSequenceExtended = true;
                    targetHighScoreDiagonal = row + column;
                    targetHighScoreDiagonalIndex = 0;
                }
                else if (targetHighScoreDiagonal == currentDiagonal)
                {
                    isSequenceExtended = true;
                }
            }
            else if ((0 < (methodName & SeqendFlag)) && targetDiagonal != 0)
            {
                targetHighScoreDiagonal = targetDiagonal;
                targetHighScoreDiagonalIndex = targetDiagonalIndex;
            }

            // Final index
            long referenceAdjustment = targetHighScoreDiagonal <= row ? targetHighScoreDiagonal - targetHighScoreDiagonalIndex - 1 : row - targetHighScoreDiagonalIndex - 1;
            long queryAdjustment = targetHighScoreDiagonal <= row ? targetHighScoreDiagonalIndex - 1 : targetHighScoreDiagonal - row + targetHighScoreDiagonalIndex - 1;
            if (0 < (~methodName & DirectionFlag))
            {
                referenceAdjustment *= -1;
                queryAdjustment *= -1;
            }

            referenceEnd = referenceStart + referenceAdjustment;
            queryEnd = queryStart + queryAdjustment;

            // Create delta list
            if (0 < (~methodName & AlignFlag))
            {
                GenerateDelta(diagonals, targetHighScoreDiagonal, targetHighScoreDiagonalIndex, row, deltas);
            }

            diagonals.Clear();

            return isSequenceExtended;
        }

        /// <summary>
        /// Calculate the score of operation
        /// </summary>
        /// <param name="currentScore">Current Score</param>
        /// <param name="deleteScore">Score of delete</param>
        /// <param name="insertScore">Score of Insert</param>
        /// <param name="matchScore">Score of match</param>
        private static void SetScore(
                Score currentScore,
                int deleteScore,
                int insertScore,
                int matchScore)
        {
            if (deleteScore > insertScore)
            {
                if (deleteScore > matchScore)
                {
                    currentScore.Value = deleteScore;
                    currentScore.State = DeleteState;
                }
                else
                {
                    currentScore.Value = matchScore;
                    currentScore.State = MatchState;
                }
            }
            else if (insertScore > matchScore)
            {
                currentScore.Value = insertScore;
                currentScore.State = InsertState;
            }
            else
            {
                currentScore.Value = matchScore;
                currentScore.State = MatchState;
            }
        }

        /// <summary>
        /// Find the Maximum Score in given list
        /// </summary>
        /// <param name="scores">List of scores</param>
        /// <returns>Maximum score</returns>
        private static Score GetMaximumScore(Score[] scores)
        {
            Score maxScore = scores[DeleteState].Value > scores[InsertState].Value ? scores[DeleteState] : scores[InsertState];
            return maxScore.Value > scores[MatchState].Value ? maxScore : scores[MatchState];
        }

        /// <summary>
        /// Generate the list of delta
        /// </summary>
        /// <param name="diagonals">List of diagonals</param>
        /// <param name="counter">diagonal index</param>
        /// <param name="currentDiagonal">Current diagonal index</param>
        /// <param name="length">Length of sequence</param>
        /// <param name="deltas">list of Deltas</param>
        private static void GenerateDelta(
                List<Diagonal> diagonals,
                long counter,
                long currentDiagonal,
                long length,
                IList<long> deltas)
        {
            int deltaCounter;
            int diagonalIndex = (int)counter;
            int conceptualDiagonal = (int)currentDiagonal;
            int nodeIndex = 0;
            int pathIndex = 0;
            int edit;
            List<int> path = new List<int>();
            Score currentScore;

            // Find the index with maximum match Score
            nodeIndex = conceptualDiagonal - diagonals[diagonalIndex].Left;
            if (nodeIndex < 0)
            {
                nodeIndex = 0;
            }

            Score maxScore = diagonals[diagonalIndex].Nodes[nodeIndex].MaximumScore;
            Score[] scoresToSearch = diagonals[diagonalIndex].Nodes[nodeIndex].Scores;
            edit = -1;
            if (scoresToSearch[DeleteState] == maxScore)
            {
                edit = DeleteState;
            }
            else if (scoresToSearch[InsertState] == maxScore)
            {
                edit = InsertState;
            }
            else if (scoresToSearch[MatchState] == maxScore)
            {
                edit = MatchState;
            }

            // traceback the path
            while (diagonalIndex >= 0)
            {
                nodeIndex = conceptualDiagonal - diagonals[diagonalIndex].Left;

                if (nodeIndex < 0)
                {
                    nodeIndex = 0;
                }

                if (edit > 2)
                {
                    edit = 2;
                }

                currentScore = diagonals[diagonalIndex].Nodes[nodeIndex].Scores[edit];

                if (pathIndex >= path.Count)
                {
                    for (int tempIndex = path.Count; tempIndex <= pathIndex; tempIndex++)
                    {
                        path.Add(0);
                    }
                }

                path[pathIndex++] = edit;

                switch (edit)
                {
                    case DeleteState:
                        conceptualDiagonal = diagonalIndex-- <= length ? conceptualDiagonal - 1 : conceptualDiagonal;
                        break;

                    case InsertState:
                        conceptualDiagonal = diagonalIndex-- <= length ? conceptualDiagonal : conceptualDiagonal + 1;
                        break;

                    case MatchState:
                        conceptualDiagonal = diagonalIndex <= length ? conceptualDiagonal - 1 : (diagonalIndex == length + 1 ? conceptualDiagonal : conceptualDiagonal + 1);
                        diagonalIndex -= 2;
                        break;

                    case StartState:
                        diagonalIndex = -1;
                        break;

                    default:
                        break;
                }

                edit = currentScore.State;
            }

            // Generate deltas
            deltaCounter = 1;
            for (pathIndex -= 2; pathIndex >= 0; pathIndex--)
            {
                switch (path[pathIndex])
                {
                    case DeleteState:
                        deltas.Add(-deltaCounter);
                        deltaCounter = 1;
                        break;

                    case InsertState:
                        deltas.Add(deltaCounter);
                        deltaCounter = 1;
                        break;

                    case MatchState:
                        deltaCounter++;
                        break;

                    case StartState:
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Find the Score of the given match
        /// </summary>
        /// <param name="diagonalCounter">diagonal counter</param>
        /// <param name="diagonalIndex">diagonal index</param>
        /// <param name="referenceSequence">Reference sequence</param>
        /// <param name="referenceIndex">Index of symbol in reference</param>
        /// <param name="querySequence">Query sequence</param>
        /// <param name="queryIndex">Index of symbol in query</param>
        /// <param name="length">Length of diagonal</param>
        /// <param name="methodName">Name of the method to be implemented</param>
        /// <returns>match Score</returns>
        private int GetMatchScore(
                int diagonalCounter,
                int diagonalIndex,
                ISequence referenceSequence,
                long referenceIndex,
                ISequence querySequence,
                long queryIndex,
                long length,
                int methodName)
        {
            int direction;
            char referenceCharacter;
            char queryCharacter;

            direction = (0 < (methodName & DirectionFlag)) ? 1 : -1;

            if (diagonalCounter <= length)
            {
                referenceCharacter = (char)referenceSequence[referenceIndex + ((diagonalCounter - diagonalIndex) * direction)];
                queryCharacter = (char)querySequence[queryIndex + (diagonalIndex * direction)];
            }
            else
            {
                referenceCharacter = (char)referenceSequence[referenceIndex + ((length - diagonalIndex) * direction)];
                queryCharacter = (char)querySequence[queryIndex + ((diagonalCounter - length + diagonalIndex) * direction)];
            }

            if (!Char.IsLetter(referenceCharacter))
            {
                referenceCharacter = StopCharacter;
            }

            if (!Char.IsLetter(queryCharacter))
            {
                queryCharacter = StopCharacter;
            }

            return SimilarityMatrix[(byte)char.ToUpperInvariant(referenceCharacter), (byte)char.ToUpperInvariant(queryCharacter)];
        }
    }

    /// <summary>
    /// Structure to hold the Score value
    /// </summary>
    internal class Score
    {
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Gets or sets the state
        /// </summary>
        public int State { get; set; }
    }

    /// <summary>
    /// Structure to hold the Node value
    /// </summary>
    internal class Node
    {
        /// <summary>
        /// Initializes a new instance of the Node class
        /// </summary>
        internal Node()
        {
            Scores = new Score[3] { new Score(), new Score(), new Score() };
        }

        /// <summary>
        /// Gets or sets the List of Score of given node
        /// </summary>
        internal Score[] Scores { get; set; }

        /// <summary>
        /// Gets or sets the maximum score of give node
        /// </summary>
        internal Score MaximumScore { get; set; }
    }

    /// <summary>
    /// Structure to hold the Diagonal value
    /// </summary>
    internal class Diagonal
    {
        /// <summary>
        /// Initializes a new instance of the Diagonal class
        /// </summary>
        public Diagonal()
        {
            Nodes = new List<Node>();
        }

        /// <summary>
        /// Gets or sets left index of diagonal
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        /// Gets or sets right index of diagonal
        /// </summary>
        public int Right { get; set; }

        /// <summary>
        /// Gets or sets the list of nodes
        /// </summary>
        public List<Node> Nodes { get; set; }
    }
}
