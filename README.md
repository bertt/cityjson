# CityJSON

Library for reading/writing CityJSON (https://www.cityjson.org/)

NuGet package: https://www.nuget.org/packages/bertt.CityJSON/

## Sample code

Reading CityJSON file:

```
var cityjson = CityJsonRoot.FromJson(json);
Assert.IsTrue(cityjson.Version == "1.0");
var expected = new List<double>() { 78248.66, 457604.591, 2.463, 79036.024, 458276.439, 37.481 };
Assert.IsTrue(cityjson.Metadata.GeographicalExtent.Length== expected.Length);
```

Writing CityJSON file:

```
var cityJsonString = cityjson.ToJson();
```


## Schema generation

Schema used: https://3d.bk.tudelft.nl/schemas/cityjson/1.1.2/cityjson.min.schema.json

Classes in CityJSON.Schema.cs are generated from https://app.quicktype.io/?share=OD2oWcD4ShAGpJx1tjLi

## History

2024-12-04: release 1.1 - to .NET 8.0

2022-11-22: from CityJSON 1.0.1 to CityJSON 1.1.2

2022-11-22: to .NET 6

2020-07-27: original coding







