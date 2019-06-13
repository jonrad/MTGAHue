using System;

namespace MagicLights.LightClients
{
    //TODO this is a factory of factories... this ain't good
    public interface ILightClientProviderFactory : IDisposable
    {
        ILightClientProvider[] Get();
    }
}
