using LightsApi;
using LightsApi.Transitions;
using MTGADispatcher;
using MTGADispatcher.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagicLights.Effects
{
    public class ExplosionEffect
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

        private readonly int speedMs;

        public ExplosionEffect(int speedMs = 2000)
        {
            this.speedMs = speedMs;
        }

        public EffectMode Mode { get; } = EffectMode.Concurrent;

        public ITransition? OnMagicEvent(Instance instance)
        {
            if (!instance.Colors.Any())
            {
                return null;
            }

            var rgbs = instance.Colors.Select(c => colorMap[c]).ToArray();

            var angleStep = 360f / rgbs.Length;
            var startAngle = 270;

            var transitions =
                rgbs.Select((rgb, i) =>
                {
                    return new AngleFilterTransition(
                        new ExplositionTransition(TimeSpan.FromMilliseconds(speedMs), rgb, .5f),
                        0,
                        0,
                        (startAngle + i * angleStep) % 360,
                        angleStep);
                });

            return new CompositeTransition(transitions.ToArray());
        }

        public ITransition? OnMagicEvent(CastSpell magicEvent)
        {
            return OnMagicEvent(magicEvent.Instance);
        }

        public ITransition? OnMagicEvent(PlayLand magicEvent)
        {
            return OnMagicEvent(magicEvent.Instance);
        }

        private class ExplositionTransition : ITransition
        {
            private readonly double totalMs;

            private readonly RGB color;

            private readonly float radius;

            private readonly double movementPerMs;

            public ExplositionTransition(
                TimeSpan time,
                RGB color,
                float radius)
            {
                this.color = color;
                this.radius = radius;
                TotalLength = time;

                var maxRadius = radius + Math.Sqrt(2);

                totalMs = time.TotalMilliseconds;
                movementPerMs = maxRadius / totalMs;
            }

            public TimeSpan TotalLength { get; }

            public RGB Get(float x, float y, long ms)
            {
                if (ms >= totalMs)
                {
                    return RGB.Black;
                }

                var currentRadius = ms * movementPerMs;

                var distanceFromCenter = Math.Sqrt(x * x + y * y);

                var diff = Math.Abs(currentRadius - distanceFromCenter);

                if (diff > radius)
                {
                    return RGB.Black;
                }

                return color * ((radius - diff) / radius);
            }
        }
    }
}
