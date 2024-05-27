# Ti84CSharp
WIP project to interpret programs written for the Ti-84+ graphing calculators.

(The goal is to be mostly able to run the previous programs I've written for the Ti-84+, not necessarily implement the entire language exactly.)

## TODO
- Almost all mathematical functions (`sin`)
- Support for smaller terminals
- Round-off errors involving small values
- Various problems with breaking expressions into tokens

## Running
Build the Ti84App project, and run `Ti84App.exe /path/to/program.txt`.

If no arguments are specified, it runs whatever program I was testing.

## Requirements
- .NET 8.0
- C# Language version 12