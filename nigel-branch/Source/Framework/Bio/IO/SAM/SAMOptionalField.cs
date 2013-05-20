using System;
using System.Text.RegularExpressions;
using Bio.Util;

namespace Bio.IO.SAM
{
    /// <summary>
    /// This class holds SAM optional field.
    /// </summary>
    [Serializable]
    public class SAMOptionalField
    {
        /// <summary>
        /// Holds regular expression pattern of Tag.
        /// </summary>
        private const string TagRegexExprPattern = "[A-Za-z][A-Za-z0-9]";

        /// <summary>
        /// Holds regular expression pattern of Vtype.
        /// </summary>
        private const string VTypeRegexExprPattern = "[AifZHB]";

        /// <summary>
        /// Holds regular expression pattern of value.
        /// </summary>
        private const string ValueRegexExprPattern = "[^\t\n\r]+";

        /// <summary>
        /// Holds regular expression for Tag.
        /// </summary>
        private static Regex TagRegexExpr = new Regex(TagRegexExprPattern);

        /// <summary>
        /// Holds regular expression for Vtype.
        /// </summary>
        private static Regex VTypeRegexExpr = new Regex(VTypeRegexExprPattern);

        /// <summary>
        /// Holds regular expression for Value.
        /// </summary>
        private static Regex ValueRegexExpr = new Regex(ValueRegexExprPattern);

        /// <summary>
        /// Holds tag value of the option field.
        /// </summary>
        private string tagValue;

        /// <summary>
        /// Holds type of the value present in the "Value" property.
        /// </summary>
        private string valueType;

        /// <summary>
        /// Holds value of the optional field.
        /// </summary>
        private string fieldValue;

        /// <summary>
        /// Tag of the option field.
        /// </summary>
        public string Tag
        {
            get
            {
                return tagValue;
            }
            set
            {
                string message = IsValidTag(value);
                if (!string.IsNullOrEmpty(message))
                {
                    throw new ArgumentException(message);
                }

                tagValue = value;
            }
        }

        /// <summary>
        /// Type of the value present in the "Value" property.
        /// </summary>
        public string VType
        {
            get
            {
                return this.valueType;
            }
            set
            {
                string message = IsValidVType(value);
                if (!string.IsNullOrEmpty(message))
                {
                    throw new ArgumentException(message);
                }

                this.valueType = value;
            }
        }

        /// <summary>
        /// Value of the optional field.
        /// </summary>
        public string Value
        {
            get
            {
                return this.fieldValue;
            }
            set
            {
                string message = IsValidValue(value);
                if (!string.IsNullOrEmpty(message))
                {
                    throw new ArgumentException(message);
                }

                this.fieldValue = value;
            }
        }

        /// <summary>
        /// Validates Tag.
        /// </summary>
        /// <param name="tag">Tag value to validate.</param>
        private static string IsValidTag(string tag)
        {
            return Helper.IsValidPatternValue("Tag", tag, TagRegexExpr);
        }

        /// <summary>
        /// Validates VType.
        /// </summary>
        /// <param name="vtype">VType value to validate.</param>
        private static string IsValidVType(string vtype)
        {
            return Helper.IsValidPatternValue("VType", vtype, VTypeRegexExpr);
        }

        /// <summary>
        /// Validates Value.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        private static string IsValidValue(string value)
        {
            return Helper.IsValidPatternValue("Value", value, ValueRegexExpr);
        }
    }
}
