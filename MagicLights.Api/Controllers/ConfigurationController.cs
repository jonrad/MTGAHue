using MagicLights.Api.Models;
using MagicLights.Configuration;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace MagicLights.Api.Controllers
{
    [Route("api/[controller]")]
    public class ConfigurationController : Controller
    {
        private readonly ILightsConfigurationProvider configurationProvider;

        public ConfigurationController(ILightsConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            throw new NotImplementedException();
        }

        [HttpPost()]
        public IActionResult Post([FromBody]ConfigurationModel? configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentException();
            }

            Save(configuration);

            return Ok(new
            {
                configuration
            });
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
