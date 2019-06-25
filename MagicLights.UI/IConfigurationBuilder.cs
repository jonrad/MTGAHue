using System;
using System.Collections.Generic;

namespace MagicLights.UI
{
    public interface IConfigurationBuilder
    {
        IEnumerable<ConfigurationField> Get(Type type);
    }
}
