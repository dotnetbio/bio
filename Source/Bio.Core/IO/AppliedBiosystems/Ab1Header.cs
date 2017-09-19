using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Bio.IO.AppliedBiosystems.Exceptions;

namespace Bio.IO.AppliedBiosystems
{
    /// <summary>
    /// The header contains information about the abi file.  This should always have the same format regardless of file version and so should
    /// be read first and then an appropriate parser chosen based on the file version.
    /// </summary>
    public class Ab1Header
    {
        /// <summary>
        /// Creates a new header and loads it from the specified stream.
        /// </summary>
        /// <param name="reader"></param>
        public Ab1Header(BinaryReader reader)
        {
            ValidateHeader(reader);
            ReadDirectories(reader);
        }

        /// <summary>
        /// Identifies the file as an abi file.  These are the first four bytes and should be ABIF.
        /// </summary>
        public string FileSignature { get; set; }

        /// <summary>
        /// Version of the abi file.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Major version pulled from the <see cref="Version"/> property.
        /// </summary>
        public int MajorVersion { get; set; }

        /// <summary>
        /// The first directory entry found after the header.  This defines the following content of the file.
        /// </summary>
        public Ab1DirectoryEntry DirectoryEntryDefinition { get; set; }

        /// <summary>
        /// Contains a list of all entries found within the file.  This does not include the initial header directory.
        /// </summary>
        public ReadOnlyCollection<Ab1DirectoryEntry> DirectoryEntries { get; private set; }

        /// <summary>
        /// Reads all directories entries based on the intiial directory entry header <see cref="DirectoryEntryDefinition"/>.
        /// </summary>
        /// <param name="reader"></param>
        private void ReadDirectories(BinaryReader reader)
        {
            reader.BaseStream.Position = DirectoryEntryDefinition.DataOffset;

            var entries = new List<Ab1DirectoryEntry>();
            for (int i = 0; i < DirectoryEntryDefinition.ElementCount; i++)
            {
                byte[] buffer = reader.ReadBytes(28);
                entries.Add(new Ab1DirectoryEntry(buffer));
            }
            DirectoryEntries = new ReadOnlyCollection<Ab1DirectoryEntry>(entries);
        }

        /// <summary>
        /// Validates the header matches this parser. 
        /// </summary>
        /// <param name="reader"></param>
        private void ValidateHeader(BinaryReader reader)
        {
            ValidateFileSignature(reader.ReadBytes(4));
            ValidateFileVersion(reader.ReadBytes(2));
            ReadDirectoryStructure(reader.ReadBytes(28));
        }

        /// <summary>
        /// Reads the definition directory entry.
        /// </summary>
        /// <param name="data"></param>
        private void ReadDirectoryStructure(byte[] data)
        {
            DirectoryEntryDefinition = new Ab1DirectoryEntry(data);
        }

        /// <summary>
        /// Validate the file version.
        /// </summary>
        /// <param name="data"></param>
        private void ValidateFileVersion(byte[] data)
        {
            int version = data[0] << 8 | data[1];
            int majorVersion = version / 100;
            if (majorVersion != Constants.MajorVersion)
                throw new InvalidFileVersionException(Constants.MajorVersion, majorVersion);

            Version = version;
            MajorVersion = majorVersion;
        }

        /// <summary>
        /// Validates the file signature.  Ensures this is an abi file.
        /// </summary>
        /// <param name="data"></param>
        private void ValidateFileSignature(byte[] data)
        {
            string value = (((char)data[0]).ToString()
                            + ((char)data[1])
                            + ((char)data[2])
                            + ((char)data[3])).ToUpperInvariant();
            if (value != Constants.FileSignature)
                throw new InvalidFileSignatureException(Constants.FileSignature, value);

            FileSignature = value;
        }

        #region Nested type: Constants

        private static class Constants
        {
            public const string FileSignature = "ABIF";
            public const int MajorVersion = 1;
        }

        #endregion
    }
}
