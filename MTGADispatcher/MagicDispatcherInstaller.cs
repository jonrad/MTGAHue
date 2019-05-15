using Castle.Facilities.Startable;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MTGADispatcher.BlockProcessing;
using MTGADispatcher.Dispatcher;

namespace MTGADispatcher
{
    public class MagicDispatcherInstaller : IWindsorInstaller
    {
        private readonly string filePath;

        private readonly Game game;

        public MagicDispatcherInstaller(string filePath, Game game)
        {
            this.filePath = filePath;
            this.game = game;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));

            container.AddFacility<StartableFacility>();

            container.Register(
                Component.For<Game>().Instance(game),
                Component.For<IMagicService>()
                    .ImplementedBy<MtgaService>(),
                Component.For(typeof(IDispatcher<>))
                    .ImplementedBy(typeof(Dispatcher<>)),

                Component.For<IInstanceBuilder>()
                    .ImplementedBy<InstanceBuilder>(),

                Component.For<IBlockBuilder>()
                    .ImplementedBy<BlockBuilder>(),
                Component.For<IBlockDispatcher>()
                    .ImplementedBy<BlockDispatcher>(),
                Component.For<ILineReader>()
                    .ImplementedBy<FileLineReader>()
                    .DependsOn(Dependency.OnValue<string>(filePath)),
                Component.For<IBlockProcessor>()
                    .ImplementedBy<BlockProcessor>(),
                Classes.FromAssemblyContaining<IGameUpdater>()
                    .BasedOn<IGameUpdater>()
                    .WithService.FromInterface());
        }
    }
}
