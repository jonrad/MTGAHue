using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MTGAHue.Configuration.Models
{
    [JsonObject]
    public class EffectConfiguration
    {
        public string? Id { get; set; }

        public JObject? Config { get; set; }
    }
}
