using System;

namespace LightsApi.Transitions
{
    public class FilteredTransition : ITransition
    {
        private readonly Func<float, float, long, bool> predicate;

        private readonly ITransition transition;

        public FilteredTransition(Func<float, float, long, bool> predicate, ITransition transition)
        {
            this.predicate = predicate;
            this.transition = transition;

            TotalLength = transition.TotalLength;
        }

        public TimeSpan TotalLength { get; }

        public RGB Get(float x, float y, long ms)
        {
            if (!predicate(x, y, ms))
            {
                return RGB.Black;
            }

            return transition.Get(x, y, ms);
        }
    }
}
