using Castle.Windsor;
using MTGADispatcher;
using MTGADispatcher.Events;
using Newtonsoft.Json.Linq;
using Q42.HueApi;
using Q42.HueApi.Streaming;
using Q42.HueApi.Streaming.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Environment;

namespace MTGAHue
{
    class Program
    {
        static async Task Main()
        {
            var path = MtgaOutputPath();
            var game = new Game();

            using (var container = new WindsorContainer())
            {
                container.Install(new AppInstaller(path, game));

                var service = container.Resolve<MtgaService>();

                var stream = await ConnectHue();
                var spellFlasher = new HueSpellFlasher(stream);

                game.Events.Subscriptions.Subscribe<CastSpell>(Debug);
                game.Events.Subscriptions.Subscribe<CastSpell>(spellFlasher.OnCastSpell);

                service.Start();

                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
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

        private static async Task<StreamingGroup> ConnectHue()
        {
            //most of this stolen from Q42 example
            var client = await GetClient();

            var all = await client.LocalHueClient.GetEntertainmentGroups();
            var group = all.FirstOrDefault();

            if (group == null)
            {
                Console.Error.WriteLine("Couldn't find any entertainment groups. Giving up");
                Exit(1);
            }

            var stream = new StreamingGroup(group.Locations);

            await client.Connect(group.Id);

            //waiting for this never returns?
            client.AutoUpdate(stream, CancellationToken.None, 50, onlySendDirtyStates: false);

            var bridgeInfo = await client.LocalHueClient.GetBridgeAsync();
            Console.WriteLine(bridgeInfo.IsStreamingActive ? "Streaming is active" : "Streaming is not active");

            return stream;
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

        private static void Debug(CastSpell obj)
        {
            Console.WriteLine($"Cast Spell with Colors: {string.Join(" ", obj.Instance.Colors)}");
        }
    }
}
