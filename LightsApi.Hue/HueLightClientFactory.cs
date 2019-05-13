using Q42.HueApi;
using Q42.HueApi.Streaming;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LightsApi.Hue
{
    public class HueLightClientFactory : ILightClientFactory
    {
        private readonly string appKey;

        private readonly string entertainmentKey;

        private readonly string entertainmentGroup;

        private StreamingHueClient hueClient;

        private HueLightClient lightClient;

        public HueLightClientFactory(
            string appKey,
            string entertainmentKey,
            string entertainmentGroup)
        {
            this.appKey = appKey;
            this.entertainmentKey = entertainmentKey;
            this.entertainmentGroup = entertainmentGroup;
        }

        public async Task<ILightClient> Create()
        {
            if (lightClient != null)
            {
                return lightClient;
            }

            return lightClient = new HueLightClient(await GetClient(), entertainmentGroup);
        }

        public void Dispose()
        {
            lightClient?.Dispose();
            hueClient?.Dispose();
        }

        private async Task<StreamingHueClient> GetClient()
        {
            Console.WriteLine("Searching for bridge...");
            var locator = new HttpBridgeLocator();
            var bridge = (await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5))).FirstOrDefault();

            if (bridge == null)
            {
                Console.Error.WriteLine("Could not find bridge! Giving up");

                throw new InvalidOperationException();
            }

            Console.WriteLine($"Found bridge! IP: {bridge.IpAddress}");

            return new StreamingHueClient(bridge.IpAddress, appKey, entertainmentKey);
        }
    }
}
