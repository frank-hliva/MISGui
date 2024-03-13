namespace MIS

open System
open System.Diagnostics

type WSLRunningCommand(command : string, proc : Process) = 
    
    member this.Command = command

    member this.Process = proc

    member this.Stop() =
        proc.Kill()


type WSLCommand(command : string) =
    let command = command

    let outputDataReceivedEvent = new Event<_>()

    let outputDataReceived (e: DataReceivedEventArgs) =
        outputDataReceivedEvent.Trigger(e);
        if e.Data <> null then
            printfn "%s" e.Data

    member this.Command = command

    member this.OutputDataReceived = outputDataReceivedEvent.Publish

    member this.Start() =
        let proc = new Process()
        proc.StartInfo <- ProcessStartInfo(
            FileName = "wsl",
            Arguments = command,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        )
        proc.OutputDataReceived.Add(outputDataReceived)
        proc.Start() |> ignore
        proc.BeginOutputReadLine() |> ignore
        proc.BeginErrorReadLine() |> ignore
        WSLRunningCommand(command, proc)
