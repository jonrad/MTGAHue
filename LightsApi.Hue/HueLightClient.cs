using Q42.HueApi.Streaming;
using Q42.HueApi.Streaming.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi.Hue
{
    public class HueLightClient : ILightClient
    {
        private readonly StreamingHueClient hueClient;

        private readonly string entertainmentGroupName;

        private readonly CancellationTokenSource stoppedSource = new CancellationTokenSource();

        private StreamingGroup streamingGroup;

        private HueLight[] lights;

        public HueLightClient(StreamingHueClient hueClient, string entertainmentGroupName)
        {
            this.hueClient = hueClient;
            this.entertainmentGroupName = entertainmentGroupName;
        }

        public ILightLayout GetLayout()
        {
            return new LightLayout(lights);
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

            await hueClient.Connect(entertainmentGroup.Id);

            var layer = streamingGroup.GetNewLayer(true);
            lights = layer.Select(l => new HueLight(l)).ToArray();

            //TODO
            hueClient.AutoUpdate(streamingGroup, stoppedSource.Token, 50, onlySendDirtyStates: false);
        }

        //TODO
        public Task Stop(CancellationToken token)
        {
            stoppedSource.Cancel();

            return Task.FromResult(true);
        }
    }
}
