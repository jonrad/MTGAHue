using Q42.HueApi.ColorConverters;
using Q42.HueApi.Streaming.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightsApi.Hue
{
    public class HueLight : ILight
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

        public Task Transition(RGB rgb, TimeSpan transitionTime, CancellationToken token)
        {
            var transition = new Transition(new RGBColor(rgb.R, rgb.G, rgb.B), 1, transitionTime);

            light.Transition = transition;
            light.Transition.Start(
                light.State.RGBColor,
                light.State.Brightness,
                token);

            return Task.Delay(transitionTime, token);
        }
    }
}
