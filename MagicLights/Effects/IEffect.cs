using LightsApi.Transitions;
using MTGADispatcher.Events;

namespace MagicLights.Effects
{
    public interface IEffect<T>
        where T : IMagicEvent
    {
        ITransition? OnMagicEvent(T magicEvent);
    }
}
