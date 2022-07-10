using CumminsEcmEditor.Cummins;
using CumminsEcmEditor.Tools.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CumminsEcmEditor.WinOLS
{
    [Serializable]
    public class Map
    {
        #region JsonProperties
        public string Name { get; set; }
        public string IdName { get; set; }
        public string FolderName { get; set; }
        public string Type { get; set; }
        public string ViewMode { get; set; }
        public string RWin { get; set; }
        public string DataOrg { get; set; }
        public string bReciprocal { get; set; }
        public string bSigned { get; set; }
        public string bDelta { get; set; }
        public string bPercent { get; set; }
        public string bOriginal { get; set; }
        public string bOriginalValues { get; set; }
        public string Columns { get; set; }
        public string Rows { get; set; }
        public string Radix { get; set; }
        public string Comment { get; set; }
        public string Precision { get; set; }
        public string SkipBytes { get; set; }
        public string LineSkipBytes { get; set; }
        public string FieldvaluesName { get; set; }
        public string FieldvaluesUnit { get; set; }
        public string FieldvaluesFactor { get; set; }
        public string FieldvaluesOffset { get; set; }
        public string FieldvaluesStartAddr { get; set; }
        public string AxisXName { get; set; }
        public string AxisXIdName { get; set; }
        public string AxisXUnit { get; set; }
        public string AxisXFactor { get; set; }
        public string AxisXOffset { get; set; }
        public string AxisXRadix { get; set; }
        public string AxisXbBackwards { get; set; }
        public string AxisXbReciprocal { get; set; }
        public string AxisXbSigned { get; set; }
        public string AxisXPrecision { get; set; }
        public string AxisXDataSrc { get; set; }
        public string AxisXDataHeader { get; set; }
        public string AxisXDataAddr { get; set; }
        public string AxisXDataOrg { get; set; }
        public string AxisXSignatureByte { get; set; }
        public string AxisXSkipBytes { get; set; }
        public string AxisYName { get; set; }
        public string AxisYIdName { get; set; }
        public string AxisYUnit { get; set; }
        public string AxisYFactor { get; set; }
        public string AxisYOffset { get; set; }
        public string AxisYRadix { get; set; }
        public string AxisYbBackwards { get; set; }
        public string AxisYbReciprocal { get; set; }
        public string AxisYbSigned { get; set; }
        public string AxisYPrecision { get; set; }
        public string AxisYDataSrc { get; set; }
        public string AxisYDataHeader { get; set; }
        public string AxisYDataAddr { get; set; }
        public string AxisYDataOrg { get; set; }
        public string AxisYSignatureByte { get; set; }
        public string AxisYSkipBytes { get; set; }
        #endregion

        public string GetName() =>
            Name;
        public int GetId() =>
            IdName.HexToInt();
        

        public DataType GetDataType()
        {

            return new();
        }
        public int GetColumns()
        {
            int columns;
            if (int.TryParse(Columns, out columns))
                return columns;
            return 0;
        }
        public int GetRows()
        {
            int rows;
            if (int.TryParse(Rows, out rows))
                return rows;
            return 0;
        }
        private float GetScalarMultiplier()
        {
            float output;
            if (float.TryParse(FieldvaluesFactor, out output))
                return output;
            return 1.0f;
        }
        private X_Axis GetAsXAxis()
        {

            return new();
        }
        private Y_Axis GetAsYAxis()
        {
            return new();
        }
        private Z_Axis GetAsZAxis()
        {
            return new();
        }
        private Table GetAsTable()
        {
            return new();
        }
    }
}