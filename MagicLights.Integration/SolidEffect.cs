using LightsApi;
using LightsApi.LightSources;
using LightsApi.Transitions;
using MagicLights.Effects;
using MTGADispatcher;
using MTGADispatcher.Events;
using System.Collections.Generic;
using System.Linq;

namespace MagicLights.Integration
{
    public class SolidEffect : IEffect<CastSpell>
    {
        private Dictionary<MagicColor, RGB> colorMap = new Dictionary<MagicColor, RGB>
        {
            [MagicColor.Black] = new RGB(100, 0, 100),
            [MagicColor.White] = new RGB(255, 255, 255),
            [MagicColor.Red] = new RGB(255, 0, 0),
            [MagicColor.Green] = new RGB(0, 255, 0),
            [MagicColor.Blue] = new RGB(0, 0, 255)
        };

        public EffectMode Mode => EffectMode.Single;

        public ITransition? OnMagicEvent(CastSpell magicEvent)
        {
            var colors = magicEvent.Instance.Colors;
            if (!colors.Any())
            {
                return null;
            }

            var color = colors.FirstOrDefault();

            return new FadeInTransition(
                new SolidLightSource(colorMap[color]),
                0);
        }
    }
}
