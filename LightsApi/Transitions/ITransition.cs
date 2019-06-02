using System.Threading;
using System.Threading.Tasks;

namespace LightsApi.Transitions
{
    public interface ITransition
    {
        Task Transition(ILayer layer, CancellationToken token = default);
    }
}
