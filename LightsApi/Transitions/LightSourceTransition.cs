using LightsApi.LightSources;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi.Transitions
{
    public class LightSourceTransition : ITransition
    {
        private readonly ILightSource lightSource;

        private readonly TimeSpan timeSpan;

        public LightSourceTransition(ILightSource lightSource, double ms)
            : this(lightSource, TimeSpan.FromMilliseconds(ms))
        {
        }

        public LightSourceTransition(ILightSource lightSource, TimeSpan timeSpan)
        {
            this.lightSource = lightSource;
            this.timeSpan = timeSpan;
        }

        public Task Transition(ILightLayout lightLayout, CancellationToken token = default)
        {
            return lightLayout.Transition(lightSource, timeSpan, token);
        }
    }
}
