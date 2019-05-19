using MTGADispatcher.Events;

namespace MTGAHue.Effects
{
    public interface IEffectFactory
    {
        IEffect<T> Get<T>(string id)
            where T : IMagicEvent;
    }
}
