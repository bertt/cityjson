using CityJSON;
using cj2glb;
using Newtonsoft.Json;
using System.CommandLine;

var inputFile = new Argument<string>("input CityJSON file");
var outputFile = new Argument<string>("Output glTF 2.0 file");
var idOption = new Option<string>(
    name: "--id",
    description: "Optional ID of the CityObject to convert"
);

var rootCommand = new RootCommand("CLI tool for converting from CityJSON to glTF 2.0 (GLB)")
{
    inputFile, outputFile,   idOption
};

rootCommand.SetHandler(RunAsync, inputFile, outputFile, idOption);

await rootCommand.InvokeAsync(args);

async Task RunAsync(string inputFile, string outputFile, string? id)
{
    Console.WriteLine("cj2glb");
    Console.WriteLine("Input file: " + inputFile);
    Console.WriteLine("Output file: " + outputFile);

    if (id != null)
    {
        Console.WriteLine("CityObject ID: " + id);
    }

    var json = File.ReadAllText(inputFile);
    var cityjsonDocument = JsonConvert.DeserializeObject<CityJsonDocument>(json);

    // check if there are textures
    var hasTextures = cityjsonDocument.Appearance != null && cityjsonDocument.Appearance.Textures != null && cityjsonDocument.Appearance.Textures.Count > 0;
    
    var fullInputPath = Path.GetFullPath(inputFile);
    var texturesBaseDirectory = Path.GetDirectoryName(fullInputPath);

    var bytes = hasTextures?
        TexturedGltfCreator.ToGltf(cityjsonDocument, texturesBaseDirectory, id) :
        GltfCreator.ToGltf(cityjsonDocument, id);
    File.WriteAllBytes(outputFile, bytes);

    Console.WriteLine("Program finished." + bytes.Length);
}

