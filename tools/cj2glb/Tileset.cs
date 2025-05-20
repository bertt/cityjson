namespace cj2glb;

public class Rootobject
{
    public Asset Asset { get; set; }
    public Root Root { get; set; }
}

public class Asset
{
    public string Version { get; set; }
}

public class Root
{
    public float[] Transform { get; set; }
    public Boundingvolume BoundingVolume { get; set; }
    public Content Content { get; set; }
}

public class Boundingvolume
{
    public float[] Region { get; set; }
}

public class Content
{
    public string uri { get; set; }
}
