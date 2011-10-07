using System.Windows;

namespace ReadSimulator
{
    /// <summary>
    /// Known default settings for sequencing methods.
    /// </summary>
    public enum DefaultSettings
    {
        SangerDideoxy,
        PyroSequencing,
        ShortRead
    }

    /// <summary>
    /// Random number distribution type. Currently understands uniform and normal distributions.
    /// </summary>
    public enum Distribution
    {
        Uniform = 0,
        Normal = 1
    }

    /// <summary>
    /// Data model for the settings provided for a simulation run of the sequencer.
    /// </summary>
    public class SimulatorSettings : DependencyObject
    {
        #region Dependency Properties
        /// <summary>
        /// The path of the input sequence file whose first sequence will be split
        /// </summary>
        public static readonly DependencyProperty InputFileProperty =
            DependencyProperty.Register("InputFile", typeof(string), typeof(SimulatorSettings));

        /// <summary>
        /// The path of the output sequence file. If more than one file is needed an
        /// indexed suffix will be applied to the file name
        /// </summary>
        public static readonly DependencyProperty OutputFileProperty =
            DependencyProperty.Register("OutputFile", typeof(string), typeof(SimulatorSettings));

        /// <summary>
        /// The expected value of the coverage to be found for each base item once simulation is run
        /// </summary>
        public static readonly DependencyProperty DepthOfCoverageProperty =
            DependencyProperty.Register("DepthOfCoverage", typeof(int), typeof(SimulatorSettings));

        /// <summary>
        /// The average length of a generated subsequence
        /// </summary>
        public static readonly DependencyProperty SequenceLengthProperty =
            DependencyProperty.Register("SequenceLength", typeof(int), typeof(SimulatorSettings));

        /// <summary>
        /// The variation plus or minus from the SequenceLength if using uniform
        /// distribution or the standard deviation if using normal distribution.
        /// </summary>
        public static readonly DependencyProperty LengthVariationProperty =
            DependencyProperty.Register("LengthVariation", typeof(int), typeof(SimulatorSettings));

        /// <summary>
        /// The frequency at which errors will be introduced to a base item. The
        /// value should be represented as a float between 0.0 and 1.0 representing
        /// a percentage chance of introducing error.
        /// </summary>
        public static readonly DependencyProperty ErrorFrequencyProperty =
            DependencyProperty.Register("ErrorFrequency", typeof(float), typeof(SimulatorSettings));

        /// <summary>
        /// Flag to indicate whether or not to introduce ambiguity characters as
        /// part of the error insertion.
        /// </summary>
        public static readonly DependencyProperty AllowAmbiguitiesProperty =
            DependencyProperty.Register("AllowAmbiguities", typeof(bool), typeof(SimulatorSettings));

        /// <summary>
        /// The maximum number of sequences to place in a single output file.
        /// </summary>
        public static readonly DependencyProperty OutputSequenceCountProperty =
            DependencyProperty.Register("OutputSequenceCount", typeof(int), typeof(SimulatorSettings));

        /// <summary>
        /// Flag indicating whether or not to reverse half of the generated subsequences.
        /// </summary>
        public static readonly DependencyProperty ReverseHalfProperty =
            DependencyProperty.Register("ReverseHalf", typeof(bool), typeof(SimulatorSettings));

        /// <summary>
        /// Value indexed from the Distribution enum
        /// </summary>
        public static readonly DependencyProperty DistributionTypeProperty =
            DependencyProperty.Register("DistributionType", typeof(int), typeof(SimulatorSettings));
        #endregion

        #region Properties
        /// <summary>
        /// The path of the input sequence file whose first sequence will be split
        /// </summary>
        public string InputFile
        {
            get { return (string)GetValue(InputFileProperty); }
            set { SetValue(InputFileProperty, value); }
        }

        /// <summary>
        /// The path of the output sequence file. If more than one file is needed an
        /// indexed suffix will be applied to the file name
        /// </summary>
        public string OutputFile
        {
            get { return (string)GetValue(OutputFileProperty); }
            set { SetValue(OutputFileProperty, value); }
        }

        /// <summary>
        /// The expected value of the coverage to be found for each base item once simulation is run
        /// </summary>
        public int DepthOfCoverage
        {
            get { return (int)GetValue(DepthOfCoverageProperty); }
            set { SetValue(DepthOfCoverageProperty, value); }
        }

        /// <summary>
        /// The average length of a generated subsequence
        /// </summary>
        public int SequenceLength
        {
            get { return (int)GetValue(SequenceLengthProperty); }
            set { SetValue(SequenceLengthProperty, value); }
        }

        /// <summary>
        /// The variation plus or minus from the SequenceLength if using uniform
        /// distribution or the standard deviation if using normal distribution.
        /// </summary>
        public int LengthVariation
        {
            get { return (int)GetValue(LengthVariationProperty); }
            set { SetValue(LengthVariationProperty, value); }
        }

        /// <summary>
        /// The frequency at which errors will be introduced to a base item. The
        /// value should be represented as a float between 0.0 and 1.0 representing
        /// a percentage chance of introducing error.
        /// </summary>
        public float ErrorFrequency
        {
            get { return (float)GetValue(ErrorFrequencyProperty); }
            set { SetValue(ErrorFrequencyProperty, value); }
        }

        /// <summary>
        /// Flag to indicate whether or not to introduce ambiguity characters as
        /// part of the error insertion.
        /// </summary>
        public bool AllowAmbiguities
        {
            get { return (bool)GetValue(AllowAmbiguitiesProperty); }
            set { SetValue(AllowAmbiguitiesProperty, value); }
        }

        /// <summary>
        /// The maximum number of sequences to place in a single output file.
        /// </summary>
        public int OutputSequenceCount
        {
            get { return (int)GetValue(OutputSequenceCountProperty); }
            set { SetValue(OutputSequenceCountProperty, value); }
        }

        /// <summary>
        /// Flag indicating whether or not to reverse half of the generated subsequences.
        /// </summary>
        public bool ReverseHalf
        {
            get { return (bool)GetValue(ReverseHalfProperty); }
            set { SetValue(ReverseHalfProperty, value); }
        }

        /// <summary>
        /// Value indexed from the Distribution enum
        /// </summary>
        public int DistributionType
        {
            get { return (int)GetValue(DistributionTypeProperty); }
            set { SetValue(DistributionTypeProperty, value); }
        }
        #endregion

        /// <summary>
        /// Constructor that sets the defaults to the Sanger Dideoxy settings.
        /// </summary>
        public SimulatorSettings()
        {
            // Set defaults
            SetDefaults(DefaultSettings.SangerDideoxy);
        }

        /// <summary>
        /// Sets defaults to known good values based on the known DefaultSettings enum.
        /// </summary>
        public void SetDefaults(DefaultSettings settings)
        {
            AllowAmbiguities = false;
            OutputSequenceCount = 1000;
            switch (settings)
            {
                case DefaultSettings.PyroSequencing:
                    DepthOfCoverage = 35;
                    SequenceLength = 150;
                    LengthVariation = 50;
                    ErrorFrequency = 0.03f;
                    DistributionType = (int)Distribution.Normal;
                    ReverseHalf = false;
                    break;
                case DefaultSettings.SangerDideoxy:
                    DepthOfCoverage = 10;
                    SequenceLength = 750;
                    LengthVariation = 250;
                    ErrorFrequency = 0.00001f;
                    DistributionType = (int)Distribution.Normal;
                    ReverseHalf = false;
                    break;
                case DefaultSettings.ShortRead:
                    DepthOfCoverage = 100;
                    SequenceLength = 37;
                    LengthVariation = 12;
                    ErrorFrequency = 0.0103f;
                    DistributionType = (int)Distribution.Uniform;
                    ReverseHalf = false;
                    break;
            }
        }
    }
}
