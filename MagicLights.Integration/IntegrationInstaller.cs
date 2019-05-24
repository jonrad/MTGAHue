using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MagicLights.LightClients;
using MTGADispatcher;
using MTGADispatcher.BlockProcessing;
using MTGADispatcher.Integration;

namespace MagicLights.Integration
{
    public class IntegrationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IntegrationLightClientProvider>().Forward<ILightClientProvider>(),
                Component.For<IGameUpdater>().ImplementedBy<GameEndedBlockHandler>(),
                Component.For<ILineReader>().ImplementedBy<LineReaderProxy>().IsDefault());
        }
    }
}
