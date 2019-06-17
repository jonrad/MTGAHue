using MagicLights.Api.Models;
using MagicLights.Configuration;
using MagicLights.Configuration.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MagicLights.Api.Controllers
{
    [Route("api/[controller]")]
    public class ConfigurationController : Controller
    {
        private readonly ILightsConfigurationProvider configurationProvider;

        private readonly MagicLightsApplication application;

        public ConfigurationController(
            ILightsConfigurationProvider configurationProvider,
            MagicLightsApplication application)
        {
            this.configurationProvider = configurationProvider;
            this.application = application;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            return Ok(GetConfiguration());
        }

        [HttpPost()]
        public async Task<IActionResult> Post([FromBody]ConfigurationModel? configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentException();
            }

            Save(configuration);

            application.Stop();

            await application.Start();

            return Ok(new
            {
                configuration
            });
        }

        private object GetConfiguration()
        {
            var configuration = configurationProvider.Get();

            return new
            {
                LightClients = configuration.LightClients.Select(ToModel).ToArray()
            };
        }

        private object ToModel(LightClientConfiguration configuration)
        {
            return new
            {
                configuration.Id,
                Value = new
                {
                    configuration.Enabled
                }
            };
        }

        private void Save(ConfigurationModel model)
        {
            var configuration = configurationProvider.Get();

            if (model.LightClients == null)
            {
                return;
            }

            foreach (var lightClient in model.LightClients)
            {
                var lightClientConfig = configuration.LightClients.FirstOrDefault(l => l.Id == lightClient.Id);

                if (lightClientConfig == null)
                {
                    continue;
                }

                lightClientConfig.Enabled = lightClient.Enabled;
            }

            configurationProvider.Save(configuration);
        }
    }
}
