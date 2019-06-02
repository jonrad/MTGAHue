using LightsApi.Injectables;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi
{
    public class Lights : ILights
    {
        private readonly object layoutSync = new object();

        private readonly AutoResetEvent layoutsExistEvent =
            new AutoResetEvent(false);

        private ILightLayout[] layouts = new ILightLayout[0];

        private readonly IDelay delayProvider;

        private readonly ILayerBuilder layerBuilder;

        private readonly ILightClient lightClient;

        private readonly TimeSpan delay;

        private CancellationTokenSource cancellationTokenSource =
            new CancellationTokenSource();

        private Task? mainLoop = null;

        public Lights(
            IDelay delayProvider,
            ILayerBuilder layerBuilder,
            ILightClient lightClient,
            TimeSpan? delay = null)
        {
            this.delayProvider = delayProvider;
            this.layerBuilder = layerBuilder;
            this.lightClient = lightClient;
            this.delay = delay ?? TimeSpan.FromMilliseconds(50);
        }

        public Lights(
            ILightClient lightClient,
            TimeSpan? delay = null)
            : this(new Delay(), new LayerBuilder(), lightClient, delay)
        {
        }

        public ILightLayout AddLayout()
        {
            var layout = layerBuilder.Build(
                lightClient.Lights.ToArray(),
                TimeSpan.FromMilliseconds(50));

            lock (layoutSync)
            {
                var newLayouts = new ILightLayout[layouts.Length + 1];
                Array.Copy(layouts, newLayouts, layouts.Length);
                newLayouts[layouts.Length] = layout;
                layouts = newLayouts;
                layoutsExistEvent.Set();
            }

            return layout;
        }

        public void RemoveLayout(ILightLayout layout)
        {
            lock (layoutSync)
            {
                //note this assumes small amount of layers
                layouts = layouts.Where(l => l != layout).ToArray();
            }
        }

        private Task MainLoop(CancellationToken token)
        {
            return Task.Factory.StartNew(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    var currentLayouts = layouts;

                    while (currentLayouts.Length == 0)
                    {
                        WaitHandle.WaitAny(new[]
                        {
                            layoutsExistEvent,
                            token.WaitHandle
                        });

                        if (token.IsCancellationRequested)
                        {
                            return;
                        }

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
                        delayProvider.Wait(delay, token)
                    });
                };
            }, TaskCreationOptions.LongRunning);
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
