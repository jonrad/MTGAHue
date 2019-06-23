using System;
using System.Windows.Forms;

namespace MagicLights.UI
{
    public partial class ConfigurationForm : Form
    {
        private ConfigurationFormModel model;

        public ConfigurationForm()
            : this(new ConfigurationFormModel())
        {
        }

        public ConfigurationForm(ConfigurationFormModel model)
        {
            InitializeComponent();

            this.model = model;

            this.Bind(f => f.TabCount, () => model.Count);
        }

        public int TabCount
        {
            get => tabControl1.TabCount;
            set
            {
                if (TabCount == value)
                {
                    return;
                }

                UpdateTabCount(value);
            }
        }

        private void UpdateTabCount(int value)
        {
            while (tabControl1.TabPages.Count > 0)
            {
                tabControl1.TabPages.RemoveAt(0);
            }

            for (var i = 0; i < value; i++)
            {
                var container = new TabPage();
                var configurationModel = model.Configurations[i];
                var configurationControl = new ConfigurationTabPage(configurationModel);

                container.Controls.Add(configurationControl);
                container.Bind(c => c.Text, () => configurationModel.Id);

                tabControl1.TabPages.Add(container);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Reset(object sender, EventArgs e)
        {
            model.Reset();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
        }
    }
}
