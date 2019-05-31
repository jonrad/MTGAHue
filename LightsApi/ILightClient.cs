using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi
{
    public interface ILightClient
    {
        IEnumerable<Position> Lights { get; }

        Task SetColors(IEnumerable<RGB> colors, CancellationToken token);
    }
}
