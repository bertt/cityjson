# CityJSON

.NET 8.0 Library for reading CityJSON files (https://www.cityjson.org/)

NuGet package: https://www.nuget.org/packages/bertt.CityJSON/

## Dependencies

- NetTopologySuite

## Sample code

Reading CityJSON file:

```
var json = File.ReadAllText("fixtures/minimal.city.json");
var cityjsonDocument = JsonConvert.DeserializeObject<CityJsonDocument>(json);
```

Sample reading CityJSON 2.0 Seq file and converting to NetTopologySuite:

```
var cityJsondocuments = CityJsonSeqReader.ReadCityJsonSeq("./fixtures/cityjsonseq/paris_tower.city.jsonl");
```

Converting CityJSON object to NetTopologySuite:

```
var features = cityObject.ToFeatures(vertices, transform);
```

## Known limitations

- Geometry type support: Solid, CompositeSurface, MultiSurface, MultiSolid, CompositeSolid 

- No support for metadata, semantics, extensions, textures, materials

- No support for writing CityJSON files

## History

2024-01-15: release 2.1 - Reading CityJSON 2.0 Seq files

2024-12-06: release 2.0 - Refactoring - Reading geometries

2024-12-04: release 1.1 - to .NET 8.0

2022-11-22: from CityJSON 1.0.1 to CityJSON 1.1.2

2022-11-22: to .NET 6

2020-07-27: original coding