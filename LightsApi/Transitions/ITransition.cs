using System.Threading;
using System.Threading.Tasks;

namespace LightsApi.Transitions
{
    public interface ITransition
    {
        Task Transition(ILightLayout lightLayout, CancellationToken token = default);
    }
}
