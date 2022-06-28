// See https://aka.ms/new-console-template for more information
using CumminsEcmEditor.IntelHex;

Console.WriteLine("Hello, World!");

Calibration xCal = new(@"Y:\WinOLS\Short.XCAL");

xCal.SaveAsMod();