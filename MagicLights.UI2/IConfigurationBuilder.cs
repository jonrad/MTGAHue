using System;
using System.Collections.Generic;

namespace MagicLights.UI2
{
    public interface IConfigurationBuilder
    {
        IEnumerable<ConfigurationField> Get(Type type);
    }
}
