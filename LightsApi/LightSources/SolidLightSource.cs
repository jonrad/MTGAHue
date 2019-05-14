namespace LightsApi.LightSources
{
    public class SolidLightSource : ILightSource
    {
        private readonly RGB rgb;

        public SolidLightSource(RGB rgb)
        {
            this.rgb = rgb;
        }

        public RGB Calculate(double x, double y)
        {
            return rgb;
        }
    }
}
