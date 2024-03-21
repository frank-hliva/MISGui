module MIS.App

open Unity

type Context =
    {
        AppName : string
    }

let createContext appName =
    {
        AppName = appName
    }

type Config =
    {
        LocalhostRoot : string
    }