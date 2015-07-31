using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bio.Algorithms.Kmer
{
    /// <summary>
    /// Constructs k-mers from given input sequence(s).
    /// For each input sequence, k-mers are constructed by sliding 
    /// a frame of size kmerLength along the input sequence, 
    /// and extracting sub-sequence inside the frame.
    /// </summary>
    public class SequenceToKmerBuilder : IKmerBuilder
    {
        /// <summary>
        /// Builds k-mers from a list of given input sequences.
        /// For each sequence in input list, constructs a KmersOfSequence 
        /// corresponding to the sequence and associated k-mers.
        /// </summary>
        /// <param name="sequences">List of input sequences.</param>
        /// <param name="kmerLength">K-mer length.</param>
        /// <returns>List of KmersOfSequence instances.</returns>
        public static KmerIndexerDictionary BuildKmerDictionary(IList<ISequence> sequences, int kmerLength)
        {
            if (sequences == null)
            {
                throw new ArgumentNullException("sequences");
            }

            if (kmerLength <= 0)
            {
                throw new ArgumentException(Properties.Resource.KmerLengthShouldBePositive);
            }

            var kmerTasks = new Task<KmerPositionDictionary>[sequences.Count];
            
            for (int index = 0; index < sequences.Count; index++)
            {
                ISequence localSequence = sequences[index];
                kmerTasks[index] = Task<KmerPositionDictionary>.Factory.StartNew(
                    o => BuildKmerDictionary(localSequence, kmerLength), TaskCreationOptions.None);
            }

          
            IList<KmerIndexer> kmerIndex;
            List<KmerPositionDictionary> kmerPositionDictionaries = new List<KmerPositionDictionary>(kmerTasks.Length);

            int totalElements = 0;
            for (int index = 0; index < kmerTasks.Length; index++)
            {
                KmerPositionDictionary kmerPositionDictionary = kmerTasks[index].Result;
                kmerPositionDictionaries.Add(kmerPositionDictionary);
                totalElements += kmerPositionDictionary.Count;
            }

            KmerIndexerDictionary maps = new KmerIndexerDictionary(totalElements);
            for (int index = 0; index < kmerPositionDictionaries.Count; index++)
            {
                foreach (KeyValuePair<ISequence, IList<long>> value in kmerPositionDictionaries[index])
                {
                    if (maps.TryGetValue(value.Key, out kmerIndex) ||
                        maps.TryGetValue(value.Key.GetReverseComplementedSequence(), out kmerIndex))
                    {
                        kmerIndex.Add(new KmerIndexer(index, value.Value));
                    }
                    else
                    {
                        maps.Add(value.Key, new List<KmerIndexer> { new KmerIndexer(index, value.Value) });
                    }
                }
            }

            return maps;
        }

        /// <summary>
        /// For input sequence, constructs k-mers by sliding 
        /// a frame of size kmerLength along the input sequence.
        /// Track positions of occurance for each kmer in sequence.
        /// Constructs KmersOfSequence for sequence and associated k-mers.
        /// </summary>
        /// <param name="sequence">Input sequence.</param>
        /// <param name="kmerLength">K-mer length.</param>
        /// <returns>KmersOfSequence constructed from sequence and associated k-mers.</returns>
        public static KmerPositionDictionary BuildKmerDictionary(ISequence sequence, int kmerLength)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            if (kmerLength > sequence.Count)
            {
                throw new ArgumentException(Properties.Resource.KmerLengthIsTooLong);
            }

            // kmers maintains the map between k-mer strings to list of positions in sequence.
            KmerPositionDictionary kmers = new KmerPositionDictionary();

            // Sequence 'kmer' stores the k-mer in each window.
            // Construct each k-mer using range from sequence.
            for (long i = 0; i <= sequence.Count - kmerLength; ++i)
            {
                ISequence kmerString = sequence.GetSubSequence(i, kmerLength);
                if (kmers.ContainsKey(kmerString))
                {
                    kmers[kmerString].Add(i);
                }
                else
                {
                    kmers[kmerString] = new List<long>() { i };
                }
            }

            return kmers;
        }

        /// <summary>
        /// Gets the set of kmer strings that occur in given sequences.
        /// </summary>
        /// <param name="sequence">Source Sequence.</param>
        /// <param name="kmerLength">Kmer Length.</param>
        /// <returns>Set of kmer strings.</returns>
        public static IEnumerable<ISequence> GetKmerSequences(ISequence sequence, int kmerLength)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            if (kmerLength <= 0)
            {
                throw new ArgumentException(Properties.Resource.KmerLengthShouldBePositive);
            }

            if (kmerLength > sequence.Count)
            {
                throw new ArgumentException(Properties.Resource.KmerLengthIsTooLong);
            }

            IList<ISequence> kmers = new List<ISequence>();
            for (int i = 0; i <= sequence.Count - kmerLength; ++i)
            {
                ISequence kmer = sequence.GetSubSequence(i, kmerLength);
                kmers.Add(kmer);
            }

            return kmers;
        }

        /// <summary>
        /// Builds k-mers from a list of given input sequences.
        /// For each sequence in input list, constructs a KmersOfSequence 
        /// corresponding to the sequence and associated k-mers.
        /// </summary>
        /// <param name="sequences">List of input sequences.</param>
        /// <param name="kmerLength">K-mer length.</param>
        /// <returns>List of KmersOfSequence instances.</returns>
        public IEnumerable<KmersOfSequence> Build(IEnumerable<ISequence> sequences, int kmerLength)
        {
            if (kmerLength <= 0)
            {
                throw new ArgumentException(Properties.Resource.KmerLengthShouldBePositive);
            }

            if (sequences == null)
            {
                throw new ArgumentNullException("sequences");
            }

            Task<KmersOfSequence>[] kmerTasks = new Task<KmersOfSequence>[sequences.Count()];
            long ndx = 0;

            foreach (ISequence sequence in sequences)
            {
                ISequence localSequence = sequence;
                kmerTasks[ndx] = Task<KmersOfSequence>.Factory.StartNew(
                    o => this.Build(localSequence, kmerLength), TaskCreationOptions.None);
                ndx++;
            }

            return kmerTasks.Select(t => t.Result);
        }

        /// <summary>
        /// For input sequence, constructs k-mers by sliding 
        /// a frame of size kmerLength along the input sequence.
        /// Track positions of occurrence for each kmer in sequence.
        /// Constructs KmersOfSequence for sequence and associated k-mers.
        /// </summary>
        /// <param name="sequence">Input sequence.</param>
        /// <param name="kmerLength">K-mer length.</param>
        /// <returns>KmersOfSequence constructed from sequence and associated k-mers.</returns>
        public KmersOfSequence Build(ISequence sequence, int kmerLength)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException(Properties.Resource.SequenceCannotBeNull);
            }

            if (kmerLength <= 0)
            {
                throw new ArgumentException(Properties.Resource.KmerLengthShouldBePositive);
            }

            if (kmerLength > sequence.Count)
            {
                throw new ArgumentException(Properties.Resource.KmerLengthIsTooLong);
            }

            // kmers maintains the map between k-mer strings to list of positions in sequence.
            Dictionary<string, List<long>> kmers = new Dictionary<string, List<long>>();

            // Sequence 'kmer' stores the k-mer in each window.
            // Construct each k-mer using range from sequence.
            for (long i = 0; i <= sequence.Count - kmerLength; ++i)
            {
                string kmerString = new string(sequence.GetSubSequence(i, kmerLength).Select(a => (char)a).ToArray());
                if (kmers.ContainsKey(kmerString))
                {
                    kmers[kmerString].Add(i);
                }
                else
                {
                    kmers[kmerString] = new List<long>() { i };
                }
            }

            return new KmersOfSequence(
                sequence,
                kmerLength,
                new HashSet<KmersOfSequence.KmerPositions>(kmers.Values.Select(l => new KmersOfSequence.KmerPositions(l))));
        }
        
    }
}
