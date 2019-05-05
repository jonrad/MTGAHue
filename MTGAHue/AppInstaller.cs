using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MTGADispatcher;

namespace MTGAHue
{
    public class AppInstaller : IWindsorInstaller
    {
        private readonly string filePath;

        private readonly Game game;

        public AppInstaller(string filePath, Game game)
        {
            this.filePath = filePath;
            this.game = game;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));

            container.Register(
                Component.For<MtgaService>(),
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
                    .ImplementedBy<BlockProcessor>()
                    .DependsOn(Dependency.OnValue<Game>(game)),
                Classes.FromAssemblyContaining<IGameUpdater>()
                    .BasedOn<IGameUpdater>()
                    .WithService.FromInterface());
        }
    }
}
