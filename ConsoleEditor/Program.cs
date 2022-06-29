// See https://aka.ms/new-console-template for more information
using CumminsEcmEditor.IntelHex;
using CumminsEcmEditor.Tools.Extensions;


Calibration xCal = new(@"Y:\WinOLS\Short.XCAL");

for (int i = 0; i <128; i++)
{
    string loopValue = "";
    if (i > 0 && i % 32 == 0)
        loopValue += "\n";
    loopValue += $"{xCal.Cursor.Read().ByteToHex()} ";
    Console.Write(loopValue);
}






//xCal.SaveModdedCalibration(true);


