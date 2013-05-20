using System;
using System.Collections.Generic;
using System.IO;
using Bio.IO.Wiggle;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests.IO.Wiggle
{
    /// <summary>
    /// Wiggle format parser and formatter.
    /// </summary>
    [TestClass]
    public class WiggleTests
    {
        /// <summary>
        /// Initialize static member of the class. Static constructor to open log and make other settings needed for test
        /// </summary>
        static WiggleTests()
        {
            Trace.Set(Trace.SeqWarnings);
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("Bio.Tests.log");
            }
        }

        /// <summary>
        /// Tests creating an annotation object from scratch.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestAnnotationObject()
        {
            WiggleAnnotation an = CreateDummyAnnotation();
            VerifyDummyAnnotation(an);
        }

        /// <summary>
        /// Verifies an annotation object agains a pre defined set of values.
        /// </summary>
        /// <param name="an"></param>
        private static void VerifyDummyAnnotation(WiggleAnnotation an)
        {
            Assert.IsTrue(an.Chromosome == "chromeee");
            Assert.IsTrue(an.Span == 100);

            try
            {
                an.GetValueArray(0, 3);
                Assert.Fail();
            }
            catch(NotSupportedException)
            { }

            var x = an.GetEnumerator();
            x.MoveNext();
            Assert.IsTrue(x.Current.Key == 100); Assert.IsTrue(x.Current.Value == 10);
            x.MoveNext();
            Assert.IsTrue(x.Current.Key == 200); Assert.IsTrue(x.Current.Value == 20);
            x.MoveNext();
            Assert.IsTrue(x.Current.Key == 300); Assert.IsTrue(x.Current.Value == 30);
        }

        /// <summary>
        /// Creates a new annotation object.
        /// </summary>
        /// <returns>Dummy annotation object.</returns>
        private static WiggleAnnotation CreateDummyAnnotation()
        {
            return new WiggleAnnotation(new KeyValuePair<long, float>[] {
                new KeyValuePair<long, float>(100,10),
                new KeyValuePair<long, float>(200,20),
                new KeyValuePair<long, float>(300,30)
            }, "chromeee", 100);
        }

        /// <summary>
        /// Verifies the parser.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestWiggleParser()
        {
            string assemblypath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
            string filepath = assemblypath + @"\TestUtils\Wiggle\variable.wig";

            TestParserVariableStep(filepath);

            filepath = assemblypath + @"\TestUtils\Wiggle\fixed.wig";

            TestParserFixedStep(filepath);
        }

        /// <summary>
        /// Verifies the formatter.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestWiggleFormatter()
        {
            string filepathTmp = Path.GetTempFileName();

            using (WiggleFormatter formatter = new WiggleFormatter(filepathTmp))
            {
                formatter.Write(CreateDummyAnnotation());
            }

            WiggleParser parser = new WiggleParser();
            VerifyDummyAnnotation(parser.Parse(filepathTmp));
        }

        // Test wiggle fixed step
        private static WiggleAnnotation TestParserFixedStep(string filename)
        {
            WiggleParser p = new WiggleParser();
            WiggleAnnotation an = p.Parse(filename);

            Assert.IsTrue(an.Chromosome == "chr19");
            Assert.IsTrue(an.BasePosition == 59307401);
            Assert.IsTrue(an.Step == 300);
            Assert.IsTrue(an.Span == 200);
            
            Assert.IsTrue(an.Metadata["type"] == "wiggle_0");
            Assert.IsTrue(an.Metadata["name"] == "ArrayExpt1");
            Assert.IsTrue(an.Metadata["description"] == "20 degrees, 2 hr");

            float[] values = an.GetValueArray(0, 3);
            Assert.IsTrue(values[0] == 1000);
            Assert.IsTrue(values[1] == 900);
            Assert.IsTrue(values[2] == 800);

            values = an.GetValueArray(7, 3);
            Assert.IsTrue(values[0] == 300);
            Assert.IsTrue(values[1] == 200);
            Assert.IsTrue(values[2] == 100);

            return an;
        }

        // Test wiggle variable step
        private static WiggleAnnotation TestParserVariableStep(string filename)
        {
            WiggleParser p = new WiggleParser();
            WiggleAnnotation an = p.Parse(filename);

            Assert.IsTrue(an.Chromosome == "chr19");
            Assert.IsTrue(an.Step == 0);
            Assert.IsTrue(an.BasePosition == 0);
            Assert.IsTrue(an.Span == 150);
            
            Assert.IsTrue(an.Metadata["type"] == "wiggle_0");
            Assert.IsTrue(an.Metadata["name"] == "ArrayExpt1");
            Assert.IsTrue(an.Metadata["description"] == "20 degrees, 2 hr");

            var x = an.GetEnumerator();
            x.MoveNext();
            Assert.IsTrue(x.Current.Key == 59304701); Assert.IsTrue(x.Current.Value == 10.0);
            x.MoveNext();
            Assert.IsTrue(x.Current.Key == 59304901); Assert.IsTrue(x.Current.Value == 12.5);
            x.MoveNext();
            Assert.IsTrue(x.Current.Key == 59305401); Assert.IsTrue(x.Current.Value == 15.0);
            x.MoveNext(); x.MoveNext(); x.MoveNext(); x.MoveNext(); x.MoveNext(); x.MoveNext();
            Assert.IsTrue(x.Current.Key == 59307871); Assert.IsTrue(x.Current.Value == 10.0);

            return an;
        }
    }
}

