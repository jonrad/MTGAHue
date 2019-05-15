using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi
{
    public class CompositeLightClient : ILightClient
    {
        private readonly ILightClient[] lightClients;

        public CompositeLightClient(params ILightClient[] lightClients)
        {
            this.lightClients = lightClients;
        }

        public ILightLayout GetLayout()
        {
            var layouts = lightClients
                .Select(l => l.GetLayout())
                .ToArray();

            return new CompositeLightLayout(layouts);
        }

        public async Task Start(CancellationToken token)
        {
            // There's a bug somewhere where if the chroma client starts before/same time
            // as the hue client, the hue client breaks (auth error?)
            foreach (var client in lightClients)
            {
                await client.Start(token);
            }
        }

        public Task Stop(CancellationToken token)
        {
            var tasks = lightClients
                .Select(l => l.Stop(token))
                .ToArray();

            return Task.WhenAll(tasks);
        }
    }
}
