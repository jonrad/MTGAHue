using LightsApi;
using LightsApi.LightSources;
using LightsApi.Transitions;
using MTGADispatcher;
using MTGADispatcher.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHue.Effects
{
    public class ExplosionEffect
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

        private readonly int speedMs;

        public ExplosionEffect(int speedMs = 2000)
        {
            this.speedMs = speedMs;
        }

        public ITransition? OnMagicEvent(CastSpell magicEvent)
        {
            var colors = magicEvent.Instance.Colors.Select(c => colorMap[c]).ToArray();
            if (colors.Length == 0)
            {
                return null;
            }

            var anglesEach = 360 / colors.Length;

            IEnumerable<ILightSource> BuildLightSources(double radius)
            {
                for (var i = 0; i < colors.Length; i++)
                {
                    var color = colors[i];
                    yield return new AngleFilterLightSource(
                        new FadedCircleLightSource(color, 0, 0, radius, Math.Min(radius, 0.5)),
                        (270 + i * anglesEach) % 360,
                        anglesEach);
                }
            }

            IEnumerable<ITransition> BuildTransitions()
            {
                var totalTime = speedMs;
                var steps = 40;
                var maxRadius = 2D;

                var stepTime = totalTime / steps;
                var radiusStep = maxRadius / (float)steps;

                for (var i = 0; i < steps; i++)
                {
                    var radius = (i + 1) * radiusStep;
                    yield return new LightSourceTransition(
                        new LayeredLightSource(BuildLightSources(radius).ToArray()),
                        stepTime);
                }
            }

            return new CompositeTransition(BuildTransitions().ToArray());
        }
    }
}
