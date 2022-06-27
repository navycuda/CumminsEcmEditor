using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CumminsEcmEditor.Tools
{
    public static class Files
    {
        public static string[] Load (string filePath)
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
    }
}
