using System;

namespace LightsApi.LightSources
{
    public class FadedCircleLightSource : ILightSource
    {
        private readonly RGB color;

        private readonly double x;

        private readonly double y;

        private readonly double radius;

        private readonly double width;

        private readonly double innerRadius;

        private readonly double outerRadius;

        public FadedCircleLightSource(
            RGB color,
            double x,
            double y,
            double radius,
            double width)
        {
            this.color = color;
            this.x = x;
            this.y = y;
            this.radius = radius;
            this.width = width;
            innerRadius = radius - width / 2;
            outerRadius = radius + width / 2;
        }

        public RGB Calculate(double lightX, double lightY)
        {
            var centerDistance = Math.Sqrt(Math.Pow(lightX - x, 2) + Math.Pow(lightY - y, 2));

            if (centerDistance > outerRadius || centerDistance < innerRadius)
            {
                return RGB.Black;
            }

            var solidDistance = Math.Abs(radius - centerDistance);
            var multiplier = 1 - solidDistance / width;

            return color * multiplier;
        }
    }
}
