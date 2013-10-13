using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bio.Phylogenetics;
using Bio.Web;
using Bio.Algorithms.Alignment.MultipleSequenceAlignment;
using Bio.IO;
using System.IO;
using System.Diagnostics;

namespace Bio.Selectome
{
    /// <summary>
    /// A class that represents the data available for a given gene in the selectome database
    /// http://selectome.unil.ch/
    /// The data is provided as a DAS server, http://www.biodas.org/wiki/Main_Page, and this class obtains this data from the xml.
    /// 
    /// Currently only provides data from the veterbrate branch, as this uses the most information..
    /// </summary>
    public class SelectomeGene
    {
        #region STATIC_CACHE
        private static WebCache cache = new WebCache("Selectome", 60.0);//A Cache that is made with a 
        private static bool useCache = false;
        private static bool cacheCreationAttempted = false;
        #endregion 

        /// <summary>
        /// Does this gene show evidence of selection as determined by Selectome?
        /// </summary>
        public bool SelectionSignature
        {
            get
            {
                return vetebrateQueryResult.SelectionInferred;
            } 
        }
        public string Label { get; private set; }
        private SelectomeTree _vetebrateTree;
        private MultipleSequenceAlignment _maskedVertebrateNucleotideAlignment;
        private MultipleSequenceAlignment _unmaskedVertebrateNucleotideAlignment;
        private MultipleSequenceAlignment _maskedVertebrateAminoAcidAlignment;
        private MultipleSequenceAlignment _unmaskedVertebrateAminoAcidAlignment;
        private SelectomeQuerySubResult vetebrateQueryResult;//just for vetebrates
#if DEBUG
        private Dictionary<SelectomeTaxaGroup, SelectomeQuerySubResult> allResults;
#endif
        private SelectomeGene()
        {
            if (!cacheCreationAttempted)
            {
                lock (cache)
                {                    
                    cacheCreationAttempted=true;
                    try
                    {
                        cache = new WebCache("Selectome", 60.0);
                        useCache = true;
                    }
                    catch
                    {
                        useCache = false;
                        cache = null;
                    }
                }
            }
        }
        internal SelectomeGene(Dictionary<SelectomeTaxaGroup,SelectomeQuerySubResult> initiatingResults,string label):this()
        {

            if (!initiatingResults.ContainsKey(SelectomeTaxaGroup.Euteleostomi))
            {
                throw new FormatException("Could not parse the vertebrate group data from the XML received from selectome.\n");
            }
            Label = label;
#if DEBUG
            allResults = initiatingResults;
#endif
            vetebrateQueryResult = initiatingResults[SelectomeTaxaGroup.Euteleostomi];
        }
        private string GetStringFromURLRequest(string suffix)
        {
            //make a URL like
            //http://selectome.unil.ch/wwwtmp/ENSGT00550000074556/Euteleostomi/ENSGT00550000074556.Euteleostomi.003.nhx
            WebAccessor wb = new WebAccessor();
            //var treePrefix = "." + new String('0', 3 - vetebrateQueryResult.RelatedLink.subTree.Length);
            string url=SelectomeConstantsAndEnums.BASE_SELECTOME_WEBSITE+"wwwtmp/"+vetebrateQueryResult.RelatedLink.Tree+"/"+SelectomeConstantsAndEnums.VERTEBRATES_GROUP_NAME+"/"
                +vetebrateQueryResult.RelatedLink.Tree+"."+SelectomeConstantsAndEnums.VERTEBRATES_GROUP_NAME+".00"+vetebrateQueryResult.RelatedLink.SubTree+"."+suffix;
            var reqUri = new Uri(url);
            if (useCache)
            {
                try
                {
                    string result;
                    bool success = cache.TryRetrieve(reqUri.AbsoluteUri, out result);
                    if (success)
                    {
                        cache.Set(reqUri.AbsoluteUri, result);
                        return result;
                    }
                }
                catch(Exception e) 
                {
                    //cache failure, abort!  Resort to fresh downloads every time
                    Debug.Write("Could not use the cache for selectome data.\n" + e.Message);
                    useCache = false;
                }
            }
            var res = wb.SubmitHttpRequest(reqUri,false,new Dictionary<string,string>());
            if (res.IsSuccessful)
            {
                if (useCache)
                {
                    try { cache.Set(reqUri.AbsoluteUri, res.ResponseString); }
                    catch (Exception e) { throw new System.IO.IOException("Could not cache web result from: " + url, e); }
                }
                return res.ResponseString;
            }
            else
            {
                throw new FormatException("Could not get data implied by url: " + url+"\n Check internet connection and attempt to connect to this in a browser");
            }              
        }
        /// <summary>
        /// Get the Blosum90 multiple sequence alignment score for the masked alignment (Gap Open =-5, Gap Extend = -2)
        /// </summary>
        /// <returns></returns>
        public double GetMaskedBlosum90AlignmentScore()
        {
            var blosum = new Bio.SimilarityMatrices.SimilarityMatrix(SimilarityMatrices.SimilarityMatrix.StandardSimilarityMatrix.Blosum90);
            return MultipleSequenceAlignment.MultipleAlignmentScoreFunction(MaskedAminoAcidAlignment.Sequences, blosum, -5, -2);
        }

        public double GetUnmaskedBlosum90AlignmentScore()
        {
            var blosum = new Bio.SimilarityMatrices.SimilarityMatrix(SimilarityMatrices.SimilarityMatrix.StandardSimilarityMatrix.Blosum90);
            return MultipleSequenceAlignment.MultipleAlignmentScoreFunction(UnmaskedAminoAcidAlignment.Sequences, blosum, -5, -2);
        }
        /// <summary>
        /// The vertebrate tree returned
        /// </summary>
        public SelectomeTree VertebrateTree
        {
            get 
            {
                if (_vetebrateTree == null)
                {
                    //make the tree
                    var treeString = GetStringFromURLRequest("nhx");
                    using (Bio.IO.Newick.NewickParser np = new IO.Newick.NewickParser())
                    {
                        var tmpTree = np.Parse(new StringBuilder(treeString));
                        _vetebrateTree = new SelectomeTree(tmpTree);
                    }
                }
                return _vetebrateTree;
            }
        }
       //TODO: These should probably all be replaced with a single method that takes an MSA reference and suffix and returns an MAS
        public MultipleSequenceAlignment MaskedAminoAcidAlignment
        {
            get 
            {
                downloadAlignmentIfNeccessary(ref _maskedVertebrateAminoAcidAlignment, "aa_masked.fas",Alphabets.AmbiguousProtein);
                return _maskedVertebrateAminoAcidAlignment;
            }
        }
        public MultipleSequenceAlignment UnmaskedAminoAcidAlignment
        {
            get
            {
                downloadAlignmentIfNeccessary(ref _unmaskedVertebrateAminoAcidAlignment, "aa.fas",Alphabets.AmbiguousProtein);
                return _unmaskedVertebrateAminoAcidAlignment;
            }
        }
        public MultipleSequenceAlignment UnmaskedDNAAlignment
        {
            get
            {
                downloadAlignmentIfNeccessary(ref _unmaskedVertebrateNucleotideAlignment, "nt.fas");
                return _unmaskedVertebrateNucleotideAlignment;
            }
        }
        public MultipleSequenceAlignment MaskedDNAAlignment
        {
            get
            {
                downloadAlignmentIfNeccessary(ref _maskedVertebrateNucleotideAlignment, "nt.fas");
                return _maskedVertebrateNucleotideAlignment;
            }
        }
        private void downloadAlignmentIfNeccessary(ref MultipleSequenceAlignment msa, string suffix,IAlphabet alphabet=null)
        {
            if (msa == null)
            {
                var alignmentString = GetStringFromURLRequest(suffix);
                using (Bio.IO.FastA.FastAParser parser = new IO.FastA.FastAParser())
                {
                    parser.Alphabet = alphabet;
                    var seqs = parser.Parse(stringToStreamReader(alignmentString));
                    msa = new MultipleSequenceAlignment(seqs.ToList());
                }
            }
        }
        private static StreamReader stringToStreamReader(string str)
        {
            MemoryStream ms=new MemoryStream(Encoding.Unicode.GetBytes(str));
            StreamReader sr = new StreamReader(ms, Encoding.Unicode);
            return sr;
        }
    }
}
