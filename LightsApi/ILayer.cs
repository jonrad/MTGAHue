using LightsApi.LightSources;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi
{
    public interface ILayer
    {
        RGB[] Colors { get; }

        Task Transition(ILightSource lightSource, TimeSpan timeSpan, CancellationToken token = default);
    }
}
