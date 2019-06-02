using MTGADispatcher.Events;

namespace MagicLights
{
    public interface IEffectPerformer<T>
        where T : IMagicEvent
    {
        void Perform(T magicEvent);
    }
}
