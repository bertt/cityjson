using Newtonsoft.Json;
using System.Numerics;

namespace CityJSON
{
    public partial class Transform
    {

        [JsonProperty("scale", Required = Required.Always)]
        public double[] Scale { get; set; }

        [JsonProperty("translate", Required = Required.Always)]
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
