using LightsApi;
using LightsApi.Hue;
using Newtonsoft.Json.Linq;
using Q42.HueApi;
using Q42.HueApi.Streaming;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Environment;

namespace MTGAHue.LightClients
{
    public class HueLightClientFactory : ILightClientFactory
    {
        private StreamingHueClient? hueClient;

        private HueLightClient? lightClient;

        private string? entertainmentGroupName;

        public HueLightClientFactory(string? entertainmentGroupName = null)
        {
            this.entertainmentGroupName = entertainmentGroupName;
        }

        public async Task<ILightClient> Create()
        {
            if (lightClient != null)
            {
                return lightClient;
            }

            hueClient = await GetClient();
            if (entertainmentGroupName == null)
            {
                entertainmentGroupName = await GetEntertainmentGroupName(hueClient);
            }

            return lightClient = new HueLightClient(hueClient, entertainmentGroupName);
        }

        public void Dispose()
        {
            lightClient?.Dispose();
            hueClient?.Dispose();
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

            throw new InvalidOperationException("Couldn't find bridge. Giving up");
        }

        //TODO make this strongly typed. And move this into a shared space
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
    }
}
