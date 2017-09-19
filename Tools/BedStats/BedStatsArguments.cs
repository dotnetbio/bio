namespace BedStats
{
    internal class BedStatsArguments
    {
        /// <summary>
        /// Display Verbose logging during processing.
        /// </summary>
        public bool verbose;

        /// <summary>
        /// NormalizeInput .BED files prior to processing.
        /// </summary>
        public bool normalizeInputs;

        /// <summary>
        /// Output file for use with VennTool.
        /// </summary>
        public string outputVennTool;

        /// <summary>
        /// Create an Excel file with BED stats.
        /// </summary>
        public string xlFilename;

        /// <summary>
        /// List of 2 or 3 input .BED files to process.
        /// </summary>
        public string[] inputFiles;

        /// <summary>
        /// Displays the help.
        /// </summary>
        public bool Help = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public BedStatsArguments()
        {
            this.Help = false;
            this.verbose = false;
            this.normalizeInputs = false;
            this.outputVennTool = null;
            this.xlFilename = null;
            this.inputFiles = null;
        }
    }
}