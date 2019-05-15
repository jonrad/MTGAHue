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

        private readonly ILightClientFactory lightClientFactory;

        public Application(
            Game game,
            IMagicService magicService,
            ILightClientFactory lightClientFactory)
        {
            this.game = game;
            this.magicService = magicService;
            this.lightClientFactory = lightClientFactory;
        }

        public async Task Run()
        {
            var lightClient = await lightClientFactory.Create();

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
