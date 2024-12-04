using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

// schema sources:
// - https://3d.bk.tudelft.nl/schemas/cityjson/1.1.2/cityjson.min.schema.json
var json = File.ReadAllText("./1.1.2/cityjson.min.schema.json");

var schema = await JsonSchema.FromJsonAsync(json);
var settings = new CSharpGeneratorSettings() { JsonLibrary = CSharpJsonLibrary.SystemTextJson };
settings.Namespace = "CityJSON";
var generator = new CSharpGenerator(schema, settings);

var file = generator.GenerateFile();
//File.WriteAllText("../../../../src/GeoParquet.cs", file);
File.WriteAllText("../../../../cityjson.core/CityJSON.Schema.cs", file);
