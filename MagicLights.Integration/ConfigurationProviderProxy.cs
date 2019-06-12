using MagicLights.Configuration;
using MagicLights.Configuration.Models;

namespace MagicLights.Integration
{
    public class ConfigurationProviderProxy : ILightsConfigurationProvider
    {
        private Config config;

        public ConfigurationProviderProxy()
        {
            config = new Config();
        }

        public Config Get()
        {
            return config;
        }

        public void Save(Config config)
        {
            this.config = config;
        }
    }
}
