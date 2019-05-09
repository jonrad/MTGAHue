using System.Threading;
using System.Threading.Tasks;

namespace MTGAHue
{
    public interface ILightLayout
    {
        Task Transition(ITransition transition, CancellationToken token);
    }
}
