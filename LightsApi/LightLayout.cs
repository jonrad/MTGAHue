using LightsApi.Transitions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi
{
    public class LightLayout : ILightLayout
    {
        private CancellationTokenSource cancellationTokenSource;

        private readonly ILight[] lights;

        public LightLayout(ILight[] lights)
        {
            this.lights = lights;
        }

        public Task Transition(ITransition transition, CancellationToken token)
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }

            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

            var tasks = lights
                .Select(l => transition.Transition(l, cancellationTokenSource.Token))
                .ToArray();

            return Task.WhenAll(tasks);
        }
    }
}
