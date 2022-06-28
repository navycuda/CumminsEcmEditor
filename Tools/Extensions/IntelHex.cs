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
        public static string ByteToHex(this byte b) =>
            b.ToString("X");
        public static string ByteToHex(this byte[] bA)
        {
            if (bA == null)
                return "";
            string output = "";
            foreach (byte b in bA)
                output += b.ByteToHex();
            return output;
        }
        public static string IntToHex(this int i) =>
            i.ToString("X2");
    }
}
