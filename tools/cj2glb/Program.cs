// See https://aka.ms/new-console-template for more information
using CityJSON;
using cj2glb;
using Newtonsoft.Json;
using System.CommandLine;

var inputFile = new Argument<string>("input CityJSON file");
var outputFile = new Argument<string>("Output glTF 2.0 file");

var rootCommand = new RootCommand("CLI tool for converting from CityJSON to glTF 2.0 (GLB)")
{
    inputFile, outputFile
};

rootCommand.SetHandler(RunAsync, inputFile, outputFile);

await rootCommand.InvokeAsync(args);

async Task RunAsync(string inputFile, string outputFile)
{
    Console.WriteLine("cj2glb 0.1");
    Console.WriteLine("Input file: " + inputFile);
    Console.WriteLine("Output file: " + outputFile);

    var json = File.ReadAllText(inputFile);
    var cityjsonDocument = JsonConvert.DeserializeObject<CityJsonDocument>(json);

    // check if there are textures
    var hasTextures = cityjsonDocument.Appearance != null && cityjsonDocument.Appearance.Textures != null && cityjsonDocument.Appearance.Textures.Count > 0;

    var bytes = hasTextures?
        TexturedGltfCreator.ToGltf(cityjsonDocument):
        GltfCreator.ToGltf(cityjsonDocument);
    File.WriteAllBytes(outputFile, bytes);

    Console.WriteLine("Program finished." + bytes.Length);
}

