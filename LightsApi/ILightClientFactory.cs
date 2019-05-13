using System;
using System.Linq;
using System.Threading.Tasks;

namespace LightsApi
{
    public interface ILightClientFactory : IDisposable
    {
        Task<ILightClient> Create();
    }
}
