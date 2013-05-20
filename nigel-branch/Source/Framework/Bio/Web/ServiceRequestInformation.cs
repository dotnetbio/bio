using System;

namespace Bio.Web
{
    /// <summary>
    /// A return value for service requests, giving the status 
    /// as well as additional information from the server.
    /// </summary>
    public class ServiceRequestInformation
    {
        #region Constructors

        /// <summary>
        /// Default Constructor: Initializes an instance of class ServiceRequestInformation
        /// </summary>
        public ServiceRequestInformation()
        { }

        #endregion Constructors

        /// <summary>
        /// The status summary.
        /// </summary>
        public ServiceRequestStatus Status { get; set; }

        /// <summary>
        /// Additional information from the server.
        /// </summary>
        public string StatusInformation { get; set; }

        /// <summary>
        /// Override the Equals implementation
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current instance.</param>
        /// <returns>Returns true is given object is equal to current instance</returns>
        public override bool Equals(object obj)
        {
            ServiceRequestInformation status = obj as ServiceRequestInformation;

            if (status == null)
            {
                return false;
            }

            return Equals(status);
        }

        /// <summary>
        /// generate a number (hash code) that corresponds to the value of an object
        /// </summary>
        /// <returns>Hash of the object</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified ServiceRequestInformation is equal to the current instance.
        /// </summary>
        /// <param name="obj">The ServiceRequestInformation to compare with the current instance.</param>
        /// <returns>Returns true is given ServiceRequestInformation is equal to current instance</returns>
        public bool Equals(ServiceRequestInformation obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (Status != obj.Status || StatusInformation != obj.StatusInformation)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Overloading ==
        /// </summary>
        /// <param name="object1">Object1 to be compared</param>
        /// <param name="object2">Object@ to be compared</param>
        /// <returns></returns>
        public static bool operator ==(ServiceRequestInformation object1, ServiceRequestInformation object2)
        {
            if (object1 == null)
            {
                return false;
            }

            return object1.Equals(object2);
        }

        /// <summary>
        /// Overloading !=
        /// </summary>
        /// <param name="object1">Object1 to be compared</param>
        /// <param name="object2">Object2 to be compared</param>
        /// <returns></returns>
        public static bool operator !=(ServiceRequestInformation object1, ServiceRequestInformation object2)
        {
            if (object1 == null)
            {
                return false;
            }

            return !object1.Equals(object2);
        }
    }
}
