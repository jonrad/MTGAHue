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
        private Dictionary<string, MagicColor> landColorMap = new Dictionary<string, MagicColor>
        {
            ["SubType_Plains"] = MagicColor.White,
            ["SubType_Mountain"] = MagicColor.Red,
            ["SubType_Forest"] = MagicColor.Green,
            ["SubType_Island"] = MagicColor.Blue,
            ["SubType_Swamp"] = MagicColor.Black
        };

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
            if (gameObject["color"] is JArray colorToken)
            {
                return GetColorFromColorArray(colorToken);
            }

            if (gameObject["cardTypes"] is JArray cardTypes && cardTypes.Values<string>().Contains("CardType_Land"))
            {
                return GetColorsForLand(gameObject);
            }

            return Enumerable.Empty<MagicColor>();
        }

        private IEnumerable<MagicColor> GetColorFromColorArray(JArray colors)
        {
            foreach (var colorString in colors.Select(c => c.Value<string>()))
            {
                var color = colorString.Split('_').Last();
                if (!Enum.TryParse<MagicColor>(color, out var colorValue))
                {
                    //Log me
                }
                else
                {
                    yield return colorValue;
                }
            }
        }

        private IEnumerable<MagicColor> GetColorsForLand(JToken gameObject)
        {
            if (!(gameObject["subtypes"] is JArray subTypes))
            {
                yield break;
            }

            foreach (var subType in subTypes.Values<string>())
            {
                if (landColorMap.TryGetValue(subType, out var color))
                {
                    yield return color;
                }
            }
        }
    }
}
