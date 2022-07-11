// See https://aka.ms/new-console-template for more information
using CumminsEcmEditor.Cummins;
using CumminsEcmEditor.IntelHex;
using CumminsEcmEditor.Tools.Extensions;
using CumminsEcmEditor.WinOLS;

// open the calibration
Calibration xCal = new(@"Y:\WinOLS\237019FU5C.XCAL");

MapPack mapPack = new(@"Y:\WinOLS\IsvCsvMapPack.json");

xCal.TableOfContents.ConvertMapPackToConfiguration(@"Y:\WinOLS\IsvCsvMapPack.json", @"Y:\WinOLS\bcxV2.ecfg");


// xCal.SaveModdedCalibration(true);
// xCal.Document.Save();