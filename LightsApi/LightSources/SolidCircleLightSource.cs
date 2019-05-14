using System;

namespace LightsApi.LightSources
{
    public class SolidCircleLightSource : ILightSource
    {
        private readonly RGB color;

        private readonly double x;

        private readonly double y;

        private readonly double radius;

        private readonly double blackRadius;

        public SolidCircleLightSource(RGB color, double x, double y, double radius, double blackRadius = -1)
        {
            this.color = color;
            this.x = x;
            this.y = y;
            this.radius = radius;
            this.blackRadius = blackRadius;
        }

        public RGB Calculate(double lightX, double lightY)
        {
            var distance = Math.Sqrt(Math.Pow(lightX - x, 2) + Math.Pow(lightY - y, 2));

            if (distance > radius || distance < blackRadius)
            {
                return RGB.Black;
            }

            return color;
        }
    }
}
