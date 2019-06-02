using LightsApi;
using MagicLights.Effects;
using MTGADispatcher.Events;
using System;
using System.Threading;

namespace MagicLights.Effects
{
    public class SingleEffectPerformer<T> : IEffectPerformer<T>
        where T : IMagicEvent
    {
        private readonly object syncObject = new object();

        private readonly ILights lights;

        private readonly IEffect<T> effect;

        private readonly Lazy<ILayer> layer;

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public SingleEffectPerformer(
            ILights lights,
            IEffect<T> effect)
        {
            this.lights = lights;
            this.effect = effect;
            layer = new Lazy<ILayer>(() => lights.AddLayer());
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

                _ = transition.Transition(layer.Value, cancellationTokenSource.Token);
            }
        }
    }
}
