using Npgsql;

// Sample for reading a set of CityJSON Seq files and inserting the geometries into a PostGIS database

var path = @"D:\aaa\barcelonnette\tile_00001.city";

var allFiles = Directory.GetFiles(path, "*.jsonl", SearchOption.AllDirectories);

var connString = "Host=localhost;Username=postgres;Password=postgres;Database=postgres;Port=5439";

using var conn = new NpgsqlConnection(connString);

conn.Open();


foreach (var file in allFiles)
{
    var wkts = TileReader.ReadCityJsonSeqTile(file, "2.2");
    foreach(var wkt in wkts)
    {
        var sql = @$"insert into public.roofer(geom)values(st_setsrid(st_GeomfromText('{wkt}'), 5698));";
        var cmd = new NpgsqlCommand(sql, conn);
        cmd.ExecuteNonQuery();

        Console.WriteLine(wkt);
    }
}

conn.Close();
