namespace CumminsEcmEditor.IntelHex
{
    public enum XCalRecordType
    {
        Data = 0x00,
        EndOfFile = 0x01,
        ExtendedSegmentAddress = 0x02,
        StarSegmentAddress = 0x03,
        ExtendedLinearAddress = 0x04
    }
}
