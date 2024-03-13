namespace MIS.Utils

open System.Runtime.CompilerServices

[<Extension>]
type EnumerableExtensions () =
    
    [<Extension>]
    static member inline ForEach(seq' : seq<_>, action) =
        seq' |> Seq.iter action