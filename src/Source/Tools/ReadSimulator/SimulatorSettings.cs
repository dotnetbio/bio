using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

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
    public class SimulatorSettings : INotifyPropertyChanged
    {
        #region Data
        private bool allowAmbiguities;
        private bool reverseHalf;
        #endregion

        #region Properties

        /// <summary>
        /// The path of the input sequence file whose first sequence will be split
        /// </summary>
        public string InputFile { get; set; }

        /// <summary>
        /// The path of the output sequence file. If more than one file is needed an
        /// indexed suffix will be applied to the file name
        /// </summary>
        public string OutputFile { get; set; }

        /// <summary>
        /// The expected value of the coverage to be found for each base item once simulation is run
        /// </summary>
        public int DepthOfCoverage { get; set; }

        /// <summary>
        /// The average length of a generated subsequence
        /// </summary>
        public int SequenceLength { get; set; }

        /// <summary>
        /// The variation plus or minus from the SequenceLength if using uniform
        /// distribution or the standard deviation if using normal distribution.
        /// </summary>
        public int LengthVariation { get; set; }

        /// <summary>
        /// The frequency at which errors will be introduced to a base item. The
        /// value should be represented as a float between 0.0 and 1.0 representing
        /// a percentage chance of introducing error.
        /// </summary>
        public float ErrorFrequency { get; set; }

        /// <summary>
        /// Flag to indicate whether or not to introduce ambiguity characters as
        /// part of the error insertion.
        /// </summary>
        public bool AllowAmbiguities
        {
            get { return allowAmbiguities; }
            set { allowAmbiguities = value; OnPropertyChanged(() => AllowAmbiguities); }
        }

        /// <summary>
        /// The maximum number of sequences to place in a single output file.
        /// </summary>
        public int OutputSequenceCount { get; set; }

        /// <summary>
        /// Flag indicating whether or not to reverse half of the generated subsequences.
        /// </summary>
        public bool ReverseHalf
        {
            get { return reverseHalf; }
            set { reverseHalf = value; OnPropertyChanged(() => ReverseHalf); }
        }

        /// <summary>
        /// Value indexed from the Distribution enum
        /// </summary>
        public int DistributionType { get; set; }
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
            ReverseHalf = false;

            switch (settings)
            {
                case DefaultSettings.PyroSequencing:
                    DepthOfCoverage = 35;
                    SequenceLength = 150;
                    LengthVariation = 50;
                    ErrorFrequency = 0.03f;
                    DistributionType = (int)Distribution.Normal;
                    break;
                case DefaultSettings.SangerDideoxy:
                    DepthOfCoverage = 10;
                    SequenceLength = 750;
                    LengthVariation = 250;
                    ErrorFrequency = 0.00001f;
                    DistributionType = (int)Distribution.Normal;
                    break;
                case DefaultSettings.ShortRead:
                    DepthOfCoverage = 100;
                    SequenceLength = 37;
                    LengthVariation = 12;
                    ErrorFrequency = 0.0103f;
                    DistributionType = (int)Distribution.Uniform;
                    break;
            }

            // All properties have changed.
            PropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
        }


        /// <summary>
        /// Property Change notification
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Method to raise INPC.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propExpr"></param>
        private void OnPropertyChanged<T>(Expression<Func<T>> propExpr)
        {
            var prop = (PropertyInfo)((MemberExpression)propExpr.Body).Member;
            PropertyChanged(this, new PropertyChangedEventArgs(prop.Name));
        }
    }
}
