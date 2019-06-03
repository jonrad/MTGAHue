using System;
using System.Linq;

namespace LightsApi.Transitions
{
    public class CompositeTransition : ITransition
    {
        private readonly TimedTransition[] transitions;

        private int currentIndex = 0;

        public CompositeTransition(params ITransition[] transitions)
        {
            var startTime = 0l;
            this.transitions = transitions.Select(t =>
            {
                var endTime = startTime + (long)t.TotalLength.TotalMilliseconds;
                var result = new TimedTransition(startTime, endTime, t);
                startTime = endTime;

                return result;
            }).ToArray();

            TotalLength = TimeSpan.FromMilliseconds(transitions.Sum(t => t.TotalLength.TotalMilliseconds));
        }

        public TimeSpan TotalLength { get; }

        public RGB Get(float x, float y, long ms)
        {
            var transition = transitions[currentIndex];

            if (transition.StartTime > ms)
            {
                currentIndex = 0;
            }

            while ((transition = transitions[currentIndex]).EndTime < ms)
            {
                if (currentIndex == transitions.Length - 1)
                {
                    break;
                }

                currentIndex++;
            }

            return transition.Transition.Get(x, y, ms - transition.StartTime);
        }

        public class TimedTransition
        {
            public long StartTime { get; }

            public long EndTime { get; }

            public ITransition Transition { get; }

            public TimedTransition(long startTime, long endTime, ITransition transition)
            {
                StartTime = startTime;
                EndTime = endTime;
                Transition = transition;
            }
        }
    }
}
