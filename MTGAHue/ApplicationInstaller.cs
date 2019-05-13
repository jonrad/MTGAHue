using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using MTGADispatcher;
using Castle.Facilities.Startable;

namespace MTGAHue
{
    public class ApplicationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IMagicService>()
                    .ImplementedBy<HueSpellFlasher>()
                    .StartUsingMethod(s => s.Start)
                    .Start(),
                Component.For<Application>());
        }
    }
}
