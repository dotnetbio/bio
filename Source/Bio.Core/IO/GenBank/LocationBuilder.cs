using System;
using System.Globalization;
using System.Text;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// This is the default implementation of the ILocationBuilder interface.
    /// This class builds the location for the specified location string
    /// and location string for the specified location instance.
    /// </summary>
    public class LocationBuilder : ILocationBuilder
    {
        #region Public Methods
        /// <summary>
        /// Returns a location for the specified location string.
        /// </summary>
        /// <param name="location">Location string.</param>
        public ILocation GetLocation(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameLocation);
            }

            string locationString = location.Replace(" ", string.Empty);
            try
            {
                ILocation loc = BuildLocation(ref locationString);

                if (!string.IsNullOrEmpty(locationString))
                {
                    string str = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidLocationString, locationString);
                    throw new ArgumentException(str);
                }

                return loc;
            }
            catch (ArgumentException ex)
            {
                string str = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidLocationString, location);
                throw new ArgumentException(str, ex);
            }
        }

        /// <summary>
        /// Returns a location string for the specified location.
        /// </summary>
        /// <param name="location">Location instance.</param>
        public string GetLocationString(ILocation location)
        {
            if (location == null)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameLocation);
            }

            StringBuilder strBuilder = new StringBuilder();
            if (location.Operator != LocationOperator.None)
            {
                strBuilder.Append(location.Operator.ToString().ToLowerInvariant() + "(");
                if (location.Operator == LocationOperator.Complement)
                {
                    if (location.SubLocations.Count > 1)
                    {
                        throw new ArgumentException(Properties.Resource.ComplementWithMorethanOneSubLocs);
                    }

                    if (location.SubLocations.Count > 0)
                    {
                        strBuilder.Append(GetLocationString(location.SubLocations[0]));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(location.Accession))
                        {
                            strBuilder.Append(location.Accession + ":");
                        }

                        strBuilder.Append(location.StartData);
                        strBuilder.Append(location.Separator);
                        strBuilder.Append(location.EndData);
                    }
                }
                else
                {
                    if (location.SubLocations.Count > 0)
                    {
                        for (int i = 0; i < location.SubLocations.Count; i++)
                        {
                            strBuilder.Append(GetLocationString(location.SubLocations[i]));
                            if (i < location.SubLocations.Count - 1)
                            {
                                strBuilder.Append(",");
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(location.Accession))
                        {
                            strBuilder.Append(location.Accession + ":");
                        }

                        strBuilder.Append(location.StartData);
                        strBuilder.Append(location.Separator);
                        strBuilder.Append(location.EndData);
                    }
                }

                strBuilder.Append(")");
            }
            else
            {
                if (!string.IsNullOrEmpty(location.Accession))
                {
                    strBuilder.Append(location.Accession + ":");
                }

                if (string.IsNullOrEmpty(location.Separator))
                {
                    strBuilder.Append(location.StartData);
                }
                else
                {
                    strBuilder.Append(location.StartData);
                    strBuilder.Append(location.Separator);
                    strBuilder.Append(location.EndData);
                }
            }

            return strBuilder.ToString();
        }
        #endregion Public Methods

        #region Private Methods
        /// <summary>
        /// Builds location from the specified location string.
        /// </summary>
        /// <param name="locationString">Location string.</param>
        /// <returns>A Location instance.</returns>
        private ILocation BuildLocation(ref string locationString)
        {
            Location location = new Location {Resolver = new LocationResolver()};

            if (locationString.StartsWith("complement(", StringComparison.OrdinalIgnoreCase))
            {
                location.Operator = LocationOperator.Complement;
                locationString = locationString.Substring(11);
                BuilSubLocation(location, ref locationString);
                return location;
            }
            else if (locationString.StartsWith("join(", StringComparison.OrdinalIgnoreCase))
            {
                location.Operator = LocationOperator.Join;
                locationString = locationString.Substring(5);
                BuilSubLocation(location, ref locationString);
                return location;
            }
            else if (locationString.StartsWith("bond(", StringComparison.OrdinalIgnoreCase))
            {
                location.Operator = LocationOperator.Bond;
                locationString = locationString.Substring(5);
                BuilSubLocation(location, ref locationString);
                return location;
            }
            else if (locationString.StartsWith("order(", StringComparison.OrdinalIgnoreCase))
            {
                location.Operator = LocationOperator.Order;
                locationString = locationString.Substring(6);
                BuilSubLocation(location, ref locationString);
                return location;
            }
            else
            {
                int index = GetNextIndex(locationString);
                string singleLocation = locationString;
                if (index != -1)
                {
                    singleLocation = locationString.Substring(0, index);
                    locationString = locationString.Substring(index);
                }
                else
                {
                    locationString = string.Empty;
                }

                if (singleLocation.Contains(":"))
                {
                    int firstIndex = singleLocation.IndexOf(":",StringComparison.OrdinalIgnoreCase);
                    if (firstIndex != singleLocation.LastIndexOf(":", StringComparison.OrdinalIgnoreCase))
                    {
                        string str = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidLocationString, locationString);
                        throw new ArgumentException(str);
                    }

                    string[] strArray = singleLocation.Split(':');
                    if (strArray.Length != 2)
                    {
                        string str = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidLocationString, locationString);
                        throw new ArgumentException(str);
                    }

                    location.Accession = strArray[0];
                    singleLocation = strArray[1];
                }

                if (singleLocation.Contains(".."))
                {
                    int firstIndex = singleLocation.IndexOf("..", StringComparison.OrdinalIgnoreCase);
                    if (firstIndex != singleLocation.LastIndexOf("..", StringComparison.OrdinalIgnoreCase))
                    {
                        string str = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidLocationString, locationString);
                        throw new ArgumentException(str);
                    }

                    string[] delisms = new string[1];
                    delisms[0] = "..";
                    string[] strArray = singleLocation.Split(delisms, StringSplitOptions.RemoveEmptyEntries);
                    if (strArray.Length != 2)
                    {
                        string str = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidLocationString, locationString);
                        throw new ArgumentException(str);
                    }

                    location.StartData = strArray[0];
                    location.EndData = strArray[1];
                    location.Separator = "..";
                }
                else if (singleLocation.Contains("."))
                {
                    int firstIndex = singleLocation.IndexOf(".", StringComparison.OrdinalIgnoreCase);
                    if (firstIndex != singleLocation.LastIndexOf(".", StringComparison.OrdinalIgnoreCase))
                    {
                        string str = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidLocationString, locationString);
                        throw new ArgumentException(str);
                    }

                    string[] strArray = singleLocation.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (strArray.Length != 2)
                    {
                        string str = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidLocationString, locationString);
                        throw new ArgumentException(str);
                    }

                    location.StartData = strArray[0];
                    location.EndData = strArray[1];
                    location.Separator = ".";
                }
                else if (singleLocation.Contains("^"))
                {
                    int firstIndex = singleLocation.IndexOf("^", StringComparison.OrdinalIgnoreCase);
                    if (firstIndex != singleLocation.LastIndexOf("^", StringComparison.OrdinalIgnoreCase))
                    {
                        string str = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidLocationString, locationString);
                        throw new ArgumentException(str);
                    }

                    string[] strArray = singleLocation.Split('^');
                    if (strArray.Length != 2)
                    {
                        string str = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidLocationString, locationString);
                        throw new ArgumentException(str);
                    }

                    location.StartData = strArray[0];
                    location.EndData = strArray[1];
                    location.Separator = "^";
                }
                else if (singleLocation.StartsWith("<", StringComparison.OrdinalIgnoreCase) 
                    || singleLocation.StartsWith(">", StringComparison.OrdinalIgnoreCase))
                {
                    int point;
                    if (int.TryParse(singleLocation.Substring(1,singleLocation.Length - 1), out point))
                    {
                        location.StartData = singleLocation;
                        location.EndData = singleLocation;
                        location.Separator = string.Empty;
                    }
                    else
                    {
                        string str = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidLocationString, locationString);
                        throw new ArgumentException(str);
                    }
                }
                else
                {
                    int point;
                    if (int.TryParse(singleLocation, out point))
                    {
                        location.StartData = singleLocation;
                        location.EndData = singleLocation;
                        location.Separator = string.Empty;
                    }
                    else
                    {
                        string str = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidLocationString, locationString);
                        throw new ArgumentException(str);
                    }
                }

                return location;
            }
        }

        /// <summary>
        /// Builds the sub-locations from the specified string and places in the specified location.
        /// </summary>
        /// <param name="location">Location instance.</param>
        /// <param name="locationString">Location string.</param>
        private void BuilSubLocation(ILocation location, ref string locationString)
        {
            while (!string.IsNullOrEmpty(locationString))
            {
                int index = GetNextIndex(locationString);
                if (index == 0)
                {
                    if (locationString[index] == ',')
                    {
                        locationString = locationString.Substring(1);
                        continue;
                    }
                    else if (locationString[index] == ')')
                    {
                        locationString = locationString.Substring(1);
                        return;
                    }
                }

                location.SubLocations.Add(BuildLocation(ref locationString));
            }
        }

        /// <summary>
        /// Returns an integer index which indicates the next ","  or ")" character.
        /// </summary>
        /// <param name="location">Location string.</param>
        private static int GetNextIndex(string location)
        {
            int locationCloseIndex = location.IndexOf(",", StringComparison.OrdinalIgnoreCase);
            int operationCloseIndex = location.IndexOf(")", StringComparison.OrdinalIgnoreCase);
            int indexToconsider;
            if (locationCloseIndex < 0 && operationCloseIndex < 0)
            {
                indexToconsider = -1;
            }
            else if (locationCloseIndex < 0)
            {
                indexToconsider = operationCloseIndex;
            }
            else if (operationCloseIndex < 0)
            {
                indexToconsider = locationCloseIndex;
            }
            else
            {
                indexToconsider = Math.Min(locationCloseIndex, operationCloseIndex);
            }

            return indexToconsider;
        }
        #endregion Private Methods
    }
}
