﻿using LightsApi;
using MagicLights.Configuration;
using MagicLights.Configuration.Models;
using MTGADispatcher;
using MTGADispatcher.Events;
using System;
using System.Linq;
using System.Windows.Input;

namespace MagicLights.UI2
{
    public class ConfigurationFormModel : Model
    {
        private readonly ILightsConfigurationProvider configurationProvider;

        private readonly ILightClientProvider[] lightClientProviders;

        private readonly IMagicLights magicLights;

        private readonly Game game;

        private Config configuration;

        private bool isDirty;

        public ConfigurationFormModel()
            : this(
                  new NullConfigurationProvider(),
                  new ILightClientProvider[] { new NullLightClientProvider() },
                  new NullMagicLights(),
                  new Game())
        {
        }

        public ConfigurationFormModel(
            ILightsConfigurationProvider configurationProvider,
            ILightClientProvider[] lightClientProviders,
            IMagicLights magicLights,
            Game game)
        {
            SaveCommand = new RelayCommand(Save);
            ResetCommand = new RelayCommand(Reset);

            this.configurationProvider = configurationProvider;
            this.lightClientProviders = lightClientProviders;
            this.magicLights = magicLights;
            this.game = game;
            Configurations = lightClientProviders.Select(l =>
                new ClientConfigurationModel(l))
                .ToArray();

            foreach (var configuration in Configurations)
            {
                configuration.ConfigurationChanged += () => IsDirty = true;
            }

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

        public string Status
        {
            get
            {
                if (IsDirty)
                {
                    return "Press Save for changed to take effect";
                }

                return "";
            }
        }

        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                isDirty = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public void Save()
        {
            configurationProvider.Save(configuration);

            IsDirty = false;

            magicLights.Stop();
            var _ = magicLights
                .Start()
                .ContinueWith(__ =>
                {
                    game.Events.Dispatch(new CastSpell(
                        new Instance(1, 1, 1, new[]
                        {
                            MagicColor.Red,
                            MagicColor.Blue
                        })));
                });
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

            IsDirty = false;
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
