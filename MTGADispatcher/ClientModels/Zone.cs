using Newtonsoft.Json;

namespace MTGADispatcher.ClientModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Zone
    {
        [JsonProperty("zoneId")]
        public int Id { get; set; }

        [JsonProperty("type")]
        public ZoneType Type { get; set; }

        [JsonProperty("visibility")]
        public Visibility Visibility { get; set; }

        [JsonProperty("ownerSeatId")]
        public int OwnerId { get; set; }

        [JsonProperty("objectInstanceIds")]
        public int[]? InstanceIds { get; set; }
    }
}
