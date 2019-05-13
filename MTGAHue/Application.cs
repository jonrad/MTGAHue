using MTGADispatcher;
using System;
using System.Threading;
using System.Threading.Tasks;
using LightsApi;

namespace MTGAHue
{
    public class Application
    {
        private readonly IMagicService magicService;

        private readonly ILightClientFactory lightClientFactories;

        public Application(
            IMagicService magicService,
            ILightClientFactory lightClientFactories)
        {
            this.magicService = magicService;
            this.lightClientFactories = lightClientFactories;
        }

        public async Task Play()
        {
            var lightClient = await lightClientFactories.Create();

            await lightClient.Start(CancellationToken.None);

            magicService.Start();

            Console.WriteLine("Press enter to quit");
            Console.ReadLine();

            await lightClient.Stop(CancellationToken.None);
        }
    }
}
