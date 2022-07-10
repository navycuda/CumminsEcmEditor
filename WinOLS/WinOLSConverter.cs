using CumminsEcmEditor.Cummins;

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
                id = map.IdName,
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
                ecmParameter.data_type = CreateParameterDataType<Floating_Point>(eD);
            }
            if (ecmPT == EcmParameterType.Fixed_Point)



            return ecmParameter;
        }

        #region Private Methods
        /// <summary>
        /// Creates a new Single Value EcmParameter
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="bytes">byte count of datatype</param>
        /// <returns>EcmParameter</returns>
        private static DataType CreateParameterDataType<T>(ElementDetails elementDetails) where T : DataType
        {
            if (typeof(T) == typeof(Floating_Point))
            {

            }
            if (typeof(T) == typeof(Fixed_Point))
            {

            }

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
            // Y_Axis - single dimension
            if (map.Type == "eEindim" && rows == 1 && columns > 1)
                return pT | EcmParameterType.Y_Axis;
            // Z_Axis - two dimensions
            if (map.Type == "eZweidim" && rows > 1 && columns > 1)
                return pT | EcmParameterType.Z_Axis;
            // Table - one dimension
            bool isTable = map.AxisXIdName == "" && map.AxisYIdName == "";
            if (map.Type == "eZweidim" && rows > 1 && columns == 1 && isTable)
                return pT | EcmParameterType.Table;
            return EcmParameterType.None;
        }
        private static EcmParameterType GetDataOrg(string dataOrg)
        {
            if (dataOrg == "eLoHiLoHi")
                return EcmParameterType.Floating_Point;
            else if (dataOrg == "eLoHiLoHi")
                return EcmParameterType.Fixed_Point;
            else if (dataOrg == "eLoHi")
                return EcmParameterType.Fixed_Point;
            else if (dataOrg == "eByte")
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
