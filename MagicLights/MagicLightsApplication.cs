using MagicLights.Configuration;
using MTGADispatcher;
using System.Threading.Tasks;

namespace MagicLights
{
    public class MagicLightsApplication
    {
        private readonly LightsSetup lightsSetup;

        private readonly IMagicService magicService;

        private readonly ILightsConfigurationProvider configurationProvider;

        public MagicLightsApplication(
            LightsSetup lightsSetup,
            IMagicService magicService,
            ILightsConfigurationProvider configurationProvider)
        {
            this.lightsSetup = lightsSetup;
            this.magicService = magicService;
            this.configurationProvider = configurationProvider;
        }

        public async Task Start()
        {
            var config = configurationProvider.Get();

            await lightsSetup.Start(config);

            magicService.Start();
        }

        public void Stop()
        {
            magicService.Stop();
        }
    }
}
