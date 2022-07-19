using CumminsEcmEditor.Tools.Extensions;

namespace CumminsEcmEditor.IntelHex
{
    public class Cursor
    {
        #region Private Properties
        private Record[] Records { get; set; }
        private Record CurrentRecord => Records[IndexPosition];
        private int Address { get; set; }
        private int IndexPosition { get; set; }
        #endregion

        #region Constructor
        public Cursor(Record[] records)
        {
            Records = records;
            IndexPosition = Array.IndexOf(Records, Records.GetFirstRecord());
            Address = CurrentRecord.GetAbsoluteStartAddress();
        }
        #endregion

        #region Public Read Methods
        public byte Read(int absoluteAddress)
        {
            if (SetCursorPosition(absoluteAddress))
                return Read();
            return 0xFF;
        }
        public byte[] Read(int absoluteAddress, int totalBytes)
        {
            if (!SetCursorPosition(absoluteAddress))
                return null;

            byte[] result = new byte[totalBytes];
            for (int t = 0; t < totalBytes; t++)
            {
                    result[t] = Read();
            }
            return result;
        }
        public byte[][] Read(int absoluteAddress, int bytesPerElement, int totalElements)
        {
            if (!SetCursorPosition(absoluteAddress))
                return null;
            byte[][] result = new byte[totalElements][];
            for (int t = 0; t < totalElements; t++)
            {
                result[t] = new byte[bytesPerElement];
                for (int e = 0; e < bytesPerElement; e++)
                    result[t][e] = Read();
            }
            return result;
        }
        #endregion

        #region Public Write Methods
        public void Write(int absoluteAddress, byte b)
        {
            if (SetCursorPosition(absoluteAddress))
                Write(b);
        }
        public void Write(int absoluteAddress, byte[] bb)
        {
            if (!SetCursorPosition(absoluteAddress))
                return;
            foreach (byte b in bb)
                Write(b);
        }
        public void Write(int absoluteAddress, byte[][] bbb)
        {
            if (!SetCursorPosition(absoluteAddress))
                return;
            foreach (byte[] bb in bbb)
                foreach (byte b in bb)
                    Write(b);
        }
        #endregion

        #region Private Methods
        private byte Read()
        {
            byte b = (byte)CurrentRecord.GetDataByte(Address);
            AdvanceAddressOneByte();
            return b;
        }   
        private void Write(byte b)
        {
            CurrentRecord.SetDataByte(Address, b);
            AdvanceAddressOneByte();
        }
        private bool SetCursorPosition(int absoluteAddress)
        {
            try
            {
                Record r = Records.Where(r => r.HasAbsoluteAddress(absoluteAddress)).First();
                IndexPosition = Array.IndexOf(Records, r);
                Address = absoluteAddress;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private void AdvanceAddressOneByte()
        {
            Address++;
            if (CurrentRecord.HasAbsoluteAddress(Address))
                return;

            while (!CurrentRecord.HasAbsoluteAddress(Address))
            {
                IndexPosition++;
                if (CurrentRecord.GetRecordType() == RecordType.EndOfFile)
                {
                    IndexPosition = 0;
                    Address = CurrentRecord.GetAbsoluteStartAddress();
                }
            }
        }
        #endregion
    }
}
