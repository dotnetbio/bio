using System;
using System.Collections.Generic;
using System.Linq;

using Bio.Core.Extensions;
using Bio.IO.AppliedBiosystems.Model;
using Bio.Util;
using NUnit.Framework;

namespace Bio.Tests.Framework.IO.AppliedBiosystems
{
    /// <summary>
    /// Validates operations on ab1 sepecified data / metadata.
    /// </summary>
    [TestFixture]
    public class TestAb1Data
    {
        private static List<short> GetTestData(int count)
        {
            var data = new List<short>();
            var rand = new Random();

            for (int i = 0; i < count; i++)
            {
                data.Add((short)rand.Next(1, 1000));
            }

            return data;
        }

        private static void AssertAreEqual(IList<short> expected, IList<short> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        /// <summary>
        /// Tests operations to trim color data and ensure that peak locationg and other
        /// indicies are properly trimmed.
        /// </summary>
        [Test]
        [Category("Priority0")]
        public void TestColorDataTrim()
        {
            const int dataCount = 40;

            // 10 points
            var confidence = new short[] {22, 31, 50, 1, 61, 22, 10, 19, 23, 10};
            var peakLocations = new short[] {2, 5, 9, 12, 15, 22, 26, 32, 35, 38};
            short[] aData = GetTestData(dataCount).ToArray();
            short[] cData = GetTestData(dataCount).ToArray();
            short[] tData = GetTestData(dataCount).ToArray();
            short[] gData = GetTestData(dataCount).ToArray();

            var data = new Ab1Metadata {ConfidenceData = confidence.ToArray(), PeakLocations = peakLocations.ToArray()};
            data.SetColorData(Alphabets.DNA.A, new Ab1ColorData(peakLocations, aData));
            data.SetColorData(Alphabets.DNA.T, new Ab1ColorData(peakLocations, tData));
            data.SetColorData(Alphabets.DNA.C, new Ab1ColorData(peakLocations, cData));
            data.SetColorData(Alphabets.DNA.G, new Ab1ColorData(peakLocations, gData));

            const int segmentStartIndex = 3;
            const int segmentLength = 5;

            data.Trim(segmentStartIndex, segmentLength);

            var newPeakLocations = new List<short> {1, 4, 11, 15, 21};
            Assert.AreEqual(segmentLength, data.ConfidenceData.Length);
            Assert.AreEqual(segmentLength, data.PeakLocations.Length);

            AssertAreEqual(confidence.GetRange(segmentStartIndex, segmentLength), data.ConfidenceData.ToList());
            AssertAreEqual(newPeakLocations, data.PeakLocations.ToList());

            data.AllColorData()
                .ToList().ForEach(colorData =>
                                      {
                                          Assert.AreEqual(segmentLength, colorData.DataByResidue.Count);
                                          int offset = 0;
                                          for (int i = 0; i < colorData.DataByResidue.Count; i++)
                                          {
                                              Assert.AreEqual(newPeakLocations[i],
                                                              colorData.DataByResidue[i].PeakIndex + offset);
                                              offset += colorData.DataByResidue[i].Data.Length;
                                          }
                                      });
        }
    }
}
