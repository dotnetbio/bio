using System;
using System.Globalization;
using System.IO;
using Bio.SimilarityMatrices.Resources;
using Bio.Util.Logging;
using System.Collections.Generic;

namespace Bio.SimilarityMatrices
{
    /// <summary>
    /// Representation of a matrix that contains similarity scores for every 
    /// pair of symbols in an alphabet. BLOSUM and PAM are well-known examples.
    /// </summary>
    public class SimilarityMatrix
    {
        /// <summary>
        /// Array containing the scores for each pair of symbols.
        /// The indices of the array are byte values of alphabet symbols.
        /// </summary>
        private int[][] similarityMatrix;

        /// <summary>
        /// 
        /// </summary>
        protected HashSet<byte> supportedAlphabets = new HashSet<byte>();

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SimilarityMatrix class
        /// Constructs one of the standard similarity matrices.
        /// </summary>
        /// <param name="matrixId">
        /// Matrix to load, BLOSUM and PAM currently supported.
        /// The enum StandardSimilarityMatrices contains list of available matrices.
        /// </param>
        public SimilarityMatrix(StandardSimilarityMatrix matrixId)
        {
            // MoleculeType.Protein for BLOSUM and PAM series supported matrices
            IAlphabet moleculeType = Alphabets.Protein;
            string matrixText = null;

            switch (matrixId)
            {
                case StandardSimilarityMatrix.Blosum45:
                    matrixText = SimilarityMatrixResources.Blosum45;
                    break;
                case StandardSimilarityMatrix.Blosum50:
                    matrixText = SimilarityMatrixResources.Blosum50;
                    break;
                case StandardSimilarityMatrix.Blosum62:
                    matrixText = SimilarityMatrixResources.Blosum62;
                    break;
                case StandardSimilarityMatrix.Blosum80:
                    matrixText = SimilarityMatrixResources.Blosum80;
                    break;
                case StandardSimilarityMatrix.Blosum90:
                    matrixText = SimilarityMatrixResources.Blosum90;
                    break;
                case StandardSimilarityMatrix.Pam250:
                    matrixText = SimilarityMatrixResources.Pam250;
                    break;
                case StandardSimilarityMatrix.Pam30:
                    matrixText = SimilarityMatrixResources.Pam30;
                    break;
                case StandardSimilarityMatrix.Pam70:
                    matrixText = SimilarityMatrixResources.Pam70;
                    break;
                case StandardSimilarityMatrix.AmbiguousDna:
                    matrixText = SimilarityMatrixResources.AmbiguousDna;
                    moleculeType = Alphabets.DNA;
                    break;
                case StandardSimilarityMatrix.AmbiguousRna:
                    matrixText = SimilarityMatrixResources.AmbiguousRna;
                    moleculeType = Alphabets.RNA;
                    break;
                case StandardSimilarityMatrix.DiagonalScoreMatrix:
                    matrixText = SimilarityMatrixResources.DiagonalScoreMatrix;
                    break;
                case StandardSimilarityMatrix.EDnaFull:
                    matrixText = SimilarityMatrixResources.EDNAFull;
                    moleculeType = Alphabets.AmbiguousDNA;
                    break;
            }

            using (TextReader reader = new StringReader(matrixText))
            {
                LoadFromStream(reader, moleculeType);
            }
        }

        /// <remarks>
        /// File or stream format:
        /// There are two slightly different formats.
        /// <para>
        /// For custom similarity matrices:
        /// First line is descriptive name, will be stored as a string.
        /// Second line define the molecule type.  Must be "DNA", "RNA", or "Protein".
        /// Third line is alphabet (symbol map). It contains n characters and optional white space.
        /// Following lines are values for each row of matrix
        /// Must be n numbers per line, n lines
        /// </para>
        /// <para>
        /// In some cases the molecule type is implicit in the matrix.  This is true for the
        /// supported standard matrices (BLOSUM and PAM series at this point), and for the standard
        /// encodings IUPACna, NCBIA2na, NCBI2na, NCBI4na, and NCBIeaa.
        /// For these cases:
        /// First line is descriptive name, will be stored as a string.
        /// Second line is the encoding name for the standard encodings (IUPACna, NCBIA2na, NCBI2na, NCBI4na, or NCBIeaa)
        ///     or the alphabet (symbol map) for the standard matrices.
        /// Following lines are values for each row of matrix
        /// Must be n numbers per line, n lines; or in the case of the supported encoding, sufficient
        /// entries to handle all possible indices (0 through max index value).
        /// </para>
        /// </remarks>
        /// <summary>
        /// Initializes a new instance of the SimilarityMatrix class.
        /// Constructs SimilarityMatrix from an input stream.
        /// </summary>
        /// <param name="reader">Text reader associated with the input sequence stream.</param>
        public SimilarityMatrix(TextReader reader)
        {
            LoadFromStream(reader, null);
        }

        /// <summary>
        /// Initializes a new instance of the SimilarityMatrix class
        /// Constructs SimilarityMatrix from an input file.
        /// </summary>
        /// <param name="fileName">File name of input sequence.</param>
        public SimilarityMatrix(string fileName)
        {
            using (TextReader reader = new StreamReader(fileName))
            {
                LoadFromStream(reader, null);
            }
        }

        /// <summary>
        /// Initializes a new instance of the SimilarityMatrix class.
        /// </summary>
        protected SimilarityMatrix()
        {
            // This constructor is solely for enabling inheritance
        }

        #endregion

        #region Nested enums

        /// <summary>
        /// List of available standard similarity matrices.
        /// </summary>
        /// <remarks>
        /// BLOSUM matrices reference:
        /// S Henikoff and J G Henikoff,
        /// "Amino acid substitution matrices from protein blocks."
        /// Proc Natl Acad Sci U S A. 1992 November 15; 89(22): 10915–10919.  PMCID: PMC50453
        /// <para>
        /// Available at:
        /// <![CDATA[http://www.pubmedcentral.nih.gov/articlerender.fcgi?tool=EBI&pubmedid=1438297]]>
        /// </para>
        /// <para>
        /// PAM matrices reference:
        /// Dayhoff, M.O., Schwartz, R. and Orcutt, B.C. (1978), 
        /// "A model of Evolutionary Change in Proteins", 
        /// Atlas of protein sequence and structure (volume 5, supplement 3 ed.), 
        /// Nat. Biomed. Res. Found., p. 345-358, ISBN 0912466073.
        /// </para>
        /// </remarks>
        public enum StandardSimilarityMatrix
        {
            /// <summary>
            /// BLOSUM45 Similarity Matrix.
            /// </summary>
            Blosum45,

            /// <summary>
            /// BLOSUM50 Similarity Matrix.
            /// </summary>
            Blosum50,

            /// <summary>
            /// BLOSUM62 Similarity Matrix.
            /// </summary>
            Blosum62,

            /// <summary>
            /// BLOSUM80 Similarity Matrix.
            /// </summary>
            Blosum80,

            /// <summary>
            /// BLOSUM90 Similarity Matrix.
            /// </summary>
            Blosum90,

            /// <summary>
            /// PAM250 Similarity Matrix.
            /// </summary>
            Pam250,

            /// <summary>
            /// PAM30 Similarity Matrix.
            /// </summary>
            Pam30,

            /// <summary>
            /// PAM70 Similarity Matrix.
            /// </summary>
            Pam70,

            /// <summary>
            /// Simple DNA Similarity Matrix.
            /// </summary>
            AmbiguousDna,

            /// <summary>
            /// RNA with ambiguous.
            /// </summary>
            AmbiguousRna,

            /// <summary>
            /// Diagonal matrix.
            /// </summary>
            DiagonalScoreMatrix,

            /// <summary>
            /// EDNAFull Similarity Matrix.
            /// </summary>
            EDnaFull,
        }

        #endregion

        #region Properties
        /// <summary> 
        /// Gets or sets descriptive name of the particular SimilarityMatrix being used. 
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets similarity matrix values in a 2-D integer array.
        /// </summary>
        public int[][] Matrix
        {
            get
            {
                return similarityMatrix;
            }

            protected set
            {
                similarityMatrix = value;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Returns value of matrix at [row, col].
        /// </summary>
        /// <param name="row">
        /// Row number. This is same as byte value
        /// corresponding to sequence symbol on the row.
        /// </param>
        /// <param name="col">
        /// Column number. This is same as byte value
        /// corresponding to sequence symbol on the column.
        /// </param>
        /// <returns>Score value of matrix at [row, col].</returns>
        public virtual int this[int row, int col]
        {
            get
            {
                return similarityMatrix[row][col];
            }
        }

        /// <summary>
        /// Reads similarity matrix from a stream.  File (or stream) format defined
        /// above with constructors to create SimilarityMatrix from stream or file.
        /// </summary>
        /// <param name="reader">Text reader associated with the input sequence stream.</param>
        /// <param name="moleculeType">Molecule type supported by SimilarityMatrix.</param>
        private void LoadFromStream(TextReader reader, IAlphabet moleculeType)
        {
            char[] delimiters = { '\t', ' ', ',' }; // basic white space, comma, can add more items later if necessary.

            Name = reader.ReadLine();
            if (String.IsNullOrEmpty(Name))
            {
                string message = Properties.Resource.SimilarityMatrix_NameMissing;
                Trace.Report(message);
                throw new InvalidDataException(message);
            }

            string line = reader.ReadLine();
            if (String.IsNullOrEmpty(line))
            {
                string message = Properties.Resource.SimilarityMatrix_SecondLineMissing;
                Trace.Report(message);
                throw new InvalidDataException(message);
            }

            // If the second line is Protein, DNA or RNA, we can set molecule type here and will have to read the alphabet from the third line.
            string secondLine = line.ToUpper(CultureInfo.InvariantCulture).Trim();
            string alphabetLine;

            if (moleculeType != null)
            {
                alphabetLine = secondLine;
            }
            else
            {
                // Find molecule type from second line
                if (!(secondLine == "DNA" || secondLine == "RNA" || secondLine == "PROTEIN"))
                {
                    string message = String.Format(
                            CultureInfo.CurrentCulture,
                            Properties.Resource.SimilarityMatrix_InvalidMoleculeType,
                            secondLine);
                    Trace.Report(message);
                    throw new InvalidDataException(message);
                }

                // Third line will be the alphabet...
                alphabetLine = reader.ReadLine().ToUpper(CultureInfo.InvariantCulture).Trim();
            }

            // We have read the two or three line header, including the alphabet if required.
            int symbolCount = 0;  // Number of symbols in alphabet map.

            // We need to parse the alphabet line.
            string[] alphabetsParsed = alphabetLine.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            byte[] uppercaseByteValues = new byte[alphabetsParsed.Length];
            byte[] lowercaseByteValues = new byte[alphabetsParsed.Length];
            foreach (string s in alphabetsParsed)
            {
                uppercaseByteValues[symbolCount] = (byte)s[0];
                lowercaseByteValues[symbolCount] = (byte)s.ToLowerInvariant()[0];
                supportedAlphabets.Add((byte)s[0]);
                supportedAlphabets.Add((byte)s.ToLowerInvariant()[0]);
                symbolCount++;
            }

            // Matrix size is kept as [byte.MaxValue,byte.MaxValue] and avoiding any possible optimizations
            // for the sake of performance.
            int rowCount = byte.MaxValue;
            int columnCount = byte.MaxValue;

            int[][] localSimilarityMatrix = new int[rowCount][];
            for (int x = 0; x < rowCount; x++)
            {
                localSimilarityMatrix[x] = new int[columnCount];
            }

            int row, col; // row and col indices.
            for (row = 0; row < symbolCount; row++)
            {
                line = reader.ReadLine();
                if (line == null || string.IsNullOrWhiteSpace(line))
                {
                    string message = Properties.Resource.SimilarityMatrix_FewerMatrixLines;
                    Trace.Report(message);
                    throw new InvalidDataException(message);
                }

                string[] rowValues = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                for (col = 0; col < symbolCount; col++)
                {
                    try
                    {
                        // Store scores at two locations
                        // One at the byte value of the uppercase alphabet
                        // Another at the byte value of the lowercase representation of the same alphabet
                        // This data duplication is for getting performance in further SM lookups.
                        localSimilarityMatrix[uppercaseByteValues[row]][uppercaseByteValues[col]] = Convert.ToInt32(rowValues[col], CultureInfo.InvariantCulture);
                        localSimilarityMatrix[lowercaseByteValues[row]][lowercaseByteValues[col]] = Convert.ToInt32(rowValues[col], CultureInfo.InvariantCulture);

                        localSimilarityMatrix[uppercaseByteValues[row]][lowercaseByteValues[col]] = Convert.ToInt32(rowValues[col], CultureInfo.InvariantCulture);
                        localSimilarityMatrix[lowercaseByteValues[row]][uppercaseByteValues[col]] = Convert.ToInt32(rowValues[col], CultureInfo.InvariantCulture);

                        // Populate TopLeft area of similarity matrix - Currently for PAMSAM
                        localSimilarityMatrix[row][col] = Convert.ToInt32(rowValues[col], CultureInfo.InvariantCulture);
                    }
                    catch (FormatException e)
                    {
                        string message = String.Format(
                                CultureInfo.CurrentCulture,
                                Properties.Resource.SimilarityMatrix_BadOrMissingValue,
                                line,
                                e.Message);
                        Trace.Report(message);
                        throw new InvalidDataException(message);
                    }
                }
            }

            this.similarityMatrix = localSimilarityMatrix;
        }

        /// <summary>
        /// Confirms that there is a symbol in the similarity matrix for every
        /// symbol in the sequence.
        /// </summary>
        /// <param name="sequence">Sequence to validate.</param>
        /// <returns>true if sequence is valid.</returns>
        public bool ValidateSequence(ISequence sequence)
        {
            foreach (byte item in sequence)
            {
                if (!supportedAlphabets.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
