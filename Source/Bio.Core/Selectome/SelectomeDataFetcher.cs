using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace Bio.Web.Selectome
{
    /// <summary>
    /// Data fetch for a selectome
    /// </summary>
    public static class SelectomeDataFetcher
    {
        private static readonly string[] NoteDivider = { "<br/", "<br /" };
        private const string SegmentRequestUrl = SelectomeConstantsAndEnums.BaseSelectomeWebsite + "das/selectome/features";
        private const string SegmentQueryKey = "segment";
        private const string EnsemblPrefix = "ENS";

       /// <summary>
        /// Fetches the data for a given ENSEMBL identifier (protein, gene or transcript)
        /// </summary>
        /// <param name="ensemblID">An ensembl gene id used to query</param>
        /// <returns>A SelectomeGene if query is a success, otherwise NULL.</returns>
        public static async Task<SelectomeQueryResult> FetchGeneByEnsemblID(string ensemblID)
        {
            if (String.IsNullOrEmpty(ensemblID))
            {
                throw new ArgumentNullException("ensemblID");
            }

            //Verify Ensembl ID
            if (!ensemblID.StartsWith(EnsemblPrefix,StringComparison.CurrentCulture))
            {
                throw new ArgumentException("Cannot query with " + ensemblID + " because the name does not start with " + EnsemblPrefix, "ensemblID");
            }

           var response = await new HttpClient().GetStringAsync(SegmentRequestUrl + string.Format("?{0}={1}", SegmentQueryKey, ensemblID));
           return ParseXML(response);
        }

        /// <summary>
        /// This method expects a single XML document and returns one BlastResult.
        /// </summary>
        /// <returns>The BlastResult.</returns>
        private static SelectomeQueryResult ParseXML(string webResponse)
        {
            webResponse = ReplaceHtmlText(webResponse);
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
            var results = new Dictionary<SelectomeTaxaGroup,SelectomeQuerySubResult>();
            string geneName = String.Empty;
            var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore };
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
                                    string[] groups = innerText.Split(NoteDivider, StringSplitOptions.RemoveEmptyEntries);
                                    var tempResults = groups.Where(x => x.Contains("selection")).Select(ProcessSelectomeData).ToArray();
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
                                    SelectomeTaxaGroup curGroup = (from kv in SelectomeConstantsAndEnums.GroupToNameList where sp1[3].Contains(kv.Value) select kv.Key).FirstOrDefault();
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
                    return new SelectomeQueryResult(QueryResult.NoVeterbrateTreeDataFound);
                    //throw new Exception("Could not parse the vertebrate group data from the XML received from selectome.  XML is:\n" + webResponse);
                }
                SelectomeGene result = new SelectomeGene(results,geneName);
                return new SelectomeQueryResult(QueryResult.Success, result);
            }
            return new SelectomeQueryResult(QueryResult.NoResultsFound);
        }

        /// <summary>
        /// The return format is variable, so this converts the HTML tags appropriately
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string ReplaceHtmlText(string str)
        {
            return str.Replace("&gt;", ">").Replace("&lt;","<").Replace("&#x3C;","<").Replace("&#x3E",">");
        }

        /// <summary>
        /// Parses one line from the note section of the XML into a SelectomeResultObject
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
       private static SelectomeQuerySubResult ProcessSelectomeData(string data)
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
    }
}
