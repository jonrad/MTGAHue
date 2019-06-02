using LightsApi;
using MagicLights.LightClients;
using System;
using System.Threading.Tasks;

namespace MagicLights.Integration
{
    public class IntegrationLightClientProvider : ILightClientProvider
    {
        public IntegrationLightClientProvider(IntegrationLightClient lightClient)
        {
            LightClient = lightClient;
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
    }
}
