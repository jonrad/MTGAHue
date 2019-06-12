using MagicLights.Configuration.Models;

namespace MagicLights.Configuration
{
    public interface ILightsConfigurationProvider
    {
        Config Get();

        void Save(Config config);
    }
}
