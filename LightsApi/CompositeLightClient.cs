using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightsApi.Transitions;

namespace LightsApi
{
    public class CompositeLightClient : ILightClient
    {
        private readonly ILightClient[] lightClients;

        public CompositeLightClient(params ILightClient[] lightClients)
        {
            this.lightClients = lightClients;
        }

        public IEnumerable<Position> Lights => lightClients.First().Lights;

        public Task SetColors(IEnumerable<RGB> colors, CancellationToken token)
        {
            return lightClients.First().SetColors(colors, token);
        }
    }
}
