using System;
using System.Threading.Tasks;

namespace LightsApi
{
    public interface ILightClientProvider : IDisposable
    {
        string Id { get; }

        Type ConfigurationType { get; }

        Task<ILightClient> Create(object configuration);
    }
}
