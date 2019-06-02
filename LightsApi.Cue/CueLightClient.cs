using CUE.NET;
using CUE.NET.Brushes;
using CUE.NET.Devices.Generic;
using CUE.NET.Devices.Generic.Enums;
using CUE.NET.Devices.Keyboard;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi.Cue
{
    public class CueLightClient : ILightClient
    {
        private KeyboardPosition[] keyboardPositions;

        private CorsairKeyboard keyboard;

        public CueLightClient()
        {
            CueSDK.Initialize(true);
            keyboard = CueSDK.KeyboardSDK;
            keyboard.Brush = (SolidColorBrush)Color.Transparent;

            float left = float.MaxValue, top = float.MaxValue;
            float right = float.MinValue, bottom = float.MinValue;

            left = keyboard[CorsairLedId.Escape].LedRectangle.Left;
            top = keyboard[CorsairLedId.Escape].LedRectangle.Top;
            bottom = keyboard[CorsairLedId.Space].LedRectangle.Bottom;
            right = keyboard[CorsairLedId.F12].LedRectangle.Right;

            var width = right - left;
            var height = bottom - top;

            keyboardPositions = keyboard.Select(key =>
            {
                var rect = key.LedRectangle;
                var x = -1 + (rect.Left + (rect.Right - rect.Left) / 2f - left) / (width / 2f);
                var y = 1 - (rect.Top + (rect.Bottom - rect.Top) / 2 - top) / (height / 2f);

                if (x < -1 || x > 1 || y < -1 || y > 1)
                {
                    return null;
                }

                return new KeyboardPosition(key, new Position(x, y));
            })
            .Where(k => k != null)
            .ToArray();

            Lights = keyboardPositions.Select(k => k.Position).ToArray();
        }

        public IEnumerable<Position> Lights { get; }

        public Task SetColors(IEnumerable<RGB> colors, CancellationToken token)
        {
            return Task.Run(() =>
            {
                var i = 0;
                foreach (var color in colors)
                {
                    keyboardPositions[i].Led.Color = new CorsairColor(
                        (byte)color.R, (byte)color.G, (byte)color.B);

                    i++;
                }

                keyboard.Update();
            }, token);
        }

        private class KeyboardPosition
        {
            public KeyboardPosition(CorsairLed led, Position position)
            {
                Led = led;
                Position = position;
            }

            public CorsairLed Led { get; }

            public Position Position { get; }
        }
    }
}
