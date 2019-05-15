using MTGADispatcher;
using System;
using System.Threading;
using System.Threading.Tasks;
using MTGAHue.LightClients;

namespace MTGAHue
{
    public class Application
    {
        private readonly Game game;

        private readonly IMagicService magicService;

        private readonly ILightClientFactory lightClientFactories;

        public Application(
            Game game,
            IMagicService magicService,
            ILightClientFactory lightClientFactories)
        {
            this.game = game;
            this.magicService = magicService;
            this.lightClientFactories = lightClientFactories;
        }

        public async Task Run()
        {
            var lightClient = await lightClientFactories.Create();

            await lightClient.Start(CancellationToken.None);

            var layout = lightClient.GetLayout();

            var flasher = new HueSpellFlasher(game, layout);
            flasher.Start();

            magicService.Start();

            Console.WriteLine("Press enter to quit");
            Console.ReadLine();

            await lightClient.Stop(CancellationToken.None);
        }
    }
}
