using System.Linq;

namespace LightsApi.LightSources
{
    public class LayeredLightSource : ILightSource //TODO needs specs
    {
        private readonly ILightSource[] layers;

        public LayeredLightSource(params ILightSource[] layers)
        {
            this.layers = layers.Reverse().ToArray();
        }

        public RGB Calculate(double x, double y)
        {
            foreach (var lightSource in layers)
            {
                var rgb = lightSource.Calculate(x, y);
                if (!rgb.Equals(RGB.Black))
                {
                    return rgb;
                }
            }

            return RGB.Black;
        }
    }
}
