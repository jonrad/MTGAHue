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

        private EntertainmentLayer hueLayer;

        private readonly Task updatingTask;

        public HueLightClient(
            StreamingHueClient hueClient,
            StreamingGroup streamingGroup)
        {
            hueLayer = streamingGroup.GetNewLayer(true);

            updatingTask = hueClient.AutoUpdate(streamingGroup, stoppedSource.Token, 50, onlySendDirtyStates: false);
        }

        public ILightLayout GetLayout()
        {
            var lights = hueLayer.Select(l => new HueLight(l)).ToArray();

            return new HueLightLayout(lights);
        }

        public async Task Start(CancellationToken token)
        {
        }

        public Task Stop(CancellationToken token)
        {
            stoppedSource.Cancel();

            return updatingTask ?? Task.FromResult(true);
        }

        public void Dispose()
        {
            Stop(CancellationToken.None)?.Wait();
        }
    }
}
