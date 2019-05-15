using LightsApi;
using System.Linq;
using System.Threading.Tasks;

namespace MTGAHue.LightClients
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
            return new CompositeLightClient(await Task.WhenAll(clients));
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
