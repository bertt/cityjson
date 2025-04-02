using CommandLine;
using ConsoleApp1;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using Npgsql;

var parser = new Parser(settings =>
{
    settings.CaseInsensitiveEnumValues = true;
    settings.HelpWriter = Console.Error;
});

parser.ParseArguments<Options>(args).WithParsed(o =>
{
    var inputFolder = o.InputFolder;
    var table = o.Table;
    var geometryColumn = o.GeometryColumn;
    var attributesColumn = o.AttributesColumn;
    var connString = o.ConnectionString;
    var targetSrs = o.TargetSrs;

    Console.WriteLine("Connection string: " + connString);
    Console.WriteLine("Table: " + table);
    Console.WriteLine("Geometry column: " + geometryColumn);
    Console.WriteLine("Attributes column: " + attributesColumn);
    Console.WriteLine("Input folder: " + inputFolder);
    Console.WriteLine("Target srs: " + targetSrs);
    Console.WriteLine("Start processing");

    if(!Directory.Exists(inputFolder))
    {
        Console.WriteLine("Input folder does not exist");
        return;
    }

    var allFiles = Directory.GetFiles(inputFolder, "*.jsonl", SearchOption.AllDirectories);

    Console.WriteLine("Found " + allFiles.Length + " files");

    var wktWriter = new WKTWriter();
    wktWriter.OutputOrdinates = Ordinates.XYZ;

    using var conn = new NpgsqlConnection(connString);

    conn.Open();

    foreach (var file in allFiles)
    {
        var features = TileReader.ReadCityJsonSeqTile(file, "2.2");
        foreach (var feature in features)
        {
            var wkt = wktWriter.Write(feature.Geometry);
            var attributes = feature.Attributes;
            var json = JsonConvert.SerializeObject(attributes);

            var sql = @$"
                INSERT INTO {table} ({geometryColumn}, {attributesColumn}) 
                VALUES (ST_SetSRID(ST_GeomFromText(@wkt), @targetSrs), @json);";

            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("wkt", wkt);
                cmd.Parameters.AddWithValue("targetSrs", targetSrs);
                cmd.Parameters.Add("json", NpgsqlTypes.NpgsqlDbType.Json).Value = json;

                // Voer de query uit
                cmd.ExecuteNonQuery();
            }

            Console.Write(".");
        }
    }

    conn.Close();

    Console.WriteLine();
    Console.WriteLine("End of process");
});


