using LightsApi.LightSources;
using LightsApi.Transitions;
using System;
using System.Windows.Forms;

namespace LightsApi.WinForms
{
    public partial class Demo : Form
    {
        private GraphicsLightClient client;

        private ILights lights;

        public Demo()
        {
            InitializeComponent();

            Disposed += Demo_Disposed;
        }

        private void Demo_Disposed(object sender, EventArgs e)
        {
            lights.Stop();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var lightSource = new LayeredLightSource(
                new AngleFilterLightSource(
                    new FadedCircleLightSource(RGB.Red, 0, 0, .5, .25),
                    0,
                    0,
                    20,
                    100),
                new AngleFilterLightSource(
                    new FadedCircleLightSource(RGB.Green, 0, 0, .5, .25),
                    0,
                    0,
                    120,
                    120),
                new AngleFilterLightSource(
                    new FadedCircleLightSource(RGB.Blue, 0, 0, .5, .25),
                    0,
                    0,
                    240,
                    140)
                );

            var layer = lights.AddLayer();
            layer.Transition(new LightSourceTransition(lightSource, 2000))
                .ContinueWith(c => lights.RemoveLayer(layer));
        }

        private void Demo_Load(object sender, EventArgs e)
        {
            client = new GraphicsLightClient(pictureBox1.CreateGraphics(), 100);
            lights = new Lights(client);

            lights.Start();
        }
    }
}
