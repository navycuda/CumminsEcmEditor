using CumminsEcmEditor.IntelHex;
using CumminsEcmEditor.Tools.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CumminsEcmEditor.Cummins
{
    public class ItnTableOfContents
    {

        #region Private Properties
        private Calibration XCal { get; set; }
        private Itn[] Contents { get; set; }
        private ConfigurationFile? Configuration { get; set; }
        private int Address { get; set; }
        private int PackedRecords { get; set; }
        private int UnpackedRecords { get; set; }
        private XCalByteOrder ByteOrder { get; set; }
        #endregion

        #region Constructor
        public ItnTableOfContents(Calibration xCal)
        {
            XCal = xCal;
            ByteOrder = XCal.GetByteOrder();
            Address = XCal.GetTableOfContentsAddress();
            PackedRecords = XCal.Cursor.Read(Address, 4).ToInt(ByteOrder);
            GetItnParameters(GetPackedItnRecords());
            Console.SetCursorPosition(0, 20);
            for (int i = 0; i < 24; i++)
            {
                string id = "0x" + Contents[i].Id.IntToHex(4);
                string address = "0x" + Contents[i].AbsoluteAddress.IntToHex(4);
                int length = Contents[i].ByteCount;

                Console.WriteLine($"{id} : {address} : {length}");
            }
        }
        #endregion

        #region Public Methods
        public void ApplyConfiguration(string ecfgPath)
        {
            // Load the configuration
            Configuration = ConfigurationFile.Load(ecfgPath);
            EcmParameter[] parameters = Configuration.GetParameters();
            // prepare a counter for the configuration list
            int config = 0;

            // iterate through the Itn Contents
            bool isPairing = true;
            int i = 0;
            int p = 0;
            while (isPairing)
            {
                // set itn and parameter shorthand
                int itn = Contents[i].Id;
                int par = parameters[p].GetId();
                // Walk through the lists
                if (itn == par)
                {
                    // Set the parameter to this itn
                    Contents[i].Parameter = parameters[p];
                    i++;
                    p++;
                }
                else if (itn > par)
                    p++;
                else if (itn < par)
                    i++;
                if (itn == Contents.Length || par == parameters.Length)
                    isPairing = false;
            }

        }
        public Itn? GetItnById(int id) =>
            Contents.Where(i => i.Id == id).FirstOrDefault();
        #endregion
        
        #region Private Methods
        private int[] GetPackedItnRecords()
        {
            List<int> recordIds = new();
            // Add 4 to offset from the record count
            int address = Address + 4;
            // records are packed as two 32-bit words.
            // 00 00 00 00 , 00 00 00 00 = itnId, sequential followers
            int elements = PackedRecords * 2;
            byte[][] packedRecords = XCal.Cursor.Read(address, 4, elements);
            // populate the id list
            for (int p = 0; p < elements; p += 2)
            {
                int id = packedRecords[p].ToInt(ByteOrder);
                int sq = packedRecords[p + 1].ToInt(ByteOrder);
                for (int i = 0; i < sq; i++)
                    recordIds.Add(id+i);
            }
            UnpackedRecords = recordIds.Count();
            return recordIds.ToArray();
        }
        private void GetItnParameters(int[] unpackedRecordIds)
        {
            // Offset the address past the end of the itn table, to the matt table
            int address = Address + 4 + (PackedRecords * 8);
            // Unpacked Records are two 32 bit words
            // absolute Address : Byte Count
            int elements = UnpackedRecords * 2;
            //Prepare the parameter array
            Contents = new Itn[UnpackedRecords];
            // Read the matt table
            byte[][] mattTable = XCal.Cursor.Read(address, 4, elements);
            // Populate the parameters
            for (int i = 0; i < elements; i += 2)
                Contents[i / 2] = new(unpackedRecordIds[i / 2],
                                      mattTable[i].ToInt(ByteOrder),
                                      mattTable[i + 1].ToInt(ByteOrder));
            // Sort for good measure
            Contents = Contents.OrderBy(c => c.Id).ToArray();
        }
        #endregion
    }
}
