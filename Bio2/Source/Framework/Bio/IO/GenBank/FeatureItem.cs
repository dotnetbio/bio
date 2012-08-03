using System;
using System.Collections.Generic;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// Feature of sequence present in the metadata can be stored in this class.
    /// All qualifiers of the feature will be stored as sub items.
    /// </summary>
    public class FeatureItem
    {
        #region Constructors

        /// <summary>
        /// Creates new feature item with given key and location.
        /// </summary>
        /// <param name="key">The feature key.</param>
        /// <param name="location">An instance of ILocation.</param>
        public FeatureItem(string key, ILocation location)
        {
            Key = key;
            if (location == null)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameLocation);
            }

            this.Location = location;
            Qualifiers = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Creates feature item with given key and location.
        /// Note that this constructor uses LocationBuilder to construct location object from the specified 
        /// location string.
        /// </summary>
        /// <param name="key">The feature key.</param>
        /// <param name="location">Location string.</param>
        public FeatureItem(string key, string location)
            : this(key, (new LocationBuilder()).GetLocation(location)) { }

        /// <summary>
        /// Private Constructor for clone method.
        /// </summary>
        /// <param name="other">FeatureItem instance to clone.</param>
        protected FeatureItem(FeatureItem other)
        {
            Key = other.Key;
            this.Location = other.Location.Clone();
            Qualifiers = new Dictionary<string, List<string>>();
            foreach (KeyValuePair<string, List<string>> kvp in other.Qualifiers)
            {
                if (kvp.Value != null)
                {
                    Qualifiers.Add(kvp.Key, new List<string>(kvp.Value));
                }
                else
                {
                    Qualifiers.Add(kvp.Key, null);
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// A label used to permanently tag a feature.
        /// </summary>
        public string Label
        {
            get
            {
                return GetSingleTextQualifier(StandardQualifierNames.Label);
            }

            set
            {
                SetSingleTextQualifier(StandardQualifierNames.Label, value);
            }
        }
        /// <summary>
        /// Gets the key for this item.  These are not necessarily unique within a list,
        /// which is why this is a property of an object to be included in a list, rather than
        /// omitting this as a property and using a dictionary instead of a list.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the location of this feature in the sequence.
        /// This may also refers to other genbank files.
        /// For example, 
        /// join(100..200,J00194.1:1..150)
        /// In this example location specifies joining of bases from 100 to 200 from the sequence
        /// in which this location data present and bases from 1 to 150 from the sequence who's 
        /// accession number is J00194.1.
        /// </summary>
        public ILocation Location { get; private set; }

        /// <summary>
        /// Gets the dictionary of attributes.
        /// </summary>
        public Dictionary<string, List<string>> Qualifiers { get; private set; }

        #endregion Properties

        #region Methods
        /// <summary>
        /// Gets the sub features depending on the location information.
        /// </summary>
        /// <param name="sequenceFeatures">SequenceFeatures instance.</param>
        public List<FeatureItem> GetSubFeatures(SequenceFeatures sequenceFeatures)
        {
            List<FeatureItem> subFeatures = new List<FeatureItem>();
            if (sequenceFeatures != null)
            {
                int start = this.Location.LocationStart;
                int end = this.Location.LocationEnd;
                foreach (FeatureItem item in sequenceFeatures.All)
                {
                    int subItemStart = item.Location.LocationStart;
                    int subItemEnd = item.Location.LocationEnd;
                    if (subItemStart >= start && subItemEnd <= end && string.IsNullOrEmpty(item.Location.Accession))
                    {
                        // do not add items with the same start and end positions.
                        if (item != this)
                        {
                            subFeatures.Add(item);
                        }
                    }
                }
            }

            return subFeatures;
        }

        /// <summary>
        /// Returns a new sequence from the specified sequence which contains bases of this feature as specified by 
        /// the location property of this feature.
        /// </summary>
        /// <param name="sequence">Sequence from which the sub sequence has to be returned.</param>
        public ISequence GetSubSequence(ISequence sequence)
        {
            ISequence seq = null;
            if (sequence != null)
            {
                seq = this.Location.GetSubSequence(sequence);
            }

            return seq;

        }

        /// <summary>
        /// Returns a sequence which contains bases from the specified sequences as specified by this feature location.
        /// If the location contains accession then the sequence from the referredSequences which matches the 
        /// accession of the location will be considered.
        /// 
        /// For example, 
        /// If a location is "join(100..200, J00089.1:10..50, J00090.2:30..40)"
        /// bases from 100 to 200 will be taken from the parent sequence and referredSequences will
        /// be searched for the J00089.1 and J00090.2 accession if found then those sequences will be considered 
        /// for constructing the output sequence.
        /// If the referred sequence is not found in the referredSequences then an exception will occur.
        /// </summary>
        /// <param name="sequence">Sequence from which the sub sequence has to be returned.</param>
        /// <param name="referredSequences">A dictionary containing Accession numbers as keys and Sequences as values, this will be used when
        /// the location or sub-locations contains accession.</param>
        public ISequence GetSubSequence(ISequence sequence, Dictionary<string, ISequence> referredSequences)
        {
            ISequence seq = null;
            if (sequence != null)
            {
                seq = this.Location.GetSubSequence(sequence, referredSequences);
            }

            return seq;
        }

        /// <summary>
        /// Returns list of qualifier values for the specified qualifier name.
        /// </summary>
        /// <param name="qualifierName">Qualifier name.</param>
        /// <returns>List of strings.</returns>
        protected List<string> GetQualifier(string qualifierName)
        {
            if (!Qualifiers.ContainsKey(qualifierName))
            {
                Qualifiers.Add(qualifierName, new List<string>());
            }

            return Qualifiers[qualifierName];
        }

        /// <summary>
        /// Returns qualifier value for the specified qualifier name.
        /// Note: This method should be used to get the text value of a 
        /// qualifier which appears only once in a feature.
        /// </summary>
        /// <param name="qualifierName">Qualifier name.</param>
        /// <returns>Qualifier value.</returns>
        protected string GetSingleTextQualifier(string qualifierName)
        {
            List<string> list = GetQualifier(qualifierName);
            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Sets the value for the specified qualifier name.
        /// Note: This method should be used to set the text value of a 
        /// qualifier which appears only once in a feature.
        /// </summary>
        /// <param name="qualifierName">Qualifier name.</param>
        /// <param name="value">Value to set.</param>
        protected void SetSingleTextQualifier(string qualifierName, string value)
        {
            List<string> list = GetQualifier(qualifierName);
            if (string.IsNullOrEmpty(value))
            {
                list.Clear();
            }
            else
            {
                list.Clear();
                list.Add(value);
            }
        }

        /// <summary>
        /// Returns bool value indicating whether the specified qualifier is there in the feature or not.
        /// </summary>
        /// <param name="qualifierName">Qualifier name.</param>
        /// <returns>Returns true if the qualifier is found in the feature, otherwise false.</returns>
        protected bool GetSingleBooleanQualifier(string qualifierName)
        {
            return GetQualifier(qualifierName).Count > 0;
        }

        /// <summary>
        /// Sets the value for the specified qualifier name.
        /// Note: This method should be used to add a qualifier which 
        /// appears only once in a feature and whose value is none.
        /// </summary>
        /// <param name="qualifierName">Qualifier name.</param>
        /// <param name="value">Value to set.</param>
        protected void SetSingleBooleanQualifier(string qualifierName, bool value)
        {
            if (GetSingleBooleanQualifier(qualifierName) != value)
            {
                List<string> list = GetQualifier(qualifierName);
                list.Clear();

                if (value)
                {
                    list.Add(string.Empty);
                }
            }
        }

        /// <summary>
        /// Creates a new FeatureItem that is a copy of the current FeatureItem.
        /// </summary>
        /// <returns>A new FeatureItem that is a copy of this FeatureItem.</returns>
        public FeatureItem Clone()
        {
            return StandardFeatureMap.GetStandardFeatureItem(new FeatureItem(this));
        }
        #endregion Methods

    }
}
