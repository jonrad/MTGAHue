using Newtonsoft.Json;

namespace MTGAHue.Configuration.Models
{
    [JsonObject]
    public class Config
    {
        public ColorsConfiguration? Colors { get; set; }

        public LightClientConfiguration[]? LightClients { get; set; }
    }
}
