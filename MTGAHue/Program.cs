using Castle.Windsor;
using CommandLine;
using LightsApi;
using LightsApi.LightSources;
using LightsApi.Transitions;
using MTGADispatcher;
using MTGADispatcher.Events;
using MTGAHue.Hue;
using Newtonsoft.Json.Linq;
using Q42.HueApi;
using Q42.HueApi.Models.Groups;
using Q42.HueApi.Streaming;
using Q42.HueApi.Streaming.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Environment;

namespace MTGAHue
{
    // Imagine the arena broken up into a grid, where X = -1 is the far left, X = 1 the far right
    // Y = 1 is the furthest in front (Where the opponent is), Y = -1 is the very back (Where the player is)
    // We can think of an Arena as two halves: the opponent and the player
    // The opponent we'll consider the area defined as Y > 0 and the player is Y < 0
    // Then each side has one or more colors. If we imagine the light coming from the LINE of where the player sits
    // (Eg for Opponent where Y = 1 or for the player Y = -1)
    // We can color that line based on the colors specified (For a single color it would be just a line of that color,
    // for two colors we'd split the line where the left is the first color, the right is the second color, 
    // and the middle is a mix of the two, etc). As we get closer to the middle, the brightness gets lower
    // until the middle where it's dull
    public class ArenaLightSource : ILightSource
    {
        private CompositeLightSource compositeLightSource;

        public ArenaLightSource(RGB[] opponentColors, RGB[] playerColors)
        {
            var radius = 1.2D;
            compositeLightSource = new CompositeLightSource(
                new LineLightSource(
                    new CompositeLightSource(BuildLightSources(opponentColors, 1).ToArray()),
                    1,
                    radius),
                new LineLightSource(
                    new CompositeLightSource(BuildLightSources(playerColors, -1).ToArray()),
                    -1,
                    radius));
        }

        private IEnumerable<ILightSource> BuildLightSources(RGB[] colors, double y)
        {
            var step = 2D / (colors.Length + 1);

            var radius = 2D / colors.Length;
            double position = -1;

            for (var i = 0; i < colors.Length; i++)
            {
                position += step;
                yield return new AreaLightSource(colors[i], position, y, radius);
            }
        }

        public RGB Calculate(double x, double y)
        {
            return compositeLightSource.Calculate(x, y);
        }

        private class LineLightSource : ILightSource
        {
            private readonly double y;

            private readonly double radius;

            private readonly ILightSource lightSource;

            public LineLightSource(ILightSource lightSource, double y, double radius)
            {
                this.y = y;
                this.radius = radius;
                this.lightSource = lightSource;
            }

            public RGB Calculate(double x, double y)
            {
                var multiplier = (radius - y) / radius;

                return lightSource.Calculate(x, y) * multiplier;
            }
        }
    }

    class Program
    {
        public class Options
        {
            [Option('e', "entertainment", Required = false, HelpText = "Entertainment Group Name")]
            public string EntertainmentGroupName { get; set; }

            [Option('d', "demo", Required = false, HelpText = "Run demo")]
            public bool Demo { get; set; }
        }

        static async Task Main(string[] args)
        {
            Options options = null;

            var optionsResults = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o => options = o);

            /*var stream = await ConnectHue(options.EntertainmentGroupName);

            var layer = stream.GetNewLayer(false);
            var layout = new LightLayout(layer.Select(l => (ILight)new HueLight(l)).ToArray());

            var arena = new ArenaLightSource(new[] { RGB.Red }, new[] { RGB.Blue, RGB.Green, RGB.Red });

            var transition = new LightSourceTransition(
                //new OmniLightSource(new RGB(31, 0, 60)),
                arena,
                TimeSpan.FromMilliseconds(2000));

            await layout.Transition(transition, CancellationToken.None);

            Console.WriteLine("Done");
            Console.ReadLine();
            return;*/

            var path = MtgaOutputPath();
            var game = new Game();

            var stream = await ConnectHue(options.EntertainmentGroupName);
            var spellFlasher = new HueSpellFlasher(stream);

            game.Events.Subscriptions.Subscribe<CastSpell>(Debug);
            game.Events.Subscriptions.Subscribe<CastSpell>(spellFlasher.OnCastSpell);

            if (options.Demo)
            {
                var demo = new Demo(game);

                demo.Start();
                return;
            }

            using (var container = new WindsorContainer())
            {
                container.Install(new AppInstaller(path, game));

                var service = container.Resolve<MtgaService>();

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

        private static async Task<StreamingGroup> ConnectHue(string entertainmentGroupName)
        {
            //most of this stolen from Q42 example
            var client = await GetClient();

            var entertainmentGroup = await GetEntertainmentGroup(client, entertainmentGroupName);

            var stream = new StreamingGroup(entertainmentGroup.Locations);

            await client.Connect(entertainmentGroup.Id);

            //waiting for this never returns?
            client.AutoUpdate(stream, CancellationToken.None, 50, onlySendDirtyStates: false);

            var bridgeInfo = await client.LocalHueClient.GetBridgeAsync();
            Console.WriteLine(bridgeInfo.IsStreamingActive ? "Streaming is active" : "Streaming is not active");

            return stream;
        }

        private static async Task<Group> GetEntertainmentGroup(StreamingHueClient client, string entertainmentGroupName)
        {
            var entertainmentGroups = await client.LocalHueClient.GetEntertainmentGroups();

            if (entertainmentGroups.Count == 0)
            {
                Console.Error.WriteLine("No entertainment groups found. Please set them up through the Hue app");
                Exit(1);
            }

            if (!string.IsNullOrEmpty(entertainmentGroupName))
            {
                var entertainmentGroup = entertainmentGroups.FirstOrDefault(g => g.Name == entertainmentGroupName);

                if (entertainmentGroup != null)
                {
                    return entertainmentGroup;
                }

                Console.Error.WriteLine($"Could not find entertainment group named {entertainmentGroupName}");
                WriteValidGroupNames(entertainmentGroups);
                Exit(1);
            }

            if (entertainmentGroups.Count == 1)
            {
                return entertainmentGroups.First();
            }

            var names = entertainmentGroups.Select(e => e.Name);
            Console.Error.WriteLine("Please select entertainment group name with the -e option");
            WriteValidGroupNames(entertainmentGroups);
            Exit(1);

            throw new InvalidOperationException();
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

        private static void Debug(CastSpell obj)
        {
            Console.WriteLine($"Cast Spell with Colors: {string.Join(" ", obj.Instance.Colors)}");
        }
    }
}
