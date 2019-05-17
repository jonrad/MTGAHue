using LightsApi.LightSources;
using System;
using System.Windows.Forms;

namespace LightsApi.WinForms
{
    public partial class Demo : Form
    {
        private GraphicsLightLayout layout;

        public Demo()
        {
            InitializeComponent();

            Disposed += Demo_Disposed;
        }

        private void Demo_Disposed(object sender, EventArgs e)
        {
            layout.Dispose();
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

            layout.Transition(lightSource, TimeSpan.FromMilliseconds(10));
        }

        private void Demo_Load(object sender, EventArgs e)
        {
            layout = new GraphicsLightLayout(pictureBox1.CreateGraphics());
        }
    }
}
