namespace ScholarsForge.Components

open System
open Fun.Blazor

[<RequireQualifiedAccess>]
module ScrollBackgroundCarousel =

    let private styles =
        """
.scroll-background-carousel {
  position: relative;
  width: 100%;
  overflow-x: hidden;
  scroll-snap-type: y mandatory;
}

.scroll-background-carousel__section {
  position: relative;
  min-height: 100vh;
  width: 100%;
  display: flex;
  align-items: flex-end;
  justify-content: center;
  padding: 2.5rem;
  box-sizing: border-box;
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  transition: opacity 0.9s ease, transform 0.9s ease;
  opacity: 0;
  transform: scale(1.035);
  color: #ffffff;
  scroll-snap-align: start;
}

.scroll-background-carousel__section::before {
  content: "";
  position: absolute;
  inset: 0;
  background: linear-gradient(180deg, rgba(0, 0, 0, 0.45) 0%, rgba(0, 0, 0, 0.65) 100%);
  opacity: 0.55;
  transition: opacity 0.9s ease;
}

.scroll-background-carousel__section.is-visible {
  opacity: 1;
  transform: scale(1);
}

.scroll-background-carousel__section.is-visible::before {
  opacity: 0.32;
}

.scroll-background-carousel__badge {
  position: absolute;
  top: 1.5rem;
  right: 1.5rem;
  z-index: 1;
  font-size: 0.85rem;
  letter-spacing: 0.12em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.76);
  padding: 0.35rem 0.8rem;
  border-radius: 999px;
  border: 1px solid rgba(255, 255, 255, 0.55);
  backdrop-filter: blur(6px);
  background: rgba(0, 0, 0, 0.28);
}

.scroll-background-carousel__hint {
  position: relative;
  z-index: 1;
  margin-bottom: 1rem;
  font-size: 0.95rem;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.7);
  background: rgba(0, 0, 0, 0.35);
  padding: 0.35rem 1.5rem;
  border-radius: 999px;
  border: 1px solid rgba(255, 255, 255, 0.18);
}

@media (max-width: 768px) {
  .scroll-background-carousel__section {
    padding: 1.75rem;
  }

  .scroll-background-carousel__badge {
    top: 1rem;
    right: 1rem;
    font-size: 0.75rem;
  }

  .scroll-background-carousel__hint {
    font-size: 0.8rem;
    padding: 0.25rem 1.1rem;
  }
}
"""

    let private initScript id =
        $"""
(function() {{
    var run = function() {{
        var root = document.getElementById('{id}');
        if (!root) return;
        window.__scrollBgCarousel = window.__scrollBgCarousel || {{}};
        if (window.__scrollBgCarousel['{id}']) return;
        var sections = root.querySelectorAll('.scroll-background-carousel__section');
        if (!sections.length) return;
        var observer = new IntersectionObserver(function(entries) {{
            entries.forEach(function(entry) {{
                entry.target.classList.toggle('is-visible', entry.isIntersecting);
            }});
        }}, {{
            threshold: 0.45
        }});
        sections.forEach(function(section) {{
            observer.observe(section);
        }});
        window.__scrollBgCarousel['{id}'] = observer;
    }};
    if (document.readyState === 'loading') {{
        document.addEventListener('DOMContentLoaded', run, {{ once: true }});
    }} else {{
        requestAnimationFrame(run);
    }}
}})();
"""

    let render (images: string list) : NodeRenderFragment =
        let componentId = $"scroll-background-carousel-{Guid.NewGuid().ToString("N")}" 
        fragment {
            style { rawText styles }
            div {
                id componentId
                class' "scroll-background-carousel"
                "data-scroll-bg-carousel", "true"
                for index, image in images |> List.indexed do
                    section {
                        class'
                            (if index = 0 then
                                 "scroll-background-carousel__section is-visible"
                             else
                                 "scroll-background-carousel__section")
                        style' ($"background-image:url('{image}')")
                        span {
                            class' "scroll-background-carousel__badge"
                            $"{index + 1}/{images.Length}"
                        }
                        if index = 0 then
                            div { class' "scroll-background-carousel__hint"; "Scroll" }
                    }
            }
            if not (List.isEmpty images) then
                script { rawText (initScript componentId) }
        }
