using LightsApi;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MagicLights.Integration
{
    public class IntegrationLightClient : ILightClient
    {
        private readonly Position[] positions;

        public IntegrationLightClient()
        {
            positions = new[]
            {
                new Position(0, 0)
            };
        }

        public RGB[]? LastColors { get; private set; }

        public IEnumerable<Position> Lights => positions;

        public Task SetColors(IEnumerable<RGB> colors, CancellationToken token)
        {
            LastColors = colors.ToArray();
            return Task.FromResult(true);
        }
    }
}
