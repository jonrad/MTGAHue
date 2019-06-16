using MagicLights.Configuration.Models;
using MTGADispatcher;
using MTGADispatcher.Events;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace MagicLights.Integration.Features
{
    public class RestartTests : BaseFeature
    {
        public RestartTests(ApplicationFixture application)
            : base(application)
        {
        }

        [RunnableInDebugOnly]
        public void RestartChroma()
        {
            RunTest(new Config
            {
                LightClients = new[]
                {
                    new LightClientConfiguration
                    {
                        Id = "chroma",
                        Enabled = true,
                        Events = new[]
                        {
                            new EventConfiguration
                            {
                                Id = "CastSpell",
                                Effect = new EffectConfiguration
                                {
                                    Id = "solid"
                                }
                            }
                        }
                    }
                }
            });
        }

        [RunnableInDebugOnly]
        public void RestartCue()
        {
            RunTest(new Config
            {
                LightClients = new[]
                {
                    new LightClientConfiguration
                    {
                        Id = "cue",
                        Enabled = true,
                        Events = new[]
                        {
                            new EventConfiguration
                            {
                                Id = "CastSpell",
                                Effect = new EffectConfiguration
                                {
                                    Id = "solid"
                                }
                            }
                        }
                    }
                }
            });
        }

        [RunnableInDebugOnly]
        public void RestartHue()
        {
            RunTest(new Config
            {
                LightClients = new[]
                {
                    new LightClientConfiguration
                    {
                        Id = "hue",
                        Enabled = true,
                        Config = new JObject
                        {
                            new JProperty("EntertainmentGroup", "Entertainment area 2")
                        },
                        Events = new[]
                        {
                            new EventConfiguration
                            {
                                Id = "CastSpell",
                                Effect = new EffectConfiguration
                                {
                                    Id = "solid"
                                }
                            }
                        }
                    }
                }
            });
        }

        private void RunTest(Config config)
        {
            application.Start(config);
            application.Game.Events.Dispatch(new CastSpell(
                new Instance(0, 0, 0, new[] { MagicColor.Red })));

            Thread.Sleep(100);
            application.Stop();

            application.Start(config);
            application.Game.Events.Dispatch(new CastSpell(
                new Instance(0, 0, 0, new[] { MagicColor.Blue })));

            Thread.Sleep(1000);

            application.Stop();
        }
    }
}
