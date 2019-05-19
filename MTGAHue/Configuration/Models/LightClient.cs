using Newtonsoft.Json;

namespace MTGAHue.Configuration.Models
{
    [JsonObject]
    public class LightClient
    {
        public string Id { get; set; }

        public bool Enabled { get; set; }

        public Colors Colors { get; set; }

        public Effect[] Effects { get; set; }
    }

    [JsonObject]
    public class Effect
    {
        public string Id { get; set; }

        public Colors Colors { get; set; }

        public Event Events { get; set; }
    }

    [JsonObject]
    public class Event
    {
        public string Id { get; set; }

        public Colors Colors { get; set; }
    }
}
