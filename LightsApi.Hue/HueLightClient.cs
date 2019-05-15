using Q42.HueApi.Streaming;
using Q42.HueApi.Streaming.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi.Hue
{
    public class HueLightClient : ILightClient, IDisposable
    {
        private readonly CancellationTokenSource stoppedSource = new CancellationTokenSource();

        private readonly StreamingHueClient hueClient;

        private readonly string entertainmentGroupName;

        private StreamingGroup streamingGroup;

        private HueLight[] lights;

        private Task updatingTask;

        public HueLightClient(StreamingHueClient hueClient, string entertainmentGroupName)
        {
            this.hueClient = hueClient;
            this.entertainmentGroupName = entertainmentGroupName;
        }

        public ILightLayout GetLayout()
        {
            return new HueLightLayout(lights);
        }

        public async Task Start(CancellationToken token)
        {
            var entertainmentGroups = await hueClient.LocalHueClient.GetEntertainmentGroups();
            var entertainmentGroup = entertainmentGroups.FirstOrDefault(g => g.Name == entertainmentGroupName);
            if (entertainmentGroup == null)
            {
                throw new ArgumentException($"Cannot find entertainment group {entertainmentGroupName}");
            }

            streamingGroup = new StreamingGroup(entertainmentGroup.Locations);

            Console.WriteLine("Attempting to connect to entertainment group");
            await hueClient.Connect(entertainmentGroup.Id).ConfigureAwait(false);
            Console.WriteLine("Connected");

            var layer = streamingGroup.GetNewLayer(true);
            lights = layer.Select(l => new HueLight(l)).ToArray();

            updatingTask = hueClient.AutoUpdate(streamingGroup, stoppedSource.Token, 50, onlySendDirtyStates: false);
        }

        public Task Stop(CancellationToken token)
        {
            stoppedSource.Cancel();

            return updatingTask;
        }

        public void Dispose()
        {
            Stop(CancellationToken.None)?.Wait();
        }
    }
}
