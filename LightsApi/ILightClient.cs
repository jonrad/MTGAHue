using System.Threading;
using System.Threading.Tasks;

namespace LightsApi
{
    public interface ILightClient
    {
        Task Start(CancellationToken token = default);

        Task Stop(CancellationToken token = default);

        ILightLayout GetLayout();
    }
}
