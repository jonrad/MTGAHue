﻿using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MagicLights.Configuration;
using MagicLights.Effects;
using MagicLights.LightClients;
using MTGADispatcher;
using MTGADispatcher.BlockProcessing;
using MTGADispatcher.Events;

namespace MagicLights.Integration
{
    public class IntegrationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IEffect<CastSpell>>()
                    .ImplementedBy<SolidEffect>()
                    .Named("solid")
                    .LifestyleTransient(),
                Component.For<IntegrationLightClient>(),
                Component.For<ILightsConfigurationProvider>().ImplementedBy<ConfigurationProviderProxy>().IsDefault(),
                Component.For<IntegrationLightClientProvider>().Forward<ILightClientProvider>(),
                Component.For<IGameUpdater>().ImplementedBy<GameEndedBlockHandler>(),
                Component.For<ILineReader>().ImplementedBy<LineReaderProxy>().IsDefault());
        }
    }
}
