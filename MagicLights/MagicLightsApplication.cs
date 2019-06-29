using MagicLights.Configuration;
using MTGADispatcher;
using System.Threading.Tasks;

namespace MagicLights
{
    public class MagicLightsApplication : IMagicLights
    {
        private readonly LightClientManager lightClientManager;

        private readonly IMagicService magicService;

        private readonly ILightsConfigurationProvider configurationProvider;

        public MagicLightsApplication(
            LightClientManager lightsSetup,
            IMagicService magicService,
            ILightsConfigurationProvider configurationProvider)
        {
            lightClientManager = lightsSetup;
            this.magicService = magicService;
            this.configurationProvider = configurationProvider;
        }

        public async Task Start()
        {
            var config = configurationProvider.Get();

            await lightClientManager.Start(config);

            magicService.Start();
        }

        public async Task Stop()
        {
            await magicService.Stop();
            lightClientManager.Stop();
        }
    }
}
