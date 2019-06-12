using LightsApi.Injectables;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi
{
    public class Lights : ILights
    {
        private readonly object layerSync = new object();

        private readonly AutoResetEvent layersExistEvent =
            new AutoResetEvent(false);

        private ILayer[] layers = new ILayer[0];

        private readonly IDelay delayProvider;

        private readonly ILayerBuilder layerBuilder;

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
            LightClient = lightClient;
            this.delay = delay ?? TimeSpan.FromMilliseconds(50);
        }

        public Lights(
            ILightClient lightClient,
            TimeSpan? delay = null)
            : this(new Delay(), new LayerBuilder(), lightClient, delay)
        {
        }

        public ILightClient LightClient { get; }

        public ILayer AddLayer()
        {
            var layer = layerBuilder.Build(
                LightClient.Lights.ToArray(),
                TimeSpan.FromMilliseconds(50));

            lock (layerSync)
            {
                var newLayers = new ILayer[layers.Length + 1];
                Array.Copy(layers, newLayers, layers.Length);
                newLayers[layers.Length] = layer;
                layers = newLayers;
                layersExistEvent.Set();
            }

            return layer;
        }

        public void RemoveLayer(ILayer layer)
        {
            lock (layerSync)
            {
                //note this assumes small amount of layers
                layers = layers.Where(l => l != layer).ToArray();
            }
        }

        private Task MainLoop(CancellationToken token)
        {
            return Task.Factory.StartNew(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    var currentLayers = layers;

                    while (currentLayers.Length == 0)
                    {
                        await LightClient.SetColors(
                            LightClient.Lights.Select(l => RGB.Black),
                            token);

                        WaitHandle.WaitAny(new[]
                        {
                            layersExistEvent,
                            token.WaitHandle
                        });

                        if (token.IsCancellationRequested)
                        {
                            return;
                        }

                        currentLayers = layers;
                    }

                    var colors = currentLayers
                        .Select(l => l.Colors)
                        .Aggregate((colors1, colors2) =>
                        {
                            return colors1.Zip(colors2, (rgb1, rgb2) => rgb1 + rgb2).ToArray();
                        });

                    await Task.WhenAll(new[]
                    {
                        LightClient.SetColors(colors, token),
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
            mainLoop?.Wait();
        }
    }
}
