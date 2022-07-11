using CumminsEcmEditor.IntelHex;
using CumminsEcmEditor.Tools.Extensions;
using CumminsEcmEditor.WinOLS;

using System;
using System.Collections.Generic;
using System.Data;
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
        private string EcfgPath { get; set; }
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
            EcfgPath = ecfgPath;
            // Load the configuration
            Configuration = ConfigurationFile.Load(EcfgPath);
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
        public Itn[] GetAllItns(SortItnsBy sortedBy)
        {
            // TO DO:
            // Add sorting options.
            return Contents;
        }
        public Itn? GetItnById(int id) =>
            Contents.Where(i => i.Id == id).FirstOrDefault();
        public byte[][] GetData(int absoluteAddress, int bytes, int elements) =>
            XCal.Cursor.Read(absoluteAddress, bytes, elements);
        public byte[] GetData(int absoluteAddress, int bytes) =>
            XCal.Cursor.Read(absoluteAddress, bytes);
        public string GetEcfgPath() => EcfgPath;
        #endregion

        #region WinOLS Conversion Public Methods
        public void ConvertMapPackToConfiguration(string mapPath, string configPath)
        {
            // Load the map pack
            MapPack mapPack = new(mapPath);
            // Prepare an emtpy Configuration File
            ConfigurationFile ecfg = new();
            // Prepare an empty list of ecm parameters
            List<EcmParameter> ecmParameters = new();
            // Iternate through the maps and convert them into the EcmParameters
            foreach (Map map in mapPack.maps)
                ecmParameters.AddRange(GetParametersFrom(map));
            // Add the parameters to the ecfg after sorted by Id.
            ecfg.Parameters = ecmParameters.OrderBy(p => p.GetId()).ToArray();
            // Save the ecfg
            ecfg.Save(configPath);
        }
        #endregion

        #region WinOLS Conversion Private Methods
        private EcmParameter[] GetParametersFrom(Map map)
        {
            // Prepare the ecmParameter Array
            List<EcmParameter> parameters = new();
            // setup the initial ecmParameter
            EcmParameter ecmParameter = new()
            {
                name = map.Name,
                id = map.IdName.HexToInt().ToString(),
                description = map.FieldvaluesName,
                group_ids = "" // Group Ids will need to be addressed as a future part of the converter.
            };
            // Setup the flags enum
            EcmParameterType ecmPT = GetEcmParameterType(map);
            // Send to the correct method depending on the ParameterType
            if (ecmPT == EcmParameterType.Floating_Point)
                ecmParameter.data_type = GetFloating_Point(map);
            else if (ecmPT == EcmParameterType.Fixed_Point)
                ecmParameter.data_type = GetFixed_Point(map);
            else if ((ecmPT & EcmParameterType.Y_Axis) != 0)
                ecmParameter.data_type = GetY_Axis(map, parameters, ecmPT);
            

            // return the list as an array
            return parameters.ToArray();
        }
        private Y_Axis GetY_Axis(Map map, List<EcmParameter> parameters, EcmParameterType pT)
        {
            DataType y_element_type;
            if ((pT & EcmParameterType.Floating_Point) != 0)
                y_element_type = GetFloating_Point(map);
            else
                y_element_type = GetFixed_Point(map);

            Y_Axis y_Axis = new()
            {
                y_element_type = y_element_type,
            };
            EcmParameter x_Axis = new()
            {
                name = map.AxisXIdName,
                id = GetX_AxisId(map, map.IdName.HexToInt()),
                description = map.AxisXName,
                group_ids = "",
                data_type = new X_Axis()
                {

                }
            };
        }
        private string GetX_AxisId(Map map, int yAxisId)
        {

            return "";
        }
        private static Fixed_Point GetFixed_Point(Map map) =>
            new()
            {
                engr_units = map.FieldvaluesUnit,
                data_length = GetDataLength(map),
                sign = map.bSigned == "1" ? "S" : "U",
                scalar_multiplier = map.FieldvaluesFactor
            };
        private static Fixed_Point GetFixed_Point(string units, string dataLength, string _sign, string scalar) =>
            new()
            {
                engr_units = units,
                data_length = dataLength,
                sign = _sign,
                scalar_multiplier = scalar
            };
        private static Floating_Point GetFloating_Point(Map map) =>
            GetFloating_Point(map.FieldvaluesUnit, "4");
        private static Floating_Point GetFloating_Point(string units, string dataLength) =>
            new() { engr_units = units, data_length = dataLength };
        private static EcmParameterType GetEcmParameterType(Map map)
        {
            int rows = map.GetRows();
            int columns = map.GetColumns();
            // Single Value Parameter, ie a toggle
            if (map.Type == "eEinzel" && (rows * columns) == 1)
                return GetDataOrg(map);
            // Y_Axis - single dimension(in winOLS) but has an x-axis
            if (map.Type == "eEindim" && rows == 1 && columns > 1)
                return EcmParameterType.Y_Axis | GetDataOrg(map);
            // Z_Axis - two dimensions(in winOLS) has x,y axis
            if (map.Type == "eZweidim" && rows > 1 && columns > 1)
                return EcmParameterType.Z_Axis | GetDataOrg(map);
            // Table - one dimension has no axis data
            bool isTable = map.AxisXIdName == "" && map.AxisYIdName == "";
            if (map.Type == "eZweidim" && rows > 1 && columns == 1 && isTable)
                return EcmParameterType.Table | GetDataOrg(map);
            return EcmParameterType.None;
        }
        private static EcmParameterType GetDataOrg(string dataOrg)
        {
            if (dataOrg == "eFloatLoHi")
                return EcmParameterType.Floating_Point;
            else if (dataOrg == "eLoHiLoHi")            // 4 bytes
                return EcmParameterType.Fixed_Point;
            else if (dataOrg == "eLoHi")                // 2 bytes
                return EcmParameterType.Fixed_Point;
            else if (dataOrg == "eByte")                // 1 byte
                return EcmParameterType.Fixed_Point;
            return EcmParameterType.None;
        }
        private static EcmParameterType GetDataOrg(Map map) =>
            GetDataOrg(map.DataOrg);
        private static string GetDataLength(Map map)
        {
            if (map.DataOrg == "eLoHiLoHi" || map.DataOrg == "eFloatLoHi")
                return "4";
            else if (map.DataOrg == "eLoHi")
                return "2";
            return "1";
        }
        private static string GetDataLength(string dataOrg)
        {
            if (dataOrg == "eLoHiLoHi" || dataOrg == "eFloatLoHi")
                return "4";
            else if (dataOrg == "eLoHi")
                return "2";
            return "1";
        }
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
                Contents[i / 2] = new(
                    this,
                    unpackedRecordIds[i / 2],
                    mattTable[i].ToInt(ByteOrder),
                    mattTable[i + 1].ToInt(ByteOrder));
            // Sort for good measure
            Contents = Contents.OrderBy(c => c.Id).ToArray();
        }
        #endregion
    }
    [Flags]
    public enum SortItnsBy
    {
        None        = 0,
        Manager     = 0b1,
        Group       = 0b01,
        Itn         = 0b001,
        Address     = 0b0001
    }
    [Flags]
    public enum EcmParameterType
    {
        None = 0,
        Floating_Point  = 0b1,
        Fixed_Point     = 0b10,
        X_Axis          = 0b100,
        Y_Axis          = 0b1000,
        Z_Axis          = 0b10000,
        Table           = 0b100000
    }
}
