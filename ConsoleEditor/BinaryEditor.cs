using System;
using CumminsEcmEditor.Binary;
using CumminsEcmEditor.IntelHex;

namespace ConsoleEditor
{
  public static class BinaryEditor
  {
    public static void Run(string[] args)
    {
      Console.WriteLine("Now in BinaryEditor\n");

      string loadBinPath = args[0];
      string saveXcalPath = args[1];

      Console.WriteLine($"loading from : {loadBinPath}");
      Console.WriteLine($"saving to    : {saveXcalPath}");

      if (File.Exists(loadBinPath))
      {
        byte[] ecmBin = File.ReadAllBytes(loadBinPath);

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


        EcmBlockData[] blockDataStructure = new EcmBlockData[]
        {
          new EcmBlockData(0x80020000, 0x00330772),
          new EcmBlockData(0x803F0000, 0x0000BF0C),
          new EcmBlockData(0xA8000700, 0x00009480),
          new EcmBlockData(0xA8020900, 0x00001000)
        };




        // take byte array and ecmblockdata
        // 

        List<Record> records = new();

        int blockIndex = 0;
        uint blockPosition = 0; 
        uint ela = 0x80020000;
        uint address = 0;

        bool newPass = true;
        EcmBlockData block = blockDataStructure[blockIndex] ;

        for (int b = 0; b < ecmBin.Length; b++)
        {
          if (newPass)

            


          {
            block = blockDataStructure[blockIndex];
            Console.WriteLine($"Starting Block @ {block.StartAddress.ToString("x8")}");
            Console.WriteLine($"  Length: {block.Length}");
            ela = (int)block.StartAddress;
            ela = ela >> 16;
            // Bytes to make up the extended linear address record
            byte[] elaBytes = new byte[]
            {
              (byte)(ela >> 8),
              (byte)(ela & 0xFF)
            };
            ela = ela << 16;
            address = (int)block.StartAddress - ela;
            newPass = false;
            blockPosition = 0;



            // New pass, needs to make a new extended linear address

            Record elaRecord = new(ela, address, 0x04, elaBytes);

          }

          // Do the work before updating the block position,
          // address, etc.















          // Update positions
          address++;
          blockPosition++;


          // turn over ela
          if (address == 0xFFFF)
          {
            ela += 0x10000;
            address = 0;

            Console.WriteLine(ela.ToString("X8"));

          }
          if (blockPosition == block.Length)
          {
            Console.WriteLine($"  > {(ela + address).ToString("x8")}");
            blockIndex++;
            newPass = true;
          }
        }
      }
      else
      {
        Console.WriteLine("Something went wrong");
      }
    }
  }



  public class EcmBlockData
  {
    public uint StartAddress { get; set; }
    public uint Length { get; set; }

    public EcmBlockData(uint startAddress, uint length)
    {
      StartAddress = startAddress;
      Length = length;
    }
  }
}

