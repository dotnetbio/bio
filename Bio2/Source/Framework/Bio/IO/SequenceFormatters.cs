using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bio.IO.FastA;
using Bio.IO.FastQ;
using Bio.IO.GenBank;
using Bio.IO.Gff;
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
        private static FastAFormatter fasta = new FastAFormatter();

        /// <summary>
        /// A singleton instance of FastQFormatter class which is capable of
        /// saving a QualitativeSequence according to the FASTQ file format.
        /// </summary>
        private static FastQFormatter fastq = new FastQFormatter();

        /// <summary>
        /// A singleton instance of GffFormatter class which is capable of
        /// saving an ISequence which contains the metadata of GFF according to the GFF file format.
        /// </summary>
        private static GffFormatter gff = new GffFormatter();

        /// <summary>
        /// A singleton instance of GenBankFormatter class which is capable of
        /// saving an ISequence which contains the metadata of GenBank according to the GenBank file format.
        /// </summary>
        private static GenBankFormatter genBank = new GenBankFormatter();

        /// <summary>
        /// List of all supported sequence formatters.
        /// </summary>
        private static List<ISequenceFormatter> all = new List<ISequenceFormatter>() 
        { 
            fasta,
            fastq,
            gff,
            genBank
        };

		/// <summary>
        /// Initializes static members of the SequenceFormatters class.
        /// </summary>
#if (SILVERLIGHT == false)
        static SequenceFormatters()
		{
            // get the registered formatter
            IList<ISequenceFormatter> registeredFormatters = GetSequenceFormatters();
            if (null != registeredFormatters)
            {
                foreach (ISequenceFormatter formatter in registeredFormatters.Where(
                    formatter => formatter != null && 
                        !all.Any(rfm => 
                            string.Compare(rfm.Name,formatter.Name,StringComparison.OrdinalIgnoreCase) == 0)))
                {
                    all.Add(formatter);
                }
            }
        }
#endif

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
        public static IList<ISequenceFormatter> All
        {
            get
            {
                return all.AsReadOnly();
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
                    formatter = new FastAFormatter(fileName);
                }
                else if (IsFastQ(fileName))
                {
                    formatter = new FastQFormatter(fileName);
                }
                else if (IsGenBank(fileName))
                {
                    formatter = new GenBankFormatter(fileName);
                }
                else if (fileName.EndsWith(Properties.Resource.GFF_FILEEXTENSION, StringComparison.InvariantCultureIgnoreCase))
                {
                    formatter = new GffFormatter(fileName);
                }
                else
                {
                    // Do a search through the known formatters to pick up custom formatters added through add-in.
                    string fileExtension = Path.GetExtension(fileName);
                    if (!string.IsNullOrEmpty(fileExtension))
                    {
                        formatter = All.FirstOrDefault(p => p.SupportedFileTypes.Contains(fileExtension));
                        // If we found a match based on extension, then open the file - this 
                        // matches the above behavior where a specific formatter was created for
                        // the passed filename - the formatter is opened automatically in the constructor.
                        if (formatter != null)
                            formatter.Open(fileName);
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
                    formatter = new FastAFormatter(fileName);
                }
                else if (formatterName == Properties.Resource.FastQName)
                {
                    formatter = new FastQFormatter(fileName);
                }
                else if (formatterName == Properties.Resource.GENBANK_NAME)
                {
                    formatter = new GenBankFormatter(fileName);
                }
                else if (formatterName == Properties.Resource.GFF_NAME)
                {
                    formatter = new GffFormatter(fileName);
                }
                else
                {
                    // Do a search through the known formatters to pick up custom formatters added through add-in.
                    formatter = All.FirstOrDefault(p => p.Name == formatterName);
                    // If we found a match based on extension, then open the file - this 
                    // matches the above behavior where a specific formatter was created for
                    // the passed filename - the formatter is opened automatically in the constructor.
                    if (formatter != null)
                        formatter.Open(fileName);
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

#if (SILVERLIGHT == false)
		/// <summary>
        /// Gets all registered formatters in core folder and addins (optional) folders.
        /// </summary>
        /// <returns>List of registered formatters.</returns>
        private static IList<ISequenceFormatter> GetSequenceFormatters()
        {
            IList<ISequenceFormatter> registeredFormatters = new List<ISequenceFormatter>();

            IList<ISequenceFormatter> addInFormatters = Registration.RegisteredAddIn.GetComposedInstancesFromAssemblyPath<ISequenceFormatter>(
                            ".NetBioSequenceFormattersExport", Registration.RegisteredAddIn.AddinFolderPath, Registration.RegisteredAddIn.DLLFilter);
            if (null != addInFormatters && addInFormatters.Count > 0)
            {
                foreach (ISequenceFormatter formatter in addInFormatters.Where(
                    formatter => formatter != null && !registeredFormatters.Any(rfm => 
                        string.Compare(rfm.Name, formatter.Name, StringComparison.OrdinalIgnoreCase) == 0)))
                {
                    registeredFormatters.Add(formatter);
                }
            }

		    return registeredFormatters;
        }
#endif
    }
}
