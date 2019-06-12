using CUE.NET;
using CUE.NET.Brushes;
using CUE.NET.Devices.Generic.Enums;
using System;
using System.Drawing;
using System.Linq;

namespace LightsApi.Cue
{
    public class CueLightClientBuilder : IDisposable
    {
        public CueLightClient Build(
            CorsairLedId? leftLed,
            CorsairLedId? rightLed,
            CorsairLedId? topLed,
            CorsairLedId? bottomLed)
        {
            if (!CueSDK.IsInitialized)
            {
                CueSDK.Initialize(true);
            }

            if (!CueSDK.HasExclusiveAccess)
            {
                CueSDK.Reinitialize(true);
            }

            var keyboard = CueSDK.KeyboardSDK;
            keyboard.Brush = (SolidColorBrush)Color.Transparent;

            float left = float.MaxValue, top = float.MaxValue;
            float right = float.MinValue, bottom = float.MinValue;

            left = leftLed == null
                ? keyboard.Min(l => l.LedRectangle.Left)
                : keyboard[leftLed.Value].LedRectangle.Left;

            right = rightLed == null
                ? keyboard.Max(l => l.LedRectangle.Right)
                : keyboard[rightLed.Value].LedRectangle.Right;

            top = topLed == null
                ? keyboard.Min(l => l.LedRectangle.Top)
                : keyboard[topLed.Value].LedRectangle.Top;

            bottom = bottomLed == null
                ? keyboard.Max(l => l.LedRectangle.Bottom)
                : keyboard[bottomLed.Value].LedRectangle.Bottom;

            var width = right - left;
            var height = bottom - top;

            var ledPositions = keyboard.Select(key =>
            {
                var rect = key.LedRectangle;
                var x = -1 + (rect.Left + (rect.Right - rect.Left) / 2f - left) / (width / 2f);
                var y = 1 - (rect.Top + (rect.Bottom - rect.Top) / 2 - top) / (height / 2f);

                if (x < -1 || x > 1 || y < -1 || y > 1)
                {
                    return null;
                }

                return new LedPosition(key, new Position(x, y));
            })
            .Where(k => k != null)
            .Cast<LedPosition>()
            .ToArray();

            return new CueLightClient(keyboard, ledPositions);
        }

        public void Dispose()
        {
            CueSDK.Reinitialize(false);
            CueSDK.Reset();
        }
    }
}
