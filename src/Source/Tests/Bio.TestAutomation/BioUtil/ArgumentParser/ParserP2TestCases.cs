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
    /// P2 Test Cases for ArgumentParser Parser Class
    /// </summary>
    [TestClass]
    public class ParserP2TestCases
    {
        #region Constructor
        
        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static ParserP2TestCases()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("bio.automation.log");
            }
        }

        #endregion Constructor
        /// <summary>
        /// Invalidates Parse<T>
        /// </summary>
        [TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateParseT()
        {
            try
            {
                int num = Parser.Parse<int>("this is not an int");
                Assert.Fail();
            }
            catch (ParseException pex)
            {
                ApplicationLog.WriteLine("Successfully caught ParseException : " + pex.Message);
            }
        }

        /// <summary>
        /// Invalidates TryParse<T>
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), TestMethod]
        [Priority(2)]
        [TestCategory("Priority2")]
        public void InvalidateTryParseT()
        {
            try
            {
                int num = 0;
                bool result = Parser.TryParse<int>("help", out num);
                Assert.Fail();
            }
            catch (Exception pex)
            {
                ApplicationLog.WriteLine("Successfully caught HelpException : " + pex.Message);
            }

            try
            {
                int num = 0;
                bool result = Parser.TryParse<int>("help!", out num);
                Assert.Fail();
            }
            catch (Exception pex)
            {
                ApplicationLog.WriteLine("Successfully caught HelpException : " + pex.Message);
            }
        }
    }
}
