using LightsApi;
using LightsApi.LightSources;
using LightsApi.Transitions;
using MTGADispatcher;
using MTGADispatcher.Events;
using System.Collections.Generic;
using System.Linq;

namespace MagicLights.Effects
{
    public class FlashEffect
        : IEffect<CastSpell>
    {
        private Dictionary<MagicColor, RGB> colorMap = new Dictionary<MagicColor, RGB>
        {
            [MagicColor.Black] = new RGB(100, 0, 100),
            [MagicColor.White] = new RGB(255, 255, 255),
            [MagicColor.Red] = new RGB(255, 0, 0),
            [MagicColor.Green] = new RGB(0, 255, 0),
            [MagicColor.Blue] = new RGB(0, 0, 255)
        };

        public ITransition? OnMagicEvent(CastSpell magicEvent)
        {
            var colors = magicEvent.Instance.Colors;

            if (colors.Length == 0)
            {
                return null;
            }

            var color = colorMap[colors[0]];

            var flash = new ITransition[]
            {
                new LightSourceTransition(new SolidLightSource(color), 500),
                new LightSourceTransition(new SolidLightSource(color * .6), 500)
            };

            var flashCount = 5;

            var transitions =
                new[] { new LightSourceTransition(new SolidLightSource(color * .1), 50) }
                .Concat(Enumerable.Repeat(flash, flashCount).SelectMany(s => s))
                .Concat(new[] { new LightSourceTransition(new SolidLightSource(color * .3), 5000) })
                .ToArray();

            return new CompositeTransition(transitions);
        }
    }
}
