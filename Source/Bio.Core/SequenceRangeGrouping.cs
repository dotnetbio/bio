using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bio
{
    /// <summary>
    /// A grouping of SequenceRange objects sorted by their ID values. The
    /// purpose of these groups is to allow a set of SequenceRange objects
    /// to be associated together by bucketing them into groups where each
    /// bucket has a unique SequenceRange ID and all SequenceRange objects
    /// within the bucket has that same ID.
    /// </summary>
    public class SequenceRangeGrouping
    {
        private Dictionary<string, List<ISequenceRange>> groups
            = new Dictionary<string, List<ISequenceRange>>();

        /// <summary>
        /// Creates an empty grouping.
        /// </summary>
        public SequenceRangeGrouping() { }

        /// <summary>
        /// Creates a grouping object from a set of currently ungrouped
        /// ISequenceRange objects.
        /// </summary>
        /// <param name="ranges">Sequence ranges.</param>
        public SequenceRangeGrouping(IEnumerable<ISequenceRange> ranges)
        {
            if (ranges == null)
            {
                throw new ArgumentNullException("ranges");
            }

            foreach (ISequenceRange range in ranges)
            {
                Add(range);
            }
        }

        /// <summary>
        /// An enumeration of all the SequenceRange IDs contained in
        /// this grouping.
        /// </summary>
        public IEnumerable<string> GroupIDs
        {
            get
            {
                return groups.Keys;
            }
        }

        /// <summary>
        /// Gets sequence range from all the groups
        /// </summary>
        public IEnumerable<ISequenceRange> GroupRanges
        {
            get { return groups.Values.SelectMany(sr => sr); }
        }

        /// <summary>
        /// Converts sequence range from all the groups to string.
        /// </summary>
        /// <returns>Sequence range from all the groups.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (ISequenceRange range in GroupRanges)
            {
                builder.AppendLine(range.ToString());
            }
            return builder.ToString();
        }

        /// <summary>
        /// Adds a SequenceRange to the grouping, creating a new bucket
        /// if the ID of the range has not yet been added to the grouping
        /// or adding to an existing bucket if it has.
        /// </summary>
        /// <param name="range">The range item to add to the grouping</param>
        public void Add(ISequenceRange range)
        {
            if (range == null)
                return;

            if (range.ID == null)
                throw new ArgumentException("Can not group a SequenceRange that has no ID");

            if (groups.ContainsKey(range.ID))
            {
                groups[range.ID].Add(range);
            }
            else
            {
                List<ISequenceRange> list = new List<ISequenceRange>();
                list.Add(range);
                groups[range.ID] = list;
            }
        }

        /// <summary>
        /// Returns a list of SequenceRange objects each of which has the
        /// ID specified in the rangeID parameter.
        /// </summary>
        /// <param name="rangeID">The identification to look up in the grouping</param>
        /// <returns>The list of sequence ranges of the required group.</returns>
        public List<ISequenceRange> GetGroup(string rangeID)
        {
            if (!groups.ContainsKey(rangeID))
                return null;

            return groups[rangeID];
        }

        /// <summary>
        /// Ungroups each of the ISequenceRanges and places them into a flat list
        /// of every ISequenceRange stored across each of the groups in this grouping.
        /// The resulting list will still be ordered in such a way that each item of
        /// a particular group will be enumerated before starting the enumeration of
        /// items from another group.
        /// </summary>
        /// <returns>The flattened list of sequence ranges.</returns>
        public List<ISequenceRange> Flatten()
        {
            List<ISequenceRange> result = new List<ISequenceRange>();

            foreach (List<ISequenceRange> ranges in groups.Values)
            {
                foreach (ISequenceRange range in ranges)
                {
                    result.Add(range);
                }
            }

            return result;
        }

        #region Merge Operation
        /// <summary>
        /// For each group in the grouping, this method traverses through each range
        /// in the group and normalizes the ranges down to the minimal spanning set
        /// required to still show the same range spans.
        /// 
        /// For instance if you had in group 'Chr1' the following ranges:
        /// 
        /// -> 10 to 100
        /// -> 200 to 250
        /// -> 35 to 45
        /// -> 90 to 150
        /// 
        /// The result of MergeOverlaps would reduce the ranges in the 'Chr1' group to:
        /// For minOverlap = 0
        /// 
        /// -> 10 to 150
        /// -> 200 to 250
        /// 
        /// for minOverlap = -50
        /// 
        /// -> 10 to 250
        /// 
        /// Running this method creates all new ISequenceRange objects and adds them
        /// to the newly created SequenceRangeGrouping returned here.
        /// </summary>
        /// <param name="minOverlap">Minmum length of bases pairs should be overlapped.</param>
        /// <param name="isParentSeqRangesRequired">If this flag is set to true then the sequence ranges from 
        /// which the new sequence range is created are added to the ParentSeqRanges property of the 
        /// new sequence range.</param>
        /// <returns>The overlapped sequence range grouping.</returns>
        public SequenceRangeGrouping MergeOverlaps(long minOverlap = 0, bool isParentSeqRangesRequired = false)
        {
            SequenceRangeGrouping seqRangeGroup = new SequenceRangeGrouping();
            List<ISequenceRange> sortedRanges = new List<ISequenceRange>();
            foreach (List<ISequenceRange> rangeList in this.groups.Values)
            {
                sortedRanges.AddRange(rangeList);
                sortedRanges.Sort();

                while (sortedRanges.Count > 0)
                {
                    ISequenceRange seqRange = new SequenceRange(sortedRanges[0].ID,
                                                sortedRanges[0].Start,
                                                sortedRanges[0].End);
                    if (isParentSeqRangesRequired)
                    {
                        AddParent(seqRange, sortedRanges[0]);
                    }

                    sortedRanges.RemoveAt(0);
                    seqRangeGroup.Add(seqRange);

                    if (sortedRanges.Count > 0)
                    {
                        while (sortedRanges.Count > 0 && (seqRange.End - sortedRanges[0].Start) >= minOverlap)
                        {
                            seqRange.End = Math.Max(seqRange.End, sortedRanges[0].End);
                            if (isParentSeqRangesRequired)
                            {
                                AddParent(seqRange, sortedRanges[0]);
                            }

                            sortedRanges.RemoveAt(0);
                        }
                    }
                }

                sortedRanges.Clear();
            }

            return seqRangeGroup;
        }

        /// <summary>
        /// Merges query sequence ranges with this sequence ranges.
        /// 
        /// For example,
        /// 
        ///  Ranges in this instance   Ranges in the query 
        ///    3 to  15                   4 to 10
        ///    5 to  18                  11 to 20
        /// 
        ///  Result for minOverlap set to 1
        ///   3 to 20
        /// 
        /// Running this method creates all new ISequenceRange objects and adds them
        /// to the newly created SequenceRangeGrouping returned here.
        /// </summary>
        /// <param name="query">Query sequence ranges.</param>
        /// <param name="minOverlap">Minmum length of bases pairs should be overlapped.</param>
        /// <param name="isParentSeqRangesRequired">If this flag is set to true then the sequence ranges from 
        /// which the new sequence range is created are added to the ParentSeqRanges property of the 
        /// new sequence range.</param>
        /// <returns>The merged sequence range grouping.</returns>
        public SequenceRangeGrouping MergeOverlaps(SequenceRangeGrouping query, long minOverlap = 0, bool isParentSeqRangesRequired = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameQuery);
            }

            List<ISequenceRange> ranges = new List<ISequenceRange>(this.Flatten());
            ranges.AddRange(query.Flatten());
            SequenceRangeGrouping seqReangeGroup = new SequenceRangeGrouping(ranges);

            return seqReangeGroup.MergeOverlaps(minOverlap, isParentSeqRangesRequired);
        }

        #endregion

        #region Intersect Operation

        /// <summary>
        /// Returns overlapping sequence ranges from this and specified SequenceRangeGroup for each group in this grouping.
        /// 
        /// For instance if you had in group 'Chr1' the following ranges:
        /// 
        ///  Ranges in this instance   Ranges in the query 
        ///    0 to   10                 20 to   40
        ///   30 to   50                 70 to  100     
        ///   60 to   80                400 to  800
        ///  300 to  500                850 to  900
        ///  600 to  700                900 to 1200
        ///  800 to 1000                
        /// 
        /// Result for minOverlap set to 1
        ///     1. If outputType is OverlappingPiecesOfIntervals.
        ///         30 to 40
        ///         70 to 80
        ///         400 to 500
        ///         600 o 700
        ///         850 to 900
        ///         900 to 1000
        ///     2. If outputType is OverlappingIntervals
        ///          30 to   50
        ///          60 to   80
        ///         300 to  500
        ///         600 to  700
        ///         800 to 1000
        ///         
        /// Running this method creates all new ISequenceRange objects and adds them
        /// to the newly created SequenceRangeGrouping returned here.
        /// </summary>
        /// <param name="query">Query grouping.</param>
        /// <param name="minOverlap">Minmum length of bases pairs should be overlapped.
        /// By default this will be set to 1.</param>
        /// <param name="outputType">
        /// Type of output required, OverlappingPiecesOfIntervals or OverlappingIntervals. 
        /// By default this will be set to OverlappingPiecesOfIntervals that is only the base pairs that overlaps with 
        /// query ranges will be returned.</param>
        /// <param name="isParentSeqRangesRequired">If this flag is set to true then the sequence ranges from 
        /// which the new sequence range is created are added to the ParentSeqRanges property of the 
        /// new sequence ranges.</param>
        /// <returns>The intersected result.</returns>
        public SequenceRangeGrouping Intersect(SequenceRangeGrouping query, long minOverlap = 1, IntersectOutputType outputType = IntersectOutputType.OverlappingPiecesOfIntervals, bool isParentSeqRangesRequired = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            SequenceRangeGrouping result = new SequenceRangeGrouping();
            List<ISequenceRange> refSeqRanges = new List<ISequenceRange>();
            List<ISequenceRange> querySeqRanges = new List<ISequenceRange>();
            SequenceRange range = null;

            // merge the query sequence ranges.
            IList<ISequenceRange> queryList = null;
            if (isParentSeqRangesRequired)
            {
                queryList = query.Flatten();
            }

            query = query.MergeOverlaps(0, isParentSeqRangesRequired);

            foreach (string id in groups.Keys)
            {
                refSeqRanges.Clear();
                querySeqRanges.Clear();

                refSeqRanges.AddRange(groups[id]);

                if (query.groups.ContainsKey(id))
                {
                    querySeqRanges.AddRange(query.groups[id]);
                    querySeqRanges.Sort();
                }

                if (querySeqRanges.Count > 0)
                {
                    foreach (ISequenceRange refRange in refSeqRanges)
                    {
                        IList<ISequenceRange> overlappingQueryRanges = GetOverlappingRenges(refRange, querySeqRanges, minOverlap);

                        if (overlappingQueryRanges == null || overlappingQueryRanges.Count == 0)
                        {
                            // If the minOverlap is lessthan or equal to zero and overlapping intervals are required.
                            // then add the ref seq to result.
                            if (minOverlap <= 0 && outputType == IntersectOutputType.OverlappingIntervals)
                            {
                                range = new SequenceRange(refRange.ID, refRange.Start, refRange.End);
                                CopyOfMetadata(range, refRange);

                                result.Add(range);

                                if (isParentSeqRangesRequired)
                                {
                                    AddParent(range, refRange);
                                }
                            }

                            continue;
                        }

                        ISequenceRange previousOverlappingRange = null;
                        foreach (ISequenceRange queryRange in overlappingQueryRanges)
                        {
                            if (outputType == IntersectOutputType.OverlappingPiecesOfIntervals)
                            {
                                // Add ref sequence only once for query ranges having same start and end.
                                if (previousOverlappingRange == null || (previousOverlappingRange.Start != queryRange.Start && previousOverlappingRange.End != queryRange.End))
                                {
                                    range = new SequenceRange(
                                        refRange.ID,
                                        Math.Max(queryRange.Start, refRange.Start),
                                        Math.Min(queryRange.End, refRange.End));

                                    result.Add(range);
                                    CopyOfMetadata(range, refRange);

                                    if (isParentSeqRangesRequired)
                                    {
                                        AddParent(range, refRange);
                                    }
                                }

                                if (isParentSeqRangesRequired)
                                {
                                    if (queryList.Contains(queryRange))
                                    {
                                        AddParent(range, queryRange);
                                    }
                                    else
                                    {
                                        if (queryRange.ParentSeqRanges.Count > 0)
                                        {
                                            AddParent(range, queryRange.ParentSeqRanges);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Add ref sequence only once.
                                if (previousOverlappingRange == null)
                                {
                                    range = new SequenceRange(refRange.ID, refRange.Start, refRange.End);
                                    CopyOfMetadata(range, refRange);
                                    result.Add(range);
                                    if (isParentSeqRangesRequired)
                                    {
                                        AddParent(range, refRange);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (isParentSeqRangesRequired)
                                {
                                    if (queryList.Contains(queryRange))
                                    {
                                        AddParent(range, queryRange);
                                    }
                                    else
                                    {
                                        if (queryRange.ParentSeqRanges.Count > 0)
                                        {
                                            AddParent(range, queryRange.ParentSeqRanges);
                                        }
                                    }
                                }
                            }

                            previousOverlappingRange = queryRange;
                        }
                    }
                }
            }

            return result;
        }
        #endregion

        #region Subtract Operation

        /// <summary>
        /// Subtracts the query SequenceRangeGrouping from this SequenceRangeGrouping.
        /// 
        /// For example,
        /// 
        ///  Ranges in this instance   Ranges in the query 
        ///     1 to  4                   2 to  6
        ///     4 to  8                   3 to  6
        ///     8 to 12                   9 to 14
        ///    25 to 35
        ///    
        /// Result for minOverlap set to 1
        /// 1. If outputType is IntervalsWithNoOverlap
        ///    25 to 35
        ///    
        /// 2. If outputType is NonOverlappingPiecesOfIntervals
        ///    1 to  2
        ///    6 to  8
        ///    8 to  9
        ///   25 to 35
        ///   
        /// Running this method creates all new ISequenceRange objects and adds them
        /// to the newly created SequenceRangeGrouping returned here.
        /// </summary>
        /// <param name="query">Query grouping.</param>
        /// <param name="minOverlap">Minmum length of overlap. By default this will be set to 1</param>
        /// <param name="outputType">
        /// Type of output required, IntervalsWithNoOverlap or NonOverlappingPiecesOfIntervals. 
        /// By default this will be set to NonOverlappingPiecesOfIntervals that is non overlapping 
        /// pieces of intervels along with non overlapping ranges from this instance 
        /// will be returned.
        /// </param>
        /// <param name="isParentSeqRangesRequired">If this flag is set to true then the sequence ranges from 
        /// which the new sequence range is created are added to the ParentSeqRanges property of the 
        /// new sequence range.</param>
        /// <returns>The resultant Sequence range grouping.</returns>
        public SequenceRangeGrouping Subtract(SequenceRangeGrouping query,
            long minOverlap = 1,
            SubtractOutputType outputType = SubtractOutputType.NonOverlappingPiecesOfIntervals,
            bool isParentSeqRangesRequired = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            SequenceRangeGrouping result = new SequenceRangeGrouping();
            List<ISequenceRange> refSeqRanges = new List<ISequenceRange>();
            List<ISequenceRange> querySeqRanges = new List<ISequenceRange>();
            List<ISequenceRange> previousSeqRanges = new List<ISequenceRange>();
            SequenceRange range = null;

            // merge the query sequence ranges.
            IList<ISequenceRange> queryList = null;
            if (isParentSeqRangesRequired)
            {
                queryList = query.Flatten();
            }

            query = query.MergeOverlaps(0, isParentSeqRangesRequired);

            foreach (string id in groups.Keys)
            {
                refSeqRanges.Clear();
                querySeqRanges.Clear();

                refSeqRanges.AddRange(groups[id]);

                if (query.groups.ContainsKey(id))
                {
                    querySeqRanges.AddRange(query.groups[id]);
                    querySeqRanges.Sort();
                }

                if (querySeqRanges.Count > 0)
                {
                    foreach (ISequenceRange refRange in refSeqRanges)
                    {
                        previousSeqRanges.Clear();
                        IList<ISequenceRange> overlappingQueryRanges = GetOverlappingRenges(refRange,
                                                                                            querySeqRanges,
                                                                                            minOverlap);

                        if (overlappingQueryRanges == null || overlappingQueryRanges.Count == 0)
                        {
                            if (minOverlap > 0 || outputType == SubtractOutputType.NonOverlappingPiecesOfIntervals)
                            {
                                range = new SequenceRange(refRange.ID, refRange.Start, refRange.End);
                                CopyOfMetadata(range, refRange);

                                if (isParentSeqRangesRequired)
                                {
                                    AddParent(range, refRange);
                                }

                                result.Add(range);
                            }

                            continue;
                        }

                        // no need to proceed if only non overlapping intervels needed.
                        if (outputType == SubtractOutputType.IntervalsWithNoOverlap)
                        {
                            continue;
                        }

                        ISequenceRange previousOverlappingRange = null;
                        foreach (ISequenceRange queryRange in overlappingQueryRanges)
                        {
                            // in case of non overlapping pieces of intervals get the non overlapping 
                            // ranges from reference sequence range.
                            if (refRange.Start < queryRange.Start)
                            {
                                if (previousSeqRanges.Count > 0 && previousSeqRanges[0].Start < queryRange.Start)
                                {
                                    // if the previous overlapping range's start and end are equal then no need to change the metadataSeqRanges.
                                    if (previousOverlappingRange == null || previousOverlappingRange.Start != queryRange.Start && previousOverlappingRange.End != queryRange.End)
                                    {
                                        for (int i = previousSeqRanges.Count - 1; i >= 0; i--)
                                        {
                                            if (previousSeqRanges[i].End > queryRange.Start)
                                            {
                                                previousSeqRanges[i].End = queryRange.Start;
                                            }
                                            else if (previousSeqRanges[i].End < queryRange.Start)
                                            {
                                                previousSeqRanges.RemoveAt(i);
                                            }
                                        }
                                    }

                                    if (isParentSeqRangesRequired)
                                    {
                                        if (queryList.Contains(queryRange))
                                        {
                                            AddParent(previousSeqRanges[0], queryRange);
                                        }
                                        else
                                        {
                                            if (queryRange.ParentSeqRanges.Count > 0)
                                            {
                                                AddParent(previousSeqRanges[0], queryRange.ParentSeqRanges);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (previousOverlappingRange == null || previousOverlappingRange.Start != queryRange.Start && previousOverlappingRange.End != queryRange.End)
                                    {
                                        range = new SequenceRange(refRange.ID, refRange.Start, queryRange.Start);
                                        result.Add(range);
                                        CopyOfMetadata(range, refRange);

                                        if (isParentSeqRangesRequired)
                                        {
                                            AddParent(range, refRange);
                                        }
                                    }

                                    if (isParentSeqRangesRequired)
                                    {
                                        if (queryList.Contains(queryRange))
                                        {
                                            AddParent(range, queryRange);
                                        }
                                        else
                                        {
                                            if (queryRange.ParentSeqRanges.Count > 0)
                                            {
                                                AddParent(range, queryRange.ParentSeqRanges);
                                            }
                                        }
                                    }
                                }
                            }

                            if (queryRange.End < refRange.End)
                            {
                                if (previousOverlappingRange == null || previousOverlappingRange.Start != queryRange.Start && previousOverlappingRange.End != queryRange.End)
                                {
                                    range = new SequenceRange(refRange.ID, queryRange.End, refRange.End);
                                    CopyOfMetadata(range, refRange);

                                    result.Add(range);
                                    previousSeqRanges.Add(range);

                                    if (isParentSeqRangesRequired)
                                    {
                                        AddParent(range, refRange);
                                    }
                                }

                                if (isParentSeqRangesRequired)
                                {
                                    if (queryList.Contains(queryRange))
                                    {
                                        AddParent(range, queryRange);
                                    }
                                    else
                                    {
                                        if (queryRange.ParentSeqRanges.Count > 0)
                                        {
                                            AddParent(range, queryRange.ParentSeqRanges);
                                        }
                                    }
                                }
                            }

                            previousOverlappingRange = queryRange;
                        }
                    }
                }
                else
                {
                    foreach (SequenceRange refRange in refSeqRanges)
                    {
                        range = new SequenceRange(refRange.ID, refRange.Start, refRange.End);
                        CopyOfMetadata(range, refRange);
                        result.Add(range);

                        if (isParentSeqRangesRequired)
                        {
                            AddParent(range, refRange);
                        }
                    }
                }
            }

            return result;
        }
        #endregion

        /// <summary>
        /// Return overlapping ranges from the query sequence ranges for the specified minimal overlap. 
        /// Note that this method does not verifies the id.
        /// This method is used by intersect and subtract methods.
        /// </summary>
        /// <param name="refSeqRange">Reference seq range</param>
        /// <param name="querySeqRanges">Query sequence ranges</param>
        /// <param name="minimalOverlap">Minimum overlap required.</param>
        /// <returns>Overlapping Ranges from query ranges.</returns>
        private static List<ISequenceRange> GetOverlappingRenges(ISequenceRange refSeqRange, 
            List<ISequenceRange> querySeqRanges, 
            long minimalOverlap)
        {
            long totalOverlap = 0;

            if (minimalOverlap <= 0)
            {
                minimalOverlap = 1;
            }

            List<ISequenceRange> result = new List<ISequenceRange>();
            foreach (ISequenceRange queryRange in querySeqRanges)
            {
                if (queryRange.Start <= refSeqRange.End && queryRange.End >= refSeqRange.Start)
                {
                    totalOverlap = totalOverlap + Math.Min(queryRange.End, refSeqRange.End) - Math.Max(queryRange.Start, refSeqRange.Start);
                    result.Add(queryRange);
                }
            }

            if (result.Count > 0 && totalOverlap >= minimalOverlap)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Adds specified parentrange to the specified range's metadata.
        /// </summary>
        /// <param name="range">ISequenceRange instance to which the parentrange to be added.</param>
        /// <param name="parentRange">Parent range of the specified range.</param>
        private static void AddParent(ISequenceRange range, ISequenceRange parentRange)
        {
            if (!range.ParentSeqRanges.Contains(parentRange))
            {
                range.ParentSeqRanges.Add(parentRange);
            }
        }

        /// <summary>
        /// Adds specified parentranges to the specified range's metadata.
        /// </summary>
        /// <param name="range">ISequenceRange instance to which the parentrange to be added.</param>
        /// <param name="parentRanges">Parent ranges of the specified range.</param>
        private static void AddParent(ISequenceRange range, IEnumerable<ISequenceRange> parentRanges)
        {
            foreach (ISequenceRange parentRange in parentRanges)
            {
                AddParent(range, parentRange);
            }
        }

        /// <summary>
        /// Copies the metadata from specified fromRange to toRange.
        /// </summary>
        /// <param name="toRange">Range to which the metadata has to be copied.</param>
        /// <param name="fromRange">Range from which the metadata has to be copied.</param>
        private static void CopyOfMetadata(ISequenceRange toRange, ISequenceRange fromRange)
        {
            toRange.Metadata.Clear();
            if (fromRange.Metadata.Count > 0)
            {
                foreach (string key in fromRange.Metadata.Keys)
                {
                    object metadataItem = fromRange.Metadata[key];
                    toRange.Metadata[key] = metadataItem;
                }
            }
        }
    }

    /// <summary>
    /// This enum indicates type of output an intersect operation should return.
    /// </summary>
    public enum IntersectOutputType
    {
        /// <summary>
        /// OverlappingIntervals indicates that intersect operation should return 
        /// intervals from the reference that overlap with the query intervals. 
        /// This option only filters out intervals that do not overlap with the query intervals.
        /// </summary>
        OverlappingIntervals = 0,

        /// <summary>
        /// OverlappingPiecesOfIntervals indicates that intersect operation should return intervals that 
        /// indicate the exact base pair overlap between the reference intervals and 
        /// the query intervals. 
        /// </summary>
        OverlappingPiecesOfIntervals
    }

    /// <summary>
    /// This enum indicates type of output an subtract operation should return.
    /// </summary>
    public enum SubtractOutputType
    {
        /// <summary>
        /// IntervalsWithNoOverlap indicates that subtract operation should return  
        /// intervals from the reference intervals that do not overlap with the query intervals. 
        /// This option only filters out intervals that overlap with the query intervals.
        /// </summary>
        IntervalsWithNoOverlap = 0,

        /// <summary>
        /// NonOverlappingPiecesOfIntervals indicates that Subtract operation should return 
        /// intervals from the reference intervals that have the intervals from the query intervals removed. 
        /// Any overlapping base pairs are removed from the range of the interval.
        /// </summary>
        NonOverlappingPiecesOfIntervals
    }
}
