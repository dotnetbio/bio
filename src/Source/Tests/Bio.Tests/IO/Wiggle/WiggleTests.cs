using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Bio.IO.Wiggle;
using NUnit.Framework;

namespace Bio.Tests.IO.Wiggle
{
    /// <summary>
    /// Wiggle format parser and formatter.
    /// </summary>
    [TestFixture]
    public class WiggleTests
    {
        /// <summary>
        /// Tests creating an annotation object from scratch.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestAnnotationObject()
        {
            WiggleAnnotation an = CreateDummyAnnotation();
            VerifyDummyAnnotation(an);
        }

        /// <summary>
        /// Verifies an annotation object against a pre-defined set of values.
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
            return new WiggleAnnotation(new[] {
                new KeyValuePair<long, float>(100,10),
                new KeyValuePair<long, float>(200,20),
                new KeyValuePair<long, float>(300,30)
            }, "chromeee", 100);
        }

        /// <summary>
        /// Verifies the parser.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestWiggleParser()
        {
            string filepath = Path.Combine("TestUtils", "Wiggle", "variable.wig");

            TestParserVariableStep(filepath);

            filepath = Path.Combine("TestUtils", "Wiggle", "fixed.wig");

            TestParserFixedStep(filepath);
        }

        /// <summary>
        /// Verifies the formatter.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestWiggleFormatter()
        {
            string filepathTmp = Path.GetTempFileName();
            WiggleFormatter formatter = new WiggleFormatter();

            using (formatter.Open(filepathTmp))
            {
                formatter.Format(CreateDummyAnnotation());
            }

            WiggleParser parser = new WiggleParser();
            VerifyDummyAnnotation(parser.Parse(filepathTmp).First());
        }

        // Test wiggle fixed step
        private static WiggleAnnotation TestParserFixedStep(string filename)
        {
            WiggleParser p = new WiggleParser();
            WiggleAnnotation an = p.Parse(filename).First();

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
            WiggleAnnotation an = p.Parse(filename).First();

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

