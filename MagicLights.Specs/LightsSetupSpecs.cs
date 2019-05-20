using LightsApi;
using LightsApi.LightSources;
using LightsApi.Transitions;
using Machine.Fakes;
using Machine.Specifications;
using MTGADispatcher;
using MTGADispatcher.Dispatcher;
using MTGADispatcher.Events;
using MTGAHue;
using MTGAHue.Configuration.Models;
using MTGAHue.Effects;
using MTGAHue.LightClients;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MagicLights.Specs
{
    [Subject(typeof(LightsSetup))]
    class LightsSetupSpecs : WithFakes
    {
        static Config config;

        static LightsSetup subject;

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

            The<ILightClient>().WhenToldTo(l => l.GetLayout()).Return(Task.FromResult(The<ILightLayout>()));

            The<IEffect<CastSpell>>().WhenToldTo(t => t.OnMagicEvent(Param.IsAny<CastSpell>()))
                .Return(The<ITransition>());

            The<IEffectFactory>()
                .WhenToldTo(e => e.Get<CastSpell>(Param.IsAny<string>()))
                .Return(The<IEffect<CastSpell>>());

            subject = new LightsSetup(
                game = new Game(),
                new[]
                {
                    provider1,
                    provider2
                },
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
                                Config = new JArray(new[]
                                {
                                    new JObject(
                                        new object[]
                                        {
                                            new JProperty("key", "Name"),
                                            new JProperty("value", "Jon")
                                        }),
                                    new JObject(
                                        new object[]
                                        {
                                            new JProperty("key", "Age"),
                                            new JProperty("value", "42")
                                        })
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
                    //Is there a race condition here...?
                    The<IEffect<CastSpell>>().WasToldTo(e => e.OnMagicEvent(Param.IsAny<CastSpell>()));

                It told_transitioned_layout = () =>
                    The<ITransition>().WasToldTo(l => l.Transition(Param.IsAny<ILightLayout>(), Param.IsAny<CancellationToken>()));
            }
        }

        public class Args1
        {
            public string Name { get; set; }

            public int Age { get; set; }
        }
    }
}
