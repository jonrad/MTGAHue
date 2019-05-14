using System;

namespace LightsApi.LightSources
{
    public class AngleFilterLightSource : ILightSource
    {
        private const double TWO_PI = Math.PI * 2;

        private readonly double radiansStart;

        private readonly double radiansEnd;

        private readonly ILightSource baseLightSource;

        public AngleFilterLightSource(
            ILightSource baseLightSource,
            double angleStart,
            double degrees)
        {
            this.baseLightSource = baseLightSource;

            radiansStart = angleStart * Math.PI / 180D;
            radiansEnd = (angleStart + degrees) * Math.PI / 180D;
        }

        public RGB Calculate(double x, double y)
        {
            var radians = CalculateRadians(x, y);

            if (radians < radiansStart && (radiansEnd < TWO_PI || radians + TWO_PI > radiansEnd))
            {
                return RGB.Black;
            }

            if (radians > radiansEnd)
            {
                return RGB.Black;
            }

            return baseLightSource.Calculate(x, y);
        }

        private double CalculateRadians(double x, double y)
        {
            var radians = Math.Atan2(y, x);

            if (radians >= 0)
            {
                return radians;
            }

            return Math.PI * 2 + radians;
        }
    }
}
