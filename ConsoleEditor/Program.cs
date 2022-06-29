// See https://aka.ms/new-console-template for more information
using CumminsEcmEditor.IntelHex;
using CumminsEcmEditor.Tools.Extensions;


Calibration xCal = new(@"Y:\WinOLS\Short.XCAL");

byte[][] bbb = xCal.Cursor.Read("80020000".HexToInt(), 32,5);
outputArray(bbb);
Console.WriteLine();


byte[] testArray = new byte[] { 0xFF, 0x17, 0x01, 0xFF };

xCal.Cursor.Write("8002002C".HexToInt(), testArray);

bbb = xCal.Cursor.Read("80020000".HexToInt(), 32, 5);

outputArray(bbb);
Console.WriteLine();

xCal.Cursor.Write("8002002C".HexToInt(), GenerateTestData(8));

bbb = xCal.Cursor.Read("80020000".HexToInt(), 32, 5);

outputArray(bbb);
Console.WriteLine();



void outputArray(byte[][] bbb)
{
    for (int i = 0; i < bbb.Length; i++)
    {
        string output = "";
        foreach (byte b in bbb[i])
            output += $"{b.ByteToHex()} ";
        Console.WriteLine(output);
    }
}

byte[][] GenerateTestData(int elements)
{
    byte[] seventeenOhOne = new byte[] { 0xFF, 0x17, 0x01, 0xFF };

    byte[][] result = new byte[elements][];
    for (int e = 0; e < elements; e++)
            result[e] = seventeenOhOne.ToArray();

    return result;
}

xCal.SaveModdedCalibration(true);