using System;
using System.Collections.Generic;

namespace LightsApi.LightSources
{
    public class GradientLightSource : ILightSource
    {
        private readonly RGB startColor;

        private readonly RGB endColor;

        private readonly float totalDistance;

        private readonly float A, B, CStart;

        public readonly float CEnd;

        public GradientLightSource(
            RGB startColor,
            RGB endColor,
            Position start,
            Position end)
        {
            this.startColor = startColor;
            this.endColor = endColor;
            A = end.X - start.X;
            B = end.Y - start.Y;
            CStart = -start.X * A - start.Y * B;
            CEnd = -end.X * A - end.Y * B;
            totalDistance = (float)Math.Sqrt(Math.Pow(end.Y - start.Y, 2) + Math.Pow(end.X - start.X, 2));
        }

        public RGB Calculate(double x, double y)
        {
            var axBy = A * x + B * y;

            var distance = (float)Math.Abs(axBy + CStart) / (Math.Sqrt(A * A + B * B));
            var distanceEnd = (float)Math.Abs(axBy + CEnd) / (Math.Sqrt(A * A + B * B));
            if (distance < 0 || distance > totalDistance || distanceEnd > totalDistance)
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
