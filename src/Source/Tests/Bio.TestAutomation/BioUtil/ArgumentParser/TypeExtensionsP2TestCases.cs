using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Util.ArgumentParser;
using System.Collections.ObjectModel;
using Bio.Util.Logging;

namespace Bio.TestAutomation.BioUtil.ArgumentParser
{
    /// <summary>
    /// P2 test cases for TypeExtensions class
    /// </summary>
    [TestClass]
    public class TypeExtensionsP2TestCases
    {
        #region Constructor
        
        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static TypeExtensionsP2TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor

        #region P2 Test Cases


        /// <summary>
        /// Invalidates GetImplementingTypes
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void InvalidateGetImplementingTypes()
        {
            try
            {
                IEnumerable<Type> types = TypeExtensions.GetImplementingTypes(typeof(OutputFile));
                Type type = types.ElementAt(0);
                Assert.Fail();
            }
            catch (ParseException pex)
            {
                ApplicationLog.WriteLine("Successfully caught ParseException : " + pex.Message);
            }
        }

        
        #endregion P2 Test Cases
    }
}
