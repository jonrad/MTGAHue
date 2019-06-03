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
    // TODO: refactor this whole thing, it's a copy of FlashEffect
    public class MultiColorFlash
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

        public EffectMode Mode { get; } = EffectMode.Single;

        public ITransition? OnMagicEvent(CastSpell magicEvent)
        {
            var colors = magicEvent.Instance.Colors;

            if (colors.Length == 0)
            {
                return null;
            }

            var rgbs = colors.Select(c => colorMap[c]).ToArray();

            var coordinateMap = new RandomCoordinateMap(rgbs);

            var flash = new ITransition[]
            {
                new LightSourceTransition(new MappedLightSource(coordinateMap, .4f), new MappedLightSource(coordinateMap, .8f), 500),
                new LightSourceTransition(new MappedLightSource(coordinateMap, .8f), new MappedLightSource(coordinateMap, .4f), 500)
            };

            var flashCount = 3;

            var transitions =
                new[] { new LightSourceTransition(new MappedLightSource(coordinateMap, .4f), 50) }
                .Concat(Enumerable.Repeat(flash, flashCount).SelectMany(s => s))
                .Concat(new[] { new LightSourceTransition(new MappedLightSource(coordinateMap, .4f), new MappedLightSource(coordinateMap, .1f), 1000) })
                .ToArray();

            return new CompositeTransition(transitions);
        }

        private class MappedLightSource : ILightSource
        {
            private readonly ICoordinateMap coordinateMap;

            private readonly float strength;

            public MappedLightSource(ICoordinateMap coordinateMap, float strength)
            {
                this.coordinateMap = coordinateMap;
                this.strength = strength;
            }

            public RGB Calculate(double x, double y)
            {
                return coordinateMap.Get(x, y) * strength;
            }
        }

        private class RandomCoordinateMap : ICoordinateMap
        {
            private readonly Dictionary<double, RGB> cache = new Dictionary<double, RGB>();

            private readonly RGB[] colors;

            private readonly Random random;

            private int selectionCount;

            public RandomCoordinateMap(RGB[] colors)
            {
                this.colors = colors.ToArray();
                selectionCount = colors.Length;
                random = new Random();
            }

            public RGB Get(double x, double y)
            {
                var lookup = x + 10 + y;
                if (cache.TryGetValue(lookup, out var value))
                {
                    return value;
                }

                var nextIndex = random.Next(0, selectionCount);

                var result = colors[nextIndex];
                colors[nextIndex] = colors[selectionCount - 1];
                colors[selectionCount - 1] = result;

                selectionCount -= 1;

                if (selectionCount == 0)
                {
                    selectionCount = colors.Length;
                }

                return cache[lookup] = result;
            }
        }

        private interface ICoordinateMap
        {
            RGB Get(double x, double y);
        }
    }
}
