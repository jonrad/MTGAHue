using LightsApi;
using MagicLights.Configuration.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagicLights.UI
{
    public interface IConfigurationBuilder
    {
        IEnumerable<ConfigurationField> Get(Type type);
    }

    public class ConfigurationBuilder : IConfigurationBuilder
    {
        public IEnumerable<ConfigurationField> Get(Type type)
        {
            var properties = type.GetProperties();
            return properties.Select(
                p => new ConfigurationField(p.Name, p.PropertyType));
        }
    }

    public class ConfigurationField
    {
        public ConfigurationField(string id, Type type)
        {
            Id = id;
            Type = type;
        }

        public string Id { get; }

        public Type Type { get; }
    }

    public class ClientConfigurationModel : Model
    {
        private readonly ILightClientProvider lightClientProvider;

        private LightClientConfiguration configuration;

        public ClientConfigurationModel(ILightClientProvider lightClientProvider)
        {
            this.lightClientProvider = lightClientProvider;
            configuration = new LightClientConfiguration
            {
                Id = lightClientProvider.Id
            };
        }

        public string Id => configuration.Id!;

        public bool Enabled
        {
            get => configuration.Enabled;
            set
            {
                configuration.Enabled = value;
                OnPropertyChanged(nameof(Enabled));
            }
        }

        public LightClientConfiguration Configuration
        {
            get => configuration;
            set
            {
                configuration = value;
                Enabled = configuration.Enabled;
            }
        }
    }
}
