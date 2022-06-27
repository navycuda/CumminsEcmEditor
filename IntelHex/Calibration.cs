using CumminsEcmEditor.Tools;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CumminsEcmEditor.IntelHex
{
    public class Calibration
    {
        #region Private Properties
        private string CheckSum { get; set; }
        private XmlHeader Header { get; set; }
        private Record[] Records { get; set; }
        #endregion

        #region Constructor
        public Calibration(string filePath)
        {
            string[] xCal = Files.Load(filePath);
            int eLA = 0;
            List<Record> records = new();
            List<string> headers = new();
            // Iterate through the xCal records
            foreach (string s in xCal)
            {
                Record r;
                int e;
                // If there this string is a valid record, then
                // create a new record and add it to the list
                // as well as updating the extended linear address
                if (Record.NewRecord(s, eLA, out r, out e))
                {
                    eLA = e;
                    records.Add(r);
                }
                // Or dump the string into the headers and deal with
                // it later.  (non critical but I'll spend some time
                // here for completeness and better xcal/ecfg compa-
                // risons.
                else
                {
                    headers.Add(s);
                }
            }
            // Set the Records array.
            Records = records.ToArray();
            // Set the CheckSum
            CheckSum = headers[0];
        }
        #endregion
    }
}
