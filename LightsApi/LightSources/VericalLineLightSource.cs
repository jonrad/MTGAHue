using System;

namespace LightsApi.LightSources
{
    public class VericalLineLightSource : ILightSource
    {
        private readonly RGB color;

        private readonly double xIntercept;

        private readonly double radius;

        public VericalLineLightSource(RGB color, double xIntercept, double radius)
        {
            this.color = color;
            this.xIntercept = xIntercept;
            this.radius = radius;
        }

        public RGB Calculate(double x, double y)
        {
            var distance = Math.Abs(x - xIntercept);
            var multiplier = (radius - distance) / radius;

            return color * multiplier;
        }
    }
}
