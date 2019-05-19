using Castle.Windsor;
using CommandLine;
using MTGADispatcher;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;

using static System.Environment;
using System;
using MTGAHue.Configuration.Serialization;
using LightsApi;
using System.Globalization;

namespace MTGAHue
{
    class Program
    {
        public class Options
        {
            [Option('e', "entertainment", Required = false, HelpText = "Entertainment Group Name (Hue Only)")]
            public string? EntertainmentGroupName { get; set; }

            [Option('d', "demo", Required = false, HelpText = "Run demo")]
            public bool Demo { get; set; }

            [Option('c', "chroma", Required = false, HelpText = "Use Chroma (Razer)")]
            public bool Chroma { get; set; }

            [Option('h', "hue", Required = false, HelpText = "Use Hue")]
            public bool Hue { get; set; }
        }

        static async Task Main(string[] args)
        {
            Options? options = null;

            var optionsResults = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o => options = o);

            if (options == null)
            {
                return;
            }

            if (!options.Chroma && !options.Hue)
            {
                Console.Error.WriteLine(
                    "WARNING! You did not specify either [-h] Hue and/or [-c] Chroma");

                Console.Error.WriteLine(
                    "Running in Debug mode. This is boring, it will only print things to the console");
            }

            var path = MtgaOutputPath();
            var game = new Game();

            var installers = new List<IWindsorInstaller>();

            installers.AddRange(
                new IWindsorInstaller[]
                {
                    new MagicDispatcherInstaller(path, game),
                    new DebuggerInstaller(),
                    new ApplicationInstaller()
                });

            if (options.Demo)
            {
                installers.Add(new DemoInstaller());
            }

            if (options.Hue)
            {
                installers.Add(new HueInstaller(options.EntertainmentGroupName));
            }

            if (options.Chroma)
            {
                installers.Add(new ChromaInstaller());
            }

            using (var container = new WindsorContainer())
            {
                container.Install(installers.ToArray());

                var application = container.Resolve<Application>();

                await application.Run();
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
