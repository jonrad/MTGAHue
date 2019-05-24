using LightsApi;
using System;
using System.Threading.Tasks;

namespace MagicLights.LightClients
{
    public abstract class AbstractLightClientProvider<TConfig> :
        ILightClientProvider
    {
        public abstract string Id { get; }

        public Type ConfigurationType { get; } = typeof(TConfig);

        Task<ILightClient> ILightClientProvider.Create(object configuration)
        {
            if (configuration is TConfig concreteConfiguration)
            {
                return CreateAsync(concreteConfiguration);
            }

            throw new ArgumentException();
        }

        public abstract Task<ILightClient> CreateAsync(TConfig configuration);

        public virtual void Dispose()
        {
        }
    }
}
