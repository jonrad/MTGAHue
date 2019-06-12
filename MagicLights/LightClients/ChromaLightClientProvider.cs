using Colore;
using LightsApi;
using LightsApi.Chroma;
using System;
using System.Threading.Tasks;

namespace MagicLights.LightClients
{
    public class ChromaLightClientProvider
        : AbstractLightClientProvider<ChromaLightClientProvider.ChromaConfiguration>
    {
        private readonly Lazy<Task<IChroma>> chromaTask =
            new Lazy<Task<IChroma>>(() => ColoreProvider.CreateNativeAsync());

        private IChroma? chroma;

        public override string Id { get; } = "chroma";

        public override async Task<ILightClient> CreateAsync(ChromaConfiguration configuration)
        {
            if (chroma == null)
            {
                chroma = await chromaTask.Value;
            }

            return new ChromaKeyboardClient(chroma, configuration.Columns, configuration.Rows);
        }

        public override void Dispose()
        {
            if (!chromaTask.IsValueCreated)
            {
                return;
            }

            var value = chromaTask.Value.Result;

            value.UninitializeAsync().Wait();
        }

        public class ChromaConfiguration
        {
            public int? Columns { get; set; } = 22;

            public int? Rows { get; set; } = 6;
        }
    }
}
