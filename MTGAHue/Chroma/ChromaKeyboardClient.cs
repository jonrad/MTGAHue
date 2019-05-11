using Colore;
using Colore.Effects.Keyboard;
using LightsApi;
using LightsApi.Transitions;
using System.Threading;
using System.Threading.Tasks;

namespace MTGAHue.Chroma
{
    public class KeyboardLayout : ILightLayout
    {
        private readonly IKeyboard keyboard;

        public KeyboardLayout(IKeyboard keyboard)
        {
            this.keyboard = keyboard;
        }

        public Task Transition(ITransition transition, CancellationToken token)
        {
            return Task.FromResult(0):
            /*keyboard.SetCustomAsync(
                new KeyboardCustom)*/
        }

        private class KeyboardPosition
        {
            public KeyboardPosition(int column, int row, double x, double y)
            {
            }
        }
    }

    public class ChromaKeyboardClient : ILightClient
    {
        private IChroma chroma;

        public ILightLayout GetLayout()
        {
            return new KeyboardLayout(chroma.Keyboard);
        }

        public async Task Start(CancellationToken token)
        {
            chroma = await ColoreProvider.CreateNativeAsync();
        }

        public Task Stop(CancellationToken token)
        {
            return Task.FromResult(0):
        }
    }
}
