using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MTGADispatcher.BlockProcessing;

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
