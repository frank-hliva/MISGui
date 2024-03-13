let from whom =
    sprintf "from %s" whom

open System
open System.Diagnostics

type WSLRunningCommand(command : string, proc : Process) = 

    member this.Command = command

    member this.Process = proc

    member this.Stop() =
        proc.Kill()


type WSLCommand(command : string) =
    let command = command

    let outputDataReceived (e: DataReceivedEventArgs) =
        if e.Data <> null then
            printfn "%s" e.Data

    let outputDataReceivedEvent = new Event<DataReceivedEventArgs>()

    member this.OutputDataReceived = outputDataReceivedEvent.Publish

    member this.Command = command

    member this.Start() =
        let proc = new Process()
        proc.StartInfo <- ProcessStartInfo(
            FileName = "wsl",
            Arguments = command,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        )
        proc.OutputDataReceived.Add(fun args -> outputDataReceivedEvent.Trigger(args))
        proc.Start() |> ignore
        proc.BeginOutputReadLine() |> ignore
        proc.BeginErrorReadLine() |> ignore
        WSLRunningCommand(command, proc)

[<EntryPoint>]
let main argv =
    let cmd = WSLCommand(
        command = "npm run local-dev -- --url https://10.111.2.42/ --reload"
    )
    let runnedCmd = cmd.Start();
    Console.ReadKey() |> ignore
    runnedCmd.Stop()
    0
