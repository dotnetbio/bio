namespace BiodexExcel
{
    using System;

    /// <summary>
    /// Class to hold the cached data and related information in the cache
    /// </summary>
    class CachedSequenceData
    {
        /// <summary>
        /// Data being held in cache
        /// </summary>
        public object CachedData { get; set; }

        /// <summary>
        /// Last accessed time of this object
        /// Used to remove items which is not recently accessed when the cache is full
        /// </summary>
        public DateTime LastAccessTime { get; set; }

        /// <summary>
        /// Creates a new CachedSequence object
        /// </summary>
        /// <param name="objectToCache">Object to the added to the cache</param>
        public CachedSequenceData(object objectToCache)
        {
            this.CachedData = objectToCache;
            this.LastAccessTime = DateTime.Now;
        }
    }
}
