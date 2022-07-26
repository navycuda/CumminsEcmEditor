using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CumminsEcmEditor.Tools
{
    public static class EcmFiles
    {
        #region Save Methods
        /// <summary>
        /// Saves a single string[] to the filePath
        /// </summary>
        public static void Save(string filePath, string[] document)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                foreach (string s in document)
                    sw.WriteLine(s);
            }
        }
        /// <summary>
        /// Allows assembling of multiple string arrays/lists.
        /// </summary>
        public static void Save(string filePath, string[][] document)
        {
            List<string> result = new();
            for (int s = 0; s < document.Length; s++)
                for (int ss = 0; ss < document[s].Length; ss++)
                    result.Add(document[s][ss]);
            Save(filePath, result.ToArray());
        }
        public static void Save<T>(T xmlFile, string filePath)
        {
            try
            {
                // Prepare variables
                string output = "";
                XmlSerializer xS = new XmlSerializer(typeof(T));
                // String writer is used to satisfied the stream requirement
                // for serialization
                using (UTF8_StringWriter sW = new())
                {
                    // Get the Xml string
                    xS.Serialize(sW, xmlFile);
                    output = sW.ToString();
                    // Setup to format the string
                    MemoryStream mS = new();
                    XmlTextWriter xT = new(mS, Encoding.UTF8);
                    XmlDocument d = new();
                    // Load the xml string into the xml document
                    d.LoadXml(output);
                    xT.Formatting = Formatting.Indented;
                    // Write the xml document into a formatting xmlTextwriter
                    d.WriteContentTo(xT);
                    xT.Flush();
                    mS.Flush();
                    // Rewind the memory stream to read contents
                    mS.Position = 0;
                    // Read MemoryStream content into streamReader
                    using (StreamReader sR = new(mS))
                        output = sR.ReadToEnd();
                }
                using (StreamWriter sW = new(filePath))
                    sW.Write(output);
            }
            catch (XmlException xmlEx)
            {
                Console.WriteLine(xmlEx);
            }
        }
        public static void Save<T>(T xmlFile, string filePath, bool overwrite)
        {
            if (overwrite)
                AndItsGone(filePath);
            Save(xmlFile, filePath);
        }
        public static void Save(string filePath, string[] document, bool overwrite)
        {
            if (!File.Exists(filePath))
                Save(filePath, document);
            if (overwrite)
                AndItsGone(filePath);
            Save(filePath, document);
        }
        public static void Save(string filePath, string[][] document, bool overwrite)
        {
            if (!File.Exists(filePath))
                Save(filePath, document);
            if (overwrite)
                AndItsGone(filePath);
            Save(filePath, document);
        }
        #endregion

        #region Load Methods
        public static byte[] LoadBinary(string binPath) 
        {
          using (FileStream fs = File.OpenRead(binPath))
          {
            List<byte> binary = new();
            using (BinaryReader reader = new(fs))
            {
              while (reader.BaseStream.Position != reader.BaseStream.Length)
              {
                binary.Add(reader.ReadByte());
              }
            }
            return binary.ToArray();
          }
        }
        /// <summary>
        /// Loads a string array from the filePatj
        /// </summary>
        public static string[] Load(string filePath)
        {
            List<string> result = new();
            using (StreamReader sr = new(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                    result.Add(line);
            }
            return result.ToArray();
        }
        /// <summary>
        /// Load XML File
        /// </summary>
        public static T? Load<T>(string filePath)
        {
            // Make sure the file exists or return a default value
            if (!File.Exists(filePath))
                return default(T);

            using (TextReader tR = new StreamReader(filePath))
            {
                using (XmlTextReader xR = new(tR))
                {
                    XmlSerializer xS = new XmlSerializer(typeof(T));
                    return (T?)xS.Deserialize(xR);
                }
            }
        }
        #endregion

        #region Xml Methods
        /// <summary>
        /// Converts the xmlModel to a string for file creation
        /// </summary>
        public static string GetAsXml<T>(T xmlModel, bool indented = false)
        {

            string output = "";
            XmlSerializer xS = new XmlSerializer(typeof(T));
            using (UTF8_StringWriter sW = new())
            {
                // Get the Xml string
                xS.Serialize(sW, xmlModel);
                output = sW.ToString();
                // Setup to format the string
                MemoryStream mS = new();
                XmlTextWriter xT = new(mS, Encoding.UTF8);
                XmlDocument d = new();
                // Load the xml string into the xml document
                d.LoadXml(output);
                xT.Formatting = indented ? Formatting.Indented : Formatting.None;
                // Write the xml document into a formatting xmlTextwriter
                d.WriteContentTo(xT);
                xT.Flush();
                mS.Flush();
                // Rewind the memory stream to read contents
                mS.Position = 0;
                // Read MemoryStream content into streamReader
                using (StreamReader sR = new(mS))
                    output = sR.ReadToEnd();
            }
            return output;
        }
        /// <summary>
        /// Currently only used to serialize the xcal header.
        /// </summary>
        public static string XmlSerialize<T>(T entity) where T : class
        {
            // removes version
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            XmlSerializer xsSubmit = new XmlSerializer(typeof(T));
            using (StringWriter sw = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create(sw, settings))
            {
                // removes namespace
                var xmlns = new XmlSerializerNamespaces();
                xmlns.Add(string.Empty, string.Empty);

                xsSubmit.Serialize(writer, entity, xmlns);
                return sw.ToString(); // Your XML
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// What a nice file you have there...
        /// </summary>
        private static void AndItsGone(string filePath) 
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
        #endregion
    }
    public class UTF8_StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
