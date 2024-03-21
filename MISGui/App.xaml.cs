using LiteDB;
using MIS;
using MIS.Storages;
using System;
using System.Configuration;
using System.Data;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows;
using Unity;
using Unity.Lifetime;

namespace MISGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        IsolatedStorageFile appDbStorageFile = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
        IUnityContainer container = new UnityContainer();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            container.RegisterInstance<MIS.App.Context>(
                MIS.App.createContext(
                    appName: "MISGui"
                )
            );
            container.RegisterInstance<MIS.App.Config>(
                new MIS.App.Config(
                    localhostRoot: "http://localhost:5000"
                )
            );
            container.RegisterFactory<IsolatedStorageFileStream>("app.db", container =>
                new IsolatedStorageFileStream("app.db", FileMode.OpenOrCreate, appDbStorageFile),
                new HierarchicalLifetimeManager()
            );
            container.RegisterType<ILiteDatabase, MIS.Databases.AppDb>();
            container.RegisterType<IBasicStorage, WindowStorage>();
            container.RegisterType<IBasicStorage, LocationsStorage>();
            container.RegisterType<MIS.LocationBuilder>();
            container.RegisterType<MainWindow>();
            container.Resolve<MainWindow>().Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            appDbStorageFile.Dispose();
            base.OnExit(e);
        }
    }
}
