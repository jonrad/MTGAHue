using System;

namespace LightsApi
{
    public interface ILayerBuilder
    {
        ILayer Build(Position[] lights, TimeSpan delay);
    }
}
