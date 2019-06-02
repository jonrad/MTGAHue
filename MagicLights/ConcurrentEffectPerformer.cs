using LightsApi;
using LightsApi.Transitions;
using MagicLights.Effects;
using MTGADispatcher.Events;
using System.Threading.Tasks;

namespace MagicLights
{
    public class ConcurrentEffectPerformer<T> : IEffectPerformer<T>
        where T : IMagicEvent
    {
        private readonly ILights lights;

        private readonly IEffect<T> effect;

        public ConcurrentEffectPerformer(
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

            _ = RunTransition(transition);
        }

        private async Task RunTransition(ITransition transition)
        {
            var layout = lights.AddLayout();
            await transition.Transition(layout);
            lights.RemoveLayout(layout);
        }
    }
}
