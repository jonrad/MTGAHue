using LightsApi;
using MTGADispatcher;
using MTGADispatcher.Events;
using MagicLights.Configuration.Models;
using MagicLights.Effects;
using MagicLights.LightClients;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MagicLights
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

        public async Task<CompositeLightClient> Start(Config configuration)
        {
            var clients = BuildClients(
                configuration.LightClients ?? new LightClientConfiguration[0]);

            return new CompositeLightClient((await clients).ToArray());
        }

        private async Task<ILightClient[]> BuildClients(
            LightClientConfiguration[] lightClients)
        {
            List<ILightClient> clients = new List<ILightClient>();

            // Why does hue hate when I'm async here...
            foreach (var l in lightClients)
            {
                if (!l.Enabled)
                {
                    continue;
                }

                clients.Add(await BuildClient(l));
            }

            return clients.ToArray();
        }

        private async Task<ILightClient> BuildClient(
            LightClientConfiguration lightClientConfiguration)
        {
            if (lightClientConfiguration.Id == null)
            {
                throw new ArgumentException("Must set id for light client");
            }

            var id = lightClientConfiguration.Id;

            if (!lightClientProviders.TryGetValue(id, out var provider))
            {
                throw new ArgumentException($"Could not find light client with id {id}");
            }

            var clientConfiguration =
                BuildClientArgs(provider, lightClientConfiguration.Config);

            var client = await provider.Create(clientConfiguration);
            var layout = await client.GetLayout();

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

            var effect = effectFactory.Get<T>(
                effectConfiguration.Id,
                effectConfiguration.Config);

            var performer = new EffectPerformer<T>(
                layout,
                effect);

            game.Events.Subscriptions.Subscribe<T>(
                performer.Perform);
        }

        private object BuildClientArgs(
            ILightClientProvider provider,
            JObject? config)
        {
            // more reflection..
            var type = provider.ConfigurationType;

            if (config == null)
            {
                var ctor = type.GetConstructor(Array.Empty<Type>());
                return ctor.Invoke(Array.Empty<object>());
            }

            return config.ToObject(type);
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
