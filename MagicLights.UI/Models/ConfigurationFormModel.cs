using LightsApi;
using MagicLights.Configuration;
using MagicLights.Configuration.Models;
using MagicLights.UI.Designer;
using MTGADispatcher;
using MTGADispatcher.Events;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MagicLights.UI.Models
{
    public class ConfigurationFormModel : BaseModel
    {
        private readonly ILightsConfigurationProvider configurationProvider;

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
            SaveCommand = new AsyncRelayCommand(Save);
            ResetCommand = new RelayCommand(Reset);
            CastSpellCommand = new RelayCommand(CastSpell);

            this.configurationProvider = configurationProvider;
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

        public ICommand CastSpellCommand { get; }

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

        public async Task Save()
        {
            configurationProvider.Save(configuration);

            IsDirty = false;

            await magicLights.Stop();
            await magicLights
                .Start()
                .ContinueWith(__ =>
                {
                    CastSpell();
                });
        }

        public void CastSpell()
        {
            game.Events.Dispatch(new CastSpell(
                new Instance(1, 1, 1, new[]
                {
                    MagicColor.Red,
                    MagicColor.Blue
                })));
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

            //TODO this is too hacky
            configuration.LightClients = Configurations.Select(c => c.Configuration).ToArray();

            IsDirty = false;
        }
    }
}
