using LightsApi;
using MagicLights.Configuration.Serialization;
using Newtonsoft.Json;

namespace MagicLights.Configuration.Models
{
    [JsonObject]
    public class ColorsConfiguration
    {
        [JsonConverter(typeof(RgbConverter))]
        public RGB? Red { get; set; }

        [JsonConverter(typeof(RgbConverter))]
        public RGB? Green { get; set; }

        [JsonConverter(typeof(RgbConverter))]
        public RGB? Blue { get; set; }

        [JsonConverter(typeof(RgbConverter))]
        public RGB? Black { get; set; }

        [JsonConverter(typeof(RgbConverter))]
        public RGB? White { get; set; }
    }
}
