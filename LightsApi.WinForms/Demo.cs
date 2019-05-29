using LightsApi.LightSources;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LightsApi.WinForms
{
    public partial class Demo : Form
    {
        private GraphicsLightClient client;

        private ILightLayout layout;

        public Demo()
        {
            InitializeComponent();

            Disposed += Demo_Disposed;
        }

        private void Demo_Disposed(object sender, EventArgs e)
        {
            client?.Dispose();
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

            layout.Transition(lightSource, TimeSpan.FromMilliseconds(2000));
        }

        private void Demo_Load(object sender, EventArgs e)
        {
            client = new GraphicsLightClient(pictureBox1.CreateGraphics(), 100);
            layout = client.GetLayout().Result;

            client.Start();
        }
    }
}
