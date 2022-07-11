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
            ElementDetails eD = new(
                map.FieldvaluesUnit,
                GetDataLength(map),
                map.bSigned == "1" ? "S" : "U",
                map.FieldvaluesFactor
                );
            // Floating Point Value
            if (ecmPT == EcmParameterType.Floating_Point)
            {
                ecmParameter.data_type = GetFloating_Point(eD);
            }
            // Fixed_Point Value
            if (ecmPT == EcmParameterType.Fixed_Point)
            {
                ecmParameter.data_type = GetFixed_Point(eD);
            }
            // Y_Axis
            else if ((ecmPT & EcmParameterType.Y_Axis) != 0)
            {
                ecmParameter.data_type = GetY_Axis(map, ecmPT);
            }
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
        private static Y_Axis GetY_Axis(Map map, EcmParameterType ecmParameterType)
        {

            // Create the new Y axis
            Y_Axis result = new();

            return new();
        } 
        private static string GetDataLength(Map map)
        {
            if (map.DataOrg == "eLoHiLoHi" || map.DataOrg == "eFloatLoHi")
                return "4";
            else if (map.DataOrg == "eLoHi")
                return "2";
            return "1";
        }
        #endregion

        #region EcmParameter Type Methods
        #endregion
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
