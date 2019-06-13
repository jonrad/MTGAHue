using LightsApi;
using System;
using System.Threading.Tasks;

namespace MagicLights.LightClients
{
    //TODO this is a factory of factories... this ain't good
    public interface ILightClientProviderFactory : IDisposable
    {
        ILightClientProvider[] Get();
    }

    public interface ILightClientProvider : IDisposable
    {
        string Id { get; }

        Type ConfigurationType { get; }

        Task<ILightClient> Create(object configuration);
    }
}
