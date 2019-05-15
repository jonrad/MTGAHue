using System.Threading;
using System.Threading.Tasks;

namespace LightsApi
{
    public interface ILightClient
    {
        Task Start(CancellationToken token);

        Task Stop(CancellationToken token);

        ILightLayout GetLayout();
    }
}
