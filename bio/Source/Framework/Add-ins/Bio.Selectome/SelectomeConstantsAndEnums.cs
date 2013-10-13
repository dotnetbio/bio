using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Bio.Selectome
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
        public const string VERTEBRATES_GROUP_NAME = "Euteleostomi";
        /// <summary>
        /// Ensembl name for primates
        /// </summary>
        public const string PRIMATES_GROUP_NAME = "Primates";
        /// <summary>
        /// Rodents and lagomorphs group
        /// </summary>
        public const string RABBIT_GROUP_NAME = "Glires";

        /// <summary>
        /// The selectome website
        /// </summary>
        public const string BASE_SELECTOME_WEBSITE = "http://selectome.unil.ch/";
        /// <summary>
        /// Maps group enumerations to their string representation
        /// </summary>
        public static readonly IReadOnlyCollection<KeyValuePair<SelectomeTaxaGroup, string>> GroupToNameList =  Array.AsReadOnly(new KeyValuePair<SelectomeTaxaGroup, string>[] {
            new KeyValuePair<SelectomeTaxaGroup,string>(SelectomeTaxaGroup.Euteleostomi,SelectomeConstantsAndEnums.VERTEBRATES_GROUP_NAME),
            new KeyValuePair<SelectomeTaxaGroup,string>(SelectomeTaxaGroup.Primates,SelectomeConstantsAndEnums.PRIMATES_GROUP_NAME),
            new KeyValuePair<SelectomeTaxaGroup,string>(SelectomeTaxaGroup.Glires,SelectomeConstantsAndEnums.RABBIT_GROUP_NAME),
        });
        /// <summary>
        /// A dictionary which maps taxonomic groups to friendly names.
        /// </summary>
        public static readonly ReadOnlyDictionary<SelectomeTaxaGroup, string> GroupToNameMapping = new ReadOnlyDictionary<SelectomeTaxaGroup, string>(new Dictionary<SelectomeTaxaGroup,string>() { 
            {SelectomeTaxaGroup.Euteleostomi,VERTEBRATES_GROUP_NAME},
            {SelectomeTaxaGroup.Primates,PRIMATES_GROUP_NAME},
            {SelectomeTaxaGroup.Glires,RABBIT_GROUP_NAME}});
    }
}
