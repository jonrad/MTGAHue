using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Facilities.TypedFactory;
using MTGADispatcher.Events;
using System.Reflection;
using MTGAHue.Effects;

namespace MTGAHue
{
    public class ApplicationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<TypedFactoryFacility>();

            container.Register(
                Component.For<IEffect<CastSpell>>()
                    .ImplementedBy<FlashEffect>()
                    .Named("flash"),
                Component.For<IEffect<CastSpell>>()
                    .ImplementedBy<ExplosionEffect>()
                    .Named("explosion"),

                Component.For<IEffectFactory>().AsFactory(f =>
                    f.SelectedWith(new NamedCustomSelector())),

                Component.For<LightsSetup>(),
                Component.For<Application>());
        }

        internal class NamedCustomSelector : DefaultTypedFactoryComponentSelector
        {
            protected override string GetComponentName(
                MethodInfo method,
                object[] arguments)
            {
                return (string)arguments[0];
            }
        }
    }
}
