using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using LightsApi;
using LightsApi.Cue;

namespace MagicLights
{
    public class CueInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ILightClientProvider>()
                    .ImplementedBy<CueLightClientProvider>()
                    .LifestyleTransient());
        }
    }
}
