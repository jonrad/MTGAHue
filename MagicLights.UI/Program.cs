using Castle.MicroKernel.Registration;
using Castle.Windsor;
using LightsApi;
using MTGADispatcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Environment;

namespace MagicLights.UI
{
    internal static class Program
    {
        [STAThread]
        private static async Task Main()
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

                var application = container.Resolve<MagicLightsApplication>();

                //await application.Start();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ConfigurationForm(
                    container.Resolve<ConfigurationFormModel>()));
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
