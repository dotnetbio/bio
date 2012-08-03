using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bio;
using Bio.Algorithms.Alignment.MultipleSequenceAlignment;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests
{
    /// <summary>
    /// Test for KmerDistanceMatrixGenerator Class and KmerDistanceScoreCalculator class
    /// </summary>
    [TestClass]
    public class KmerDistanceMatrixGeneratorTests
    {
        /// <summary>
        /// Test KmerDistanceMatrixGenerator Class and KmerDistanceScoreCalculator class
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestKimuraDistanceMatrixGenerator()
        {
            List<ISequence> sequences = new List<ISequence>();
            sequences.Add(new Sequence(Alphabets.DNA, "ACGTAA"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAATCAATCAG"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAATCTTATCAG"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGACAAAATCAG"));

            int kmerLength = 3;

            // test kmer counting
            KmerDistanceScoreCalculator kmerDistanceScoreCalculator =
                new KmerDistanceScoreCalculator(kmerLength, Alphabets.AmbiguousDNA);

            Dictionary<String, float> countDictionaryA =
                KmerDistanceScoreCalculator.CalculateKmerCounting(sequences[0], kmerLength);
            Dictionary<String, float> countDictionaryB =
                KmerDistanceScoreCalculator.CalculateKmerCounting(sequences[1], kmerLength);

            Dictionary<String, float> expectedCountDictionaryA = new Dictionary<String, float>();
            expectedCountDictionaryA.Add("ACG", 1);
            expectedCountDictionaryA.Add("CGT", 1);
            expectedCountDictionaryA.Add("GTA", 1);
            expectedCountDictionaryA.Add("TAA", 1);

            Assert.AreEqual(countDictionaryA["ACG"], expectedCountDictionaryA["ACG"]);
            Assert.AreEqual(countDictionaryA["CGT"], expectedCountDictionaryA["CGT"]);
            Assert.AreEqual(countDictionaryA["GTA"], expectedCountDictionaryA["GTA"]);
            Assert.AreEqual(countDictionaryA["TAA"], expectedCountDictionaryA["TAA"]);

            Dictionary<String, float> expectedCountDictionaryB = new Dictionary<String, float>();
            expectedCountDictionaryB.Add("GGG", 1);
            expectedCountDictionaryB.Add("GGA", 1);
            expectedCountDictionaryB.Add("GAA", 1);
            expectedCountDictionaryB.Add("AAT", 2);
            expectedCountDictionaryB.Add("ATC", 2);
            expectedCountDictionaryB.Add("TCA", 2);
            expectedCountDictionaryB.Add("CAA", 1);
            expectedCountDictionaryB.Add("CAG", 1);

            Assert.AreEqual(countDictionaryB["GGG"], expectedCountDictionaryB["GGG"]);
            Assert.AreEqual(countDictionaryB["GGA"], expectedCountDictionaryB["GGA"]);
            Assert.AreEqual(countDictionaryB["GAA"], expectedCountDictionaryB["GAA"]);
            Assert.AreEqual(countDictionaryB["AAT"], expectedCountDictionaryB["AAT"]);
            Assert.AreEqual(countDictionaryB["ATC"], expectedCountDictionaryB["ATC"]);
            Assert.AreEqual(countDictionaryB["TCA"], expectedCountDictionaryB["TCA"]);
            Assert.AreEqual(countDictionaryB["CAA"], expectedCountDictionaryB["CAA"]);
            Assert.AreEqual(countDictionaryB["CAG"], expectedCountDictionaryB["CAG"]);

            foreach (var pair in countDictionaryA)
            {
                foreach (char s in pair.Key)
                {
                    Console.Write(s + " ");
                }
                Console.WriteLine(pair.Value);
            }
            foreach (var pair in countDictionaryB)
            {
                foreach (char s in pair.Key)
                {
                    Console.Write(s + " ");
                }
                Console.WriteLine(pair.Value);
            }

            float distanceScore = kmerDistanceScoreCalculator.CalculateDistanceScore(countDictionaryA, countDictionaryB);
            Console.WriteLine(distanceScore);

            PAMSAMMultipleSequenceAligner.parallelOption = new ParallelOptions { MaxDegreeOfParallelism = 2 };
            KmerDistanceMatrixGenerator kmerDistanceMatrixGenerator = new KmerDistanceMatrixGenerator(sequences, kmerLength, Alphabets.AmbiguousDNA);

            for (int i = 0; i < sequences.Count - 1; ++i)
            {
                for (int j = i + 1; j < sequences.Count; ++j)
                {
                    Console.WriteLine("Kmer Distance of sequence {0}, and {1} is: {2}", i, j, kmerDistanceMatrixGenerator.DistanceMatrix[i, j]);
                }
            }


            // test kmer counting CoVariance
            KmerDistanceScoreCalculator kmerDistanceScoreCalculatorB = new KmerDistanceScoreCalculator(kmerLength, Alphabets.AmbiguousDNA, DistanceFunctionTypes.CoVariance);

            countDictionaryA = KmerDistanceScoreCalculator.CalculateKmerCounting(sequences[0], kmerLength);
            countDictionaryB = KmerDistanceScoreCalculator.CalculateKmerCounting(sequences[1], kmerLength);

            distanceScore = kmerDistanceScoreCalculatorB.CalculateDistanceScore(countDictionaryA, countDictionaryB);
            Console.WriteLine(distanceScore);

            KmerDistanceMatrixGenerator kmerDistanceMatrixGeneratorB = new KmerDistanceMatrixGenerator(sequences, kmerLength, Alphabets.AmbiguousDNA, DistanceFunctionTypes.CoVariance);

            for (int i = 0; i < sequences.Count - 1; ++i)
            {
                for (int j = i + 1; j < sequences.Count; ++j)
                {
                    Console.WriteLine("Kmer Distance of sequence {0}, and {1} is: {2}", i, j, kmerDistanceMatrixGeneratorB.DistanceMatrix[i, j]);
                }
            }


            // test kmer counting ModifiedMUSCLE
            KmerDistanceScoreCalculator kmerDistanceScoreCalculatorC = new KmerDistanceScoreCalculator(kmerLength, Alphabets.AmbiguousDNA, DistanceFunctionTypes.ModifiedMUSCLE);

            countDictionaryA = KmerDistanceScoreCalculator.CalculateKmerCounting(sequences[0], kmerLength);
            countDictionaryB = KmerDistanceScoreCalculator.CalculateKmerCounting(sequences[1], kmerLength);

            distanceScore = kmerDistanceScoreCalculatorC.CalculateDistanceScore(countDictionaryA, countDictionaryB);
            Console.WriteLine(distanceScore);

            KmerDistanceMatrixGenerator kmerDistanceMatrixGeneratorC = new KmerDistanceMatrixGenerator(sequences, kmerLength, Alphabets.AmbiguousDNA, DistanceFunctionTypes.ModifiedMUSCLE);

            for (int i = 0; i < sequences.Count - 1; ++i)
            {
                for (int j = i + 1; j < sequences.Count; ++j)
                {
                    Console.WriteLine("Kmer Distance of sequence {0}, and {1} is: {2}", i, j, kmerDistanceMatrixGeneratorC.DistanceMatrix[i, j]);
                }
            }

            // test kmer counting PearsonCorrelation
            KmerDistanceScoreCalculator kmerDistanceScoreCalculatorD = new KmerDistanceScoreCalculator(kmerLength, Alphabets.AmbiguousDNA, DistanceFunctionTypes.PearsonCorrelation);

            countDictionaryA = KmerDistanceScoreCalculator.CalculateKmerCounting(sequences[0], kmerLength);
            countDictionaryB = KmerDistanceScoreCalculator.CalculateKmerCounting(sequences[1], kmerLength);

            distanceScore = kmerDistanceScoreCalculatorD.CalculateDistanceScore(countDictionaryA, countDictionaryB);
            Console.WriteLine(distanceScore);

            KmerDistanceMatrixGenerator kmerDistanceMatrixGeneratorD = new KmerDistanceMatrixGenerator(sequences, kmerLength, Alphabets.AmbiguousDNA, DistanceFunctionTypes.PearsonCorrelation);

            for (int i = 0; i < sequences.Count - 1; ++i)
            {
                for (int j = i + 1; j < sequences.Count; ++j)
                {
                    Console.WriteLine("Kmer Distance of sequence {0}, and {1} is: {2}", i, j, kmerDistanceMatrixGeneratorD.DistanceMatrix[i, j]);
                }
            }


            // Test for case 2
            sequences.Clear();
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAAAAATCAGATT"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAAAAATCAGATT"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGACAAAATCAG"));

            // test kmer counting

            countDictionaryA = KmerDistanceScoreCalculator.CalculateKmerCounting(sequences[0], kmerLength);
            countDictionaryB = KmerDistanceScoreCalculator.CalculateKmerCounting(sequences[1], kmerLength);

            foreach (var pair in countDictionaryA)
            {
                foreach (char s in pair.Key)
                {
                    Console.Write(s + " ");
                }
                Console.WriteLine(pair.Value);
            }
            foreach (var pair in countDictionaryB)
            {
                foreach (char s in pair.Key)
                {
                    Console.Write(s + " ");
                }
                Console.WriteLine(pair.Value);
            }

            distanceScore = kmerDistanceScoreCalculator.CalculateDistanceScore(countDictionaryA, countDictionaryB);
            Console.WriteLine(distanceScore);

            kmerDistanceMatrixGenerator = new KmerDistanceMatrixGenerator(sequences, kmerLength, Alphabets.AmbiguousDNA);

            for (int i = 0; i < sequences.Count - 1; ++i)
            {
                for (int j = i + 1; j < sequences.Count; ++j)
                {
                    Console.WriteLine("Kmer Distance of sequence {0}, and {1} is: {2}", i, j, kmerDistanceMatrixGenerator.DistanceMatrix[i, j]);
                }
            }

            // Test on larger dataset
            sequences = new List<ISequence>();
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAAAAATCAGATT"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAATCAAAATCAG"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAATCAAAATCAG"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAAATCG"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAATCAATCAG"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAATCTTATCAG"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGACAAAATCAG"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAAAAATCAGATT"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGAATCAAAATCAG"));
            sequences.Add(new Sequence(Alphabets.DNA, "GGGACAAAATCAG"));

            kmerLength = 4;
            kmerDistanceMatrixGenerator =
                new KmerDistanceMatrixGenerator(sequences, kmerLength, Alphabets.AmbiguousDNA, DistanceFunctionTypes.EuclideanDistance);

            kmerDistanceScoreCalculator = new KmerDistanceScoreCalculator(kmerLength, Alphabets.AmbiguousDNA, DistanceFunctionTypes.EuclideanDistance);
            for (int i = 0; i < kmerDistanceMatrixGenerator.DistanceMatrix.Dimension - 1; ++i)
            {
                for (int j = i + 1; j < kmerDistanceMatrixGenerator.DistanceMatrix.Dimension; ++j)
                {
                    countDictionaryA = KmerDistanceScoreCalculator.CalculateKmerCounting(sequences[i], kmerLength);
                    countDictionaryB = KmerDistanceScoreCalculator.CalculateKmerCounting(sequences[j], kmerLength);
                    MsaUtils.Normalize(countDictionaryA);
                    MsaUtils.Normalize(countDictionaryB);
                    float score = kmerDistanceScoreCalculator.CalculateDistanceScore(countDictionaryA, countDictionaryB);
                    Console.WriteLine("{0}-{1}: {2}", i, j, score);
                    Console.WriteLine("{0}-{1}: {2}", i, j, kmerDistanceMatrixGenerator.DistanceMatrix[i, j]);
                    // Assert.AreEqual(score, kmerDistanceMatrixGenerator.DistanceMatrix[i, j]);
                }
            }
        }
    }
}
