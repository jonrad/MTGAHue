using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using LightsApi.LightSources;

namespace LightsApi.WinForms
{
    public class GraphicsLightLayout : ILightLayout
    {
        private const int count = 100;

        private readonly BufferedGraphics buffer;

        public GraphicsLightLayout(Graphics graphics)
        {
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
        }

        public async Task Transition(
            ILightSource lightSource,
            TimeSpan timeSpan,
            CancellationToken token = default)
        {
            var bufferTask = Task.Run(() =>
            {
                var stepSize = 2f / count;
                for (var i = 0; i < count; i++)
                {
                    for (var j = 0; j < count; j++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            return;
                        }

                        var x = -1f + i * stepSize;
                        var y = -1f + j * stepSize;

                        var color = lightSource.Calculate(x + stepSize / 2, y + stepSize / 2);

                        buffer.Graphics.FillRectangle(
                            new SolidBrush(Color.FromArgb((int)color.R, (int)color.G, (int)color.B)),
                            x, y, 2f / count, 2f / count);
                    }
                }
            });

            await Task.WhenAll(
                bufferTask,
                Task.Delay(timeSpan, token));

            if (token.IsCancellationRequested)
            {
                return;
            }

            buffer.Render();
        }
    }
}
