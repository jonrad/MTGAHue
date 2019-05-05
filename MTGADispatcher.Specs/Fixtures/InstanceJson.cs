using Newtonsoft.Json.Linq;

namespace MTGADispatcher.Specs.Fixtures
{
    public static class InstanceJson
    {
        public static JObject Merge(JObject instance, object other)
        {
            if (other == null)
            {
                return instance;
            }

            instance.Merge(JObject.FromObject(other));

            return instance;
        }

        public static JObject Build(object withProperties = null)
        {
            var instance = new JObject();
            instance["instanceId"] = 1;
            instance["grpId"] = 1;

            return Merge(instance, withProperties);
        }

        public static JObject WithColors(this JObject instance, params string[] colors)
        {
            return Merge(
                instance,
                new { color = colors });
        }

        public static JObject WithCardTypes(this JObject instance, params string[] types)
        {
            return Merge(
                instance,
                new { cardTypes = types });
        }

        public static JObject WithSubTypes(this JObject instance, params string[] types)
        {
            return Merge(
                instance,
                new { subtypes = types });
        }
    }
}
