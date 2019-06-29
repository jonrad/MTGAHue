using MagicLights.UI.Models;
using System.Windows;

namespace MagicLights.UI
{
    public partial class App : Application
    {
        public App()
            : this(new ConfigurationFormModel())
        {
        }

        public App(ConfigurationFormModel model)
        {
            Model = model;
        }

        public ConfigurationFormModel Model { get; }

        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            this.StartupUri = new System.Uri("MainWindow.xaml", System.UriKind.Relative);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var window = new MainWindow
            {
                DataContext = Model
            };

            window.Show();
        }
    }
}
