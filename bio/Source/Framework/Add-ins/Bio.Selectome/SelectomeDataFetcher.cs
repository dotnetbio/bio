using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bio.Web;
using Bio.Phylogenetics;
using System.Data;
using System.Xml;
using System.IO;

namespace Bio.Selectome
{
    public static class SelectomeDataFetcher
    {
       /// <summary>
        /// Fetches the data for a given ENSEMBL identifier (protein, gene or transcript)
        /// </summary>
        /// <param name="ensemblID">An ensembl gene id used to query</param>
        /// <returns>A SelectomeGene if query is a success, otherwise NULL.</returns>
        public static SelectomeQueryResult FetchGeneByEnsemblID(string ensemblID)
        {
            if (String.IsNullOrEmpty(ensemblID))
            {
                throw new ArgumentException("Tried to query with null ensembl ID", "ensemblID");
            }
            //Verify Ensembl ID
            if (!ensemblID.StartsWith(ENSEMBL_PREFIX,StringComparison.CurrentCulture))
            {
                throw new ArgumentException("Cannot query with " + ensemblID + " because the name does not start with " + ENSEMBL_PREFIX, "ensemblID");
            }
            //TODO: Handle request to make sure a correct taxon/gene is selected?
            else
            {
                //Get data, i.e. submit and parse http://selectome.unil.ch/das/selectome/features?segment=ENSBTAG00000038321
                WebAccessor wb = new WebAccessor();
                Dictionary<string, string> requestParam = new Dictionary<string, string>();
                requestParam[SEGMENT_QUERY_KEY] = ensemblID;
                var reqUri = new Uri(SEGMENT_REQUEST_URL);
                var res = wb.SubmitHttpRequest(reqUri, false, requestParam);
                return ParseXML(res.ResponseString);                
            }
        }
        /// <summary>
        /// Factory methods to create query ansewer 
        /// </summary>


            /// <summary>
            /// This method expects a single XML document and returns one BlastResult.
            /// </summary>
            /// <param name="doc">A Stringbuilder containing the XML document.</param>
            /// <returns>The BlastResult.</returns>
        private static SelectomeQueryResult ParseXML(string webResponse)
        {
            webResponse = replaceHTMLText(webResponse);
            //PARSE THE XML, example below:            
            //<?xml version="1.0" standalone="yes"?>
            //<?xml-stylesheet type="text/xsl" href="features.xsl"?>
            //<!DOCTYPE DASGFF SYSTEM "http://www.biodas.org/dtd/dasgff.dtd">
            //<DASGFF>
            //  <GFF href="http://selectome.unil.ch/das/selectome/features">
            //<SEGMENT id="ENSCPOT00000004318" start="1">
            //<FEATURE id="ENSGT00550000074701_Euteleostomi_1_ENSCPOT00000004318" label="ENSCPOT00000004318">
            //<TYPE id="Positive selection" />
            //<METHOD id="codeml branch-site model" />
            //<NOTE>Positions under positive selection found in ENSCPOT00000004318 
            //(sub-family ENSGT00550000074701.Euteleostomi.1) 
            //in&#x3C;br/&#x3E;Sarcopterygii/Clupeocephala (57 amino-acid positions, pval = 0.00089362)</NOTE>
            //<LINK href="http://selectome.unil.ch/cgi-bin/subfamily.cgi?ac=ENSGT00550000074701&#x26;sub=1&#x26;tax=Euteleostomi" />
            //</FEATURE>
            //</SEGMENT>
            //  </GFF>
            //</DASGFF>

            //Because the selection results and links appear on different lines, going to merge them with dictionary, grr...
            Dictionary<SelectomeTaxaGroup, SelectomeQuerySubResult> results = new Dictionary<SelectomeTaxaGroup,SelectomeQuerySubResult>();
            string geneName = String.Empty;
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Ignore;   // don't error when encountering a DTD spec
                // Setting the XmlResolver to null causes the DTDs specified in the XML
                // header to be ignored. 
                settings.XmlResolver = null;
                using (var sr = new StringReader(webResponse))
                {
                    using (XmlReader r = XmlReader.Create(sr, settings))
                    {
                        string curElement = string.Empty;
                        bool alreadyAdvanced = false;
                        while (alreadyAdvanced || r.Read())
                        {
                            alreadyAdvanced = false;
                            switch (r.NodeType)
                            {
                                case XmlNodeType.Element:
                                    curElement = r.Name;
                                    if (curElement == "FEATURE")
                                    {
                                        geneName = r.GetAttribute("label");
                                    }
                                    else if (curElement == "NOTE")
                                    {
                                        //Construct the data from the note
                                        string innerText = r.ReadInnerXml();
                                        alreadyAdvanced = true;
                                        string[] groups = innerText.Split(NOTE_DIVIDER, StringSplitOptions.RemoveEmptyEntries);
                                        var tempResults = groups.Where(x => x.Contains("selection")).Select(x => processSelectomeData(x)).ToArray();
                                        foreach (var selectionRes in tempResults)
                                        {
                                            results[selectionRes.Group] = selectionRes;
                                        }
                                    }
                                    else if (curElement == "LINK")
                                    {
                                        //example a la =<LINK href="http://selectome.unil.ch/cgi-bin/subfamily.cgi?ac=ENSGT00390000009030&#x26;sub=1&#x26;tax=Euteleostomi" />
                                        string linkText = r.GetAttribute("href");
                                        var sp1 = linkText.Split('=');
                                        var treename = sp1[1].Split('&')[0];//ENSGT0039000000009446
                                        var subTreeName = sp1[2].Split('&')[0]; //1
                                        SelectomeTaxaGroup curGroup = SelectomeTaxaGroup.NotSet;
                                        foreach (var kv in SelectomeConstantsAndEnums.GroupToNameList)
                                        {
                                            if (sp1[3].Contains(kv.Value))
                                            {
                                                curGroup = kv.Key;
                                                break;
                                            }
                                        }
                                        if (curGroup != SelectomeTaxaGroup.NotSet)
                                        {
                                            //Verify it isn't over-writing another gene
                                            var sql = new SelectomeQueryLink(curGroup, treename, subTreeName);
                                            if (results.ContainsKey(curGroup) && results[curGroup].RelatedLink!=null)
                                            {
                                                var old = results[curGroup];
                                                if ((old.RelatedLink.Group != sql.Group || old.RelatedLink.SubTree != sql.SubTree || old.RelatedLink.Tree != sql.Tree))
                                                    throw new ArgumentException("Over-writing past tree group, investigate this gene.\r\n" + webResponse);
                                            }
                                            else
                                            {
                                                results[curGroup].RelatedLink = sql;
                                            }
                                        }                                            
                                    }
                                    break;
                                case XmlNodeType.XmlDeclaration:
                                    break;
                                case XmlNodeType.ProcessingInstruction:
                                    break;
                                case XmlNodeType.Comment:
                                    break;
                                case XmlNodeType.EndElement:
                                    break;
                            }
                        }
                    }
                }
                
                if (results != null && results.Count> 0)
                {
                    if (!results.ContainsKey(SelectomeTaxaGroup.Euteleostomi))
                    {
                        return new SelectomeQueryResult(QUERY_RESULT.NoVeterbrateTreeDataFound);
                        //throw new Exception("Could not parse the vertebrate group data from the XML received from selectome.  XML is:\n" + webResponse);
                    }
                    SelectomeGene result = new SelectomeGene(results,geneName);
                    return new SelectomeQueryResult(QUERY_RESULT.Success, result);
                }
            }
            catch
            {
                throw;
            }
            return new SelectomeQueryResult(QUERY_RESULT.NoResultsFound);
        }
        /// <summary>
        /// The return format is variable, so this converts the < and > tags appropriately
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string replaceHTMLText(string str)
        {
            return str.Replace("&gt;", ">").Replace("&lt;","<").Replace("&#x3C;","<").Replace("&#x3E",">");
        }

        /// <summary>
        /// Parses one line from the note section of the XML into a SelectomeResultObject
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
       private static SelectomeQuerySubResult processSelectomeData(string data)
       {

           //First assign taxa to a group
           SelectomeTaxaGroup group=SelectomeTaxaGroup.NotSet;
           foreach(var kv in SelectomeConstantsAndEnums.GroupToNameList)
           {
               if(data.Contains(kv.Value))
               {
                   group=kv.Key;
                   break;
               }
           }
           //Now determine if positive seleciton has occurred.           
           bool positiveSelectionSignature = false;
           if (data.Contains("Positive selection found"))
           {
               positiveSelectionSignature = true;
           }
           else if (data.Contains("No positive selection found"))
           {
               positiveSelectionSignature = false;
           }
           else if (data.Contains("Branch(es) under positive selection BUT no site found"))
           {
               positiveSelectionSignature=true;
           }
           else
           {
               throw new FormatException("Could not determine whether gene was under positive selection.\n XML was:" + data);
           }
           var result = new SelectomeQuerySubResult(group,positiveSelectionSignature);
           return result;

       }
      
       private static readonly string[] NOTE_DIVIDER = new string[] { "<br/", "<br /" };

        /// <summary>
        /// Regex to parse p-value from query
        /// </summary>

        private static string pvalRegExString=@"pval\s=\s(0.\d+)";
        private static Regex pvalRegEx = new Regex(pvalRegExString);
        
        /// <summary>
        /// Regex to parse inferred number of amino acids under selection from query. 
        /// </summary>
        private const string aminoAcidNumberRegExString=@"(\d+)\samino-acid\spositions";
        private static Regex aminoAcidNumberRegEx = new Regex(aminoAcidNumberRegExString);

        /// <summary>
        /// The website for DAS server requests
        /// </summary>
        private const string SEGMENT_REQUEST_URL = SelectomeConstantsAndEnums.BASE_SELECTOME_WEBSITE + "das/selectome/features";

        private const string SEGMENT_QUERY_KEY = "segment";
        /// <summary>
        /// Prefix in front of all ensembl segment identifiers
        /// </summary>
        private const string ENSEMBL_PREFIX = "ENS";

        /// <summary>
        /// These are the suffixes allowed for gene names, as only these species are currently used by selectome.  
        /// TODO: Not currently used.
        /// </summary>
        private static readonly string[] acceptableSpecies = new string[] {"ACAG",
        "AME",        "BTA",        "CAF",        "CJA",
        "CPO",        "DAR",        "DNO",        "FCA",
        "GAC",        "GAL",        "GGO",        "GMO",
        "LAC",        "LAF",        "MGA",        "MIC",
        "MLU",        "MMU",        "MOD",        "MUS",
        "NLE",        "OAN",        "OCU",        "OGA",
        "ONI",        "ORL",        "PCA",        "PPY",
        "PSI",        "PTR",        "RNO",        "SAR",
        "SHA",        "SSC",        "STO",        "TGU",
        "TRU",        "TTR",        "XET"};
    }
}
