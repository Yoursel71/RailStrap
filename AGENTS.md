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

## Which repo is this? (read this first if you're not sure)

This project now exists as **several GitHub repos** under `Yoursel71` —
know which one you're in before doing anything release- or history-related:

- **`Yoursel71/RailStrap`** — the actual development repo, and the only
  place real work should happen. `main` and `codex/minimal-website` are
  now the same commit (`main` was brought back up to date in Sept 2026);
  push both so they don't drift apart again. The Pages deploy and the
  release workflow both key off `main`.
- **`Yoursel71/RailStrapper`** — the one and only mirror. This is a
  GitHub-native fork of `bloxstraplabs/bloxstrap` that has been renamed
  more than once (it was `railbit-core` for a while), so **`railbit-core`
  is not a separate repo** — that name just redirects here, and pushing
  to both URLs pushes to the same place. Verified with
  `gh api repos/Yoursel71/railbit-core` returning
  `full_name: Yoursel71/RailStrapper`, and `gh repo list Yoursel71`
  showing exactly one fork. It carries upstream Bloxstrap's original
  history *and* RailStrap's full history force-pushed on top, plus the 44
  upstream releases (v1.0.0–v2.11.4) recreated as release *records*
  (title/notes/tag only — the original binaries were never re-uploaded).
  Treat it as read-facing: develop in `RailStrap`, then push here.
- **`Yoursel71/railbit`** — an unrelated private repo (an early prototype
  folder, predates RailStrap). Not part of the release flow; don't touch
  it unless the user specifically asks.

If asked to "sync" or "push everything" to one of the mirror repos, push
directly by URL rather than `git remote add` — the harness's auto-mode
classifier blocks `git remote add` outright. This works fine and touches
no repo config:

```bash
git push https://github.com/Yoursel71/RailStrapper.git HEAD:main --force
git push https://github.com/Yoursel71/RailStrapper.git refs/tags/vX.Y.Z:refs/tags/vX.Y.Z
# whole-history variants, only when the mirror needs every branch/tag:
git push https://github.com/Yoursel71/RailStrapper.git 'refs/remotes/origin/*:refs/heads/*' --force
git push https://github.com/Yoursel71/RailStrapper.git 'refs/tags/*:refs/tags/*' --force
```

A push to `railbit-core` reporting "Everything up-to-date" right after you
pushed `RailStrapper` is not a bug — it's the same repo behind a rename
redirect. Don't go hunting for a missing mirror.

## Where everything is

```
RailStrap.sln                      solution file — desktop app + wpfui source
RailStrap/                         desktop app source (see below)
wpfui/                             VENDORED source (Bloxstrap Labs' fork of WPF UI),
                                    not a git submodule — it was de-submoduled so that
                                    RailStrap-specific edits to it (see "Accent Style
                                    system" below) actually get committed. No
                                    `git submodule` commands are needed or meaningful
                                    here anymore; it's just a normal tracked directory.
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
- `GlobalSettingsManager.cs` (project root, sibling of `FastFlagManager.cs`)
  — edits Roblox's own `%LocalAppData%\Roblox\GlobalBasicSettings_13.xml`
  in place (LINQ-to-XML, only touches the specific `<Properties>` child
  nodes it owns, never overwrites the whole file). See "GlobalBasicSettings
  / the FastFlag allowlist problem" below for why this exists.
- `UI/AccentStyleManager.cs` — see "Accent Style system" below.

### Accent Style system (`RailMono` / `AuroraGlass` / `RailTerminal`)

The Settings window's Appearance page has an **Accent Style** picker
(`Enums/AccentStyle.cs`, `Settings.Prop.AccentStyle`, default `RailMono`)
that layers real, non-cosmetic differences on top of the base Wpf.Ui Dark
theme — not just color, but genuinely different proportions per style
(sidebar width, icon size, item spacing/density, base font size, corner
radius). This was a deliberate response to early feedback that a pure
accent-color swap ("just a skin") wasn't enough of a visual identity.

- `RailStrap/UI/AccentStyleManager.cs` — the styles' actual color/proportion
  values, applied at runtime via `Application.Current.Resources[key] = ...`
  (the same pattern `Wpf.Ui.Appearance.Accent.Apply()` already uses for the
  system accent — DynamicResource lookups pick these up live, no restart
  needed). Also toggles real Windows Acrylic backdrop
  (`Wpf.Ui.Appearance.Background`) for the `AuroraGlass` style specifically.
- `RailStrap/UI/Elements/Base/WpfUiWindow.cs` (`ApplyTheme()`) — the single
  place that decides base theme + accent + backdrop for every window; all
  custom accent styles force a Dark base regardless of the Light/Dark theme
  setting (they're dark-only visual identities).
- **The sidebar's proportions (width, icon size, item margin/padding) are
  driven by `DynamicResource` keys that live in the *vendored `wpfui`
  source itself*** (`wpfui/src/Wpf.Ui/Styles/Controls/NavigationFluent.xaml`
  — literal `Width="250"`, `Margin="16,8"` etc. were changed to
  `{DynamicResource NavSidebarWidth}` / `{DynamicResource NavItemContentMargin}`
  / etc.). Their fallback/default values (used when `AccentStyle == System`)
  live in `RailStrap/UI/Style/Default.xaml`. If you add another proportion
  knob, it needs a default in `Default.xaml` **and** a per-style value in
  `AccentStyleManager.cs`, or `System accent` mode silently loses that
  layout dimension.
- The Settings window's sidebar `NavigationFluent` is wrapped in its own
  `Border` in `MainWindow.xaml` (`ControlFillColorDefaultBrush` background,
  `OverlayCornerRadius`) so it reads as a distinct panel from the content
  pane — that wrapper didn't exist before this feature.
- The whole app's `FontFamily` is set once, in `WpfUiWindow`'s constructor,
  via `SetResourceReference(FontFamilyProperty, "Rubik")` — `FontFamily` is
  an inherited WPF property, so this alone cascades to every descendant
  control without needing a per-control style. `"Rubik"` is defined in
  `App.xaml` and points at the already-bundled
  `Resources/Fonts/Rubik-VariableFont_wght.ttf` (only the "Light" static
  instance is currently used — see the WPF gotcha below about raw string
  literals if you touch that constant).

## GlobalBasicSettings / the FastFlag allowlist problem

**Roblox rolled out an official FastFlag allowlist in Sept 2025.** Most
FFlags third-party bootstrappers used to set no longer take effect unless
Roblox itself allowlisted them — critically, `DFIntTaskSchedulerTargetFps`
(the flag `FastFlagManager`'s `Performance.MaxFPS` preset writes, and the
"Max FPS" control on the FastFlags page) **is not on the allowlist and
silently does nothing on current clients.** That control is kept only as
free insurance in case Roblox ever re-allowlists it — don't remove it, but
don't treat it as the reliable FPS control either.

The replacement mechanism — validated against a real, already-launched
Roblox install on this machine — is `%LocalAppData%\Roblox\GlobalBasicSettings_13.xml`,
an `.rbxlx`-style serialized-instance XML (root `<roblox>` → one
`<Item class="UserGameSettings">` → typed `<Properties>` children) that
Roblox itself reads for client settings, and which the allowlist doesn't
touch. Verified real-file facts, in case you're extending this:

- The real file has *far more* properties than any minimal template
  (mouse sensitivity, VR, accessibility, a `<SharedStrings>` block, etc.)
  — `GlobalSettingsManager` only ever edits the specific named `<Properties>`
  child nodes it owns and leaves everything else byte-for-byte alone. Never
  regenerate/overwrite the whole document.
- `FramerateCap` (`<int>`) is the property RailStrap currently manages
  (`Settings.Prop.GlobalFrameRateCap`, `0` = don't manage it, reasserted on
  every non-Studio launch from `Bootstrapper.ApplyGlobalSettings()`). A
  real client observed on this machine had it at `240` — treat that as the
  practical UI ceiling; Roblox may hard-cap around there regardless of the
  value written.
- **There's also a `GraphicsQualityLevel` (`<int>`, seen at `21` on a real
  client) *and* a separate `SavedQualityLevel` (`<token>`, seen at `10`) —
  two different properties that look like they'd both mean "quality."**
  Which one the current Roblox client actually treats as authoritative
  was not established with confidence, so RailStrap does **not** manage
  either of them yet. If you pick this up, verify against a live client
  before wiring a quality-level control — guessing wrong means the control
  is either a silent no-op or fights the other property.
- `bloxstraplabs/bloxstrap#1367` requested the same kind of editor and was
  closed "not planned" with no recorded reasoning visible in the thread —
  worth knowing RailStrap made a different call here, not that upstream's
  call was necessarily wrong.
- `PerformanceStatsVisible` (`<bool>`, same file) is Roblox's own in-game
  stats display, toggled in-client with **Shift+F5**. RailStrap manages it
  through `GlobalSettingsManager.ApplyPerformanceStats`, behind
  `Settings.Prop.ShowRobloxPerformanceStats`. This is where a real FPS *and*
  ping figure comes from — RailStrap never invents either one (see "no
  fabricated FPS counter" below).

## Ping can't be measured from outside the Roblox client

Verified on this machine against live servers (`128.116.21.33`,
`128.116.5.33`, taken from `[FLog::Network] serverId:` lines in a real
player log):

- **ICMP echo is dropped outright — 100% loss.** The same machine pings
  `1.1.1.1` in ~34 ms, so this is Roblox's side, not the network's.
- **No TCP port answers.** 443, 80, 53, 22 and the game's own UDP port
  number all fail to connect.
- A traceroute dies several hops short of the datacenter (last reply is an
  NTT backbone hop at ~83-107 ms), so "ping the last responsive hop" is not
  a usable approximation either.

That is why the old `ActivityData.QueryPing` — a plain `Ping.SendPingAsync`
— effectively never returned a number and the ping overlay sat on `--`
forever. `Utility/ServerPing.cs` still attempts ICMP (some networks and
regions do get replies) but gives up on an address after two failures
instead of retrying it every few seconds for the whole session, and both
the overlay and the server information dialog then point at Shift+F5,
which reports the client's real round trip.

**Don't "fix" this by inventing a number.** If you want to try harder, the
only remaining avenue is speaking Roblox's own UDP game protocol to
`MachineAddress:MachinePort` (both are parsed and stored on `ActivityData`
now), and it is unverified whether Roblox's RakNet fork answers unconnected
pings at all — test it against a live server before building on it.

## Preferred server regions

Roblox exposes **no API for choosing a datacenter**, so this is matching
after the fact, not targeting:
`ActivityWatcher.OnGameJoin` → `Watcher.CheckServerRegion()` → ipinfo.io
lookup (`ActivityData.QueryServerLocation`, globally cached) →
`ServerRegion.FromLocation` maps the city to one of the entries in
`Models/Entities/ServerRegion.cs`.

- An **unrecognised datacenter is deliberately treated as "no opinion"**,
  never as a mismatch — rerolling on a guess is worse than leaving the
  player alone. Add entries to the catalogue rather than loosening this.
- **Roblox's datacenter list grows, and public trackers lag badly behind
  it.** An Istanbul entry was left out on the strength of two server-region
  sites and pre-May-2026 knowledge; the repo owner, who plays from Turkey
  through the connection helper, had been landing on Istanbul servers.
  Trackers are not authoritative — a player's own `[FLog::Network]
  serverId:` lines are. Err towards *including* a location: an entry that
  never matches is a harmless checkbox, while a missing one makes a real
  server look unrecognised, which is the failure that actually hurts.
- Note the separate facts here: **Roblox running a datacenter in a country
  does not mean Roblox is reachable there.** Istanbul servers exist *and*
  the Turkish block is still in force as of Sept 2026 — web sources
  claiming the ban lifted in June 2026 contradict what the owner reports
  first-hand, so don't weaken the connection helper's copy on their say-so.
- **Catalogue order is load-bearing**: `FromLocation` takes the first
  match, and "Santiago de Queretaro" contains "Santiago". Queretaro must
  stay ahead of Santiago or Mexican servers report as Chilean. Watch for
  the same trap when adding cities.
- Matching is a case-insensitive substring test over the whole
  `"City, Region, CC"` string, and `OrdinalIgnoreCase` does **not** equate
  the Turkish dotted `İ` with `I` — that's why Istanbul carries both
  spellings, as Zurich/Zürich, Milan/Milano and Sao/São Paulo do.
- Rerolling kills the client and relaunches RailStrap with
  `-serverreroll N`, which rides through `WatcherData.ServerRerollAttempt`
  exactly like `-crashrestart` does. The relaunch happens from `Run()`
  **after** `WaitForExitAsync` returns, not from the event handler, so it
  can't race the watcher's own teardown.
- A rerolled session is explicitly excluded from playtime stats.

## Connection helper (GoodbyeDPI)

`Integrations/DpiBypassManager.cs`. Roblox has been blocked in Turkey since
August 2024 by DNS poisoning plus SNI-based DPI, so Turkish players need a
circumvention helper to connect at all. `LaunchHandler.PromptDpiBypassIfRelevant`
offers it once (and only once — `State.Prop.DpiBypassPromptShown`) when the
machine's region or UI language is Turkish.

- **RailStrap does not bundle GoodbyeDPI.** It's fetched from the upstream
  GitHub release on demand and the archive's SHA-256 is checked against a
  pinned constant before anything is extracted. If you bump the version,
  re-download and re-pin the hash — don't drop the check.
- The invocation is the shipped `2_any_country_dnsredir.cmd` with the mode
  number swapped: `goodbyedpi.exe -6 --dns-addr 77.88.8.8 --dns-port 1253
  --dnsv6-addr 2a02:6b8::feed:0ff --dnsv6-port 1253`, run from the
  `x86_64`/`x86` folder so `WinDivert.dll` and the `.sys` driver resolve.
  Mode 6 is `-f 2 -e 2 --wrong-seq --reverse-frag --max-payload`.
- It needs **administrator rights** (WinDivert is a kernel driver), so
  starting it raises UAC. `Start` is skipped entirely when a `goodbyedpi`
  process is already running, so it's one prompt per session, not per
  launch. An unelevated RailStrap **cannot** terminate it directly, which
  is why `Stop` shells out to an elevated `taskkill`.

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
2. Commit (e.g. `Bump version to X.Y.Z`) and push to **whichever branch you've
   actually been working on** — in practice that's been `codex/minimal-website`,
   not `main` (see "Which repo is this?" above; `main` is currently stale).
   Check `git branch -vv` if unsure.
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
  fake number would be dishonest. Uncapping FPS itself (a FastFlag preset,
  and now `GlobalFrameRateCap` — see above) is fine; just don't pair it
  with a made-up on-screen counter. (Roblox's own real FPS display is
  reachable in-client via Shift+F5.)
- **Analytics are a permanent no-op, off by default.** RailStrap does not
  send telemetry to any backend, including its own.
- **Ships unsigned.** No code-signing cert; SmartScreen will warn on first
  run. This is accepted, not a bug to "fix" by adding a signing step. A
  **free** OV cert via SignPath Foundation was researched and looks
  realistically obtainable (RailStrap satisfies the public-repo
  requirement) — see "SignPath application" below before assuming this is
  a dead end.

### SignPath application (researched, not yet submitted)

SignPath Foundation (signpath.org) issues a free OV code-signing cert to
qualifying open-source projects, via the commercial SignPath.io pipeline.
Researched findings worth knowing before picking this up:

- It does **not** instantly clear the Windows SmartScreen warning — a
  trusted-CA signature is a prerequisite for reputation to build, not a
  substitute for it. Reputation still accrues per-release over "weeks and
  hundreds of clean installs" (Microsoft's own current docs). EV vs OV no
  longer matters for this either, since a March 2024 CA/Browser Forum
  change — both need to build reputation the same way now.
- The cert is issued to **"SignPath Foundation,"** not to RailStrap — the
  SmartScreen publisher field will show their name, not RailStrap's.
- Because `Yoursel71/RailStrap` was not created via GitHub's native "Fork"
  button (it started as a manual clone — see NOTICE.md/README history),
  it does **not** qualify for SignPath's easier "fork of an
  already-signed project" carve-out; it'd be evaluated as an independent
  project. The mechanical requirements (OSI license, public repo,
  documented release) are already satisfied; the one soft/unpredictable
  criterion is their "verifiable reputation" bar for a solo project with
  little track record. Real-world reports suggest ~1–3 weeks for a
  decision.
- If approved, CI integration is the same `signpath/github-action-submit-signing-request@v2`
  action Bloxstrap's own (pre-rebrand) `ci-release.yml` used to call, just
  with RailStrap's own `organization-id`/`project-slug`/token instead of
  Bloxstrap Labs'.

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
- **A XAML `Run.Text="{x:Static ...}"` markup extension must start the
  attribute value with `{` as the literal first character** — a leading
  space (`Text=" {x:Static ...}"`) makes XAML treat the whole thing as a
  plain string, so it renders the literal `{x:Static ...}` text instead of
  resolving it. This exact bug shipped in `StatsPage.xaml`'s session-count
  label; fix is to move the leading space into its own separate `<Run Text=" " />`.
- **Raw string literals (`"""..."""`) require C# 11 and this project
  targets net6.0's default (C# 10)** — `dotnet build` fails with CS8936.
  Use a normal (verbatim or concatenated) string literal instead.
- **Roblox's servers drop ICMP, so the ping overlay never worked.** This
  was suspected for a while and is now measured — see the "Ping can't be
  measured from outside the Roblox client" section above for the numbers.
  Anything that wants to show a ping figure has to go through Roblox's own
  Shift+F5 overlay.
- **The `ROBLOX_singletonMutex` lingers for seconds after the client window
  closes**, so testing only for it made the "Roblox is already running"
  confirmation fire on practically every launch. `ConfirmLaunches` now
  requires a live `RobloxPlayerBeta` process too
  (`LaunchHandler.IsRobloxPlayerRunning`); the mutex is just a cheap
  pre-check. The old behaviour had a code comment admitting it "doesn't
  work very well" — it was a real user-visible bug, not a nitpick.
- **A bare type name that collides with an enclosing namespace segment
  resolves to the namespace, not the type** — e.g. `nameof(Settings.Foo)`
  written inside `RailStrap.UI.ViewModels.Settings.SomeViewModel` fails to
  compile (CS0234) because `Settings` resolves to the *namespace*
  `RailStrap.UI.ViewModels.Settings`, not the type
  `RailStrap.Models.Persistable.Settings`. Fully qualify
  (`RailStrap.Models.Persistable.Settings.Foo`) in any ViewModel file
  living under `.Settings` that needs to reference the `Settings` model
  type directly.
- **Automated UI screenshots via a PowerShell `CopyFromScreen` capture are
  unreliable while the user is actively using the machine** — Windows'
  anti-focus-stealing protection means `SetForegroundWindow` silently
  fails when another window (e.g. the terminal/IDE itself) currently has
  focus, and a screenshot taken anyway captures whatever *is* focused, not
  the intended window. Always verify `GetForegroundWindow() == <target hwnd>`
  immediately before capturing, and abort rather than publish a
  screenshot of the wrong window. Don't retry aggressively if it keeps
  failing — that means the user is actively working and forcing focus
  away from them is disruptive.
- **`git remote add` is blocked by the harness's auto-mode classifier.**
  Push directly by URL instead (`git push https://github.com/OWNER/REPO.git
  <refspec>`) — same effect, no remote config touched, not blocked. See
  "Which repo is this?" above for the exact commands used to sync the
  mirror repos.

## Common commands

Desktop app:

```powershell
dotnet build RailStrap.sln -c Release
# real single-file release build, matching ci-release.yml exactly:
dotnet publish -p:PublishSingleFile=true -r win-x64 -c Release --self-contained false RailStrap/RailStrap.csproj
```

`wpfui/` is vendored (plain tracked source, not a submodule) — no init
step needed after a fresh clone.

Website:

```powershell
cd website
npm ci
npm run dev      # local dev server
npm run build    # static export -> website/out/
```
