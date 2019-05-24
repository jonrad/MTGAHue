using MTGADispatcher.Events;
using Newtonsoft.Json.Linq;

namespace MagicLights.Effects
{
    public interface IEffectFactory
    {
        IEffect<T> Get<T>(string id, JObject? configuration)
            where T : IMagicEvent;
    }
}
