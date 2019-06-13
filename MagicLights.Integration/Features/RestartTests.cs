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
        public void RestartHue()
        {
            var config = new Config
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
            };

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
