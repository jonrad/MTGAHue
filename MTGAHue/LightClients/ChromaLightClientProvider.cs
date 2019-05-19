using LightsApi;
using LightsApi.Chroma;
using System.Threading.Tasks;

namespace MTGAHue.LightClients
{
    public class ChromaLightClientProvider : AbstractLightClientProvider<ChromaLightClientProvider.ChromaConfiguration>
    {
        public override string Id { get; } = "chroma";

        public override Task<ILightClient> Create(ChromaConfiguration configuration)
        {
            return Task.FromResult<ILightClient>(new ChromaKeyboardClient());
        }

        public class ChromaConfiguration { }
    }
}
