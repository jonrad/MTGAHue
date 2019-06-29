using System.Threading.Tasks;

namespace MagicLights.UI.Designer
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
}
