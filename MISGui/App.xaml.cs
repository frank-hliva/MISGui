using System.Configuration;
using System.Data;
using System.Windows;

namespace MISGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public class Context
        {
            public readonly string AppName = "MISGui";
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var appContext = new App.Context();
            var windowStorage = new RegistryStorage(appContext, "Window");
            var locationsStorage = new RegistryStorage(appContext, "Locations");
            var window = new MainWindow(appContext, windowStorage, locationsStorage);
            window.Show();
        }
    }

}
