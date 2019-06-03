using System;
using System.Linq;

namespace LightsApi.Transitions
{
    public class CompositeTransition : ITransition
    {
        private readonly ITransition[] transitions;

        public CompositeTransition(params ITransition[] transitions)
        {
            this.transitions = transitions;
            TotalLength = transitions.Max(t => t.TotalLength);
        }

        public TimeSpan TotalLength { get; }

        public RGB Get(float x, float y, long ms)
        {
            return transitions
                .Select(t => t.Get(x, y, ms))
                .Aggregate((rgb1, rgb2) => rgb1 + rgb2);
        }
    }
}
