using Castle.Windsor;
using CommandLine;
using MTGADispatcher;
using MTGADispatcher.Events;
using Newtonsoft.Json.Linq;
using Q42.HueApi;
using Q42.HueApi.Models.Groups;
using Q42.HueApi.Streaming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Environment;
using LightsApi;
using Castle.MicroKernel.Registration;

namespace MTGAHue
{
    class Program
    {
        public class Options
        {
            [Option('e', "entertainment", Required = false, HelpText = "Entertainment Group Name")]
            public string EntertainmentGroupName { get; set; }

            [Option('d', "demo", Required = false, HelpText = "Run demo")]
            public bool Demo { get; set; }

            [Option('c', "chroma", Required = false, HelpText = "Use Chroma (Razer)")]
            public bool Chroma { get; set; }
        }

        static async Task Main(string[] args)
        {
            Options options = null;

            var optionsResults = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o => options = o);

            var path = MtgaOutputPath();
            var game = new Game();

            //TODO: move to container
            var clients = new List<ILightClient>();

            var installers = new List<IWindsorInstaller>();

            installers.Add(new AppInstaller(path, game));
            installers.Add(new DebuggerInstaller());

            if (options.Demo)
            {
                installers.Add(new DemoInstaller());
            }

            if (options.Chroma)
            {
                installers.Add(new ChromaInstaller());
            }

            installers.Add(new LightsInstaller());
            installers.Add(new ApplicationInstaller());

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

        private static async Task<string> GetEntertainmentGroupName(StreamingHueClient client)
        {
            var entertainmentGroups = await client.LocalHueClient.GetEntertainmentGroups();

            if (entertainmentGroups.Count == 0)
            {
                Console.Error.WriteLine("No entertainment groups found. Please set them up through the Hue app");
                Exit(1);
            }

            if (entertainmentGroups.Count == 1)
            {
                return entertainmentGroups.First().Name;
            }

            var names = entertainmentGroups.Select(e => e.Name);
            Console.Error.WriteLine("Please select entertainment group name with the -e option");
            WriteValidGroupNames(entertainmentGroups);
            Exit(1);

            throw new ArgumentException();
        }

        private static void WriteValidGroupNames(IEnumerable<Group> entertainmentGroups)
        {
            var names = entertainmentGroups.Select(e => e.Name);
            Console.Error.WriteLine($"Possible group names are: {string.Join(", ", names)}");
        }

        private static async Task<StreamingHueClient> GetClient()
        {
            Console.WriteLine("Searching for bridge...");
            var locator = new HttpBridgeLocator();
            var bridge = (await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5))).FirstOrDefault();

            if (bridge == null)
            {
                Console.Error.WriteLine("Could not find bridge! Giving up");

                Exit(1);
            }

            Console.WriteLine($"Found bridge! IP: {bridge.IpAddress}");

            var settings = await GetSettings();

            var (appKey, entertainmentKey) = (settings.Value<string>("appKey"), settings.Value<string>("entertainmentKey"));
            if (appKey == null || entertainmentKey == null)
            {
                Console.WriteLine("Looks like I don't have the proper keys");
                Console.WriteLine("Please press bridge button. I'll wait");

                (appKey, entertainmentKey) = await GenerateKeys(bridge.IpAddress);

                Console.WriteLine("App keys generated. Saving");

                settings["appKey"] = appKey;
                settings["entertainmentKey"] = entertainmentKey;

                SaveSettings(settings);
            }

            return new StreamingHueClient(bridge.IpAddress, appKey, entertainmentKey);
        }

        private static void SaveSettings(JObject settings)
        {
            var path = GetSettingsPath();

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (var stream = File.OpenWrite(path))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(settings.ToString());
                }
            }
        }

        private static async Task<(string, string)> GenerateKeys(string ipAddress)
        {
            const int retryCount = 5;

            for (var i = 0; i < retryCount; i++)
            {
                try
                {
                    var result = await LocalHueClient.RegisterAsync(ipAddress, "MTGAHue", "MTGAHue", true);

                    return (result.Username, result.StreamingClientKey);
                }
                catch (LinkButtonNotPressedException)
                {
                }

                await Task.Delay(TimeSpan.FromMilliseconds(5000));
            }

            Console.WriteLine("Couldn't find bridge. Ginving up");
            Exit(1);

            return (null, null);
        }

        //TODO make this strongly typed
        private static async Task<JObject> GetSettings()
        {
            var path = GetSettingsPath();
            if (!File.Exists(path))
            {
                return new JObject();
            }

            return JObject.Parse(await File.ReadAllTextAsync(path));
        }

        private static string GetSettingsPath()
        {
            return Path.Combine(
                GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify),
                "MTGAHue",
                "settings.json");
        }
    }
}
