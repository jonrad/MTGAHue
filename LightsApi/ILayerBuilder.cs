using System;

namespace LightsApi
{
    public interface ILayerBuilder
    {
        ILightLayout Build(Position[] lights, TimeSpan delay);
    }

    public class LayerBuilder : ILayerBuilder
    {
        public ILightLayout Build(Position[] lights, TimeSpan delay)
        {
            return new LightLayout(lights, delay);
        }
    }
}
