// See https://aka.ms/new-console-template for more information
using CityJSON;
using Newtonsoft.Json;
using System.CommandLine;
using CityJSON.Extensions;
using Triangulate;
using Wkx;
using cj2gltf;

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
    Console.WriteLine("Input file: " + inputFile);
    Console.WriteLine("Output file: " + outputFile);

    var json = File.ReadAllText(inputFile);
    var cityjsonDocument = JsonConvert.DeserializeObject<CityJsonDocument>(json);
    var transform = cityjsonDocument!.Transform;
    var feature = cityjsonDocument.ToFeature(transform);
    var wkt = feature.Geometry.AsText();
    var g = (MultiPolygon)Geometry.Deserialize<WktSerializer>(wkt);
    var wkb = g.AsBinary();
    var wkbTriangulated = Triangulator.Triangulate(wkb);
    var triangulatedGeometry = (MultiPolygon)Geometry.Deserialize<WkbSerializer>(wkbTriangulated);

    GltfCreator.CreateGltf(outputFile, triangulatedGeometry.Geometries);

    Console.WriteLine("Program finished.");

}


