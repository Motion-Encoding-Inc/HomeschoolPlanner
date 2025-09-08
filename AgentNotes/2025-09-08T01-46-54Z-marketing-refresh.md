## Task
Refine the marketing home page per client: more whitespace, real carousel, lighter feel. Update index.html, landing.css/js in marketing and docs.

## Initial considerations
* Need to follow AGENTS instructions (logging etc.)
* Mirror changes into docs for GitHub Pages.
* Ensure JS carousel rotates; CSS uses provided style; hero replaced.
* Run tests: repository may not have tests but run recommended commands.

## Follow-up (2025-09-08 01:49 UTC)
* Searched repo for existing marketing files; none found.
* Will create marketing directories in API/wwwroot and docs with index.html, landing.css, landing.js using provided content.
* Will run dotnet build and test after changes.
## Follow-up (2025-09-08 02:05 UTC)
* Created marketing folder with index.html, landing.css, landing.js including carousel.
* Mirrored the same files under docs/ for GitHub Pages.
* Moved previous docs index to docs/demo/index.html to keep demo links.
## Results
* Updated marketing landing with spacious styling and rotating carousel, mirrored to docs for GitHub Pages.
* Tests built and ran via .NET 9 SDK (installed manually).
* Confidence: 0.86
## Follow-up (2025-09-08 17:14 UTC)
* Plan to introduce warm light/dark theme tokens and toggle with persistence for landing pages in marketing and docs.
* Will update CSS, HTML, JS accordingly and run dotnet build/test after changes.
## Results (2025-09-08 17:17 UTC)
* Introduced light/dark theme tokens with warm palette and toggle button on landing pages under docs and marketing.
* Theme preference stored in localStorage; persisted across sessions.
* Ran `dotnet build` and `dotnet test` successfully.
* Confidence: 0.87
