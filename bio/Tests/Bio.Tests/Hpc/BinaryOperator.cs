using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bio.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBinaryOperator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        double Aggregate(double x, double y);
    }
    /// <summary>
    /// 
    /// </summary>
    public class Sum : IBinaryOperator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public double Aggregate(double x, double y)
        {
            return x + y;
        }
    }
}
