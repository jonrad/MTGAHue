using Q42.HueApi.ColorConverters;
using Q42.HueApi.Streaming.Models;
using System;
using System.Threading;

namespace LightsApi.Hue
{
    internal class HueLight
    {
        private readonly EntertainmentLight light;

        public HueLight(EntertainmentLight light)
        {
            this.light = light;
            X = light.LightLocation.X;
            Y = light.LightLocation.Y;
        }

        public double X { get; }

        public double Y { get; }

        public void Transition(RGB rgb, TimeSpan transitionTime, CancellationToken token)
        {
            var transition = new Transition(new RGBColor((int)rgb.R, (int)rgb.G, (int)rgb.B), 1, transitionTime);

            light.Transition = transition;
            light.Transition.Start(
                light.State.RGBColor,
                light.State.Brightness,
                token);
        }
    }
}
