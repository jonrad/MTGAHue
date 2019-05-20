using Colore;
using System.Threading.Tasks;

namespace LightsApi.Chroma
{
    public class ChromaKeyboardClient : ILightClient
    {
        private readonly KeyboardLayout layout;

        public ChromaKeyboardClient(IChroma chroma)
        {
            layout = new KeyboardLayout(chroma.Keyboard);
        }

        public Task<ILightLayout> GetLayout()
        {
            return Task.FromResult<ILightLayout>(layout);
        }
    }
}
