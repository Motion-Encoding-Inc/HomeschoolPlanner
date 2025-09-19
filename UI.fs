[<AutoOpen>]
module ScholarsForge.UI

open FSharp.Data.Adaptive
open Fun.Blazor
open Microsoft.FluentUI.AspNetCore.Components


type Cmd<'Msg> =
    | NoCmd
    | Cmd of action:(('Msg -> unit) -> Async<unit>)

let createDispatch (state:'State cval) update : 'Message -> unit =
        let rec dispatch msg = transact(fun _ ->
            state.Value <-
                match update state.Value msg with
                | NoCmd,state' -> state'
                | Cmd f,state' ->
                    f dispatch |> Async.Start
                    state'
            )
        dispatch

type App = {
    app : NodeRenderFragment
    title : string
}

type IGlobalStore with
    member store.NameList = store.CreateCVal("NameList",["dog";"cat"])

let listView (items:'T list aval) =
    adaptiview() {
        let! items = items
        div {
            childContent [
                for i in items do
                    p { $"{i}" }
            ]
        }
    }
