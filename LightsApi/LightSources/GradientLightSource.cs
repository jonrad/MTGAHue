namespace LightsApi.LightSources
{
    public class GradientLightSource : ILightSource
    {
        private readonly RGB startColor;

        private readonly RGB endColor;

        private readonly float start;

        private readonly float end;

        private readonly float width;

        public GradientLightSource(
            RGB startColor,
            RGB endColor,
            float start,
            float end)
        {
            this.startColor = startColor;
            this.endColor = endColor;
            this.start = start;
            this.end = end;
            width = end - start;
        }

        public RGB Calculate(double x, double y)
        {
            if (x < start || x > end)
            {
                return RGB.Black;
            }

            var percentage = (float)((x - start) / width);

            return new RGB(
                (endColor.R - startColor.R) * percentage + startColor.R,
                (endColor.G - startColor.G) * percentage + startColor.G,
                (endColor.B - startColor.B) * percentage + startColor.B);
        }
    }
}
