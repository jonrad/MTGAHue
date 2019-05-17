using Newtonsoft.Json;

namespace MTGADispatcher.ClientModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GameInfo
    {
        [JsonProperty("stage")]
        public string Stage { get; set; }
    }
}
