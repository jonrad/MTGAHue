using Colore;
using LightsApi;
using LightsApi.Chroma;
using System.Threading.Tasks;

namespace MTGAHue.LightClients
{
    public class ChromaLightClientProvider : AbstractLightClientProvider<ChromaLightClientProvider.ChromaConfiguration>
    {
        public override string Id { get; } = "chroma";

        public override async Task<ILightClient> CreateAsync(ChromaConfiguration configuration)
        {
            var chroma = await ColoreProvider.CreateNativeAsync();
            return new ChromaKeyboardClient(chroma);
        }

        public class ChromaConfiguration { }
    }
}
