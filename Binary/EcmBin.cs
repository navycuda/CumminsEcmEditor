using CumminsEcmEditor.Cummins;
using CumminsEcmEditor.IntelHex;
using CumminsEcmEditor.Tools;

namespace CumminsEcmEditor.Binary
{
  public class EcmBin 
  {
    #region Private Properties
    private byte[] EcmBinary { get; set; }

    #endregion

    #region Properties
    #endregion

    public EcmBin(byte[] ecmBinary)
    {
      EcmBinary = ecmBinary;
    }






  }
}