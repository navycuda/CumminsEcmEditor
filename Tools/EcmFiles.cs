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
        public static void OverwriteSave(string filePath, string[] document)
        {
            AndItsGone(filePath);
            Save(filePath, document);
        }
        public static void OverwriteSave(string filePath, string[][] document)
        {
            AndItsGone(filePath);
            Save(filePath, document);
        }
        public static string GetAsXml<T>(T xmlModel)
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
            return output;
        }
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
        private static void AndItsGone(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
    public class UTF8_StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
