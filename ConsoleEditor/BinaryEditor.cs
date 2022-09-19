using System;
using CumminsEcmEditor.Binary;

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
        EcmBin ecmBin = new(File.ReadAllBytes(loadBinPath));



      }


    }
  }



  public class EcmBlockData
  {




  }
}

