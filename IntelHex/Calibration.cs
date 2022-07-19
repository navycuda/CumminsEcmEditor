using CumminsEcmEditor.Cummins;
using CumminsEcmEditor.Tools;
using CumminsEcmEditor.Tools.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CumminsEcmEditor.IntelHex
{
    public class Calibration
    {
        #region Private Properties
        private string FilePath { get; set; }
        private string? CheckSum { get; set; }
        private XmlHeader? Header { get; set; }
        private Record[] Records { get; set; }
        #endregion

        #region Properites
        public Cursor Cursor { get; set; }
        public ItnTableOfContents TableOfContents { get; set; }
        public EngineDocument Document { get; set; }
        #endregion

        #region Constructor
        public Calibration(string filePath)
        {
            FilePath = filePath;
            string[] xCal = EcmFiles.Load(filePath);
            int eLA = 0;
            List<Record> records = new();
            List<string> headers = new();
            // Iterate through the xCal records
            foreach (string s in xCal)
            {
                Record r;
                int e;
                // If this string is a valid record, then
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
            // Setup the cursor
            Cursor = new(Records);
            // Set the CheckSum, if the file is not headerless

            GetXmlHeader(headers.ToArray());


            // Must be assembled last, after calibration has been loaded.
            TableOfContents = new(this);
            // Assign the Xcal a Document
            Document = new(this);
        }
        #endregion

        #region Public Methods
        public void SaveModdedCalibration(bool overwrite = false)
        {
            string[][] calibration = new string[2][];
            string filePath = FilePath.Replace(".XCAL", "");
            filePath += "_mod.XCAL";
            List<string> header = new();
            if (CheckSum != null)
                header.Add(CheckSum);
            if (Header != null)
                header.Add(Header.GetXmlHeader());
            calibration[0] = header.ToArray();
            calibration[1] = GetIntelHexRecords();

            EcmFiles.Save(filePath, calibration, overwrite);
        }
        public XCalByteOrder GetByteOrder()
        {
            if (Header == null)
                return XCalByteOrder.LittleEndian;
            else if (Header.byte_order == "LittleEndian")
                return XCalByteOrder.LittleEndian;
            return XCalByteOrder.BigEndian;
        }
        public int GetTableOfContentsAddress() => Header.index_table_address.HexToInt();
        public string GetXCalPath() => FilePath;
        #endregion

        #region Private Methods
        /// <summary>
        /// Generates the intel hex line record for the .XCAL
        /// </summary>
        private string[] GetIntelHexRecords()
        {
            string[] records = new string[Records.Length];
            for (int i = 0; i < records.Length; i++)
                records[i] = Records[i].GetIntelHexString();
            return records;
        }
        private void GetXmlHeader(string[] xmlHeader)
        {
            string result;
            if (xmlHeader.Length > 0)
                CheckSum = xmlHeader[0];
            result = string.Join("", xmlHeader[1..]);
            using (StringReader sr = new(result))
            {
                XmlSerializer xmlSerializer = new(typeof(XmlHeader));
                Header = (XmlHeader?)xmlSerializer.Deserialize(sr);
            }
        }
        #endregion
    }
    public enum XCalByteOrder
    {
        LittleEndian = 0x00,
        BigEndian = 0x01,
    }
}
