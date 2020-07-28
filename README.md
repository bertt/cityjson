# CityJSON

Library for reading/writing CityJSON (https://www.cityjson.org/)

# Sample code

Parsing CityJSON file:

```
var json = File.ReadAllText("fixtures/denhaag.json");
var cityjson = JsonConvert.DeserializeObject<CityJSONRoot>(json);
Assert.IsTrue(cityjson.Version == "1.0");
var expected = new List<double>() { 78248.66, 457604.591, 2.463, 79036.024, 458276.439, 37.481 };
Assert.IsTrue(cityjson.Metadata.GeographicalExtent.Length== expected.Length);
```

# Schema generation

Schema used: https://3d.bk.tudelft.nl/schemas/cityjson/1.0.1/cityjson.min.schema.json

Classes in CityJSON.Schema.cs are generated from https://app.quicktype.io?share=OD2oWcD4ShAGpJx1tjLi






