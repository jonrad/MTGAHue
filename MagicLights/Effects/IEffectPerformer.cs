using MTGADispatcher.Events;

namespace MagicLights.Effects
{
    public interface IEffectPerformer<T>
        where T : IMagicEvent
    {
        void Perform(T magicEvent);
    }
}
