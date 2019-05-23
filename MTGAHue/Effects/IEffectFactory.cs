using MTGADispatcher.Events;
using Newtonsoft.Json.Linq;

namespace MTGAHue.Effects
{
    public interface IEffectFactory
    {
        IEffect<T> Get<T>(string id, JObject? configuration)
            where T : IMagicEvent;
    }
}
