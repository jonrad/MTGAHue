using Newtonsoft.Json;

namespace MagicLights.Configuration.Models
{
    [JsonObject]
    public class Config
    {
        public ColorsConfiguration? Colors { get; set; }

        public LightClientConfiguration[]? LightClients { get; set; }
    }
}
