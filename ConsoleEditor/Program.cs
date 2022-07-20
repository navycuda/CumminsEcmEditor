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
// Run the help output.
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
// Extract all the addresses from intel hex
else if (args.Length == 1)
{
  checkExists(args[0]);
  getExtendedAddressList(args[0]);
}
// Standard method of opening, calibration plus the correct configuration.
else if (args.Length == 2) 
{
  Console.WriteLine($"Calibration Path  : {args[0]}");
  checkExists(args[0]);
  Console.WriteLine($"Configuration Path: {args[1]}");
  checkExists(args[1]);
  load(args[0], args[1]);
}
// Use case for converting a modified mappack to a configuration
else if (args.Length == 3)
{
  Console.WriteLine($"Calibration Path  : {args[0]}");
  checkExists(args[0]);
  Console.WriteLine($"MapPack Path      : {args[1]}");
  checkExists(args[1]);
  Console.WriteLine($"Config Out Path   : {args[2]}");
  if (File.Exists(args[2]))
    Console.WriteLine($"\toverwriting...");
  else
    Console.WriteLine($"\tcreating...");
  convert(args[0],args[1],args[2]);
}
// Something went wrong
else
{
  Console.WriteLine("Unable to Parse Arguments. help for help.");
  Console.WriteLine("exiting...");
  Environment.Exit(0);
}


// open the calibration
//Calibration xCal = new(args[0]);

//MapPack mapPack = new(@"Y:\WinOLS\IsvCsvMapPack.json");

//xCal.TableOfContents.ConvertMapPackToConfiguration(@"Y:\WinOLS\IsvCsvMapPack.json", @"Y:\WinOLS\bcxV2.ecfg");

// xCal.Document.Save();

void load(string xCalPath, string ecfgPath){
  // Load the calibration
  Calibration xCal = new(xCalPath);
  // Apply the configuration
  xCal.TableOfContents.ApplyConfiguration(ecfgPath);
  // If only we had something to do... you know, like an editor?
  // xCal.SaveModdedCalibration(true);
  // Oh OH!  for now can document the engine, that's, you know, important
  Console.WriteLine($"\tEngine document saved at {xCal.Document.Save()}");
}
void convert(string xCalPath, string mapPackPath, string configSavePath){
  // Load the xCal that goes with the mapPack.  needed to have correct
  // itn to match itns to parameters.
  Calibration xCal = new(xCalPath);
  // Load and convert the map pack to ecfg
  xCal.TableOfContents.ConvertMapPackToConfiguration(mapPackPath, configSavePath);
  // Commentary of some kind?  Other tasks after creation needed?
  Console.WriteLine($"Configuration saved at {configSavePath}");
}
void getExtendedAddressList(string xCalPath) {
  // Load the Calibration
  Calibration xCal = new(xCalPath);
  // Save the extended Addresses to a list.
  xCal.SaveExtendedAddressList();
}
void checkExists(string filePath)
{
  if (File.Exists(filePath))
    return;
  Console.WriteLine($"\tNot Found : {filePath}");
  Console.WriteLine($"\texiting...");
  Environment.Exit(0);
}