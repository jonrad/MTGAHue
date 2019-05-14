using System;
using System.Linq;

namespace LightsApi.LightSources
{
    public class CompositeLightSource : ILightSource
    {
        private ILightSource[] sources;

        public CompositeLightSource(params ILightSource[] lightSources)
        {
            sources = lightSources;
        }

        public RGB Calculate(double x, double y)
        {
            return sources
                .Select(c => c.Calculate(x, y))
                .Aggregate(new RGB(), (rgb1, rgb2) => new RGB(
                        Math.Max(rgb1.R, rgb2.R),
                        Math.Max(rgb1.G, rgb2.G),
                        Math.Max(rgb1.B, rgb2.B)));
        }
    }
}
