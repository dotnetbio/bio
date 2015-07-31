using System;
using System.Diagnostics;

namespace Bio.Algorithms.Alignment
{
    /// <summary>
    /// This class defines generic properties of any alignment algorithm.
    /// </summary>
    public class AlignmentInfo
    {
        #region -- Constants --

        /// <summary>
        /// "int" data type argument
        /// </summary>
        public const string IntType = "INT";

        /// <summary>
        /// "float" data type argument
        /// </summary>
        public const string FloatType = "FLOAT";

        /// <summary>
        /// "string" data type argument
        /// </summary>
        public const string StringListType = "STRINGLIST";

        #endregion

        #region -- Private Variables --

        /// <summary>
        /// Data type of the parameter
        /// </summary>
        private string dataType;

        #endregion

        #region -- Constructors --

        /// <summary>
        /// Initializes a new instance of the AlignmentInfo class, 
        /// specifying all properties.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="description">A friendly description of property.</param>
        /// <param name="required">True if this is a required property.</param>
        /// <param name="defaultValue">The default value that will be used (expressed as a string).</param>
        /// <param name="dataType">The data type: INT, FLOAT, or STRINGLIST.</param>
        /// <param name="validator">The validation object, or null if no validation is required.</param>
        public AlignmentInfo(
            string name,
            string description,
            bool required,
            string defaultValue,
            string dataType,
            IParameterValidator validator)
        {
            Name = name;
            Description = description;
            Required = required;
            DefaultValue = defaultValue;
            DataType = dataType;
            Validator = validator;
        }

        #endregion

        #region -- Properties --

        /// <summary>
        /// Gets or sets the name of attribute.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets description of the parameter's meaning.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parameter is required property or not.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets default value (as a string). Ignored if Required = true.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets data type ("int", "double", "float" or "string") of parameter.
        /// </summary>
        public string DataType 
        {
            get
            {
                return this.dataType;
            }

            set
            {
                ValidateDataType(value);
                this.dataType = value;
            }
        }

        /// <summary>
        /// Gets or sets validation object that tests values. If this is null, 
        /// any value will be accepted
        /// </summary>
        public IParameterValidator Validator { get; set; }

        #endregion

        #region -- Private Methods --

        /// <summary>
        /// Validate the data type value
        /// </summary>
        /// <param name="dataType">Type to be validated</param>
        private static void ValidateDataType(string dataType)
        {
            if (0 != string.Compare(dataType, IntType, StringComparison.OrdinalIgnoreCase)
                && 0 != string.Compare(dataType, FloatType, StringComparison.OrdinalIgnoreCase)
                && 0 != string.Compare(dataType, StringListType, StringComparison.OrdinalIgnoreCase))
            {
                string message = Properties.Resource.InvalidSearchParameter;
                Debug.WriteLine(message);
                throw new NotSupportedException(message);
            }
        }

        #endregion
    }
}
