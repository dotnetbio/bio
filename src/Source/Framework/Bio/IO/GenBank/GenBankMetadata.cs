using System;
using System.Collections.Generic;
using System.Linq;
using Bio.Util.Logging;
using System.ComponentModel;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// GenBankMetadata class holds metadata provided 
    /// by the gen bank flat file format.
    /// </summary>
    public class GenBankMetadata
    {
        #region Constructors
        /// <summary>
        /// Default Constructor.
        /// Creates GenBankMetadata instance.
        /// </summary>
        public GenBankMetadata()
        {
            References = new List<CitationReference>();
            Comments = new List<string>();
        }

        /// <summary>
        /// Private Constructor for clone method.
        /// </summary>
        /// <param name="other">GenBankMetadata instance to clone.</param>
        private GenBankMetadata(GenBankMetadata other)
        {
            if (other.Locus != null)
            {
                Locus = other.Locus.Clone();
            }

            Definition = other.Definition;
            if (other.Accession != null)
            {
                Accession = other.Accession.Clone();
            }

            if (other.Version != null)
            {
                Version = other.Version.Clone();
            }

            if (other.Project != null)
            {
                Project = other.Project.Clone();
            }
            if (other.DbLinks != null)
            {
                DbLinks = other.DbLinks.ToList();
            }

            DbSource = other.DbSource;
            Keywords = other.Keywords;
            if (other.Segment != null)
            {
                Segment = other.Segment.Clone();
            }

            if (other.Source != null)
            {
                Source = other.Source.Clone();
            }

            References = new List<CitationReference>();
            foreach (CitationReference reference in other.References)
            {
                References.Add(reference.Clone());
            }

            Comments = new List<string>(other.Comments);
            Primary = other.Primary;

            if (other.Features != null)
            {
                Features = other.Features.Clone();
            }

            BaseCount = other.BaseCount;
            Origin = other.Origin;
            Contig = other.Contig;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the locaus information.
        /// Locus is a short mnemonic name for the entry, chosen to suggest the
        /// sequence's definition
        /// </summary>
        public GenBankLocusInfo Locus { get; set; }

        /// <summary>
        /// Gets or sets the definition.
        /// Definition is a concise description of the sequence
        /// </summary>
        public string Definition { get; set; }

        /// <summary>
        /// Accession is identifier assigned to each GenBank sequence record.
        /// </summary>
        public GenBankAccession Accession { get; set; }

        /// <summary>
        /// A compound identifier consisting of the primary accession number and 
        /// a numeric version number associated with the current version of the 
        /// sequence data in the record. This is followed by an integer key 
        /// (a "GI") assigned to the sequence by NCBI.
        /// </summary>
        public GenBankVersion Version { get; set; }

        /// <summary>
        /// The identifier of a project (such as a Genome Sequencing Project) 
        /// to which a GenBank sequence record belongs.
        /// 
        /// This is obsolete and was removed from the GenBank flat-file format 
        /// after Release 171.0 in April 2009.
        /// </summary>
        public ProjectIdentifier Project { get; set; }

        /// <summary>
        /// DBLink provides cross-references to resources that support the existence 
        /// a sequence record, such as the Project Database and the NCBI 
        /// Trace Assembly Archive.
        /// </summary>
        [Obsolete("Use the IList in DbLinks instead as there can be more than one DBLink.")]
        public CrossReferenceLink DbLink {
            get
            {
                if (DbLinks != null && DbLinks.Count > 0)
                {
                    return DbLinks[0];
                }
                else
                {
                    return null;
                }

            }
            set {
                DbLinks = new List<CrossReferenceLink>();
                DbLinks.Add(value);
            }

        }

        /// <summary>
        /// DBLinks provides a list of cross-references to resources that support the existence 
        /// a sequence record, such as the Project Database and the NCBI 
        /// Trace Assembly Archive.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList<CrossReferenceLink> DbLinks { get; set; }

        /// <summary>
        /// DBSource provies reference to the GenBank record from which the protein 
        /// translation was obtained.
        /// </summary>
        public string DbSource { get; set; }

        /// <summary>
        /// Short phrases describing gene products and other information about the sequence.
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// Segment provides the information on the order in which this entry appears in a
        /// series of discontinuous sequences from the same molecule.
        /// </summary>
        public SequenceSegment Segment { get; set; }

        /// <summary>
        /// Source provides the common name of the organism or the name most frequently used
        /// in the literature along with the taxonomic classification levels 
        /// </summary>
        public SequenceSource Source { get; set; }

        /// <summary>
        /// Citations for all articles containing data reported in this entry.
        /// </summary>
        public IList<CitationReference> References { get; private set; }

        /// <summary>
        /// Cross-references to other sequence entries, comparisons to
        /// other collections, notes of changes in LOCUS names, and other remarks.
        /// </summary>
        public IList<string> Comments { get; private set; }

        /// <summary>
        /// Provides the reference to the primary GenBank files from which annotations 
        /// in this file are derived.
        /// </summary>
        public string Primary { get; set; }

        /// <summary>
        /// Containing information on portions of the sequence that code for 
        /// proteins and RNA molecules and information on experimentally determined 
        /// sites of biological significance.
        /// </summary>
        public SequenceFeatures Features { get; set; }

        /// <summary>
        /// Summary of the number of occurrences of each base pair code 
        /// (a, c, t, g, and other) in the sequence.
        /// 
        /// This is obsolete and was removed from the GenBank flat-file 
        /// format in October 2003.
        /// </summary>
        public string BaseCount { get; set; }

        /// <summary>
        /// Specification of how the first base of the reported sequence is 
        /// operationally located within the genome. Where possible, this 
        /// includes its location within a larger genetic map.
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// provides information about how individual sequence records can be 
        /// combined to form larger-scale biological objects, such as chromosomes 
        /// or complete genomes. Rather than presenting actual sequence data, a 
        /// special join() statement provides the accession numbers and base pair 
        /// ranges of the underlying records which comprise the object.
        /// </summary>
        public string Contig { get; set; }
        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns the features which are partly or completely inside the specified range.
        /// Note that the startPosition and endPosition are one based position.
        /// </summary>
        /// <param name="startPosition">Start position.</param>
        /// <param name="endPosition">End position</param>
        public List<FeatureItem> GetFeatures(int startPosition, int endPosition)
        {
            return GetFeatures(string.Empty, startPosition, endPosition);
        }

        /// <summary>
        /// Returns the features which are partly or completely inside the specified range 
        /// and belongs to specified accession.
        /// Note that the startPosition and endPosition are one based position.
        /// </summary>
        /// <param name="accession">Accession number.</param>
        /// <param name="startPosition">Start position.</param>
        /// <param name="endPosition">End position</param>
        public List<FeatureItem> GetFeatures(string accession, int startPosition, int endPosition)
        {
            if (startPosition > endPosition)
            {
                Trace.Report(Properties.Resource.InvalidStartNEndPositions);
                throw new ArgumentException(Properties.Resource.InvalidStartNEndPositions);
            }

            List<FeatureItem> features = new List<FeatureItem>();
            foreach (FeatureItem feature in Features.All)
            {
                if (startPosition <= feature.Location.LocationEnd && feature.Location.LocationStart <= endPosition)
                {
                    if (string.IsNullOrEmpty(accession) && string.IsNullOrEmpty(feature.Location.Accession))
                    {
                        features.Add(feature);
                    }
                    else
                    {
                        if (string.Compare(accession, feature.Location.Accession, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            features.Add(feature);
                        }
                    }
                }
            }

            return features;
        }

        /// <summary>
        /// Returns list of citation references in this metadata which are referred in the specified feature.
        /// </summary>
        /// <param name="item">Feature Item.</param>
        public List<CitationReference> GetCitationsReferredInFeature(FeatureItem item)
        {
            List<CitationReference> list = new List<CitationReference>();
            if (item == null || !item.Qualifiers.ContainsKey(StandardQualifierNames.Citation))
            {
                return list;
            }

            foreach (string str in item.Qualifiers[StandardQualifierNames.Citation])
            {
                if (!string.IsNullOrEmpty(str))
                {
                    string strCitationNumber = str.Replace("[", string.Empty).Replace("]", string.Empty);
                    int citationNumber = -1;
                    if (int.TryParse(strCitationNumber, out citationNumber))
                    {
                        CitationReference citation = References.FirstOrDefault(F => F.Number == citationNumber);
                        if (citation != null && !list.Contains(citation))
                        {
                            list.Add(citation);
                        }
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Returns list of citation references in this metadata which are referred in features.
        /// </summary>
        public List<CitationReference> GetCitationsReferredInFeatures()
        {
            List<CitationReference> list = new List<CitationReference>();
            if (Features != null)
            {
                foreach (FeatureItem item in Features.All)
                {
                    list.InsertRange(list.Count, GetCitationsReferredInFeature(item));
                }
            }

            return list.Distinct().ToList();
        }

        /// <summary>
        /// Creates a new GenBankMetadata that is a copy of the current GenBankMetadata.
        /// </summary>
        /// <returns>A new GenBankMetadata that is a copy of this GenBankMetadata.</returns>
        public GenBankMetadata Clone()
        {
            return new GenBankMetadata(this);
        }

        #endregion Methods
    }
}
