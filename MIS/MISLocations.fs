namespace MIS

open System

type MISLocations(sourceUrl: Uri) =

    let localhostRoot = "http://localhost:3000"

    let toLocalhostUri (mainUrl : Uri) = 
        $"{localhostRoot}{mainUrl.PathAndQuery}" |> Uri

    let toSpaceUri localhostUrl = 
        $"{localhostRoot}/ims/html2/admin/space.html" |> Uri

    let toRunLocalhostCommand (mainUrl : Uri) = 
        $"npm run local-dev -- --url {mainUrl.Scheme}{Uri.SchemeDelimiter}{mainUrl.Host}:{mainUrl.Port}{mainUrl.LocalPath} --reload"

    member locs.Source = sourceUrl
    member locs.Localhost = sourceUrl |> toLocalhostUri
    member locs.Space = sourceUrl |> toSpaceUri
    member locs.RunLocalhostCommand = sourceUrl |> toRunLocalhostCommand

    new (sourceUrl : string) =
        MISLocations(sourceUrl |> Uri)