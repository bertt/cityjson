using Newtonsoft.Json;

namespace CityJSON
{
    public partial class Transform
    {
        [JsonProperty("scale")]
        public double[] Scale { get; set; }

        [JsonProperty("translate")]
        public double[] Translate { get; set; }
    }
}
