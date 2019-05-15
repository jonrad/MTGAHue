using Colore;
using Colore.Data;
using Colore.Effects.Keyboard;
using LightsApi;
using LightsApi.LightSources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi.Chroma
{
    internal class KeyboardLayout : ILightLayout
    {
        private const int ColumnCount = 16;

        private const int RowCount = 6;

        private readonly IKeyboard keyboard;

        private readonly KeyboardPosition[] positions;

        private KeyboardCustom currentColors = KeyboardCustom.Create();

        private CancellationTokenSource cancellationTokenSource;

        public KeyboardLayout(IKeyboard keyboard)
        {
            this.keyboard = keyboard;
            positions = CalculatePositions().ToArray();
        }

        private IEnumerable<KeyboardPosition> CalculatePositions()
        {
            var keyboardColumnStep = 2d / ColumnCount;
            var keyboardRowStep = 2d / RowCount;

            var startX = -1 + keyboardColumnStep / 2;
            var startY = 1 - keyboardRowStep / 2;

            for (var column = 0; column < ColumnCount; column++)
            {
                for (var row = 0; row < RowCount; row++)
                {
                    yield return new KeyboardPosition(
                        column,
                        row,
                        startX + column * keyboardColumnStep,
                        startY - row * keyboardRowStep);
                }
            }
        }

        public async Task Transition(ILightSource lightSource, TimeSpan timeSpan, CancellationToken childToken = default)
        {
            var totalMilliseconds = timeSpan.TotalMilliseconds;

            //TODO: this looks familiar. clean up with transition code from Hue
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }

            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(childToken);

            var token = cancellationTokenSource.Token;

            var startingColors = currentColors;
            var endingColors = KeyboardCustom.Create();

            foreach (var pos in positions)
            {
                var rgb = lightSource.Calculate(pos.X, pos.Y);
                endingColors[pos.Row, pos.Column] = new Color((byte)rgb.R, (byte)rgb.G, (byte)rgb.B);
            }

            var stopwatch = Stopwatch.StartNew();

            long elapsed;
            while (!token.IsCancellationRequested && (elapsed = stopwatch.ElapsedMilliseconds) < totalMilliseconds)
            {
                var nextColors = KeyboardCustom.Create();
                var percentage = elapsed / totalMilliseconds;

                for (var i = 0; i < KeyboardConstants.MaxKeys; i++)
                {
                    var color = new Color(
                        (byte)(startingColors[i].R + (endingColors[i].R - startingColors[i].R) * percentage),
                        (byte)(startingColors[i].G + (endingColors[i].G - startingColors[i].G) * percentage),
                        (byte)(startingColors[i].B + (endingColors[i].B - startingColors[i].B) * percentage));

                    nextColors[i] = color;
                }

                var delay = Math.Min(100, totalMilliseconds - elapsed);

                currentColors = nextColors;
                await Task.WhenAll(new[]
                {
                    keyboard.SetCustomAsync(nextColors),
                    Task.Delay(TimeSpan.FromMilliseconds(delay), token)
                });
            }

            currentColors = endingColors;
            await keyboard.SetCustomAsync(endingColors);
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
