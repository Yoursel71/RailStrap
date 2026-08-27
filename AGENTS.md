# RailStrap — agent notes

This file is the shared brief for any AI coding agent working in this repo
(Claude Code, Codex, Cursor, etc.). Keep it up to date when structure,
versioning, or release flow changes — agents rely on it instead of
rediscovering the repo from scratch every session.

## What this repo is

RailStrap is an independent personal fork of [Bloxstrap](https://github.com/bloxstraplabs/bloxstrap)
(by pizzaboxer, MIT licensed — see [NOTICE.md](NOTICE.md)): a third-party
Roblox bootstrapper for Windows. It wraps the normal Roblox launch process
and adds features around it (FastFlag presets, crash auto-restart, a mod/
theme gallery, playtime stats, Discord Rich Presence, etc.). It does **not**
inject into or tamper with the Roblox client process — that's a deliberate,
locked decision (see "Locked product decisions" below).

Two things live in this one repo and are versioned/released independently:

1. **The desktop app** — `RailStrap/`, a C#/.NET 6 WPF application.
2. **The website** — `website/`, a Next.js 14 + TypeScript + Tailwind static
   site deployed to GitHub Pages at <https://yoursel71.github.io/RailStrap/>.

## Where everything is

```
RailStrap.sln                      solution file — desktop app + wpfui submodule
RailStrap/                         desktop app source (see below)
wpfui/                             git submodule: Bloxstrap Labs' fork of WPF UI
                                    (referenced by RailStrap.csproj; run
                                    `git submodule update --init --recursive`
                                    after a fresh clone)
website/                           the Next.js site (see below)
gallery/                           manifest.json + themes/*.zip served raw via
                                    raw.githubusercontent.com — powers the
                                    in-app Gallery page (Utility/GalleryDownloader.cs)
.github/workflows/
  ci-release.yml                     tag push (vX.Y.Z) -> builds a single-file
                                      publish exe -> creates/publishes a
                                      GitHub Release with asset RailStrap-vX.Y.Z.exe
  ci-debug.yml                       debug build check on push/PR
  deploy-pages.yml                   builds website/ and deploys website/out
                                      to GitHub Pages via GitHub Actions
                                      (Pages source must be set to "GitHub
                                      Actions" in repo settings, not "Deploy
                                      from a branch")
.claude/launch.json                 Claude Code Browser-pane dev-server config
                                     for `website` (see gotcha below)
README.md, NOTICE.md, LICENSE       LICENSE is MIT and unmodified (keep
                                     pizzaboxer's original copyright — NOTICE.md
                                     carries the derivative-work attribution)
```

### Desktop app — `RailStrap/`

- `App.xaml.cs` — identity constants (`ProjectName`, `ProjectRepository`,
  `ProjectDownloadLink`, etc.). Analytics (`SendStat`/`SendLog`) are
  permanent no-ops — RailStrap does not send telemetry anywhere.
- `Bootstrapper.cs`, `LaunchHandler.cs`, `Watcher.cs` — the actual
  launch/relaunch/crash-watch flow.
- `Installer.cs`, `Paths.cs` — install locations, shortcuts, uninstall
  registry keys. Installs to `%LocalAppData%\RailStrap`.
- `UI/Elements/Settings/Pages/*.xaml` + `UI/ViewModels/Settings/*.cs` — the
  Settings window tabs (MVVM). Adding a new settings page = new page +
  matching ViewModel + a `NavigationItem` entry in
  `UI/Elements/Settings/MainWindow.xaml`.
- `Models/Persistable/*.cs` — JSON-persisted state (`Settings.cs`,
  `State.cs`, `PlaytimeSession.cs`, `InstalledGalleryItem.cs`), loaded/saved
  through `JsonManager<T>` / `LazyJsonManager<T>` (`JsonManager.cs`). Files
  live under `%LocalAppData%\RailStrap\*.json`.
- `Resources/Strings.resx` + `Resources/Strings.Designer.cs` — localization.
  **The build does not regenerate Designer.cs from the .resx.** Any string
  key you add or remove must be hand-edited in *both* files or the app
  won't compile / won't find the string. Only edit the base (English)
  `Strings.resx`; the 35 translated `Strings.<lang>.resx` files are a
  known, accepted gap.
- `Resources/RailStrapLogo.png` — the real high-res brand asset. Use this
  (not `RailStrap.ico`) for any on-screen `<Image>` larger than a titlebar
  icon — see the WPF gotcha below.

### Website — `website/`

- Next.js 14 App Router, TypeScript, Tailwind, shadcn-style components
  under `components/ui/`. Statically exported (`output: "export"` in
  `next.config.mjs`) — there is no server at runtime, it's plain HTML/CSS/JS
  on GitHub Pages.
- `lib/site.ts` (`siteAsset()`) — prepends the `/RailStrap` basePath to
  asset URLs in production. Because `images.unoptimized: true` is set,
  `next/image` does **not** auto-prefix `src` with basePath — always wrap
  local asset paths passed to `<Image src=...>` with `siteAsset(...)`, or
  they 404 on the deployed site (they still work in local dev, which is
  why this is easy to miss).
- `lib/version.ts` (`getAppVersion()`) — reads `<Version>` straight out of
  `../RailStrap/RailStrap.csproj` at build time. `components/hero.tsx` uses
  it for the "Version X.Y.Z" badge. Don't hardcode the version anywhere
  else on the site — read it from here so it can't drift again.
- `public/screenshots/` — **real screenshots of the running app**, not
  fabricated mockups. If you need a new one, ask for a saved file path
  (pasted chat images aren't reachable as files — see gotcha below) and
  drop it in here, then reference it the same way `hero.tsx` /
  `playtime-feature.tsx` do.
- `components/ui/container-scroll-animation.tsx` — Aceternity's
  `ContainerScroll` (framer-motion scroll-linked tilt/scale). Its wrapper
  height controls how much scroll distance the reveal eats — keep it
  proportional to viewport height or scrolling feels like it "never ends."

## Where the version number lives (source of truth + release flow)

The **only** source of truth is `<Version>` / `<FileVersion>` in
[`RailStrap/RailStrap.csproj`](RailStrap/RailStrap.csproj). Everything else
either reads from it (website) or is derived from it at release time (git
tag, GitHub Release, download filename). To cut a release:

1. Bump `<Version>` and `<FileVersion>` in `RailStrap/RailStrap.csproj`.
2. Commit (e.g. `Bump version to X.Y.Z`) and push to `main`.
3. `git tag vX.Y.Z && git push origin vX.Y.Z`.
4. `ci-release.yml` triggers on the tag push, builds
   `dotnet publish -p:PublishSingleFile=true -r win-x64 --self-contained false`,
   and publishes a GitHub Release (`RailStrap vX.Y.Z`) with
   `RailStrap-vX.Y.Z.exe` attached — it currently publishes directly, no
   manual draft step needed.
5. `deploy-pages.yml` triggers on the same push (it watches
   `RailStrap/RailStrap.csproj` specifically, plus any `v*` tag, plus
   anything under `website/**`) and redeploys the site, so the hero
   version badge updates automatically. If you ever change these trigger
   paths, make sure a csproj-only version bump still redeploys the site —
   that exact gap is what caused the badge to sit two releases stale
   before it was wired to `lib/version.ts`.

Don't hand-edit a version number anywhere else — if you find one, it's
stale by definition and should be replaced with a read from the csproj
(website) or just removed.

## Locked product decisions

Do not revisit these without the user explicitly re-raising them:

- **No multi-instance launcher.** Rejected outright — it requires patching
  Roblox's own process memory (spoofing `ROBLOX_singletonMutex`), which
  risks Byfron/Hyperion anti-cheat bans. Do not implement, do not suggest.
- **Friend activity panel is opt-in, off by default.** It's the only
  feature that touches a user's Roblox `.ROBLOSECURITY` cookie; the cookie
  is DPAPI-encrypted at rest (`Utility/SecureStorage.cs`) and only ever
  sent directly to Roblox's own API.
- **No fabricated FPS counter.** The overlay shows real ping/network data
  only — Roblox doesn't expose real FPS outside its own process, so a
  fake number would be dishonest. Uncapping FPS itself (a FastFlag preset)
  is fine; just don't pair it with a made-up on-screen counter.
- **Analytics are a permanent no-op, off by default.** RailStrap does not
  send telemetry to any backend, including its own.
- **Ships unsigned.** No code-signing cert; SmartScreen will warn on first
  run. This is accepted, not a bug to "fix" by adding a signing step.

## Gotchas learned the hard way this project

- **`Run.Text` bindings in WPF default to `TwoWay`** even though `Run.Text`
  isn't a real `DependencyProperty` — binding it to a get-only view-model
  property throws `InvalidOperationException` wrapped in
  `TargetInvocationException` the moment the page is constructed (this was
  the exact cause of [issue #11](https://github.com/Yoursel71/RailStrap/issues/11),
  a crash opening Playtime Stats). Always add `Mode=OneWay` explicitly on
  any `<Run Text="{Binding ...}">` against a read-only property.
- **A multi-frame `.ico` loaded via `pack://` into a plain WPF `<Image>`
  silently decodes to the smallest embedded frame**, not the largest —
  this made every large on-screen logo blurry. Use the real PNG
  (`Resources/RailStrapLogo.png`) for anything bigger than a titlebar icon.
- **`.claude/launch.json` is resolved relative to the session's primary
  working directory, not the repo directory** — on this machine the repo
  lives on `F:\...\bloxstrap` but the primary working directory is `C:\`,
  so the Browser pane's `preview_start` looks for
  `C:\.claude\launch.json`, and the config's `runtimeArgs` use an absolute
  `--prefix` path into `website/` since a relative `cwd` isn't supported.
  If dev-server preview stops working after a machine/session change,
  check this first.
- **Pasted chat images are not files.** If you need to use a screenshot
  the user shared inline, ask them for a saved path — there is no
  filesystem access to inline message attachments.
- Resx/Designer.cs must be edited together by hand (see above) — this has
  caused build breaks when only one side was updated.

## Common commands

Desktop app:

```powershell
git submodule update --init --recursive   # first clone only
dotnet build RailStrap.sln -c Release
```

Website:

```powershell
cd website
npm ci
npm run dev      # local dev server
npm run build    # static export -> website/out/
```
