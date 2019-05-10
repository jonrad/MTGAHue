using LightsApi.Transitions;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi
{
    public interface ILightLayout
    {
        Task Transition(ITransition transition, CancellationToken token);
    }
}
