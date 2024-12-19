# CityJSON

.NET 8.0 Library for reading CityJSON files (https://www.cityjson.org/)

NuGet package: https://www.nuget.org/packages/bertt.CityJSON/

## Dependencies

- NetTopologySuite

## Sample code

Reading CityJSON file:

```
var json = File.ReadAllText("fixtures/minimal.city.json");
var cityjson = JsonConvert.DeserializeObject<CityJsonDocument>(json);
Assert.IsTrue(cityjson.Version == "1.0");
```

Sample reading CityJSON 2.0 Seq file and converting to NetTopologySuite:

```
var jsonSeq = File.ReadAllText("fixtures/tile_00000.city.jsonl");

var allLines = jsonSeq.Split('\n');

var firstLine = allLines[0];
var cityJson = JsonConvert.DeserializeObject<CityJsonDocument>(firstLine);
Assert.That(cityJson.Type == "CityJSON");
Assert.That(cityJson.Version == "2.0");

var transform = cityJson.Transform;
            
var secondLine = allLines[1];
var cityJsonSecond = JsonConvert.DeserializeObject<CityJsonDocument>(secondLine);
Assert.That(cityJsonSecond.CityObjects.Count == 2);
var feature = cityJsonSecond.ToFeature(transform);

// read with NetTopologySuite
var geom = feature.Geometry;
Assert.That(geom.GeometryType == "MultiPolygon");
```

## Kown limitations

- Geometry type support: Solid, CompositeSurface, MultiSurface, MultiSolid, CompositeSolid 

- No support for metadata, semantics, extensions, textures, materials

- No support for writing CityJSON files

## History

2024-12-06: release 2.0 - Refactoring - Reading geometries

2024-12-04: release 1.1 - to .NET 8.0

2022-11-22: from CityJSON 1.0.1 to CityJSON 1.1.2

2022-11-22: to .NET 6

2020-07-27: original coding