using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
