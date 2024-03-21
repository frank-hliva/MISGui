namespace MIS.Storages

type IBasicStorage =
    abstract member GetValue : key:string -> string
    abstract member SetValue : key:string * value:string -> unit
    abstract member With : defaultStore:string -> IBasicStorage