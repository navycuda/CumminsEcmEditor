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

        #region Methods

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
        private string GetDocumentLine() =>
            GetPaddedName() + GetPaddedValue() + GetPaddedUnits() + GetComment();
        private string GetPaddedName() =>
            $"  {GetName()}".ToPaddedString(42);
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

            return output.ToPaddedString(9);
        }
        private string GetComment() =>
            HasParameter() ? Parameter.description.ToDocumentSafe() : GetUnlistedComment();
        private string GetUnlistedComment() =>
            $"Unlisted Engine Parameter - {GetByteCount()}";
        private string GetUnitsFromDataType()
        {
            DataType dT = Parameter.data_type;
            if (dT is Fixed_Point fP)
                return fP.engr_units;
            else if (dT is Fixed_Point xP)
                return xP.engr_units;
            return "";
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
            List<string> result = new List<string>()
            {
                $"",
                $"  X: {xAxis.GetName()} ({xAxis.GetUnitsFromDataType()}) - {xAxis.GetComment()}",
                $"  Y: {GetName()} ({GetUnitsFromDataType()}) - {GetComment()}",
                $"  " + "X".PadLeft(8) + " " + "Y".PadLeft(8),
            };


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
