namespace ScholarsForge

open System
open Fun.Blazor

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

  // Apply the chosen or default mode to <html data-theme="">
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
  // BUGFIX: getbute → getAttribute, and fall back to 'scholar' if absent.
  setMode(root.getAttribute('data-theme') || 'scholar', false);
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
            class' "site-header" 
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
            class' "site-footer" 
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
        // Core: strict, no options
    let staticDocumentCore
        (docTitle: string)
        (description: string)
        (activeHref: string option)      // header expects option already
        (bodyContent: NodeRenderFragment)
        (bodyClass: string)              // "" = no class
        (includeFooter: bool) =

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
                    // root-relative to avoid route 404s
                    link { rel "icon"; type' "image/png"; href "/images/icowbkg.png" }
                    link { rel "stylesheet"; href "/landing.css" }
                    script { themeBootstrapScript }
                }

                // prebuilt inner body to avoid control-flow inside CEs with custom ops
                let innerBody =
                    fragment {
                        // App shell grid: header / window / footer
                        div {
                            class' "shell"

                            // Top bar (nav)
                            header activeHref

                            // Middle viewport where pages render
                            div {
                                class' "window"
                                bodyContent   // <-- Landing main.v-stage OR About/Contact main
                            }

                            // Bottom bar
                            if includeFooter then
                                footer
                        }

                        // Scripts at end of body
                        script { src "/landing.js" }
                        script { themePersistScript }
                    }

                if String.IsNullOrWhiteSpace bodyClass then
                    body { innerBody }
                else
                    body {
                        class' bodyClass
                        innerBody
                    }
            }
        }

    // wrapper: takes explicit options (NOT ?-args), normalizes, calls core
    let staticDocument
        (docTitle: string)
        (description: string)
        (activeHref: string option)
        (bodyContent: NodeRenderFragment)
        (bodyClassOpt: string option)
        (includeFooterOpt: bool option) =

        let bodyClass     = bodyClassOpt     |> Option.defaultValue ""
        let includeFooter = includeFooterOpt |> Option.defaultValue true

        staticDocumentCore docTitle description activeHref bodyContent bodyClass includeFooter

module LandingPage =
    type LandingSlide =
        { Key: string
          AriaLabel: string
          Classes: string list
          Background: string option
          Anchor: string option
          Content: NodeRenderFragment }

    let heroCarouselSlides =
        [ "images/LearningThroughPlay.png"
          "images/MessyDayKitchenTableSchool.png"
          "images/DiscoveryMoments.png"
          "images/SiblingTeaching.png"
          "images/WorksideLearning.png"
          "images/IndependantButSupported.png"
          "images/IndependantButSupported2.png" ]

    let heroContent =
        div {
            class' "v-center"              // <— centers inside the slide viewport
            div {
                class' "container hero"
                "aria-labelledby", "hero-title"
                div {
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
                    class' "hero-carousel"
                    "aria-label", "Screenshots"
                    for index, path in heroCarouselSlides |> List.indexed do
                        div {
                            class' (if index = 0 then "hero-frame active" else "hero-frame")
                            style' ($"background-image:url('{path}')")
                        }
                    div {
                        class' "hero-dots"
                        for index in 0 .. heroCarouselSlides.Length - 1 do
                            span { class' (if index = 0 then "hero-dot active" else "hero-dot") }
                    }
                }
            }
        }

    let hardContent =
        div {
            class' "v-left-50"   // grid: [ content | spacer ]
            // left content cell
            div {
                class' "container section"
                "aria-labelledby", "hard-title"
                h2 { id "hard-title"; "What Makes Homeschooling Hard?" }
                p {
                    class' "muted"
                    "Spoiler: it isn’t your ability to teach. It’s the tools that don’t fit real life."
                }
                div {
                    class' "grid-1"   // stack pills vertically
                    div {
                        class' "card"
                        h3 { "Documentation fears" }
                        p {
                            class' "muted"
                            "Worry you’ll do it wrong. Requirements can be confusing. "
                            "We help you record what happened in plain language and turn it into reports that make sense."
                        }
                    }
                    div {
                        class' "card"
                        h3 { "Not a public-school bell schedule" }
                        p {
                            class' "muted"
                            "Most planners assume math 9–10, English 10–11, lunch 11:30. "
                            "Homeschooling flows around real life: errands, co-ops, sick days, and field trips."
                        }
                    }
                    div {
                        class' "card"
                        h3 { "Confidence and clarity" }
                        p {
                            class' "muted"
                            "You don’t need more spreadsheets. You need a simple way to see what’s planned, what’s done, and what to do next."
                        }
                    }
                }
            }
            // right spacer cell
            div { () }
        }
    let realLifeContent =
        div {
            class' "v-right-50"   // grid: [ spacer | content ]
            // left spacer cell (empty)
            div { () }
            // right content cell
            div {
                class' "container section"
                "aria-labelledby", "how-title"

                // New heading + tagline
                h2 { id "how-title"; "Why Scholar’s Forge is different" }
                p {
                    class' "muted"
                    em { "Designed around how families actually school—" }
                    "not how institutions schedule."
                }

                // Two feature cards, plus a third card that spans both columns
                div {
                    class' "grid-2"

                    // Column 1
                    div {
                        class' "card"
                        h3 { "Plan the way you teach" }
                        div {
                            class' "bullet"
                            span { class' "dotline" }
                            span { strong { "Books, time, or custom activities:" }; " Add a book, a timed habit, or a one-off activity." }
                        }
                        div {
                            class' "bullet"
                            span { class' "dotline" }
                            span {
                                strong { "Flexible pacing:" }
                                " Life happens—"
                                em { "Skip" }
                                " to push the timeline, "
                                em { "Catch-up" }
                                " to keep the deadline, or "
                                em { "Do Extra" }
                                " today to get ahead."
                            }
                        }
                        div {
                            class' "bullet"
                            span { class' "dotline" }
                            span { strong { "Student agency:" }; " let learners choose the order, skip, double, or push—with guardrails you set—to build responsibility." }
                        }
                    }

                    // Column 2
                    div {
                        class' "card"
                        h3 { "Documentation that makes sense" }
                        div {
                            class' "bullet"
                            span { class' "dotline" }
                            span { strong { "Portfolio evidence:" }; " Attach photos, files, links, and notes to any completion; export polished weekly summaries." }
                        }
                        div {
                            class' "bullet"
                            span { class' "dotline" }
                            span { strong { "Progress at a glance:" }; " Subject progress bars and pacing indicators keep everyone aligned." }
                        }
                        div {
                            class' "bullet"
                            span { class' "dotline" }
                            span { strong { "AI-assisted logging:" }; " Tell the app what you did; it turns that into clean records and reports - Without the busywork." }
                        }
                    }

                    // Spanning card (third “pill”) — sits under both columns
                    div {
                        class' "card span-2"
                        h3 { "Freedom to school your way" }
                        p {
                            class' "muted"
                            "Classical, Montessori, or Desk-and-Bell—no judgment. Focus on Latin, violin, or drill math facts; Scholar’s Forge adapts to your priorities."
                        }
                    }
                }
            }
        }

    let featuresContent =
        div {
            class' "v-center"        // vertically center the whole stack within the slide
            div {
                class' "features-wrap"
                "aria-labelledby", "features-title"

                // Row 1: title (centered by .features-wrap)
                h2 { id "features-title"; "Everything you need to start" }

                // keep the data as tuples
                let allCards : (string * string) list =
                    [ "⌛ Fast planning",           "Paste chapters or add time blocks in minutes; auto-schedule to allowed days."
                      "🔄 Real-world rescheduling", "Skip, push, catch-up, or do extra—without breaking your plan."
                      "🗒️ Portfolio & reports",     "Evidence gallery and exportable PDFs/CSVs."
                      "📊 Progress by subject",     "On-track, ahead, or catching-up at a glance."
                      "👫 Parent-first privacy",    "Kids don’t need logins for MVP; you control exports." ]

                let top3, last2 = List.splitAt 3 allCards

                // row of 3
                div {
                  class' "features-row-3"
                  for (title, copy) in top3 do
                    div { class' "card"; strong { title }; p { class' "muted"; copy } }
                }

                // row of 2
                div {
                  class' "features-row-2"
                  for (title, copy) in last2 do
                    div { class' "card"; strong { title }; p { class' "muted"; copy } }
                }
            }
        }


    let betaContent =
        div {
            class' "v-quarters"                   // 2×2 quarters that fill the slide
            // Card parked in the top-right quarter
            div {
                class' "qr-top-right"
                div {
                    class' "card beta-card"
                    h2 { id "beta-title"; "Beta testers wanted for December" }
                    p {
                        class' "muted"
                        "Try Scholar’s Forge Planner, then tell us what to polish and what to add."
                    }
                    ul {
                        class' "muted"
                        li { "Use it with real learners for a couple of weeks" }
                        li { "Send feedback on planning, rescheduling, and reports" }
                    }
                    form {
                        class' "beta-form mt-8"
                        "aria-label", "Beta sign-up"
                        input { type' "email"; placeholder "you@example.com"; "required", "" }
                        button { class' "btn primary"; type' "submit"; "Add me to the beta" }
                    }
                }
            }
        }


    let pricingContent =
        div {
            class' "v-center"
            div {
                class' "container centered-stack"
                "aria-labelledby", "pricing-title"
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
            }
        }

    let slides : LandingSlide list =
        [ { Key = "hero"
            AriaLabel = "Homeschool, Your Way"
            Classes = [ "hero-slide" ]
            Background = None
            Anchor = None
            Content = heroContent }
          { Key = "hard"
            AriaLabel = "What Makes Homeschooling Hard?"
            Classes = []
            Background = None   // later you can set Some "…" when you have themed images
            Anchor = Some "hard"
            Content = hardContent }
          { Key = "how"
            AriaLabel = "Designed around how families actually school"
            Classes = []
            Background = None
            Anchor = Some "how"
            Content = realLifeContent }
          { Key = "features"
            AriaLabel = "Everything you need to start"
            Classes = []
            Background = None
            Anchor = Some "features"
            Content = featuresContent }
          { Key = "beta"
            AriaLabel = "Beta testers wanted for December"
            Classes = []
            Background = None
            Anchor = Some "beta"
            Content = betaContent }
          { Key = "pricing"
            AriaLabel = "Simple, transparent pricing"
            Classes = []
            Background = None
            Anchor = Some "pricing"
            Content = pricingContent } ]

    let slideNode (slide: LandingSlide) =
        // Avoid name collision with the custom op `classes`
        let cls     = String.concat " " ("v-slide" :: slide.Classes)
        let idValue = $"slide-{slide.Key}"

        section {
            class' cls
            id idValue
            "role", "listitem"
            "aria-label", slide.AriaLabel

            // Put conditional attributes into an attribute builder
            domAttr {
                match slide.Background with
                | Some bg -> "data-bg", bg
                | None -> ()

                match slide.Anchor with
                | Some anchor -> "data-anchor", anchor
                | None -> ()
            }

            // Children/content go after attributes
            slide.Content
        }



    let view =
        div {
            class' "window"       // middle grid row
            main {
                class' "v-stage"
                id "landing-stage"
                "aria-live", "polite"

                div {
                    class' "v-track"
                    id "landing-track"
                    "role", "list"
                    "aria-label", "Vertical carousel of featured sections"
                    for slide in slides do
                        slideNode slide
                }

                div {
                    class' "v-dots"
                    "aria-label", "Slide navigation"
                    for index, slide in slides |> List.indexed do
                        a {
                            class' (if index=0 then "v-dot active" else "v-dot")
                            href ($"#slide-{slide.Key}")       // snap target
                            domAttr {
                                "role","button"
                                "data-index", string index
                                "aria-label", $"Go to {slide.AriaLabel}"
                                if index = 0 then "aria-current","true"
                            }
                        }
                }
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
            (Some "/")            // activeHref : string option
            view                  // bodyContent : NodeRenderFragment
            (Some "landing-body") // bodyClassOpt : string option
            (Some true)          // includeFooterOpt : bool option
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
                            "Let’s connect! Families, co-ops, teachers, or anyone interested — reach out with your questions or ideas."
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
        StaticAssets.staticDocumentCore
            "About Us — Scholar’s Forge Planner"
            "Learn about the Scholar’s Forge Planner team."
            (Some "/about")
            view
            ""      // no body class
            true    // include footer

module ContactPage =
    let view =
        main {
            section {
                class' "contact-hero"
                id "contact"
                "aria-labelledby", "contact-title"
                // Page-scoped styles for this component
                style {
                    """
                    .contact-hero{
                        position: relative;
                        min-height: 60vh; /* adjust as you like */
                        background-image: var(--contact-hero-overlay), var(--contact-hero-image);
                        background-size: cover;
                        background-position: center;
                        background-repeat: no-repeat;
                    }
                    .contact-hero .contact-hero-content{
                        position: relative;
                        z-index: 1; /* keeps card above the background */
                    }
                    """
                }
                div {
                    class' "container contact-hero-content"
                    div {
                        class' "card contact-card"
                        h1 { id "contact-title"; "Contact Us" }
                        p {
                            class' "muted"
                            "Let’s connect! Families, co-ops, teachers, or anyone interested — reach out with your questions or ideas."
                        }
                        form {
                            class' "contact-form"
                            method "post"
                            action "/contact/send"
                            div {
                                class' "contact-form-main"
                                div {
                                    class' "contact-column"
                                    div {
                                        class' "contact-field"
                                        label {
                                            "for", "first-name"
                                            "First name"
                                        }
                                        input {
                                            id "first-name"
                                            "name", "firstName"
                                            type' "text"
                                            "autocomplete", "given-name"
                                            "required", ""
                                        }
                                    }
                                    div {
                                        class' "contact-field"
                                        label {
                                            "for", "telephone"
                                            "Telephone"
                                        }
                                        input {
                                            id "telephone"
                                            "name", "telephone"
                                            type' "tel"
                                            "autocomplete", "tel"
                                            "required", ""
                                        }
                                    }
                                    div {
                                        class' "contact-field"
                                        label {
                                            "for", "title"
                                            span { "Title" }
                                            span { class' "optional"; "Optional" }
                                        }
                                        input {
                                            id "title"
                                            "name", "title"
                                            type' "text"
                                            "autocomplete", "organization-title"
                                        }
                                    }
                                }
                                div {
                                    class' "contact-column"
                                    div {
                                        class' "contact-field"
                                        label {
                                            "for", "last-name"
                                            "Last name"
                                        }
                                        input {
                                            id "last-name"
                                            "name", "lastName"
                                            type' "text"
                                            "autocomplete", "family-name"
                                            "required", ""
                                        }
                                    }
                                    div {
                                        class' "contact-field"
                                        label {
                                            "for", "email"
                                            "Email"
                                        }
                                        input {
                                            id "email"
                                            "name", "email"
                                            type' "email"
                                            "autocomplete", "email"
                                            "required", ""
                                        }
                                    }
                                    div {
                                        class' "contact-field"
                                        label {
                                            "for", "company"
                                            span { "Company" }
                                            span { class' "optional"; "Optional" }
                                        }
                                        input {
                                            id "company"
                                            "name", "company"
                                            type' "text"
                                            "autocomplete", "organization"
                                        }
                                    }
                                }
                            }
                            div {
                                class' "contact-form-message"
                                div {
                                    class' "contact-field"
                                    label {
                                        "for", "message"
                                        "Message"
                                    }
                                    textarea {
                                        id "message"
                                        "name", "message"
                                        "rows", "6"
                                        "required", ""
                                    }
                                }
                            }
                            //div {
                            //    class' "contact-recaptcha"
                            //    id "contact-recaptcha"
                            //}
                            div {
                                class' "contact-submit"
                                button {
                                    class' "btn primary"
                                    type' "submit"
                                    "Send"
                                }
                            }
                        }
                    }
                }
            }
        }

    type Component() =
        inherit FunBlazorComponent()
        override _.Render() =
            view

    let page ctx =
        StaticAssets.staticDocumentCore
            "Contact Us — Scholar’s Forge Planner"
            "Reach out to the Scholar’s Forge Planner team."
            (Some "/contact")
            view
            ""
            true
