using LightsApi.LightSources;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi
{
    public class CompositeLightLayout : ILightLayout
    {
        private readonly ILightLayout[] lightLayouts;

        public CompositeLightLayout(params ILightLayout[] lightLayouts)
        {
            this.lightLayouts = lightLayouts;
        }

        public Task Transition(ILightSource lightSource, TimeSpan timeSpan, CancellationToken token = default)
        {
            var tasks = lightLayouts
                .Select(l => l.Transition(lightSource, timeSpan, token))
                .ToArray();

            return Task.WhenAll(tasks);
        }

        public void Dispose()
        {
        }
    }
}
