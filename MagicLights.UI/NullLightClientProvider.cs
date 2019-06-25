using LightsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MagicLights.UI
{
    public class NullMagicLights : IMagicLights
    {
        public Task Start()
        {
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            return Task.CompletedTask;
        }
    }

    public class NullLightClientProvider : ILightClientProvider
    {
        public string Id => "UiProxy";

        public Type ConfigurationType => throw new NotImplementedException();

        public Task<ILightClient> Create(object configuration)
        {
            return Task.FromResult<ILightClient>(new NullLightClient());
        }

        public void Dispose()
        {
        }

        public class Configuration
        {
            public int Width { get; set; }

            public int Height { get; set; }
        }

        public class NullLightClient : ILightClient
        {
            public IEnumerable<Position> Lights => Enumerable.Empty<Position>();

            public Task SetColors(IEnumerable<RGB> colors, CancellationToken token)
            {
                return Task.CompletedTask;
            }
        }
    }
}
