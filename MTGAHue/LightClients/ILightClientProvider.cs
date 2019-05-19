using LightsApi;
using System;
using System.Threading.Tasks;

namespace MTGAHue.LightClients
{
    public interface ILightClientProvider : IDisposable
    {
        string Name { get; }

        Task<ILightClient> Create();
    }
}
