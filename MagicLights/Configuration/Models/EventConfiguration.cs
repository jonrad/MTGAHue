using Newtonsoft.Json;

namespace MagicLights.Configuration.Models
{
    [JsonObject]
    public class EventConfiguration
    {
        public string? Id { get; set; }

        public EffectConfiguration? Effect { get; set; }
    }
}
