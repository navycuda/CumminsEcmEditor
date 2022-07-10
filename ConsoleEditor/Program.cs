// See https://aka.ms/new-console-template for more information
using CumminsEcmEditor.IntelHex;
using CumminsEcmEditor.Tools.Extensions;

// open the calibration
Calibration xCal = new(@"Y:\WinOLS\237019FU5C.XCAL");




// xCal.SaveModdedCalibration(true);
xCal.Document.Save();