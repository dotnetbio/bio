using System.Collections.Generic;
using System.Linq;
using Bio.Web.Blast;
using System;

namespace Bio.Web
{
    /// <summary>
    /// WebServices class is an abstraction class which provides instances
    /// and lists of all Web services currently supported by Bio. 
    /// </summary>
    public static class WebServices
    {
        /// <summary>
        /// List of supported Web services by the .NET Bio framework.
        /// </summary>
        private readonly static Lazy<List<IServiceHandler>> all = new Lazy<List<IServiceHandler>>(() => new List<IServiceHandler>(GetServiceHandlers()));

        /// <summary>
        /// Gets an instance of NcbiQBlast class which implements the client side 
        /// functionality required to perform Blast Search Requests against the 
        /// the NCBI QBlast system using their Blast URL APIs. 
        /// </summary>
        public static IBlastServiceHandler NcbiBlast
        {
            get
            {
                return All.FirstOrDefault(sh => sh is IBlastServiceHandler && sh.Name.Equals(Properties.Resource.NCBIQBLAST_NAME)) as IBlastServiceHandler;
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
                return All.FirstOrDefault(sh => sh is IBlastServiceHandler && sh.Name.Equals(Properties.Resource.EBIWUBLAST_NAME)) as IBlastServiceHandler;
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
                return All.FirstOrDefault(sh => sh is IBlastServiceHandler && sh.Name.Equals(Properties.Resource.BIOHPC_BLAST_NAME)) as IBlastServiceHandler;
            }
        }

        /// <summary>
        /// Returns the given web service handler by name
        /// </summary>
        /// <param name="name">Name to search for</param>
        /// <returns>Service Handler</returns>
        public static IServiceHandler FindWebServiceHandlerByName(string name)
        {
            return All.FirstOrDefault(sh => sh.Name.Equals(name)) as IBlastServiceHandler;
        }

        /// <summary>
        /// Gets the list of all Web services supported by .NET Bio
        /// </summary>
        public static IList<IServiceHandler> All
        {
            get
            {
                return all.Value.AsReadOnly();
            }
        }

        /// <summary>
        /// Returns all the composed service handlers
        /// </summary>
        /// <returns></returns>
        private static IList<IServiceHandler> GetServiceHandlers()
        {
            IList<IServiceHandler> registeredHandlers = new List<IServiceHandler>();
#if !SILVERLIGHT
            IList<IServiceHandler> addInHandlers = Registration.RegisteredAddIn.GetComposedInstancesFromAssemblyPath<IServiceHandler>(
                        ".NetBioServiceHandlersExport", Registration.RegisteredAddIn.AddinFolderPath, Registration.RegisteredAddIn.DLLFilter);
            if (null != addInHandlers && addInHandlers.Count > 0)
            {
                foreach (var handler in addInHandlers.Where(
                    sh => sh != null && !registeredHandlers.Any(sp =>
                            string.Compare(sp.Name, sh.Name, StringComparison.OrdinalIgnoreCase) == 0)))
                {
                    registeredHandlers.Add(handler);
                }
            }
#endif
            return registeredHandlers;
        }
    }
}
