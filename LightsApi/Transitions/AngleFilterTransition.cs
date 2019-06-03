using System;

namespace LightsApi.Transitions
{
    public class AngleFilterTransition : ITransition
    {
        private const double TWO_PI = Math.PI * 2;

        private readonly double centerX;

        private readonly double centerY;

        private readonly double radiansStart;

        private readonly double radiansEnd;

        private readonly ITransition baseTransition;

        public AngleFilterTransition(
            ITransition baseTransition,
            double centerX,
            double centerY,
            double angleStart,
            double degrees)
        {
            this.baseTransition = baseTransition;
            this.centerX = centerX;
            this.centerY = centerY;
            radiansStart = angleStart * Math.PI / 180D;
            radiansEnd = (angleStart + degrees) * Math.PI / 180D;
        }

        public TimeSpan TotalLength => baseTransition.TotalLength;

        public RGB Get(float x, float y, long ms)
        {
            var radians = CalculateRadians(x - centerX, y - centerY);

            if (radians < radiansStart && (radiansEnd < TWO_PI || radians + TWO_PI > radiansEnd))
            {
                return RGB.Black;
            }

            if (radians > radiansEnd)
            {
                return RGB.Black;
            }

            return baseTransition.Get(x, y, ms);
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
