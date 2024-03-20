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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var appContext = MIS.App.createContext(
                appName: "MISGui"
            );
            var windowStorage = new MIS.RegistryStorage(appContext, defaultStore: "Window");
            var locationsStorage = new MIS.RegistryStorage(appContext, defaultStore: "Locations");
            var window = new MainWindow(appContext, windowStorage, locationsStorage);
            window.Show();
        }
    }

}
