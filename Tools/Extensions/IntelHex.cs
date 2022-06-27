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
    }
}
