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
        private readonly object syncObject = new object();

        private readonly CancellationTokenSource stoppedSource = new CancellationTokenSource();

        private readonly StreamingHueClient hueClient;

        private readonly StreamingGroup streamingGroup;

        private ILightLayout? layout;

        private Task? updatingTask;

        public HueLightClient(
            StreamingHueClient hueClient,
            StreamingGroup streamingGroup)
        {
            this.hueClient = hueClient;
            this.streamingGroup = streamingGroup;
        }

        public Task<ILightLayout> GetLayout()
        {
            return Task.FromResult(GetLayoutSync());
        }

        private ILightLayout GetLayoutSync()
        {
            if (layout != null)
            {
                return layout;
            }

            lock (syncObject)
            {
                if (layout != null)
                {
                    return layout;
                }

                var hueLayer = streamingGroup.GetNewLayer(true);

                updatingTask = hueClient.AutoUpdate(streamingGroup, stoppedSource.Token, 50, onlySendDirtyStates: false);
                var lights = hueLayer.Select(l => new HueLight(l)).ToArray();

                return layout = new HueLightLayout(lights);
            }
        }

        public void Dispose()
        {
            stoppedSource.Cancel();
            updatingTask?.Wait();
        }
    }
}
