using System;
using System.Collections.Generic;
using System.IO;

using Bio.IO.SFF;
using Bio.Properties;
using Bio.Registration;

[assembly: BioRegister(typeof(SFFParser))]

namespace Bio.IO.SFF
{
    /// <summary>
    /// This class parses the Standard Flowgram Format (SFF) as defined by
    /// http://www.ncbi.nlm.nih.gov/Traces/trace.cgi?cmd=show&amp;f=formats&amp;m=doc&amp;s=format
    /// and returns in-memory ISequence objects with both sequence information and quality data in the Sanger format.
    /// SFF was designed by 454 Life Sciences (Roche), the Whitehead Institute for Biomedical Research and the Wellcome Trust Sanger Institute. 
    /// </summary>
    public sealed class SFFParser : ISequenceParser
    {
        SffHeader parsedHeader;
        int lastIndex;

        /// <summary>
        /// The header information included in the SFF file format.
        /// </summary>
        class SffHeader
        {
            public uint Version { get; set; }
            public ushort Length { get; set; }
            public ulong IndexOffset { get; set; }
            public uint IndexLength { get; set; }
            public uint NumberOfReads { get; set; }
            public ushort KeyLength { get; set; }
            public ushort NumberOfFlowsPerRead { get; set; }
            public byte FlowgramFormatCode { get; set; }
            public char[] FlowChars { get; set; }
            public string KeySequence { get; set; }
        }

        /// <summary>
        /// Description of the parser
        /// </summary>
        public string Description
        {
            get { return Resource.SFF_Description; }
        }

        /// <summary>
        /// Name of the parser
        /// </summary>
        public string Name
        {
            get { return Resource.SFF_Name; }
        }

        /// <summary>
        /// Supported file formats (extensions) for this parser
        /// </summary>
        public string SupportedFileTypes
        {
            get { return Resource.SFF_Extensions; }
        }

        /// <summary>
        /// The alphabet to use for sequences (automatically set if not supplied)
        /// </summary>
        public IAlphabet Alphabet { get; set; }

        /// <summary>
        /// Parse a sequence out of a stream.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <returns>Sequence</returns>
        public ISequence ParseOne(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream, AsciiEncoding.Default))
            {
                if (parsedHeader == null)
                {
                    parsedHeader = ParseHeader(reader);
                    lastIndex = 0;
                }

                if (parsedHeader.NumberOfReads > lastIndex)
                {
                    lastIndex++;
                    return ParseOne(parsedHeader, reader);
                }
            }

            parsedHeader = null;
            lastIndex = 0;
            return null;
        }


        /// <summary>
        /// Parses the sequences out of a stream.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <returns>Set of sequences</returns>
        public IEnumerable<ISequence> Parse(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream, AsciiEncoding.Default))
            {
                parsedHeader = ParseHeader(reader);
                for (lastIndex = 0; lastIndex < parsedHeader.NumberOfReads; lastIndex++)
                {
                    yield return ParseOne(parsedHeader, reader);
                }
            }

            parsedHeader = null;
            lastIndex = 0;
        }

        /// <summary>
        /// Parses a single sequence from the file.
        /// </summary>
        /// <param name="header">Parsed header</param>
        /// <param name="reader">Binary reader</param>
        /// <returns>Sequence</returns>
        private ISequence ParseOne(SffHeader header, BinaryReader reader)
        {
            // Parse out the read header.
            ushort headerLength = C2BE(reader.ReadUInt16());
            ushort nameLength = C2BE(reader.ReadUInt16());
            uint numberOfBases = C2BE(reader.ReadUInt32());

            // TODO: use clipping data
            ushort clipQualityLeft = C2BE(reader.ReadUInt16());
            ushort clipQualityRight = C2BE(reader.ReadUInt16());
            ushort clipAdapterLeft = C2BE(reader.ReadUInt16());
            ushort clipAdapterRight = C2BE(reader.ReadUInt16());

            string name = new string(reader.ReadChars(nameLength));

            long paddingSize = headerLength - (16 + nameLength);
            if (paddingSize < 0 || paddingSize > 8)
                throw new Exception("Invalid read header size found.");
            if (paddingSize > 0)
            {
                if (reader.Read(new char[8], 0, (int)paddingSize) != paddingSize)
                    throw new Exception("Could not parse read header (padding).");
            }

            // Parse out the read data section
            ushort[] flowgramValues = new ushort[header.NumberOfFlowsPerRead];
            for (int flowCount = 0; flowCount < header.NumberOfFlowsPerRead; flowCount++)
                flowgramValues[flowCount] = C2BE(reader.ReadUInt16());

            byte[] flowIndexPerBase = new byte[numberOfBases];
            if (reader.Read(flowIndexPerBase, 0, (int)numberOfBases) != numberOfBases)
                throw new Exception("Unable to read flow indexes.");

            byte[] bases = new byte[numberOfBases];
            if (reader.Read(bases, 0, (int)numberOfBases) != numberOfBases)
                throw new Exception("Unable to read base information.");

            byte[] qscores = new byte[numberOfBases];
            if (reader.Read(qscores, 0, (int)numberOfBases) != numberOfBases)
                throw new Exception("Unable to read quality scores.");
            for (int i = 0; i < qscores.Length; i++)
                qscores[i] += 33; // adjust for Sanger
                
            // Adjust for 8-byte padding at end of read segment
            long currentSize = header.NumberOfFlowsPerRead*2 + 3*numberOfBases;
            if ((currentSize & 7) > 0)
            {
                paddingSize = (((currentSize >> 3) + 1) << 3) - currentSize;
                if (paddingSize < 0 || paddingSize > 8)
                    throw new Exception("Invalid read data size found.");
                    if (paddingSize > 0)
                    {
                        if (reader.Read(new char[8], 0, (int)paddingSize) != paddingSize)
                            throw new Exception("Could not parse read header (padding).");
                    }
            }

            // Determine the alphabet.
            var alphabet = Alphabet 
                ?? Alphabets.AutoDetectAlphabet(bases, 0, bases.Length, null)
                ?? Alphabets.AmbiguousDNA;

            // Return our qSequence
            return new QualitativeSequence(alphabet, FastQFormatType.Sanger,  bases, qscores, false) { ID = name };
        }

        /// <summary>
        /// Parses out the header from the file
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private SffHeader ParseHeader(BinaryReader reader)
        {
            // Fixed header block
            //   magic_number               uint32      +4 = 0-3
            //   version                    char[4]     +4 = 4-7
            //   index_offset               uint64      +8 = 8-15
            //   index_length               uint32      +4 = 16-19
            //   number_of_reads            uint32      +4 = 20-23
            //   header_length              uint16      +2 = 24-25
            //   key_length                 uint16      +2 = 26-27
            //   number_of_flows_per_read   uint16      +2 = 28-29
            //   flowgram_format_code       uint8       +1 = 30
            const uint magicNumber = 0x2e736666;  // ".sff"
            const int fixedHeaderSize = 31;

            // First, parse out the header to verify the file format.
            // Check the magic string
            if (C2BE(reader.ReadUInt32()) != magicNumber)
                throw new Exception("Missing .sff magic number - cannot parse.");

            // Parse out all the fixed header elements.
            var header = new SffHeader
            {
                Version = C2BE(reader.ReadUInt32()),
                IndexOffset = C2BE(reader.ReadUInt64()),
                IndexLength = C2BE(reader.ReadUInt32()),
                NumberOfReads = C2BE(reader.ReadUInt32()),
                Length = C2BE(reader.ReadUInt16()),
                KeyLength = C2BE(reader.ReadUInt16()),
                NumberOfFlowsPerRead = C2BE(reader.ReadUInt16()),
                FlowgramFormatCode = reader.ReadByte()
            };

            if (header.Version != 1)
                throw new Exception("Unexpected SFF version number (" + header.Version + "), can only parse V1");
            if (header.FlowgramFormatCode != 1)
                throw new Exception("Unexpected SFF flowgram format (" + header.FlowgramFormatCode +
                                    "), only recognize format 1.");

            // Grab the flow characters
            header.FlowChars = new char[header.NumberOfFlowsPerRead];
            int readBytes = reader.Read(header.FlowChars, 0, header.NumberOfFlowsPerRead);
            if (readBytes != header.NumberOfFlowsPerRead)
                throw new Exception("Could not parse header (flow_chars).");

            char[] keySequence = new char[header.KeyLength];
            readBytes = reader.Read(keySequence, 0, header.KeyLength);
            if (readBytes != header.KeyLength)
                throw new Exception("Could not parse header (key_sequence).");

            header.KeySequence = new string(keySequence);

            // Calculate the padding
            int paddingSize = header.Length - (fixedHeaderSize + header.NumberOfFlowsPerRead + header.KeyLength);
            if (paddingSize < 0 || paddingSize > 8)
                throw new Exception("Invalid header size found.");
            if (paddingSize > 0)
            {
                if (reader.Read(new char[8], 0, paddingSize) != paddingSize)
                    throw new Exception("Could not parse header (padding).");
            }

            return header;
        }

        /// <summary>
        /// Method to convert to BigEndian for a ushort
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private ushort C2BE(ushort value)
        {
            return (!BitConverter.IsLittleEndian)
                ? value
                : (ushort)(((value & 0xff) << 8) | ((value & 0xff00) >> 8));
        }

        /// <summary>
        /// Method to convert to BigEndian for a uint
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private uint C2BE(uint value)
        {
            return (!BitConverter.IsLittleEndian)
                ? value
                : (((value & 0xff) << 24) | ((value & 0xff00) << 8) |
                    ((value & 0xff0000) >> 8) | ((value & 0xff000000) >> 24));
        }

        /// <summary>
        /// Method to convert to BigEndian for a ulong
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private ulong C2BE(ulong value)
        {
            return (!BitConverter.IsLittleEndian)
                        ? value
                        : (value & 0x00000000000000FFUL) << 56 |
                            (value & 0x000000000000FF00UL) << 40 |
                            (value & 0x0000000000FF0000UL) << 24 |
                            (value & 0x00000000FF000000UL) << 8 |
                            (value & 0x000000FF00000000UL) >> 8 |
                            (value & 0x0000FF0000000000UL) >> 24 |
                            (value & 0x00FF000000000000UL) >> 40 |
                            (value & 0xFF00000000000000UL);
        }
    }
}
