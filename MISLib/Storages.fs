namespace MIS.Storages

open MIS

type WindowStorage(appContext : App.Context) =
    inherit RegistryStorage(appContext.AppName, "Window")

type LocationsStorage(appContext : App.Context) =
    inherit RegistryStorage(appContext.AppName, "Locations")