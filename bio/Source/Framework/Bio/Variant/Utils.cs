using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bio.Variant
{
    public static class Utils
    {        
        public static double GetLog10ErrorProbability(int phredScore)
        {
            return phredScore / -10.0;
        }
    }
}
