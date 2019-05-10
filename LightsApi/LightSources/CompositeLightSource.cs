using System;
using System.Collections.Generic;
using System.Linq;

namespace LightsApi.LightSources
{
    public class CompositeLightSource : ILightSource
    {
        private List<ILightSource> sources = new List<ILightSource>();

        public CompositeLightSource(params ILightSource[] lightSources)
        {
            sources.AddRange(lightSources);
        }

        public void Add(ILightSource lightSource)
        {
            sources.Add(lightSource);
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
