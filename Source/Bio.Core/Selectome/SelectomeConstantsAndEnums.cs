using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Bio.Web.Selectome
{
    /// <summary>
    /// An enumeration of the taxonomic groups checked by Selectome
    /// </summary>
    public enum SelectomeTaxaGroup { 
        /// <summary>
        /// Not yet set.
        /// </summary>
        NotSet=0,
        /// <summary>
        /// Most Vetebrates
        /// </summary>
        Euteleostomi, 
        /// <summary>
        /// Primates
        /// </summary>
        Primates, 
        /// <summary>
        /// Rodents and lagomorphs
        /// </summary>
        Glires };

    /// <summary>
    /// Constants and enumerations used when interacting with Selectome.
    /// </summary>
    public class SelectomeConstantsAndEnums
    {
        static SelectomeConstantsAndEnums() { }
        /// <summary>
        /// Group name of all vertebrates used by selectome
        /// </summary>
        public const string VertebratesGroupName = "Euteleostomi";
        /// <summary>
        /// Ensembl name for primates
        /// </summary>
        public const string PrimatesGroupName = "Primates";
        /// <summary>
        /// Rodents and lagomorphs group
        /// </summary>
        public const string RabbitGroupName = "Glires";

        /// <summary>
        /// The selectome website
        /// </summary>
        public const string BaseSelectomeWebsite = "http://selectome.unil.ch/";
        /// <summary>
        /// Maps group enumerations to their string representation
        /// </summary>
        public static readonly IReadOnlyCollection<KeyValuePair<SelectomeTaxaGroup, string>> GroupToNameList =  new[] {
            new KeyValuePair<SelectomeTaxaGroup,string>(SelectomeTaxaGroup.Euteleostomi,VertebratesGroupName),
            new KeyValuePair<SelectomeTaxaGroup,string>(SelectomeTaxaGroup.Primates,PrimatesGroupName),
            new KeyValuePair<SelectomeTaxaGroup,string>(SelectomeTaxaGroup.Glires,RabbitGroupName),
        };

        /// <summary>
        /// A dictionary which maps taxonomic groups to friendly names.
        /// </summary>
        public static readonly ReadOnlyDictionary<SelectomeTaxaGroup, string> GroupToNameMapping = new ReadOnlyDictionary<SelectomeTaxaGroup, string>(new Dictionary<SelectomeTaxaGroup,string>() { 
            {SelectomeTaxaGroup.Euteleostomi,VertebratesGroupName},
            {SelectomeTaxaGroup.Primates,PrimatesGroupName},
            {SelectomeTaxaGroup.Glires,RabbitGroupName}});
    }
}
