using CumminsEcmEditor.Tools.Extensions;

namespace CumminsEcmEditor.Cummins
{
    public class Itn
    {
        #region Properties
        public int Id { get; set; }
        public int AbsoluteAddress { get; set; }
        public int ByteCount { get; set; }
        public EcmParameter? Parameter { get; set; }
        #endregion

        #region Private Properties
        private ItnTableOfContents ToC { get; set; }
        private bool IsModified { get; set; }
        #endregion

        #region Constructor
        public Itn(ItnTableOfContents toC, int id, int absoluteAddress, int byteCount)
        {
            ToC = toC;
            Id = id;
            AbsoluteAddress = absoluteAddress;
            ByteCount = byteCount;
        }
        #endregion

        #region Get Methods
        public string GetHexId() =>
            $"0x{Id.IntToHex(4)}";
        public string GetHexAddress() =>
            $"0x{Id.IntToHex(AbsoluteAddress)}";
        public string GetByteCount() =>
            $"{ByteCount}B";
        public bool HasParameter() =>
            Parameter != null;
        public bool IsSingleValue() =>
            HasParameter() ? (Parameter.data_type is Floating_Point ||
                              Parameter.data_type is Fixed_Point) :
                              ByteCount <= 4;
        public string GetName() =>
            HasParameter() ? Parameter.name : $"UL_ITN_{GetHexId()}";
        public byte[][] GetRawData()
        {
            if (HasParameter())
            {
                //  how many bytes wide the parameter is
                int bytes = GetElementBytes();
                // Setup the element count and address offset
                bool hasCount;
                int elements = GetElementCount(out hasCount);
                int absoluteAddress = hasCount ? AbsoluteAddress + 2 : AbsoluteAddress;
                // Get the Data and return it
                return ToC.GetData(absoluteAddress, bytes, elements);
            }
            return new byte[][] { ToC.GetData(AbsoluteAddress, ByteCount) }; 
        }
        #endregion

        #region Public Document Methods
        public string[] GetAsDocumented()
        {
            if (IsSingleValue())
                return new[] { GetDocumentLine() };
            return GetAsTable();
        }
        public string[] GetAsDeepDocumented()
        {
            if (IsSingleValue())
                return new[] { GetDeepDocumentLine() };
            return GetAsTable();
        }
        #endregion

        #region Private Document Methods
        private bool HasCorrectElementCount(out int rawElements, out bool hasCount)
        {
            // calculate the number of elements based off itn length
            // and data type byte count.  if no greater than +2, good
            // to go.  +2 is for a certain table type I haven't figured
            // out yet.  It preceeds the table data, and gives a count
            // of the number of contained elements.
            int elementBytes = GetElementBytes();
            rawElements = ByteCount / elementBytes;
            int remainder = ByteCount % rawElements;
            hasCount = remainder == 2;

            if (remainder == 0)
                return true;
            if (hasCount)
                return true;
            return false;
        }
        private int GetElementCount(out bool hasCount)
        {
            DataType dT = Parameter.data_type;
            int elements = 1;
            hasCount = false;
            if (HasCorrectElementCount(out elements, out hasCount))
                return elements;
            return 0;

        }
        private int GetElementBytes()
        {
            DataType dT = Parameter.data_type;
            if (dT is Floating_Point)
                return 4;
            if (dT is Fixed_Point xP)
                return xP.GetDataLength();
            if (dT is X_Axis xA)
                if (xA.x_element_type is Floating_Point xFP)
                    return 4;
                else if (xA.x_element_type is Fixed_Point xXP)
                    return xXP.GetDataLength();
            if (dT is Y_Axis yA)
                if (yA.y_element_type is Floating_Point yFP)
                    return 4;
                else if (yA.y_element_type is Fixed_Point yXP)
                    return yXP.GetDataLength();
            if (dT is Z_Axis zA)
                if (zA.z_element_type is Floating_Point zFP)
                    return 4;
                else if (zA.z_element_type is Fixed_Point zXP)
                    return zXP.GetDataLength();
            if (dT is Table t)
                if (t.element_type is Floating_Point tFP)
                    return 4;
                else if (t.element_type is Fixed_Point tXP)
                    return tXP.GetDataLength();
            return 0;
        }
        // every field has a ' ' to seperate
        // 1, 48, 12, 10 plus forced space.

        // values always 12 chars, tables or otherwise
        private string GetDocumentLine() =>
            $"{GetParameterMarker()} {GetPaddedName()} {GetPaddedValue()} {GetPaddedUnits()} ITN{GetHexId()} {GetComment()}";
        private string GetPaddedName() =>
            $"{GetName()}".ToPaddedString(48);
        private string GetPaddedValue() =>
            $"...".ToPaddedString(12);
        private string GetPaddedUnits()
        {
            string output = "...";

            if (HasParameter())
                if (IsSingleValue())
                    output = GetUnitsFromDataType();
                else
                    output = "array";

            return output.ToPaddedString(10);
        }
        private char GetParameterMarker()
        {
            if (IsModified)
                return '~';
            return ' ';
        }
        private string GetComment() =>
            HasParameter() ? Parameter.description.ToDocumentSafe() : GetUnlistedComment();
        private string GetUnlistedComment() =>
            $"{GetByteCount().PadLeft(4)} Unlisted Engine Parameter";
        private string GetUnitsFromDataType()
        {
            DataType dT = Parameter.data_type;
            if (dT is Floating_Point fP)
                return fP.engr_units;
            if (dT is Fixed_Point xP)
                return xP.engr_units;
            if (dT is X_Axis xA)
                if (xA.x_element_type is Floating_Point xFP)
                    return xFP.engr_units;
                else if (xA.x_element_type is Fixed_Point xXP)
                    return xXP.engr_units;
            if (dT is Y_Axis yA)
                if (yA.y_element_type is Floating_Point yFP)
                    return yFP.engr_units;
                else if (yA.y_element_type is Fixed_Point yXP)
                    return yXP.engr_units;
            if (dT is Z_Axis zA)
                if (zA.z_element_type is Floating_Point zFP)
                    return zFP.engr_units;
                else if (zA.z_element_type is Fixed_Point zXP)
                    return zXP.engr_units;
            if (dT is Table t)
                if (t.element_type is Floating_Point tFP)
                    return tFP.engr_units;
                else if (t.element_type is Fixed_Point tXP)
                    return tXP.engr_units;
            return "...";
        }
        private string[] GetAsTable()
        {
            DataType dT = Parameter.data_type;




            return new[] { "No table yet" };
        }
        private string[] GetAsYAxis()
        {
            // Assumes data_type is Y_axis
            Y_Axis yAxis = (Y_Axis)Parameter.data_type;
            Itn xAxis = ToC.GetItnById(yAxis.GetXAxisId());
            // Prepare the table header in the construction of the string list
            List<string> result = new List<string>()
            {
                $"",
                $"  X: {xAxis.GetName()} ({xAxis.GetUnitsFromDataType()}) - {xAxis.GetComment()}",
                $"  Y: {GetName()} ({GetUnitsFromDataType()}) - {GetComment()}",
                $"  " + "X".PadLeft(12) + " " + "Y".PadLeft(12),
            };
            bool xCount;
            bool yCount;
            if (xAxis.GetElementCount(out xCount) == GetElementCount(out yCount))



            return result.ToArray();
        }
        private string[] GetAsZAxis()
        {
            // Assumes data_type is Z_axis
            List<string> result = new List<string>()
            {

            };
        }
        #endregion

        #region Private Deep Document Methods
        private string GetDeepDocumentLine()
        {
            return "";
        }
        private string[] GetAsDeepTable()
        {
            return new[] { "No deep table yet" };
        }
        #endregion

    }
}
