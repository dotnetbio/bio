using System.ComponentModel.Composition;
namespace Bio.Web
{
    /// <summary>
    /// Interface that must be extended by all the service providers of Microsoft 
    /// Biology Foundation. This interface contains properties that are common to any 
    /// type of service provided by Bio.
    /// </summary>
    [InheritedExport(".NetBioServiceHandlersExport", typeof(IServiceHandler))]
    public interface IServiceHandler
    {
        /// <summary>
        /// Gets user-friendly implementation description
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets user-friendly implementation name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets settings for web access, such as user-agent string and 
        /// proxy configuration
        /// </summary>
        ConfigParameters Configuration { get; set; }
    }
}
