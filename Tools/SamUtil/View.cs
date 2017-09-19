using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Bio.IO.BAM;
using Bio.IO.SAM;
using SamUtil.Properties;

namespace SamUtil
{
    /// <summary>
    /// Class for View option.
    /// </summary>
    public class View
    {
        #region Public Fields

        /// <summary>
        /// Output in BAM format.
        /// </summary>
        public bool BAMOutput;

        /// <summary>
        /// Print header with alignment.
        /// </summary>
        public bool Header;

        /// <summary>
        /// Print only header.
        /// </summary>
        public bool HeaderOnly;

        /// <summary>
        /// Input file is in SAM format.
        /// </summary>
        public bool SAMInput;

        /// <summary>
        /// Display uncompressed Bam file.
        /// </summary>
        public bool UnCompressedBAM;

        /// <summary>
        /// Display flag in HEX.
        /// </summary>
        public bool FlagInHex;

        /// <summary>
        /// Display flag as string.
        /// </summary>   
        public bool FlagAsString;

        /// <summary>
        /// Path of file containing reference name and length in tab delimited format.
        /// </summary>
        public string ReferenceNamesAndLength;

        /// <summary>
        /// Path of file containing reference sequence.
        /// </summary>
        public string ReferenceSequenceFile;

        /// <summary>
        /// Output file name
        /// </summary>
        public string OutputFilename;

        /// <summary>
        /// Only output alignments with all bits in INT present in the FLAG field. 
        /// </summary>
        public int FlagRequired;

        /// <summary>
        /// Skip alignments with bits present in INT.
        /// </summary>
        public int FilteringFlag;

        /// <summary>
        /// Skip alignments with MAPQ smaller than INT.
        /// </summary>
        public int QualityMinimumMapping;

        /// <summary>
        /// Only output reads in library STR.
        /// </summary>
        public string Library;

        /// <summary>
        /// Only output reads in read group STR.
        /// </summary>
        public string ReadGroup;

        ///<summary>
        ///If no region is specified, all the alignments will be printed; 
        ///otherwise only alignments overlapping the specified regions will be output. 
        ///</summary>
        public string Region;

        /// <summary>
        /// Path of input file.
        /// </summary>
        public string InputFilePath;

        /// <summary>
        /// Usage.
        /// </summary>
        public bool Help;

        #endregion

        #region Private Fields

        /// <summary>
        /// Stores Region Variable.
        /// </summary>
        private Region region;

        /// <summary>
        /// Stores information about RG present in header used for storing library information of the reads.
        /// </summary>
        private List<SAMRecordField> rgRecFields;

        /// <summary>
        /// Writes to Console or File based on user option.
        /// </summary>
        private TextWriter writer;

        /// <summary>
        /// Holds Uncompressed out put of BAM.
        /// </summary>
        private Stream bamUncompressedOutStream;

        /// <summary>
        /// Holds compressed out put of BAM.
        /// </summary>
        private Stream bamCompressedOutStream;

        /// <summary>
        /// holds bam formatter.
        /// </summary>
        private BAMFormatter bamformatter;

        /// <summary>
        /// holds bam parser;
        /// </summary>
        private BAMParser bamparser;

        /// <summary>
        /// holds max limit upto which memory stream can be used.
        /// 1GB = 1 * 1024 * 1024 * 1024.
        /// This limits the max memory utilization to ~1.25GB.
        /// </summary>
        private const long MemStreamLimit = 1 * 1024 * 1024 * 1024;

        /// <summary>
        /// Temp file path for uncompressed bam file.
        /// </summary>
        private string uncompressedTempfile = string.Empty;

        /// <summary>
        /// Temp file path for compressed bam file.
        /// </summary>
        private string compressedTempfile = string.Empty;
        #endregion

        #region Public Methods

        /// <summary>
        /// Extract/print all or sub alignments in SAM or BAM format.
        /// By default, this command assumes the file on the command line is in
        /// BAM format and it prints the alignments in SAM.
        /// SAMUtil.exe view in.bam
        /// </summary>
        public void ViewResult()
        {
            try
            {
                if (string.IsNullOrEmpty(InputFilePath))
                {
                    throw new InvalidOperationException("Input File Not specified");
                }

                if (!string.IsNullOrEmpty(Region))
                {
                    StringToRegionConverter();
                }

                Initialize();


                if (!SAMInput)
                {
                    this.ConvertFromBAMToSAM();
                }
                else
                {
                    this.ConvertFromSAMTOBAM();
                }
            }
            finally
            {
                Close();
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Converts the input BAM to SAM file format.
        /// </summary>
        private void ConvertFromBAMToSAM()
        {
            using (Stream stream = new FileStream(InputFilePath, FileMode.Open, FileAccess.Read))
            {
                SAMAlignmentHeader header = null;
                try
                {
                    header = bamparser.GetHeader(stream);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(Resources.InvalidBAMFile, ex);
                }

                WriteHeader(header);

                if (!HeaderOnly)
                {
                    if (!string.IsNullOrEmpty(Library))
                    {
                        rgRecFields = header.RecordFields.Where(R => R.Typecode.ToUpper().Equals("RG")).ToList();
                    }

                    foreach (SAMAlignedSequence alignedSequence in GetAlignedSequence(stream))
                    {
                        WriteAlignedSequence(header, alignedSequence);
                    }
                }
            }
        }

        /// <summary>
        /// Converts the input SAM to BAM file format.
        /// </summary>
        private void ConvertFromSAMTOBAM()
        {
            SAMAlignmentHeader header = null;
            try
            {
                using (var reader = new StreamReader(InputFilePath))
                    header = SAMParser.ParseSAMHeader(reader);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException(Resources.InvalidSAMFile,ex);
            }

            if (header == null)
            {
                Console.Error.WriteLine("Warning: SAM file doesn't contain header");
            }

            if (HeaderOnly)
            {
                if (header != null)
                {
                    WriteHeader(header);
                }
            }
            else
            {
                if (header == null)
                {
                    header = new SAMAlignmentHeader();
                }

                if (!string.IsNullOrEmpty(Library))
                {
                    rgRecFields = header.RecordFields.Where(R => R.Typecode.ToUpper().Equals("RG")).ToList();
                }

                if (!string.IsNullOrEmpty(ReferenceNamesAndLength))
                {
                    this.UpdateReferenceInformationFromFile(header);
                }
                else if (header.ReferenceSequences.Count == 0)
                {
                    this.UpdateReferenceInformationFromReads(header);
                }

                WriteHeader(header);
                using (StreamReader textReader = new StreamReader(InputFilePath))
                {
                    foreach (SAMAlignedSequence alignedSeq in GetAlignedSequence(textReader))
                    {
                        WriteAlignedSequence(header, alignedSeq);
                    }
                }
            }

            if (UnCompressedBAM)
            {
                bamUncompressedOutStream.Flush();
                if (writer != null)
                {
                    DisplayBAMContent(bamUncompressedOutStream);
                }
            }

            if (BAMOutput && !UnCompressedBAM)
            {
                bamUncompressedOutStream.Flush();
                bamUncompressedOutStream.Seek(0, SeekOrigin.Begin);
                bamformatter.CompressBAMFile(bamUncompressedOutStream, bamCompressedOutStream);
                bamCompressedOutStream.Flush();
                if (writer != null)
                {
                    DisplayBAMContent(bamCompressedOutStream);
                }
            }
        }

        /// <summary>
        /// Updates the header with reference name from reads in input file.
        /// </summary>
        /// <param name="header">SAM alignment header.</param>
        private void UpdateReferenceInformationFromReads(SAMAlignmentHeader header)
        {
            // If the ReferenceNamesAndLength file name is not specified and there is no @SQ header, 
            // then get the refernece names from read information.
            List<string> refSeqNames = new List<string>();
            using (StreamReader textReader = new StreamReader(InputFilePath))
            {
                foreach (SAMAlignedSequence alignedSeq in GetAlignedSequence(textReader))
                {
                    if (!alignedSeq.RName.Equals("*", StringComparison.OrdinalIgnoreCase)
                        && !refSeqNames.Contains(alignedSeq.RName, StringComparer.OrdinalIgnoreCase))
                    {
                        refSeqNames.Add(alignedSeq.RName);
                    }
                }
            }

            foreach (string refname in refSeqNames)
            {
                header.ReferenceSequences.Add(new ReferenceSequenceInfo(refname, 0));
            }
        }
        /// <summary>
        /// Updates the header with reference name and length from ReferenceNamesAndLength file.
        /// </summary>
        /// <param name="header">SAM alignment header.</param>
        private void UpdateReferenceInformationFromFile(SAMAlignmentHeader header)
        {
            header.ReferenceSequences.Clear();

            using (StreamReader reader = new StreamReader(ReferenceNamesAndLength))
            {
                header.ReferenceSequences.Clear();
                string read = reader.ReadLine();
                while (!string.IsNullOrEmpty(read))
                {
                    string[] splitRegion = read.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitRegion.Length > 1)
                    {
                        string name = splitRegion[0];
                        long len = long.Parse(splitRegion[1], CultureInfo.InvariantCulture);
                        header.ReferenceSequences.Add(new ReferenceSequenceInfo(name, len));
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid file for reference name and length");
                    }

                    read = reader.ReadLine();
                }
            }
        }

        /// <summary>
        ///  Initializes required parsers, formatters, input and output files based on user option.
        /// </summary>
        private void Initialize()
        {
            bamparser = new BAMParser();
            bamformatter = new BAMFormatter();

            bamUncompressedOutStream = null;
            bamCompressedOutStream = null;

            if (string.IsNullOrEmpty(OutputFilename))
            {
                writer = Console.Out;
            }
            else
            {
                if (UnCompressedBAM || BAMOutput)
                {
                    writer = null;

                    if (UnCompressedBAM)
                    {
                        bamUncompressedOutStream = new FileStream(OutputFilename, FileMode.Create, FileAccess.ReadWrite);
                    }
                    else
                    {
                        bamCompressedOutStream = new FileStream(OutputFilename, FileMode.Create, FileAccess.ReadWrite);
                    }
                }
                else
                {
                    writer = new StreamWriter(OutputFilename);
                }
            }

            #region Intialize temp files
            long inputfileSize = (new FileInfo(InputFilePath)).Length;
            long unCompressedSize = inputfileSize;

            if (!SAMInput)
            {
                unCompressedSize = inputfileSize * 4; // as uncompressed bam file will be Aprox 4 times that of the compressed file.
            }

            long compressedSize = unCompressedSize / 4;

            // uncompressed file is required for both uncompressed and compressed outputs.
            if ((UnCompressedBAM || BAMOutput) && bamUncompressedOutStream == null)
            {
                if (HeaderOnly || (MemStreamLimit >= unCompressedSize))
                {
                    bamUncompressedOutStream = new MemoryStream();
                }
                else
                {
                    uncompressedTempfile = Path.GetTempFileName();
                    bamUncompressedOutStream = new FileStream(uncompressedTempfile, FileMode.Open, FileAccess.ReadWrite);
                }
            }

            if (BAMOutput && !UnCompressedBAM && bamCompressedOutStream == null)
            {
                if (HeaderOnly || (MemStreamLimit >= compressedSize))
                {
                    bamCompressedOutStream = new MemoryStream((int)(inputfileSize));
                }
                else
                {
                    compressedTempfile = Path.GetTempFileName();
                    bamCompressedOutStream = new FileStream(compressedTempfile, FileMode.Open, FileAccess.ReadWrite);
                }
            }
            #endregion Intialize temp files
        }

        /// <summary>
        /// Displays pending data and closes all streams.
        /// 
        /// </summary>
        private void Close()
        {
            if (writer != null)
            {
                writer.Close();
            }

            if (bamCompressedOutStream != null)
            {
                bamCompressedOutStream.Close();
                bamCompressedOutStream = null;
            }

            if (bamUncompressedOutStream != null)
            {
                bamUncompressedOutStream.Close();
                bamUncompressedOutStream = null;
            }

            if (string.IsNullOrEmpty(uncompressedTempfile) && File.Exists(uncompressedTempfile))
            {
                File.Delete(uncompressedTempfile);
            }

            if (string.IsNullOrEmpty(compressedTempfile) && File.Exists(compressedTempfile))
            {
                File.Delete(compressedTempfile);
            }

            bamformatter = null;
            if (bamparser != null)
            {
                bamparser.Dispose();
                bamparser = null;
            }
        }

        /// <summary>
        /// Gets a value to indicate whether filter is required or not.
        /// </summary>
        private bool IsFilterApplied()
        {
            if (FlagRequired != 0 || FilteringFlag != 0
                || QualityMinimumMapping != 0 || !string.IsNullOrEmpty(Library)
                || !string.IsNullOrEmpty(ReadGroup) || !string.IsNullOrEmpty(Region))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes the header to output stream
        /// </summary>
        /// <param name="header"></param>
        private void WriteHeader(SAMAlignmentHeader header)
        {
            if (!Header && !HeaderOnly)
            {
                return;
            }

            if (UnCompressedBAM || BAMOutput)
            {
                // Incase of compressed bamoutput uncompressed file will be compressed before sending it to output stream.
                bamformatter.WriteHeader(header, bamUncompressedOutStream);
            }
            else
            {
                SAMFormatter.WriteHeader(writer, header);
            }
        }

        /// <summary>
        /// Writes aligned sequence to output stream.
        /// </summary>
        /// <param name="header">Alignment header.</param>
        /// <param name="alignedSequence">Aligned sequence to write.</param>
        private void WriteAlignedSequence(SAMAlignmentHeader header, SAMAlignedSequence alignedSequence)
        {
            if (UnCompressedBAM || BAMOutput)
            {
                // In case of compressed bamoutput uncompressed file will be compressed before sending it to output stream.
                bamformatter.WriteAlignedSequence(header, alignedSequence, bamUncompressedOutStream);
            }
            else
            {
                SAMFormatter.WriteSAMAlignedSequence(writer, alignedSequence);
            }
        }

        /// <summary>
        /// Gets Aligned sequences in the Specified BAM file.
        /// </summary>
        /// <param name="bamStream"></param>
        private IEnumerable<SAMAlignedSequence> GetAlignedSequence(Stream bamStream)
        {
            bool isFilterRequired = IsFilterApplied();
            bool display = true;

            while (!bamparser.IsEOF())
            {
                SAMAlignedSequence alignedSequence = bamparser.GetAlignedSequence(false);
                //TODO: The parser should probably never return a null sequence
                //this may be a band aid over a lurking problem, fix in future
                if (alignedSequence != null)
                {
                    if (isFilterRequired)
                    {
                        display = Filter(alignedSequence);
                    }

                    if (display)
                    {
                        yield return alignedSequence;
                    }
                }
            }
        }

        /// <summary>
        /// Gets Aligned sequences in the Specified SAM file.
        /// </summary>
        /// <param name="textReader">SAM file stream.</param>
        private IEnumerable<SAMAlignedSequence> GetAlignedSequence(TextReader textReader)
        {
            bool isFilterRequired = IsFilterApplied();
            bool display = true;

            //Displays SAM as output.
            string line = ReadNextLine(textReader);
            while (line != null)
            {
                // Ignore headers.
                if (!line.StartsWith(@"@", StringComparison.OrdinalIgnoreCase))
                {
                    SAMAlignedSequence alignedSequence = SAMParser.ParseSequence(line);
                    if (isFilterRequired)
                    {
                        display = Filter(alignedSequence);
                    }

                    if (display)
                    {
                        yield return alignedSequence;
                    }
                }

                line = ReadNextLine(textReader);
            }
        }

        /// <summary>
        /// Reads next line considering
        /// </summary>
        /// <returns></returns>
        private static string ReadNextLine(TextReader reader)
        {
            if (reader.Peek() == -1)
            {
                //line = null;
                return null;
            }

            var line = reader.ReadLine();
            while (string.IsNullOrWhiteSpace(line) && reader.Peek() != -1)
            {
                line = reader.ReadLine();
            }

            return line;
        }

        /// <summary>
        /// Displays the bam content to Console.
        /// </summary>
        /// <param name="stream">BAM stream</param>
        private void DisplayBAMContent(Stream stream)
        {
            int blockSizeToRead = 4096;
            stream.Seek(0, SeekOrigin.Begin);

            byte[] bytes = new byte[blockSizeToRead];
            int bytesRead = 0;
            for (int i = 0; i < stream.Length / blockSizeToRead; i++)
            {
                bytesRead = stream.Read(bytes, 0, (int)stream.Length);

                if (bytesRead > 0)
                {
                    string str = System.Text.ASCIIEncoding.ASCII.GetString(bytes, 0, bytesRead);
                    writer.Write(str);
                }
            }

            if (stream.Position < (stream.Length - 1))
            {
                bytesRead = stream.Read(bytes, 0, (int)stream.Length);
                if (bytesRead > 0)
                {
                    string str = System.Text.ASCIIEncoding.ASCII.GetString(bytes, 0, bytesRead);
                    writer.Write(str);
                }
            }

            writer.Flush();
        }

        /// <summary>
        /// Converts Flag to string.
        /// In a string FLAG, each character represents one bit with
        ///p=0x1 (paired), P=0x2 (properly paired), u=0x4 (unmapped),
        ///U=0x8 (mate unmapped), r=0x10 (reverse), R=0x20 (mate reverse)
        ///1=0x40 (first), 2=0x80 (second), s=0x100 (not primary),
        ///f=0x200 (failure) and d=0x400 (duplicate). 
        /// </summary>
        /// <param name="flag">Sequence Flag.</param>
        /// <returns>String of flag.</returns>
        private string GetFlagDesc(SAMFlags flag)
        {
            string str = string.Empty;
            if ((flag & SAMFlags.PairedRead) == SAMFlags.PairedRead)
            {
                str = str + "p";
            }


            if ((flag & SAMFlags.MappedInProperPair) == SAMFlags.MappedInProperPair)
            {
                str = str + "P";
            }

            if ((flag & SAMFlags.UnmappedQuery) == SAMFlags.UnmappedQuery)
            {
                str = str + "u";
            }

            if ((flag & SAMFlags.UnmappedMate) == SAMFlags.UnmappedMate)
            {
                str = str + "U";
            }

            if ((flag & SAMFlags.QueryOnReverseStrand) == SAMFlags.QueryOnReverseStrand)
            {
                str = str + "r";
            }

            if ((flag & SAMFlags.MateOnReverseStrand) == SAMFlags.MateOnReverseStrand)
            {
                str = str + "R";
            }

            if ((flag & SAMFlags.FirstReadInPair) == SAMFlags.FirstReadInPair)
            {
                str = str + "1";
            }

            if ((flag & SAMFlags.SecondReadInPair) == SAMFlags.SecondReadInPair)
            {
                str = str + "2";
            }

            if ((flag & SAMFlags.NonPrimeAlignment) == SAMFlags.NonPrimeAlignment)
            {
                str = str + "s";
            }

            if ((flag & SAMFlags.QualityCheckFailure)
                == SAMFlags.QualityCheckFailure)
            {
                str = str + "f";
            }

            if ((flag & SAMFlags.Duplicate) == SAMFlags.Duplicate)
            {
                str = str + "d";
            }

            return str;
        }

        /// <summary>
        /// Filters Sequence based on user inputs.
        /// </summary>
        /// <param name="alignedSequence">Aligned Sequence.</param>
        /// <returns>Whether aligned sequence matches user defined options.</returns>
        private bool Filter(SAMAlignedSequence alignedSequence)
        {
            bool filter = true;
            if (filter && FlagRequired != 0)
            {
                filter = (((int)alignedSequence.Flag) & FlagRequired) == FlagRequired;
            }

            if (filter && FilteringFlag != 0)
            {
                filter = ((((int)alignedSequence.Flag) & FilteringFlag) == 0);
            }

            if (filter && QualityMinimumMapping != 0)
            {
                filter = alignedSequence.MapQ == QualityMinimumMapping;
            }

            if (filter && !string.IsNullOrEmpty(Library) && rgRecFields.Count > 0)
            {
                filter = rgRecFields.First(
                        a => a.Tags.First(
                        b => b.Tag.Equals("ID")).Value.Equals(alignedSequence.OptionalFields.First(
                        c => c.Tag.Equals("RG")).Value)).Tags.First(
                        d => d.Tag.Equals("LB")).Value.Equals(Library);
            }

            if (filter && !string.IsNullOrEmpty(ReadGroup))
            {
                filter = alignedSequence.OptionalFields.AsParallel().Where(
                   O => O.Tag.ToUpper().Equals("RG")).ToList().Any(a => a.Value.Equals(ReadGroup));
            }

            if (filter && !string.IsNullOrEmpty(Region))
            {
                if (alignedSequence.RName.Equals(region.Chromosome))
                {
                    if (region.Start > -1)
                    {
                        if (alignedSequence.Pos >= region.Start)
                        {
                            if (region.End > -1)
                            {
                                if (alignedSequence.Pos <= region.End)
                                {
                                    filter = true;
                                }
                                else
                                {
                                    filter = false;
                                }
                            }
                            else
                            {
                                filter = true;
                            }
                        }
                        else
                        {
                            filter = false;
                        }
                    }
                    else
                    {
                        filter = true;
                    }
                }
                else
                {
                    filter = false;
                }
            }

            return filter;
        }

        /// <summary>
        /// Converts region passed as command line argument to Region strucure.
        /// </summary>
        /// <param name="region">String passed as command line argument.</param>
        /// <returns>Region structure.</returns>
        private void StringToRegionConverter()
        {
            string[] splitRegion = Region.Split(new char[] { ':', '-' });
            if (splitRegion.Length == 1)
            {
                region = new Region()
                {
                    Chromosome = splitRegion[0],
                    Start = -1,
                    End = -1
                };
            }
            else if (splitRegion.Length == 2)
            {
                region = new Region()
                {
                    Chromosome = splitRegion[0],
                    Start = uint.Parse(splitRegion[1], CultureInfo.InvariantCulture),
                    End = -1
                };
            }
            else if (splitRegion.Length == 3)
            {
                region = new Region()
                {
                    Chromosome = splitRegion[0],
                    Start = uint.Parse(splitRegion[1], CultureInfo.InvariantCulture),
                    End = uint.Parse(splitRegion[2], CultureInfo.InvariantCulture)
                };
            }
            else
            {
                throw new InvalidOperationException("Region cannot be parsed");
            }
        }

        #endregion
    }

    #region Public Structures

    /// <summary>
    /// An alignment may be given multiple times if it is overlapping several regions. 
    /// A region can be presented, for example, in the following format: 
    /// ‘chr2’ (the whole chr2), 
    /// ‘chr2:1000000’ (region starting from 1,000,000bp) 
    /// or ‘chr2:1,000,000-2,000,000’ (region between 1,000,000 and 2,000,000bp including the end points). 
    /// The coordinate is 1-based. 
    /// </summary>
    public struct Region
    {
        /// <summary>
        /// Chromosome Number.
        /// </summary>
        public string Chromosome { get; set; }

        /// <summary>
        /// Start position of alignment.
        /// </summary>
        public long Start { get; set; }

        /// <summary>
        /// End position of alignment.
        /// </summary>
        public long End { get; set; }
    }

    #endregion
}
