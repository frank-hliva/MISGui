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
            var windowStorage = new RegistryStorage(appContext, defaultStore: "Window");
            var locationsStorage = new RegistryStorage(appContext, defaultStore: "Locations");
            var window = new MainWindow(appContext, windowStorage, locationsStorage);
            window.Show();
        }
    }

}
