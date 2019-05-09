namespace MTGAHue
{
    public class OmniLightSource : ILightSource
    {
        private readonly RGB rgb;

        public OmniLightSource(RGB rgb)
        {
            this.rgb = rgb;
        }

        public RGB Calculate(double x, double y)
        {
            return rgb;
        }
    }
}
