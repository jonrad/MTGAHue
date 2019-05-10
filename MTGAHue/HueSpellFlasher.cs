﻿using LightsApi;
using LightsApi.LightSources;
using LightsApi.Transitions;
using MTGADispatcher;
using MTGADispatcher.Events;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.Streaming.Effects;
using Q42.HueApi.Streaming.Extensions;
using Q42.HueApi.Streaming.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTGAHue
{
    public class HueSpellFlasher
    {
        private Dictionary<MagicColor?, Func<CancellationToken, Task>> effectMap =
            new Dictionary<MagicColor?, Func<CancellationToken, Task>>();

        private CancellationTokenSource cancellationTokenSource;

        private readonly LightLayout layout;

        public HueSpellFlasher(LightLayout layout)
        {
            effectMap.Add(MagicColor.Black, BuildEffect(new RGB(255, 255, 255) * .3));
            effectMap.Add(MagicColor.White, BuildEffect(new RGB(255, 255, 255)));
            effectMap.Add(MagicColor.Red, BuildEffect(new RGB(255, 0, 0)));
            effectMap.Add(MagicColor.Green, BuildEffect(new RGB(0, 255, 0)));
            effectMap.Add(MagicColor.Blue, BuildEffect(new RGB(0, 0, 255)));

            this.layout = layout;
        }

        private Func<CancellationToken, Task> BuildEffect(RGB color)
        {
            var flash = new ITransition[]
            {
                new LightSourceTransition(new OmniLightSource(color), 500),
                new LightSourceTransition(new OmniLightSource(color * .6), 500)
            };

            var flashCount = 5;

            var transitions =
                new[] { new LightSourceTransition(new OmniLightSource(color * .1), 50) }
                .Concat(Enumerable.Repeat(flash, flashCount).SelectMany(s => s))
                .Concat(new[] { new LightSourceTransition(new OmniLightSource(color * .3), 5000) })
                .ToArray();

            return async token =>
            {
                foreach (var transition in transitions)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    await layout.Transition(transition, token);
                }
            };
        }

        public void OnCastSpell(CastSpell spell)
        {
            CancelPrevious();

            var colors = spell.Instance.Colors;

            if (colors.Length == 0)
            {
                return;
            }

            var color = colors[0];

            cancellationTokenSource = new CancellationTokenSource();

            effectMap[color](cancellationTokenSource.Token)
                .ContinueWith(t => { }, TaskContinuationOptions.OnlyOnCanceled);
        }

        private void CancelPrevious()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
        }

        private class State
        {
            public RGBColor Rgb { get; set; }

            public double Brightness { get; set; }

            public TimeSpan TransitionTime { get; set; }

            public State(RGBColor color, double brightness, int ms)
            {
                Rgb = color;
                Brightness = brightness;
                TransitionTime = TimeSpan.FromMilliseconds(ms);
            }
        }
    }
}
