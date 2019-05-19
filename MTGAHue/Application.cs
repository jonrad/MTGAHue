using MTGADispatcher;
using System;
using System.Threading;
using System.Threading.Tasks;
using MTGAHue.LightClients;
using System.Linq;
using LightsApi;
using MTGADispatcher.Events;
using System.Collections.Generic;

namespace MTGAHue
{
    public class Application
    {
        private readonly Game game;

        private readonly IMagicService magicService;

        private readonly ILightClientProvider[] lightClientProviders;

        private readonly IEventEffectBuilder eventEffects;

        public Application(
            Game game,
            IMagicService magicService,
            ILightClientProvider[] lightClientProviders,
            IEventEffectBuilder eventEffects)
        {
            this.game = game;
            this.magicService = magicService;
            this.lightClientProviders = lightClientProviders;
            this.eventEffects = eventEffects;
        }

        public async Task Run()
        {
            var lightClient = await BuildLightClient();

            await lightClient.Start(CancellationToken.None);

            var layout = lightClient.GetLayout();

            var unsubscribe = Subscribe<CastSpell>(layout).ToArray();

            magicService.Start();

            Console.WriteLine("Press enter to quit");
            Console.ReadLine();

            await lightClient.Stop(CancellationToken.None);

            foreach (var action in unsubscribe)
            {
                action();
            }
        }

        private IEnumerable<Action> Subscribe<T>(ILightLayout lightLayout)
            where T : IMagicEvent
        {
            var handlers = eventEffects.Get<T>(game, lightLayout);
            foreach (var handler in handlers)
            {
                game.Events.Subscriptions.Subscribe<T>(handler.OnMagicEvent);
                yield return () =>
                {
                    game.Events.Subscriptions.Unsubscribe<T>(handler.OnMagicEvent);
                };
            }
        }

        public async Task<ILightClient> BuildLightClient()
        {
            var lightClients = await Task.WhenAll(
                lightClientProviders
                    .Select(l => l.Create())
                    .ToArray());

            return new CompositeLightClient(lightClients);
        }
    }
}
