using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bio.Util.ArgumentParser
{
    /// <summary>
    /// ParallelOptionsParser
    /// </summary>
    public class ParallelOptionsParser 
    {
        /// <summary>
        /// Max Degree Of Parallelism.
        /// </summary>
        [Parse(ParseAction.Required)]
        public uint MaxDegreeOfParallelism { get; set; }

        /// <summary>
        /// Convert Parallel options to Parallel options Parser type.
        /// </summary>
        /// <param name="parser">Parallel options Parser.</param>
        /// <returns> Converted Parallel options to Parallel options Parser type.</returns>
        public static implicit operator ParallelOptions(ParallelOptionsParser parser)
        {
            if (parser == null)
                return null;
            return new ParallelOptions() { MaxDegreeOfParallelism = (int)parser.MaxDegreeOfParallelism };
        }

        /// <summary>
        /// Convert Parallel options Parser to Parallel options type.
        /// </summary>
        /// <param name="parallelOptions">Parallel Options.</param>
        /// <returns>Converted Parallel options Parser to Parallel options type.</returns>
        public static implicit operator ParallelOptionsParser(ParallelOptions parallelOptions)
        {
            if (parallelOptions == null)
                return null;

            return new ParallelOptionsParser() { MaxDegreeOfParallelism = (uint)parallelOptions.MaxDegreeOfParallelism };
        }
    }
}
