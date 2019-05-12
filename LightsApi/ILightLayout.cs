using LightsApi.LightSources;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi
{
    public interface ILightLayout
    {
        Task Transition(ILightSource lightSource, TimeSpan timeSpan, CancellationToken token = default);
    }
}
