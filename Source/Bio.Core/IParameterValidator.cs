namespace Bio
{
    /// <summary>
    /// A simple interface to an object that can check a value
    /// for conformance to any required validation rules.
    /// </summary>
    public interface IParameterValidator
    {
        /// <summary>
        /// Given a value as an object, return true if the value is allowed.
        /// </summary>
        /// <param name="parameterValue">The value.</param>
        /// <returns>True if the value is valid.</returns>
        bool IsValid(object parameterValue);

        /// <summary>
        /// Given a value in string form, return true if the value is allowed.
        /// </summary>
        /// <param name="parameterValue">The value.</param>
        /// <returns>True if the value is valid.</returns>
        bool IsValid(string parameterValue);
    }
}
