using LightsApi;
using LightsApi.Cue;
using System;
using System.Threading.Tasks;

namespace MagicLights.LightClients
{
    public class CueLightClientProvider : AbstractLightClientProvider<CueLightClientProvider.CueConfiguration>
    {
        private readonly Lazy<CueLightClient> chromaTask =
            new Lazy<CueLightClient>(() => new CueLightClient());

        public override string Id { get; } = "cue";

        public override Task<ILightClient> CreateAsync(CueConfiguration configuration)
        {
            return Task.FromResult<ILightClient>(chromaTask.Value);
        }

        public class CueConfiguration
        {
        }
    }
}
