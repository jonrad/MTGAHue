using LightsApi;
using LightsApi.LightSources;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MagicLights.Integration
{
    public class IntegrationLightLayout : ILightLayout
    {
        public ILightSource? LastLightSource { get; set; }

        public RGB? CenterColor { get; set; }

        public Task Transition(ILightSource lightSource, TimeSpan timeSpan, CancellationToken token = default)
        {
            LastLightSource = lightSource;
            CenterColor = lightSource.Calculate(0, 0);

            return Task.Delay(timeSpan);
        }
    }
}
