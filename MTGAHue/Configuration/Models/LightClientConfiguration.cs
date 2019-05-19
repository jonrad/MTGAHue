using Newtonsoft.Json;

namespace MTGAHue.Configuration.Models
{
    [JsonObject]
    public class LightClientConfiguration
    {
        public string? Id { get; set; }

        public bool Enabled { get; set; }

        public KeyValueConfiguration[]? Config { get; set; }

        public EventConfiguration[]? Events { get; set; }
    }
}
