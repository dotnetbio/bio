using System.IO;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;

namespace Bio.TestUtils.SimulatorUtility
{
    /// <summary>
    /// This class represents a repository of mock output object. On request reads 
    /// maps a testcaseId to output file path, deserialize the stream in file and 
    /// return the mock output object.
    /// Implements a singleton pattern.
    /// </summary>
    public class MockOutputRepository
    {
        /// <summary>
        /// Static instance of MockOutputRepository.
        /// </summary>
        private static MockOutputRepository _MockOutputRepository;

        private XmlDocument mockOutputMap = null;

        /// <summary>
        /// Private constructor.
        /// </summary>
        private MockOutputRepository()
        {
            mockOutputMap = new XmlDocument();
            mockOutputMap.Load(@"TestUtils\Testcases.xml");
        }
    
        /// <summary>
        /// Gets the singleton instance of MockOutputRepository.
        /// </summary>
        public static MockOutputRepository Instance
        {
            get
            {
                if (_MockOutputRepository == null)
                {
                    lock (typeof(MockOutputRepository))
                    {
                        if (_MockOutputRepository == null)
                        {
                            _MockOutputRepository = new MockOutputRepository();
                        }
                    }
                }

                return _MockOutputRepository;
            }
        }

        /// <summary>
        /// Reads the output stream from file, deserializes and return the object.
        /// </summary>
        /// <param name="testcaseId">Test case identifier.</param>
        public object GetOutput(string testcaseId)
        {
            string mockOutputPath = GetMockOutputPath(testcaseId);
            Stream stream = ReadStream(mockOutputPath);
            object result = Deserialize(stream);
            Close(stream);
            return result;
        }

        /// <summary>
        /// Returns the filename containing mock outtput for given testcaseId.
        /// </summary>
        /// <param name="testcaseId">Test case identifier.</param>
        private string GetMockOutputPath(string testcaseId)
        {
            XmlNode node = mockOutputMap.SelectSingleNode("/TestCases/TestCase[@TestCaseID='" + testcaseId + "']");
            return node.InnerText;
        }

        /// <summary>
        /// Read the stream from file.
        /// </summary>
        /// <param name="filePath">File path.</param>
        private Stream ReadStream(string filePath)
        {
            MemoryStream ms = null;
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
                ms = new MemoryStream(bytes);
            }

            return ms;
        }

        /// <summary>
        /// Deserialize the stream to object.
        /// </summary>
        /// <param name="stream">Stream object to be deserialized.</param>
        private object Deserialize(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);            
        }

        private void Close(Stream stream)
        {
            if (stream != null)
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}
