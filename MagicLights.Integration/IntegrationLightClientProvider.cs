using LightsApi;
using MagicLights.LightClients;
using System;
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
            public IntegrationLightClient()
            {
                LightLayout = new IntegrationLightLayout();
            }

            public IntegrationLightLayout LightLayout { get; }

            public Task<ILightLayout> GetLayout()
            {
                return Task.FromResult<ILightLayout>(LightLayout);
            }

            public void Start()
            {
            }

            public void Stop()
            {
            }
        }
    }
}
