using CumminsEcmEditor.IntelHex;
using CumminsEcmEditor.Tools;
using CumminsEcmEditor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CumminsEcmEditor.Cummins
{
  public class BlockDataStructure
  {
    // Table
    // 60 Bytes.

    // 3A 00        = 0x003A      - Size of entire table, not including this field.  Always = 58 bytes 
    // 00 01        = 0x0001      - messageSize??? unknown purpose
    // 04 00        = 0x0004      - Number of blocks in this calibration

    // 04 00        = 0x0004      - byte size of each data point.  04 is four bytes, or 32bit.
    // 00 00 02 80  = 0x80020000  - block1 start Address
    // 00 00 3F 80  = 0x803F0000  - block2 start Address
    // 00 07 00 A8  = 0xA8000700  - block3 start Address
    // 00 09 02 A8  = 0xA8020900  - block4 start Address  

    // 04 00        = 0x0004      - byte size of each data point. 04 is four bytes, or 32 bit.
    // 72 07 33 00  = 0x00330772  - ???
    // 00 00 01 00  = 0x00010000  - ???
    // 80 94 00 00  = 0x00009480  - ???
    // 00 10 00 00  = 0x00001000  - ???

    // 04 00        = 0x0004      - byte size of each data point. 04 is four bytes, or 32 bit.
    // 72 07 33 00  = 0x00330772  - block1.length
    // 0C BF 00 00  = 0x0000BF0C  - block2.length
    // 80 94 00 00  = 0x00009480  - block3.length
    // 00 10 00 00  = 0x00001000  - block4.length

    private Calibration XCal {get; set;} 
    private byte[][] Data { get; set; }

    public BlockDataStructure(Calibration xCal, byte[] blockDataStructure) 
    {
      XCal = xCal;
      byte[] b = blockDataStructure;

      Data = new byte[][] 
      {
        new byte[]{ b[00], b[01] },               // 00 Size of entire table, not including this field.  Always = 58 bytes
        new byte[]{ b[02], b[03] },               // 01 messageSize??? unknown purpose
        new byte[]{ b[04], b[05] },               // 02 Number of blocks in this calibration

        new byte[]{ b[06], b[07] },               // 03 byte size of each data point.  04 is four bytes, or 32bit.
        new byte[]{ b[08], b[09], b[10], b[11] }, // 04 block0 start Address
        new byte[]{ b[12], b[13], b[14], b[15] }, // 05 block1 start Address
        new byte[]{ b[16], b[17], b[18], b[19] }, // 06 block2 start Address
        new byte[]{ b[20], b[21], b[22], b[23] }, // 07 block3 start Address

        new byte[]{ b[24], b[25] },               // 08 byte size of each data point.  04 is four bytes, or 32bit.
        new byte[]{ b[26], b[27], b[28], b[29] }, // 09 ???
        new byte[]{ b[30], b[31], b[32], b[33] }, // 10 ???
        new byte[]{ b[34], b[35], b[36], b[37] }, // 11 ???
        new byte[]{ b[38], b[39], b[40], b[41] }, // 12 ???
        
        new byte[]{ b[42], b[43] },               // 13 byte size of each data point.  04 is four bytes, or 32bit.
        new byte[]{ b[44], b[45], b[46], b[47] }, // 14 block0.Length
        new byte[]{ b[48], b[49], b[50], b[51] }, // 15 block1.Length
        new byte[]{ b[52], b[53], b[54], b[55] }, // 16 block2.Length
        new byte[]{ b[56], b[57], b[58], b[59] }, // 17 block3.Length
      };
    } 
      
    public int GetBlockStartAddress(int blockIndex) => Data[4 + blockIndex].ToInt(XCal.GetByteOrder());
    public int GetBlockLength(int blockIndex) => Data[14 + blockIndex].ToInt(XCal.GetByteOrder());
  }
}