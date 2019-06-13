﻿using LightsApi;
using LightsApi.Transitions;
using Machine.Fakes;
using Machine.Specifications;
using MagicLights.Configuration.Models;
using MagicLights.Effects;
using MagicLights.LightClients;
using MTGADispatcher;
using MTGADispatcher.Dispatcher;
using MTGADispatcher.Events;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MagicLights.Specs
{
    [Subject(typeof(LightClientManager))]
    class LightClientManagerSpecs : WithFakes
    {
        static Config config;

        static LightClientManager subject;

        static ILightClientProvider provider1;

        static ILightClientProvider provider2;

        static Game game;

        Establish context = () =>
        {
            provider1 = An<ILightClientProvider>();
            provider2 = An<ILightClientProvider>();
            provider1.WhenToldTo(p => p.Id).Return("id1");
            provider1.WhenToldTo(p => p.ConfigurationType).Return(typeof(Args1));
            provider2.WhenToldTo(p => p.Id).Return("id2");
            provider2.WhenToldTo(p => p.ConfigurationType).Return(typeof(object));

            provider1.WhenToldTo(p => p.Create(Param.IsAny<object>()))
                .Return(Task.FromResult(The<ILightClient>()));

            The<ILightClientProviderFactory>().WhenToldTo(p => p.Get())
                .Return(new[] { provider1, provider2 });

            The<IEffect<CastSpell>>().WhenToldTo(t => t.Mode)
                .Return(EffectMode.Single);

            The<IEffect<CastSpell>>().WhenToldTo(t => t.OnMagicEvent(Param.IsAny<CastSpell>()))
                .Return(The<ITransition>());

            The<IEffectFactory>()
                .WhenToldTo(e => e.Get<CastSpell>(Param.IsAny<string>(), Param.IsAny<JObject>()))
                .Return(The<IEffect<CastSpell>>());

            subject = new LightClientManager(
                game = new Game(),
                The<ILightClientProviderFactory>(),
                The<IEffectFactory>());
        };

        class when_building
        {
            Because of = () =>
                subject.Start(config).Await();

            class when_empty
            {
                Establish context = () =>
                    config = new Config();

                It did_not_call_provider1 = () =>
                    provider1.WasNotToldTo(p => p.Create(Param.IsAny<object>()));

                It did_not_call_provider2 = () =>
                    provider2.WasNotToldTo(p => p.Create(Param.IsAny<object>()));
            }

            class when_not_enabled
            {
                Establish context = () =>
                    config = new Config
                    {
                        LightClients = new[]
                        {
                        new LightClientConfiguration
                        {
                            Id = "id1",
                            Enabled = false
                        }
                        }
                    };

                It did_not_call = () =>
                    provider1.WasNotToldTo(p => p.Create(Param.IsAny<object>()));
            }
        }

        class once_built
        {
            class when_enabled_with_args
            {
                Establish context = () =>
                {
                    config = new Config
                    {
                        LightClients = new[]
                        {
                            new LightClientConfiguration
                            {
                                Id = "id1",
                                Enabled = true,
                                Config = new JObject(new[]
                                {
                                    new JProperty("Name", "Jon"),
                                    new JProperty("Age", 42)
                                }),
                                Events = new[]
                                {
                                    new EventConfiguration
                                    {
                                        Id = "CastSpell",
                                        Effect = new EffectConfiguration
                                        {
                                            Id = "bring towel"
                                        }
                                    }
                                }
                            }
                        }
                    };

                    subject.Start(config).Await();
                };

                Because of = () =>
                    game.Events.Dispatch(new CastSpell(new Instance(1, 1, 1, Array.Empty<MagicColor>())));

                It created_with_proper_args = () =>
                    provider1.WasToldTo(c => c.Create(Param<Args1>.Matches(p => p.Name == "Jon" && p.Age == 42)));

                It dispatched = () =>
                    The<IEffect<CastSpell>>().WasToldTo(e => e.OnMagicEvent(Param.IsAny<CastSpell>()));
            }
        }

        public class Args1
        {
            public string Name { get; set; }

            public int Age { get; set; }
        }
    }
}
