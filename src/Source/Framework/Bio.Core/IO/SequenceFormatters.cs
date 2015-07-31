using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bio.IO.FastA;
using Bio.IO.FastQ;
using Bio.IO.GenBank;
using Bio.IO.Gff;
using Bio.Registration;
using Bio.Util;

namespace Bio.IO
{
    /// <summary>
    /// SequenceFormatter class is an abstraction class which provides instances
    /// and lists of all Formatter currently supported by Bio.
    /// </summary>
    public static class SequenceFormatters
    {
        /// <summary>
        /// A singleton instance of FastaFormatter class which is capable of
        /// saving a ISequence according to the FASTA file format.
        /// </summary>
        private static readonly FastAFormatter fasta = new FastAFormatter();

        /// <summary>
        /// A singleton instance of FastQFormatter class which is capable of
        /// saving a QualitativeSequence according to the FASTQ file format.
        /// </summary>
        private static readonly FastQFormatter fastq = new FastQFormatter();

        /// <summary>
        /// A singleton instance of GffFormatter class which is capable of
        /// saving an ISequence which contains the metadata of GFF according to the GFF file format.
        /// </summary>
        private static readonly GffFormatter gff = new GffFormatter();

        /// <summary>
        /// A singleton instance of GenBankFormatter class which is capable of
        /// saving an ISequence which contains the metadata of GenBank according to the GenBank file format.
        /// </summary>
        private static readonly GenBankFormatter genBank = new GenBankFormatter();

        /// <summary>
        /// List of all supported sequence formatters.
        /// </summary>
        private static readonly List<ISequenceFormatter> allFormatters = new List<ISequenceFormatter>() 
        { 
            fasta,
            fastq,
            gff,
            genBank
        };

        /// <summary>
        /// Initializes static members of the SequenceFormatters class.
        /// </summary>
        static SequenceFormatters()
		{
            // get the registered formatter
            IEnumerable<ISequenceFormatter> registeredFormatters = GetSequenceFormatters();
            if (null != registeredFormatters)
            {
                foreach (ISequenceFormatter formatter in registeredFormatters.Where(
                    formatter => formatter != null && 
                        allFormatters.All(rfm => string.Compare(rfm.Name, formatter.Name, StringComparison.OrdinalIgnoreCase) != 0)))
                {
                    allFormatters.Add(formatter);
                }
            }
        }

        /// <summary>
        /// Gets an instance of FastaFormatter class which is capable of
        /// saving a ISequence according to the FASTA file format.
        /// </summary>
        public static FastAFormatter Fasta
        {
            get
            {
                return fasta;
            }
        }

        /// <summary>
        /// Gets an instance of FastQFormatter class which is capable of
        /// saving a IQualitativeSequence according to the FASTQ file format.
        /// </summary>
        public static FastQFormatter FastQ
        {
            get
            {
                return fastq;
            }
        }

        /// <summary>
        /// Gets an instance of GffFormatter class which is capable of
        /// saving a ISequence which contains the metadata of gff according to the GFF file format.
        /// </summary>
        public static GffFormatter Gff
        {
            get
            {
                return gff;
            }
        }

        /// <summary>
        /// Gets an instance of GenBankFormatter class which is capable of
        /// saving a ISequence which contains the metadata of GenBank according to the GenBank file format.
        /// </summary>
        public static GenBankFormatter GenBank
        {
            get
            {
                return genBank;
            }
        }

        /// <summary>
        /// Gets the list of all formatters which is supported by the framework.
        /// </summary>
        public static IReadOnlyList<ISequenceFormatter> All
        {
            get
            {
                return allFormatters;
            }
        }

        /// <summary>
        /// Returns formatter which supports the specified file.
        /// </summary>
        /// <param name="fileName">File name for which the formatter is required.</param>
        /// <returns>If found returns the formatter as ISequenceFormatter else returns null.</returns>
        public static ISequenceFormatter FindFormatterByFileName(string fileName)
        {
            ISequenceFormatter formatter = null;

            if (!string.IsNullOrEmpty(fileName))
            {
                if (IsFasta(fileName))
                {
                    formatter = fasta;
                }
                else if (IsFastQ(fileName))
                {
                    formatter = fastq;
                }
                else if (IsGenBank(fileName))
                {
                    formatter = genBank;
                }
                else if (fileName.EndsWith(Properties.Resource.GFF_FILEEXTENSION, StringComparison.OrdinalIgnoreCase))
                {
                    formatter = gff;
                }
                else
                {
                    // Do a search through the known formatters to pick up custom formatters added through add-in.
                    string fileExtension = Path.GetExtension(fileName);
                    if (!string.IsNullOrEmpty(fileExtension))
                    {
                        formatter = All.FirstOrDefault(p => p.SupportedFileTypes.Contains(fileExtension));
                    }
                }
            }

            return formatter;
        }

        /// <summary>
        /// Returns parser which supports the specified file.
        /// </summary>
        /// <param name="fileName">File name for which the parser is required.</param>
        /// <param name="formatterName">Name of the formatter to use.</param>
        /// <returns>If found returns the formatter as ISequenceFormatter else returns null.</returns>
        public static ISequenceFormatter FindFormatterByName(string fileName, string formatterName)
        {
            ISequenceFormatter formatter = null;

            if (!string.IsNullOrEmpty(fileName))
            {
                if (formatterName == Properties.Resource.FastAName)
                {
                    formatter = fasta;
                }
                else if (formatterName == Properties.Resource.FastQName)
                {
                    formatter = fastq;
                }
                else if (formatterName == Properties.Resource.GENBANK_NAME)
                {
                    formatter = genBank;
                }
                else if (formatterName == Properties.Resource.GFF_NAME)
                {
                    formatter = gff;
                }
                else
                {
                    // Do a search through the known formatters to pick up custom formatters added through add-in.
                    formatter = All.FirstOrDefault(p => p.Name == formatterName);
                }
            }

            return formatter;
        }

        /// <summary>
        /// Identifies if a file extension is a
        /// valid extension for FASTA formats.
        /// </summary>
        /// <returns>
        /// True  : if it is a valid fasta file extension.
        /// False : if it is a in-valid fasta file extension.
        /// </returns>
        public static bool IsFasta(string fileName)
        {
            return Helper.IsFasta(fileName);
        }

        /// <summary>
        /// Identifies if a file extension is a
        /// valid extension for FastQ formats.
        /// </summary>
        /// <returns>
        /// True  : if it is a valid fastq file extension.
        /// False : if it is a in-valid fastq file extension.
        /// </returns>
        public static bool IsFastQ(string fileName)
        {
            return Helper.IsFastQ(fileName);
        }

        /// <summary>
        /// Identifies if a file extension is a
        /// valid extension for GenBnak formats.
        /// </summary>
        /// <returns>
        /// True  : if it is a valid GenBank file extension.
        /// False : if it is a in-valid GenBank file extension.
        /// </returns>
        public static bool IsGenBank(string fileName)
        {
            return Helper.IsGenBank(fileName);
        }

		/// <summary>
        /// Gets all registered formatters in core folder and addins (optional) folders.
        /// </summary>
        /// <returns>List of registered formatters.</returns>
        private static IEnumerable<ISequenceFormatter> GetSequenceFormatters()
        {
            var registeredFormatters = new List<ISequenceFormatter>();
            var implementations = BioRegistrationService.LocateRegisteredParts<ISequenceFormatter>();

            foreach (var impl in implementations)
            {
                try
                {
                    ISequenceFormatter formatter = Activator.CreateInstance(impl) as ISequenceFormatter;
                    if (formatter != null)
                        registeredFormatters.Add(formatter);
                }
                catch
                {
                    // Cannot create - no default ctor?
                }
            }

            return registeredFormatters;
        }
    }
}
