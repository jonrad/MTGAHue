using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi
{
    public class Delay : IDelay
    {
        public Task Wait(TimeSpan timeSpan, CancellationToken cancellationToken)
        {
            return Task.Delay(timeSpan, cancellationToken);
        }
    }
}
