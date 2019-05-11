using Colore;
using LightsApi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTGAHue.Chroma
{
    public class KeyboardLight : ILight
    {
        private readonly IKeyboard keyboard;

        private readonly int keyboardRow;

        private readonly int keyboardColumn;

        public KeyboardLight(
            IKeyboard keyboard,
            int col,
            int row,
            double x,
            double y)
        {
            this.keyboard = keyboard;
            keyboardRow = row;
            keyboardColumn = col;
            X = x;
            Y = y;
        }

        public double X { get; }

        public double Y { get; }

        public Task Transition(RGB rgb, TimeSpan transitionTime, CancellationToken token)
        {
            var wait = Task.Delay(transitionTime, token);

            var task = keyboard.SetPositionAsync(
                keyboardRow,
                keyboardColumn,
                new Colore.Data.Color((byte)rgb.R, (byte)rgb.G, (byte)rgb.B),
                false);

            return Task.WhenAll(wait, task);
        }
    }
}
