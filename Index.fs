namespace ScholarsForge

open Microsoft.AspNetCore.Mvc.Rendering
open Fun.Blazor
open FSharp.Data.Adaptive
open Microsoft.FluentUI.AspNetCore.Components


type Index() =
    inherit FunBlazorComponent()

    let drawerOpen = cval true
    let toggle (v,setter) : _ -> unit =
        fun _ ->
            setter (not v)
    let defaultApp = {
        title = "Default"
        app =
            div {
                "Please select a Workflow from the left"
                adaptiview() {
                    let! opened = drawerOpen
                    let! drawerOpened = drawerOpen.WithSetter()
                    p { $"drawerOpen: {opened}" }
                    FluentButton'() {
                        onclick (toggle drawerOpened)
                        if opened then "Close Drawer" else "Open Drawer"
                    }
                }
            }
    }
    let selectedApp = cval defaultApp
    let layout (apps:App list) (selectedApp:App cval) =
        adaptiview() {
            FluentToastProvider'.create()
            FluentDialogProvider'.create()
            FluentTooltipProvider'.create()
            FluentMessageBarProvider'.create()
            FluentMenuProvider'.create()
            //app
            let! drawerOpen = drawerOpen.WithSetter()
            let! selectedApp,setSelectedApp = selectedApp.WithSetter()
            FluentMainLayout'() {                
                Header (adaptiview() {
                    h3 { "App Header" }
                })
                SubHeader (adaptiview() {
                    h5 { "App SubHeader" }
                })
                NavMenuWidth 275
                NavMenuContent (adaptiview() {
                    //h5 { "App NavMenuContent" }
                    for a in apps do
                    FluentNavLink'() {
                        onclick (fun _ -> setSelectedApp a)
                        a.title
                    }
                })
                Body selectedApp.app
            }
            ()
        }        

    override _.Render() =
        layout [AdaptiveExample.app; DispatchExample.app; InjectionExample.app] selectedApp

    static member page ctx =
        fragment {
            doctype "html"
            html' {
                head {
                    title { "Scholar's Forge" }
                    baseUrl "/"
                    meta { charset "utf-8" }
                    meta {
                        name "viewport"
                        content "width=device-width, initial-scale=1.0"
                    }
                    link {
                        href "https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap"
                        rel "stylesheet"
                    }
                    link {
                        href "ScholarsForge.styles.css"
                        rel "stylesheet"
                    }
                    link {
                        href "_content/Microsoft.FluentUI.AspNetCore.Components/css/reboot.css"
                        rel "stylesheet"
                    }
                }
                body {
                    rootComp<Index> ctx RenderMode.ServerPrerendered
                    script { src "_content/Microsoft.FluentUI.AspNetCore.Components/Microsoft.FluentUI.AspNetCore.Components.lib.module.js"; type' "module"; ``async`` () }
                    script { src "_framework/blazor.server.js" }
                }
            }
        }
