namespace MIS

open System

type MyWork =
    {
        Locations : Locations
    }

and Locations =
    {
        Source : Uri
        Localhost : Uri
        LocalhostWithoutQuery : Uri
        Space : Uri
        RunLocalhostCommand : string
    }


type LocationBuilder(config : App.Config) =

    let localhostRoot = "http://localhost:3000"

    let toLocalhostUri (mainUrl : Uri) = 
        $"{localhostRoot}{mainUrl.PathAndQuery}" |> Uri

    let toLocalhostWithoutQueryUri (mainUrl : Uri) = 
        $"{localhostRoot}{mainUrl.LocalPath}" |> Uri

    let toSpaceUri localhostUrl = 
        $"{localhostRoot}/ims/html2/admin/space.html" |> Uri

    let toRunLocalhostCommand (mainUrl : Uri) = 
        $"npm run local-dev -- --url {mainUrl.Scheme}{Uri.SchemeDelimiter}{mainUrl.Host}:{mainUrl.Port}{mainUrl.LocalPath} --reload"

    member locs.GetAllLocationsFor(sourceUrl: Uri) =
        {
            Source = sourceUrl
            Localhost = sourceUrl |> toLocalhostUri
            LocalhostWithoutQuery = sourceUrl |> toLocalhostWithoutQueryUri
            Space = sourceUrl |> toSpaceUri
            RunLocalhostCommand = sourceUrl |> toRunLocalhostCommand
        }

    member locs.GetAllLocationsFor(sourceUrl: string) =
        sourceUrl
        |> Uri
        |> locs.GetAllLocationsFor