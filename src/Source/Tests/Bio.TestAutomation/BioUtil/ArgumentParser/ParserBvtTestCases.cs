using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bio.Util.ArgumentParser;
using System.Collections.ObjectModel;

namespace Bio.TestAutomation.BioUtil.ArgumentParser
{
    enum enumForParse
    {
        one, two, three
    }
    /// <summary>
    /// BVT Test Cases for ArgumentParser Parser Class
    /// </summary>
    [TestClass]
    public class ParserBvtTestCases
    {

        /// <summary>
        /// Validates HasParseMethod
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateHasParseMethod()
        {
            string str = string.Empty; 
            Assert.IsFalse(Parser.HasParseMethod(str.GetType()));
            int num = 1;
            Assert.IsTrue(Parser.HasParseMethod(num.GetType()));
            double d = 1;
            Assert.IsTrue(Parser.HasParseMethod(d.GetType()));
            float f = 1 ;
            Assert.IsTrue(Parser.HasParseMethod(f.GetType()));
            bool b = true;
            Assert.IsTrue(Parser.HasParseMethod(b.GetType()));

        }

        /// <summary>
        /// Validates ParseAll
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateParseAll()
        {
            IEnumerable<string> stringVals;
            Collection<string> strColl = new Collection<string>();
            strColl.Add("1"); strColl.Add("2");
            stringVals = strColl.OrderBy(a => a);
            IEnumerable<int> resultInt = Parser.ParseAll<int>(stringVals);
            Assert.AreEqual(1, resultInt.ElementAt(0));
            Assert.AreEqual(2, resultInt.ElementAt(1));
        }

        /// <summary>
        /// Validates TryParseAll
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateTryParseAll()
        {
            IEnumerable<string> stringVals;
            Collection<string> strColl = new Collection<string>();
            strColl.Add("1"); strColl.Add("2"); 
            stringVals = strColl.OrderBy(a => a);
            IList<int> resultInt;
            Assert.IsTrue(Parser.TryParseAll(stringVals, out resultInt));
            Assert.AreEqual(1, resultInt.ElementAt(0));
            Assert.AreEqual(2, resultInt.ElementAt(1));

            strColl.Clear();
            strColl.Add("true"); strColl.Add("false");
            stringVals = strColl.OrderBy(a => a);
            IList<bool> resultBool;
            Assert.IsTrue(Parser.TryParseAll(stringVals, out resultBool));
            Assert.AreEqual(false, resultBool.ElementAt(0));
            Assert.AreEqual(true, resultBool.ElementAt(1));

            strColl.Clear();
            strColl.Add("1.23"); strColl.Add("7.98");
            stringVals = strColl.OrderBy(a => a);
            IList<float> resultFloat;
            Assert.IsTrue(Parser.TryParseAll(stringVals, out resultFloat));
            Assert.AreEqual(1.23f, resultFloat.ElementAt(0));
            Assert.AreEqual(7.98f, resultFloat.ElementAt(1));
        }

        /// <summary>
        /// Validates Parse
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateParse()
        {
            string intStr = "1";
            int num = 0;
            object result = Parser.Parse(intStr, num.GetType());
            num = (int)result;
            Assert.AreEqual(1, num);

            string boolStr = "true";
            bool check = false;
            result = Parser.Parse(boolStr, check.GetType());
            check=(bool)result;
            Assert.IsTrue(check);

            string floatStr ="1.23";
            float f = 1;
            result = Parser.Parse(floatStr,f.GetType());
            f = (float)result;
            Assert.AreEqual(1.23f, f);
        }


        /// <summary>
        /// Validates Parse<T>
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateParseT()
        {
            int num = Parser.Parse<int>("1");
            Assert.AreEqual(1, num);

            float f = Parser.Parse<float>("1.23");
            Assert.AreEqual(1.23f, f);

            bool check = Parser.Parse<bool>("true");
            Assert.IsTrue(check);

            Assert.AreEqual(0, Parser.Parse<int>(null));
            Assert.IsFalse(Parser.Parse<bool>(null));
            Assert.AreEqual(0, Parser.Parse<float>(null));

            Assert.AreEqual(0, Parser.Parse<int>(""));
            Assert.IsFalse(Parser.Parse<bool>(""));
            Assert.AreEqual(0, Parser.Parse<float>(""));
        }

        /// <summary>
        /// Validates TryParse<T>
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void ValidateTryParseT()
        {
            int num = 0;
            Assert.IsTrue(Parser.TryParse<int>("5", out num));
            float f = 0;
            Assert.IsTrue(Parser.TryParse<float>("1.23", out f));
            bool check = false;
            Assert.IsTrue(Parser.TryParse<bool>("true", out check));
            int? nullable = null;
            Assert.IsTrue(Parser.TryParse<int?>("null", out nullable));
            Assert.IsTrue(Parser.TryParse<int?>("", out nullable));
            Assert.IsTrue(Parser.TryParse<int?>("3", out nullable));

            enumForParse i = 0;
            Assert.IsTrue(Parser.TryParse<enumForParse>("one", out i));

            string str = string.Empty;
            Assert.IsTrue(Parser.TryParse<string>("strToParse", out str));

            Collection<int> intColl = new Collection<int>();
            intColl.Add(1); intColl.Add(2);
            Collection<int> resultColl;
            Assert.IsTrue(Parser.TryParse<Collection<int>>("1", out resultColl));
            Assert.IsTrue(Parser.TryParse<Collection<int>>("", out resultColl));
            Assert.IsTrue(Parser.TryParse<Collection<int>>("null", out resultColl));
            Assert.IsTrue(Parser.TryParse<Collection<int>>("(1)", out resultColl));

          
        }

    }
}
