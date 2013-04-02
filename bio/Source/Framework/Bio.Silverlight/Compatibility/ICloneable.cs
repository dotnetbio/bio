namespace System
{
    /// <summary>
    /// Supports cloning, which creates a new instance of a class with the same value
    /// as an existing instance.
    /// </summary>
    public interface ICloneable
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new instance of the object</returns>
        object Clone();
    }
}
