using Newtonsoft.Json;

namespace MTGADispatcher.ClientModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class InstanceModel
    {
        [JsonProperty("instanceId")]
        public int Id { get; set; }

        [JsonProperty("grpId")]
        public int CardId { get; set; }

        [JsonProperty("ownerSeatId")]
        public int OwnerId { get; set; }

        [JsonProperty("color")]
        public string[] Colors { get; set; }

        [JsonProperty("cardTypes")]
        public string[] CardTypes { get; set; }

        [JsonProperty("subTypes")]
        public string[] CardSubtypes { get; set; }
    }
}
