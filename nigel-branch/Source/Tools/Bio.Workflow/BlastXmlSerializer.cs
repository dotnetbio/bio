namespace Bio.Workflow
{
    #region -- Using Directive --

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    #endregion -- Using Directive --

    /// <summary>
    /// BlastXmlSerializer implements IBlastSerializer interface, 
    /// would serialize the blast output using Xmlserializer. 
    /// </summary>
    public class BlastXmlSerializer : IBlastSerializer
    {
        /// <summary>
        /// Describes the Serializer type used.
        /// </summary>
        private string serializerType;
        
        /// <summary>
        /// Gets the serializer type used for serialization
        /// </summary>
        public string SerializerType 
        {
            get
            {
                return this.serializerType;
            }
        }

        /// <summary>
        /// This method would serialize the blast result 
        /// and return the serialized stream.
        /// </summary>
        /// <param name="result">Blast Result</param>
        /// <returns>Serialized stream</returns>
        public Stream SerializeBlastOutput(IList<BlastResultCollator> result)
        {
            XmlTextWriter xmlWriter = null;
            MemoryStream memStream = null;
            try
            {
                this.serializerType = "XmlSerializer";
                XmlSerializer serializer = new XmlSerializer(typeof(List<BlastResultCollator>));
                memStream = new MemoryStream();
                xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);
                serializer.Serialize(xmlWriter, result);                
                return memStream;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            finally
            {
                if (xmlWriter != null && memStream != null)
                {
                    xmlWriter.Close();
                    memStream.Close();
                }
            }
        }
    }
}
