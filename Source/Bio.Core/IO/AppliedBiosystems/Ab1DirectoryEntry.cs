using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Bio.IO.AppliedBiosystems
{
    /// <summary>
    /// A directory entry is a section of bytes that identify an element of metadata within an abi file.  The structure of this entry is defined
    /// in: http://www6.appliedbiosystems.com/support/software_community/ABIF_File_Format.pdf
    /// </summary>
    public sealed class Ab1DirectoryEntry
    {
        //private const string Key = "Ab1DirectoryEntry";

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public Ab1DirectoryEntry()
        {
        }

        /// <summary>
        /// Reads an entry from the buffer.
        /// </summary>
        /// <param name="buffer"></param>
        public Ab1DirectoryEntry(byte[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            Initialize(buffer);
        }

        /// <summary>
        /// Raw data associated with this entry.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public byte[] Buffer { get; set; }

        /// <summary>
        /// Number of bytes in the file this entry is offset.  This is only valid for
        /// items of size greater than 4 bytes.
        /// </summary>
        public Int32 DataOffset { get; set; }

        /// <summary>
        /// Size in bytes of the entry.
        /// </summary>
        public Int32 DataSize { get; set; }

        /// <summary>
        /// The number of elements in the directory entry.
        /// </summary>
        public Int32 ElementCount { get; set; }

        /// <summary>
        /// The size of each element.
        /// </summary>
        public Int16 ElementSize { get; set; }

        /// <summary>
        /// Identifies the type of element.
        /// </summary>
        public Int16 ElementTypeCode { get; set; }

        /// <summary>
        /// Data handle - this is not used and is reserved.
        /// </summary>
        public Int32 Reserved { get; set; }

        /// <summary>
        /// Name of the element.
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// An arbitrary number associated with the directory entry.  It is customary for this
        /// value to be less than 1000.
        /// </summary>
        public Int32 TagNumber { get; set; }

#if FALSE
        /// <summary>
        /// Serializes the object data.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException("info");
            var objectBuffer = new byte[28];
            using (var stream = new MemoryStream(objectBuffer))
            {
                var writer = new BinaryWriter(stream);
                for (int i = 0; i < TagName.Length; i++)
                    writer.Write((byte)TagName[i]);

                writer.Write(TagNumber);
                writer.Write(ElementTypeCode);
                writer.Write(ElementSize);
                writer.Write(ElementCount);
                writer.Write(DataSize);
                writer.Write(DataOffset);
                writer.Write(Reserved);
                writer.Flush();
            }

            info.AddValue(Key, objectBuffer);
        }
#endif

        private void Initialize(byte[] directoryBuffer)
        {
            if (directoryBuffer == null) throw new ArgumentNullException("directoryBuffer");
            Buffer = directoryBuffer;
            using (var stream = new MemoryStream(directoryBuffer.Length))
            {
                var writer = new BinaryWriter(stream);
                writer.Write(directoryBuffer);
                writer.Flush();
                stream.Position = 0;

                var reader = new BinaryReader(stream);

                TagName = new string(reader.ReadChars(4));
                TagNumber = (reader.ReadByte() << 24 | reader.ReadByte() << 16 | reader.ReadByte() << 8 |
                             reader.ReadByte());
                ElementTypeCode = (short)(reader.ReadByte() << 8 | reader.ReadByte());
                ElementSize = (short)(reader.ReadByte() << 8 | reader.ReadByte());
                ElementCount = (reader.ReadByte() << 24 | reader.ReadByte() << 16 | reader.ReadByte() << 8 |
                                reader.ReadByte());
                DataSize = (reader.ReadByte() << 24 | reader.ReadByte() << 16 | reader.ReadByte() << 8 |
                            reader.ReadByte());
                DataOffset = (reader.ReadByte() << 24 | reader.ReadByte() << 16 | reader.ReadByte() << 8 |
                              reader.ReadByte());
                Reserved = (reader.ReadByte() << 24 | reader.ReadByte() << 16 | reader.ReadByte() << 8 |
                            reader.ReadByte());
            }
        }
    }
}
