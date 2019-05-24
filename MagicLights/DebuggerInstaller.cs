using Castle.Windsor;
using MTGADispatcher;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Facilities.Startable;

namespace MagicLights
{
    public class DebuggerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<Debugger>().Start());
                    //.ImplementedBy<Debugger>()
                    //.Start());
        }
    }
}
