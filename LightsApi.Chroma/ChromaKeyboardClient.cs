using Colore;
using LightsApi.LightSources;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi.Chroma
{
    public class ChromaKeyboardClient : ILightClient
    {
        private IChroma chroma;

        public ILightLayout GetLayout()
        {
            return new KeyboardLayout(chroma.Keyboard);
        }

        public async Task Start(CancellationToken token = default)
        {
            chroma = await ColoreProvider.CreateNativeAsync();
        }

        public Task Stop(CancellationToken token)
        {
            return Task.FromResult(0);
        }
    }
}
