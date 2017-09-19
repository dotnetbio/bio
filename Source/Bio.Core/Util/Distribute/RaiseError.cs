using System;

using Bio.Util.ArgumentParser;

namespace Bio.Util.Distribute
{
    /// <summary>
    /// For objects that shouldn't self distribute
    /// </summary>
    public class RaiseError : IDistribute, IParsable
    {
        #region IDistributor Members

        /// <summary>
        /// Distribute.
        /// </summary>
        /// <param name="distributableObject"></param>
        public void Distribute(IDistributable distributableObject)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IParsable Members

        /// <summary>
        /// Finalize Parse.
        /// </summary>
        public void FinalizeParse()
        {
        }

        #endregion
    }
}
