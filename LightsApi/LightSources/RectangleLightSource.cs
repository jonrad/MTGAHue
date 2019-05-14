namespace LightsApi.LightSources
{
    public class RectangleLightSource : ILightSource
    {
        private readonly RGB rgb;

        private readonly double left;

        private readonly double top;

        private readonly double right;

        private readonly double bottom;

        public RectangleLightSource(RGB rgb, double left, double top, double width, double height)
        {
            this.rgb = rgb;
            this.left = left;
            this.top = top;
            right = left + width;
            bottom = top - height;
        }

        public RGB Calculate(double x, double y)
        {
            if (x >= left && y <= top && x <= right && y >= bottom)
            {
                return rgb;
            }

            return RGB.Black;
        }
    }
}
