using System;

namespace MagicLights.UI
{
    public class ConfigurationField
    {
        public ConfigurationField(string id, Type type)
        {
            Id = id;
            Type = type;
        }

        public string Id { get; }

        public Type Type { get; }
    }
}
