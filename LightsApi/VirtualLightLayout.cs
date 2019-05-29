using LightsApi.LightSources;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi
{
    public class VirtualLightLayout : ILightLayout
    {
        private readonly Position[] positions;

        private readonly int msPerTransition;

        public VirtualLightLayout(Position[] positions, int msPerTransition)
        {
            this.positions = positions;
            this.msPerTransition = msPerTransition;
            Colors = positions.Select(_ => RGB.Black).ToArray();
        }

        public RGB[] Colors { get; private set; }

        public async Task Transition(ILightSource lightSource, TimeSpan timeSpan, CancellationToken token = default)
        {
            var totalMilliseconds = timeSpan.TotalMilliseconds;

            var colorCount = Colors.Length;

            var startColors = Colors;

            var endingColors = new RGB[colorCount];

            for (var i = 0; i < endingColors.Length; i++)
            {
                var position = positions[i];
                endingColors[i] = lightSource.Calculate(position.X, position.Y);
            }

            var stopwatch = Stopwatch.StartNew();

            long elapsed;
            while (!token.IsCancellationRequested && (elapsed = stopwatch.ElapsedMilliseconds) < totalMilliseconds)
            {

                var nextColors = new RGB[colorCount];
                var percentage = (float)(elapsed / totalMilliseconds);

                for (var i = 0; i < nextColors.Length; i++)
                {
                    var start = startColors[i];
                    var end = endingColors[i];

                    var rgb = new RGB(
                        start.R + (end.R - start.R) * percentage,
                        start.G + (end.G - start.G) * percentage,
                        start.B + (end.B - start.B) * percentage);

                    nextColors[i] = rgb;
                }

                var delay = Math.Min(msPerTransition, totalMilliseconds - elapsed);

                Colors = nextColors;

                await Task.Delay(TimeSpan.FromMilliseconds(delay), token);
            }

            Colors = endingColors;
        }
    }
}
