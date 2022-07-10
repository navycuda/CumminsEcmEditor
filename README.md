<h1>CumminsEcmEditor</h1>
<h3> An experimental editor for the Cummins ecm calibrations. </h3>

<h4>Introduction:</h4>

This project orginally began as it became clear how locked behind a paywall the diesel tuning community is.I have a unique Cummins Turbo Diesel engine that I want to tune myself.  The available tools are missing
parameters for the engine, or do not match up correctly to the part number of my ecm.

So... the solution... through months of research, agonizing over fine details, writing and re-writing...
is rewriting again!

<h4>Purpose:</h4>

To correctly load, modify and save the Engine Calibration to a new file that can be installed with either
an orginal Cummins tool, a Dealership tool (ie Nissan Consult III), or an aftermarket tool (ie Ez Lynk).



<h4>ToDo:</h4>
Get Engine Documentation Working
Import my previous ecfg library file and use that
	Eventually have to work on a new library compiler, but for now
	the previous ecfg I made will work with the model I copy/pasted!
Convert WinOLS map to Cummins Engine Parameter
	This is a tough one and I'm avoiding it as long as I can.


<h4>ToDo:</h4>

Xcal checksum calculator

<h4>Questions/Thougths:</h4>

Does the device installing the bin care if the data is stored as little endian or not?  Is the byte
order relevant to the device or does it just install the bits in the order they're presented in the
xCal?