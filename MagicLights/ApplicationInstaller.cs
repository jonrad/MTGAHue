using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Facilities.TypedFactory;
using MTGADispatcher.Events;
using System.Reflection;
using MagicLights.Effects;
using Castle.MicroKernel;
using System;
using Newtonsoft.Json.Linq;

namespace MagicLights
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
                    .ImplementedBy<MultiColorFlash>()
                    .Named("multiflash"),
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

            protected override Func<IKernelInternal, IReleasePolicy, object> BuildFactoryComponent(MethodInfo method, string componentName, Type componentType, Arguments additionalArguments)
            {
                var configuration = (JObject?)additionalArguments["configuration"];
                if (configuration != null)
                {
                    foreach (var parameter in configuration)
                    {
                        additionalArguments.AddNamed(
                            parameter.Key,
                            parameter.Value);
                    }
                }

                return base.BuildFactoryComponent(method, componentName, componentType, additionalArguments);
            }
        }
    }
}
