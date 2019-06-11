using MagicLights.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace MagicLights.Api.Controllers
{
    [Route("api/[controller]")]
    public class ConfigurationController : Controller
    {
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

            return Ok(new
            {
                configuration
            });
        }
    }
}
