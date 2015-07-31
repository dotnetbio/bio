namespace Bio.SimilarityMatrices
{
    /// <summary>
    /// Diagonal similarity matrix is a special case and needs its own class.
    /// It does not have an actual matrix, instead using a test "if (col == row)" and
    /// returning the diagonal value if true, and the off diagonal value if false.
    /// </summary>
    public class DiagonalSimilarityMatrix : SimilarityMatrix
    {
        /// <summary>
        /// Score value at diagonals. To be used when (col == row).
        /// </summary>
        private int diagonalValue;

        /// <summary>
        /// Score value off diagonals. To be used when (col != row).
        /// </summary>
        private int offDiagonalValue;

        /// <summary>
        /// Initializes a new instance of the DiagonalSimilarityMatrix class.
        /// Creates a SimilarityMatrix with one value for match and one for mis-match.
        /// </summary>
        /// <param name="matchValue">Diagonal score for (col == row).</param>
        /// <param name="mismatchValue">Off-diagonal score for (col != row).</param>
        public DiagonalSimilarityMatrix(int matchValue, int mismatchValue)
        {
            diagonalValue = matchValue;
            offDiagonalValue = mismatchValue;
            Matrix = null; // not used

            ////= new Basic(symbols);
            Name = "Diagonal: match value " + diagonalValue + ", non-match value " + offDiagonalValue;

            // Set allowed symbols
            const string symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ*-abcdefghijklmnopqrstuvwxyz";
            foreach (char symbol in symbols)
            {
                supportedAlphabets.Add((byte)symbol);
            }
        }

        /// <summary>
        /// Gets or sets the diagonal value (match value) for the diagonal similarity matrix.
        /// </summary>
        public int DiagonalValue
        {
            get { return diagonalValue; }
            set { diagonalValue = value; }
        }

        /// <summary>
        /// Gets or sets the off diagonal value (mis-match value for the diagonal similarity matrix.
        /// </summary>
        public int OffDiagonalValue
        {
            get { return offDiagonalValue; }
            set { offDiagonalValue = value; }
        }

        /// <summary>
        /// Returns value of diagonal similarity matrix at [row,col].
        /// </summary>
        /// <param name="row">
        /// Row number. This is same as byte value
        /// corresponding to sequence symbol on the row.
        /// </param>
        /// <param name="col">
        /// Column number. This is same as byte value
        /// corresponding to sequence symbol on the column.
        /// </param>
        /// <returns>Score value of matrix at [row,col].</returns>
        public override int this[int row, int col]
        {
            get
            {
                return (col == row) ? diagonalValue : offDiagonalValue;
            }
        }
    }
}
