using Newtonsoft.Json;

namespace CityJSON
{
    public partial class Transform
    {

        [JsonProperty("scale", Required = Required.Always)]
        public double[] Scale { get; set; }

        [JsonProperty("translate", Required = Required.Always)]
        public double[] Translate { get; set; }
    }
}
