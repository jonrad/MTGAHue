using MTGAHue.Chroma;
using System;
using System.Threading.Tasks;

namespace LightsApi.Chroma
{
    public class ChromaLightClientFactory : ILightClientFactory
    {
        public Task<ILightClient> Create()
        {
            return Task.FromResult<ILightClient>(new ChromaKeyboardClient());
        }

        public void Dispose()
        {
        }
    }
}
