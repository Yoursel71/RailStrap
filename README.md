> [!CAUTION]
> The only official place to download RailStrap is this GitHub repository. Any other websites offering downloads or claiming to be us are not owned by us.

> [!NOTE]
> RailStrap is a personal fork of [Bloxstrap](https://github.com/bloxstraplabs/bloxstrap) by pizzaboxer, under the same MIT license. See [LICENSE](LICENSE) and [NOTICE.md](NOTICE.md).

<p align="center">
    <img src="Images/RailStrap.png" width="380">
</p>

<div align="center">

[![License][shield-repo-license]][repo-license]
[![GitHub Workflow Status][shield-repo-workflow]][repo-actions]
[![Downloads][shield-repo-releases]][repo-releases]
[![Version][shield-repo-latest]][repo-latest]

</div>

----

RailStrap is a third-party replacement for the standard Roblox bootstrapper, providing additional useful features and improvements.

Running into a problem or need help with something? [Submit an issue](https://github.com/Yoursel71/RailStrap/issues).

RailStrap is only supported for PCs running Windows.

## Frequently Asked Questions

**Q: Is this malware?**

**A:** No. The source code here is viewable to all. Just be sure you're downloading it from an official source - the only official source is this GitHub repository.

**Q: Can using this get me banned?**

**A:** No, it shouldn't. RailStrap doesn't interact with the Roblox client in the same way that exploits do.

## Features

- Hassle-free Discord Rich Presence to let your friends know what you're playing at a glance
- Simple support for modding of content files for customizability (death sound, mouse cursor, etc)
- See where your server is geographically located (courtesy of [ipinfo.io](https://ipinfo.io))
- Ability to configure graphics fidelity and UI experience

## Installing

Download the [latest release of RailStrap](https://github.com/Yoursel71/RailStrap/releases/latest), and run it. Configure your preferences if needed, and install. That's about it!

You will also need the [.NET 6 Desktop Runtime](https://aka.ms/dotnet-core-applaunch?missing_runtime=true&arch=x64&rid=win11-x64&apphost_version=6.0.36&gui=true). If you don't already have it installed, you'll be prompted to install it anyway. Be sure to install RailStrap after you've installed this.

RailStrap ships unsigned for now, so Windows Smartscreen will likely show a popup when you run it for the first time. To dismiss it, just click on "More info" and then "Run anyway".

Once installed, RailStrap is added to your Start Menu, where you can access the menu and reconfigure your preferences if needed.

## Code

RailStrap uses the [WPF UI](https://github.com/lepoco/wpfui) library for the user interface design, via [Bloxstrap Labs' fork](https://github.com/bloxstraplabs/wpfui) of it.

[shield-repo-license]:  https://img.shields.io/github/license/Yoursel71/RailStrap
[shield-repo-workflow]: https://img.shields.io/github/actions/workflow/status/Yoursel71/RailStrap/ci-release.yml?branch=main&label=builds
[shield-repo-releases]: https://img.shields.io/github/downloads/Yoursel71/RailStrap/latest/total?color=981bfe
[shield-repo-latest]:   https://img.shields.io/github/v/release/Yoursel71/RailStrap?color=7a39fb

[repo-license]:  https://github.com/Yoursel71/RailStrap/blob/main/LICENSE
[repo-actions]:  https://github.com/Yoursel71/RailStrap/actions
[repo-releases]: https://github.com/Yoursel71/RailStrap/releases
[repo-latest]:   https://github.com/Yoursel71/RailStrap/releases/latest
