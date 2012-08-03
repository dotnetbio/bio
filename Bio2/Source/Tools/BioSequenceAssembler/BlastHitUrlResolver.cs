using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bio.Web;
using Bio.Web.Blast;

namespace SequenceAssembler
{
    /// <summary>
    /// Utility class which will generate a web url for a given blast hit
    /// </summary>
    public static class BlastHitUrlResolver
    {
        /// <summary>
        /// Get the url for a blast hit
        /// </summary>
        /// <param name="hitID">Blast hit id</param>
        /// <param name="blastService">Instance of the blast service</param>
        /// <param name="databaseName">Database name used in the blast query</param>
        /// <returns>A url if resolved, else null</returns>
        public static string ResolveUrl(string hitID, IBlastServiceHandler blastService, string databaseName)
        {
            // NCBI
            if (blastService.Name == WebServices.NcbiBlast.Name)
            {
                string[] idSplit = hitID.Split('|');
                if (idSplit.Length < 2)
                    return null;

                string id = idSplit[1];

                return string.Format("http://www.ncbi.nlm.nih.gov/protein/{0}", id);
            }

            // EBI
            else if (blastService.Name == WebServices.EbiBlast.Name)
            {
                string[] idSplit = hitID.Split(':', ';');
                if (idSplit.Length < 2)
                    return null;

                string id = idSplit[1];

                switch(databaseName)
                {
                    case "em_rel":
                        return string.Format("http://www.ebi.ac.uk/ena/data/view/{0}", id);

                    case "uniprot":
                        return string.Format("http://www.uniprot.org/uniprot/{0}.html", id);

                    default:
                        return null;
                }
            }

            // Return null if url was not resolved
            return null;
        }
    }
}
