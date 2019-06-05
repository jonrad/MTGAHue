using LightsApi;
using LightsApi.LightSources;
using LightsApi.Transitions;
using MagicLights.Configuration.Models;
using MTGADispatcher;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace MagicLights
{
    public class Application
    {
        private readonly LightsSetup lightsSetup;

        private readonly IMagicService magicService;

        public Application(
            LightsSetup lightsSetup,
            IMagicService magicService)
        {
            this.lightsSetup = lightsSetup;
            this.magicService = magicService;
        }

        public async Task Run(Config config)
        {
            var lights = await lightsSetup.Start(config);

            foreach (var light in lights)
            {
                var layer = light.AddLayer();
            }

            magicService.Start();
        }

        public Task Run()
        {
            var text = File.ReadAllText("config.json");
            var config = JsonConvert.DeserializeObject<Config>(text);

            return Run(config);
        }

        public void Stop()
        {
            magicService.Stop();
        }
    }
}
