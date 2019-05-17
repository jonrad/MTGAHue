using MTGADispatcher.ClientModels;
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

        public Instance Build(InstanceModel model)
        {
            //TODO: error handling, logging
            var instanceId = model.Id;
            var cardId = model.CardId;
            var ownerId = model.OwnerId;

            var colors = GetColors(model).ToArray();
            return new Instance(instanceId, cardId, ownerId, colors);
        }

        private IEnumerable<MagicColor> GetColors(InstanceModel model)
        {
            if (model.Colors != null)
            {
                return GetColorFromColorArray(model.Colors);
            }

            if (model.CardTypes != null && model.CardTypes.Contains("CardType_Land"))
            {
                return GetColorsForLand(model);
            }

            return Enumerable.Empty<MagicColor>();
        }

        private IEnumerable<MagicColor> GetColorFromColorArray(string[] colors)
        {
            foreach (var colorString in colors)
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

        private IEnumerable<MagicColor> GetColorsForLand(InstanceModel model)
        {
            if (model.CardSubtypes == null)
            {
                yield break;
            }

            foreach (var subType in model.CardSubtypes)
            {
                if (landColorMap.TryGetValue(subType, out var color))
                {
                    yield return color;
                }
            }
        }
    }
}
