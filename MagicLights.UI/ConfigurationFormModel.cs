using Castle.Core.Logging;
using LightsApi;
using MagicLights.Configuration;
using MagicLights.Configuration.Models;
using System;
using System.Linq;

namespace MagicLights.UI
{
    public class ConfigurationFormModel : Model
    {
        private readonly ILightsConfigurationProvider configurationProvider;

        private Config configuration;

        private readonly ILightClientProvider[] lightClientProviders;

        public ConfigurationFormModel()
            : this(
                  new FileConfigurationProvider(@"config.json"),
                  new ILightClientProvider[] { new NullLightClientProvider() })
        {
        }

        public ConfigurationFormModel(
            ILightsConfigurationProvider configurationProvider,
            ILightClientProvider[] lightClientProviders)
        {
            this.configurationProvider = configurationProvider;
            this.lightClientProviders = lightClientProviders;

            Configurations = lightClientProviders.Select(l =>
                new ClientConfigurationModel(l))
                .ToArray();

            configuration = new Config();

            Reset();
        }

        public ILogger Logger { get; set; } = NullLogger.Instance;

        public int Count
        {
            get => Configurations.Length;
        }

        public ClientConfigurationModel[] Configurations { get; }

        public void Save()
        {
            configurationProvider.Save(configuration);
        }

        public void Reset()
        {
            configuration = configurationProvider.Get();
            var clients = configuration.LightClients.ToLookup(l => l.Id);

            foreach (var configurationUiModel in Configurations)
            {
                configurationUiModel.Configuration =
                    clients[configurationUiModel.Id].FirstOrDefault();
            }
        }
    }
}
