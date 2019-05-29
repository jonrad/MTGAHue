using System.Linq;
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

        public async Task<ILightLayout> GetLayout()
        {
            var layouts = Task.WhenAll(
                lightClients
                    .Select(async l => await l.GetLayout())
                    .ToArray());

            return new CompositeLightLayout(await layouts);
        }

        public void Start()
        {
            foreach (var client in lightClients)
            {
                client.Start();
            }
        }

        public void Stop()
        {
            foreach (var client in lightClients)
            {
                client.Stop();
            }
        }
    }
}
