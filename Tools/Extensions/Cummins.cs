using CumminsEcmEditor.IntelHex;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CumminsEcmEditor.Tools.Extensions
{
    public static class Cummins
    {
        public static int ToInt(this byte[] b, XCalByteOrder byteOrder)
        {
            if (b.Length > 4)
                return -1;
            int result = 0;
            if (byteOrder == XCalByteOrder.LittleEndian)
            {
                for (int i = 0; i < b.Length; i++)
                    result += b[i] << (i * 8);
            }
            else
            {
                b = b.Reverse().ToArray();
                for (int i = 0; i < b.Length; i++)
                    result += b[i] << (i * 8);
            }

            return result;
        }
        public static uint ToUInt(this byte[] b, XCalByteOrder byteOrder)
        {
            if (b.Length > 4)
                return 0;
            if (byteOrder == XCalByteOrder.LittleEndian)
                return BitConverter.ToUInt32(b.Reverse().ToArray());
            return BitConverter.ToUInt32(b);
        }
        public static float ToFloat(this byte[] b, XCalByteOrder byteOrder)
        {
            if (b.Length != 4)
                return -1f;
            if (byteOrder == XCalByteOrder.LittleEndian)
                return BitConverter.ToSingle(b.Reverse().ToArray());
            return BitConverter.ToSingle(b);
        }
        public static string ToHex(this byte[] b, XCalByteOrder byteOrder)
        {
            if (b.Length > 4)
                return "overByte";
            if (byteOrder == XCalByteOrder.LittleEndian)
                return b.Reverse().ToArray().ByteToHex();
            return b.ByteToHex();
        }
    }
}
