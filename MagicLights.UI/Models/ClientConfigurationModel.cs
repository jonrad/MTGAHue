using LightsApi;
using MagicLights.Configuration.Models;
using System;

namespace MagicLights.UI.Models
{
    public class ClientConfigurationModel : BaseModel
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

        public event Action ConfigurationChanged;

        public string Id => configuration.Id;

        public bool Enabled
        {
            get => configuration.Enabled;
            set
            {
                configuration.Enabled = value;
                OnPropertyChanged(nameof(Enabled));

                //yuck
                ConfigurationChanged?.Invoke();
            }
        }

        public LightClientConfiguration Configuration
        {
            get => configuration;
            set
            {
                configuration = value ?? new LightClientConfiguration
                {
                    Id = lightClientProvider.Id
                };

                Enabled = configuration.Enabled;
            }
        }
    }
}
