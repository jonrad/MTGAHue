﻿using Castle.Windsor;
using CommandLine;
using MTGADispatcher;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using static System.Environment;

namespace MagicLights.Console
{
    class Program
    {
        public class Options
        {
            [Option('d', "demo", Required = false, HelpText = "Run demo")]
            public bool Demo { get; set; }
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

            if (options.Demo)
            {
                installers.Add(new DemoInstaller());
            }

            installers.Add(new HueInstaller());
            installers.Add(new ChromaInstaller());

            using (var container = new WindsorContainer())
            {
                container.Install(installers.ToArray());

                var application = container.Resolve<Application>();

                await application.Run();

                System.Console.WriteLine("Press enter to quit");
                System.Console.ReadLine();
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
