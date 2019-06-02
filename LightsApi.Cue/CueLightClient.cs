using CUE.NET.Devices.Generic;
using CUE.NET.Devices.Keyboard;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi.Cue
{
    public class CueLightClient : ILightClient
    {
        private LedPosition[] ledPositions;

        private CorsairKeyboard keyboard;

        internal CueLightClient(CorsairKeyboard keyboard, LedPosition[] ledPositions)
        {
            this.keyboard = keyboard;
            this.ledPositions = ledPositions;
            Lights = ledPositions.Select(k => k.Position).ToArray();
        }

        public IEnumerable<Position> Lights { get; }

        public Task SetColors(IEnumerable<RGB> colors, CancellationToken token)
        {
            return Task.Run(() =>
            {
                var i = 0;
                foreach (var color in colors)
                {
                    ledPositions[i].Led.Color = new CorsairColor(
                        (byte)color.R, (byte)color.G, (byte)color.B);

                    i++;
                }

                keyboard.Update();
            }, token);
        }
    }
}
