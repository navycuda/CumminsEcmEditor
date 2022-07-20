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
        public bool IsModified { get; set; }
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
        public EcmParameterType GetEcmParameterType()
        {
          DataType dT = Parameter.data_type;
          if (!HasParameter())
            return EcmParameterType.None;
          EcmParameterType result = GetElementType(dT, EcmParameterType.None);
          if (result != EcmParameterType.None)
            return result;
          if (dT is Z_Axis z)
            return GetElementType(z.z_element_type, EcmParameterType.Z_Axis);
          else if (dT is Y_Axis y)
            return GetElementType(y.y_element_type, EcmParameterType.Y_Axis);
          else if (dT is X_Axis x)
            return GetElementType(x.x_element_type, EcmParameterType.X_Axis);
          else if (dT is Table t)
            return GetElementType(t.element_type, EcmParameterType.Table);
          return result;
        }
        private EcmParameterType GetElementType(DataType dT, EcmParameterType pT)
        {
          EcmParameterType result = EcmParameterType.None;
          if (dT is Floating_Point)
            result = EcmParameterType.Floating_Point;
          else if (dT is Fixed_Point)
            result = EcmParameterType.Fixed_Point;
          return result | pT;
        }
        public string GetHexId() =>
            $"0x{Id.IntToHex(4)}";
        public string GetHexAddress() =>
            $"0x{AbsoluteAddress.IntToHex(4)}";
        public string GetByteCount() =>
            $"{ByteCount}B";
        public bool HasParameter() =>
            Parameter != null;
        public bool IsSingleValue() =>
            HasParameter() ? (Parameter.data_type is Floating_Point ||
                              Parameter.data_type is Fixed_Point) :
                              ByteCount <= 4;
        public string GetName() =>
            HasParameter() ? Parameter.name : $"Unk_ITN{GetHexId()}";
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
        public string GetSingleValue() {
          byte[] singleValue = ToC.GetData(AbsoluteAddress, ByteCount);
          if (!HasParameter())
            return singleValue.ByteToHex();
          if (Parameter.data_type is Floating_Point fP)
            return singleValue.ToFloat(ToC.ByteOrder).ToString("0.000");
          else if (Parameter.data_type is Fixed_Point xP)
            return singleValue.ToFixedPoint(ToC.ByteOrder, xP);
          return "err_noGsv";
        }
        public string GetUnits()
        {
            string output = "...";

            if (HasParameter())
                if (IsSingleValue())
                    output = GetUnitsFromDataType();
                else
                    output = "array";

            return output;
        }
        public string GetComment() =>
            HasParameter() ? Parameter.description.ToDocumentSafe() : GetUnlistedComment();
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
        private string GetUnlistedComment() =>
            $"{GetByteCount().PadLeft(4)} Unlisted Parameter @ {this.GetHexAddress()}";
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
        #endregion
    }
}
