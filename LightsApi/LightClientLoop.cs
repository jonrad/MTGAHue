using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi
{
    public class LightClientLoop
    {
        private readonly object syncObject = new object();

        private VirtualLightLayout[] layouts = new VirtualLightLayout[0];

        private readonly Func<IEnumerable<RGB>, CancellationToken, Task> setColors;

        private readonly TimeSpan delay;

        private CancellationTokenSource cancellationTokenSource =
            new CancellationTokenSource();

        private Task? mainLoop = null;

        public LightClientLoop(Func<IEnumerable<RGB>, CancellationToken, Task> setColors, TimeSpan? delay = null)
        {
            this.setColors = setColors;
            this.delay = delay ?? TimeSpan.FromMilliseconds(50);
        }

        public void AddLayout(VirtualLightLayout layout)
        {
            lock (syncObject)
            {
                var newLayouts = new VirtualLightLayout[layouts.Length + 1];
                Array.Copy(layouts, newLayouts, layouts.Length);
                newLayouts[layouts.Length] = layout;
                layouts = newLayouts;
            }
        }

        private async Task MainLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (layouts.Length == 0)
                {
                    Console.WriteLine("No layouts");
                    await Task.Delay(TimeSpan.FromMilliseconds(200), token);
                    continue;
                }

                var colors = layouts
                    .Select(l => l.Colors)
                    .Aggregate((colors1, colors2) =>
                    {
                        return colors1.Zip(colors2, (rgb1, rgb2) => rgb1 + rgb2).ToArray();
                    });

                await Task.WhenAll(new[]
                {
                    setColors(colors, token),
                    Task.Delay(delay, token)
                });
            }
        }

        public void Start()
        {
            if (mainLoop == null)
            {
                mainLoop = MainLoop(cancellationTokenSource.Token);
            }
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            mainLoop = null;
        }
    }
}
