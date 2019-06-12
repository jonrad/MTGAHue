using Castle.Facilities.TypedFactory;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MagicLights.Configuration;
using MagicLights.Effects;
using MTGADispatcher.Events;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace MagicLights
{
    public class ApplicationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<TypedFactoryFacility>();

            container.Register(
                Component.For<IEffect<CastSpell>>()
                    .ImplementedBy<BoxOutEffect>()
                    .Named("boxOut")
                    .LifestyleTransient(),
                Component.For<IEffect<CastSpell>>()
                    .ImplementedBy<FlashEffect>()
                    .Named("flash")
                    .LifestyleTransient(),
                Component.For<IEffect<CastSpell>>()
                    .ImplementedBy<MultiColorFlash>()
                    .Named("multiflash")
                    .LifestyleTransient(),
                Component.For<IEffect<CastSpell>>()
                    .ImplementedBy<ExplosionEffect>()
                    .Named("explosion")
                    .LifestyleTransient(),

                Component.For<IEffectFactory>().AsFactory(f =>
                    f.SelectedWith(new NamedCustomSelector())),

                Component.For<ILightsConfigurationProvider>()
                    .ImplementedBy<FileConfigurationProvider>()
                    .DependsOn(
                        Dependency.OnValue<string>("config.json")),

                Component.For<LightsSetup>(),
                Component.For<MagicLightsApplication>());
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
