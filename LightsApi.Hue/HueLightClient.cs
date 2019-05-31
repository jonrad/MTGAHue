using Q42.HueApi.ColorConverters;
using Q42.HueApi.Streaming;
using Q42.HueApi.Streaming.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi.Hue
{
    public class HueLightClient : ILightClient
    {
        private readonly StreamingHueClient hueClient;

        private readonly StreamingGroup streamingGroup;

        private readonly EntertainmentLayer hueLayer;

        public IEnumerable<Position> Lights { get; private set; }

        public HueLightClient(
            StreamingHueClient hueClient,
            StreamingGroup streamingGroup)
        {
            this.hueClient = hueClient;
            this.streamingGroup = streamingGroup;
            hueLayer = streamingGroup.GetNewLayer(true);

            Lights = hueLayer.Select(l => new Position((float)l.LightLocation.X, (float)l.LightLocation.Y)).ToArray();
        }

        public Task SetColors(IEnumerable<RGB> colors, CancellationToken token)
        {
            return Task.Run(() =>
            {
                var i = 0;
                foreach (var color in colors)
                {
                    var state = new EntertainmentState();
                    state.SetRGBColor(new RGBColor((int)color.R, (int)color.G, (int)color.B));
                    state.SetBrightness(1);
                    hueLayer[i].State = state;

                    i++;
                }

                hueClient.ManualUpdate(streamingGroup);
            });
        }
    }
}
