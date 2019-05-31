using LightsApi.Transitions;
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

        private readonly AutoResetEvent layoutsExistEvent =
            new AutoResetEvent(false);

        private VirtualLightLayout[] layouts = new VirtualLightLayout[0];

        private readonly ILightClient lightClient;

        private readonly TimeSpan delay;

        private CancellationTokenSource cancellationTokenSource =
            new CancellationTokenSource();

        private Task? mainLoop = null;

        public LightClientLoop(ILightClient lightClient, TimeSpan? delay = null)
        {
            this.lightClient = lightClient;
            this.delay = delay ?? TimeSpan.FromMilliseconds(50);
        }

        public VirtualLightLayout AddLayout()
        {
            var layout = new VirtualLightLayout(lightClient.Lights.ToArray(), 50);

            lock (syncObject)
            {
                var newLayouts = new VirtualLightLayout[layouts.Length + 1];
                Array.Copy(layouts, newLayouts, layouts.Length);
                newLayouts[layouts.Length] = layout;
                layouts = newLayouts;
                layoutsExistEvent.Set();
            }

            return layout;
        }

        public void RemoveLayout(VirtualLightLayout layout)
        {
            lock (syncObject)
            {
                layouts = layouts.Where(l => l != layout).ToArray();
            }
        }

        public async Task Transition(ITransition transition)
        {
            var layout = AddLayout();
            await transition.Transition(layout);
            RemoveLayout(layout);
        }

        private Task MainLoop(CancellationToken token)
        {
            return Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    var currentLayouts = layouts;

                    while (currentLayouts.Length == 0)
                    {
                        layoutsExistEvent.WaitOne();
                        currentLayouts = layouts;
                    }

                    var colors = currentLayouts
                        .Select(l => l.Colors)
                        .Aggregate((colors1, colors2) =>
                        {
                            return colors1.Zip(colors2, (rgb1, rgb2) => rgb1 + rgb2).ToArray();
                        });

                    await Task.WhenAll(new[]
                    {
                        lightClient.SetColors(colors, token),
                        Task.Delay(delay, token)
                    });
                };
            });
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
