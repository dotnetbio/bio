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
        /// Holds allowable values for Vtype.
        /// </summary>
        private static char[] VTypeAllowableValues = "AifZHB".ToCharArray();

        /// <summary>
        /// Holds illegal characters for value.
        /// </summary>
        private static char[] ValueIllegalCharacters = new char[] {'\t','\n','\r'};

        /// <summary>
        /// Holds regular expression for Tag.
        /// </summary>
        private static Regex TagRegexExpr = new Regex(TagRegexExprPattern);

   
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
            if(string.IsNullOrEmpty(tag) || tag.Length!=2 || !ValidateTagRegex(tag))
            {
                string message = string.Format(System.Globalization.CultureInfo.CurrentCulture,
                                Properties.Resource.InvalidPatternMessage,
                                "Tag",
                                tag,
                                TagRegexExpr.ToString());
                return message;
            }
            return string.Empty;
        }
        /// <summary>
        /// Validates that a TAG is a valid regex by converting to an integer and testing it is in the appropriate range
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private static bool ValidateTagRegex(string tag)
        {
            //Going to validate that the tag matches this regex "[A-Za-z][A-Za-z0-9]"
            //without actually using the regex class, to do this we use the following ascii conversions
            //A=65
            //Z=90
            //a=97
            //z=122
            //0=48
            //9=57
            byte c1 = (byte)tag[0];
            byte c2 = (byte)tag[1];
            bool oneOk = (c1>=65 && c1<=90) | (c1>=97 && c1<=122);
            bool twoOk = (c2 >= 65 && c2 <= 90) | (c2 >= 97 && c2 <= 122) | (c2 >= 48 && c2 <= 57);
            return oneOk & twoOk;
        }

        /// <summary>
        /// Validates VType.
        /// </summary>
        /// <param name="vtype">VType value to validate.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
        private static string IsValidVType(string vtype)
        {
            if (vtype.Length != 1)
            {
                return "Optional field variable type must be of length 1, but was of length: " + vtype.Length.ToString();
            }
                //note in this case they are "legal" characters
            else if (Helper.StringContainsIllegalCharacters(vtype, VTypeAllowableValues))
            {
                return String.Empty;
            }
            else
            {
                return "SAM optional field identifier: " + vtype + " was not recognized";
            }
        }

        /// <summary>
        /// Validates Value.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        private static string IsValidValue(string value)
        {
            bool notOkay = Helper.StringContainsIllegalCharacters(value, ValueIllegalCharacters);
            if (!notOkay)
            {
                return String.Empty;
            }
            else
            {
                return value + " is not a valid SAM optional value";
            }
        }
    }
}
