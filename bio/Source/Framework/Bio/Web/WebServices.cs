using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bio.Web.Blast;
using Bio.Registration;

namespace Bio.Web
{
    /// <summary>
    /// WebServices class is an abstraction class which provides instances
    /// and lists of all Webservices currently supported by Bio. 
    /// </summary>
    public static class WebServices
    {
        /// <summary>
        /// List of supported Webservices by the Bio.
        /// </summary>
        private static List<IServiceHandler> all = (List<IServiceHandler>)
            RegisteredAddIn.GetInstancesFromAssembly<IServiceHandler>(
                Path.Combine(AssemblyResolver.BioInstallationPath, Properties.Resource.SERVICE_HANDLER_ASSEMBLY));

        /// <summary>
        /// Gets an instance of NcbiQBlast class which implements the client side 
        /// functionality required to perform Blast Search Requests against the 
        /// the NCBI QBlast system using their Blast URL APIs. 
        /// </summary>
        public static IBlastServiceHandler NcbiBlast
        {
            get
            {
                foreach (IBlastServiceHandler serviceHandler in
                    All.Where(service => service is IBlastServiceHandler))
                {
                    if (serviceHandler.Name.Equals(Properties.Resource.NCBIQBLAST_NAME))
                    {
                        return serviceHandler;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets an instance of EBI WUBlast class which will implement the 
        /// client side functionality required to perform Blast Search Requests 
        /// against the EBI WUBlast web-service using their published interface proxy.
        /// </summary>
        public static IBlastServiceHandler EbiBlast
        {
            get
            {
                foreach (IBlastServiceHandler serviceHandler in
                    All.Where(service => service is IBlastServiceHandler))
                {
                    if (serviceHandler.Name.Equals(Properties.Resource.EBIWUBLAST_NAME))
                    {
                        return serviceHandler;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets an instance of BioHPC Blast class which will implement the 
        /// client side functionality required to perform Blast Search Requests 
        /// against the Azure Blast web-service using their published interface proxy.
        /// </summary>
        public static IBlastServiceHandler BioHPCBlast
        {
            get
            {
                foreach (IBlastServiceHandler serviceHandler in
                    All.Where(service => service is IBlastServiceHandler))
                {
                    if (serviceHandler.Name.Equals(Properties.Resource.BIOHPC_BLAST_NAME))
                    {
                        return serviceHandler;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the list of all Webservices supported by the Bio.
        /// </summary>
        public static IList<IServiceHandler> All
        {
            get
            {
                return all.AsReadOnly();
            }
        }
    }
}
