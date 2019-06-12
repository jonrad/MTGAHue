using MagicLights.Configuration.Models;
using Newtonsoft.Json;
using System.IO;

namespace MagicLights.Configuration
{
    public class FileConfigurationProvider : ILightsConfigurationProvider
    {
        private readonly string path;

        public FileConfigurationProvider(string path)
        {
            this.path = path;
        }

        public Config Get()
        {
            var text = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Config>(text);
        }

        public void Save(Config config)
        {
            var serialized = JsonConvert.SerializeObject(config);

            File.WriteAllText(path, serialized);
        }
    }
}
