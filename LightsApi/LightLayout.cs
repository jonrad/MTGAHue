using LightsApi.Injectables;
using LightsApi.LightSources;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi
{
    public class LightLayout : ILightLayout
    {
        private readonly IStopwatchBuilder stopwatchBuilder;

        private readonly IDelay delay;

        private readonly Position[] positions;

        private readonly int msPerTransition;

        public LightLayout(
            IStopwatchBuilder stopwatchBuilder,
            IDelay delay,
            Position[] positions,
            TimeSpan transitionDelay)
        {
            this.stopwatchBuilder = stopwatchBuilder;
            this.delay = delay;
            this.positions = positions;
            this.msPerTransition = (int)transitionDelay.TotalMilliseconds;
            Colors = positions.Select(_ => RGB.Black).ToArray();
        }

        public LightLayout(Position[] positions, TimeSpan transitionDelay)
            : this(new StopwatchBuilder(), new Delay(), positions, transitionDelay)
        {
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

            var stopwatch = stopwatchBuilder.StartNew();

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

                var delayMs = Math.Min(msPerTransition, totalMilliseconds - elapsed);

                Colors = nextColors;

                await delay.Wait(TimeSpan.FromMilliseconds(delayMs), token);
            }

            if (!token.IsCancellationRequested)
            {
                Colors = endingColors;
            }
        }
    }
}
