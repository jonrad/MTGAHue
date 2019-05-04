using MTGADispatcher;
using MTGADispatcher.Events;
using Q42.HueApi.ColorConverters;
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
        private Dictionary<MagicColor, Func<CancellationToken, Task>> effectMap =
            new Dictionary<MagicColor, Func<CancellationToken, Task>>();

        private CancellationTokenSource cancellationTokenSource;

        private EntertainmentLayer entLayer;

        public HueSpellFlasher(StreamingGroup stream)
        {
            entLayer = stream.GetNewLayer(isBaseLayer: true);

            effectMap.Add(MagicColor.Black, BuildEffect(new RGBColor(255, 255, 255), .2));
            effectMap.Add(MagicColor.White, BuildEffect(new RGBColor(255, 255, 255)));
            effectMap.Add(MagicColor.Red, BuildEffect(new RGBColor(255, 0, 0)));
            effectMap.Add(MagicColor.Green, BuildEffect(new RGBColor(0, 255, 0)));
            effectMap.Add(MagicColor.Blue, BuildEffect(new RGBColor(0, 0, 255)));
        }

        private Func<CancellationToken, Task> BuildEffect(
            RGBColor color,
            double fullBrightness = 1.0)
        {
            var flash = new[]
            {
                new State(color, fullBrightness, 500),
                new State(color, fullBrightness * .6, 500)
            };

            var flashCount = 5;

            var states =
                new[] { new State(color, fullBrightness * .1, 50) }
                .Concat(Enumerable.Repeat(flash, flashCount).SelectMany(s => s))
                .Concat(new[] { new State(color, fullBrightness * .3, 5000) })
                .ToArray();

            return async token =>
            {
                foreach (var state in states)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    entLayer.SetState(token, state.Rgb, state.Brightness, state.TransitionTime, true);

                    await Task.Delay(state.TransitionTime, token);
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
