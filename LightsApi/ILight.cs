using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi
{
    public interface ILight
    {
        Task Transition(RGB rgb, TimeSpan transitionTime, CancellationToken token);

        double X { get; }

        double Y { get; }
    }
}
