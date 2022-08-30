using CumminsEcmEditor.Cummins;
using CumminsEcmEditor.IntelHex;
using CumminsEcmEditor.Tools;

namespace CumminsEcmEditor.Binary
{
  public class EcmBin 
  {
    #region Private Properties
    private byte[] EcmBinary { get; set; }
    private Calibration XCalOfBin { get; set; }
    #endregion

    #region Properties
    public BlockDataStructure DataStructure { get; set; }
    #endregion

    public EcmBin(string binPath, BlockDataStructure dataStructure)
    {
      EcmBinary = EcmFiles.LoadBinary(binPath);
      DataStructure = dataStructure;
    }
    //public Calibration GetCalibration() 
    //{
    //  if (EcmBinary.Length != DataStructure.GetTotalBytes())
    //    throw new Exception("Not good bro");
    //  List<Record> records = new();
    //  int absPos = 0;
    //  int Pos = DataStructure.GetBlockStartAddress(0);

    //  return new Calibration();

    //}
  }
}