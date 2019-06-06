using LightsApi;
using LightsApi.LightSources;
using LightsApi.Transitions;
using MTGADispatcher;
using MTGADispatcher.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagicLights.Effects
{
    public class BoxOutEffect
        : IEffect<CastSpell>, IEffect<PlayLand>
    {
        private Dictionary<MagicColor, RGB> colorMap = new Dictionary<MagicColor, RGB>
        {
            [MagicColor.Black] = new RGB(100, 0, 100),
            [MagicColor.White] = new RGB(255, 255, 255),
            [MagicColor.Red] = new RGB(255, 0, 0),
            [MagicColor.Green] = new RGB(0, 255, 0),
            [MagicColor.Blue] = new RGB(0, 0, 255)
        };

        public EffectMode Mode => EffectMode.Concurrent;

        public ITransition? InstanceEvent(Instance instance)
        {
            var colors = instance.Colors;
            if (!colors.Any())
            {
                return null;
            }

            var color1 = colors[0];

            var color2 = colors.Length >= 2
                ? colors[1]
                : color1;

            var time = TimeSpan.FromMilliseconds(1000);

            //var lightSource = new GradientLightSource(colorMap[color1], RGB.Black, new Position(0, 0), new Position(1, 0));
            //return new FadeInTransition(lightSource, lightSource, 2000);

            return new CompositeTransition(
                    new FilteredTransition(
                        (x, y, _) => x >= 0,
                        new LinearMovementTransition(
                            new GradientLightSource(RGB.Black, colorMap[color1], new Position(-1, 0), new Position(0, 0)),
                            2,
                            0,
                            time)),
                    new FilteredTransition(
                        (x, y, _) => x <= 0,
                        new LinearMovementTransition(
                            new GradientLightSource(colorMap[color2], RGB.Black, new Position(0, 0), new Position(1, 0)),
                            -2,
                            0,
                            time)));
        }

        public ITransition? OnMagicEvent(CastSpell magicEvent)
        {
            return InstanceEvent(magicEvent.Instance);
        }

        public ITransition? OnMagicEvent(PlayLand magicEvent)
        {
            return InstanceEvent(magicEvent.Instance);
        }
    }
}
