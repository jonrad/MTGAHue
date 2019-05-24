using Castle.Windsor;
using MTGADispatcher;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;

namespace MagicLights
{
    public class DemoInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IMagicService>().ImplementedBy<Demo>().IsDefault());
        }
    }
}
