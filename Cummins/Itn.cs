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

        #region Constructor
        public Itn(int id, int absoluteAddress, int byteCount)
        {
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
        #endregion
    }
}
