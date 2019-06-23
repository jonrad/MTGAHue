using System.Windows.Forms;

namespace MagicLights.UI
{
    public partial class ConfigurationTabPage : UserControl
    {
        private readonly ClientConfigurationModel model;

        public ConfigurationTabPage()
            : this(new ClientConfigurationModel(new NullLightClientProvider()))
        {
        }

        public ConfigurationTabPage(ClientConfigurationModel model)
        {
            InitializeComponent();

            this.model = model;

            checkBox1.Bind(c => c.Checked, () => model.Enabled);
        }

        private void ConfigurationTabPage_Load(object sender, System.EventArgs e)
        {
        }
    }
}
