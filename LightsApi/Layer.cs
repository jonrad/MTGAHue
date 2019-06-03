using LightsApi.Injectables;
using LightsApi.Transitions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi
{
    public class Layer : ILayer
    {
        private readonly IStopwatchBuilder stopwatchBuilder;

        private readonly IDelay delay;

        private readonly Position[] positions;

        private readonly int msPerTransition;

        public Layer(
            IStopwatchBuilder stopwatchBuilder,
            IDelay delay,
            Position[] positions,
            TimeSpan transitionDelay)
        {
            this.stopwatchBuilder = stopwatchBuilder;
            this.delay = delay;
            this.positions = positions;
            msPerTransition = (int)transitionDelay.TotalMilliseconds;
            Colors = positions.Select(_ => RGB.Black).ToArray();
        }

        public Layer(Position[] positions, TimeSpan transitionDelay)
            : this(new StopwatchBuilder(), new Delay(), positions, transitionDelay)
        {
        }

        public RGB[] Colors { get; private set; }

        public async Task Transition(ITransition transition, CancellationToken token = default)
        {
            var totalMilliseconds = transition.TotalLength.TotalMilliseconds;

            var stopwatch = stopwatchBuilder.StartNew();

            long elapsed = 0;
            while (!token.IsCancellationRequested && (elapsed = stopwatch.ElapsedMilliseconds) < totalMilliseconds)
            {
                Colors = GetColors(transition, elapsed);

                var delayMs = Math.Min(msPerTransition, totalMilliseconds - elapsed);

                await delay.Wait(TimeSpan.FromMilliseconds(delayMs), token);
            }

            if (!token.IsCancellationRequested)
            {
                Colors = GetColors(transition, elapsed);
            }
        }

        private RGB[] GetColors(ITransition transition, long ms)
        {
            var nextColors = new RGB[positions.Length];

            for (var i = 0; i < nextColors.Length; i++)
            {
                var position = positions[i];
                nextColors[i] = transition.Get(position.X, position.Y, ms);
            }

            return nextColors;
        }
    }
}
