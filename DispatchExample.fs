[<AutoOpen>]
module ScholarsForge.DispatchExample

open FSharp.Data.Adaptive
open Fun.Blazor
open Microsoft.FluentUI.AspNetCore.Components

type State =
    | Ready of count:int
    | Loading of msg:string
type Message =
    | Init
    | FetchCounter
    | NewCounterValue of count:int
    | UpdateLoadingMsg of msg:string
    | AddName of IGlobalStore

let update (state:State) msg =
    match state, msg with
    | Ready count, FetchCounter ->
        fun dispatcher ->
            async {
                do! Async.Sleep 1000
                $"waiting 1s ..." |> UpdateLoadingMsg |> dispatcher
                do! Async.Sleep 2000
                $"waiting 3s ..." |> UpdateLoadingMsg |> dispatcher
                do! Async.Sleep 2000
                $"waiting 5s ..." |> UpdateLoadingMsg |> dispatcher
                System.Random.Shared.Next(1000,10000) |> NewCounterValue |> dispatcher
            }
        |> Cmd,
        Loading "Starting..."
    | Loading _, NewCounterValue count -> NoCmd,Ready count
    | Loading _, UpdateLoadingMsg msg -> NoCmd,Loading msg
    | _, AddName global' ->
        fun _ ->
            async {
                transact(fun _ -> global'.NameList.Value <- $"name{global'.NameList.Value.Length}"::global'.NameList.Value)
            }
        |> Cmd, state
    | _ -> NoCmd,state // otherwise don't do anything

type IShareStore with
    member store.State = store.CreateCVal("DispatchExample_State",Ready 0)

let app =
    {
        title = "Dispatch Example"
        app =
            html.inject(fun (hook:IComponentHook, local: IShareStore, global': IGlobalStore) ->
                let dispatch = createDispatch local.State update
                adaptiview () {
                    let! state', _ = local.State.WithSetter()

                    div {
                        match state' with
                        | Ready count ->
                            div { $"Here is the count {count}" }
                        | Loading msg ->
                            p { style' "color:purple"; msg }

                        FluentButton'() {
                            Disabled (match state' with | Ready _ -> false | _ -> true)
                            onclick (fun _ -> dispatch FetchCounter)
                            "Fetch Counter"
                        }
                    }
                    FluentButton'() {
                        //Disabled (match state' with | Ready _ -> false | _ -> true)
                        onclick (fun _ -> dispatch <| AddName global')
                        "Add Name"
                    }
                    listView global'.NameList
                }
            )
    }
