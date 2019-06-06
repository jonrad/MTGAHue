using System;

namespace LightsApi.LightSources
{
    public class GradientLightSource : ILightSource
    {
        private readonly RGB startColor;

        private readonly RGB endColor;

        private readonly double totalDistance;

        private readonly float a, b, c;

        public GradientLightSource(
            RGB startColor,
            RGB endColor,
            Position start,
            Position end)
        {
            this.startColor = startColor;
            this.endColor = endColor;

            //Ax + By + C = 0
            a = end.X - start.X;
            b = end.Y - start.Y;
            c = -start.X * a - start.Y * b;

            var denominator = Math.Sqrt(a * a + b * b);
            totalDistance = Math.Sqrt(Math.Pow(end.Y - start.Y, 2) + Math.Pow(end.X - start.X, 2)) * denominator;
        }

        public RGB Calculate(double x, double y)
        {
            // Note that this has been optimized to not perform powers and sqrts
            // in the calculate function. So this isn't the traditional distance
            // from a line formula
            var distance = a * x + b * y + c;
            if (distance < 0 || distance > totalDistance)
            {
                return RGB.Black;
            }

            var percentage = (float)(distance / totalDistance);

            return new RGB(
                (endColor.R - startColor.R) * percentage + startColor.R,
                (endColor.G - startColor.G) * percentage + startColor.G,
                (endColor.B - startColor.B) * percentage + startColor.B);
        }
    }
}
