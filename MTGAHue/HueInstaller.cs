using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using MTGAHue.LightClients;

namespace MTGAHue
{
    public class HueInstaller : IWindsorInstaller
    {
        private readonly string entertainmentGroupName;

        public HueInstaller(string entertainmentGroupName = null)
        {
            this.entertainmentGroupName = entertainmentGroupName;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ILightClientFactory>()
                    .ImplementedBy<HueLightClientFactory>()
                    .DependsOn(Dependency.OnValue<string>(entertainmentGroupName)));
        }
    }
}
