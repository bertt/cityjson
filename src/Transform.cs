using System.Text.Json;
using System.Numerics;
using System.Text.Json.Serialization;

namespace CityJSON
{
    public partial class Transform
    {

        [JsonPropertyName("scale")]
        [JsonRequired]
        public double[] Scale { get; set; }

        [JsonPropertyName("translate")]
        [JsonRequired]
        public double[] Translate { get; set; }

        public Vector3 TranslateVector3()
        {
            return new Vector3((float)Translate[0], (float)Translate[1], (float)Translate[2]);
        }

        public Vector3 ScaleVector3()
        {
            return new Vector3((float)Scale[0], (float)Scale[1], (float)Scale[2]);
        }
    }
}
