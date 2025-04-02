// See https://aka.ms/new-console-template for more information
using System.CommandLine;

var inputFile = new Argument<string>("input CityJSON file");
var outputFile = new Argument<string>("Output glTF 2.0 file");

var rootCommand = new RootCommand("CLI tool for converting from CityJSON to glTF 2.0")
{
    inputFile, outputFile
};

rootCommand.SetHandler(RunAsync, inputFile, outputFile);

await rootCommand.InvokeAsync(args);

async Task RunAsync(string inputFile, string outputFile)
{
    Console.WriteLine("cj2gltf 0.1");
}