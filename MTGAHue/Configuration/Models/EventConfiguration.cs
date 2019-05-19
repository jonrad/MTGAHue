using Newtonsoft.Json;

namespace MTGAHue.Configuration.Models
{
    [JsonObject]
    public class EventConfiguration
    {
        public string? Id { get; set; }

        public EffectConfiguration Effect { get; set; }
    }
}
