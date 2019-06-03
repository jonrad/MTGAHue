using LightsApi.Transitions;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi
{
    public interface ILayer
    {
        RGB[] Colors { get; }

        Task Transition(ITransition transition, CancellationToken token = default);
    }
}
