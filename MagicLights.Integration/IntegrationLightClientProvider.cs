using LightsApi;
using MagicLights.LightClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MagicLights.Integration
{
    public class IntegrationLightClientProvider : ILightClientProvider
    {
        public IntegrationLightClientProvider()
        {
            LightClient = new IntegrationLightClient();
        }

        public string Id => "integration";

        public Type ConfigurationType => typeof(object);

        public IntegrationLightClient LightClient { get; }

        public Task<ILightClient> Create(object configuration)
        {
            return Task.FromResult<ILightClient>(LightClient);
        }

        public void Dispose()
        {
        }

        public class IntegrationLightClient : ILightClient
        {
            private readonly Position[] positions;

            public IntegrationLightClient()
            {
                positions = new[]
                {
                    new Position(0, 0)
                };
            }

            public RGB[]? LastColors { get; private set; }

            public IEnumerable<Position> Lights => positions;

            public Task SetColors(IEnumerable<RGB> colors, CancellationToken token)
            {
                LastColors = colors.ToArray();
                return Task.FromResult(true);
            }
        }
    }
}
