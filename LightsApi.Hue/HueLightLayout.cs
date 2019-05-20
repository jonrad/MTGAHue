using System;
using System.Threading;
using System.Threading.Tasks;
using LightsApi.LightSources;

namespace LightsApi.Hue
{
    internal class HueLightLayout : ILightLayout
    {
        private readonly HueLight[] lights;

        // TODO refactor this+chroma
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        public HueLightLayout(HueLight[] lights)
        {
            this.lights = lights;
        }

        public void Dispose()
        {
        }

        public Task Transition(
            ILightSource lightSource,
            TimeSpan timeSpan,
            CancellationToken token = default)
        {
            tokenSource.Cancel();

            tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

            foreach (var light in lights)
            {
                light.Transition(lightSource.Calculate(light.X, light.Y), timeSpan, tokenSource.Token);
            }

            return Task.Delay(timeSpan, tokenSource.Token);
        }
    }
}
