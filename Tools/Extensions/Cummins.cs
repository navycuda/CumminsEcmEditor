using CumminsEcmEditor.Cummins;
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
            b = b.FillArray(byteOrder);
            if (byteOrder == XCalByteOrder.LittleEndian)
            {
              if (b.Length == 1)
                return BitConverter.ToUInt32(b.Reverse().ToArray());
            }
            return BitConverter.ToUInt32(b);
        }
        private static byte[] FillArray(this byte[] b, XCalByteOrder byteOrder)
        {
          byte[] result = new byte[4];
          if (byteOrder == XCalByteOrder.LittleEndian)
          {
            
            int bPos = 0;
            for (int r = 0; r < 4; r++)
            {
              if (bPos < b.Length)
              {
                result[r] = b[bPos];
                bPos ++;
              }
              else
              {
                result[r] = 0x00;
              }
            }
          }
          return result;
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
        public static string ToFixedPoint(this byte[] b, XCalByteOrder byteOrder, Fixed_Point xP){
          if (b.Length != xP.GetDataLength())
            return "err_b.len";
          if (xP.engr_units == "HEX")
            return b.ToHex(byteOrder);
          else if (xP.sign == "S")
            return (b.ToInt(byteOrder) * xP.GetScalarMultiplier()).ToString();
          else if (xP.sign == "U")
            return (b.ToUInt(byteOrder) * xP.GetScalarMultiplier()).ToString();
          return "err_noCap";
        }
        public static string ToPaddedString(this string input, int length)
        {
            string output = "";
            for (int i = 0; i < length; i++)
            {
                if (i < input.Length)
                    output += input[i];
                else
                    output += ' ';
            }
            return output;
        }
        public static string ToDocumentSafe(this string input)
        {
            int length = input.Length;
            string result = "";

            foreach (char c in input)
            {
                if (char.IsControl(c) ||
                    c == '\n' ||
                    c == '\r' ||
                    c == '\t')
                    continue;
                result += c;
            }
            return result;
        }
    }
}
