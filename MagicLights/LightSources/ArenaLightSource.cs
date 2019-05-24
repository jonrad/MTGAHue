using LightsApi;
using LightsApi.LightSources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagicLights.LightSources
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
                PlayerLightSource.Build(1, opponentColors, radius),
                PlayerLightSource.Build(-1, playerColors, radius));
        }

        public RGB Calculate(double x, double y)
        {
            return compositeLightSource.Calculate(x, y);
        }

        private class PlayerLightSource : ILightSource
        {
            public static PlayerLightSource Build(double y, RGB[] colors, double playerRadius)
            {
                var step = 2D / (colors.Length + 1);

                var areaLightSourceRadius = 2D / colors.Length;

                var areaLightSources = 
                    colors.Select((color, i) =>
                    {
                        var position = -1D + step * (i + 1);
                        return new AreaLightSource(colors[i], position, y, areaLightSourceRadius);
                    }).ToArray();

                var composite = new CompositeLightSource(areaLightSources);

                return new PlayerLightSource(composite, y, playerRadius);
            }

            private readonly double lineY;

            private readonly double radius;

            private readonly ILightSource lightSource;

            public PlayerLightSource(ILightSource lightSource, double y, double radius)
            {
                lineY = y;
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
}
