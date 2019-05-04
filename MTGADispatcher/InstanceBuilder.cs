using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGADispatcher
{
    // Thoughts: An instance can be multiple types (eg Card vs Ability)
    // with different properties based on type
    // This should return an interface and build all of them
    public class InstanceBuilder : IInstanceBuilder
    {
        public Instance Build(JToken gameObject)
        {
            //TODO: error handling, logging
            var instanceId = gameObject["instanceId"].Value<int>();
            var cardId = gameObject["grpId"].Value<int>();

            var colors = GetColors(gameObject).ToArray();
            return new Instance(instanceId, cardId, colors);
        }

        private IEnumerable<MagicColor> GetColors(JToken gameObject)
        {
            //would be nice if we could figure out how to make
            //lands the proper colors
            if (!(gameObject["color"] is JArray colorToken))
            {
                yield break;
            }

            foreach (var colorString in colorToken.Select(c => c.Value<string>()))
            {
                var color = colorString.Split('_').Last();
                if (!Enum.TryParse<MagicColor>(color, out var colorValue))
                {
                    //Log me
                    //Console.WriteLine($"Unknown color {colorString}");
                }
                else
                {
                    yield return colorValue;
                }
            }
        }
    }
}
