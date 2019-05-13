using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi
{
    public class CompositeLightClientFactory : ILightClientFactory
    {
        private readonly ILightClientFactory[] factories;

        public CompositeLightClientFactory(ILightClientFactory[] factories)
        {
            this.factories = factories;
        }

        public async Task<ILightClient> Create()
        {
            var clients = factories.Select(f => f.Create());
            var client = new CompositeLightClient(await Task.WhenAll(clients));

            await client.Start(CancellationToken.None);

            return client;
        }

        public void Dispose()
        {
            foreach (var factory in factories)
            {
                factory.Dispose();
            }
        }
    }
}
