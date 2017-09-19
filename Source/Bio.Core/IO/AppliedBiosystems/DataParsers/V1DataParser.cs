using System;
using System.Linq;
using Bio.IO.AppliedBiosystems.DataTypes;
using Bio.IO.AppliedBiosystems.Exceptions;
using Bio.Util;

namespace Bio.IO.AppliedBiosystems.DataParsers
{
    /// <summary>
    /// Parses a major version 1 ab1 data file directory entries.  
    /// http://www6.appliedbiosystems.com/support/software_community/ABIF_File_Format.pdf
    /// Note that all numeric values are stored with the higher order byte at the beginning of the entry.
    /// </summary>
    public class V1DataParser : IAb1DataVisitor, IVersionedDataParser
    {
        /// <summary>
        /// Version this parser works with.
        /// </summary>
        public const int MajorVersion = 1;

        #region Implementation of IAb1DataVisitor

        /// <summary>
        /// Parser context.
        /// </summary>
        public IParserContext Context { get; private set; }

        /// <summary>
        /// Visit byte item.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(ByteDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            byte[] value = GetItemValue(item);
            item.Value = value;
            Context.DataItems.Add(item);
        }

        /// <summary>
        /// Visit char item.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(CharDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            byte[] value = GetItemValue(item);
            item.Value = (from b in value select (char)b).ToArray();
            Context.DataItems.Add(item);
        }

        /// <summary>
        /// Visit word item.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(WordDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            byte[] values = GetItemValue(item);
            item.Value =
                ParserHelper.ConvertSegmentsToArray(
                    ParserHelper.SegmentArray(values, 2, true),
                    (segment => BitConverter.ToUInt16(segment, 0)));
            Context.DataItems.Add(item);
        }

        /// <summary>
        /// Visit short item.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(ShortDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            byte[] values = GetItemValue(item);
            item.Value =
                ParserHelper.ConvertSegmentsToArray(
                    ParserHelper.SegmentArray(values, 2, true),
                    (segment => BitConverter.ToInt16(segment, 0)));
            Context.DataItems.Add(item);
        }

        /// <summary>
        /// Visit long item.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(LongDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            byte[] values = GetItemValue(item);
            item.Value =
                ParserHelper.ConvertSegmentsToArray(
                    ParserHelper.SegmentArray(values, 4, true),
                    (segment => BitConverter.ToInt32(segment, 0)));
            Context.DataItems.Add(item);
        }

        /// <summary>
        /// Visit float item.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(FloatDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            byte[] values = GetItemValue(item);
            item.Value =
                ParserHelper.ConvertSegmentsToArray(
                    ParserHelper.SegmentArray(values, 4, true),
                    (segment => BitConverter.ToSingle(segment, 0)));
            Context.DataItems.Add(item);
        }

        /// <summary>
        /// Visit double item.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(DoubleDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            byte[] values = GetItemValue(item);
            item.Value =
                ParserHelper.ConvertSegmentsToArray(
                    ParserHelper.SegmentArray(values, 8, true),
                    (segment => BitConverter.ToDouble(segment, 0)));
            Context.DataItems.Add(item);
        }

        /// <summary>
        /// Visit date item.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(DateDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            byte[] values = GetItemValue(item);
            Int16 year = BitConverter.ToInt16(new[] {values[1], values[0]}, 0);
            Int16 month = BitConverter.ToInt16(new byte[] {values[2], 0}, 0);
            Int16 day = BitConverter.ToInt16(new byte[] {values[3], 0}, 0);

            item.Value = new DateTime(year, month, day);
            Context.DataItems.Add(item);
        }

        /// <summary>
        /// Visit time item.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(TimeDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            byte[] values = GetItemValue(item);
            Int16 hour = BitConverter.ToInt16(new byte[] {values[0], 0}, 0);
            Int16 minute = BitConverter.ToInt16(new byte[] {values[1], 0}, 0);
            Int16 second = BitConverter.ToInt16(new byte[] {values[2], 0}, 0);
            Int16 hsecond = BitConverter.ToInt16(new byte[] {values[3], 0}, 0);

            item.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, second,
                                      hsecond * 10);
            Context.DataItems.Add(item);
        }

        /// <summary>
        /// Visit pstring item.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(PStringDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            // 
            // First byte indicates the size of the string.
            //

            byte[] values = GetItemValue(item).Skip(1).ToArray();

            var chars = new char[values.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)values[i];
            }
            item.Value = new string(chars);
            Context.DataItems.Add(item);
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="item"></param>
        public void Visit(ThumbDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            // Ignore for now
        }

        /// <summary>
        /// Visit bool item.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(BoolDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            byte[] values = GetItemValue(item);
            item.Value = BitConverter.ToBoolean(values, 0);
            Context.DataItems.Add(item);
        }

        /// <summary>
        /// Note Supported
        /// </summary>
        /// <param name="item"></param>
        public void Visit(UserDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            // Ignore for now.
        }

        /// <summary>
        /// Visit cstring item.
        /// </summary>
        /// <param name="item"></param>
        public void Visit(CStringDataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            // 
            // Last byte is a null character.
            //

            byte[] values = GetItemValue(item);
            values = values.Take(values.Length - 1).ToArray();

            var chars = new char[values.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)values[i];
            }
            item.Value = new string(chars);
            Context.DataItems.Add(item);
        }

        #endregion

        #region Implementation of IVersionedDataParser

        /// <summary>
        /// Parser data.
        /// </summary>
        /// <param name="context"></param>
        public void ParseData(IParserContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (context.Header.MajorVersion != MajorVersion)
                throw new InvalidFileVersionException(MajorVersion, context.Header.MajorVersion);

            Context = context;

            Context.Header.DirectoryEntries.ForEach(
                entry =>
                    {
                        IAb1DataItem item = DataItemFactory.TryCreateDataItem(entry);
                        if (item == null) return;
                        item.Accept(this);
                    });
        }

        #endregion

        /// <summary>
        /// Returns the item value.  
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="InvalidItemSizeException">Thrown if the item size does not match the expected size.</exception>"
        /// <returns></returns>
        private byte[] GetItemValue(IAb1DataItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            //
            // Based on the Ab1 specifications, if a item size is greater than 4 bytes the offset points to the data
            // otherwise it contains the data.
            //

            byte[] result =
                item.Entry.DataSize <= Constants.MaxLocalItemByteSize
                    ? GetLocalItemValue(item)
                    : GetRemoteItemValue(item);

            ValidateExpectedSize(result, item);
            return result;
        }

        /// <summary>
        /// Returns the bytes containing the item value based on the data offset pointer and data size.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private byte[] GetRemoteItemValue(IAb1DataItem item)
        {
            Context.Reader.BaseStream.Position = item.Entry.DataOffset;
            byte[] result = Context.Reader.ReadBytes(item.Entry.DataSize);
            return result;
        }

        /// <summary>
        /// Returns the bytes containing the item value.  Note that the data offset field contains the bytes starting
        /// at the high-order byte of the 32 bit field.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static byte[] GetLocalItemValue(IAb1DataItem item)
        {
            var result = new byte[item.Entry.DataSize];

            for (int i = 0; i < item.Entry.DataSize; i++)
            {
                result[i] = (byte)(item.Entry.DataOffset >> (24 - 8 * i));
            }

            return result;
        }

        private static void ValidateExpectedSize(byte[] value, IAb1DataItem item)
        {
            int expectedSize = item.Size * item.Entry.ElementCount;
            if (expectedSize != value.Length)
                throw new InvalidItemSizeException(expectedSize, value.Length, item.Name);
        }

        #region Nested type: Constants

        private static class Constants
        {
            public const int MaxLocalItemByteSize = 4;
        }

        #endregion
    }
}
