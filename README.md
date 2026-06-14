# Transit Night Scheduler
A **Cities: Skylines II** mod that lets you configure the number of vehicles per transit line during night hours. Made with Claude because I'm bad af at developing things.

## Features

- **Night vehicles slider** directly in the transit line panel (only visible for Day+Night lines)
- Vehicles automatically **return to depot** at night and **come back** during the day
- Configurable **night start and end hours** in mod settings
- Settings are **saved per city** (i hope this works lol) and persist between sessions
- **Enable/disable** the mod globally from settings

## How to Use
1. Select a transit line configured as **Day + Night**
2. Use the **Night Vehicles** slider that appears below the Vehicles slider
3. Set the desired number of vehicles for night hours
4. Configure night hours in **Options > Mods > Transit Night Scheduler**

## Settings
| Setting | Default | Description |
|---|---|---|
| Enable night mode | On | Enable or disable the mod globally |
| Night start (hour) | 21 | Hour when night mode starts |
| Night end (hour) | 6 | Hour when night mode ends |

For a night spanning midnight (e.g. 21h to 6h), set the start hour higher than the end hour.
For a night within the same day (e.g. 0h to 3h), set the start hour lower than the end hour.

## Notes
- Lines are identified by their **transport type and route number** (e.g. `Tram_1`). If you change the route number, the night configuration for that line will be lost.
- The slider only appears on lines set to **Day + Night** schedule.

## Installation
Install via [Paradox Mods](https://mods.paradoxplaza.com).

## Building from Source
### Requirements
- .NET SDK 8
- Unity 2022.3.62f2
- Node.js 20
- JetBrains Rider or Visual Studio 2022

### Linux specifics
I struggled a lot working on this even if Claude made everything, I made the mod on linux (Arch) with CS2 running under Proton. The toolchain requires some manual setup (the instructions bellow might not be true, but thats what i remember):

1. Install the mod template from the game files:
```bash
dotnet new install "$CSII_INSTALLATIONPATH/Cities2_Data/Content/Game/.ModdingToolchain/ColossalOrder.ModTemplate.1.0.0.nupkg"
```

2. Open the Unity mod project (`UnityModsProject.zip`) in Unity 2022.3.62f2 to generate the `Library` folder.

3. Set environment variables pointing to your CS2 installation and user data paths.

4. The `ModPostProcessor` and `ModPublisher` are patched in `Mod.targets` to run via `dotnet` instead of Wine.

### Build
Open in Rider and build — this compiles both C# and TypeScript in one step. You will had to adapt the Mod.targets file if you need to run it on windows

## Contributing
PRs welcome!

## License
I don't know what I should add there ?
