using CumminsEcmEditor.Cummins;
using CumminsEcmEditor.IntelHex;
using CumminsEcmEditor.Tools;

namespace CumminsEcmEditor.Binary
{
  public class EcmBin 
  {
    #region Properties
    private byte[] EcmBinary { get; set; }
    private BlockDataStructure DataStructure { get; set; }
    private Record[] Records { get; set; }
    #endregion

    public EcmBin(string binPath, BlockDataStructure dataStructure)
    {
      EcmBinary = EcmFiles.LoadBinary(binPath);
      DataStructure = dataStructure;
    }
    public Calibration GetCalibration() 
    {
      if (EcmBinary.Length != DataStructure.GetTotalBytes())
        throw new Exception("Not good bro");
      List<Record> records = new();
      int absPos = 0;
      int Pos = DataStructure.GetBlockStartAddress(0);
      




    }
  }
}