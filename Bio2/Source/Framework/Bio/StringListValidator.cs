using System;
using System.Collections.Generic;

namespace Bio
{
    /// <summary>
    /// A validator for string values that has a specific list of allowed values.
    /// </summary>
    public class StringListValidator : IParameterValidator
    {
        private List<string> _validValues = new List<string>();

        #region -- Properties --

        /// <summary>
        /// The list of allowed values.
        /// </summary>
        public IList<string> ValidValues
        {
            get
            {
                return _validValues.AsReadOnly();
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
                if (!_validValues.Contains(value))
                {
                    _validValues.Add(value);
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
            if (s == null)
            {
                return false;
            }
            return IsValid(s);
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
                foreach (string s in _validValues)
                {
                    if (s.Equals(parameterValue, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return _validValues.Contains(parameterValue);
            }
        }
    }
}
