using System;

namespace LightsApi.LightSources
{
    public class AreaLightSource : ILightSource
    {
        private readonly RGB color;

        private readonly double x;

        private readonly double y;

        private readonly double radius;

        public AreaLightSource(RGB color, double x, double y, double radius)
        {
            this.color = color;
            this.x = x;
            this.y = y;
            this.radius = radius;
        }

        public RGB Calculate(double lightX, double lightY)
        {
            var distance = Math.Sqrt(Math.Pow(lightX - x, 2) + Math.Pow(lightY - y, 2));

            if (distance > radius)
            {
                return RGB.Black;
            }

            var multiplier = (radius - distance) / radius;

            return color * multiplier;
        }
    }
}
