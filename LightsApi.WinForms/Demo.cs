using LightsApi.LightSources;
using LightsApi.Transitions;
using System;
using System.Windows.Forms;

namespace LightsApi.WinForms
{
    public partial class Demo : Form
    {
        private GraphicsLightClient client;

        private LightClientLoop loop;

        public Demo()
        {
            InitializeComponent();

            Disposed += Demo_Disposed;
        }

        private void Demo_Disposed(object sender, EventArgs e)
        {
            loop.Stop();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var lightSource = new LayeredLightSource(
                new AngleFilterLightSource(
                    new FadedCircleLightSource(RGB.Red, 0, 0, .5, .25),
                    20, 
                    100),
                new AngleFilterLightSource(
                    new FadedCircleLightSource(RGB.Green, 0, 0, .5, .25),
                    120, 
                    120),
                new AngleFilterLightSource(
                    new FadedCircleLightSource(RGB.Blue, 0, 0, .5, .25),
                    240, 
                    140)
                );

            loop.Transition(new LightSourceTransition(lightSource, 2000));
        }

        private void Demo_Load(object sender, EventArgs e)
        {
            client = new GraphicsLightClient(pictureBox1.CreateGraphics(), 100);
            loop = new LightClientLoop(client);

            loop.Start();
        }
    }
}
