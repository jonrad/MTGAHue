using System;

namespace LightsApi
{
    public class LayerBuilder : ILayerBuilder
    {
        public ILayer Build(Position[] lights, TimeSpan delay)
        {
            return new Layer(lights, delay);
        }
    }
}
