using CumminsEcmEditor.IntelHex;
using CumminsEcmEditor.Tools.Extensions;
using CumminsEcmEditor.WinOLS;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
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
        private string EcfgPath { get; set; }
        private List<EcmParameter> EcmParameters { get; set; }
        private int _NegativeCounter = -1;
        private int NegativeCounter => _NegativeCounter--;
        #endregion

        #region Properties
        public XCalByteOrder ByteOrder { get; set; }
        #endregion

        #region Constructor
        public ItnTableOfContents(Calibration xCal)
        {
            XCal = xCal;
            ByteOrder = XCal.GetByteOrder();
            Address = XCal.GetTableOfContentsAddress();
            PackedRecords = XCal.Cursor.Read(Address, 4).ToInt(ByteOrder);
            GetItnParameters(GetPackedItnRecords());
        }
        #endregion

        #region Public Methods
        public void ApplyConfiguration(string ecfgPath)
        {
            EcfgPath = ecfgPath;
            // Load the configuration
            Configuration = ConfigurationFile.Load(EcfgPath);
            EcmParameter[] parameters = Configuration.GetParameters();
            Console.WriteLine($"\tparameters.Length = {parameters.Length}");
            Console.WriteLine($"\tContents.Length   = {Contents.Length}");
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
                if (i == Contents.Length || p == parameters.Length)
                    isPairing = false;


                // Temp for debugging
                if (itn == 3)
                {
                  Console.WriteLine($"Calibration_Date_Stamp address = {Contents[i].GetHexAddress()}");
                  byte[] dateStamp = GetData(Contents[i].AbsoluteAddress, 6);
                  foreach (byte b in dateStamp){
                    Console.WriteLine(b.ToString("X2"));
                  }
                }
                if (itn == 1)
                {
                  Console.WriteLine($"Block Data Structure address = {Contents[i].GetHexAddress()}");
                  byte[] blockStructure = GetData(Contents[i].AbsoluteAddress, 60);
                  Console.WriteLine();
                  foreach (byte b in blockStructure)
                  {
                    Console.Write($"{b.ToString("X2")} ");
                  }
                  Console.WriteLine();
                }
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
          // Curiousity sake, number of itns
          Console.WriteLine($"{Contents.Length} itns to process");
          // Load the map pack
          MapPack mapPack = new(mapPath);
          // Prepare an emtpy Configuration File
          ConfigurationFile ecfg = new();
          // Prepare an empty list of ecm parameters
          EcmParameters = new();
          // Iternate through the maps and convert them into the EcmParameters
          foreach (Map map in mapPack.maps)
              EcmParameters.AddRange(GetParametersFrom(map));
          // Add the parameters to the ecfg after sorted by Id.
          ecfg.Parameters = EcmParameters.OrderBy(p => p.GetId()).ToArray();
          // How many Parameters Found?
          Console.WriteLine($"{ecfg.Parameters.Length} parameters mapped...");
          // Save the ecfg
          ecfg.Save(configPath);
          // Clear the Parameters
          EcmParameters.Clear();
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
            else if ((ecmPT & EcmParameterType.Z_Axis) != 0)
                ecmParameter.data_type = GetZ_Axis(map, parameters, ecmPT);
            else if ((ecmPT & EcmParameterType.Table) != 0)
                ecmParameter.data_type = GetTable(map, parameters, ecmPT);
            
            // Add the completed parameter
            parameters.Add(ecmParameter);
            // return the list as an array
            return parameters.ToArray();
        }
        private Table GetTable(Map map, List<EcmParameter> parameters, EcmParameterType pT)
        {
            // Setup the element type
            DataType elementType = SetupElement(map, pT);
            // Setup the table
            return new()
            {
                element_type = elementType,
                element_count = map.Rows,
            };
        }
        private DataType SetupElement(Map map, EcmParameterType pT)
        {
            DataType elementType;
            if ((pT & EcmParameterType.Floating_Point) != 0)
                elementType = GetFloating_Point(map);
            else
                elementType = GetFixed_Point(map);
            return elementType;
        }
        private Z_Axis GetZ_Axis(Map map, List<EcmParameter> parameters, EcmParameterType pT)
        {
            // Setup the Z element type
            DataType zElementType = SetupElement(map, pT);
            // Find the related X Axis Id
            string relatedXAxisId = GetX_AxisId(map, parameters);
            // Find the related Y Axis Id
            string relatedYAxisId = GetY_AxisId(map, parameters);
            // assemble the Z Axis
            return new()
            {
                z_element_type = zElementType,
                related_x_axis_id = relatedXAxisId,
                related_y_axis_id = relatedYAxisId,
            };
        }
        private Y_Axis GetY_Axis(Map map, List<EcmParameter> parameters, EcmParameterType pT)
        {
            // Setup the Y axis element datatype
            DataType yElementType = SetupElement(map, pT);
            // Find the related X Axis Id.
            string relatedXAxisId = GetX_AxisId(map, parameters);
            // Assemble the y axis
            return new()
            {
                y_element_type = yElementType,
                element_count = map.Columns,
                related_x_axis_id = relatedXAxisId,
            };
        }
        private string GetY_AxisId(Map map, List<EcmParameter> parameters)
        {
            // Get the name of the y axis
            string name = map.AxisYIdName;
            // Search and see if this axis exists already.
            if (EcmParameters.Any(p => p.name == name))
                return EcmParameters.Where(p => p.name == name).First().id;
            // Setup variables to calculate address offsets
            int baseAddress = map.FieldvaluesStartAddr.HexToInt();
            int yAddress = (int)(map.AxisYDataAddr.HexToInt() + 0x80020000);
            int offset = yAddress - baseAddress;
            int yItnId = NegativeCounter;
            // Does this Itn have the parameter assigned to it already?
            if (Contents.Any(c => c.Id == map.IdName.HexToInt()))
            {
                Itn itn = Contents.Where(c => c.Id == map.IdName.HexToInt()).First();
                Itn yItn;
                //yAddress = itn.AbsoluteAddress + offset;
                if (Contents.Any(c => c.AbsoluteAddress == yAddress))
                {
                    yItn = Contents.Where((c) => c.AbsoluteAddress == yAddress).First();
                    if (yItn.HasParameter())
                        return yItn.Id.ToString();
                    yItnId = yItn.Id;
                }
                else if (Contents.Any(c => c.AbsoluteAddress == yAddress - 2 && c.ByteCount > 2))
                {
                    yItn = Contents.Where(c => c.AbsoluteAddress == yAddress - 2).First();
                    if (yItn.HasParameter())
                        return yItn.Id.ToString();
                    yItnId = yItn.Id;
                }
            }
            // setup the y axis element datatype
            DataType y_element_type;
            if (GetDataOrg(map.AxisYDataOrg) == EcmParameterType.Floating_Point)
                y_element_type = GetFloating_Point(map.AxisYUnit, map.AxisYDataOrg);
            else
                y_element_type = GetFixed_Point(map.AxisYUnit, map.AxisYDataOrg, map.AxisYbSigned, map.AxisYFactor);
            // Setup the Y axis
            EcmParameter y_Axis = new()
            {
              name = map.AxisYIdName,
              id = yItnId.ToString(),
              description = map.AxisYName,
              group_ids = "",
              data_type = new Y_Axis()
              {
                y_element_type = y_element_type,
                element_count = map.Rows,
              }
            };
            parameters.Add(y_Axis);
            return yItnId.ToString();
        }
        private string GetX_AxisId(Map map, List<EcmParameter> parameters)
        {
            // Get the name of the x axis
            string name = map.AxisXIdName;
            // Search and see if this axis exists already.  
            if (EcmParameters.Any(p => p.name == name))
                return EcmParameters.Where(p => p.name == name).First().id;
            // Setup variables to calculate address offsets
            int baseAddress = map.FieldvaluesStartAddr.HexToInt();
            int xAddress = (int)(map.AxisXDataAddr.HexToInt() + 0x80020000);
            int offset = xAddress - baseAddress;
            int xItnId = NegativeCounter;
            // Does this Itn have the parameter assigned to it already?
            if (Contents.Any(c => c.Id == map.IdName.HexToInt()))
            {
                Itn itn = Contents.Where(c => c.Id == map.IdName.HexToInt()).First();
                Itn xItn;
                //xAddress = itn.AbsoluteAddress + offset;
                if (Contents.Any(c => c.AbsoluteAddress == xAddress))
                {
                    xItn = Contents.Where((c) => c.AbsoluteAddress == xAddress).First();
                    if (xItn.HasParameter())
                        return xItn.Id.ToString();
                    xItnId = xItn.Id;
                }
                else if (Contents.Any(c => c.AbsoluteAddress == xAddress - 2 && c.ByteCount > 2))
                {
                    xItn = Contents.Where((c) => c.AbsoluteAddress == xAddress - 2).First();
                    if (xItn.HasParameter())
                        return xItn.Id.ToString();
                    xItnId = xItn.Id;
                }
            }
            // Setup the x axis element datatype
            DataType x_element_type;
            if (GetDataOrg(map.AxisXDataOrg) == EcmParameterType.Floating_Point)
                x_element_type = GetFloating_Point(map.AxisXUnit, map.AxisXDataOrg);
            else
                x_element_type = GetFixed_Point(map.AxisXUnit, map.AxisXDataOrg, map.AxisXbSigned, map.AxisXFactor);
            // Setup the X axis
            EcmParameter x_Axis = new()
            {
                name = map.AxisXIdName,
                id = xItnId.ToString(),
                description = map.AxisXName,
                group_ids = "",
                data_type = new X_Axis()
                {
                    x_element_type = x_element_type,
                    element_count = map.Columns
                }
            };
            // Add the x- axis to the parameters list
            // For the time being if I can't index the location of 
            // the itn, I won't add it.  The hope is that perhaps
            // some of the axes used multiple times, eventually one
            // will be right.
            parameters.Add(x_Axis);
            return xItnId.ToString();
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
                data_length = GetDataLength(dataLength),
                sign = _sign == "1" ? "S" : "U",
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
