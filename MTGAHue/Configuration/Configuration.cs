using MTGAHue.LightClients;
using System.Collections.Generic;

namespace MTGAHue.Configuration
{
    public class Configuration
    {
        public Configuration(
            ILightClientProvider[] lightClientProviders)
        {
            LightClientProviders = lightClientProviders;
        }

        public IEnumerable<ILightClientProvider> LightClientProviders { get; }
    }
}
