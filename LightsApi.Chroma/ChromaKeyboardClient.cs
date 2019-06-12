using Colore;
using Colore.Effects.Keyboard;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi.Chroma
{
    public class ChromaKeyboardClient : ILightClient
    {
        private readonly KeyboardPosition[] keyboardPositions;

        private readonly Position[] positions;

        private readonly IKeyboard keyboard;

        public ChromaKeyboardClient(IChroma chroma, int? columnCount, int? rowCount)
        {
            keyboard = chroma.Keyboard;
            keyboardPositions = CalculatePositions(columnCount ?? KeyboardConstants.MaxColumns, rowCount ?? KeyboardConstants.MaxRows).ToArray();
            positions = keyboardPositions.Select(k => new Position(k.X, k.Y)).ToArray();
        }

        public IEnumerable<Position> Lights => positions;

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public Task SetColors(IEnumerable<RGB> colors, CancellationToken token)
        {
            var nextColors = KeyboardCustom.Create();

            var i = 0;
            foreach (var color in colors)
            {
                var position = keyboardPositions[i];
                nextColors[position.Row, position.Column] = new Colore.Data.Color(
                    (byte)color.R, (byte)color.G, (byte)color.B);

                i++;
            }

            return keyboard.SetCustomAsync(nextColors);
        }

        //TODO move to factory
        private IEnumerable<KeyboardPosition> CalculatePositions(int columnCount, int rowCount)
        {
            var keyboardColumnStep = 2f / columnCount;
            var keyboardRowStep = 2f / rowCount;

            var startX = -1 + keyboardColumnStep / 2;
            var startY = 1 - keyboardRowStep / 2;

            for (var column = 0; column < columnCount; column++)
            {
                for (var row = 0; row < rowCount; row++)
                {
                    yield return new KeyboardPosition(
                        column,
                        row,
                        startX + column * keyboardColumnStep,
                        startY - row * keyboardRowStep);
                }
            }
        }

        private class KeyboardPosition
        {
            public KeyboardPosition(int column, int row, float x, float y)
            {
                Column = column;
                Row = row;
                X = x;
                Y = y;
            }

            public int Column { get; }

            public int Row { get; }

            public float X { get; }

            public float Y { get; }
        }
    }
}
