using Colore;
using System.Threading.Tasks;

namespace LightsApi.Chroma
{
    public class ChromaKeyboardClient : ILightClient
    {
        private readonly KeyboardLayout layout;

        public ChromaKeyboardClient(IChroma chroma, int? columnCount, int? rowCount)
        {
            layout = new KeyboardLayout(chroma.Keyboard, columnCount, rowCount);
        }

        public Task<ILightLayout> GetLayout()
        {
            return Task.FromResult<ILightLayout>(layout);
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}
