using LightsApi;
using MagicLights.Configuration;
using MagicLights.Configuration.Models;
using System;
using System.Linq;
using System.Windows.Input;

namespace MagicLights.UI2
{
    public class ConfigurationFormModel : Model
    {
        private readonly ILightsConfigurationProvider configurationProvider;

        private Config configuration;

        private readonly ILightClientProvider[] lightClientProviders;

        public ConfigurationFormModel()
            : this(
                  new NullConfigurationProvider(),
                  new ILightClientProvider[] { new NullLightClientProvider() })
        {
        }

        public ConfigurationFormModel(
            ILightsConfigurationProvider configurationProvider,
            ILightClientProvider[] lightClientProviders)
        {
            SaveCommand = new RelayCommand(Save);
            ResetCommand = new RelayCommand(Reset);

            this.configurationProvider = configurationProvider;
            this.lightClientProviders = lightClientProviders;

            Configurations = lightClientProviders.Select(l =>
                new ClientConfigurationModel(l))
                .ToArray();

            configuration = new Config();

            Reset();
        }

        public int Count
        {
            get => Configurations.Length;
        }

        public ClientConfigurationModel[] Configurations { get; }

        public ICommand SaveCommand { get; }

        public ICommand ResetCommand { get; }

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
                    clients[configurationUiModel.Id].FirstOrDefault() ?? new LightClientConfiguration
                    {
                        Id = configurationUiModel.Id
                    };
            }

            //TODO this should be part of configurationprovider maybe?
            configuration.LightClients = Configurations.Select(c => c.Configuration).ToArray();
        }

        private class NullConfigurationProvider : ILightsConfigurationProvider
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

    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;

        public RelayCommand(Action<object> execute)
        {
            this.execute = execute;
        }

        public RelayCommand(Action execute)
            : this(_ => execute())
        {
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
