using System;
using System.Collections.Generic;
using System.Linq;

namespace Bio
{
    /// <summary>
    /// A validator for string values that has a specific list of allowed values.
    /// </summary>
    public class StringListValidator : IParameterValidator
    {
        private readonly List<string> validValues = new List<string>();

        #region -- Properties --

        /// <summary>
        /// The list of allowed values.
        /// </summary>
        public IReadOnlyList<string> ValidValues
        {
            get
            {
                return this.validValues;
            }
        }

        /// <summary>
        /// If IgnoreCase is false (the default), a string will only be considered
        /// valid if it matches one of the stored values exactly, including case.
        /// Otherwise, any case (including mixed) is accepted.
        /// </summary>
        public bool IgnoreCase { get; set; }

        #endregion -- Properties --

        /// <summary>
        /// Constructor that initializes the value list. IgnoreCase defaults to false.
        /// </summary>
        /// <param name="values">An array of valid value strings.</param>
        public StringListValidator(params string[] values)
            : this(false, values)
        {
        }

        /// <summary>
        /// Constructor that allows case sensitivity to be specified.
        /// </summary>
        /// <param name="ignoreCase">true means case will be ignored when validating.</param>
        /// <param name="values">An array of valid value strings.</param>
        public StringListValidator(bool ignoreCase, params string[] values)
        {
            IgnoreCase = ignoreCase;
            AddValidValues(values);
        }

        /// <summary>
        /// Add one or more strings to the list of valid values.
        /// </summary>
        /// <param name="values"></param>
        public void AddValidValues(params string[] values)
        {
            foreach (string value in values)
            {
                if (!this.validValues.Contains(value))
                {
                    this.validValues.Add(value);
                }
            }
        }

        /// <summary>
        /// Given a string value as an object, return true if the value is in the list.
        /// </summary>
        /// <param name="parameterValue">The value.</param>
        /// <returns>True if the value is valid.</returns>
        public bool IsValid(object parameterValue)
        {
            string s = parameterValue as string;
            return s != null && this.IsValid(s);
        }

        /// <summary>
        /// Given a string value, return true if the value is in the list.
        /// </summary>
        /// <param name="parameterValue">The value.</param>
        /// <returns>True if the value is valid.</returns>
        public bool IsValid(string parameterValue)
        {
            if (IgnoreCase)
            {
                return this.validValues.Any(s => s.Equals(parameterValue, StringComparison.CurrentCultureIgnoreCase));
            }
            
            return this.validValues.Contains(parameterValue);
        }
    }
}
