// See https://aka.ms/new-console-template for more information
using CumminsEcmEditor.Cummins;
using CumminsEcmEditor.IntelHex;
using CumminsEcmEditor.Tools.Extensions;
using CumminsEcmEditor.WinOLS;

// open the calibration
Calibration xCal = new(@"Y:\WinOLS\237019FU5C.XCAL");

MapPack mapPack = new(@"Y:\WinOLS\IsvCsvMapPack.json");

ConfigurationFile ecfg = mapPack.ToConfigurationFile();

ecfg.Save(@"Y:\WinOLS\bcx.ecfg");

// xCal.SaveModdedCalibration(true);
// xCal.Document.Save();