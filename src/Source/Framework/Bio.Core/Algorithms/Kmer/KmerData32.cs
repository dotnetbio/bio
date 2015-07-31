using System;
using System.Collections.Generic;

namespace Bio.Algorithms.Kmer
{
    /// <summary>
    /// Holds the KmerData.
    /// </summary>
    public struct KmerData32
    {
        /// <summary>
        /// Value to encode an A with as 2 bits
        /// </summary>
        public const ulong DNA_A_VALUE = 0;
        /// <summary>
        /// Value to encode a C with as 2 bits
        /// </summary>
        public const ulong DNA_C_VALUE = 1;
        /// <summary>
        /// Value to encode a G with as 2 bits
        /// </summary>
        public const ulong DNA_G_VALUE = 2;
        /// <summary>
        /// Value to encode a T with as 2 bits
        /// </summary>
        public const ulong DNA_T_VALUE = 3;

        /// <summary>
        /// Maximum allowed value for a kmer
        /// </summary>
        public const int MAX_KMER_LENGTH = 31;

        /// <summary>
        /// Minimum allowed value for a kmer
        /// </summary>
        public const int MIN_KMER_LENGTH = 12;

        /// <summary>
        /// Compressed value of the kmer that will be stored in the De-Bruijn Node.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public ulong KmerData { get;  set; }

        /// <summary>
        /// Returns the decompressed value of the kmer from the De-Bruijn node.
        /// Note: use this method to get the original sequence symbols.
        /// </summary>
        /// <param name="kmerLength">Length of the kmer.</param>
        /// <returns>Decompressed value of the kmer.</returns>
        public byte[] GetOriginalSymbols(int kmerLength)
        {
            return ConvertLongToSequence(this.KmerData, kmerLength);
        }

        /// <summary>
        /// Note: use this method to get the original sequence symbols.
        /// </summary>
        /// <param name="kmerLength">Length of the kmer.</param>
        /// <returns>Returns the reverse complement of the decompressed kmer.</returns>
        public byte[] GetReverseComplementOfOriginalSymbols(int kmerLength)
        {
            return ConvertLongToSequence(GetReverseComplement(this.KmerData, kmerLength), kmerLength);
        }

        /// <summary>
        /// Sets the kmer value from the specific sequence.
        /// </summary>
        /// <param name="sequence">Sequence who value is to be compressed.</param>
        /// <param name="from">Start position from where the kmer to be extracted.</param>
        /// <param name="kmerLength">Length of the kmer.</param>
        public bool SetKmerData(ISequence sequence, long from, int kmerLength)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            ulong compressedKmer = 0;
            for (long index = from; index < from + kmerLength; index++)
            {
                ulong value;
                switch (sequence[index])
                {
                    case 65: // 'A'
                    case 97: // 'a'
                        value = DNA_A_VALUE;
                        break;
                    case 67: // 'C'
                    case 99: // 'c'
                        value = DNA_C_VALUE;
                        break;
                    case 71: // 'G'
                    case 103: // 'g'
                        value = DNA_G_VALUE;
                        break;
                    case 84: // 'T'
                    case 116: // 't'
                        value = DNA_T_VALUE;
                        break;
                    default:
                        throw new ArgumentException("Character not supported");
                }
                compressedKmer = (compressedKmer << 2) + value;
            }

            return this.SetKmerData(compressedKmer, kmerLength);
        }
        /// <summary>
        /// Iterates through a sequence producing all possible kmers in it.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="kmerLength"></param>
        /// <returns></returns>
        public static KmerData32[] GetKmers(ISequence sequence, int kmerLength)
        {
            if (sequence == null)
                throw new ArgumentNullException("sequence");

            long count = sequence.Count;
            if (kmerLength > count || kmerLength > MAX_KMER_LENGTH)
                throw new ArgumentException("Invalid k-mer length - cannot exceed " + MAX_KMER_LENGTH, "kmerLength");

            KmerData32[] kmers = new KmerData32[count - kmerLength + 1];
            
            //First to make a mask to hide higher bits as we move things over
            ulong mask = ulong.MaxValue;//should be all bits in ulong
            mask <<= (kmerLength * 2);//move mask over filling in regions to keep with zeros
            mask = ~mask;//then flip the bits to get the mask
            
            ulong compressedKmer = 0;
            for (long i = 0; i < count; ++i)
            {
                ulong value;
                switch (sequence[i])
                {
                    case 65: // 'A'
                    case 97: // 'a'
                        value = DNA_A_VALUE;
                        break;
                    case 67: // 'C'
                    case 99: // 'c'
                        value = DNA_C_VALUE;
                        break;
                    case 71: // 'G'
                    case 103: // 'g'
                        value = DNA_G_VALUE;
                        break;
                    case 84: // 'T'
                    case 116: // 't'
                        value = DNA_T_VALUE;
                        break;
                    default:
                        throw new ArgumentException("Character not supported");
                }
                compressedKmer = (compressedKmer << 2) + value;
                if (i >= (kmerLength - 1))
                {
                    //hide top bits
                    compressedKmer = compressedKmer & mask;
                    //get reverse compliment
                    KmerData32 nk = new KmerData32();
                    nk.SetKmerData(compressedKmer, kmerLength);
                    kmers[i - kmerLength + 1] = nk;
                }
            }
            return kmers;
        }
        /// <summary>
        /// Sets the kmer value from the specific sequence.
        /// </summary>
        /// <param name="encodedKmer">The kmer to encode, will be reverse complimented if need be.</param>
        /// <param name="kmerLength">Length of the kmer.</param>
        public bool SetKmerData(ulong encodedKmer, int kmerLength)
        {
            this.KmerData = encodedKmer;
            // Get the Reverse Complement value. This is a duplication of the code in a method of this class
            // perhaps put here to ensure the method is inlined?
            ulong revComplementKey = GetReverseComplement(encodedKmer, kmerLength);
            //equal values should be impossible with odd length k-mer
            bool forwardOrientation = this.KmerData >= revComplementKey;
            if (!forwardOrientation)
            {
                this.KmerData = revComplementKey;
            }
            return forwardOrientation;
        }

        /// <summary>
        /// Sets the kmer value from the specific sequence.
        /// Note: Used in generating the links.
        ///  Do not use this method to add kmerData to the tree.
        /// </summary>
        /// <param name="sequence">Sequence who value is to be compressed.</param>
        /// <param name="kmerLength">Length of the kmer.</param>
        public void SetKmerData(byte[] sequence, int kmerLength)
        {
            if (sequence == null)
                throw new ArgumentNullException("sequence");

            if (sequence.Length > kmerLength)
                throw new ArgumentException("sub-sequence length cannot be more than the kmer length");

            this.KmerData = ConvertSequenceToLong(sequence);
        }

        /// <summary>
        /// Checks whether the kmer value is palindrome or not.
        /// </summary>
        /// <returns>True if the kmer value is palindrome else false.</returns>
        public bool IsPalindrome(int kmerLength)
        {
            return (this.KmerData.Equals(GetReverseComplement(this.KmerData, kmerLength)));
        }

        /// <summary>
        /// Compares this instance to a specified instance of IKmerData and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">Instance of the IKmerData to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance. Zero This
        /// instance is equal to value. Greater than zero This instance is greater than
        /// value.
        /// </returns>
        public int CompareTo(KmerData32 other)
        {
            return this.KmerData.CompareTo(other.KmerData);
        }

        /// <summary>
        /// Compares this instance to a specified instance of object and returns an indication of their relative values.
        /// </summary>
        /// <param name="value">Instance of the object to compare.</param>
        /// <returns>
        ///  A signed number indicating the relative values of this instance. Zero This
        ///  instance is equal to value. Greater than zero This instance is greater than
        ///  value.
        /// </returns>
        public int CompareTo(object value)
        {
            KmerData32 kmer = (KmerData32)value;
            return this.KmerData.CompareTo(kmer.KmerData);
        }

        /// <summary>
        /// Returns the first symbol of the sequence.
        /// </summary>
        /// <returns>Returns the first symbol from the decompressed kmer value.</returns>
        public byte GetFirstSymbol(int kmerLength, bool orientation)
        {
            return orientation ? ConvertLongToSequence(this.KmerData, kmerLength)[0] : ConvertLongToSequence(GetReverseComplement(this.KmerData, kmerLength), kmerLength)[0];
        }

        /// <summary>
        /// Returns the last symbol of the sequence.
        /// </summary>
        /// <returns>Returns the last symbol from the decompressed kmer value.</returns>
        public byte GetLastSymbol(int kmerLength, bool orientation)
        {
            byte[] seq = ConvertLongToSequence(orientation ? this.KmerData : GetReverseComplement(this.KmerData, kmerLength), kmerLength);
            return seq[seq.Length - 1];
        }

        /// <summary>
        /// Returns the decompressed value of the kmer.
        /// </summary>
        /// <param name="kmerLength">Length of the kmer.</param>
        /// <returns>Decompressed value of the kmer.</returns>
        public byte[] GetKmerData(int kmerLength)
        {
            return ConvertLongToSequence(this.KmerData, kmerLength);
        }

        /// <summary>
        /// Returns the reverse complement of the kmer value.
        /// </summary>
        /// <param name="kmerLength">Length of the kmer.</param>
        /// <returns>Returns the reverse complement of the kmer.</returns>
        public byte[] GetReverseComplementOfKmerData(int kmerLength)
        {
            return ConvertLongToSequence(GetReverseComplement(this.KmerData, kmerLength), kmerLength);
        }

        /// <summary>
        /// Generates reverse complement for long compressed kmer.
        /// </summary>
        /// <param name="kmer">Compressed kmer.</param>
        /// <param name="kmerLength">Kmer length.</param>
        /// <returns>Long representation of reverse complement kmer.</returns>
        private static ulong GetReverseComplement(ulong kmer, int kmerLength)
        {
            ulong reverse = 0;
            checked
            {
                for (int index = 0; index < kmerLength * 2; index += 2)
                {
                    ulong bits = kmer & 3;
                    kmer = kmer >> 2;
                    // Reversing the bits and adding to new long will generate reverse complement.
                    reverse = (reverse << 2) + ((~bits) & 3);
                }
            }
            return reverse;
        }

        /// <summary>
        /// Converts sequence to long.
        /// If kmer length is less than or equal to 32, we can fit into a usigned 64 bit long.
        /// </summary>
        /// <param name="sequence">Kmer sequence.</param>
        /// <returns>Compressed kmer.</returns>
        private static ulong ConvertSequenceToLong(IEnumerable<byte> sequence)
        {
            ulong compressedKmer = 0;

            // Push each sequence alphabet in its binary represenatation into an long.
            foreach (byte seq in sequence)
            {
                ulong value;
                switch (seq)
                {
                    case 65: // 'A'
                    case 97: // 'a'
                        value = DNA_A_VALUE;
                        break;
                    case 67: // 'C'
                    case 99: // 'c'
                        value = DNA_C_VALUE;
                        break;
                    case 71: // 'G'
                    case 103: // 'g'
                        value = DNA_G_VALUE;
                        break;
                    case 84: // 'T'
                    case 116: // 't'
                        value = DNA_T_VALUE;
                        break;
                    default:
                        throw new ArgumentException("Character not supported");
                }

                compressedKmer = (compressedKmer << 2) + value;
            }

            return compressedKmer;
        }

        /// <summary>
        /// Decompress kmer as long to sequence.
        /// </summary>
        /// <param name="compressedKmer">Compressed kmer.</param>
        /// <param name="kmerLength">Kmer Length.</param>
        /// <returns>Kmer sequence.</returns>
        private static byte[] ConvertLongToSequence(ulong compressedKmer, int kmerLength)
        {
            var seq = new byte[kmerLength];

            // Converting bits to sequence and adding to readonly false sequence. 
            for (int index = 0; index < kmerLength; index++)
            {
                switch (compressedKmer & 3)
                {
                    case 0:
                        seq[(kmerLength - 1) - index] = Alphabets.DNA.A;
                        break;
                    case 1:
                        seq[(kmerLength - 1) - index] = Alphabets.DNA.C;
                        break;
                    case 2:
                        seq[(kmerLength - 1) - index] = Alphabets.DNA.G;
                        break;
                    case 3:
                        seq[(kmerLength - 1) - index] = Alphabets.DNA.T;
                        break;
                    default:
                        throw new ArgumentException("Alphabet not supported");
                }

                compressedKmer = compressedKmer >> 2;
            }

            return seq;
        }
    }
}
