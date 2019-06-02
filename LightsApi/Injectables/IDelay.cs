using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi.Injectables
{
    public interface IDelay
    {
        Task Wait(TimeSpan timeSpan, CancellationToken cancellationToken);
    }
}
