using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Util.ArgumentParser;
using System.Collections.ObjectModel;
using Bio.Util.Logging;
using System.IO;
using Bio;
using System.Threading;
using Bio.Util.Distribute;
using Bio.Util;

namespace Bio.TestAutomation.BioUtil.ArgumentParser
{
    /// <summary>
    /// BVT Test Cases for ArgumentCollection Class
    /// </summary>
    [TestClass]
    public class ArgumentCollectionBvtTestCases
    {

        /// <summary>
        /// Validates EnumerateValuesOfTypeFromParsable
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateEnumerateValuesOfTypeFromParsable()
        {
            int num = 5;
            object obj = num;
            IEnumerable<int> integers = ArgumentCollection.EnumerateValuesOfTypeFromParsable<int>(obj);
            Assert.AreEqual(5, integers.ElementAt(0));
            Assert.AreEqual(null, ArgumentCollection.EnumerateValuesOfTypeFromParsable<int>(null));

            OutputFile outFile = new OutputFile();
            outFile.FullName = "Outfile";
            obj = outFile;
            IEnumerable<OutputFile> outFiles = ArgumentCollection.EnumerateValuesOfTypeFromParsable<OutputFile>(obj);
            Assert.AreEqual("Outfile", outFiles.ElementAt(0).FullName);

            IEnumerable<ParsableFile> parsableFiles = ArgumentCollection.EnumerateValuesOfTypeFromParsable<ParsableFile>(obj);
            Assert.AreEqual("Outfile", parsableFiles.ElementAt(0).FullName);
        }


        /// <summary>
        /// Validates CreateHelpMessage
        /// </summary>
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateCreateHelpMessage()
        {
            Type t = typeof(AggregateNumbers);
            bool includeDateStamp = true;
            HelpException hex = ArgumentCollection.CreateHelpMessage(t, includeDateStamp);
            Assert.IsNotNull(hex.Message);
            Assert.IsTrue(hex.Message.Length > 0);
        }

    }
}
