using System;
using System.Collections.Generic;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// Location, holds the feature location information.
    /// This is the default implementation of the ILocation interface.
    /// This holds Start and End points of location. 
    /// If in case location refers to some other sequence (for example, J00194.1:1..150) 
    /// then the accession number information should be stored in the Accession property.
    /// Resolver property is used to resolve any ambiguity in the location start-data and end-data. 
    /// By default this will be set to an instance of LocationResolver class.  
    /// </summary>
        
    public class Location : ILocation
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Location()
        {
            SubLocations = new List<ILocation>();
            Resolver = new LocationResolver();
        }

        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the ILocationResolver instance used to resolve 
        /// ambiguity in start-data and end-data
        /// </summary>
        public ILocationResolver Resolver { get; set; }
        #endregion Properties

        /// <summary>
        /// Creates a new Location that is a copy of the current Location.
        /// </summary>
        /// <returns>A new Location that is a copy of this Location.</returns>
        public Location Clone()
        {
            Location loc = new Location();
            loc.Accession = Accession;
            loc.StartData = StartData;
            loc.EndData = EndData;
            loc.Separator = Separator;
            loc.Operator = Operator;
            loc.Resolver = Resolver.Clone();

            foreach (ILocation subloc in SubLocations)
            {
                loc.SubLocations.Add(subloc.Clone());
            }

            return loc;
        }

        #region ILocation Members

        /// <summary>
        /// Gets the start position of the location.
        /// Note that this is one based position.
        /// </summary>
        public int LocationStart
        {
            get
            {
                if (Resolver == null)
                {
                    throw new InvalidOperationException(Properties.Resource.NullResolver);
                }

                return Resolver.GetStart(this);
            }
        }

        /// <summary>
        /// Gets the end position of the location.
        /// Note that this is one based position.
        /// </summary>
        public int LocationEnd
        {
            get
            {
                if (Resolver == null)
                {
                    throw new InvalidOperationException(Properties.Resource.NullResolver);
                }

                return Resolver.GetEnd(this);
            }
        }

        /// <summary>
        /// Gets or sets the start position data.
        /// All positions are one based.
        /// </summary>
        public string StartData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the end position data.
        /// All positions are one based.
        /// </summary>
        public string EndData
        {
            get;
            set;
        }

        /// <summary>
        /// Accession number of referred sequence.
        /// </summary>
        public string Accession { get; set; }

        /// <summary>
        /// Start and end positions separator.
        /// </summary>
        public string Separator
        {
            get;
            set;
        }

        /// <summary>
        /// Operator like none, complement, join and order.
        /// </summary>
        public LocationOperator Operator
        {
            get;
            set;
        }

        /// <summary>
        /// Sub locations.
        /// </summary>
        public List<ILocation> SubLocations
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns true if the specified position is within the start positions of the location.
        /// </summary>
        /// <param name="position">Position to be verified.</param>
        public bool IsInStart(int position)
        {
            if (Resolver == null)
            {
                throw new InvalidOperationException(Properties.Resource.NullResolver);
            }

            return Resolver.IsInStart(this, position);
        }

        /// <summary>
        /// Returns true if the specified position is within the end positions of the location.
        /// </summary>
        /// <param name="position">Position to be verified.</param>
        public bool IsInEnd(int position)
        {
            if (Resolver == null)
            {
                throw new InvalidOperationException(Properties.Resource.NullResolver);
            }

            return Resolver.IsInEnd(this, position);
        }

        /// <summary>
        /// Returns true if the specified position is within the start and end positions of the location.
        /// </summary>
        /// <param name="position">Position to be verified.</param>
        public bool IsInRange(int position)
        {
            if (Resolver == null)
            {
                throw new InvalidOperationException(Properties.Resource.NullResolver);
            }

            return Resolver.IsInRange(this, position);
        }

        /// <summary>
        /// Returns a sequence which contains bases from the specified sequences as specified by this location.
        /// </summary>
        /// <param name="sequence">Sequence from which the sub sequence has to be returned.</param>
        public ISequence GetSubSequence(ISequence sequence)
        {
            if (Resolver == null)
            {
                throw new InvalidOperationException(Properties.Resource.NullResolver);
            }

            return Resolver.GetSubSequence(this, sequence);
        }

        /// <summary>
        /// Returns a sequence which contains bases from the specified sequences as specified by this location.
        /// If the location contains accession then the sequence from the referredSequences which matches the 
        /// accession of the location will be considered.
        /// 
        /// For example, 
        /// If a location is "join(100..200, J00089.1:10..50, J00090.2:30..40)"
        /// bases from 100 to 200 will be taken from the sequence parameter and referredSequences will
        /// be searched for the J00089.1 and J00090.2 accession if found then those sequences will be considered 
        /// for constructing the output sequence.
        /// If the referred sequence is not found in the referredSequences then an exception will occur.
        /// </summary>
        /// <param name="sequence">Sequence instance from which the sub sequence has to be returned.</param>
        /// <param name="referredSequences">A dictionary containing Accession numbers as keys and Sequences as values, this will be used when
        /// the location or sublocations contains accession.</param>
        public ISequence GetSubSequence(ISequence sequence, Dictionary<string, ISequence> referredSequences)
        {
            if (Resolver == null)
            {
                throw new InvalidOperationException(Properties.Resource.NullResolver);
            }

            return Resolver.GetSubSequence(this, sequence, referredSequences);
        }

        /// <summary>
        /// Gets the leaf locations present in this location.
        /// </summary>
        public List<ILocation> GetLeafLocations()
        {
            List<ILocation> locations = new List<ILocation>();
            GetLeafLocations(locations, this);
            return locations;
        }

        /// <summary>
        /// Creates a new Location that is a copy of the current Location.
        /// </summary>
        /// <returns>A new ILocation that is a copy of this Location.</returns>
        ILocation ILocation.Clone()
        {
            return Clone();
        }
        #endregion ILocation Members

        #region IComparable Members
        /// <summary>
        ///  Compares this instance to a specified object and returns an 
        ///  indication of their relative values.
        /// </summary>
        /// <param name="obj">Loction instance to compare.</param>
        /// <returns> 
        /// A signed number indicating the relative values of this instance and value.
        /// Return Value Description Less than zero This instance is less than value.
        /// Zero This instance is equal to value. Greater than zero This instance is
        /// greater than value.  -or- value is null.
        /// </returns>
        public int CompareTo(object obj)
        {
            ILocation other = obj as ILocation;
            if (other == null)
            {
                return 1;
            }

            int startresult = LocationStart.CompareTo(other.LocationStart);

            if (startresult != 0)
            {
                return startresult;
            }

            int endresult = LocationEnd.CompareTo(other.LocationEnd);
            if (startresult == endresult)
            {
                return 0;
            }

            return endresult;
        }
        #endregion IComparable Members


        #region Private Methods
        /// <summary>
        /// Recursively gets the leaf locations.
        /// </summary>
        /// <param name="locations">Locations list.</param>
        /// <param name="location">location instance.</param>
        private void GetLeafLocations(List<ILocation> locations, ILocation location)
        {
            if (location.SubLocations.Count > 0)
            {
                foreach (ILocation loc in location.SubLocations)
                {
                    GetLeafLocations(locations, loc);
                }
            }
            else
            {
                if (!locations.Contains(location))
                {
                    locations.Add(location);
                }
            }
        }
        #endregion Private Methods
    }
}
