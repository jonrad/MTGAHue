using CUE.NET.Devices.Generic.Enums;
using LightsApi;
using LightsApi.Cue;
using System.Threading.Tasks;

namespace MagicLights.LightClients
{
    public class CueLightClientProvider : AbstractLightClientProvider<CueLightClientProvider.CueConfiguration>
    {
        private readonly CueLightClientBuilder builder;

        public override string Id { get; } = "cue";

        public CueLightClientProvider()
        {
            builder = new CueLightClientBuilder();
        }

        public override Task<ILightClient> CreateAsync(CueConfiguration configuration)
        {
            return Task.FromResult<ILightClient>(
                builder.Build(
                    configuration.LeftLed,
                    configuration.RightLed,
                    configuration.TopLed,
                    configuration.BottomLed));
        }

        public override void Dispose()
        {
            builder.Dispose();
        }

        public class CueConfiguration
        {
            public CorsairLedId? LeftLed { get; set; }

            public CorsairLedId? RightLed { get; set; }

            public CorsairLedId? TopLed { get; set; }

            public CorsairLedId? BottomLed { get; set; }
        }
    }
}
