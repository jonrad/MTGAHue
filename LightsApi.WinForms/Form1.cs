using MTGAHue;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LightsApi.WinForms
{
    public partial class Form1 : Form
    {
        // Imagine the arena broken up into a grid, where X = -1 is the far left, X = 1 the far right
        // Y = 1 is the furthest in front (Where the opponent is), Y = -1 is the very back (Where the player is)
        // We can think of an Arena as two halves: the opponent and the player
        // The opponent we'll consider the area defined as Y > 0 and the player is Y < 0
        // Then each side has one or more colors. If we imagine the light coming from the LINE of where the player sits
        // (Eg for Opponent where Y = 1 or for the player Y = -1)
        // We can color that line based on the colors specified (For a single color it would be just a line of that color,
        // for two colors we'd split the line where the left is the first color, the right is the second color, 
        // and the middle is a mix of the two, etc). As we get closer to the middle, the brightness gets lower
        // until the middle where it's dull
        public class ArenaLightSource : ILightSource
        {
            private CompositeLightSource compositeLightSource;

            public ArenaLightSource(RGB[] opponentColors, RGB[] playerColors)
            {
                var radius = 2D;
                compositeLightSource = new CompositeLightSource(
                    new LineLightSource(
                        new CompositeLightSource(BuildLightSources(opponentColors, 1).ToArray()),
                        1,
                        radius),
                    new LineLightSource(
                        new CompositeLightSource(BuildLightSources(playerColors, -1).ToArray()),
                        -1,
                        radius));
            }

            private IEnumerable<ILightSource> BuildLightSources(RGB[] colors, double y)
            {
                var step = 2D / (colors.Length + 1);

                var radius = 2D / colors.Length;
                double position = -1;

                for (var i = 0; i < colors.Length; i++)
                {
                    position += step;
                    yield return new AreaLightSource(colors[i], position, y, radius);
                }
            }

            public RGB Calculate(double x, double y)
            {
                return compositeLightSource.Calculate(x, y);
            }

            private class LineLightSource : ILightSource
            {
                private readonly double lineY;

                private readonly double radius;

                private readonly ILightSource lightSource;

                public LineLightSource(ILightSource lightSource, double y, double radius)
                {
                    this.lineY = y;
                    this.radius = radius;
                    this.lightSource = lightSource;
                }

                public RGB Calculate(double x, double y)
                {
                    var distance = Math.Abs(lineY - y);
                    var multiplier = (radius - distance) / radius;

                    var rgb = lightSource.Calculate(x, lineY);
                    return rgb * multiplier;
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var lightSource = new ArenaLightSource(
                new[] { RGB.Green },
                new[] { RGB.Blue, RGB.Red });

            var count = 100;

            using (var graphics = pictureBox1.CreateGraphics())
            {
                // Transforms our coordinate system to make it -1 <= X <= 1, -1 <= Y <= 1
                // AKA a 2x2 grid starting at top left -1, -1 and ending at bottom right 1, 1
                graphics.ScaleTransform(pictureBox1.Width / 2f, pictureBox1.Height / 2f);
                graphics.TranslateTransform(1, 1);

                var stepSize = 2f / count;
                for (var i = 0; i < count; i++)
                {
                    for (var j = 0; j < count; j++)
                    {
                        var x = -1f + i * stepSize;
                        var y = -1f + j * stepSize;

                        var color = lightSource.Calculate(x + stepSize / 2, y + stepSize / 2);

                        graphics.FillRectangle(
                            new SolidBrush(Color.FromArgb(color.R, color.G, color.B)),
                            x, y, 2f / count, 2f / count);
                    }
                }
            }
        }
    }
}
