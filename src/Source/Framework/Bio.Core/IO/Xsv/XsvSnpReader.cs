using System;
using System.Collections;
using System.Globalization;
using System.IO;

namespace Bio.IO.Xsv
{
    /// <summary>
    /// This is used to read SNP items from a text reader available as 
    /// character separated values in a line. There is one SNP item per line and 
    /// each line has four character separated fields whose column numbers are identified: 
    /// a chromosome number, the Position, and 
    /// the two allele values that are valid for that SNP. 
    /// There may be additional columns too, but their values are ignored.
    /// Classes that extend from this can override the protected properties and virtual methods 
    /// to perform Snp file format specific transformations before returning the field values.
    /// 
    /// </summary>
    public class XsvSnpReader : XsvTextReader, ISnpReader
    {
        /// <summary>
        /// Field name enums for SNP fields in the file
        /// </summary>
        public enum FieldNames
        {
            /// <summary>
            /// The chromosome number field
            /// </summary>
            Chromosome,

            /// <summary>
            /// The position within chromosome number field
            /// </summary>
            Position,

            /// <summary>
            /// The symbol for allele one field
            /// </summary>
            AlleleOne,

            /// <summary>
            /// The symbol for allele two field
            /// </summary>
            AlleleTwo
        }

        /// <summary>
        /// If true, the chromosome numbers are assumed to be sorted when 
        /// SkipToChromosome* methods are called.
        /// </summary>
        public bool IsChromosomeSorted;

        /// <summary>
        /// If true, the chromosome numbers AND chromosome positions within them 
        /// are assumed to be sorted when SkipToChromosome* methods are called.
        /// </summary>
        public bool IsChromosomePositionSorted;

        /// <summary>
        /// the zero-based column number in a line which 
        /// corresponds to the chromosome field
        /// </summary>
        protected int ChromosomeColumn
        {
            get;
            set;
        }

        /// <summary>
        /// the zero-based column number in a line which 
        /// corresponds to the Snp position field
        /// </summary>
        protected int PositionColumn
        {
            get;
            set;
        }

        /// <summary>
        /// the zero-based column number in a line which 
        /// corresponds to the first allele field for the SNP
        /// </summary>
        protected int AlleleOneColumn
        {
            get;
            set;
        }

        /// <summary>
        /// the zero-based column number in a line which 
        /// corresponds to the second allele field for the SNP
        /// </summary>
        protected int AlleleTwoColumn
        {
            get;
            set;
        }

        /// <summary>
        /// The SNP item that has been parsed for the current line
        /// </summary>
        protected SnpItem CurrentSnpItem { get; set; }

        /// <summary>
        /// True if the current line contains the first line read from the reader
        /// </summary>
        protected bool IsFirstLine { get; set; }

        /// <summary>
        /// Creates a SNP reader from the given text reader that has character 
        /// separated values for Snpitems.
        /// </summary>
        /// 
        /// <param name="reader">Source text reader for the SNP lines</param>
        /// <param name="separators">Valid character separators between fields in the line 
        /// e.g. '\t', ',', etc.</param>
        /// <param name="ignoreWhiteSpace">If true, trims the white space around a field value</param>
        /// <param name="hasHeader">If true, treats the field values of the first line in 
        /// the text reader as a header with the names of the fields.</param>
        /// <param name="chromosomeColumn">The zero-based column number in a line that 
        /// corresponds to the chromosome field</param>
        /// <param name="positionColumn">The zero-based column number in a line that 
        /// corresponds to the Snp position field</param>
        /// <param name="alleleOneColumn">The zero-based column number in a line that 
        /// corresponds to the first allele</param>
        /// <param name="alleleTwoColumn">The zero-based column number in a line that 
        /// corresponds to the second allele</param>
        public XsvSnpReader (TextReader reader, char[] separators, bool ignoreWhiteSpace, bool hasHeader,
                         int chromosomeColumn, int positionColumn, int alleleOneColumn, int alleleTwoColumn) :
            base(reader, separators, ignoreWhiteSpace, hasHeader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // initialize columns
            ChromosomeColumn = chromosomeColumn;
            PositionColumn = positionColumn;
            AlleleOneColumn = alleleOneColumn;
            AlleleTwoColumn = alleleTwoColumn;

            this.IsFirstLine = true;
        }

        /// <summary>
        /// Go to the next line in the text reader, unless it is the first line in which case, 
        /// it has already been called in the constructor and we do not move
        /// the TextReader.
        /// </summary>
        /// <returns>True if we have a valid SnpItem in the next line moved to</returns>
        public bool MoveNext ()
        {
            if (!this.IsFirstLine)
            {
                GoToNextLine();
            }
            else
            {
                // skip...we've already read the first line
                this.IsFirstLine = false;
            }

            // parse the next SNP...if next line not present, set currentSnpitem to null and return false
            if (!HasLines)
            {
                this.CurrentSnpItem = null;
                return false;
            }

            try 
            {
                // we have a current Snp
                this.CurrentSnpItem = MakeSnpForCurrentLine();
            } 
            catch (Exception ex) 
            {
                throw new FormatException(Properties.Resource.Parser_InvalidFileFormat, ex);
            }
            return true;
        }

        /// <summary>
        /// This method is not implemented for XsvSnpReader. Use BufferedSnpReader() instead.
        /// </summary>
        public void Reset ()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///  Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>The element in the collection at the current position of the enumerator.</returns>
        public SnpItem Current
        {
            get
            {
                if (this.IsFirstLine)
                    return null; // Not called MoveNext() yet. So return null as its the default behaviour of Enumerators.

                return this.CurrentSnpItem;
            }
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        /// <returns>The current element in the collection.</returns>
        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

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
        public virtual bool SkipToChromosome (int chromosomeNumber)
        {
            // NOTE: same code as BufferedSnpReader.SkipToChromosome
            bool hasMoveNext = true;

            if (Current == null)
            {
                return false;
            }

            if (IsChromosomeSorted || IsChromosomePositionSorted) // chromosome number sorted
            {
                while (hasMoveNext && Current.Chromosome < chromosomeNumber)
                {
                    hasMoveNext = MoveNext();
                }
            }
            else // chromosome number NOT sorted
            {
                while (hasMoveNext && Current.Chromosome != chromosomeNumber)
                {
                    hasMoveNext = MoveNext();
                }
            }
            // if we hit the EOF before we locate the chromosome number, then we did not find the chromosome line
            return hasMoveNext && Current.Chromosome == chromosomeNumber;
        }

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
        public virtual bool SkipToChromosomePosition (int chromosomeNumber, int position)
        {
            // NOTE: same code as BufferedSnpReader.SkipToChromosomePosition
            bool hasMoveNext;
            if (IsChromosomeSorted || IsChromosomePositionSorted) // chromosome number sorted
            {
                hasMoveNext = SkipToChromosome(chromosomeNumber);
                if(!hasMoveNext) return false;

                if (IsChromosomePositionSorted) // chromosome position sorted
                {
                    while (hasMoveNext && Current.Chromosome == chromosomeNumber && Current.Position < position) 
                    {
                      hasMoveNext = MoveNext();
                    }
                    return hasMoveNext && Current.Chromosome == chromosomeNumber && Current.Position == position;
                }
                // else, chromosome position NOT sorted

                while (hasMoveNext && Current.Chromosome == chromosomeNumber && Current.Position != position) 
                {
                    hasMoveNext = MoveNext();
                }
                return hasMoveNext && Current.Chromosome == chromosomeNumber && Current.Position == position;                
            }

            // else, chromosome number nor position sorted            
            hasMoveNext = true;
            while (hasMoveNext && (Current.Chromosome != chromosomeNumber || Current.Position != position)) 
            {
                hasMoveNext = MoveNext();
            }
            return hasMoveNext && Current.Chromosome == chromosomeNumber && Current.Position == position;            
        }


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
        public virtual bool SkipToChromosomePosition (int position)
        {
            // NOTE: same code as BufferedSnpReader.SkipToChromosomePosition
            if (Current == null)
            {
                return false;
            }
            int chromosomeNumber = Current.Chromosome;
            return SkipToChromosomePosition(chromosomeNumber, position);
        }

        /// <summary>
        /// Returns the field value as a string for the given field name.
        /// </summary>
        /// <param name="column">field name enumerator to get the string value for</param>
        /// <returns>Returns the field value as a string for the given field enum.</returns>
        protected virtual string GetFieldValue (FieldNames column)
        {
            int col = GetColumnNumber(column);
            if (col < 0)
                return null;
            return col >= Fields.Length ? null : Fields[col];
        }

        /// <summary>
        /// Returns the column number for the given field name
        /// </summary>
        /// <param name="column">Snp field enumerator to get the column number for</param>
        /// <returns>Returns the column number for the given Snp field enum.</returns>
        private int GetColumnNumber (FieldNames column)
        {
            switch (column)
            {
                case FieldNames.Chromosome:
                    return ChromosomeColumn;
                case FieldNames.Position:
                    return PositionColumn;
                case FieldNames.AlleleOne:
                    return AlleleOneColumn;
                case FieldNames.AlleleTwo:
                    return AlleleTwoColumn;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// Makes a SNP item for the current line in the XSV reader.
        /// Since the XSV reader reads ahead, this is actually the next 
        /// SNP item for the enumerator.
        /// </summary>
        /// <returns>Creates a SnpItem for the current line in the XsvReader</returns>
        private SnpItem MakeSnpForCurrentLine ()
        {
            string allele1 = GetFieldValue(FieldNames.AlleleOne);
            string allele2 = GetFieldValue(FieldNames.AlleleTwo);

            return new SnpItem
            {
                Chromosome = (byte) ushort.Parse(
                        this.GetFieldValue(FieldNames.Chromosome),
                        CultureInfo.InvariantCulture),
                Position = int.Parse(
                        this.GetFieldValue(FieldNames.Position),
                        CultureInfo.InvariantCulture),
                AlleleOne = (!string.IsNullOrEmpty(allele1)) ? allele1[0] : char.MinValue,
                AlleleTwo = (!string.IsNullOrEmpty(allele2)) ? allele2[0] : char.MinValue
            };
        }
    }
}
