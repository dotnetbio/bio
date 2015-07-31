using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Util.Logging;
using Bio.Util.ArgumentParser;
using System.Collections.ObjectModel;

namespace Bio.TestAutomation.BioUtil.ArgumentParser
{
    /// <summary>
    /// P2 Test cases for TypeFactory Class
    /// </summary>
    [TestClass]
    public class TypeFactoryP2TestCases
    {
        #region Constructor
        
        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static TypeFactoryP2TestCases()
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
        /// Invalidates TryGetType
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateTryGetType()
        {
            try
            {
                Type returnType;
                int num = 0;
                bool check = TypeFactory.TryGetType(null, num.GetType(), out returnType);
                Assert.Fail();
            }
            catch (ArgumentNullException anex)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentNullException : " + anex.Message);
            }
            try
            {
                Type returnType;
                bool check = TypeFactory.TryGetType("int", null, out returnType);
                Assert.Fail();
            }
            catch (ArgumentNullException anex)
            {
                ApplicationLog.WriteLine("Successfully caught ArgumentNullException : " + anex.Message);
            }

            try
            {
                Type returnType;
                Collection<int> intcoll = new Collection<int>();
                intcoll.Add(1);
                bool check = TypeFactory.TryGetType("<>", intcoll.GetType(), out returnType);
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                ApplicationLog.WriteLine("Successfully caught Exception : " + ex.Message);
            }
        }


        #endregion P2 Test Cases
    }
}
