using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace MTGADispatcher.Integration
{
    public class IntegrationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IGameUpdater>().ImplementedBy<GameEndedBlockHandler>());
        }
    }
}
