using CumminsEcmEditor.Cummins;
using CumminsEcmEditor.Tools.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CumminsEcmEditor.WinOLS
{
    public static class WinOLSConverter
    {
        public static EcmParameter? ToEcmParameter(this Map map)
        {
            // Create new parameter and fill out the basic details
            EcmParameter ecmParameter = new()
            {
                name = map.Name,
                id = map.IdName.HexToInt().ToString(),
                description = map.FieldvaluesName,
            };
            // Classify this map
            EcmParameterType ecmPT = map.Classify();
            // Select creation method from the ecmParameterType
            if (ecmPT == EcmParameterType.None)
                return null;
            // Floating Point Value
            if (ecmPT == EcmParameterType.Floating_Point)
            {
                ElementDetails eD = new(map.FieldvaluesUnit,"4");
                ecmParameter.data_type = GetFloating_Point(eD);
            }
            // Fixed_Point Value
            else if (ecmPT == EcmParameterType.Fixed_Point)
            {
                ElementDetails eD = new(
                    map.FieldvaluesUnit,
                    GetDataLength(map),
                    map.bSigned == "1" ? "S" : "U",
                    map.FieldvaluesFactor
                    );
                ecmParameter.data_type = GetFixed_Point(eD);
            }
            // Y_Axis
            return ecmParameter;
        }

        public static ConfigurationFile ToConfigurationFile(this MapPack mapPack)
        {
            // Prepare an empty configuration file
            ConfigurationFile ecfg = new();
            // Prepare an empty list to assemble parameters into
            List<EcmParameter?> ecmParameters = new List<EcmParameter>();
            // Iterate through the maps and convert them into EcmParameters
            foreach (Map map in mapPack.maps)
                ecmParameters.Add(map.GetEcmParameter());
            // Add the ecmParameters to the ecfg
            ecfg.Parameters = ecmParameters.ToArray();
            
            return ecfg;
        }

        #region Private Methods
        private static DataType GetDataTypeByFlag(ElementDetails elementDetails, EcmParameterType ecmParameterType)
        {
            if ((ecmParameterType & EcmParameterType.Floating_Point) != 0)
                return GetFloating_Point(elementDetails);
            return GetFixed_Point(elementDetails);
        }
        private static Floating_Point GetFloating_Point(ElementDetails elementDetails) =>
            new()
            {
                engr_units = elementDetails.engr_units,
                data_length = elementDetails.data_length,
            };
        private static Fixed_Point GetFixed_Point(ElementDetails elementDetails) =>
            new()
            {
                engr_units = elementDetails.engr_units,
                data_length = elementDetails.data_length,
                sign = elementDetails.sign,
                scalar_multiplier = elementDetails.scalar_multiplier,
            };
        private static string GetDataLength(Map map)
        {
            if (map.DataOrg == "eLoHiLoHi")
                return "4";
            else if (map.DataOrg == "eLoHi")
                return "2";
            return "1";
        }
        #endregion

        #region EcmParameter Type Methods
        private static EcmParameterType Classify(this Map map)
        {
            // Add the correct type flag for this map.
            EcmParameterType pT = GetDataOrg(map.DataOrg);
            // Add the correct parameter flag for this map.
            return map.AddParameterType(pT);
        }
        private static EcmParameterType AddParameterType(this Map map, EcmParameterType pT)
        {
            int rows = map.GetRows();
            int columns = map.GetColumns();
            // Single Value Parameter, ie a toggle
            if (map.Type == "eEinzel" && (rows * columns) == 1)
                return pT;
            // Y_Axis - single dimension(in winOLS) but has an x-axis
            if (map.Type == "eEindim" && rows == 1 && columns > 1)
                return pT | EcmParameterType.Y_Axis;
            // Z_Axis - two dimensions(in winOLS) has x,y axis
            if (map.Type == "eZweidim" && rows > 1 && columns > 1)
                return pT | EcmParameterType.Z_Axis;
            // Table - one dimension has no axis data
            bool isTable = map.AxisXIdName == "" && map.AxisYIdName == "";
            if (map.Type == "eZweidim" && rows > 1 && columns == 1 && isTable)
                return pT | EcmParameterType.Table;
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
        #endregion
    }
    [Flags]
    public enum EcmParameterType
    {
        None                = 0,
        Floating_Point      = 0b1,
        Fixed_Point         = 0b10,
        X_Axis              = 0b100,
        Y_Axis              = 0b1000,
        Z_Axis              = 0b10000,
        Table               = 0b100000
    }
    public struct ElementDetails
    {
        public string engr_units { get; set; }
        public string data_length { get; set; }
        public string sign { get; set; }
        public string scalar_multiplier { get; set; }

        public ElementDetails(
            string units,
            string length,
            string _sign = "",
            string _scalar = ""
            )
        {
            engr_units = units;
            data_length = length;
            sign = _sign;
            scalar_multiplier = _scalar;
        }
    }
    public struct AxisDetails
    {

    }
}
