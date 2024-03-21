namespace MIS.Databases

open Unity
open LiteDB
open System.IO.IsolatedStorage

type AppDb([<Dependency("app.db")>] isoStream: IsolatedStorageFileStream) =
    inherit LiteDatabase(isoStream)