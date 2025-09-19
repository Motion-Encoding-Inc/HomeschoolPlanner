// hot-reload
// hot-reload is the flag to let cli know this file should be included
// It has dependency requirement: the root is the app which is used in the Index.fs
// All other files which want have hot reload, need to drill down to that file, and all the middle file should also add the '// hot-reload' flag at the top of that file
[<AutoOpen>]
module ScholarsForge.InjectionExample

open FSharp.Data.Adaptive
open Fun.Blazor
open Microsoft.FluentUI.AspNetCore.Components

type State =
    | Loading of msg:string
    | Ready of int

let startJob(waitTime:int, state:State cval) =
    async {
        transact(fun _ -> state.Value <- Loading "Starting ...")
        do! Async.Sleep(waitTime)
        let v = System.Random().Next(1000,10000)
        transact(fun _ -> state.Value <- Ready v)
    } |> Async.StartImmediate
        

let button (state:State cval) =
    let disabled' = match state.Value with | Loading _ -> true | _ -> false
    FluentButton'() {
        Disabled disabled'
        onclick (fun _ -> startJob(5000,state))
        "Start"
    }

let app =
    {
        title = "Html.inject example"
        app =
            html.inject(fun (hook:IComponentHook, localStore: IShareStore, globalStore: IGlobalStore) ->
                let state = cval(Ready 0)
                adaptiview () {
                    let! state', _ = state.WithSetter()

                    div {
                        match state' with
                        | Ready count ->
                            div { $"Here is the count {count}" }
                        | Loading msg ->
                            p { style' "color:purple"; msg }
                        button state
                    }
                }
            )
    }
