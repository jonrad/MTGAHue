using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MagicLights.Configuration;
using MagicLights.Configuration.Models;
using MTGADispatcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace MagicLights.Integration
{
    public class ApplicationFixture : IDisposable
    {
        private readonly LineReaderProxy lineReader;

        public IntegrationLightClient LightClient { get; private set; }

        public ApplicationFixture()
        {
            Game = new Game();

            var installers = new List<IWindsorInstaller>();

            installers.AddRange(
                new IWindsorInstaller[]
                {
                    new MagicDispatcherInstaller(string.Empty, Game),
                    new DebuggerInstaller(),
                    new ApplicationInstaller(),
                    new HueInstaller()
                });

            installers.Add(new IntegrationInstaller());

            Container = new WindsorContainer();
            Container.Install(installers.ToArray());

            lineReader = (LineReaderProxy)Container.Resolve<ILineReader>();
            LightClient = Container.Resolve<IntegrationLightClient>();
        }

        public Game Game { get; }

        public WindsorContainer Container { get; }

        public void Dispose()
        {
        }

        public void Stop()
        {
            var application = Container.Resolve<MagicLightsApplication>();
            application.Stop();
        }

        public void Start(Config config)
        {
            var application = Container.Resolve<MagicLightsApplication>();
            var configuration = (ConfigurationProviderProxy)Container.Resolve<ILightsConfigurationProvider>();

            configuration.Save(config);

            application.Start().Wait();
        }

        public void Start()
        {
            Start(new Config
            {
                LightClients = new[]
                {
                    new LightClientConfiguration
                    {
                        Enabled = true,
                        Id = "integration",
                        Events = new[]
                        {
                            new EventConfiguration
                            {
                                Id = "CastSpell",
                                Effect = new EffectConfiguration
                                {
                                    Id = "solid"
                                }
}
                        }
                    }
                }
            });
        }

        public void WaitForGameEnd()
        {
            var manualReset = new ManualResetEvent(false);

            var action = new Action<GameEnded>(g => { manualReset.Set(); });

            Game.Events.Subscriptions.Subscribe(action);
            lineReader.AddText(@"[Client GRE]5/4/2019 1:50:05 PM: Match to X: GreToClientEvent
{
'Game': 'Over'
}");

            var succeeded = manualReset.WaitOne(TimeSpan.FromSeconds(3));
            Game.Events.Subscriptions.Unsubscribe(action);

            if (!succeeded)
            {
                throw new InvalidOperationException("Game didn't end after waiting");
            }
        }

        public void WriteContents(string resourceName)
        {
            var assembly = Assembly.GetAssembly(GetType());
            var assemblyName = assembly.GetName().Name;

            using (var stream =
                assembly.GetManifestResourceStream($"{assemblyName}.Games.{resourceName}"))
            {
                using (var reader = new StreamReader(stream))
                {
                    var contents = reader.ReadToEnd();
                    lineReader.AddText(contents);
                }
            }
        }
    }
}
