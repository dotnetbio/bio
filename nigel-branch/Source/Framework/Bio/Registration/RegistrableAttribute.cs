namespace Bio.Registration
{
    using System;

    /// <summary>
    /// Self registrable  mechanism's attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class RegistrableAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the RegistrableAttribute class.
        /// </summary>
        /// <param name="isRegistrable">Registrable or not.</param>
        public RegistrableAttribute(bool isRegistrable)
        {
            this.IsRegistrable = isRegistrable;
        }

        /// <summary>
        /// Gets a value indicating whether its registrable or not.
        /// </summary>
        public bool IsRegistrable { get; private set; }
    }
}
