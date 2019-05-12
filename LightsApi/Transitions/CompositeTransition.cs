using System.Threading;
using System.Threading.Tasks;

namespace LightsApi.Transitions
{
    public class CompositeTransition : ITransition
    {
        private readonly ITransition[] transitions;

        public CompositeTransition(params ITransition[] transitions)
        {
            this.transitions = transitions;
        }

        public async Task Transition(ILightLayout lightLayout, CancellationToken token = default)
        {
            foreach (var transition in transitions)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                await transition.Transition(lightLayout, token);
            }
        }
    }
}
