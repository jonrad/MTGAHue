﻿using MagicLights.Configuration;
using MagicLights.Configuration.Models;

namespace MagicLights.UI.Designer
{
    internal class NullConfigurationProvider : ILightsConfigurationProvider
    {
        public Config Get()
        {
            return new Config
            {
                LightClients = new LightClientConfiguration[0]
            };
        }

        public void Save(Config config)
        {
        }
    }
}
