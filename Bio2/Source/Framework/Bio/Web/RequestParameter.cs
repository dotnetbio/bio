using System;
using Bio.Util.Logging;

namespace Bio.Web
{
    /// <summary>
    /// A RequestParameter is a description of a single parameter that might
    /// be used in a remote service request (HTTP, SOAP, etc.) The name is
    /// a string, and the value can be an int, double, or string (but may
    /// be represented in string form when submitted, as in HTTP). Validation
    /// is extensible so that any needed rules can be used to validate values.
    /// </summary>
    public class RequestParameter
    {
        #region Constructors

        /// <summary>
        /// Construct a RequestParameter, specifying all properties.
        /// </summary>
        /// <param name="name">The name of the parameter as required by a service.</param>
        /// <param name="description">A friendly description of the parameter.</param>
        /// <param name="required">True if this is a required parameter.</param>
        /// <param name="defaultValue">The default value that will be used (expressed as a string).</param>
        /// <param name="dataType">The data type: int, double, or string.</param>
        /// <param name="validator">The validation object, or null if no validation is required.</param>
        public RequestParameter(
            string name,
            string description,
            bool required,
            string defaultValue,
            string dataType,
            IParameterValidator validator)
        {
            SubmitName = name;
            Description = description;
            Required = required;
            DefaultValue = defaultValue;
            if (dataType != "int" && dataType != "double" && dataType != "string")
            {
                string message = Properties.Resource.SearchParamInvalidArgs;
                Trace.Report(message);
                throw new NotSupportedException(message);
            }
            DataType = dataType;
            Validator = validator;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name as it should be submitted to a service.
        /// </summary>
        public string SubmitName { get; set; }

        /// <summary>
        /// A friendly description of the parameter's meaning.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// True if the parameter is required by the service.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// The default value (as a string) that the service will assume if the
        /// parameter is not specified. Ignored if Required = true.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// A string indicating the data type ("int", "double", or "string").
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// The validation object that tests values. If this is null, any
        /// value will be accepted.
        /// </summary>
        public IParameterValidator Validator { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Given a value expressed as a string, return true if the value
        /// is valid according to this parameter's semantics.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>True if the value is allowed.</returns>
        public bool IsValid(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return !Required;   // ok to omit?
            }
            if (Validator == null)
            {
                return true;    // no restrictions
            }
            return Validator.IsValid(value);
        }

        #endregion
    }
}
