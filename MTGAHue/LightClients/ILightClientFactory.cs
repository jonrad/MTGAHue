using LightsApi;
using System;
using System.Threading.Tasks;

namespace MTGAHue.LightClients
{
    public interface ILightClientFactory : IDisposable
    {
        Task<ILightClient> Create();
    }
}
