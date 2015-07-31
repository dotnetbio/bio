using System;
using System.Globalization;
using System.Linq;

namespace Bio.Util
{
    /// <summary>
    /// A set of predefined converter pairs for use with ConvertValueView.
    /// </summary>
    public abstract class ValueConverter
    {

        /// <summary>
        /// Tries to convert a character into any other type.
        /// </summary>
        /// <typeparam name="T">The other type</typeparam>
        /// <returns>The converter</returns>
        static public ValueConverter<char, T> GetCharToGeneric<T>()
        {
            if (typeof(T) == typeof(string))
            {
                return (ValueConverter<char, T>)(object)CharToString;
            }
            if (typeof(T) == typeof(int))
            {
                return (ValueConverter<char, T>)(object)CharToInt;
            }
            if (typeof(T) == typeof(double))
            {
                return (ValueConverter<char, T>)(object)CharToDouble;
            }

            throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "Don't have a predefined converter for char to {0}.", typeof(T)));
        }

        /// <summary>
        /// Converts the characters '0'...'9' to the integers 0 ... 9
        /// </summary>
        public static readonly ValueConverter<char, int> CharToInt = new CharToIntConverter();
        /// <summary>
        /// Converts the integers 0...9 to the characters '0' ... '9'
        /// </summary>
        public static readonly ValueConverter<int, char> IntToChar = CharToInt.Inverted;
        /// <summary>
        /// Converts a character into a string.
        /// </summary>
        public static readonly ValueConverter<char, string> CharToString = new CharToStringConverter();
        /// <summary>
        /// Converts a length-one string into a character.
        /// </summary>
        public static readonly ValueConverter<string, char> StringToChar = CharToString.Inverted;
        /// <summary>
        /// Converts the characters '0' ... '9' to the doubles 0.0 ... 9.0.
        /// </summary>
        public static readonly ValueConverter<char, double> CharToDouble = new CharToDoubleConverter();
        /// <summary>
        /// Converts an integer to a double
        /// </summary>
        public static readonly ValueConverter<int, double> IntToDouble = new IntToDoubleConverter();
        /// <summary>
        /// Converts an int 0 and int 1 to a double
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly ValueConverter<int, double> Int01ToDouble = new Int01ToDoubleConverter();
        /// <summary>
        /// Converts the doubles 0.0 ... 9.0 to the characters '0' ... '9'.
        /// </summary>
        public static readonly ValueConverter<double, char> DoubleToChar = CharToDouble.Inverted;
        /// <summary>
        /// Converts a double to an int.
        /// </summary>
        public static readonly ValueConverter<double, int> DoubleToInt = IntToDouble.Inverted;
        /// <summary>
        /// Converts a double to an int 0 or int 1.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly ValueConverter<double, int> DoubleToInt01 = Int01ToDouble.Inverted;
        /// <summary>
        /// Converts the character '0' to the double -1.0 and the character '1' to the double 1.0.
        /// </summary>
        public static readonly ValueConverter<char, double> Char01ToMinusOneOne = new Char01ToMinusOneOne();
        /// <summary>
        /// Converts the double -1.0 to the character '0' and the double 1.0 to the character '1'.
        /// </summary>
        public static readonly ValueConverter<double, char> MinusOneOneToChar01 = Char01ToMinusOneOne.Inverted;
        /// <summary>
        /// Converts the character '0' to boolean false and the character '1' to boolean true.
        /// </summary>
        public static readonly ValueConverter<char, bool?> Char01ToBool = new Char01ToBool();
        /// <summary>
        ///  Converts boolean false to the character '0' and boolean true to character '1'
        /// </summary>
        public static readonly ValueConverter<bool?, char> BoolToChar01 = Char01ToBool.Inverted;
        /// <summary>
        /// Converts the characters 'A', 'C', 'T', 'G' to doubles 0.0, 1.0, 3.0, 2.0.
        /// Lower case is accepted, the inverse always produces uppercase.
        /// </summary>
        public static readonly ValueConverter<char, double> CharACTGToDouble0123Converter = new CharACTGToDouble0123Converter();
    }
    /// <summary>
    /// A pair of functions used by ConvertView to transform a matrix's non-missing values from one type to another. The two functions are
    /// perfect inverses of each other.
    /// </summary>
    /// <typeparam name="TInput">The type to convert from</typeparam>
    /// <typeparam name="TOutput">The type to convert to</typeparam>
    /// 
    public class ValueConverter<TInput, TOutput> : ValueConverter
    {
        /// <summary>
        /// A function used to a matrix's non-missing values from one type to another.
        /// </summary>
        public Func<TInput, TOutput> ConvertForward { get; protected set; }
        /// <summary>
        /// The perfect inverse of ConvertForward
        /// </summary>
        public Func<TOutput, TInput> ConvertBackward { get; protected set; }

        /// <summary>
        /// The ValueConverter that reverses ConvertForward and ConvertBackward
        /// </summary>
        public ValueConverter<TOutput, TInput> Inverted { get; protected set; }

        private ValueConverter()
        {
        }


        /// <summary>
        /// Initializes a new instance of the ValueConverter class.
        /// </summary>
        /// <param name="convertForward">A function used to a matrix's non-missing values from one type to another.</param>
        /// <param name="convertBackward">The perfect inverse of ConvertForward</param>
        public ValueConverter(Func<TInput, TOutput> convertForward, Func<TOutput, TInput> convertBackward)
        {
            ConvertForward = convertForward;
            ConvertBackward = convertBackward;
            Inverted = new ValueConverter<TOutput, TInput>
            {
                ConvertBackward = this.ConvertForward,
                ConvertForward = this.ConvertBackward,
                Inverted = this
            };
        }
    }

    #region Implementations

    internal class CharToIntConverter : ValueConverter<char, int>
    {
        public CharToIntConverter()
            : base(c => int.Parse(c.ToString()), i => i.ToString().Cast<char>().Single())
        {
        }
    }

    internal class CharToStringConverter : ValueConverter<char, string>
    {
        public CharToStringConverter()
            : base(c => c.ToString(), s => s.Cast<char>().Single())
        {
        }
    }

    internal class CharToDoubleConverter : ValueConverter<char, double>
    {
        public CharToDoubleConverter()
            : base(c => double.Parse(c.ToString()), i => i.ToString().Cast<char>().Single())
        {
        }
    }

    internal class IntToDoubleConverter : ValueConverter<int, double>
    {
        public IntToDoubleConverter()
            : base(
                i => (double)i,
                d =>
                {
                    int i = (int)d;
                    if (i != d)
                    {
                        throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "Cannot convert {0} into an integer", d));
                    }
                    return i;
                }
                )
        { }
    }

    internal class Int01ToDoubleConverter : ValueConverter<int, double>
    {
        public Int01ToDoubleConverter()
            : base(
                i =>
                {
                    switch (i)
                    {
                        case 0: return 0.0;
                        case 1: return 1.0;
                    }
                    throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "Expect value to be 0 or 1, not {0}", i));
                },

                d =>
                {
                    if (d == 0.0) return 0;
                    if (d == 1.0) return 1;
                    throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "Expect value to be 0.0 or 1.0, not {0}", d));
                }
                )
        { }
    }


    internal class CharACTGToDouble0123Converter : ValueConverter<char, double>
    {
        public CharACTGToDouble0123Converter() : base(ACTGTo0123Double, Zero123DoubleToACTG) { }

        static public char Zero123DoubleToACTG(double d)
        {
            if (d == 0.0) return 'A';
            else if (d == 1.0) return 'C';
            else if (d == 2.0) return 'G';
            else if (d == 3.0) return 'T';
            else throw new ArgumentOutOfRangeException(string.Format(Properties.Resource.ErrorConvertingDoubleToNucleotide, d));
        }

        static public double ACTGTo0123Double(char c)
        {
            if (c == 'A' || c == 'a')
            {
                return 0.0;
            }
            else if (c == 'C' || c == 'c')
            {
                return 1.0;
            }
            else if (c == 'G' || c == 'g')
            {
                return 2.0;
            }
            else if (c == 'T' || c == 't')
            {
                return 3.0;
            }
            else
            {
                throw new ArgumentOutOfRangeException(string.Format(Properties.Resource.ErrorConvertingCharacterNucleotideToDouble, c));
            }
        }
    }

    internal class Char01ToMinusOneOne : ValueConverter<char, double>
    {
        public Char01ToMinusOneOne() : base(ZeroOneCharToMinus11Double, Minus11DoubleAsZeroOneChar) { }

        static public double ZeroOneCharToMinus11Double(char c)
        {
            if (c == '0')
            {
                return -1.0;
            }
            else if (c == '1')
            {
                return 1.0;
            }
            Helper.CheckCondition(false, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedCharToBeZeroOrOne, c));
            return double.NaN;
        }

        static public char Minus11DoubleAsZeroOneChar(double r)
        {
            if (r == -1.0)
            {
                return '0';
            }
            else if (r == 1.0)
            {
                return '1';
            }
            Helper.CheckCondition(false, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectDoubleToBeMinusOneOrOne, r));
            return default(char);
        }

    }

    internal class Char01ToBool : ValueConverter<char, bool?>
    {
        public Char01ToBool() : base(ZeroOneCharToBool, BoolToZeroOneChar) { }

        static public bool? ZeroOneCharToBool(char c)
        {
            if (c == '0')
            {
                return false;
            }
            else if (c == '1')
            {
                return true;
            }
            Helper.CheckCondition(false, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedCharToBeZeroOrOne, c));
            return null;
        }

        static public char BoolToZeroOneChar(bool? r)
        {
            Helper.CheckCondition(r.HasValue, () => Properties.Resource.ExpectedBoolToBeTrueOrFalse);
            if (r.Value)
            {
                return '1';
            }
            else
            {
                return '0';
            }
        }
    }


    /// <summary>
    /// A char to double converter that limits the size of the double.
    /// </summary>
    public class CharToDoubleWithLimitsConverter : ValueConverter<char, double>
    {
        int MaxValue;

        /// <summary>
        /// Create a char to double converter that limits the size of the double.
        /// </summary>
        /// <param name="maxValue">The largest double that can ever be returned.</param>
        public CharToDoubleWithLimitsConverter(int maxValue)
            : base(null, null)
        {
            MaxValue = maxValue;
            ConvertForward = InternalConvertForward;
            ConvertBackward = InternalConvertBackward;
        }
        private double InternalConvertForward(char c)
        {
            double r = double.Parse(c.ToString());
            Helper.CheckCondition(r <= MaxValue, "Value {0} should be <= {1}", c, MaxValue);
            return r;
        }
        private char InternalConvertBackward(double r)
        {
            Helper.CheckCondition(r <= MaxValue, "Value {0} should be <= {1}", r, MaxValue);
            char c = r.ToString().Cast<char>().Single();
            return c;
        }
    }

    /// <summary>
    /// This is a slow parser that will parse to anything with a Parse or TryParse method using the Parser class. This uses reflection and will not
    /// be terribly efficient.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class CharToGenericConverter<T> : ValueConverter<char, T>
    {
        public CharToGenericConverter()
            : base(c => Parser.Parse<T>(c.ToString()), val =>
            {
                string valAsString = val.ToString();
                Helper.CheckCondition(1 == valAsString.Length, "Cannot convert value ({0}) to a character because it is not one character long", valAsString);
                return valAsString[0];
            })
        {
        }
    }

    #endregion
}
