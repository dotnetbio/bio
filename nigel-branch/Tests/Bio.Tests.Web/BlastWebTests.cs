using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Bio.TestUtils.SimulatorUtility;
using Bio.Web;
using Bio.Web.Blast;
using Bio.Util.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bio.Tests.Web
{
    /// <summary>
    /// Test the BLAST web query services.
    /// </summary>
    [TestClass]
    public class BlastWebTests
    {
        private static readonly TestCaseSimulator TestCaseSimulator;

        /// <summary>
        /// Static constructor to open log and make other settings needed for test
        /// </summary>
        static BlastWebTests()
        {
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open("Bio.Tests.log");
            }

            TestCaseSimulator = new TestCaseSimulator();
        }

        /// <summary>
        /// Test case Id TestNcbiQBlast_Protein
        /// </summary>
        private const string TESTCASE_TESTNCBIQBLASTPROTEIN = "TestNcbiQBlast_Protein";

        /// <summary>
        /// Test case Id TestNcbiQBlast_Protein_UserQuery
        /// </summary>
        private const string TESTCASE_TESTNCBIQBLASTPROTEINUSERQUERY = "TestNcbiQBlast_Protein_UserQuery";

        /// <summary>
        /// Test case Id TestNcbiQBlast_Dna
        /// </summary>
        private const string TESTCASE_TESTNCBIQBLASTDNA = "TestNcbiQBlast_Dna";

        /// <summary>
        /// Test case Id TestNcbiQBlast_Casing
        /// </summary>
        private const string TESTCASE_TESTNCBIQBLASTCASING = "TestNcbiQBlast_Casing";

        /// <summary>
        /// Test case Id TestNcbiQBlast_BadDatabase
        /// </summary>
        private const string TESTCASE_TESTNCBIQBLASTBADDATABASE = "TestNcbiQBlast_BadDatabase";

        /// <summary>
        /// Test case Id TestEbiWuBlast_Protein
        /// </summary>
        private const string TESTCASE_TESTEBIWUBLASTPROTEIN = "TestEbiWuBlast_Protein";

        /// <summary>
        /// Test case Id TestEBIWuBlast_Dna
        /// </summary>
        private const string TESTCASE_TESTEBIWUBLASTDNA = "TestEBIWuBlast_Dna";

        /// <summary>
        /// Test case Id TestEBIWuBlast_Casing
        /// </summary>
        private const string TESTCASE_TESTEBIWUBLASTCASING = "TestEBIWuBlast_Casing";

        /// <summary>
        /// Test case Id TestEBIWuBlast_GetServiceMetadata
        /// </summary>
        private const string TESTCASE_TESTEBIWUBLASTGETSERVICEMETADATA = "TestEBIWuBlast_GetServiceMetadata";

        /// <summary>
        /// Test case Id TestBioHPCBlast_Protein
        /// </summary>
        private const string TESTCASE_TESTBIOHPCBLASTPROTEIN = "TestBioHPCBlast_Protein";

        /// <summary>
        /// Test case Id TestBioHPCBlast_Dna
        /// </summary>
        private const string TESTCASE_TESTBIOHPCBLASTDNA = "TestBioHPCBlast_Dna";

        /// <summary>
        /// Test case Id TestBioHPCBlast_GetServiceMetadata
        /// </summary>
        private const string TESTCASE_TESTBIOHPCBLASTGETSERVICEMETADATA = "TestBioHPCBlast_GetServiceMetadata";

        /// <summary>
        /// Program parameters
        /// </summary>
        private const string BLASTPARAMETERS = "BLASTPARAMETERS";

        /// <summary>
        /// sequence string
        /// </summary>
        private const string TESTSEQUENCE = "SEQUENCE";

        /// <summary>
        /// Job identifier
        /// </summary>
        private const string JOBIDENTIFIER = "JOBIDENTIFIER";

        /// <summary>
        /// Service kind
        /// </summary>
        private const string SERVICEKIND = "SERVICEKIND";

        /// <summary>
        /// Blast handler
        /// </summary>
        private const string BLASTHANDLER = "BLASTHANDLER";

        /// <summary>
        /// Program parameters
        /// </summary>
        private const string PARAMETER_PROGRAM = "Program";

        /// <summary>
        /// Program BLASTP
        /// </summary>
        private const string PROGRAM_BLASTP = "blastp";

        /// <summary>
        /// Program BLASTN
        /// </summary>
        private const string PROGRAM_BLASTN = "blastn";

        /// <summary>
        /// Database parameters
        /// </summary>
        private const string PARAMETER_DATABASE = "Database";

        /// <summary>
        /// Database SWISSPROT
        /// </summary>
        private const string DATABASE_SWISSPROT = "swissprot";

        /// <summary>
        /// Database NR
        /// </summary>
        private const string DATABASE_NR = "nr";

        /// <summary>
        /// Database NT
        /// </summary>
        private const string DATABASE_NT = "nt";

        /// <summary>
        /// Database em_rel
        /// </summary>
        private const string DATABASE_EMREL = "em_rel";

        /// <summary>
        /// Query parameters
        /// </summary>
        private const string PARAMETER_QUERY = "Query";

        /// <summary>
        /// Expect parameters
        /// </summary>
        private const string PARAMETER_EXPECT = "Expect";

        /// <summary>
        /// CompositionBasedStatistics parameters
        /// </summary>
        private const string PARAMETER_COMPOSITIONBASEDSTATISTICS = "CompositionBasedStatistics";

        /// <summary>
        /// Email parameters
        /// </summary>
        private const string PARAMETER_EMAIL = "Email";

        /// <summary>
        /// Email Notify parameters
        /// </summary>
        private const string PARAMETER_EMAILNOTIFY = "EmailNotify";

        /// <summary>
        /// Flag to find if the server could not be contacted.
        /// </summary>
        int flag = 0;

        /// <summary>
        /// Test the NcbiQBlast class for protein (BLASTP).
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestNcbiQBlast_Protein()
        {
            IBlastServiceHandler service = null;
            try
            {
                // Initialize Test case parameters
                Dictionary<string, object> testCaseParameters = new Dictionary<string, object>();

                // Initialize Blast Parameters
                BlastParameters searchParams = new BlastParameters();
                searchParams.Add(PARAMETER_PROGRAM, PROGRAM_BLASTP);
                searchParams.Add(PARAMETER_DATABASE, DATABASE_SWISSPROT);
                searchParams.Add(PARAMETER_EXPECT, "1e-10");
                searchParams.Add(PARAMETER_COMPOSITIONBASEDSTATISTICS, "0");
                // Add to test case parameters
                testCaseParameters.Add(BLASTPARAMETERS, searchParams);

                // sample query obtained from NCBI help page (http://www.ncbi.nlm.nih.gov/BLAST/blastcgihelp.shtml), 
                // courtesy of National Library of Medicine.
                string query = @"QIKDLLVSSSTDLDTTLVLVNAIYFKGMWKTAFNAEDTREMPFHVTKQESKPVQMMCMNNSFNVATLPAE"
                    + "KMKILELPFASGDLSMLVLLPDEVSDLERIEKTINFEKLTEWTNPNTMEKRRVKVYLPQMKIEEKYNLTS"
                    + "VLMALGMTDLFIPSANLTGISSAESLKISQAVHGAFMELSEDGIEMAGSTGVIEDIKHSPESEQFRADHP"
                    + "FLFLIKHNPTNTIVYFGRYWSP";
                Sequence sequence = new Sequence(Alphabets.Protein, query);
                // Add to test case parameters
                testCaseParameters.Add(TESTSEQUENCE, sequence);

                service = new NCBIBlastHandler();
                ConfigParameters configParams = new ConfigParameters();
                configParams.UseBrowserProxy = true;
                service.Configuration = configParams;
                // Add to test case parameters
                testCaseParameters.Add(BLASTHANDLER, service);

                TestCaseParameters parameters = new TestCaseParameters(TESTCASE_TESTNCBIQBLASTPROTEIN,
                    null,
                    Callback_FetchResultsAsync,
                    testCaseParameters);
                TestCaseOutput output = TestCaseSimulator.Simulate(parameters);
                object resultsObject = output.Result;
                Assert.IsNotNull(resultsObject);

                List<BlastResult> results = resultsObject as List<BlastResult>;
                Assert.IsNotNull(results);
                Assert.AreEqual(results.Count, 1);
                Assert.AreEqual(results[0].Records.Count, 1);
                BlastSearchRecord record = results[0].Records[0];
                Assert.AreEqual(record.Statistics.Kappa, 0.041);
                Assert.AreEqual(record.Statistics.Lambda, 0.267);
                Assert.AreEqual(record.Statistics.Entropy, 0.14);

                if (null != record.Hits
                    && 0 < record.Hits.Count)
                {
                    Assert.AreEqual(record.Hits.Count, 100);

                    Hit hit = record.Hits[0];
                    Assert.AreEqual(hit.Accession, "P01013");
                    Assert.AreEqual(hit.Id, "gi|129295|sp|P01013.1|OVALX_CHICK");
                    if (null != hit.Hsps
                        && 0 < hit.Hsps.Count)
                    {
                        Assert.AreEqual(hit.Hsps.Count, 1);
                        Assert.AreEqual(hit.Hsps[0].HitSequence.Substring(0, 30), "QIKDLLVSSSTDLDTTLVLVNAIYFKGMWK");
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }

                    parameters = new TestCaseParameters(TESTCASE_TESTNCBIQBLASTPROTEIN,
                        null,
                        Callback_FetchResultsSync,
                        testCaseParameters);
                    IList<BlastResult> results2 = (IList<BlastResult>)TestCaseSimulator.Simulate(parameters).Result;
                    Assert.IsNotNull(results2);

                    if (null != results[0].Records[0].Hits
                            && 0 < results[0].Records[0].Hits.Count
                            && null != results[0].Records[0].Hits[0].Hsps
                            && 0 < results[0].Records[0].Hits[0].Hsps.Count)
                    {
                        Assert.AreEqual(results[0].Records[0].Hits[0].Hsps[0].QuerySequence,
                            results2[0].Records[0].Hits[0].Hsps[0].QuerySequence);
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }
                }
                else
                {
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "No significant hits found with the these parameters."));
                }
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }
        }

        /// <summary>
        /// Test the NcbiQBlast class for protein (BLASTP), using a custom Query parameter.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestNcbiQBlast_Protein_UserQuery()
        {
            IBlastServiceHandler service = null;
            try
            {
                // Initialize Test case parameters
                Dictionary<string, object> testCaseParameters = new Dictionary<string, object>();

                // Initialize Blast Parameters
                BlastParameters searchParams = new BlastParameters();
                searchParams.Add(PARAMETER_PROGRAM, PROGRAM_BLASTP);
                searchParams.Add(PARAMETER_DATABASE, DATABASE_SWISSPROT);
                searchParams.Add(PARAMETER_EXPECT, "1e-10");
                searchParams.Add(PARAMETER_COMPOSITIONBASEDSTATISTICS, "0");
                // sample query obtained from NCBI help page (http://www.ncbi.nlm.nih.gov/BLAST/blastcgihelp.shtml), 
                // courtesy of National Library of Medicine.
                string query = @">gi|129295|sp|P01013|OVAX_CHICK GENE X PROTEIN (OVALBUMIN-RELATED)
QIKDLLVSSSTDLDTTLVLVNAIYFKGMWKTAFNAEDTREMPFHVTKQESKPVQMMCMNNSFNVATLPAE
KMKILELPFASGDLSMLVLLPDEVSDLERIEKTINFEKLTEWTNPNTMEKRRVKVYLPQMKIEEKYNLTS
VLMALGMTDLFIPSANLTGISSAESLKISQAVHGAFMELSEDGIEMAGSTGVIEDIKHSPESEQFRADHP
FLFLIKHNPTNTIVYFGRYWSP";
                searchParams.Add(PARAMETER_QUERY, query);
                // Add to test case parameters
                testCaseParameters.Add(BLASTPARAMETERS, searchParams);
                string query1 = @"QIKDLLVSSSTDLDTTLVLVNAIYFKGMWKTAFNAEDTREMPFHVTKQESKPVQMMCMNNSFNVATLPAE"
                   + "KMKILELPFASGDLSMLVLLPDEVSDLERIEKTINFEKLTEWTNPNTMEKRRVKVYLPQMKIEEKYNLTS"
                   + "VLMALGMTDLFIPSANLTGISSAESLKISQAVHGAFMELSEDGIEMAGSTGVIEDIKHSPESEQFRADHP"
                   + "FLFLIKHNPTNTIVYFGRYWSP";
                Sequence sequence = new Sequence(Alphabets.Protein, query1);
                // Add to test case parameters
                testCaseParameters.Add(TESTSEQUENCE, sequence);

                service = new NCBIBlastHandler();
                ConfigParameters configParams = new ConfigParameters();
                configParams.UseBrowserProxy = true;
                service.Configuration = configParams;
                // Add to test case parameters
                testCaseParameters.Add(BLASTHANDLER, service);

                TestCaseParameters parameters = new TestCaseParameters(TESTCASE_TESTNCBIQBLASTPROTEINUSERQUERY,
                    null,
                    Callback_FetchResultsAsync,
                    testCaseParameters);
                TestCaseOutput output = TestCaseSimulator.Simulate(parameters);
                object resultsObject = output.Result;
                Assert.IsNotNull(resultsObject);

                List<BlastResult> results = output.Result as List<BlastResult>;
                Assert.IsNotNull(results);
                Assert.AreEqual(results.Count, 1);
                Assert.AreEqual(results[0].Records.Count, 1);
                BlastSearchRecord record = results[0].Records[0];
                Assert.AreEqual(record.Statistics.Kappa, 0.041);
                Assert.AreEqual(record.Statistics.Lambda, 0.267);
                Assert.AreEqual(record.Statistics.Entropy, 0.14);

                if (null != record.Hits
                        && 0 < record.Hits.Count)
                {
                    Assert.AreEqual(record.Hits.Count, 100);

                    Hit hit = record.Hits[0];
                    Assert.AreEqual(hit.Accession, "P01013");
                    Assert.AreEqual(hit.Id, "gi|129295|sp|P01013.1|OVALX_CHICK");
                    if (null != hit.Hsps
                            && 0 < hit.Hsps.Count)
                    {
                        Assert.AreEqual(hit.Hsps.Count, 1);
                        Assert.AreEqual(hit.Hsps[0].HitSequence.Substring(0, 30), "QIKDLLVSSSTDLDTTLVLVNAIYFKGMWK");
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }

                    parameters = new TestCaseParameters(TESTCASE_TESTNCBIQBLASTPROTEINUSERQUERY,
                        null,
                        Callback_FetchResultsSync,
                        testCaseParameters);
                    IList<BlastResult> results2 = (IList<BlastResult>)TestCaseSimulator.Simulate(parameters).Result;
                    Assert.IsNotNull(results2);

                    if (null != results[0].Records[0].Hits
                            && 0 < results[0].Records[0].Hits.Count
                            && null != results[0].Records[0].Hits[0].Hsps
                            && 0 < results[0].Records[0].Hits[0].Hsps.Count)
                    {
                        Assert.AreEqual(results[0].Records[0].Hits[0].Hsps[0].QuerySequence,
                            results2[0].Records[0].Hits[0].Hsps[0].QuerySequence);
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }
                }
                else
                {
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "No significant hits found with the these parameters."));
                }
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }
        }

        /// <summary>
        /// Test the NcbiQBlast class for dna (BLASTN).
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestNcbiQBlast_Dna()
        {
            IBlastServiceHandler service = null;
            try
            {
                var configParams = new ConfigParameters();
                configParams.UseBrowserProxy = true;

                service = new NCBIBlastHandler(configParams);
                // Initialize Test case parameters
                Dictionary<string, object> testCaseParameters = new Dictionary<string, object>();

                // Initialize Blast Parameters
                BlastParameters searchParams = new BlastParameters();
                searchParams.Add(PARAMETER_PROGRAM, PROGRAM_BLASTN);
                searchParams.Add(PARAMETER_DATABASE, DATABASE_NR);
                searchParams.Add(PARAMETER_EXPECT, "1e-10");
                searchParams.Add(PARAMETER_COMPOSITIONBASEDSTATISTICS, "0");
                // Add to test case parameters
                testCaseParameters.Add(BLASTPARAMETERS, searchParams);

                string query = @"GACGCCGCCGCCACCACCGCCACCGCCGCAGCAGAAGCAGCGCACCGCAGGAGGGAAG" +
                    "ATGCCGGCGGGGCACGGGCTGCGGGCGCGGACGGCGACCTCTTCGCGCGGCCGTTCCGCAAGAAGGGTTA" +
                    "CATCCCGCTCACCACCTACCTGAGGACGTACAAGATCGGCGATTACGTNGACGTCAAGGTGAACGGTG";
                Sequence sequence = new Sequence(Alphabets.Protein, query);
                // Add to test case parameters
                testCaseParameters.Add(TESTSEQUENCE, sequence);

                // Add to test case parameters
                testCaseParameters.Add(BLASTHANDLER, service);

                TestCaseParameters parameters = new TestCaseParameters(TESTCASE_TESTNCBIQBLASTDNA,
                    null,
                    Callback_FetchResultsAsync,
                    testCaseParameters);
                TestCaseOutput output = TestCaseSimulator.Simulate(parameters);
                object resultsObject = output.Result;
                Assert.IsNotNull(resultsObject);

                List<BlastResult> results = resultsObject as List<BlastResult>;
                Assert.IsNotNull(results);

                Assert.AreEqual(results.Count, 1);
                Assert.AreEqual(results[0].Records.Count, 1);
                BlastSearchRecord record = results[0].Records[0];
                Assert.AreEqual(record.Statistics.Kappa, 0.41);
                Assert.AreEqual(record.Statistics.Lambda, 0.625);
                Assert.AreEqual(record.Statistics.Entropy, 0.78);

                if (null != record.Hits
                        && 0 < record.Hits.Count)
                {
                    Assert.AreEqual(record.Hits.Count, 100);

                    Hit hit = record.Hits[0];
                    Assert.AreEqual(hit.Accession, "NM_001186331");
                    Assert.AreEqual(hit.Id, "gi|297721792|ref|NM_001186331.1|");

                    if (null != hit.Hsps
                            && 0 < hit.Hsps.Count)
                    {
                        Assert.AreEqual(hit.Hsps.Count, 1);
                        Assert.AreEqual(hit.Hsps[0].HitSequence.Substring(0, 30), "GACGCCGCCGCCACCACCGCCACCGCCGCA");
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }

                    parameters = new TestCaseParameters(TESTCASE_TESTNCBIQBLASTDNA,
                        null,
                        Callback_FetchResultsSync,
                        testCaseParameters);
                    IList<BlastResult> results2 = (IList<BlastResult>)TestCaseSimulator.Simulate(parameters).Result;
                    Assert.IsNotNull(results2);

                    if (null != results[0].Records[0].Hits
                            && 0 < results[0].Records[0].Hits.Count
                            && null != results[0].Records[0].Hits[0].Hsps
                            && 0 < results[0].Records[0].Hits[0].Hsps.Count)
                    {
                        Assert.AreEqual(results[0].Records[0].Hits[0].Hsps[0].QuerySequence,
                            results2[0].Records[0].Hits[0].Hsps[0].QuerySequence);
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }
                }
                else
                {
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "No significant hits found with the these parameters."));
                }
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }
        }

        /// <summary>
        /// Test the NcbiQBlast class with mixed case Program argument
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestNcbiQBlast_Casing()
        {
            NCBIBlastHandler service = null;
            try
            {

                // Initialize Test case parameters
                Dictionary<string, object> testCaseParameters = new Dictionary<string, object>();

                // Initialize Blast Parameters
                BlastParameters searchParams = new BlastParameters();
                searchParams.Add(PARAMETER_PROGRAM, "BLaStN");
                searchParams.Add(PARAMETER_DATABASE, DATABASE_NR);
                searchParams.Add(PARAMETER_EXPECT, "1e-10");
                searchParams.Add(PARAMETER_COMPOSITIONBASEDSTATISTICS, "0");
                // Add to test case parameters
                testCaseParameters.Add(BLASTPARAMETERS, searchParams);

                string query = @"GACGCCGCCGCCACCACCGCCACCGCCGCAGCAGAAGCAGCGCACCGCAGGAGGGAAG" +
                    "ATGCCGGCGGGGCACGGGCTGCGGGCGCGGACGGCGACCTCTTCGCGCGGCCGTTCCGCAAGAAGGGTTA" +
                    "CATCCCGCTCACCACCTACCTGAGGACGTACAAGATCGGCGATTACGTNGACGTCAAGGTGAACGGTG";
                Sequence sequence = new Sequence(Alphabets.Protein, query);
                // Add to test case parameters
                testCaseParameters.Add(TESTSEQUENCE, sequence);

                service = new NCBIBlastHandler();

                ConfigParameters configParams = new ConfigParameters();
                configParams.UseBrowserProxy = true;
                service.Configuration = configParams;
                // Add to test case parameters
                testCaseParameters.Add(BLASTHANDLER, service);


                TestCaseParameters parameters = new TestCaseParameters(TESTCASE_TESTNCBIQBLASTCASING,
                    null,
                    Callback_FetchResultsAsync,
                    testCaseParameters);
                TestCaseOutput output = TestCaseSimulator.Simulate(parameters);
                object resultsObject = output.Result;
                Assert.IsNotNull(resultsObject);

                List<BlastResult> results = resultsObject as List<BlastResult>;
                Assert.IsNotNull(results);

                Assert.AreEqual(results.Count, 1);
                Assert.AreEqual(results[0].Records.Count, 1);
                BlastSearchRecord record = results[0].Records[0];
                Assert.AreEqual(record.Statistics.Kappa, 0.41);
                Assert.AreEqual(record.Statistics.Lambda, 0.625);
                Assert.AreEqual(record.Statistics.Entropy, 0.78);

                if (null != record.Hits
                        && 0 < record.Hits.Count)
                {
                    Assert.AreEqual(record.Hits.Count, 100);

                    Hit hit = record.Hits[0];
                    Assert.AreEqual(hit.Accession, "NM_001186331");
                    Assert.AreEqual(hit.Id, "gi|297721792|ref|NM_001186331.1|");
                    if (null != hit.Hsps
                            && 0 < hit.Hsps.Count)
                    {
                        Assert.AreEqual(hit.Hsps.Count, 1);
                        Assert.AreEqual(hit.Hsps[0].HitSequence.Substring(0, 30), "GACGCCGCCGCCACCACCGCCACCGCCGCA");
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }

                    parameters = new TestCaseParameters(TESTCASE_TESTNCBIQBLASTCASING,
                        null,
                        Callback_FetchResultsSync,
                        testCaseParameters);
                    IList<BlastResult> results2 = (IList<BlastResult>)TestCaseSimulator.Simulate(parameters).Result;
                    Assert.IsNotNull(results2);

                    if (null != results[0].Records[0].Hits
                            && 0 < results[0].Records[0].Hits.Count
                            && null != results[0].Records[0].Hits[0].Hsps
                            && 0 < results[0].Records[0].Hits[0].Hsps.Count)
                    {
                        Assert.AreEqual(results[0].Records[0].Hits[0].Hsps[0].QuerySequence,
                            results2[0].Records[0].Hits[0].Hsps[0].QuerySequence);
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }
                }
                else
                {
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "No significant hits found with the these parameters."));
                }
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }
        }

        /// <summary>
        /// Test the NcbiQBlast class for dna (BLASTN), specifying nonexistent database.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        [Ignore]
        public void TestNcbiQBlast_BadDatabase()
        {
            /* This test no longer works - the service simply returns "Ready" with no status, no error is returned now */

            IBlastServiceHandler service = null;
            try
            {
                string badDbName = "ThisDatabaseDoesNotExist";
                // Initialize Test case parameters
                var testCaseParameters = new Dictionary<string, object>();

                // Initialize Blast Parameters
                BlastParameters searchParams = new BlastParameters();
                searchParams.Add(PARAMETER_PROGRAM, PROGRAM_BLASTN);
                searchParams.Add(PARAMETER_DATABASE, badDbName);
                searchParams.Add(PARAMETER_EXPECT, "1e-10");
                searchParams.Add(PARAMETER_COMPOSITIONBASEDSTATISTICS, "0");
                // Add to test case parameters
                testCaseParameters.Add(BLASTPARAMETERS, searchParams);

                const string query = @"GACGCCGCCGCCACCACCGCCACCGCCGCAGCAGAAGCAGCGCACCGCAGGAGGGAAG" +
                                     "ATGCCGGCGGGGCACGGGCTGCGGGCGCGGACGGCGACCTCTTCGCGCGGCCGTTCCGCAAGAAGGGTTA" +
                                     "CATCCCGCTCACCACCTACCTGAGGACGTACAAGATCGGCGATTACGTNGACGTCAAGGTGAACGGTG";
                // Add to test case parameters
                testCaseParameters.Add(TESTSEQUENCE, new Sequence(Alphabets.Protein, query));

                ConfigParameters configParams = new ConfigParameters { UseBrowserProxy = true };
                service = new NCBIBlastHandler {Configuration = configParams};
                
                // Add to test case parameters
                testCaseParameters.Add(BLASTHANDLER, service);
                TestCaseParameters parameters = new TestCaseParameters(TESTCASE_TESTNCBIQBLASTBADDATABASE, null, Callback_FetchResultsAsync, testCaseParameters);

                ServiceRequestInformation info = ServiceRequestInfoForDatabase(parameters.CallbackParameters);

                bool ok = false;
                if (info.Status != ServiceRequestStatus.Waiting && info.Status != ServiceRequestStatus.Ready)
                {
                    if (info.StatusInformation.Contains(badDbName) &&
                        info.StatusInformation.Contains("No alias or index file found for nucleotide database"))
                    {
                        ok = true;
                    }
                }
                if (!ok)
                {
                    Assert.Fail("Failed to find server error message for bad request. Info: " + info.StatusInformation);
                }
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }
        }

        /// <summary>
        /// Test the EbiWuBlast class for protein (BLASTP).
        /// Commented in automation also. As this service is down many times so these test case fails.
        /// </summary>
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestEbiWuBlast_Protein()
        {
            IBlastServiceHandler service = null;
            try
            {
                // Initialize Test case parameters
                Dictionary<string, object> testCaseParameters = new Dictionary<string, object>();

                // Initialize Blast Parameters
                BlastParameters searchParams = new BlastParameters();
                searchParams.Add(PARAMETER_PROGRAM, PROGRAM_BLASTP);
                searchParams.Add(PARAMETER_DATABASE, DATABASE_SWISSPROT);
                searchParams.Add(PARAMETER_EXPECT, "1e-10");
                searchParams.Add(PARAMETER_EMAIL, "msrerbio@microsoft.com");
                //Added as this param was missing..
                searchParams.Add("SequenceType", "protein");  
                // Add to test case parameters
                testCaseParameters.Add(BLASTPARAMETERS, searchParams);

                // sample query obtained from NCBI help page (http://www.ncbi.nlm.nih.gov/BLAST/blastcgihelp.shtml), 
                // courtesy of National Library of Medicine.
                string query = @"QIKDLLVSSSTDLDTTLVLVNAIYFKGMWKTAFNAEDTREMPFHVTKQESKPVQMMCMNNSFNVATLPAE"
                    + "KMKILELPFASGDLSMLVLLPDEVSDLERIEKTINFEKLTEWTNPNTMEKRRVKVYLPQMKIEEKYNLTS"
                    + "VLMALGMTDLFIPSANLTGISSAESLKISQAVHGAFMELSEDGIEMAGSTGVIEDIKHSPESEQFRADHP"
                    + "FLFLIKHNPTNTIVYFGRYWSP";
                Sequence sequence = new Sequence(Alphabets.Protein, query);
                // Add to test case parameters
                testCaseParameters.Add(TESTSEQUENCE, sequence);

                service = new EbiWuBlastHandler();
                ConfigParameters configParams = new ConfigParameters();
                configParams.UseBrowserProxy = true;
                service.Configuration = configParams;
                // Add to test case parameters
                testCaseParameters.Add(BLASTHANDLER, service);

                TestCaseParameters parameters = new TestCaseParameters(TESTCASE_TESTEBIWUBLASTPROTEIN,
                    null,
                    Callback_FetchResultsAsync,
                    testCaseParameters);
                TestCaseOutput output = TestCaseSimulator.Simulate(parameters);
                object resultsObject = output.Result;
                List<BlastResult> results = resultsObject as List<BlastResult>;
                Assert.IsNotNull(results);

                Assert.AreEqual(results.Count, 1);
                Assert.AreEqual(results[0].Records.Count, 1);
                BlastSearchRecord record = results[0].Records[0];
                Assert.AreEqual(record.Statistics.Kappa, 0.03);
                Assert.AreEqual(record.Statistics.Lambda, 0.244);
                Assert.AreEqual(record.Statistics.Entropy, 0.18);

                if (null != record.Hits
                        && 0 < record.Hits.Count)
                {
                    Assert.AreEqual(record.Hits.Count, 50);

                    Hit hit = record.Hits[0];
                    Assert.AreEqual(hit.Accession, "SW:OVALX_CHICK");
                    Assert.AreEqual(hit.Id, "SW:OVALX_CHICK");

                    if (null != hit.Hsps
                            && 0 < hit.Hsps.Count)
                    {
                        Assert.AreEqual(hit.Hsps.Count, 1);
                        Assert.AreEqual(hit.Hsps[0].HitSequence.Substring(0, 30), "QIKDLLVSSSTDLDTTLVLVNAIYFKGMWK");
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }

                    parameters = new TestCaseParameters(TESTCASE_TESTEBIWUBLASTPROTEIN,
                        null,
                        Callback_FetchResultsSync,
                        testCaseParameters);
                    IList<BlastResult> results2 = (IList<BlastResult>)TestCaseSimulator.Simulate(parameters).Result;
                    Assert.IsNotNull(results2);

                    if (null != results[0].Records[0].Hits
                            && 0 < results[0].Records[0].Hits.Count
                            && null != results[0].Records[0].Hits[0].Hsps
                            && 0 < results[0].Records[0].Hits[0].Hsps.Count)
                    {
                        Assert.AreEqual(results[0].Records[0].Hits[0].Hsps[0].QuerySequence,
                            results2[0].Records[0].Hits[0].Hsps[0].QuerySequence);
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }
                }
                else
                {
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "No significant hits found with the these parameters."));
                }
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }
        }

        /// <summary>
        /// Test the EbiWuBlast class for dna (BLASTN).
        /// Commented in automation also. As this service is down many times so these test case fails.
        /// </summary>
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestEBIWuBlast_Dna()
        {
            // test parameters
            IBlastServiceHandler service = null;
            try
            {
                // Initialize Test case parameters
                Dictionary<string, object> testCaseParameters = new Dictionary<string, object>();

                // Initialize Blast Parameters
                BlastParameters searchParams = new BlastParameters();
                searchParams.Add(PARAMETER_PROGRAM, PROGRAM_BLASTN);
                searchParams.Add(PARAMETER_DATABASE, DATABASE_EMREL);
                searchParams.Add(PARAMETER_EXPECT, "1e-10");
                searchParams.Add(PARAMETER_EMAIL, "msrerbio@microsoft.com");
                searchParams.Add("SequenceType", "dna");
                // Add to test case parameters
                testCaseParameters.Add(BLASTPARAMETERS, searchParams);

                string query = @"GACGCCGCCGCCACCACCGCCACCGCCGCAGCAGAAGCAGCGCACCGCAGGAGGGAAG" +
                    "ATGCCGGCGGGGCACGGGCTGCGGGCGCGGACGGCGACCTCTTCGCGCGGCCGTTCCGCAAGAAGGGTTA" +
                    "CATCCCGCTCACCACCTACCTGAGGACGTACAAGATCGGCGATTACGTNGACGTCAAGGTGAACGGTG";
                Sequence sequence = new Sequence(Alphabets.Protein, query);
                // Add to test case parameters
                testCaseParameters.Add(TESTSEQUENCE, sequence);

                service = new EbiWuBlastHandler();

                ConfigParameters configParams = new ConfigParameters();
                configParams.UseBrowserProxy = true;
                service.Configuration = configParams;
                // Add to test case parameters
                testCaseParameters.Add(BLASTHANDLER, service);

                TestCaseParameters parameters = new TestCaseParameters(TESTCASE_TESTEBIWUBLASTDNA,
                    null,
                    Callback_FetchResultsAsync,
                    testCaseParameters);
                TestCaseOutput output = TestCaseSimulator.Simulate(parameters);
                object resultsObject = output.Result;
                Assert.IsNotNull(resultsObject);

                List<BlastResult> results = resultsObject as List<BlastResult>;
                Assert.IsNotNull(results);

                Assert.AreEqual(1, results.Count);
                Assert.AreEqual(1, results[0].Records.Count);
                BlastSearchRecord record = results[0].Records[0];
                Assert.AreEqual(0.0151, record.Statistics.Kappa);
                Assert.AreEqual(0.104, record.Statistics.Lambda);
                Assert.AreEqual(0.06, record.Statistics.Entropy);

                if (null != record.Hits
                        && 0 < record.Hits.Count)
                {
                    Assert.AreEqual(50, record.Hits.Count);

                    Hit hit = record.Hits[0];
                    Assert.AreEqual("EM_EST:D15320;", hit.Accession);
                    Assert.AreEqual("EM_EST:D15320;", hit.Id);

                    if (null != hit.Hsps
                            && 0 < hit.Hsps.Count)
                    {
                        Assert.AreEqual(1, hit.Hsps.Count);
                        Assert.AreEqual("GACGCCGCCGCCACCACCGCCACCGCCGCA", hit.Hsps[0].HitSequence.Substring(0, 30));
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }

                    parameters = new TestCaseParameters(TESTCASE_TESTEBIWUBLASTDNA,
                        null,
                        Callback_FetchResultsSync,
                        testCaseParameters);
                    IList<BlastResult> results2 = (IList<BlastResult>)TestCaseSimulator.Simulate(parameters).Result;
                    Assert.IsNotNull(results2);

                    if (null != results[0].Records[0].Hits
                            && 0 < results[0].Records[0].Hits.Count
                            && null != results[0].Records[0].Hits[0].Hsps
                            && 0 < results[0].Records[0].Hits[0].Hsps.Count)
                    {
                        Assert.AreEqual(results[0].Records[0].Hits[0].Hsps[0].QuerySequence,
                            results2[0].Records[0].Hits[0].Hsps[0].QuerySequence);
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }
                }
                else
                {
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "No significant hits found with the these parameters."));
                }
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }
        }

        /// <summary>
        /// Test the EbiWuBlast class with mixed case Program argument.
        /// Commented in automation also. As this service is down many times so these test case fails.
        /// </summary>
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestEBIWuBlast_Casing()
        {
            IBlastServiceHandler service = null;
            try
            {
                // Initialize Test case parameters
                Dictionary<string, object> testCaseParameters = new Dictionary<string, object>();

                // Initialize Blast Parameters
                BlastParameters searchParams = new BlastParameters();
                // The WU-BLAST service doesn't seem to care about case of the Program
                // parameter, but make sure it works with forcing upper case.
                searchParams.Add(PARAMETER_PROGRAM, "BLaSTn");
                searchParams.Add(PARAMETER_DATABASE, DATABASE_EMREL);
                searchParams.Add(PARAMETER_EXPECT, "1e-10");
                searchParams.Add(PARAMETER_EMAIL, "msrerbio@microsoft.com");
                searchParams.Add("SequenceType", "dna");
                // Add to test case parameters
                testCaseParameters.Add(BLASTPARAMETERS, searchParams);

                string query = @"GACGCCGCCGCCACCACCGCCACCGCCGCAGCAGAAGCAGCGCACCGCAGGAGGGAAG" +
                    "ATGCCGGCGGGGCACGGGCTGCGGGCGCGGACGGCGACCTCTTCGCGCGGCCGTTCCGCAAGAAGGGTTA" +
                    "CATCCCGCTCACCACCTACCTGAGGACGTACAAGATCGGCGATTACGTNGACGTCAAGGTGAACGGTG";
                Sequence sequence = new Sequence(Alphabets.Protein, query);
                // Add to test case parameters
                testCaseParameters.Add(TESTSEQUENCE, sequence);

                service = new EbiWuBlastHandler();

                ConfigParameters configParams = new ConfigParameters();
                configParams.UseBrowserProxy = true;
                service.Configuration = configParams;
                // Add to test case parameters
                testCaseParameters.Add(BLASTHANDLER, service);

                TestCaseParameters parameters = new TestCaseParameters(TESTCASE_TESTEBIWUBLASTCASING,
                    null,
                    Callback_FetchResultsAsync,
                    testCaseParameters);
                TestCaseOutput output = TestCaseSimulator.Simulate(parameters);
                object resultsObject = output.Result;
                Assert.IsNotNull(resultsObject);

                List<BlastResult> results = resultsObject as List<BlastResult>;
                Assert.IsNotNull(results);

                Assert.AreEqual(1, results.Count);
                Assert.AreEqual(1, results[0].Records.Count);
                BlastSearchRecord record = results[0].Records[0];
                Assert.AreEqual(0.0151, record.Statistics.Kappa);
                Assert.AreEqual(0.104, record.Statistics.Lambda);
                Assert.AreEqual(0.06, record.Statistics.Entropy);

                if (null != record.Hits
                        && 0 < record.Hits.Count)
                {
                    Assert.AreEqual(50, record.Hits.Count);

                    Hit hit = record.Hits[0];
                    Assert.AreEqual("EM_EST:D15320;", hit.Accession);
                    Assert.AreEqual("EM_EST:D15320;", hit.Id);

                    if (null != record.Hits[0].Hsps
                            && 0 < record.Hits[0].Hsps.Count)
                    {
                        Assert.AreEqual(1, hit.Hsps.Count);
                        Assert.AreEqual("GACGCCGCCGCCACCACCGCCACCGCCGCA", hit.Hsps[0].HitSequence.Substring(0, 30));
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }

                    parameters = new TestCaseParameters(TESTCASE_TESTEBIWUBLASTCASING,
                        null,
                        Callback_FetchResultsSync,
                        testCaseParameters);
                    IList<BlastResult> results2 = (IList<BlastResult>)TestCaseSimulator.Simulate(parameters).Result;
                    Assert.IsNotNull(results2);

                    if (null != results[0].Records[0].Hits
                            && 0 < results[0].Records[0].Hits.Count
                            && null != results[0].Records[0].Hits[0].Hsps
                            && 0 < results[0].Records[0].Hits[0].Hsps.Count)
                    {
                        Assert.AreEqual(results[0].Records[0].Hits[0].Hsps[0].QuerySequence,
                            results2[0].Records[0].Hits[0].Hsps[0].QuerySequence);
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }
                }
                else
                {
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "No significant hits found with the these parameters."));
                }
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }

        }

        /// <summary>
        /// Test the EbiWuBlast method for fetching database names and other server metadata.
        /// Commented in automation also. As this service is down many times so these test case fails.
        /// </summary>
        //[TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestEBIWuBlast_GetServiceMetadata()
        {
            string[] kinds = { 
                                 EbiWuBlastHandler.MetadataDatabases, 
                                 EbiWuBlastHandler.MetadataFilter, 
                                 EbiWuBlastHandler.MetadataMatrices, 
                                 EbiWuBlastHandler.MetadataPrograms, 
                                 EbiWuBlastHandler.MetadataSensitivity, 
                                 EbiWuBlastHandler.MetadataSort, 
                                 EbiWuBlastHandler.MetadataStatistics, 
                                 EbiWuBlastHandler.MetadataXmlFormats 
                             };

            foreach (string kind in kinds)
            {
                string testCaseId = TESTCASE_TESTEBIWUBLASTGETSERVICEMETADATA + kind;
                // Initialize Test case parameters
                Dictionary<string, object> testCaseParameters = new Dictionary<string, object>();
                testCaseParameters.Add(SERVICEKIND, kind);
                TestCaseParameters parameters = new TestCaseParameters(testCaseId,
                    null,
                    Callback_EBIWuBlast_GetServiceMetadata,
                    testCaseParameters);

                TestCaseOutput output = TestCaseSimulator.Simulate(parameters);
                IList<string> resultsObject = output.Result as IList<string>;

                ApplicationLog.WriteLine("{0}:", kind);
                foreach (string s in resultsObject)
                {
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "\t{0}", s));
                }
            }
        }

        /// <summary>
        /// Test the BioHPC Blast class for protein (BLASTP).
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestBioHPCBlast_Protein()
        {
            IBlastServiceHandler service = null;
            try
            {
                // Initialize Test case parameters
                Dictionary<string, object> testCaseParameters = new Dictionary<string, object>();

                // Initialize Blast Parameters
                BlastParameters searchParams = new BlastParameters();
                searchParams.Add(PARAMETER_PROGRAM, PROGRAM_BLASTP);
                searchParams.Add(PARAMETER_DATABASE, DATABASE_SWISSPROT);
                searchParams.Add(PARAMETER_EXPECT, "1e-10");
                // E-mail notification option is optional - default is "no"
                searchParams.Add(PARAMETER_EMAILNOTIFY, "no");
                // Add to test case parameters
                testCaseParameters.Add(BLASTPARAMETERS, searchParams);

                // sample query obtained from NCBI help page (http://www.ncbi.nlm.nih.gov/BLAST/blastcgihelp.shtml), 
                // courtesy of National Library of Medicine.
                string query = @"QIKDLLVSSSTDLDTTLVLVNAIYFKGMWKTAFNAEDTREMPFHVTKQESKPVQMMCMNNSFNVATLPAE"
                    + "KMKILELPFASGDLSMLVLLPDEVSDLERIEKTINFEKLTEWTNPNTMEKRRVKVYLPQMKIEEKYNLTS"
                    + "VLMALGMTDLFIPSANLTGISSAESLKISQAVHGAFMELSEDGIEMAGSTGVIEDIKHSPESEQFRADHP"
                    + "FLFLIKHNPTNTIVYFGRYWSP";
                Sequence sequence = new Sequence(Alphabets.Protein, query);
                // Add to test case parameters
                testCaseParameters.Add(TESTSEQUENCE, sequence);

                service = new BioHPCBlastHandler();
                ConfigParameters configParams = new ConfigParameters();
                configParams.EmailAddress = "msrerbio@microsoft.com";
                configParams.Password = "";
                service.Configuration = configParams;
                // Add to test case parameters
                testCaseParameters.Add(BLASTHANDLER, service);

                TestCaseParameters parameters = new TestCaseParameters(TESTCASE_TESTBIOHPCBLASTPROTEIN,
                    null,
                    Callback_FetchResultsAsync,
                    testCaseParameters);
                TestCaseOutput output = TestCaseSimulator.Simulate(parameters);

                object resultsObject = output.Result;
                Assert.IsNotNull(resultsObject);

                List<BlastResult> results = resultsObject as List<BlastResult>;
                Assert.IsNotNull(results);

                Assert.AreEqual(results.Count, 1);
                Assert.AreEqual(results[0].Records.Count, 1);
                BlastSearchRecord record = results[0].Records[0];
                Assert.AreEqual(record.Statistics.Kappa, 0.041);
                Assert.AreEqual(record.Statistics.Lambda, 0.267);
                Assert.AreEqual(record.Statistics.Entropy, 0.14);

                if (null != record.Hits
                        && 0 < record.Hits.Count)
                {
                    Assert.AreEqual(record.Hits.Count, 10);

                    Hit hit = record.Hits[0];
                    Assert.AreEqual(hit.Accession, "P01013");
                    Assert.AreEqual(hit.Id, "gi|129295|sp|P01013.1|OVALX_CHICK");

                    if (null != hit.Hsps
                            && 0 < hit.Hsps.Count)
                    {
                        Assert.AreEqual(hit.Hsps.Count, 1);
                        Assert.AreEqual(hit.Hsps[0].HitSequence.Substring(0, 30), "QIKDLLVSSSTDLDTTLVLVNAIYFKGMWK");
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }

                    parameters = new TestCaseParameters(TESTCASE_TESTBIOHPCBLASTPROTEIN,
                        null,
                        Callback_FetchResultsSync,
                        testCaseParameters);
                    IList<BlastResult> results2 = (IList<BlastResult>)TestCaseSimulator.Simulate(parameters).Result;
                    Assert.IsNotNull(results2);

                    if (null != results[0].Records[0].Hits
                            && 0 < results[0].Records[0].Hits.Count
                            && null != results[0].Records[0].Hits[0].Hsps
                            && 0 < results[0].Records[0].Hits[0].Hsps.Count)
                    {
                        Assert.AreEqual(results[0].Records[0].Hits[0].Hsps[0].QuerySequence,
                            results2[0].Records[0].Hits[0].Hsps[0].QuerySequence);
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }
                }
                else
                {
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "No significant hits found with the these parameters."));
                }
            
                
            }
            catch (Exception ex)
            {
                if (ex.Message == "BioHPC server could not be contacted to create a job.")
                {
                    ApplicationLog.WriteLine(ex.Message);
                    flag = 1;
                }
            }
            finally
            {
                if(flag == 0)
                {
                    if (service != null)
                        ((IDisposable)service).Dispose();
                }
                flag = 0;
            }
        }

        /// <summary>
        /// Test the BioHPCBlast class for dna (BLASTN).
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestBioHPCBlast_Dna()
        {
            IBlastServiceHandler service = null;
            try
            {
                // Initialize Test case parameters
                Dictionary<string, object> testCaseParameters = new Dictionary<string, object>();

                // Initialize Blast Parameters
                BlastParameters searchParams = new BlastParameters();
                searchParams.Add(PARAMETER_PROGRAM, PROGRAM_BLASTN);
                searchParams.Add(PARAMETER_DATABASE, DATABASE_NT);
                searchParams.Add(PARAMETER_EXPECT, "1e-10");
                // E-mail notification option is optional - default is "no"
                searchParams.Add(PARAMETER_EMAILNOTIFY, "no");
                // Add to test case parameters
                testCaseParameters.Add(BLASTPARAMETERS, searchParams);

                // sample query obtained from NCBI help page (http://www.ncbi.nlm.nih.gov/BLAST/blastcgihelp.shtml), 
                // courtesy of National Library of Medicine.
                string query = @"GACGCCGCCGCCACCACCGCCACCGCCGCAGCAGAAGCAGCGCACCGCAGGAGGGAAG" +
                    "ATGCCGGCGGGGCACGGGCTGCGGGCGCGGACGGCGACCTCTTCGCGCGGCCGTTCCGCAAGAAGGGTTA" +
                    "CATCCCGCTCACCACCTACCTGAGGACGTACAAGATCGGCGATTACGTNGACGTCAAGGTGAACGGTG";
                Sequence sequence = new Sequence(Alphabets.Protein, query);
                // Add to test case parameters
                testCaseParameters.Add(TESTSEQUENCE, sequence);

                service = new BioHPCBlastHandler();

                ConfigParameters configParams = new ConfigParameters();
                configParams.EmailAddress = "msrerbio@microsoft.com";
                configParams.Password = "";
                service.Configuration = configParams;
                // Add to test case parameters
                testCaseParameters.Add(BLASTHANDLER, service);

                TestCaseParameters parameters = new TestCaseParameters(TESTCASE_TESTBIOHPCBLASTDNA,
                    null,
                    Callback_FetchResultsAsync,
                    testCaseParameters);
                TestCaseOutput output = TestCaseSimulator.Simulate(parameters);
                object resultsObject = output.Result;
                Assert.IsNotNull(resultsObject);

                List<BlastResult> results = resultsObject as List<BlastResult>;
                Assert.IsNotNull(results);

                Assert.AreEqual(1, results.Count);
                Assert.AreEqual(1, results[0].Records.Count);
                BlastSearchRecord record = results[0].Records[0];
                Assert.AreEqual(0.710603, record.Statistics.Kappa);
                Assert.AreEqual(1.37406, record.Statistics.Lambda);
                Assert.AreEqual(1.30725, record.Statistics.Entropy);

                if (null != record.Hits
                        && 0 < record.Hits.Count)
                {
                    Assert.AreEqual(10, record.Hits.Count);

                    Hit hit = record.Hits[0];
                    Assert.AreEqual("NM_001055459", hit.Accession);
                    Assert.AreEqual("gi|115450646|ref|NM_001055459.1|", hit.Id);

                    if (null != hit.Hsps
                            && 0 < hit.Hsps.Count)
                    {
                        Assert.AreEqual(1, hit.Hsps.Count);
                        Assert.AreEqual("GACGCCGCCGCCACCACCGCCACCGCCGCA", hit.Hsps[0].HitSequence.Substring(0, 30));
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }

                    parameters = new TestCaseParameters(TESTCASE_TESTBIOHPCBLASTDNA,
                        null,
                        Callback_FetchResultsSync,
                        testCaseParameters);
                    IList<BlastResult> results2 = (IList<BlastResult>)TestCaseSimulator.Simulate(parameters).Result;
                    Assert.IsNotNull(results2);

                    if (null != results[0].Records[0].Hits
                            && 0 < results[0].Records[0].Hits.Count
                            && null != results[0].Records[0].Hits[0].Hsps
                            && 0 < results[0].Records[0].Hits[0].Hsps.Count)
                    {
                        Assert.AreEqual(results[0].Records[0].Hits[0].Hsps[0].QuerySequence,
                            results2[0].Records[0].Hits[0].Hsps[0].QuerySequence);
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }
                }
                else
                {
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "No significant hits found with the these parameters."));
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "BioHPC server could not be contacted to create a job.")
                {
                    ApplicationLog.WriteLine(ex.Message);
                    flag = 1;
                }
            }
            finally
            {
                if (flag == 0)
                {
                    if (service != null)
                        ((IDisposable)service).Dispose();
                }
                flag = 0;
            }
        }

        /// <summary>
        /// Test the BioHPCBlast method for fetching database names and other server metadata.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestBioHPCBlast_GetServiceMetadata()
        {
            string[] kinds = { 
                                 BioHPCBlastHandler.MetadataDatabasesDna,
                                 BioHPCBlastHandler.MetadataDatabasesProt,
                                 BioHPCBlastHandler.MetadataFilter, 
                                 BioHPCBlastHandler.MetadataMatrices, 
                                 BioHPCBlastHandler.MetadataPrograms,  
                                 BioHPCBlastHandler.MetadataFormats 
                             };

            foreach (string kind in kinds)
            {
                string testCaseId = TESTCASE_TESTBIOHPCBLASTGETSERVICEMETADATA + kind;
                // Initialize Test case parameters
                Dictionary<string, object> testCaseParameters = new Dictionary<string, object>();
                testCaseParameters.Add(SERVICEKIND, kind);
                TestCaseParameters parameters = new TestCaseParameters(testCaseId,
                    null,
                    Callback_BioHPCBlast_GetServiceMetadata,
                    testCaseParameters);

                TestCaseOutput output = TestCaseSimulator.Simulate(parameters);
                IList<string> resultsObject = output.Result as IList<string>;

                ApplicationLog.WriteLine("{0}:", kind);
                foreach (string s in resultsObject)
                {
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "\t{0}", s));
                }
            }
        }

        /// <summary>
        /// Test the BLAST XML parser.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestBlastXmlParser()
        {
            string filename = @"TestUtils\BlastXML\NewYork.xml";
            Assert.IsTrue(File.Exists(filename));

            BlastXmlParser parser = new BlastXmlParser();
            IList<BlastResult> results = parser.Parse(filename);
            Assert.AreEqual(1, results.Count);

            BlastXmlMetadata meta = results[0].Metadata;
            Assert.AreEqual(meta.Database, "nt");
            Assert.AreEqual(meta.QueryId, "55025");
            Assert.AreEqual(meta.ParameterExpect, 10.0);
            Assert.AreEqual(meta.ParameterFilter, "L;m;");
            Assert.AreEqual(meta.ParameterGapOpen, 5);
            Assert.AreEqual(meta.ParameterGapExtend, 2);

            Assert.AreEqual(1, results[0].Records.Count);
            BlastSearchRecord record = results[0].Records[0];
            Assert.AreEqual(record.Statistics.DatabaseLength, 1658539059);
            Assert.AreEqual(record.Statistics.Kappa, 0.710603);

            if (null != record.Hits
                    && 0 < record.Hits.Count)
            {
                Assert.AreEqual(record.Hits.Count, 500);

                Assert.AreEqual(record.Hits[0].Accession, "CY063566");
                Assert.AreEqual(record.Hits[0].Hsps.Count, 1);

                if (null != record.Hits[0].Hsps
                        && 0 < record.Hits[0].Hsps.Count)
                {
                    Hsp hsp = record.Hits[0].Hsps[0];
                    Assert.AreEqual(hsp.AlignmentLength, 1701);
                    Assert.AreEqual(hsp.BitScore, 3372.48);
                    Assert.AreEqual(hsp.HitSequence, "ATGAAGGCAATACTAGTAGTTCTGCTATATACATTTGCAACCGCAAATGCAGACACATTATGTATAGGTTATCATGCGAACAATTCAACAGACACTGTAGACACAGTACTAGAAAAGAATGTAACAGTAACACACTCTGTTAACCTTCTAGAAGACAAGCATAACGGGAAACTATGCAAACTAAGAGGGGTAGCCCCATTGCATTTGGGTAAATGTAACATTGCTGGCTGGATCCTGGGAAATCCAGAGTGTGAATCACTCTCCACAGCAAGCTCATGGTCCTACATTGTGGAAACATCTAGTTCAGACAATGGAACGTGTTACCCAGGAGATTTCATCGATTATGAGGAGCTAAGAGAGCAATTGAGCTCAGTGTCATCATTTGAAAGGTTTGAGATATTCCCCAAGACAAGTTCATGGCCCAATCATGACTCGAACAAAGGTGTAACGGCAGCATGTCCTCATGCTGGAGCAAAAAGCTTCTACAAAAATTTAATATGGCTAGTTAAAAAAGGAAATTCATACCCAAAGCTCAGCAAATCCTACATTAATGATAAAGGGAAAGAAGTCCTCGTGCTATGGGGCATTCACCATCCATCTACTAGTGCTGACCAACAAAGTCTCTATCAGAATGCAGATGCATATGTTTTTGTGGGGACATCAAGATACAGCAAGAAGTTCAAGCCGGAAATAGCAATAAGACCCAAAGTGAGGGATCAAGAAGGGAGAATGAACTATTACTGGACACTAGTAGAGCCGGGAGACAAAATAACATTCGAAGCAACTGGAAATCTAGTGGTACCGAGATATGCATTCGCAATGGAAAGAAATGCTGGATCTGGTATTATCATTTCAGATACACCAGTCCACGATTGCAATACAACTTGTCAGACACCCAAGGGTGCTATAAACACCAGCCTCCCATTTCAGAATATACATCCGATCACAATTGGAAAATGTCCAAAATATGTAAAAAGCACAAAATTGAGACTGGCCACAGGATTGAGGAATGTCCCGTCTATTCAATCTAGAGGCCTATTTGGGGCCATTGCCGGTTTCATTGAAGGGGGGTGGACAGGGATGGTAGATGGATGGTACGGTTATCACCATCAAAATGAGCAGGGGTCAGGATATGCAGCCGACCTGAAGAGCACACAGAATGCCATTGACGAGATTACTAACAAAGTAAATTCTGTTATTGAAAAGATGAATACACAGTTCACAGCAGTAGGTAAAGAGTTCAACCACCTGGAAAAAAGAATAGAGAATTTAAATAAAAAAATTGATGATGGTTTCCTGGACATTTGGACTTACAATGCCGAACTGTTGGTTCTATTGGAAAATGAAAGAACTTTGGACTACCACGATTCAAATGTGAAGAACTTATATGAAAAGGTAAGAAGCCAGTTAAAAAACAATGCCAAGGAAATTGGAAACGGCTGCTTTGAATTTTACCACAAATGCGATAACACGTGCATGGAAAGTGTCAAAAATGGGACTTATGACTACCCAAAATACTCAGAGGAAGCAAAATTAAACAGAGAAGAAATAGATGGGGTAAAGCTGGAATCAACAAGGATTTACCAGATTTTGGCGATCTATTCAACTGTCGCCAGTTCATTGGTACTGGTAGTCTCCCTGGGGGCAATCAGTTTCTGGATGTGCTCTAATGGGTCTCTACAGTGTAGAATATGTATTTAA");
                }
                else
                {
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "No significant hits found with the these parameters."));
                }
            }
            else
            {
                ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                    "No significant hits found with the these parameters."));
            }
        }

        /// <summary>
        /// Test the BLAST XML parser with empty input.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [TestCategory("Priority0")]
        public void TestBlastXmlParser_EmptyInput()
        {
            string content = string.Empty;
            StringReader reader = new StringReader(content);
            BlastXmlParser parser = new BlastXmlParser();
            bool ok = false;
            try
            {
                parser.Parse(reader);
            }
            catch (FormatException ex)
            {
                Assert.IsTrue(ex.Message.Contains("No records were found in the input."));
                ok = true;
            }
            Assert.IsTrue(ok);
        }

        /// <summary>
        /// Test the Azure Blast class for DNA (BLASTX).
        /// <remarks>
        ///     "alu.a" database does not support DNA sequence
        ///     Commenting / Disabling this test case.
        /// </remarks>
        /// </summary>
        //[TestMethod] [Priority(0)] [TestCategory("Priority0")]
        public void TestAzureWuBlast_Dna()
        {
            ConfigParameters configParams = new ConfigParameters();
            configParams.DefaultTimeout = 60;
            configParams.UseBrowserProxy = true;
            configParams.Connection = new Uri("http://blast2.cloudapp.net/BlastService.svc");

            IBlastServiceHandler service = null;
            try
            {
                service = new AzureBlastHandler(configParams);

                BlastParameters blastParams = new BlastParameters();
                blastParams.Add("Program", "BlastX");
                blastParams.Add("Database", "nr.25");

                string query = @"ATTTTTTACGAACCTGTGGAAATTTTTGGTTATGACAATAAATCTAGTTTAGTACTTGTGAAACGTTTAA"
                        + "TTACTCGAATGTATCAACAGAATTTTTTGATTTCTTCGGTTAATGATTCTAACCAAAAAGGATTTTGGGG"
                        + "GCACAAGCATTTTTTTTCTTCTCATTTTTCTTCTCAAATGGTATCAGAAGGTTTTGGAGTCATTCTGGAA"
                        + "ATTCCATTCTCGTCGCAATTAGTATCTTCTCTTGAAGAAAAAAAAATACCAAAATATCAGAATTTACGAT"
                        + "CTATTCATTCAATATTTCCCTTTTTAGAAGACAAATTTTTACATTTGAATTATGTGTCAGATCTACTAAT"
                        + "ACCCCATCCCATCCATCTGGAAATCTTGGTTCAAATCCTTCAATGCCGGATCAAGGATGTTCCTTCTTTG"
                        + "CATTTATTGCGATTGCTTTTCCACGAATATCATAATTTGAATAGTCTCATTACTTCAAAGAAATTCATTT"
                        + "ACGCCTTTTCAAAAAGAAAGAAAAGATTCCTTTGGTTACTATATAATTCTTATGTATATGAATGCGAATA"
                        + "TCTATTCCAGTTTCTTCGTAAACAGTCTTCTTATTTACGATCAACATCTTCTGGAGTCTTTCTTGAGCGA"
                        + "ACACATTTATATGTAAAAATAGAACATCTTCTAGTAGTGTGTTGTAATTCTTTTCAGAGGATCCTATGCT"
                        + "TTCTCAAGGATCCTTTCATGCATTATGTTCGATATCAAGGAAAAGCAATTCTGGCTTCAAAGGGAACTCT"
                        + "TATTCTGATGAAGAAATGGAAATTTCATCTTGTGAATTTTTGGCAATCTTATTTTCACTTTTGGTCTCAA"
                        + "CCGTATAGGATTCATATAAAGCAATTATCCAACTATTCCTTCTCTTTTCTGGGGTATTTTTCAAGTGTAC"
                        + "TAGAAAATCATTTGGTAGTAAGAAATCAAATGCTAGAGAATTCATTTATAATAAATCTTCTGACTAAGAA"
                        + "ATTCGATACCATAGCCCCAGTTATTTCTCTTATTGGATCATTGTCGAAAGCTCAATTTTGTACTGTATTG"
                        + "GGTCATCCTATTAGTAAACCGATCTGGACCGATTTCTCGGATTCTGATATTCTTGATCGATTTTGCCGGA"
                        + "TATGTAGAAATCTTTGTCGTTATCACAGCGGATCCTCAAAAAAACAGGTTTTGTATCGTATAAAATATAT"
                        + "ACTTCGACTTTCGTGTGCTAGAACTTTGGCACGGAAACATAAAAGTACAGTACGCACTTTTATGCGAAGA"
                        + "TTAGGTTCGGGATTATTAGAAGAATTCTTTATGGAAGAAGAA";
                Sequence sequence = new Sequence(Alphabets.DNA, query);

                string jobID = service.SubmitRequest(sequence, blastParams);
                ServiceRequestInformation info = service.GetRequestStatus(jobID);

                int maxAttempts = 20;
                int attempt = 1;
                object resultsObject = null;
                while (attempt <= maxAttempts
                        && info.Status != ServiceRequestStatus.Error
                        && info.Status != ServiceRequestStatus.Ready)
                {
                    ++attempt;
                    info = service.GetRequestStatus(jobID);
                    Thread.Sleep(
                        info.Status == ServiceRequestStatus.Waiting
                        || info.Status == ServiceRequestStatus.Queued
                        ? 20000 * attempt
                        : 0);
                }

                IBlastParser blastXmlParser = new BlastXmlParser();
                resultsObject = blastXmlParser.Parse(
                        new StringReader(service.GetResult(jobID, blastParams)));
                Assert.IsNotNull(resultsObject);
                List<BlastResult> results = resultsObject as List<BlastResult>;
                Assert.IsNotNull(results);
                Assert.AreEqual(results.Count, 1);
                Assert.AreEqual(results[0].Records.Count, 1);
                BlastSearchRecord record = results[0].Records[0];
                Assert.AreEqual(record.Statistics.Kappa, 0.041);
                Assert.AreEqual(record.Statistics.Lambda, 0.267);
                Assert.AreEqual(record.Statistics.Entropy, 0.14);

                if (null != record.Hits
                        && 0 < record.Hits.Count)
                {
                    Assert.AreEqual(record.Hits.Count, 259);

                    Hit hit = record.Hits[0];
                    Assert.AreEqual(hit.Accession, "5412286");
                    Assert.AreEqual(hit.Id, "gnl|BL_ORD_ID|5412286");

                    if (null != hit.Hsps
                            && 0 < hit.Hsps.Count)
                    {
                        Assert.AreEqual(hit.Hsps.Count, 1);
                        Assert.AreEqual(hit.Hsps[0].HitSequence.Substring(0, 30), "FYKPVEIFGYDNKSSLVLVKRLITRMYQQN");
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }

                    IList<BlastResult> results2 = service.FetchResultsSync(jobID, blastParams) as List<BlastResult>;
                    Assert.IsNotNull(results2);
                    if (null != results[0].Records[0].Hits
                            && 0 < results[0].Records[0].Hits.Count
                            && null != results[0].Records[0].Hits[0].Hsps
                            && 0 < results[0].Records[0].Hits[0].Hsps.Count)
                    {
                        Assert.AreEqual(results[0].Records[0].Hits[0].Hsps[0].QuerySequence,
                            results2[0].Records[0].Hits[0].Hsps[0].QuerySequence);
                    }
                    else
                    {
                        ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                            "No significant hits found with the these parameters."));
                    }
                }
                else
                {
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "No significant hits found with the these parameters."));
                }
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }


        }

        /// <summary>
        /// Test the Azure Blast class for protein (BLASTX).
        /// Will return 0 Hits
        /// <remarks>
        ///     Test data in this test case work fine in azure web page.
        ///     But returns invalid result path when accessed using web service.
        ///     Looks like there is some issue with AzureBlast service. 
        ///     Commenting / Disabling this test case.
        /// </remarks>
        /// </summary>
        //[TestMethod] [Priority(0)] [TestCategory("Priority0")]
        public void TestAzureWuBlast_Protein()
        {
            ConfigParameters configParams = new ConfigParameters();
            configParams.DefaultTimeout = 60;
            configParams.UseBrowserProxy = true;
            configParams.Connection = new Uri("http://blast2.cloudapp.net/BlastService.svc");

            IBlastServiceHandler service = null;
            try
            {
                service = new AzureBlastHandler(configParams);

                BlastParameters blastParams = new BlastParameters();
                blastParams.Add("Program", "BlastP");
                blastParams.Add("Database", "alu.a");

                string query = @"MAYPMQLGFQDATSPIMEELLHFHDHTLMIVFLISSLVLYIISLMLTTKLTHTSTMDAQEVETIWTILPAIILILI"
                        + "ALPSLRILYMMDEINNPSLTVKTMGHQWYWSYEYTDYEDLSFDSYMIPTSELKPGELRLLEVDNRVVLPMEMTIRM"
                        + "LVSSEDVLHSWAVPSLGLKTDAIPGRLNQTTLMSSRPGLYYGQCSEICGSNHSFMPIVLELVPLKYFEKWSASML";
                Sequence sequence = new Sequence(Alphabets.Protein, query);

                string jobID = service.SubmitRequest(sequence, blastParams);
                ServiceRequestInformation info = service.GetRequestStatus(jobID);

                int maxAttempts = 20;
                int attempt = 1;
                object resultsObject = null;
                while (attempt <= maxAttempts
                        && info.Status != ServiceRequestStatus.Error
                        && info.Status != ServiceRequestStatus.Ready)
                {
                    ++attempt;
                    info = service.GetRequestStatus(jobID);
                    Thread.Sleep(
                        info.Status == ServiceRequestStatus.Waiting
                        || info.Status == ServiceRequestStatus.Queued
                        ? 20000 * attempt
                        : 0);
                }

                IBlastParser blastXmlParser = new BlastXmlParser();
                resultsObject = blastXmlParser.Parse(
                        new StringReader(service.GetResult(jobID, blastParams)));
                Assert.IsNotNull(resultsObject);
                List<BlastResult> results = resultsObject as List<BlastResult>;
                Assert.IsNotNull(results);
                Assert.AreEqual(results.Count, 1);
                Assert.AreEqual(results[0].Records.Count, 2);
                BlastSearchRecord record = results[0].Records[0];
                Assert.AreEqual(record.Statistics.Kappa, 0.041);
                Assert.AreEqual(record.Statistics.Lambda, 0.267);
                Assert.AreEqual(record.Statistics.Entropy, 0.14);

                if (null == record.Hits
                        || 0 == record.Hits.Count)
                {
                    ApplicationLog.WriteLine(string.Format((IFormatProvider)null,
                        "No significant hits found with the these parameters."));
                }
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }
        }
        private static ServiceRequestInformation ServiceRequestInfoForDatabase(Dictionary<string, object> testCaseParameters)
        {
            IBlastServiceHandler service = testCaseParameters[BLASTHANDLER] as IBlastServiceHandler;
            BlastParameters searchParams = testCaseParameters[BLASTPARAMETERS] as BlastParameters;
            ISequence sequence = testCaseParameters[TESTSEQUENCE] as ISequence;

            // create the request
            string jobId = service.SubmitRequest(sequence, searchParams);
            Assert.IsFalse(string.IsNullOrEmpty(jobId));
            
            // Add to test case parameters
            testCaseParameters.Add(JOBIDENTIFIER, jobId);

            // query the status
            ServiceRequestInformation info = service.GetRequestStatus(jobId);
            return info;
        }
        /// <summary>
        /// Callback function to execute blast search asynchronously
        /// </summary>
        /// <param name="testCaseParameters">Test case parameters</param>
        /// <returns>Test case output</returns>
        private static TestCaseOutput Callback_FetchResultsAsync(Dictionary<string, object> testCaseParameters)
        {
            IBlastServiceHandler service = testCaseParameters[BLASTHANDLER] as IBlastServiceHandler;
            BlastParameters searchParams = testCaseParameters[BLASTPARAMETERS] as BlastParameters;
            Sequence sequence = testCaseParameters[TESTSEQUENCE] as Sequence;

            // create the request
            string jobId = service.SubmitRequest(sequence, searchParams);
            Assert.IsFalse(string.IsNullOrEmpty(jobId));
            
            // Add to test case parameters
            testCaseParameters.Add(JOBIDENTIFIER, jobId);

            // query the status
            ServiceRequestInformation info = service.GetRequestStatus(jobId);
            if (info.Status != ServiceRequestStatus.Waiting && info.Status != ServiceRequestStatus.Ready)
            {
                string err = ApplicationLog.WriteLine("Unexpected status: '{0}'", info.Status);
                Assert.Fail(err);
            }

            // get async results, poll until ready
            int maxAttempts = 10;
            int attempt = 1;
            object resultsObject = null;
            while (attempt <= maxAttempts
                    && info.Status != ServiceRequestStatus.Error
                    && info.Status != ServiceRequestStatus.Ready)
            {
                ++attempt;
                info = service.GetRequestStatus(jobId);
                Thread.Sleep(
                    info.Status == ServiceRequestStatus.Waiting
                    || info.Status == ServiceRequestStatus.Queued
                    ? 20000 * attempt
                    : 0);
            }

            IBlastParser blastXmlParser = new BlastXmlParser();

            using (StringReader reader = new StringReader(service.GetResult(jobId, searchParams)))
            {
                resultsObject = blastXmlParser.Parse(reader);
            }

            return new TestCaseOutput(resultsObject, false);
        }

        /// <summary>
        /// Callback function to execute blast search synchronously
        /// </summary>
        /// <param name="testCaseParameters">Test case parameters</param>
        /// <returns>Test case output</returns>
        private static TestCaseOutput Callback_FetchResultsSync(Dictionary<string, object> testCaseParameters)
        {
            IBlastServiceHandler service = testCaseParameters[BLASTHANDLER] as IBlastServiceHandler;
            BlastParameters searchParams = testCaseParameters[BLASTPARAMETERS] as BlastParameters;
            string jobID = testCaseParameters[JOBIDENTIFIER] as string;

            IList<BlastResult> results2 = service.FetchResultsSync(jobID, searchParams) as List<BlastResult>;
            return new TestCaseOutput(results2, false);
        }

        /// <summary>
        /// Callback function to execute Submit request and Get status
        /// </summary>
        /// <param name="testCaseParameters">Test case parameters</param>
        /// <returns>Test case output</returns>
        private static TestCaseOutput Callback_EBIWuBlast_GetServiceMetadata(Dictionary<string, object> testCaseParameters)
        {
            EbiWuBlastHandler service = null;
            TestCaseOutput output = null;

            try
            {
                service = new EbiWuBlastHandler();
                string kind = testCaseParameters[SERVICEKIND] as string;

                IList<string> info = service.GetServiceMetadata(kind);
                output = new TestCaseOutput(info, false);
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }

            return output;
        }

        /// <summary>
        /// Callback function to execute Submit request and Get status
        /// </summary>
        /// <param name="testCaseParameters">Test case parameters</param>
        /// <returns>Test case output</returns>
        private static TestCaseOutput Callback_BioHPCBlast_GetServiceMetadata(Dictionary<string, object> testCaseParameters)
        {
            BioHPCBlastHandler service = null;
            TestCaseOutput output = null;

            try
            {
                service = new BioHPCBlastHandler();
                service.Configuration = new ConfigParameters();
                service.Configuration.EmailAddress = "msrerbio@microsoft.com";
                service.Configuration.Password = "";
                string kind = testCaseParameters[SERVICEKIND] as string;

                IList<string> info = service.GetServiceMetadata(kind);
                output = new TestCaseOutput(info, false);
            }
            finally
            {
                if (service != null)
                    ((IDisposable)service).Dispose();
            }

            return output;
        }
    }
}
