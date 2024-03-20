module MIS.App

type Context =
    {
        AppName : string
    }

let createContext appName =
    {
        AppName = appName
    }