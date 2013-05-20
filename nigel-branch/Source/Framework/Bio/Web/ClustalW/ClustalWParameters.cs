using System;
using System.Collections.Generic;
using System.Globalization;

namespace Bio.Web.ClustalW
{
    /// <summary>
    /// The parameters collection for the Clustal W web service. Consists of a static set of
    /// allowed parameters and validation methods, and a collection of parameter/value
    /// pairs that have been validated and added to the instance.
    /// </summary>
    public class ClustalWParameters
    {
        /// <summary>
        /// Email address for reporting job status/problems.
        /// </summary>
        public const string Email = "EMAIL";

        /// <summary>
        /// Type of Action to Perform.
        /// </summary>
        public const string Action = "ACTION";

        /// <summary>
        /// Perform alignment action.
        /// </summary>
        public const string ActionAlign = "Align";

        /// <summary>
        /// Run in Batch (or) Real-Time.
        /// </summary>
        public const string RunOption = "RUNOPTION";

        /// <summary>
        /// Run in Batch Mode
        /// </summary>
        public const string RunBatch = "RUNBATCH";

        /// <summary>
        /// Number of Cores available
        /// </summary>
        public const string CpuOption = "CPUOPTION";

        /// <summary>
        /// Cluster to be used.
        /// </summary>
        public const string ClusterOption = "CLUSTEROPTION";

        /// <summary>
        /// Format of Alignment output.
        /// </summary>
        public const string FormatAlignment = "FORMATALIGNMENT";

        /// <summary>
        /// ClustalW Format.
        /// </summary>
        public const string FormatClustalW = "CLUSTALWFORMAT";

        /// <summary>
        /// Nexus Format.
        /// </summary>
        public const string FormatNexus = "NEXUSFORMAT";

        /// <summary>
        /// Phylip Format.
        /// </summary>
        public const string FormatPhylip = "PHYLIPFORMAT";

        /// <summary>
        /// Order of output.
        /// </summary>
        public const string OrderOption = "OUTPUTORDER";

        /// <summary>
        /// Order by input.
        /// </summary>
        public const string OrderInput = "INPUTORDER";

        /// <summary>
        /// Order by alignment
        /// </summary>
        public const string OrderAlign = "ALIGNORDER";

        /// <summary>
        /// Protein Matrix to be used.
        /// </summary>
        public const string ProteinMatrixOption = "PROTEINMATRIX";

        /// <summary>
        /// Dna Matrix to be used.
        /// </summary>
        public const string DnaMatrixOption = "DNAMATRIX";

        /// <summary>
        /// Gap Opening Penalty.
        /// </summary>
        public const string GapOpeningPenalty = "GAPOPENINGPENALTY";

        /// <summary>
        /// Gap Extension Penalty.
        /// </summary>
        public const string GapExtensionPenalty = "GAPEXTENSIONPENALTY";

        /// <summary>
        /// Gap Separation Penalty.
        /// </summary>
        public const string GapSeparationPenalty = "GAPSEPARATIONPENALTY";

        /// <summary>
        /// Use Gap End.
        /// </summary>
        public const string GapEnd = "GAPEND";

        /// <summary>
        /// Use Gap Hydrophilic.
        /// </summary>
        public const string GapHydrophilic = "GAPHYDROPHILIC";

        /// <summary>
        /// Use Kimura Correction.
        /// </summary>
        public const string KimuraOption = "KIMURAOPTION";

        /// <summary>
        /// Boot Strap Value.
        /// </summary>
        public const string BootstrapOption = "BOOTSTRAPOPTION";

        /// <summary>
        /// Label Tree Option.
        /// </summary>
        public const string LabelTreeOption = "LABELTREEOPTION";

        /// <summary>
        /// Format of Tree output.
        /// </summary>
        public const string FormatTree = "FORMATTREE";

        /// <summary>
        /// List of request parameter
        /// </summary>
        private static Dictionary<string, RequestParameter> parameters = new Dictionary<string, RequestParameter>();

        /// <summary>
        /// List of parameter values
        /// </summary>
        private Dictionary<string, object> values = new Dictionary<string, object>();

        /// <summary>
        /// Initializes static members of the ClustalWParameters class.
        /// The static constructor defines the initial set of allowed parameters and values.
        /// </summary>
        static ClustalWParameters()
        {
            RequestParameter requestParameter = new RequestParameter(
                    Email,
                    "Email address for reporting job status/problems.",
                    true,
                    string.Empty,
                    "string",
                    null);
            parameters.Add(Email, requestParameter);

            // Can add "Profile", "Tree" & "Bootstrap"
            requestParameter = new RequestParameter(
                    Action,
                    "Type of Action to Perform.",
                    true,
                    ActionAlign,
                    "string",
                    new StringListValidator(ActionAlign));
            parameters.Add(Action, requestParameter);

            requestParameter = new RequestParameter(
                    RunOption,
                    "Run in Batch (or) Real-Time",
                    true,
                    "Batch",
                    "string",
                    null);
            parameters.Add(RunOption, requestParameter);

            // 100 is not correct value, need to verify and change it.
            requestParameter = new RequestParameter(
                        CpuOption,
                        "Number of Cores available.",
                        true,
                        "8",
                        "int",
                        new IntRangeValidator(1, 100));
            parameters.Add(CpuOption, requestParameter);

            requestParameter = new RequestParameter(
                    ClusterOption,
                    "Cluster to be used.",
                    false,
                    string.Empty,
                    "string",
                    null);
            parameters.Add(ClusterOption, requestParameter);

            requestParameter = new RequestParameter(
                    FormatAlignment,
                    "Format of Alignment output.",
                    true,
                    FormatClustalW,
                    "string",
                    new StringListValidator(true, FormatClustalW, FormatPhylip, FormatNexus));
            parameters.Add(FormatAlignment, requestParameter);

            requestParameter = new RequestParameter(
                    OrderOption,
                    "Order of output.",
                    false,
                    string.Empty,
                    "string",
                    new StringListValidator(
                        true, OrderInput, OrderAlign));
            parameters.Add(OrderOption, requestParameter);

            requestParameter = new RequestParameter(
                    ProteinMatrixOption,
                    "Protein Matrix.",
                    false,
                    string.Empty,
                    "string",
                    null);
            parameters.Add(ProteinMatrixOption, requestParameter);

            requestParameter = new RequestParameter(
                    DnaMatrixOption,
                    "Dna Matrix.",
                    false,
                    string.Empty,
                    "string",
                    null);
            parameters.Add(DnaMatrixOption, requestParameter);

            requestParameter = new RequestParameter(
                    GapOpeningPenalty,
                    "Gap Opening Penalty.",
                    false,
                    string.Empty,
                    "double",
                    null);
            parameters.Add(GapOpeningPenalty, requestParameter);

            requestParameter = new RequestParameter(
                    GapExtensionPenalty,
                    "Gap Extension Penalty.",
                    false,
                    string.Empty,
                    "double",
                    null);
            parameters.Add(GapExtensionPenalty, requestParameter);

            requestParameter = new RequestParameter(
                    GapSeparationPenalty,
                    "Gap Separation Penalty.",
                    false,
                    string.Empty,
                    "double",
                    null);
            parameters.Add(GapSeparationPenalty, requestParameter);

            requestParameter = new RequestParameter(
                    GapEnd,
                    "Use Gap End.",
                    false,
                    string.Empty,
                    "string",
                    null);
            parameters.Add(GapEnd, requestParameter);

            requestParameter = new RequestParameter(
                    GapHydrophilic,
                    "Use Gap Hydrophilic.",
                    false,
                    string.Empty,
                    "string",
                    null);
            parameters.Add(GapHydrophilic, requestParameter);

            requestParameter = new RequestParameter(
                    KimuraOption,
                    "Use Kimura Correction.",
                    false,
                    string.Empty,
                    "string",
                    null);
            parameters.Add(KimuraOption, requestParameter);

            requestParameter = new RequestParameter(
                    BootstrapOption,
                    "Boot Strap Value.",
                    false,
                    string.Empty,
                    "int",
                    null);
            parameters.Add(BootstrapOption, requestParameter);

            requestParameter = new RequestParameter(
                    LabelTreeOption,
                    "Label Tree Option.",
                    false,
                    string.Empty,
                    "string",
                    null);
            parameters.Add(LabelTreeOption, requestParameter);

            requestParameter = new RequestParameter(
                    FormatAlignment,
                    "Format of Tree output.",
                    true,
                    FormatClustalW,
                    "string",
                    new StringListValidator(
                        true, FormatClustalW, FormatPhylip, FormatNexus));
            parameters.Add(FormatTree, requestParameter);
        }

        /// <summary>
        /// Gets the various parameters required for a ClustalW service and the
        /// possible values for those parameters.
        /// </summary>
        public static Dictionary<string, RequestParameter> Parameters
        {
            get { return parameters; }
        }

        /// <summary>
        /// Gets or sets values of parameters is the collection of parameter/value pairs that
        /// have been validated and added to this instance. 
        /// </summary>
        public Dictionary<string, object> Values
        {
            get { return values; }
        }

        /// <summary>
        /// Validate a parameter/value pair, and add them to Values,
        /// replacing any value already present for that parameter.
        /// </summary>
        /// <param name="parameterName">The parameter name</param>
        /// <param name="parameterValue">The parameter value</param>
        public void Add(string parameterName, string parameterValue)
        {
            if (!parameters.ContainsKey(parameterName))
            {
                string message = String.Format(CultureInfo.InvariantCulture,
                        Properties.Resource.PARAMETER_UNKNOWN,
                        parameterName);
                throw new Exception(message);
            }

            RequestParameter param = parameters[parameterName];
            if (!param.IsValid(parameterValue))
            {
                string message = String.Format(CultureInfo.InvariantCulture,
                        Properties.Resource.PARAMETER_VALUE_INVALID,
                        parameterValue,
                        parameterName);
                throw new Exception(message);
            }

            Values[param.SubmitName] = parameterValue;
        }
    }
}
