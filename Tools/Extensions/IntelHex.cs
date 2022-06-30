using CumminsEcmEditor.IntelHex;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CumminsEcmEditor.Tools.Extensions
{
    public static class IntelHex
    {
        public static int HexToInt(this string hex)
        {
            if (hex.Length < 2)
                return -1;
            if (hex[..2] == "0x")
                return Convert.ToInt32(hex, 16);
            return Convert.ToInt32($"0x{hex}", 16);
        }
        public static byte HexToByte(this string hex) =>
            (byte)HexToInt(hex);
        public static byte[] HexToBytes(this string hex)
        {
            byte[] array = new byte[hex.Length / 2];

            for (int b = 0; b < array.Length; b++)
            {
                string s = "0x";
                for (int bb = 0; bb < 2; bb++)
                {
                    s += hex[(b * 2) + bb];
                }
                array[b] = (byte)Convert.ToInt32(s, 16);
            }
            return array;
        }
        public static string ByteToHex(this byte b) =>
            b.ToString("X2");
        public static string ByteToHex(this byte[] bA)
        {
            if (bA == null)
                return "";
            string output = "";
            foreach (byte b in bA)
                output += b.ByteToHex();
            return output;
        }
        public static string IntToHex(this int i,int bytes)
        {
            if (bytes == 1)
                return i.ToString("X2");
            else if (bytes == 2)
                return i.ToString("X4");
            return i.ToString("X8");
        }
        public static byte CheckSum(this string hex)
        {
            byte[] array = hex.HexToBytes();
            int sum = 0;
            foreach (byte b in array)
                sum += b & 0xFF;
            return (byte)-sum;
        }
        public static int GetStartingAbsoluteAddress(this Record[] r) =>
            r.GetFirstRecord().GetAbsoluteStartAddress();
        public static Record GetFirstRecord(this Record[] r) =>
            r.Where(r => r.GetRecordType() == RecordType.Data).First();
    }
}
