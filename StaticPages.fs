namespace ScholarsForge

open System
open Fun.Blazor
open ScholarsForge.Components

[<RequireQualifiedAccess>]
type StaticNav =
    { Label: string
      Href: string
      External: bool }
    static member Create(label, href, ?external) =
        { Label = label
          Href = href
          External = defaultArg external false }

module StaticAssets =
    let themeBootstrapScript =
        """
/* Scholar/Forge first-visit default + live-follow if no user choice */
(function () {
  var KEY = 'sf_theme';              // persisted user choice, if any
  var root = document.documentElement;
  var saved = null;

  try { saved = localStorage.getItem(KEY); } catch(_) {}

  // Decide initial mode
  var mode = saved;
  if (!mode) {
    // If the browser reports a preference, map dark→forge, light→scholar
    if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
      mode = 'forge';
    } else {
      // Unknown/unsupported ⇒ default to Scholar (light)
      mode = 'scholar';
    }
  }

  root.setAttribute('data-theme', mode);

  // If the user hasn't chosen yet, reflect OS changes live
  if (!saved && window.matchMedia) {
    var mql = window.matchMedia('(prefers-color-scheme: dark)');
    var onChange = function (e) {
      // Only follow if the user still hasn't picked a theme
      try { if (!localStorage.getItem(KEY)) {
        root.setAttribute('data-theme', e.matches ? 'forge' : 'scholar');
      }} catch(_) {}
    };
    if (mql.addEventListener) mql.addEventListener('change', onChange);
    else if (mql.addListener) mql.addListener(onChange); // Safari/legacy
  }
})();
"""

    let themePersistScript =
        """
/* Segmented “Scholar / Forge” toggle → persist user choice */
(function () {
  var KEY = 'sf_theme';
  var root = document.documentElement;
  var bScholar = document.getElementById('mode-scholar');
  var bForge   = document.getElementById('mode-forge');

  function setMode(mode, persist) {
    root.setAttribute('data-theme', mode);
    if (persist) {
      try { localStorage.setItem(KEY, mode); } catch(_) {}
    }
    if (bScholar && bForge) {
      bScholar.classList.toggle('active', mode === 'scholar');
      bForge.classList.toggle('active',   mode === 'forge');
      bScholar.setAttribute('aria-selected', mode === 'scholar');
      bForge.setAttribute('aria-selected',   mode === 'forge');
    }
  }

  if (bScholar) bScholar.addEventListener('click', function(){ setMode('scholar', true); });
  if (bForge)   bForge.addEventListener('click',   function(){ setMode('forge',   true); });

  // Ensure the segmented control reflects whatever the head script chose
  setMode(root.getbute('data-theme') || 'scholar', false);
})();
"""

    let navLinks =
        [ 
            StaticNav.Create("Blog", "https://scholarsforge.blogspot.com/", true)
            StaticNav.Create("About Us", "/about")
            StaticNav.Create("Contact Us", "/contact") 
        ]

    let navLink (activeHref: string option) (link:StaticNav) =
        let normalizeHref (href: string) =
            if String.IsNullOrWhiteSpace href then "/"
            elif href.StartsWith "/" then href
            else "/" + href.Trim()

        let isActive =
            activeHref
            |> Option.map (fun h -> String.Equals(normalizeHref h, normalizeHref link.Href, StringComparison.OrdinalIgnoreCase))
            |> Option.defaultValue false

        a {
            href link.Href
            domAttr {
                if link.External then
                    "target", "_blank"
                if link.External then
                    "rel", "noopener"
                if isActive then
                    "aria-current", "page"
            }
            link.Label
        }

    let header activeHref =
        header {
            div {
                class' "container nav"
                "role", "navigation"
                "aria-label", "Primary"
                a {
                    class' "brand"
                    href "/"
                    "aria-label", "Scholar’s Forge Planner"
                    span {
                        class' "mark"
                        "aria-hidden", "true"
                        img { src "images/ico.png"; alt "" }
                    }
                    span { "Scholar’s Forge Planner" }
                }
                div {
                    class' "links"
                    for link in navLinks do
                        navLink activeHref link
                    div {
                        class' "mode-switch"
                        "role", "tablist"
                        "aria-label", "Theme mode"
                        button {
                            id "mode-scholar"
                            "role", "tab"
                            "aria-selected", "true"
                            "Scholar"
                        }
                        button {
                            id "mode-forge"
                            "role", "tab"
                            "aria-selected", "false"
                            "Forge"
                        }
                    }
                }
            }
        }

    let footer =
        footer {
            div {
                class' "container foot"
                div {
                    class' "brand"
                    span {
                        class' "mark"
                        "aria-hidden", "true"
                        img { src "images/ico.png"; alt "" }
                    }
                    span { "Scholar’s Forge Planner" }
                }
                div {
                    for index, link in navLinks |> List.indexed do
                        if index > 0 then
                            span { class' "sep"; " • " }
                        navLink None link
                }
                div {
                    $"© {DateTime.UtcNow.Year} Scholar’s Forge LLC. All rights reserved."
                }
            }
        }

    let staticDocument (docTitle:string) description activeHref (bodyContent:NodeRenderFragment) =
        fragment {
            doctype "html"
            html' {
                lang "en"
                head {
                    title { docTitle }
                    meta { charset "utf-8" }
                    meta { name "viewport"; content "width=device-width, initial-scale=1" }
                    if not (String.IsNullOrWhiteSpace description) then
                        meta { name "description"; content description }
                    meta { name "theme-color"; content "#f9f6f3" }
                    link { rel "icon"; type' "image/png"; href "images/icowbkg.png" }
                    link { rel "stylesheet"; href "landing.css" }
                    script { themeBootstrapScript }
                }
                body {
                    header activeHref
                    bodyContent
                    footer
                    script { src "landing.js" }
                    script { themePersistScript }
                }
            }
        }

module LandingPage =
    let carouselSlides =
        [ "images/LearningThroughPlay.png"
          "images/MessyDayKitchenTableSchool.png"
          "images/DiscoveryMoments.png"
          "images/SiblingTeaching.png"
          "images/WorksideLearning.png"
          "images/IndependantButSupported.png"
          "images/IndependantButSupported2.png" ]

    let immersiveShowcase =
        ScrollBackgroundCarousel.render carouselSlides

    let heroSection =
        section {
            class' "container hero"
            "aria-labelledby", "hero-title"
            div {
                br {}
                h1 { id "hero-title"; "Homeschool"; br {}; "Your Way" }
                p {
                    class' "lead"
                    "No one will love your kids like you do—why trust anyone else to raise them? Scholar’s Forge gives you tools to "
                    strong { "plan flexibly" }
                    ", "
                    strong { "document confidently" }
                    ", and help students take ownership of learning."
                }
                div {
                    class' "hero-card"
                    "aria-label", "Highlights"
                    ul {
                        class' "hero-bullets"
                        li {
                            strong { "Plan" }
                            " from books, time, or custom activities"
                        }
                        li {
                            strong { "Skip • Push • Catch-up • Continue" }
                            " — real-life rescheduling"
                        }
                        li {
                            strong { "Portfolio evidence" }
                            " & weekly reports"
                        }
                        li {
                            strong { "Clear progress" }
                            " by subject"
                        }
                    }
                    div {
                        class' "pills"
                        style' "margin-top:1rem"
                        a {
                            class' "btn primary"
                            href "#beta"
                            "Join the December beta"
                        }
                        a {
                            class' "btn"
                            href "#how"
                            "How it works"
                        }
                    }
                }
            }
            div {
                class' "carousel"
                "aria-label", "Screenshots"
                for index, path in carouselSlides |> List.indexed do
                    div {
                        class' (if index = 0 then "slide active" else "slide")
                        style' ($"background-image:url('{path}')")
                    }
                div {
                    class' "dots"
                    for index in 0 .. carouselSlides.Length - 1 do
                        span { class' (if index = 0 then "dot active" else "dot") }
                }
            }
            br {}
            br {}
        }

    let realLifeSection =
        section {
            class' "container"
            id "how"
            "aria-labelledby", "how-title"
            br {}
            h2 { id "how-title"; "Designed around how families actually school" }
            p {
                class' "muted"
                "Not a bell schedule—our "
                em { "Week Preview" }
                " and "
                em { "Day Detail" }
                " make it obvious what’s planned, what’s done, and what’s next. "
            }
            div {
                class' "grid-2"
                div {
                    class' "card"
                    h3 { "Plan the way you teach" }
                    p {
                        class' "muted"
                        "Books, time blocks, or custom activities. Add Saxon, schedule free reading 30 minutes, or log that grocery-store math. "
                    }
                    div {
                        class' "bullet"
                        span { class' "dotline" }
                        span {
                            strong { "Flexible pacing:" }
                            " "
                            em { "Skip" }
                            " to push the timeline, "
                            em { "Catch-up" }
                            " to keep the deadline, or "
                            em { "Do Extra" }
                            " to pull work forward. "
                        }
                    }
                    div {
                        class' "bullet"
                        span { class' "dotline" }
                        span {
                            strong { "Student agency:" }
                            " learners can choose order, skip, or double—with parent guardrails. "
                        }
                    }
                }
                div {
                    class' "card"
                    h3 { "Documentation that makes sense" }
                    div {
                        class' "bullet"
                        span { class' "dotline" }
                        span {
                            strong { "Portfolio evidence:" }
                            " photos/files/notes on any completion; export weekly summaries (CSV/PDF). "
                        }
                    }
                    div {
                        class' "bullet"
                        span { class' "dotline" }
                        span {
                            strong { "Progress at a glance:" }
                            " bars & pace indicators keep everyone aligned. "
                        }
                    }
                }
            }
            br {}
            br {}
        }

    let featuresSection =
        section {
            class' "container"
            id "features"
            "aria-labelledby", "features-title"
            br {}
            h2 { id "features-title"; "Everything you need to start" }
            div {
                class' "grid-3"
                let cards =
                    [ "⌛Fast planning", "Paste chapters or add time blocks in minutes; auto-schedule to your allowed days."
                      "🔄Real-world rescheduling", "Skip, push, catch-up, or do extra—without breaking your plan. "
                      "🗒️Portfolio & reports", "Evidence gallery and exportable PDFs/CSVs. "
                      "📊Progress by subject", "On-track, ahead, or catching-up at a glance. "
                      "👫Parent-first privacy", "Kids don’t need logins for MVP; you control exports. " ]
                for title, copy in cards do
                    div {
                        class' "card"
                        strong { title }
                        p { class' "muted"; copy }
                    }
            }
            br {}
            br {}
        }

    let betaSection =
        section {
            class' "container"
            id "beta"
            "aria-labelledby", "beta-title"
            br {}
            div {
                class' "grid-2"
                div {
                    h2 { id "beta-title"; "Beta testers wanted for December" }
                    p {
                        class' "muted"
                        "We’re targeting December for testing. Join the list to try Scholar’s Forge Planner. Share what you love and what you don’t, and shape the roadmap."
                    }
                    form {
                        class' "beta-form"
                        "aria-label", "Beta sign-up"
                        input { type' "email"; placeholder "you@example.com"; "required", "" }
                        button { class' "btn primary"; type' "submit"; "Add me to the beta" }
                    }
                }
                div {
                    class' "card"
                    h3 { "What we’ll ask from you" }
                    ul {
                        class' "muted"
                        li { "Use the planner with real learners for a couple of weeks." }
                        li { "Send feedback on planning, rescheduling, and reports." }
                        li { "Tell us what to improve or add next." }
                    }
                }
            }
            br {}
            br {}
        }

    let aboutSection =
        section {
            class' "container"
            id "about"
            "aria-labelledby", "about-title"
            br {}
            h2 { id "about-title"; "About us" }
            div {
                class' "grid-2"
                div {
                    class' "card"
                    p {
                        strong { "Founded by a former homeschooling mom of four" }
                        ", Scholar’s Forge is built with a deep respect for family-first education. We’ve taught, tested, and iterated, so you can stay focused on your kids."
                    }
                }
                div {
                    class' "card"
                    h3 { "Contact us" }
                    p {
                        class' "muted"
                        "Conference organizers, co-ops, and partners, we’d love to connect. Speaking opportunities welcome."
                    }
                    p {
                        a { href "mailto:support@scholarsforge.com"; "support@scholarsforge.com" }
                    }
                }
            }
            br {}
            br {}
        }

    let pricingSection =
        section {
            class' "container"
            id "pricing"
            "aria-labelledby", "pricing-title"
            br {}
            h2 { id "pricing-title"; "Simple, transparent pricing" }
            div {
                class' "grid-2"
                div {
                    class' "card"
                    h3 { "👨‍👩‍👧‍👦 Family (unlimited children)" }
                    span { class' "btn disabled"; "aria-disabled", "true"; "Coming soon" }
                }
                div {
                    class' "card"
                    h3 { "🏫 Pro (charters and co-ops)" }
                    span { class' "btn disabled"; "aria-disabled", "true"; "Coming soon" }
                }
            }
            p { class' "muted mt-12"; "No credit card required to start. Cancel anytime." }
            br {}
            br {}
        }

    let view =
        fragment {
            immersiveShowcase
            main {
                heroSection
                realLifeSection
                featuresSection
                betaSection
                aboutSection
                pricingSection
            }
        }

    type Component() =
        inherit FunBlazorComponent()
        override _.Render() =
            view

    let page ctx =
        StaticAssets.staticDocument
            "Scholar’s Forge Planner — Homeschool, Your Way"
            "Plan from books, time, or custom activities. Skip → Catch-up or Do Extra with a tap. Track progress and export clean reports—homeschool, your way."
            (Some "/")
            view

module AboutPage =
    let view =
        main {
            section {
                class' "container"
                id "about"
                "aria-labelledby", "about-title"
                br {}
                h1 { id "about-title"; "About Us" }
                div {
                    class' "grid-2"
                    div {
                        class' "card"
                        p {
                            strong { "Founded by a former homeschooling mom of four" }
                            ", Scholar’s Forge is built with a deep respect for family-first education. We’ve taught, tested, and iterated, so you can stay focused on your kids."
                        }
                    }
                    div {
                        class' "card"
                        h3 { "Contact us" }
                        p {
                            class' "muted"
                            "Conference organizers, co-ops, and partners, we’d love to connect. Speaking opportunities welcome."
                        }
                        p { a { href "mailto:support@scholarsforge.com"; "support@scholarsforge.com" } }
                    }
                }
                br {}
                br {}
            }
        }

    type Component() =
        inherit FunBlazorComponent()
        override _.Render() =
            view

    let page ctx =
        StaticAssets.staticDocument
            "About Us — Scholar’s Forge Planner"
            "Learn about the Scholar’s Forge Planner team."
            (Some "/about")
            view

module ContactPage =
    let view =
        main {
            section {
                class' "container"
                id "contact"
                "aria-labelledby", "contact-title"
                br {}
                h1 { id "contact-title"; "Contact Us" }
                p {
                    class' "muted"
                    "Conference organizers, co-ops, and partners, we’d love to connect. Speaking opportunities welcome."
                }
                p { a { href "mailto:support@scholarsforge.com"; "support@scholarsforge.com" } }
                br {}
                br {}
            }
        }

    type Component() =
        inherit FunBlazorComponent()
        override _.Render() =
            view

    let page ctx =
        StaticAssets.staticDocument
            "Contact Us — Scholar’s Forge Planner"
            "Reach out to the Scholar’s Forge Planner team."
            (Some "/contact")
            view
