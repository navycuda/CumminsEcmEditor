// See https://aka.ms/new-console-template for more information
using CumminsEcmEditor.IntelHex;

Console.WriteLine("Hello, World!");

Calibration xCal = new(@"Y:\WinOLS\Short.XCAL");

xCal.SetValue("8002002C", 0xFF);
xCal.SetValue("80020040", 0xFF);
xCal.SetValue("8002005F", 0xFF);

xCal.SaveModdedCalibration(true);