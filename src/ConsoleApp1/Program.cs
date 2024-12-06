using Npgsql;

// Sample for reading a set of CityJSON Seq files and inserting the geometries into a PostGIS database

var path = "D:\\gisdata\\3dbag\\france\\output\\tiles";

var allFiles = Directory.GetFiles(path, "*.jsonl", SearchOption.AllDirectories);

var connString = "Host=localhost;Username=postgres;Password=postgres;Database=postgres";

using var conn = new NpgsqlConnection(connString);

conn.Open();


foreach (var file in allFiles)
{
    var wkts = TileReader.ReadCityJsonSeqTile(file);
    foreach(var wkt in wkts)
    {
        var sql = @$"insert into public.roofer(geom)values(st_setsrid(st_GeomfromText('{wkt}'), 5698));";
        var cmd = new NpgsqlCommand(sql, conn);
        cmd.ExecuteNonQuery();

        Console.WriteLine(wkt);
    }
}

conn.Close();
