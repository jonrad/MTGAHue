using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MTGAHue.Configuration.Models
{
    [JsonObject]
    public class LightClientConfiguration
    {
        public string? Id { get; set; }

        public bool Enabled { get; set; }

        public JObject? Config { get; set; }

        public EventConfiguration[]? Events { get; set; }
    }
}
