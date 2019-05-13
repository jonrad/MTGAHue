using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using LightsApi;
using Castle.Facilities.Startable;

namespace MTGAHue
{
    public class LightsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ILightClientFactory>()
                    .ImplementedBy<CompositeLightClientFactory>().IsDefault(),
                Component.For<ILightClient>()
                    .UsingFactoryMethod(k => k.Resolve<ILightClientFactory>().Create().Result),
                Component.For<ILightLayout>().UsingFactoryMethod(k => k.Resolve<ILightClient>().GetLayout()));
        }
    }
}
