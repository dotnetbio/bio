namespace VennTool
{
    internal class VennToolArguments
    {
        /// <summary>
        /// Display Verbose output during processing
        /// </summary>
        public bool Verbose;

        /// <summary>
        /// Help
        /// </summary>
        public bool Help = false;

        /// <summary>
        /// PreSort .BED files prior to processing
        /// </summary>
        public bool preSort;

        /// <summary>
        /// Write result using polar coordinates
        /// </summary>
        public bool polar;

        /// <summary>
        /// XL OutputFile
        /// </summary>
        public string xl;

        /// <summary>
        /// Values 3 or 7 values for regions in chart, [A B AB] or [A B C AB AC BC ABC]
        /// </summary>
        public double[] regionArray;

        /// <summary>
        /// Constructor
        /// </summary>
        public VennToolArguments()
        {
            this.Help = false;
            this.Verbose = false;
            this.preSort = false;
            this.polar = false;
            this.regionArray = new double[0];
            this.xl = "";
        }
    }
}