using LightsApi.LightSources;
using System;

namespace LightsApi.Transitions
{
    public class LinearMovementTransition : ITransition
    {
        private readonly ILightSource lightSource;

        private readonly float xMovement;

        private readonly float yMovement;

        private readonly double totalMs;

        public LinearMovementTransition(
            ILightSource lightSource,
            float xMovement,
            float yMovement,
            TimeSpan timeSpan)
        {
            this.lightSource = lightSource;
            this.xMovement = xMovement;
            this.yMovement = yMovement;
            TotalLength = timeSpan;
            totalMs = TotalLength.TotalMilliseconds;
        }

        public TimeSpan TotalLength { get; }

        public RGB Get(float x, float y, long ms)
        {
            return lightSource.Calculate(
                x - xMovement * (ms / totalMs),
                y - yMovement * (ms / totalMs));
        }
    }
}
