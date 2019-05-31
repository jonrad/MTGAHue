using LightsApi;
using MTGADispatcher.Events;
using MagicLights.Effects;

namespace MagicLights
{
    public class EffectPerformer<T>
        where T : IMagicEvent
    {
        private readonly LightClientLoop loop;

        private readonly IEffect<T> effect;

        public EffectPerformer(
            LightClientLoop loop,
            IEffect<T> effect)
        {
            this.loop = loop;
            this.effect = effect;
        }

        public void Perform(T magicEvent)
        {
            var transition = effect.OnMagicEvent(magicEvent);

            if (transition == null)
            {
                return;
            }

            loop.Transition(transition);
        }
    }
}
