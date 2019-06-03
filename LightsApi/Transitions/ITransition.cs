using System;

namespace LightsApi.Transitions
{
    public interface ITransition
    {
        TimeSpan TotalLength { get; }

        RGB Get(float x, float y, long ms);
    }
}
