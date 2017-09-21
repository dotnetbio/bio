using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bio;
using Bio.Algorithms.Alignment.MultipleSequenceAlignment;
using Bio.IO.FastA;
using Bio.SimilarityMatrices;
using NUnit.Framework;
using Bio.Tests;

namespace Bio.Pamsam.Tests
{
    /// <summary>
    /// MsaBenchMarkTest
    /// </summary>
    [TestFixture]
    public class MsaBenchMarkTests
    {

        /// <summary>
        /// Test MsaBenchMark
        /// </summary>
        [Test]
        public void TestMsaBenchMark()
        {
            string fileDirectory = @"TestUtils\Fasta\Protein\Balibase\RV911\".TestDir();
            DirectoryInfo iD = new DirectoryInfo(fileDirectory);

            PAMSAMMultipleSequenceAligner.FasterVersion = false;
            PAMSAMMultipleSequenceAligner.UseWeights = false;
            PAMSAMMultipleSequenceAligner.UseStageB = true;
            PAMSAMMultipleSequenceAligner.NumberOfCores = 2;
            
            SimilarityMatrix similarityMatrix;
            int gapOpenPenalty = -20;
            int gapExtendPenalty = -5;
            int kmerLength = 4;

            int numberOfDegrees = 2;//Environment.ProcessorCount;
            int numberOfPartitions = 16;// Environment.ProcessorCount * 2;

            DistanceFunctionTypes distanceFunctionName = DistanceFunctionTypes.EuclideanDistance;
            UpdateDistanceMethodsTypes hierarchicalClusteringMethodName = UpdateDistanceMethodsTypes.Average;
            ProfileAlignerNames profileAlignerName = ProfileAlignerNames.NeedlemanWunschProfileAligner;
            ProfileScoreFunctionNames profileProfileFunctionName = ProfileScoreFunctionNames.WeightedInnerProductCached;
            
            similarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Blosum62);

            List<float> allQ = new List<float>();
            List<float> allTC = new List<float>();

            foreach (FileInfo fi in iD.GetFiles())
            {
                String filePath = fi.FullName;
                Console.WriteLine(filePath);
                FastAParser parser = new FastAParser();

                parser.Alphabet = AmbiguousProteinAlphabet.Instance;
                IList<ISequence> orgSequences = parser.Parse(filePath).ToList();

                List<ISequence> sequences = MsaUtils.UnAlign(orgSequences);

                int numberOfSequences = orgSequences.Count;

                Console.WriteLine("The number of sequences is: {0}", numberOfSequences);
                Console.WriteLine("Original unaligned sequences are:");
                for (int i = 0; i < numberOfSequences; ++i)
                {
                    //Console.WriteLine(sequences[i].ToString());
                }
                Console.WriteLine("Original aligned sequences are:");
                for (int i = 0; i < numberOfSequences; ++i)
                {
                    //Console.WriteLine(orgSequences[i].ToString());
                }

                PAMSAMMultipleSequenceAligner msa = new PAMSAMMultipleSequenceAligner
                    (sequences, kmerLength, distanceFunctionName, hierarchicalClusteringMethodName,
                    profileAlignerName, profileProfileFunctionName, similarityMatrix, gapOpenPenalty, gapExtendPenalty,
                    numberOfPartitions, numberOfDegrees);

                Console.WriteLine("Aligned sequences in stage 1: {0}", msa.AlignmentScoreA);
                for (int i = 0; i < msa.AlignedSequencesA.Count; ++i)
                {
                    //Console.WriteLine(msa.AlignedSequencesA[i].ToString());
                }
                Console.WriteLine("Aligned sequences in stage 2: {0}", msa.AlignmentScoreB);
                for (int i = 0; i < msa.AlignedSequencesB.Count; ++i)
                {
                    //Console.WriteLine(msa.AlignedSequencesB[i].ToString());
                }
                Console.WriteLine("Aligned sequences in stage 3: {0}", msa.AlignmentScoreC);
                for (int i = 0; i < msa.AlignedSequencesC.Count; ++i)
                {
                    //Console.WriteLine(msa.AlignedSequencesC[i].ToString());
                }

                Console.WriteLine("Aligned sequences final: {0}", msa.AlignmentScore);
                for (int i = 0; i < msa.AlignedSequences.Count; ++i)
                {
                    //Console.WriteLine(msa.AlignedSequences[i].ToString());
                }
                float scoreQ = MsaUtils.CalculateAlignmentScoreQ(msa.AlignedSequences, orgSequences);
                float scoreTC = MsaUtils.CalculateAlignmentScoreTC(msa.AlignedSequences, orgSequences);
                allQ.Add(scoreQ);
                allTC.Add(scoreTC);
                Console.WriteLine("Alignment score Q is: {0}", scoreQ);
                Console.WriteLine("Alignment score TC is: {0}", scoreTC);
            }
            Console.WriteLine("Number of datasets is: {0}", allQ.Count);
            Console.WriteLine("average Q score is: {0}", MsaUtils.Mean(allQ.ToArray()));
            Console.WriteLine("average TC score is: {0}", MsaUtils.Mean(allTC.ToArray()));

        }

        /// <summary>
        /// Test MsaBenchMark on very large dataset
        /// </summary>
        [Test]
        public void TestMsaBenchMarkLargeDataset()
        {
            // Test on DNA benchmark dataset
            string filePathObj = @"TestUtils\BOX032Small.xml.afa".TestDir();
            var orgSequences = new FastAParser().Parse(filePathObj).ToList();

            var sequences = MsaUtils.UnAlign(orgSequences);
            int numberOfSequences = orgSequences.Count;
            Assert.AreEqual(numberOfSequences, sequences.Count);

            string outputFilePath = Path.GetTempFileName();
            try
            {
                using (StreamWriter writer = new StreamWriter(outputFilePath, true))
                {
                    foreach (ISequence sequence in sequences)
                    {
                        // write sequence
                        writer.WriteLine(">" + sequence.ID);
                        for (int lineStart = 0; lineStart < sequence.Count; lineStart += 60)
                            writer.WriteLine(new String(sequence.Skip(lineStart).Take((int)Math.Min(60, sequence.Count - lineStart)).Select(a => (char)a).ToArray()));
                        writer.Flush();
                    }
                }
                sequences = new FastAParser().Parse(outputFilePath).ToList();
            }
            finally
            {
                File.Delete(outputFilePath);
            }

            Console.WriteLine("Original sequences are:");
            sequences.ForEach(Console.WriteLine);
            
            Console.WriteLine("Benchmark sequences are:");
            orgSequences.ForEach(Console.WriteLine);

            // Begin alignment
            PAMSAMMultipleSequenceAligner.FasterVersion = false;
            PAMSAMMultipleSequenceAligner.UseWeights = false;
            PAMSAMMultipleSequenceAligner.UseStageB = true;
            PAMSAMMultipleSequenceAligner.NumberOfCores = 2;

            int gapOpenPenalty = -13;
            int gapExtendPenalty = -5;
            int kmerLength = 3;
            int numberOfDegrees = 2;
            int numberOfPartitions = 16;

            SimilarityMatrix similarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Blosum62);

            DistanceFunctionTypes distanceFunctionName = DistanceFunctionTypes.EuclideanDistance;
            UpdateDistanceMethodsTypes hierarchicalClusteringMethodName = UpdateDistanceMethodsTypes.Average;
            ProfileAlignerNames profileAlignerName = ProfileAlignerNames.NeedlemanWunschProfileAligner;
            ProfileScoreFunctionNames profileProfileFunctionName = ProfileScoreFunctionNames.WeightedInnerProduct;

            PAMSAMMultipleSequenceAligner msa = new PAMSAMMultipleSequenceAligner
               (sequences, kmerLength, distanceFunctionName, hierarchicalClusteringMethodName,
               profileAlignerName, profileProfileFunctionName, similarityMatrix, gapOpenPenalty, gapExtendPenalty,
               numberOfPartitions, numberOfDegrees);

            Console.WriteLine("Benchmark SPS score is: {0}", MsaUtils.MultipleAlignmentScoreFunction(orgSequences, similarityMatrix, gapOpenPenalty, gapExtendPenalty));

            Console.WriteLine("Aligned sequences in stage 1: {0}", msa.AlignmentScoreA);
            for (int i = 0; i < msa.AlignedSequencesA.Count; ++i)
                Console.WriteLine(msa.AlignedSequencesA[i]);
            Console.WriteLine("Alignment score Q is: {0}", MsaUtils.CalculateAlignmentScoreQ(msa.AlignedSequencesA, orgSequences));
            Console.WriteLine("Alignment score TC is: {0}", MsaUtils.CalculateAlignmentScoreTC(msa.AlignedSequencesA, orgSequences));

            Console.WriteLine("Aligned sequences in stage 2: {0}", msa.AlignmentScoreB);
            for (int i = 0; i < msa.AlignedSequencesB.Count; ++i)
                Console.WriteLine(msa.AlignedSequencesB[i]);
            Console.WriteLine("Alignment score Q is: {0}", MsaUtils.CalculateAlignmentScoreQ(msa.AlignedSequencesB, orgSequences));
            Console.WriteLine("Alignment score TC is: {0}", MsaUtils.CalculateAlignmentScoreTC(msa.AlignedSequencesB, orgSequences));

            Console.WriteLine("Aligned sequences in stage 3: {0}", msa.AlignmentScoreC);
            for (int i = 0; i < msa.AlignedSequencesC.Count; ++i)
                Console.WriteLine(msa.AlignedSequencesC[i]);
            Console.WriteLine("Alignment score Q is: {0}", MsaUtils.CalculateAlignmentScoreQ(msa.AlignedSequencesC, orgSequences));
            Console.WriteLine("Alignment score TC is: {0}", MsaUtils.CalculateAlignmentScoreTC(msa.AlignedSequencesC, orgSequences));

            Console.WriteLine("Aligned sequences final: {0}", msa.AlignmentScore);
            for (int i = 0; i < msa.AlignedSequences.Count; ++i)
                Console.WriteLine(msa.AlignedSequences[i]);
            Console.WriteLine("Alignment score Q is: {0}", MsaUtils.CalculateAlignmentScoreQ(msa.AlignedSequences, orgSequences));
            Console.WriteLine("Alignment score TC is: {0}", MsaUtils.CalculateAlignmentScoreTC(msa.AlignedSequences, orgSequences));
        }

        /// <summary>
        /// Test on SABmark
        /// </summary>
        [Test]
        public void TestMsaBenchMarkOnSABmark()
        {
            List<float> allQ = new List<float>();
            List<float> allTC = new List<float>();

            string fileDirectory = @"TestUtils\Fasta\Protein\SABmark".TestDir();
            DirectoryInfo iD = new DirectoryInfo(fileDirectory);

            PAMSAMMultipleSequenceAligner.FasterVersion = false;
            PAMSAMMultipleSequenceAligner.UseWeights = false;
            PAMSAMMultipleSequenceAligner.UseStageB = true;
            PAMSAMMultipleSequenceAligner.NumberOfCores = 2;

            SimilarityMatrix similarityMatrix;
            int gapOpenPenalty = -13;
            int gapExtendPenalty = -5;
            int kmerLength = 3;

            int numberOfDegrees = 2;//Environment.ProcessorCount;
            int numberOfPartitions = 16;// Environment.ProcessorCount * 2;

            DistanceFunctionTypes distanceFunctionName = DistanceFunctionTypes.EuclideanDistance;
            UpdateDistanceMethodsTypes hierarchicalClusteringMethodName = UpdateDistanceMethodsTypes.Average;
            ProfileAlignerNames profileAlignerName = ProfileAlignerNames.NeedlemanWunschProfileAligner;
            ProfileScoreFunctionNames profileProfileFunctionName = ProfileScoreFunctionNames.WeightedInnerProduct;

            similarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.Blosum62);

            foreach (DirectoryInfo fi in iD.GetDirectories())
            {
                foreach (DirectoryInfo fii in fi.GetDirectories())
                {
                    foreach (FileInfo fiii in fii.GetFiles())
                    {
                        String filePath = fiii.FullName;
                        Console.WriteLine(filePath);
                        FastAParser parser = new FastAParser();

                        IList<ISequence> orgSequences = parser.Parse(filePath).ToList();

                        List<ISequence> sequences = MsaUtils.UnAlign(orgSequences);

                        int numberOfSequences = orgSequences.Count;

                        Console.WriteLine("The number of sequences is: {0}", numberOfSequences);
                        Console.WriteLine("Original unaligned sequences are:");

                        PAMSAMMultipleSequenceAligner msa = new PAMSAMMultipleSequenceAligner
                            (sequences, kmerLength, distanceFunctionName, hierarchicalClusteringMethodName,
                            profileAlignerName, profileProfileFunctionName, similarityMatrix, gapOpenPenalty, gapExtendPenalty,
                            numberOfPartitions, numberOfDegrees);

                        Console.WriteLine("Aligned sequences final: {0}", msa.AlignmentScore);
                        for (int i = 0; i < msa.AlignedSequences.Count; ++i)
                        {
                            //Console.WriteLine(msa.AlignedSequences[i].ToString());
                        }
                        float scoreQ = MsaUtils.CalculateAlignmentScoreQ(msa.AlignedSequences, orgSequences);
                        float scoreTC = MsaUtils.CalculateAlignmentScoreTC(msa.AlignedSequences, orgSequences);
                        allQ.Add(scoreQ);
                        allTC.Add(scoreTC);
                        Console.WriteLine("Alignment score Q is: {0}", scoreQ);
                        Console.WriteLine("Alignment score TC is: {0}", scoreTC);

                        if (allQ.Count % 1000 == 0)
                        {
                            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                            Console.WriteLine("average Q score is: {0}", MsaUtils.Mean(allQ.ToArray()));
                            Console.WriteLine("average TC score is: {0}", MsaUtils.Mean(allTC.ToArray()));
                        }
                    }
                }
            }

            Console.WriteLine("average Q score is: {0}", MsaUtils.Mean(allQ.ToArray()));
            Console.WriteLine("average TC score is: {0}", MsaUtils.Mean(allTC.ToArray()));

        }

        /// <summary>
        /// Test on SABmark
        /// </summary>
        [Test]
        public void TestMsaBenchMarkOnBralibase()
        {
            var allQ = new List<float>();
            var allTC = new List<float>();

            string fileDirectory = @"TestUtils\Fasta\RNA\k10".TestDir();
            DirectoryInfo iD = new DirectoryInfo(fileDirectory);

            PAMSAMMultipleSequenceAligner.FasterVersion = false;
            PAMSAMMultipleSequenceAligner.UseWeights = false;
            PAMSAMMultipleSequenceAligner.UseStageB = false;
            PAMSAMMultipleSequenceAligner.NumberOfCores = 2;

            var similarityMatrix = new SimilarityMatrix(SimilarityMatrix.StandardSimilarityMatrix.AmbiguousRna); ;
            int gapOpenPenalty = -20;
            int gapExtendPenalty = -5;
            int kmerLength = 4;

            int numberOfDegrees = 2;
            int numberOfPartitions = 16;

            DistanceFunctionTypes distanceFunctionName = DistanceFunctionTypes.EuclideanDistance;
            UpdateDistanceMethodsTypes hierarchicalClusteringMethodName = UpdateDistanceMethodsTypes.Average;
            ProfileAlignerNames profileAlignerName = ProfileAlignerNames.NeedlemanWunschProfileAligner;
            ProfileScoreFunctionNames profileProfileFunctionName = ProfileScoreFunctionNames.WeightedInnerProductCached;

            foreach (DirectoryInfo fi in iD.GetDirectories())
            {
                foreach (FileInfo fiii in fi.GetFiles())
                {
                    String filePath = fiii.FullName;
                    Console.WriteLine($"Loading: {filePath}");

                    var orgSequences = new FastAParser() { Alphabet = AmbiguousRnaAlphabet.Instance }.Parse(filePath).ToList();
                    var sequences = MsaUtils.UnAlign(orgSequences);

                    int numberOfSequences = orgSequences.Count;
                    Console.WriteLine("The number of sequences is: {0}", numberOfSequences);

                    PAMSAMMultipleSequenceAligner msa = new PAMSAMMultipleSequenceAligner
                        (sequences, kmerLength, distanceFunctionName, hierarchicalClusteringMethodName,
                        profileAlignerName, profileProfileFunctionName, similarityMatrix, gapOpenPenalty, gapExtendPenalty,
                        numberOfPartitions, numberOfDegrees);

                    Console.WriteLine("Aligned sequences final: {0}", msa.AlignmentScore);

                    float scoreQ = MsaUtils.CalculateAlignmentScoreQ(msa.AlignedSequences, orgSequences);
                    float scoreTC = MsaUtils.CalculateAlignmentScoreTC(msa.AlignedSequences, orgSequences);
                    Console.WriteLine("Alignment score Q is: {0}", scoreQ);
                    Console.WriteLine("Alignment score TC is: {0}", scoreTC);

                    allQ.Add(scoreQ);
                    allTC.Add(scoreTC);

                    if (allQ.Count % 1000 == 0)
                    {
                        Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                        Console.WriteLine("average Q score is: {0}", MsaUtils.Mean(allQ.ToArray()));
                        Console.WriteLine("average TC score is: {0}", MsaUtils.Mean(allTC.ToArray()));
                    }
                }
            }
            Console.WriteLine("number of datasets is: {0}", allQ.Count);
            Console.WriteLine("average Q score is: {0}", MsaUtils.Mean(allQ.ToArray()));
            Console.WriteLine("average TC score is: {0}", MsaUtils.Mean(allTC.ToArray()));
        }
    }
}
