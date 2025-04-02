using CommandLine;

namespace ConsoleApp1;
public class Options
{
    [Option('c', Required = true, HelpText = "Database connection string")]
    public required string ConnectionString { get; set; }

    [Option('t', "table", Required = true, HelpText = "output table")]
    public required string Table { get; set; }

    [Option("geometrycolumn", Required = false, Default = "geom", HelpText = "Geometry column")]
    public required string GeometryColumn { get; set; }

    [Option("attributescolumn", Required = false, Default = "attributes", HelpText = "Attributes column")]
    public required string AttributesColumn { get; set; }

    [Option('i', "input", Required = false, HelpText = "Input folder CityJSON files", Default = ".")]
    public required string InputFolder{ get; set; }

    [Option("t_srs", Required = false, HelpText = "Target srs", Default = 5698)]
    public required int TargetSrs { get; set; }



}
