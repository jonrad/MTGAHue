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

namespace MTGAHue.Chroma
{
    internal class KeyboardLayout : ILightLayout
    {
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

        public async Task Transition(ILightSource lightSource, TimeSpan timeSpan, CancellationToken childToken = default)
        {
            //this looks familiar
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

            while (!token.IsCancellationRequested && stopwatch.ElapsedMilliseconds < timeSpan.Milliseconds)
            {
                var passed = stopwatch.ElapsedMilliseconds;

                KeyboardCustom nextColors;
                if (passed > timeSpan.Milliseconds)
                {
                    nextColors = endingColors;
                }
                else
                {
                    var percentage = (float)passed / timeSpan.Milliseconds;
                    nextColors = KeyboardCustom.Create();

                    for (var i = 0; i < KeyboardConstants.MaxKeys; i++)
                    {
                        var color = new Color(
                            (byte)(startingColors[i].R + (endingColors[i].R - startingColors[i].R) * percentage),
                            (byte)(startingColors[i].G + (endingColors[i].G - startingColors[i].G) * percentage),
                            (byte)(startingColors[i].B + (endingColors[i].B - startingColors[i].B) * percentage));

                        nextColors[i] = color;
                    }
                }

                currentColors = nextColors;
                await Task.WhenAll(new[]
                {
                    keyboard.SetCustomAsync(nextColors),
                    Task.Delay(100, token)
                });
            }
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
