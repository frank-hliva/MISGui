namespace MIS.Storages

open MIS
open Microsoft.Win32
open System

type RegistryStorage(appName: string, defaultStore: string) =
    interface IBasicStorage with
        member s.GetValue(key: string) = s.ReadFromRegistry(defaultStore, key)
        member s.SetValue(key: string, value: string) = s.WriteToRegistry(defaultStore, key, value)
        member s.With(defaultStore: string) = RegistryStorage(appName, defaultStore)

    new(context: App.Context, defaultStore: string) = RegistryStorage(context.AppName, defaultStore)

    member s.GetValue(key: string) = (s :> IBasicStorage).GetValue(key)
    member s.SetValue(key: string, value: string) = (s :> IBasicStorage).SetValue(key, value)
    member s.With(defaultStore: string) = (s :> IBasicStorage).With(defaultStore)

    member private s.ReadFromRegistry(store: string, key: string) =
        let mutable value = null
        let appKey = Registry.CurrentUser.OpenSubKey(sprintf "SOFTWARE\\%s" appName)
        if appKey <> null then
            let valueKey = appKey.OpenSubKey(store)
            if valueKey <> null then
                value <- valueKey.GetValue(key)
                valueKey.Close()
            appKey.Close()
        value :?> string

    member private s.WriteToRegistry(store: string, key: string, value: string) =
        let appKey = Registry.CurrentUser.CreateSubKey(sprintf "SOFTWARE\\%s" appName)
        let valueKey = appKey.CreateSubKey(store)
        valueKey.SetValue(key, value)
        valueKey.Close()
        appKey.Close()
    