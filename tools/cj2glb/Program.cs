using CityJSON;
using cj2glb;
using Newtonsoft.Json;
using System.CommandLine;
using System.Numerics;

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

    var geographicalExtent = cityjsonDocument.Metadata.GeographicalExtent;

    Console.WriteLine("CRS:" + cityjsonDocument.Metadata.ReferenceSystem);  

    if (geographicalExtent != null)
    {

        var geographicalExtentString = string.Join(", ", geographicalExtent);
        Console.WriteLine($"Geographical Extent: {geographicalExtentString}");
        var center_x = geographicalExtent[0] + (geographicalExtent[3] - geographicalExtent[0]) / 2;
        var center_y = geographicalExtent[1] + (geographicalExtent[4] - geographicalExtent[1]) / 2;
        var center_z = geographicalExtent[2] + (geographicalExtent[5] - geographicalExtent[2]) / 2;

        var center = new Vector3((float)center_x, (float)center_y, (float)center_z);
        Console.WriteLine("Center: " + center.ToString("F4"));
    }


    // check if there are textures
    var hasTextures = cityjsonDocument.Appearance != null && cityjsonDocument.Appearance.Textures != null && cityjsonDocument.Appearance.Textures.Count > 0;
    
    var fullInputPath = Path.GetFullPath(inputFile);
    var texturesBaseDirectory = Path.GetDirectoryName(fullInputPath);

    var bytes = hasTextures?
        TexturedGltfCreator.ToGltf(cityjsonDocument, texturesBaseDirectory, id) :
        GltfCreator.ToGltf(cityjsonDocument, id);
    File.WriteAllBytes(outputFile, bytes);

    Console.WriteLine("GLB file created: " + outputFile);
    Console.WriteLine("GLB file size: " + new FileInfo(outputFile).Length + " bytes");
    Console.WriteLine("Program finished.");
}

