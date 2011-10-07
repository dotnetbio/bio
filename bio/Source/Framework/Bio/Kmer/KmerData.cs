using System;

namespace Bio.Algorithms.Kmer
{
    /// <summary>
    /// Holds the KmerData.
    /// </summary>
    public struct KmerData32 : IKmerData
    {
        /// <summary>
        /// Compressed value of the kmer that will be stored in the De-Bruijn Node.
        /// </summary>
        private ulong kmerData;

        /// <summary>
        /// Returns the decompressed value of the kmer from the De-Bruijn node.
        /// Note: use this method to get the original sequence symbols.
        /// </summary>
        /// <param name="kmerLength">Length of the kmer.</param>
        /// <param name="orientation">Orientation of connecting edge.</param>
        /// <returns>Decompressed value of the kmer.</returns>
        public byte[] GetOriginalSymbols(int kmerLength, bool orientation)
        {
            if (orientation)
            {
                return ConvertLongToSequence(this.kmerData, kmerLength);
            }
            else
            {
                return ConvertLongToSequence(GetReverseComplement(this.kmerData, kmerLength), kmerLength);
            }
        }

        /// <summary>
        /// Note: use this method to get the original sequence symbols.
        /// </summary>
        /// <param name="kmerLength">Length of the kmer.</param>
        /// <param name="orientation">Orientation of connecting edge.</param>
        /// <returns>Returns the reverse complement of the decompressed kmer.</returns>
        public byte[] GetReverseComplementOfOriginalSymbols(int kmerLength, bool orientation)
        {
            if (orientation)
            {
                return ConvertLongToSequence(GetReverseComplement(this.kmerData, kmerLength), kmerLength);
            }
            else
            {
                return ConvertLongToSequence(this.kmerData, kmerLength);
            }
        }

        /// <summary>
        /// Sets the kmer value from the specific sequence.
        /// </summary>
        /// <param name="sequence">Sequence who value is to be compressed.</param>
        /// <param name="from">Start postition from where the kmer to be extracted.</param>
        /// <param name="kmerLength">Length of the kmer.</param>
        public bool SetKmerData(ISequence sequence, long from, int kmerLength)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            ulong compressedKmer = 0;
            byte value;
            for (long index = from; index < from + kmerLength; index++)
            {
                switch (sequence[index])
                {
                    case 65: // 'A'
                    case 97: // 'a'
                        value = 0;
                        break;
                    case 67: // 'C'
                    case 99: // 'c'
                        value = 1;
                        break;
                    case 71: // 'G'
                    case 103: // 'g'
                        value = 2;
                        break;
                    case 84: // 'T'
                    case 116: // 't'
                        value = 3;
                        break;
                    default:
                        throw new ArgumentException("Character not supproted");
                }

                compressedKmer = (compressedKmer << 2) + value;
            }

            this.kmerData = compressedKmer;

            // Get the Reverse Complement value.
            ulong kmer = this.kmerData;
            ulong revComplementKey = 0;
            ulong bits = 0;
            checked
            {
                int kmerValueIndex = (kmerLength * 2);
                for (long index = 0; index < kmerValueIndex; index += 2)
                {
                    bits = kmer & 3;
                    kmer = kmer >> 2;

                    // Reversing the bits and adding to new long will generate reverse complement.
                    revComplementKey = (revComplementKey << 2) + ((~bits) & 3);
                }
            }

            bool forwardOrientation = this.kmerData <= revComplementKey;
            if (!forwardOrientation)
            {
                this.kmerData = revComplementKey;
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
            {
                throw new ArgumentNullException("sequence");
            }

            if (sequence.Length > kmerLength)
            {
                throw new ArgumentException("sub-sequence length cannot be more than the kmer length");
            }

            this.kmerData = ConvertSequenceToLong(sequence);
        }

        /// <summary>
        /// Checks whether the kmer value is palindrome or not.
        /// </summary>
        /// <returns>True if the kmer value is palindrome else false.</returns>
        public bool IsPalindrome(int kmerLength)
        {
            return (this.kmerData.Equals(GetReverseComplement(this.kmerData, kmerLength)));
        }

        /// <summary>
        /// Compares this instance to a specified instance of IKmerData and returns an indication of their relative values.
        /// </summary>
        /// <param name="kmer">Instance of the IKmerData to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance. Zero This
        /// instance is equal to value. Greater than zero This instance is greater than
        /// value.
        /// </returns>
        public int CompareTo(IKmerData kmer)
        {
            if (kmer == null)
            {
                throw new ArgumentNullException("kmer");
            }

            ulong compValue = ((KmerData32)kmer).kmerData;
            if (this.kmerData == compValue)
            {
                return 0;
            }
            else if (this.kmerData < compValue)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Compares this instance to a specified instance of object and returns an indication of their relative values.
        /// </summary>
        /// <param name="obj">Instance of the object to compare.</param>
        /// <returns>
        ///  A signed number indicating the relative values of this instance. Zero This
        ///  instance is equal to value. Greater than zero This instance is greater than
        ///  value.
        /// </returns>
        public int CompareTo(object obj)
        {
            KmerData32 kmer = (KmerData32)obj;
            return this.kmerData.CompareTo(kmer.kmerData);
        }

        /// <summary>
        /// Returns the first symbol of the sequence.
        /// </summary>
        /// <returns>Returns the first symbol from the decompressed kmer value.</returns>
        public byte GetFirstSymbol(int kmerLength, bool orientation)
        {
            if (orientation)
            {
                return ConvertLongToSequence(this.kmerData, kmerLength)[0];
            }
            else
            {
                return ConvertLongToSequence(GetReverseComplement(this.kmerData, kmerLength), kmerLength)[0];
            }
        }

        /// <summary>
        /// Returns the last symbol of the sequence.
        /// </summary>
        /// <returns>Returns the last symbol from the decompressed kmer value.</returns>
        public byte GetLastSymbol(int kmerLength, bool orientation)
        {
            byte[] seq;

            if (orientation)
            {
                seq = ConvertLongToSequence(this.kmerData, kmerLength);
            }
            else
            {
                seq = ConvertLongToSequence(GetReverseComplement(this.kmerData, kmerLength), kmerLength);
            }

            return seq[seq.Length - 1];
        }

        /// <summary>
        /// Returns the decompressed value of the kmer.
        /// </summary>
        /// <param name="kmerLength">Length of the kmer.</param>
        /// <returns>Decompressed value of the kmer.</returns>
        public byte[] GetKmerData(int kmerLength)
        {
            return ConvertLongToSequence(this.kmerData, kmerLength);
        }

        /// <summary>
        /// Returns the reverse complement of the kmer value.
        /// </summary>
        /// <param name="kmerLength">Length of the kmer.</param>
        /// <returns>Returns the reverse complement of the kmer.</returns>
        public byte[] GetReverseComplementOfKmerData(int kmerLength)
        {
            return ConvertLongToSequence(GetReverseComplement(this.kmerData, kmerLength), kmerLength);
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
            ulong bits = 0;
            for (int index = 0; index < kmerLength * 2; index += 2)
            {
                bits = kmer & 3;
                kmer = kmer >> 2;
                
                // Reversing the bits and adding to new long will generate reverse complement.
                reverse = (reverse << 2) + ((~bits) & 3);
            }

            return reverse;
        }

        /// <summary>
        /// Converts sequence to long.
        /// If kmer length is less than or equal to 32, we can fit into a usigned 64 bit long.
        /// </summary>
        /// <param name="sequence">Kmer sequence.</param>
        /// <returns>Compressed kmer.</returns>
        private static ulong ConvertSequenceToLong(byte[] sequence)
        {
            ulong compressedKmer = 0;
            byte value;

            // Push each sequence alphabet in its binary represenatation into an long.
            foreach (byte seq in sequence)
            {
                switch (seq)
                {
                    case 65: // 'A'
                    case 97: // 'a'
                        value = 0;
                        break;
                    case 67: // 'C'
                    case 99: // 'c'
                        value = 1;
                        break;
                    case 71: // 'G'
                    case 103: // 'g'
                        value = 2;
                        break;
                    case 84: // 'T'
                    case 116: // 't'
                        value = 3;
                        break;
                    default:
                        throw new ArgumentException("Character not supproted");
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
            byte[] seq = new byte[kmerLength];

            // Converting bits to sequence and adding to readonly false sequence. 
            for (int index = 0; index < kmerLength; index++)
            {
                switch (compressedKmer & 3)
                {
                    case 0:
                        {
                            seq[(kmerLength - 1) - index] = Alphabets.DNA.A;
                            break;
                        }

                    case 1:
                        {
                            seq[(kmerLength - 1) - index] = Alphabets.DNA.C;
                            break;
                        }

                    case 2:
                        {
                            seq[(kmerLength - 1) - index] = Alphabets.DNA.G;
                            break;
                        }

                    case 3:
                        {
                            seq[(kmerLength - 1) - index] = Alphabets.DNA.T;
                            break;
                        }

                    default:
                        {
                            throw new ArgumentException("Alphabet not supported");
                        }
                }

                compressedKmer = compressedKmer >> 2;
            }

            return seq;
        }
    }
}
