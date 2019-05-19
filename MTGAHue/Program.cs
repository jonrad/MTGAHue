﻿using Castle.Windsor;
using CommandLine;
using MTGADispatcher;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;

using static System.Environment;
using System;
using Newtonsoft.Json;
using MTGAHue.Configuration.Models;
using MTGAHue.LightClients;
using LightsApi;
using MTGADispatcher.Events;
using Newtonsoft.Json.Linq;
using System.Linq;

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
