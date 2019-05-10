using System.Threading;
using System.Threading.Tasks;

namespace LightsApi.Transitions
{
    public interface ITransition
    {
        Task Transition(ILight light, CancellationToken token = default);
    }
}
