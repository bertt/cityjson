using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using System;
using System.Threading.Tasks;
using System.IO;

namespace cityjson.schema.generate
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var version = "1.0.1";
            var schemaPart = "cityjson.min";

            var schema = await JsonSchema.FromFileAsync($"schemas/{version}/{schemaPart}.schema.json");
            var settings = new CSharpGeneratorSettings()
            {
                ClassStyle = CSharpClassStyle.Poco,
                HandleReferences = true,
                Namespace = "CityJSON.Core"
            };

            var generator = new CSharpGenerator(schema, settings);
            var file = generator.GenerateFile();
            File.WriteAllText($"../../../../cityjson.core/{schemaPart}.cs", file);
        }
    }
}
