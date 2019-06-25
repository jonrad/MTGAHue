using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MagicLights.UI2;
using MTGADispatcher;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Environment;

namespace MagicLights.UI
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Game game = new Game();

            var path = MtgaOutputPath();

            var installers = new List<IWindsorInstaller>();

            installers.AddRange(
                new IWindsorInstaller[]
                {
                    new MagicDispatcherInstaller(path, game),
                    new DebuggerInstaller(),
                    new ApplicationInstaller()
                });

            installers.Add(new HueInstaller());
            installers.Add(new ChromaInstaller());
            installers.Add(new CueInstaller());

            installers.Add(new UiInstaller());

            using (var container = new WindsorContainer())
            {
                container.Install(installers.ToArray());

                var application = container.Resolve<IMagicLights>();

                var _ = application.Start();

                var model = container.Resolve<ConfigurationFormModel>();

                var ui = new App(model);
                ui.Run();
            }
        }

        private static string MtgaOutputPath()
        {
            var localPath = GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify);
            return Path.Combine(
                Path.GetDirectoryName(localPath),
                "LocalLow",
                "Wizards Of The Coast",
                "MTGA",
                "output_log.txt");
        }
    }
}
