// See https://aka.ms/new-console-template for more information
using CumminsEcmEditor.Cummins;
using CumminsEcmEditor.IntelHex;
using CumminsEcmEditor.Tools.Extensions;
using CumminsEcmEditor.WinOLS;
using System;



if(args.Length == 0)
{
  Console.WriteLine("No arguments passed, help for help");
  Console.WriteLine("exiting...");
  Environment.Exit(0);
}
else if (args.Length == 1 && args[0] == "help")
{
  string[] helpResponse = new string[]
  {
    "CumminsEcmEditor - ConsoleEditor",
    "--------------------------------",
    "",
    "Usage: CumminsEcmEditor <Calibration Path> <Configuration Path>",
    "\tStandard usage case.  Calibration and Configuration to read/write",
    "\tthe calibration.",
    "",
    "Usage: CumminsEcmEditor <calibration path> <mappack json path> <generated configuration path>",
    "\tThis use case is only intended to take a mappack that has been exported from WinOLS",
    "\tto json, that json cleaned of formatting issues (ie FieldValues.Name to FieldValuesName)",
    "\tand convert it the correct Configuration and Parameter type, saving the result to a new",
    "\t.ecfg to the generated configuration path.  Note, overwrite without asking is the default."
  };
  foreach (string s in helpResponse)
    Console.WriteLine(s);
  Environment.Exit(0);
}
else if (args.Length == 2) 
{
  Console.WriteLine($"Calibration Path  : {args[0]}");
  checkExists(args[0]);
  Console.WriteLine($"Configuration Path: {args[1]}");
  checkExists(args[1]);
}
else if (args.Length == 3)
{
  Console.WriteLine($"Calibration Path  : {args[0]}");
  checkExists(args[0]);
  Console.WriteLine($"MapPack Path      : {args[1]}");
  checkExists(args[1]);
  Console.WriteLine($"Config Out Path   : {args[2]}");
  if (File.Exists(args[2]))
    Console.WriteLine($"\tconfig out exists... overwriting");
}
else
{
  Console.WriteLine("Unable to Parse Arguments. help for help.");
  Console.WriteLine("exiting...");
  Environment.Exit(0);
}


// open the calibration
//Calibration xCal = new(@"Y:\WinOLS\WinOLS.xcal");

//MapPack mapPack = new(@"Y:\WinOLS\IsvCsvMapPack.json");

//xCal.TableOfContents.ConvertMapPackToConfiguration(@"Y:\WinOLS\IsvCsvMapPack.json", @"Y:\WinOLS\bcxV2.ecfg");

// xCal.SaveModdedCalibration(true);
// xCal.Document.Save();

void checkExists(string filePath)
{
  if (File.Exists(filePath))
    return;
  Console.WriteLine($"\tNot Found : {filePath}");
  Console.WriteLine($"\texiting...");
  Environment.Exit(0);
}