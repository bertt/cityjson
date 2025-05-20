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

    var geographicalExtent = cityjsonDocument.Metadata.GeographicalExtent;
    Console.WriteLine("CRS:" + cityjsonDocument.Metadata.ReferenceSystem);  

    if(geographicalExtent != null)
    {
        var geographicalExtentString = string.Join(", ", geographicalExtent);
        Console.WriteLine($"Geographical Extent: {geographicalExtentString}");
    }
    var translation = cityjsonDocument.Transform.Translate;
    Console.WriteLine("Translation: " + string.Join(", ", translation));    

       
    var translationWgs84 = CoordinateTransformer.TransformToWGS84(translation[0], translation[1], translation[2], cityjsonDocument.Metadata.ReferenceSystem);
    Console.WriteLine("Translation in WGS84: " + translationWgs84);

    var ext = cityjsonDocument.GetVerticesEnvelope();
    var env = ext.envelope;
    var minExtWgs94 = CoordinateTransformer.TransformToWGS84(env.MinX, env.MinY, ext.minZ, cityjsonDocument.Metadata.ReferenceSystem);
    var maxExtWgs94 = CoordinateTransformer.TransformToWGS84(env.MaxX, env.MaxY, ext.maxZ, cityjsonDocument.Metadata.ReferenceSystem);

    var region = new float[]
    {
        ToRadians((float)minExtWgs94.X),
        ToRadians((float) minExtWgs94.Y),
        ToRadians((float) maxExtWgs94.X),
        ToRadians((float) maxExtWgs94.Y),
        ext.minZ,
        ext.maxZ
    };
    Console.WriteLine("Region: " + string.Join(", ", region));

    var translationEcef = SpatialConvertor.GeodeticToEcef(translationWgs84[0], translationWgs84[1], ext.minZ);
    Console.WriteLine("Translation in ECEF: " + translationEcef);
    var matrix = SpatialConvertor.EcefToEnu(translationEcef);

    var hasTextures = cityjsonDocument.Appearance != null && cityjsonDocument.Appearance.Textures != null && cityjsonDocument.Appearance.Textures.Count > 0;
    
    var fullInputPath = Path.GetFullPath(inputFile);
    var texturesBaseDirectory = Path.GetDirectoryName(fullInputPath);

    var bytes = hasTextures?
        TexturedGltfCreator.ToGltf(cityjsonDocument, texturesBaseDirectory, id) :
        GltfCreator.ToGltf(cityjsonDocument, id);
    File.WriteAllBytes(outputFile, bytes);

    Console.WriteLine("GLB file created: " + outputFile);
    Console.WriteLine("GLB file size: " + new FileInfo(outputFile).Length + " bytes");

    var tilesetJson = new Rootobject();
    var asset = new Asset();
    asset.Version = "1.0";
    tilesetJson.Asset = asset;

    var transform = new float[]
    {
        matrix.M11, matrix.M12, matrix.M13, matrix.M14,
        matrix.M21, matrix.M22, matrix.M23, matrix.M24,
        matrix.M31, matrix.M32, matrix.M33, matrix.M34,
        matrix.M41, matrix.M42, matrix.M43, matrix.M44
    };

    var root = new Root();
    var boundingVolume = new Boundingvolume();  
    boundingVolume.Region = region;
    root.BoundingVolume = boundingVolume;

    root.Transform = transform;
    var content = new Content();
    content.uri = Path.GetFileName(outputFile);
    root.Content = content;
    tilesetJson.Root = root;

    var settings = new JsonSerializerSettings
        {
        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
        Formatting = Formatting.Indented
    };
    var tilesetJsonString = JsonConvert.SerializeObject(tilesetJson, settings);

    var path = Path.GetDirectoryName(outputFile);
    File.WriteAllText(path + Path.DirectorySeparatorChar + "tileset.json", tilesetJsonString);

    Console.WriteLine("Program finished.");
}

static float ToRadians(float degrees)
{
    return (float)(degrees * Math.PI / 180.0);
}