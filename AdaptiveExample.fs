[<AutoOpen>]
module ScholarsForge.AdaptiveExample

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

//let update (state:State) msg =
//    match state, msg with
//    | Ready count, FetchCounter ->
//        fun dispatcher ->
//            async {
//                do! Async.Sleep 1000
//                $"waiting 1s ..." |> UpdateLoadingMsg |> dispatcher
//                do! Async.Sleep 2000
//                $"waiting 3s ..." |> UpdateLoadingMsg |> dispatcher
//                do! Async.Sleep 2000
//                $"waiting 5s ..." |> UpdateLoadingMsg |> dispatcher
//                System.Random.Shared.Next(1000,10000) |> NewCounterValue |> dispatcher
//            }
//        |> Cmd,
//        Loading "Starting..."
//    | Loading _, NewCounterValue count -> NoCmd,Ready count
//    | Loading _, UpdateLoadingMsg msg -> NoCmd,Loading msg
//    | _ -> NoCmd,state // otherwise don't do anything

type IShareStore with
    member store.Counter = store.CreateCVal("AdaptiveExample_counter",Some 0)
    member store.LoadingMessage : string option cval = store.CreateCVal("AdaptiveExample_loadingMessage",None)
    //member store.State = store.CreateCVal("AdaptiveExample_State",Ready 0)

let createState (local:IShareStore) (global':IGlobalStore) =
    aval {
        match! local.Counter with
        | None ->
            match! local.LoadingMessage with
            | None -> return Loading "Starting..."
            | Some msg -> return Loading msg
        | Some counter -> return Ready counter
    }

let fetchCounter (local:IShareStore) =
    async {
        transact(fun _ -> local.Counter.Value <- None)
        do! Async.Sleep 1000
        transact(fun _ -> local.LoadingMessage.Value <- $"waiting 1s ..." |> Some)
        do! Async.Sleep 2000
        transact(fun _ -> local.LoadingMessage.Value <- $"waiting 3s ..." |> Some)
        do! Async.Sleep 2000
        transact(fun _ -> local.LoadingMessage.Value <- $"waiting 5s ..." |> Some)
        do! Async.Sleep 1000
        transact(fun _ -> local.Counter.Value <- System.Random.Shared.Next(1000,10000) |> Some)
    } |> Async.Start

let addName (global':IGlobalStore) =
    async {
        transact(fun _ -> global'.NameList.Value <- $"name{global'.NameList.Value.Length}"::global'.NameList.Value)
    } |> Async.Start

let app =
    {
        title = "Adaptive Example"
        app =
            html.inject(fun (hook:IComponentHook, local: IShareStore, global': IGlobalStore) ->
                let state = createState local global'
                adaptiview () {
                    let! state' = state

                    div {
                        match state' with
                        | Ready count ->
                            div { $"Here is the count {count}" }
                        | Loading msg ->
                            p { style' "color:purple"; msg }

                        FluentButton'() {
                            Disabled (match state' with | Ready _ -> false | _ -> true)
                            onclick (fun _ -> fetchCounter local)
                            "Fetch Counter"
                        }
                    }
                    FluentButton'() {
                        //Disabled (match state' with | Ready _ -> false | _ -> true)
                        onclick (fun _ -> addName global')
                        "Add Name"
                    }
                    listView global'.NameList
                }
            )
    }
