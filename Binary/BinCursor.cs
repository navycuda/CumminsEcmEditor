namespace CumminsEcmEditor.Binary
{
  public class BinCursor
  {
    private EcmBin Binary { get; set; }
    private int Length { get; set; }
    private int AbsPos = 0;
    
    
    public BinCursor(EcmBin binary)
    {
      Binary = binary;
      Length = Binary.DataStructure.GetTotalBytes();
    }






  }
}