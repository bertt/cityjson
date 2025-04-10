# CityJSON

.NET 6.0 Library for reading CityJSON files (https://www.cityjson.org/)

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

Convert CityJSONDocument to NetTopologySuite:

```
var features = cityjsonDocument.ToFeatures(vertices, transform);
```

Converting CityJSON object to NetTopologySuite:

```
var feature = cityObject.ToFeature(vertices, transform);
```

## Tools

- cj2pg: from CityJSON to PostGIS

- cj2glb: from CityJSON to GLB

## Known limitations

- Geometry type support: Solid, CompositeSurface, MultiSurface, MultiSolid, CompositeSolid;

- Support for interrior rings;

- Appearance support: Textures, Vertices-Texture; 

- No support for metadata, semantics, extensions, materials;

- No support for writing CityJSON files;

## History

2025-01-15: release 2.1 - Reading CityJSON 2.0 Seq files

2024-12-06: release 2.0 - Refactoring - Reading geometries

2024-12-04: release 1.1 - to .NET 8.0

2022-11-22: from CityJSON 1.0.1 to CityJSON 1.1.2

2022-11-22: to .NET 6

2020-07-27: original coding
