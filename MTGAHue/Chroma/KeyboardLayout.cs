using Colore;
using Colore.Data;
using Colore.Effects.Keyboard;
using LightsApi;
using LightsApi.LightSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTGAHue.Chroma
{
    public class KeyboardLayout : ILightLayout
    {
        private readonly IKeyboard keyboard;

        private readonly KeyboardPosition[] positions;

        public KeyboardLayout(IKeyboard keyboard)
        {
            this.keyboard = keyboard;
            positions = CalculatePositions().ToArray();
        }

        private IEnumerable<KeyboardPosition> CalculatePositions()
        {
            var keyboardColumnStep = 2d / KeyboardConstants.MaxColumns;
            var keyboardRowStep = 2d / KeyboardConstants.MaxRows;

            var startX = -1 + keyboardColumnStep / 2;
            var startY = -1 + keyboardRowStep / 2;

            for (var column = 0; column < KeyboardConstants.MaxColumns; column++)
            {
                for (var row = 0; row < KeyboardConstants.MaxRows; row++)
                {
                    yield return new KeyboardPosition(column, row, startX + column * keyboardColumnStep, startY + row * keyboardRowStep);
                }
            }
        }

        public Task Transition(ILightSource lightSource, TimeSpan timeSpan, CancellationToken token = default)
        {
            var keyboardCustom = KeyboardCustom.Create();

            foreach (var pos in positions)
            {
                var rgb = lightSource.Calculate(pos.X, pos.Y);
                keyboardCustom[pos.Row, pos.Column] = new Color((byte)rgb.R, (byte)rgb.G, (byte)rgb.B);
            }

            return Task.WhenAll(new[]
            {
                Task.Delay(timeSpan, token),
                keyboard.SetCustomAsync(keyboardCustom)
            });
        }

        private class KeyboardPosition
        {
            public KeyboardPosition(int column, int row, double x, double y)
            {
                Column = column;
                Row = row;
                X = x;
                Y = y;
            }

            public int Column { get; }

            public int Row { get; }

            public double X { get; }

            public double Y { get; }
        }
    }
}
