using Newtonsoft.Json;

namespace MTGADispatcher.ClientModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Turn
    {
        [JsonProperty("phase")]
        public string Phase { get; set; }

        [JsonProperty("Step")]
        public string Step { get; set; }
    }
}
