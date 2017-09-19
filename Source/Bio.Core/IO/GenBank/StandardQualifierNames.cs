using System.Collections.Generic;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// Static class to hold standard qualifier names.
    /// </summary>
    public static class StandardQualifierNames
    {
        #region Qualifier Names
        /// <summary>
        /// Qualifier name for Allele.
        /// </summary>
        public const string Allele = "allele";

        /// <summary>
        /// Qualifier name for AntiCodon.
        /// </summary>
        public const string AntiCodon = "anticodon";

        /// <summary>
        /// Qualifier name for Bio_Material.
        /// </summary>
        public const string BioMaterial = "bio_material";

        /// <summary>
        /// Qualifier name for Bound_Moiety.
        /// </summary>
        public const string BoundMoiety = "bound_moiety";

        /// <summary>
        /// Qualifier name for Cell_Line.
        /// </summary>
        public const string CellLine = "cell_line";

        /// <summary>
        /// Qualifier name for Cell_Type.
        /// </summary>
        public const string CellType = "cell_type";

        /// <summary>
        /// Qualifier name for Chromosome.
        /// </summary>
        public const string Chromosome = "chromosome";

        /// <summary>
        /// Qualifier name for Citation.
        /// </summary>
        public const string Citation = "citation";

        /// <summary>
        /// Qualifier name for Clone.
        /// </summary>
        public const string ClonedFrom = "clone";

        /// <summary>
        /// Qualifier name for Clone_Lib.
        /// </summary>
        public const string CloneLibrary = "clone_lib";

        /// <summary>
        /// Qualifier name for Codon.
        /// </summary>
        public const string Codon = "codon";

        /// <summary>
        /// Qualifier name for Codon_Start.
        /// </summary>
        public const string CodonStart = "codon_start";

        /// <summary>
        /// Qualifier name for Collected_By.
        /// </summary>
        public const string CollectedBy = "collected_by";

        /// <summary>
        /// Qualifier name for Collection_Date.
        /// </summary>
        public const string CollectionDate = "collection_date";

        /// <summary>
        /// Qualifier name for Compare.
        /// </summary>
        public const string Compare = "compare";

        /// <summary>
        /// Qualifier name for Country.
        /// </summary>
        public const string Country = "country";

        /// <summary>
        /// Qualifier name for Cultivar.
        /// </summary>
        public const string CultivatedVariety = "cultivar";

        /// <summary>
        /// Qualifier name for Culture_Collection.
        /// </summary>
        public const string CultureCollection = "culture_collection";

        /// <summary>
        /// Qualifier name for DatabaseCrossReference (Db_Xref).
        /// </summary>
        public const string DatabaseCrossReference = "db_xref";

        /// <summary>
        /// Qualifier name for Dev_Stage.
        /// </summary>
        public const string DevelopmentalStage = "dev_stage";

        /// <summary>
        /// Qualifier name for Direction.
        /// </summary>
        public const string Direction = "direction";

        /// <summary>
        /// Qualifier name for EC_Number.
        /// </summary>
        public const string EnzymeCommissionNumber = "EC_number";

        /// <summary>
        /// Qualifier name for Ecotype.
        /// </summary>
        public const string Ecotype = "ecotype";

        /// <summary>
        /// Qualifier name for Environmental_Sample.
        /// </summary>
        public const string EnvironmentalSample = "environmental_sample";

        /// <summary>
        /// Qualifier name for Estimated_Length.
        /// </summary>
        public const string EstimatedLength = "estimated_length";

        /// <summary>
        /// Qualifier name for Exception.
        /// </summary>
        public const string Exception = "exception";

        /// <summary>
        /// Qualifier name for Experiment.
        /// </summary>
        public const string Experiment = "experiment";

        /// <summary>
        /// Qualifier name for Focus.
        /// </summary>
        public const string Focus = "focus";

        /// <summary>
        /// Qualifier name for Frequency.
        /// </summary>
        public const string Frequency = "frequency";

        /// <summary>
        /// Qualifier name for Function.
        /// </summary>
        public const string Function = "function";

        /// <summary>
        /// Qualifier name for GeneSymbol.
        /// </summary>
        public const string GeneSymbol = "gene";

        /// <summary>
        /// Qualifier name for Gene_Synonym.
        /// </summary>
        public const string GeneSynonym = "gene_synonym";

        /// <summary>
        /// Qualifier name for Germline.
        /// </summary>
        public const string Germline = "germline";

        /// <summary>
        /// Qualifier name for Haplotype.
        /// </summary>
        public const string Haplotype = "haplotype";

        /// <summary>
        /// Qualifier name for Host.
        /// </summary>
        public const string Host = "host";

        /// <summary>
        /// Qualifier name for Identified_By.
        /// </summary>
        public const string IdentifiedBy = "identified_by";

        /// <summary>
        /// Qualifier name for Inference.
        /// </summary>
        public const string Inference = "inference";

        /// <summary>
        /// Qualifier name for Isolate.
        /// </summary>
        public const string Isolate = "isolate";

        /// <summary>
        /// Qualifier name for Isolation_Source.
        /// </summary>
        public const string IsolationSource = "isolation_source";

        /// <summary>
        /// Qualifier name for Lab_Host.
        /// </summary>
        public const string LabHost = "lab_host";

        /// <summary>
        /// Qualifier name for Label.
        /// </summary>
        public const string Label = "label";

        /// <summary>
        /// Qualifier name for Lat_Lon.
        /// </summary>
        public const string LatitudeLongitude = "lat_lon";

        /// <summary>
        /// Qualifier name for Locus_Tag.
        /// </summary>
        public const string LocusTag = "locus_tag";

        /// <summary>
        /// Qualifier name for Macronuclear.
        /// </summary>
        public const string Macronuclear = "macronuclear";

        /// <summary>
        /// Qualifier name for Map.
        /// </summary>
        public const string GenomicMapPosition = "map";

        /// <summary>
        /// Qualifier name for Mating_Type.
        /// </summary>
        public const string MatingType = "mating_type";

        /// <summary>
        /// Qualifier name for Mobile_Element.
        /// </summary>
        public const string MobileElement = "mobile_element";

        /// <summary>
        /// Qualifier name for Mod_Base.
        /// </summary>
        public const string ModifiedNucleotideBase = "mod_base";

        /// <summary>
        /// Qualifier name for Mol_Type.
        /// </summary>
        public const string MoleculeType = "mol_type";

        /// <summary>
        /// Qualifier name for ncRNA_Class.
        /// </summary>
        public const string NonCodingRnaClass = "ncRNA_class";

        /// <summary>
        /// Qualifier name for Note.
        /// </summary>
        public const string Note = "note";

        /// <summary>
        /// Qualifier name for Number.
        /// </summary>
        public const string Number = "number";

        /// <summary>
        /// Qualifier name for Old_Locus_Tag.
        /// </summary>
        public const string OldLocusTag = "old_locus_tag";

        /// <summary>
        /// Qualifier name for Operon.
        /// </summary>
        public const string Operon = "operon";

        /// <summary>
        /// Qualifier name for Organelle.
        /// </summary>
        public const string Organelle = "organelle";

        /// <summary>
        /// Qualifier name for Organism.
        /// </summary>
        public const string Organism = "organism";

        /// <summary>
        /// Qualifier name for PCR_Conditions.
        /// </summary>
        public const string PcrConditions = "PCR_conditions";

        /// <summary>
        /// Qualifier name for PCR_Primers.
        /// </summary>
        public const string PcrPrimers = "PCR_primers";

        /// <summary>
        /// Qualifier name for Phenotype.
        /// </summary>
        public const string Phenotype = "phenotype";

        /// <summary>
        /// Qualifier name for Plasmid.
        /// </summary>
        public const string Plasmid = "plasmid";

        /// <summary>
        /// Qualifier name for Pop_Variant.
        /// </summary>
        public const string PopulationVariant = "pop_variant";

        /// <summary>
        /// Qualifier name for Product.
        /// </summary>
        public const string Product = "product";

        /// <summary>
        /// Qualifier name for Protein_Id.
        /// </summary>
        public const string ProteinId = "protein_id";

        /// <summary>
        /// Qualifier name for Proviral.
        /// </summary>
        public const string Proviral = "proviral";

        /// <summary>
        /// Qualifier name for Pseudo.
        /// </summary>
        public const string Pseudo = "pseudo";

        /// <summary>
        /// Qualifier name for Rearranged.
        /// </summary>
        public const string Rearranged = "rearranged";

        /// <summary>
        /// Qualifier name for Replace.
        /// </summary>
        public const string Replace = "replace";

        /// <summary>
        /// Qualifier name for Ribosomal_Slippage.
        /// </summary>
        public const string RibosomalSlippage = "ribosomal_slippage";

        /// <summary>
        /// Qualifier name for Rpt_Family.
        /// </summary>
        public const string RepeatedSequenceFamily = "rpt_family";

        /// <summary>
        /// Qualifier name for Rpt_Type.
        /// </summary>
        public const string RepeatedSequenceType = "rpt_type";

        /// <summary>
        /// Qualifier name for Rpt_Unit_Range.
        /// </summary>
        public const string RepeatedRange = "rpt_unit_range";

        /// <summary>
        /// Qualifier name for Rpt_Unit_Seq.
        /// </summary>
        public const string RepeatedSequence = "rpt_unit_seq";

        /// <summary>
        /// Qualifier name for Satellite.
        /// </summary>
        public const string Satellite = "satellite";

        /// <summary>
        /// Qualifier name for Segment.
        /// </summary>
        public const string Segment = "segment";

        /// <summary>
        /// Qualifier name for Serotype.
        /// </summary>
        public const string Serotype = "serotype";

        /// <summary>
        /// Qualifier name for Serovar.
        /// </summary>
        public const string Serovar = "serovar";

        /// <summary>
        /// Qualifier name for Sex.
        /// </summary>
        public const string Sex = "sex";

        /// <summary>
        /// Qualifier name for Specimen_Voucher.
        /// </summary>
        public const string SpecimenVoucher = "specimen_voucher";

        /// <summary>
        /// Qualifier name for Standard_Name.
        /// </summary>
        public const string StandardName = "standard_name";

        /// <summary>
        /// Qualifier name for Strain.
        /// </summary>
        public const string Strain = "strain";

        /// <summary>
        /// Qualifier name for Sub_Clone.
        /// </summary>
        public const string SubClone = "sub_clone";

        /// <summary>
        /// Qualifier name for Sub_Species.
        /// </summary>
        public const string SubSpecies = "sub_species";

        /// <summary>
        /// Qualifier name for Sub_Strain.
        /// </summary>
        public const string SubStrain = "sub_strain";

        /// <summary>
        /// Qualifier name for Tag_Peptide.
        /// </summary>
        public const string TagPeptide = "tag_peptide";

        /// <summary>
        /// Qualifier name for Tissue_Lib.
        /// </summary>
        public const string TissueLibrary = "tissue_lib";

        /// <summary>
        /// Qualifier name for Tissue_Type.
        /// </summary>
        public const string TissueType = "tissue_type";

        /// <summary>
        /// Qualifier name for Trans_Splicing.
        /// </summary>
        public const string TransSplicing = "trans_splicing";

        /// <summary>
        /// Qualifier name for Transgenic.
        /// </summary>
        public const string Transgenic = "transgenic";

        /// <summary>
        /// Qualifier name for Transl_Except.
        /// </summary>
        public const string TranslationalExcept = "transl_except";

        /// <summary>
        /// Qualifier name for Transl_Table.
        /// </summary>
        public const string TranslationTable = "transl_table";

        /// <summary>
        /// Qualifier name for Translation.
        /// </summary>
        public const string Translation = "translation";

        /// <summary>
        /// Qualifier name for Variety.
        /// </summary>
        public const string Variety = "variety";

        #endregion Qualifier Names

        private static List<string> all;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static StandardQualifierNames()
        {
            all = new List<string>
                  {
                      Allele,
                      AntiCodon,
                      BioMaterial,
                      BoundMoiety,
                      CellLine,
                      CellType,
                      Chromosome,
                      Citation,
                      ClonedFrom,
                      CloneLibrary,
                      Codon,
                      CodonStart,
                      CollectedBy,
                      CollectionDate,
                      Compare,
                      Country,
                      CultivatedVariety,
                      CultureCollection,
                      DatabaseCrossReference,
                      DevelopmentalStage,
                      Direction,
                      EnzymeCommissionNumber,
                      Ecotype,
                      EnvironmentalSample,
                      EstimatedLength,
                      Exception,
                      Experiment,
                      Focus,
                      Frequency,
                      Function,
                      GeneSymbol,
                      GeneSynonym,
                      Germline,
                      Haplotype,
                      Host,
                      IdentifiedBy,
                      Inference,
                      Isolate,
                      IsolationSource,
                      LabHost,
                      Label,
                      LatitudeLongitude,
                      LocusTag,
                      Macronuclear,
                      GenomicMapPosition,
                      MatingType,
                      MobileElement,
                      ModifiedNucleotideBase,
                      MoleculeType,
                      NonCodingRnaClass,
                      Note,
                      Number,
                      OldLocusTag,
                      Operon,
                      Organelle,
                      Organism,
                      PcrConditions,
                      PcrPrimers,
                      Phenotype,
                      Plasmid,
                      PopulationVariant,
                      Product,
                      ProteinId,
                      Proviral,
                      Pseudo,
                      Rearranged,
                      Replace,
                      RibosomalSlippage,
                      RepeatedSequenceFamily,
                      RepeatedSequenceType,
                      RepeatedRange,
                      RepeatedSequence,
                      Satellite,
                      Segment,
                      Serotype,
                      Serovar,
                      Sex,
                      SpecimenVoucher,
                      StandardName,
                      Strain,
                      SubClone,
                      SubSpecies,
                      SubStrain,
                      TagPeptide,
                      TissueLibrary,
                      TissueType,
                      TransSplicing,
                      Transgenic,
                      TranslationalExcept,
                      TranslationTable,
                      Translation,
                      Variety
                  };

            #region Add qualifier names to list

            #endregion Add qualifier names to list
        }

        /// <summary>
        /// Returns a list of standard qualifier names.
        /// </summary>
        public static IReadOnlyList<string> All
        {
            get
            {
                return all;
            }
        }
    }

}
