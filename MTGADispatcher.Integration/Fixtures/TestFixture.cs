using Castle.Windsor;
using MTGADispatcher.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace MTGADispatcher.Integration.Fixtures
{
    public class TestFixture : IDisposable
    {
        private string path;

        private StreamWriter streamWriter;

        private WindsorContainer container;

        public List<PlayLand> PlayedLands = new List<PlayLand>();

        public List<CastSpell> SpellsCast = new List<CastSpell>();

        public Game Game { get; private set; }

        public Game CreateGame()
        {
            path = Path.GetTempFileName();
            streamWriter = new StreamWriter(path);

            Game = new Game();

            container = new WindsorContainer();
            container.Install(
                new MagicDispatcherInstaller(path, Game),
                new IntegrationInstaller());

            var service = container.Resolve<IMagicService>();
            service.Start();

            Game.Events.Subscriptions.Subscribe<CastSpell>(OnCastSpell);
            Game.Events.Subscriptions.Subscribe<PlayLand>(OnPlayLand);

            return Game;
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
                    streamWriter.Write(contents);
                    streamWriter.Flush();
                }
            }
        }

        public void WaitForGameEnd()
        {
            var manualReset = new ManualResetEvent(false);

            var action = new Action<GameEnded>(g => { manualReset.Set(); });

            Game.Events.Subscriptions.Subscribe(action);
            streamWriter.WriteLine(@"[Client GRE]5/4/2019 1:50:05 PM: Match to X: GreToClientEvent
{
'Game': 'Over'
}");

            streamWriter.Flush();

            var succeeded = manualReset.WaitOne(TimeSpan.FromSeconds(3));
            Game.Events.Subscriptions.Unsubscribe(action);

            if (!succeeded)
            {
                throw new InvalidOperationException("Game didn't end after waiting");
            }
        }

        private void OnCastSpell(CastSpell spell)
        {
            SpellsCast.Add(spell);
        }

        private void OnPlayLand(PlayLand playLand)
        {
            PlayedLands.Add(playLand);
        }

        public void Dispose()
        {
            if (Game == null)
            {
                return;
            }

            container.Dispose();
            Game.Events.Subscriptions.Unsubscribe<CastSpell>(OnCastSpell);
            Game.Events.Subscriptions.Unsubscribe<PlayLand>(OnPlayLand);
            streamWriter?.Dispose();

            if (path != null && File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
