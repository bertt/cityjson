# cityjson2pg

Dotnet 8 global console tool for reading CityJSON files (https://www.cityjson.org/)
and converting to PostGIS

## Parameters

```
 -c                  Required. Database connection string

  -t, --table         Required. output table

  --geometrycolumn    (Default: geom) Geometry column

  --attributescolumn  (Default: attributes) Attributes column

  -i, --input         (Default: .) Input folder CityJSON files

  --t_srs             (Default: 5698) Target srs

  --help              Display this help screen.

  --version           Display version information.
```

## Sample

```
cityjson2pg -c "Host=localhost;Username=postgres;Password=postgres;Database=postgres;Port=5432" -t public.my_table_ -i ./output/tiles
```

Sample output table:

```psql
CREATE TABLE public.my_table_ (
    id SERIAL PRIMARY KEY,
    geom geometry,
    attributes json
);

```


### Installation

.NET 8.0 SDK is required

```
git clone https://github.com/bertt/cityjson
cd samples/cityjson2pg
dotnet pack --output ./nupkg
dotnet tool install --global --add-source ./nupkg cityjson2pg
cityjson2pg --version
cityjson2pg 0.1.5
```

Or update:

```
dotnet tool update --global --add-source ./nupkg cityjson2pg
```