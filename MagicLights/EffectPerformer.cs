using LightsApi;
using MTGADispatcher.Events;
using MagicLights.Effects;
using LightsApi.Transitions;

namespace MagicLights
{
    public class EffectPerformer<T>
        where T : IMagicEvent
    {
        private readonly ILights lights;

        private readonly IEffect<T> effect;

        public EffectPerformer(
            ILights lights,
            IEffect<T> effect)
        {
            this.lights = lights;
            this.effect = effect;
        }

        public void Perform(T magicEvent)
        {
            var transition = effect.OnMagicEvent(magicEvent);

            if (transition == null)
            {
                return;
            }

            RunTransition(transition);
        }

        private void RunTransition(ITransition transition)
        {
            var layout = lights.AddLayout();
            transition.Transition(layout);
            lights.RemoveLayout(layout);
        }
    }
}
