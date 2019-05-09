using System.Threading;
using System.Threading.Tasks;

namespace MTGAHue
{
    public interface ITransition
    {
        Task Transition(ILight light, CancellationToken token = default);
    }
}
