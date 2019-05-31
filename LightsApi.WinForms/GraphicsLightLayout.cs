using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi.WinForms
{
    public class GraphicsLightClient : ILightClient
    {
        private readonly Position[] positions;

        private readonly int count;

        private readonly BufferedGraphics buffer;

        public GraphicsLightClient(Graphics graphics, int count)
        {
            this.count = count;
            buffer = BufferedGraphicsManager.Current.Allocate(
                graphics,
                new Rectangle(
                    (int)graphics.VisibleClipBounds.X,
                    (int)graphics.VisibleClipBounds.Y,
                    (int)graphics.VisibleClipBounds.Width,
                    (int)graphics.VisibleClipBounds.Height));

            // Transforms our coordinate system to make it -1 <= X <= 1, -1 <= Y <= 1
            // AKA a 2x2 grid starting at top left -1, 1 and ending at bottom right 1, -1
            // AKA the cartesian coordinate system that we all know and love
            buffer.Graphics.ScaleTransform(graphics.VisibleClipBounds.Width / 2f, graphics.VisibleClipBounds.Height / 2f);
            buffer.Graphics.TranslateTransform(1, 1);
            buffer.Graphics.ScaleTransform(1.0F, -1.0F);
            positions = InitializePositions().ToArray();
        }

        public IEnumerable<Position> Lights => positions;

        public Task SetColors(IEnumerable<RGB> colors, CancellationToken token)
        {
            return Task.Run(() =>
            {
                var i = 0;
                foreach (var color in colors)
                {
                    var position = positions[i];

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    buffer.Graphics.FillRectangle(
                        new SolidBrush(Color.FromArgb((int)color.R, (int)color.G, (int)color.B)),
                        position.X, position.Y, 2f / count, 2f / count);

                    i++;
                }

                buffer.Render();
            }, cancellationToken: token);
        }

        private IEnumerable<Position> InitializePositions()
        {
            var stepSize = 2f / count;
            for (var i = 0; i < count; i++)
            {
                for (var j = 0; j < count; j++)
                {
                    var x = -1f + i * stepSize;
                    var y = -1f + j * stepSize;
                    yield return new Position(x, y);
                }
            }
        }
    }
}
