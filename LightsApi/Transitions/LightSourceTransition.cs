using LightsApi.LightSources;
using System;

namespace LightsApi.Transitions
{
    public class LightSourceTransition : ITransition
    {
        private readonly ILightSource previous;

        private readonly ILightSource lightSource;

        private readonly float totalMs;

        public LightSourceTransition(ILightSource lightSource, double ms)
            : this(new SolidLightSource(RGB.Black), lightSource, (float)ms)
        {
        }

        public LightSourceTransition(ILightSource previous, ILightSource lightSource, float totalMs)
        {
            this.previous = previous;
            this.lightSource = lightSource;
            this.totalMs = totalMs;
            TotalLength = TimeSpan.FromMilliseconds(totalMs);
        }

        public TimeSpan TotalLength { get; }

        public RGB Get(float x, float y, long ms)
        {
            var start = previous.Calculate(x, y);
            var end = lightSource.Calculate(x, y);

            var percentage = totalMs == 0 || ms > totalMs 
                ? 1 
                : ms / totalMs;

            var r = start.R + (end.R - start.R) * percentage;

            return new RGB(
                start.R + (end.R - start.R) * percentage,
                start.G + (end.G - start.G) * percentage,
                start.B + (end.B - start.B) * percentage);
        }
    }
}
