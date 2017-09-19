using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bio.IO.AppliedBiosystems.DataParsers;
using Bio.IO.AppliedBiosystems.DataTypes;
using NUnit.Framework;

namespace Bio.Tests.Framework.IO.AppliedBiosystems
{
    /// <summary>
    /// Test visitor that validates the abi parser against an xml class created by Applied Biosystem's ab1 to xml converter.
    /// </summary>
    public class Ab1DataValidator : IAb1DataVisitor
    {
        private readonly Dictionary<string, List<AB_RootDataTag>> tagsByType;

        /// <summary>
        /// Creates a validator with a set of tags used for validation.
        /// </summary>
        /// <param name="tags"></param>
        public Ab1DataValidator(IEnumerable<AB_RootDataTag> tags)
        {
            this.tagsByType = new Dictionary<string, List<AB_RootDataTag>>();

            tags.ToList().ForEach(
                tag =>
                    {
                        List<AB_RootDataTag> typeTags;
                        string adjustedType = GetAdjustedType(tag);
                        if (!this.tagsByType.TryGetValue(adjustedType, out typeTags))
                        {
                            typeTags = new List<AB_RootDataTag>();
                            this.tagsByType.Add(adjustedType, typeTags);
                        }

                        typeTags.Add(tag);
                    });
        }

        /// <summary>
        /// The xml converter is loaded with inconsistencies, one is the unknown data type "char_byte" which is
        /// just a char array...
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private static string GetAdjustedType(AB_RootDataTag tag)
        {
            if (tag.Type == "char_byte")
                return "char";

            return tag.Type;
        }

        /// <summary>
        /// Retrives the matching xml tag of type and id.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private AB_RootDataTag RetrieveTag(IAb1DataItem item)
        {
            return this.tagsByType[item.Name].First(tag =>
                                                    tag.ID == item.Entry.TagNumber.ToString(CultureInfo.InvariantCulture)
                                                    && tag.Name == item.Entry.TagName);
        }

        /// <summary>
        /// Converts the xml array list of values to a C# list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rawData"></param>
        /// <param name="parse"></param>
        /// <returns></returns>
        private static T[] GetData<T>(string rawData, Func<string, T> parse)
        {
            if (string.IsNullOrEmpty(rawData)) return null;

            return new List<T>((from text in rawData.Split(' ')
                                select parse(text))).ToArray();
        }

        private static void ValidateAreEqual<T>(IList<T> expected, IList<T> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        #region Implementation of IAb1DataVisitor

        /// <summary>
        /// Parser context.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public IParserContext Context { get; private set; }

        /// <summary>
        /// Validate byte visit.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(ByteDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            AB_RootDataTag tag = RetrieveTag(item);
            ValidateAreEqual(GetData(tag.Value, value => byte.Parse(value, CultureInfo.InvariantCulture)), item.Value);
        }

        /// <summary>
        /// Validate char visit.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(CharDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            AB_RootDataTag tag = RetrieveTag(item);

            if (tag.Type == "char_byte")
            {
                ValidateAreEqual(GetData(tag.Value, value => (char)byte.Parse(value, CultureInfo.InvariantCulture)), item.Value);
            }
            else
            {
                Assert.AreEqual(tag.Value, new string(item.Value));
            }
        }


        /// <summary>
        /// Validate word visit.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(WordDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            // TODO: Test file does not hit this yet.
            AB_RootDataTag tag = RetrieveTag(item);
            ValidateAreEqual(GetData(tag.Value, value => ushort.Parse(value, CultureInfo.InvariantCulture)), item.Value);
        }

        /// <summary>
        /// Validate short visit.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(ShortDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            AB_RootDataTag tag = RetrieveTag(item);
            ValidateAreEqual(GetData(tag.Value, value => short.Parse(value, CultureInfo.InvariantCulture)), item.Value);
        }

        /// <summary>
        /// Validate long visit.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(LongDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            AB_RootDataTag tag = RetrieveTag(item);
            ValidateAreEqual(GetData(tag.Value, value => int.Parse(value, CultureInfo.InvariantCulture)), item.Value);
        }

        /// <summary>
        /// Validate float visit.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(FloatDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            AB_RootDataTag tag = RetrieveTag(item);

            //
            // Xml converter only stores 3 decimal places
            //

            ValidateAreEqual(GetData(tag.Value,
                                     value => (float)Math.Round(float.Parse(value, CultureInfo.InvariantCulture), 3)),
                             (from v in item.Value select (float)Math.Round(v, 3)).ToArray());
        }

        /// <summary>
        /// Validate double visit.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(DoubleDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            // TODO: Test file does not hit this yet.
            AB_RootDataTag tag = RetrieveTag(item);
            Assert.AreEqual(GetData(tag.Value, value => double.Parse(value, CultureInfo.InvariantCulture)), item.Value);
        }

        /// <summary>
        /// Validate date visit.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(DateDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            AB_RootDataTag tag = RetrieveTag(item);
            DateTime value = DateTime.Parse(tag.Value, CultureInfo.InvariantCulture);
            Assert.IsTrue(value.Equals(item.Value));
        }

        /// <summary>
        /// Validate time visit.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(TimeDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            AB_RootDataTag tag = RetrieveTag(item);
            DateTime value = DateTime.Parse(tag.Value, CultureInfo.InvariantCulture);
            Assert.IsTrue(value.Equals(item.Value));
        }

        /// <summary>
        /// Validate pstring visit.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(PStringDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            AB_RootDataTag tag = RetrieveTag(item);
            Assert.AreEqual(tag.Value, "\"" + item.Value + "\"");
        }

        /// <summary>
        /// Validate thumb visit.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(ThumbDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            // Not tested with current data file, obsolete.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validate bool visit.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(BoolDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            // Not tested with current data file, obsolete.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validate user visit.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(UserDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            // Not tested with current data file, obsolete.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validate cstring visit.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(CStringDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            AB_RootDataTag tag = RetrieveTag(item);
            Assert.AreEqual(tag.Value, "\"" + item.Value + "\"");
        }

        #endregion
    }
}
