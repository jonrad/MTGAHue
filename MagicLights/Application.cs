using MTGADispatcher;
using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using MagicLights.Configuration.Models;

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

        public async Task Run()
        {
            var text = File.ReadAllText("config.json");
            var config = JsonConvert.DeserializeObject<Config>(text);

            await lightsSetup.Start(config);

            magicService.Start();

            Console.WriteLine("Press enter to quit");
            Console.ReadLine();
        }
    }
}
