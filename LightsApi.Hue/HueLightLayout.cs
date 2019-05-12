using System;
using System.Threading;
using System.Threading.Tasks;
using LightsApi.LightSources;

namespace LightsApi.Hue
{
    internal class HueLightLayout : ILightLayout
    {
        private readonly HueLight[] lights;

        // gross, we need a wrapper around lightlayout
        private CancellationTokenSource tokenSource;

        public HueLightLayout(HueLight[] lights)
        {
            this.lights = lights;
        }

        public Task Transition(
            ILightSource lightSource,
            TimeSpan timeSpan,
            CancellationToken token = default)
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
            }

            tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

            foreach (var light in lights)
            {
                light.Transition(lightSource.Calculate(light.X, light.Y), timeSpan, tokenSource.Token);
            }

            return Task.Delay(timeSpan, tokenSource.Token);
        }
    }
}
