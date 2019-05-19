using LightsApi;
using LightsApi.Chroma;
using System.Threading.Tasks;

namespace MTGAHue.LightClients
{
    public class ChromaLightClientFactory : ILightClientProvider
    {
        public string Name { get; } = "Chroma (Razer) Keyboards";

        public Task<ILightClient> Create()
        {
            return Task.FromResult<ILightClient>(new ChromaKeyboardClient());
        }

        public void Dispose()
        {
        }
    }
}
