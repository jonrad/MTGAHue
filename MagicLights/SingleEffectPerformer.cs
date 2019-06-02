using LightsApi;
using MTGADispatcher.Events;
using MagicLights.Effects;
using LightsApi.Transitions;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace MagicLights
{
    public class SingleEffectPerformer<T> : IEffectPerformer<T>
        where T : IMagicEvent
    {
        private readonly object syncObject = new object();

        private readonly ILights lights;

        private readonly IEffect<T> effect;

        private readonly Lazy<ILightLayout> layout;

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public SingleEffectPerformer(
            ILights lights,
            IEffect<T> effect)
        {
            this.lights = lights;
            this.effect = effect;
            layout = new Lazy<ILightLayout>(CreateLayout);
        }

        public void Perform(T magicEvent)
        {
            var transition = effect.OnMagicEvent(magicEvent);

            if (transition == null)
            {
                return;
            }

            lock (syncObject)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource = new CancellationTokenSource();

                _ = transition.Transition(layout.Value, cancellationTokenSource.Token);
            }
        }

        private ILightLayout CreateLayout()
        {
            return lights.AddLayout();
        }
    }
}
