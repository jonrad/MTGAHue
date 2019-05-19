using LightsApi;
using MTGADispatcher;
using MTGADispatcher.Events;
using MTGAHue.Configuration.Models;
using MTGAHue.Effects;
using MTGAHue.LightClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MTGAHue
{
    public class LightsSetup
    {
        private readonly Dictionary<string, ILightClientProvider> lightClientProviders;

        private readonly Dictionary<string, Action<ILightLayout, EffectConfiguration>> eventsById;

        private readonly Game game;

        private readonly IEffectFactory effectFactory;

        public LightsSetup(
            Game game,
            ILightClientProvider[] lightClientProviders,
            IEffectFactory effectFactory)
        {
            this.lightClientProviders =
                lightClientProviders.ToDictionary(l => l.Id);
            this.game = game;
            this.effectFactory = effectFactory;
            eventsById = BuildRegisters();
        }

        public async Task<CompositeLightClient> Build(Config configuration)
        {
            var clients = BuildClients(
                configuration.LightClients ?? new LightClientConfiguration[0]);

            return new CompositeLightClient((await clients).ToArray());
        }

        private async Task<ILightClient[]> BuildClients(
            LightClientConfiguration[] lightClients)
        {
            var results = lightClients
                .Where(l => l.Enabled)
                .Select(BuildClient)
                .ToArray();

            return await Task.WhenAll(results);
        }

        private async Task<ILightClient> BuildClient(
            LightClientConfiguration lightClientConfiguration)
        {
            if (lightClientConfiguration.Id == null)
            {
                throw new ArgumentException();
            }

            var id = lightClientConfiguration.Id;

            if (!lightClientProviders.TryGetValue(id, out var provider))
            {
                throw new ArgumentException();
            }

            var clientConfiguration =
                BuildClientArgs(provider);

            var client = await provider.Create(clientConfiguration);
            await client.Start();
            var layout = client.GetLayout();

            foreach (var eventConfiguration in 
                lightClientConfiguration.Events ?? Enumerable.Empty<EventConfiguration>())
            {
                var eventId = eventConfiguration.Id;
                if (eventId == null)
                {
                    continue;
                }

                var register = eventsById[eventId];

                var effectConfiguration = eventConfiguration.Effect;

                if (effectConfiguration == null)
                {
                    continue;
                }

                register(layout, effectConfiguration);
            }

            return client;
        }

        private void Register<T>(
            ILightLayout layout,
            EffectConfiguration effectConfiguration)
            where T : IMagicEvent
        {
            if (effectConfiguration.Id == null)
            {
                return;
            }

            var effect = effectFactory.Get<T>(effectConfiguration.Id);
            var performer = new EffectPerformer<T>(
                layout,
                effect);

            game.Events.Subscriptions.Subscribe<T>(
                performer.Perform);
        }

        private object BuildClientArgs(ILightClientProvider provider)
        {
            var type = provider.ConfigurationType;
            var ctor = type.GetConstructor(Array.Empty<Type>());
            return ctor.Invoke(Array.Empty<object>());
        }

        private Dictionary<string, Action<ILightLayout, EffectConfiguration>> BuildRegisters()
        {
            var register = GetType().GetMethod(nameof(Register), BindingFlags.Instance | BindingFlags.NonPublic);
            // Too much reflection here, but that's what refactoring is for :)
            return typeof(IMagicEvent).Assembly
                .GetTypes()
                .Where(t => typeof(IMagicEvent).IsAssignableFrom(t))
                .ToDictionary(
                    t => t.Name,
                    t => (Action<ILightLayout, EffectConfiguration>)register.MakeGenericMethod(t).CreateDelegate(typeof(Action<ILightLayout, EffectConfiguration>), this));
        }
    }
}
