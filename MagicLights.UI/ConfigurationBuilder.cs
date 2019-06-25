using System;
using System.Collections.Generic;
using System.Linq;

namespace MagicLights.UI
{
    public class ConfigurationBuilder : IConfigurationBuilder
    {
        public IEnumerable<ConfigurationField> Get(Type type)
        {
            var properties = type.GetProperties();
            return properties.Select(
                p => new ConfigurationField(p.Name, p.PropertyType));
        }
    }
}
