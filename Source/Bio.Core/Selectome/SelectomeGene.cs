using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Bio.Algorithms.Alignment;
using System.IO;
using System.Linq;
using System.Text;

using Bio.IO.FastA;
using Bio.IO.Newick;

namespace Bio.Web.Selectome
{
    /// <summary>
    /// A class that represents the data available for a given gene in the selectome database
    /// http://selectome.unil.ch/
    /// The data is provided as a DAS server, http://www.biodas.org/wiki/Main_Page, and this class obtains this data from the xml.
    /// 
    /// Currently only provides data from the vertebrate branch, as this uses the most information..
    /// </summary>
    public class SelectomeGene
    {
        private SelectomeTree vetebrateTree;
        private MultiSequenceAlignment maskedVertebrateNucleotideAlignment;
        private MultiSequenceAlignment unmaskedVertebrateNucleotideAlignment;
        private MultiSequenceAlignment maskedVertebrateAminoAcidAlignment;
        private MultiSequenceAlignment unmaskedVertebrateAminoAcidAlignment;
        private readonly SelectomeQuerySubResult vetebrateQueryResult;//just for vetebrates

        /// <summary>
        /// Does this gene show evidence of selection as determined by Selectome?
        /// </summary>
        public bool SelectionSignature
        {
            get
            {
                return this.vetebrateQueryResult.SelectionInferred;
            } 
        }

        /// <summary>
        /// Label for this gene
        /// </summary>
        public string Label { get; private set; }

        internal SelectomeGene(Dictionary<SelectomeTaxaGroup,SelectomeQuerySubResult> initiatingResults, string label)
        {
            if (!initiatingResults.ContainsKey(SelectomeTaxaGroup.Euteleostomi))
            {
                throw new FormatException("Could not parse the vertebrate group data from the XML received from selectome.\n");
            }
            this.Label = label;
            this.vetebrateQueryResult = initiatingResults[SelectomeTaxaGroup.Euteleostomi];
        }

        private async Task<string> GetStringFromURLRequest(string suffix)
        {
            //make a URL like
            //http://selectome.unil.ch/wwwtmp/ENSGT00550000074556/Euteleostomi/ENSGT00550000074556.Euteleostomi.003.nhx
            var treePrefix = "." + new String('0', 3 - this.vetebrateQueryResult.RelatedLink.SubTree.Length)+this.vetebrateQueryResult.RelatedLink.SubTree;
            string url = SelectomeConstantsAndEnums.BaseSelectomeWebsite+"wwwtmp/"+this.vetebrateQueryResult.RelatedLink.Tree+"/"+SelectomeConstantsAndEnums.VertebratesGroupName+"/"
                +this.vetebrateQueryResult.RelatedLink.Tree+"."+SelectomeConstantsAndEnums.VertebratesGroupName+treePrefix+"."+suffix;

            var reqUri = new Uri(url);
            return await new HttpClient().GetStringAsync(reqUri);
        }
        /// <summary>
        /// Get the Blosum90 multiple sequence alignment score for the masked alignment (Gap Open =-5, Gap Extend = -2)
        /// </summary>
        /// <returns></returns>
        public double GetMaskedBlosum90AlignmentScore()
        {
            var blosum = new Bio.SimilarityMatrices.SimilarityMatrix(SimilarityMatrices.SimilarityMatrix.StandardSimilarityMatrix.Blosum90);
            return MultiSequenceAlignment.MultipleAlignmentScoreFunction(this.MaskedAminoAcidAlignment.Sequences.ToList(), blosum, -5, -2);
        }

        /// <summary>
        /// Get the Blosum90 alignment score.
        /// </summary>
        /// <returns></returns>
        public double GetUnmaskedBlosum90AlignmentScore()
        {
            var blosum = new Bio.SimilarityMatrices.SimilarityMatrix(SimilarityMatrices.SimilarityMatrix.StandardSimilarityMatrix.Blosum90);
            return MultiSequenceAlignment.MultipleAlignmentScoreFunction(this.UnmaskedAminoAcidAlignment.Sequences.ToList(), blosum, -5, -2);
        }
        /// <summary>
        /// The vertebrate tree returned
        /// </summary>
        public SelectomeTree VertebrateTree
        {
            get 
            {
                if (this.vetebrateTree == null)
                {
                    //make the tree
                    var treeString = this.GetStringFromURLRequest("nhx").Result;
                    NewickParser np = new NewickParser();
                    var tmpTree = np.Parse(new StringBuilder(treeString));
                    this.vetebrateTree = new SelectomeTree(tmpTree);
                }
                return this.vetebrateTree;
            }
        }
        /// <summary>
        /// The vertebrate tree returned
        /// </summary>
        public Phylogenetics.Tree VertebrateTreeAsStandardTree
        {
            get
            {
                //make the tree
                var treeString = this.GetStringFromURLRequest("nhx").Result;
                var np = new NewickParser();
                {
                    return np.Parse(new StringBuilder(treeString));
                }
            }
        }


       //TODO: These should probably all be replaced with a single method that takes an MSA reference and suffix and returns an MAS
        /// <summary>
        /// The masked amino acid alignment, masked by selectome procedures
        /// </summary>
        public MultiSequenceAlignment MaskedAminoAcidAlignment
        {
            get 
            {
                this.DownloadAlignmentIfNeccessary(ref this.maskedVertebrateAminoAcidAlignment, "aa_masked.fas",Alphabets.AmbiguousProtein);
                return this.maskedVertebrateAminoAcidAlignment;
            }
        }
        /// <summary>
        /// The unmasked alignment.
        /// </summary>
        public MultiSequenceAlignment UnmaskedAminoAcidAlignment
        {
            get
            {
                this.DownloadAlignmentIfNeccessary(ref this.unmaskedVertebrateAminoAcidAlignment, "aa.fas",Alphabets.AmbiguousProtein);
                return this.unmaskedVertebrateAminoAcidAlignment;
            }
        }
        /// <summary>
        /// DNA alignment
        /// </summary>
        public MultiSequenceAlignment UnmaskedDNAAlignment
        {
            get
            {
                this.DownloadAlignmentIfNeccessary(ref this.unmaskedVertebrateNucleotideAlignment, "nt.fas");
                return this.unmaskedVertebrateNucleotideAlignment;
            }
        }

        /// <summary>
        /// Masked DNA alignment
        /// </summary>
        public MultiSequenceAlignment MaskedDNAAlignment
        {
            get
            {
                this.DownloadAlignmentIfNeccessary(ref this.maskedVertebrateNucleotideAlignment, "nt_masked.fas");
                return this.maskedVertebrateNucleotideAlignment;
            }
        }
        private void DownloadAlignmentIfNeccessary(ref MultiSequenceAlignment msa, string suffix,IAlphabet alphabet=null)
        {
            if (msa == null)
            {
                var alignmentString = this.GetStringFromURLRequest(suffix).Result;
                var parser = new FastAParser { Alphabet = alphabet };
                var seqs = parser.Parse(new MemoryStream(Encoding.Unicode.GetBytes(alignmentString)));
                msa = new MultiSequenceAlignment(seqs.ToList());
            }
        }
    }
}
