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
                    if (x == 0D && y == 0D)
                    {
                        Console.WriteLine("HERE");
                    }
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
                new[] { RGB.Red, RGB.Blue},
                new[] { RGB.Green, RGB.Black, RGB.White });

            var count = 100;

            var (boxWidth, boxHeight) = ((float)pictureBox1.Width / count, (float)pictureBox1.Height / count);
            using (var graphics = pictureBox1.CreateGraphics())
            {
                for (var i = 0; i < count; i++)
                {
                    for (var j = 0; j < count; j++)
                    {
                        var x = -1f + (i + 1) / (float)count * 2f;
                        var y = -1f + (j + 1) / (float)count * 2f;

                        var color = lightSource.Calculate(x, y);

                        //I forget the API to change coordinate systems in winforms
                        //so this will do for now
                        graphics.FillRectangle(
                            new SolidBrush(Color.FromArgb(color.R, color.G, color.B)),
                            i * boxWidth, j * boxHeight, boxWidth, boxHeight);
                    }
                }
            }
        }
    }
}
