using LightsApi;
using LightsApi.Hue;
using Newtonsoft.Json.Linq;
using Q42.HueApi;
using Q42.HueApi.Streaming;
using Q42.HueApi.Streaming.Models;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Environment;

namespace MagicLights.LightClients
{
    public class HueLightClientProvider :
        AbstractLightClientProvider<HueLightClientProvider.Configuration>
    {
        private readonly Lazy<Task<StreamingHueClient>> buildClient =
            new Lazy<Task<StreamingHueClient>>(() => GetClient());

        private readonly SemaphoreSlim connectEntertainmentGroupEvent =
            new SemaphoreSlim(1);

        private ConcurrentDictionary<string, ILightClient> lightClients =
            new ConcurrentDictionary<string, ILightClient>();

        private StreamingHueClient? hueClient;

        public override string Id { get; } = "hue";

        public override async Task<ILightClient> CreateAsync(Configuration configuration)
        {
            var entertainmentGroupName = configuration.EntertainmentGroup;

            if (hueClient == null)
            {
                hueClient = await buildClient.Value;
            }

            if (entertainmentGroupName == null)
            {
                entertainmentGroupName = await GetEntertainmentGroupName(hueClient);

                if (entertainmentGroupName == null)
                {
                    throw new InvalidOperationException("Must set entertainment group name");
                }
            }

            if (lightClients.TryGetValue(entertainmentGroupName, out var client))
            {
                return client;
            }

            try
            {
                await connectEntertainmentGroupEvent.WaitAsync();

                if (lightClients.TryGetValue(entertainmentGroupName, out client))
                {
                    return client;
                }

                return lightClients[entertainmentGroupName] =
                    await Connect(hueClient, entertainmentGroupName);
            }
            finally
            {
                connectEntertainmentGroupEvent.Release();
            }
        }

        private async Task<HueLightClient> Connect(StreamingHueClient hueClient, string entertainmentGroupName)
        {
            var entertainmentGroups = await hueClient.LocalHueClient.GetEntertainmentGroups();
            var entertainmentGroup = entertainmentGroups.FirstOrDefault(g => g.Name == entertainmentGroupName);

            if (entertainmentGroup == null)
            {
                throw new ArgumentException($"Cannot find entertainment group {entertainmentGroupName}");
            }

            var streamingGroup = new StreamingGroup(entertainmentGroup.Locations);

            Console.WriteLine("Attempting to connect to entertainment group");
            await hueClient.Connect(entertainmentGroup.Id).ConfigureAwait(false);
            Console.WriteLine("Connected");

            return new HueLightClient(hueClient, streamingGroup);
        }

        public override void Dispose()
        {
            base.Dispose();

            hueClient?.Dispose();

            hueClient = null;
        }

        private static async Task<StreamingHueClient> GetClient()
        {
            Console.WriteLine("Searching for bridge...");
            var locator = new HttpBridgeLocator();
            var bridge = (await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5))).FirstOrDefault();

            if (bridge == null)
            {
                throw new InvalidOperationException("Could not find bridge! Giving up");
            }

            Console.WriteLine($"Found bridge! IP: {bridge.IpAddress}");

            var settings = GetSettings();

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
                    var result = await LocalHueClient.RegisterAsync(ipAddress, "MagicLights", "MagicLights", true);

                    return (result.Username, result.StreamingClientKey);
                }
                catch (LinkButtonNotPressedException)
                {
                }

                await Task.Delay(TimeSpan.FromMilliseconds(5000));
            }

            throw new InvalidOperationException("Couldn't find bridge. Giving up");
        }

        //TODO make this strongly typed. And move this into a shared space
        private static JObject GetSettings()
        {
            var path = GetSettingsPath();
            if (!File.Exists(path))
            {
                return new JObject();
            }

            return JObject.Parse(File.ReadAllText(path));
        }

        private static string GetSettingsPath()
        {
            return Path.Combine(
                GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify),
                "MTGAHue",
                "settings.json");
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

            throw new InvalidOperationException(
                "Please select entertainment group name with the -e option");
        }

        public class Configuration
        {
            public string? EntertainmentGroup { get; set; }
        }
    }
}
