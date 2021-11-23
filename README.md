# DawnVR [![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/S6S244CYE)
VR mod for Life Is Strange: Before The Storm

## Installation
1. Follow the [Automated Installation guide](https://melonwiki.xyz/#/?id=automated-installation) on the MelonLoader wiki page, installing to the Life is Strange: Before the Storm exe.
   - If you do not know where your install's exe is, open Steam, find the game, right click it, and select Manage -> Browse Local Files, this will lead you to your game's installation.
2. Download the latest release from [here](https://github.com/TrevTV/DawnVR/releases/latest) and open it.
3. In your game's installation directory, drag all the files from the zip into that directory, Windows should merge the folders.
4. In your Steam launch parameters (Right click -> Properties -> Launch Options) add `-vrmode OpenVR` (case-sensitive)
5. Start the game, on first boot after installation the VREnabler plugin will extract the needed files, which may take a minute.

## Controls
(?) In-game controls match and can be changed in game using the default settings menu.

## Configurable Options

### Turning
- `UseSmoothTurning` allows you to toggle smooth turning `(default: true)`
- `SmoothTurnSpeed` allows you to change the speed of smooth turning, if enabled `(default: 120)`
- `UseSnapTurning` allows you to toggle snap turning `(default: false)`
- `SnapTurnAngle` allows you to change the angle that snap turning rotates by `(default: 45)`

### Spectator Camera
- `SpectatorEnabled` enables a separate camera for the monitor which has a higher FOV by default `(default: false)`
- `SpectatorFOV` allows you to change the FOV of the spectator camera `(default: 90)`

### Debugging
- `EnableInternalLogging` allows you to redirect internal logging calls from the game and have them shown in the MelonLoader console `(default: false)`

## FAQ

### Can this be used on the Linux version?
No, Deck Nine's offical release (the Windows version) was heavily obfuscated (as you can see in the code), so to make a version for the Linux build would require you to replace all of the obfuscated references with the clean names.

### I am receiving a Initalization Error in the console on startup!
Make sure you have the `-vrmode OpenVR` paramater setup and that SteamVR is open before launching the game. If neither of those are the issue, go into `Life is Strange - Before the Storm_Data` and delete the file `globalgamemanagers.bak` and restart the game.

### How can I temporarily disable the mod?
In the Steam launch parameters, simply replace OpenVR with None, the mod will recognize that change and will let the game load normally.

## Credits
- [MrPurple](https://github.com/MrPurple6411) & [DrBibop](https://github.com/DrBibop) for the VREnabler code (though I have modified it to just extract a pre-modified globalgamemanagers)
- [DrBibop](https://github.com/DrBibop) for the RoR2 mod and for helping me with some VR rig handling
- [Sinai](https://github.com/sinai-dev/) for creating UnityExplorer (and quickly fixing a bug I had) which definitely helped speed up the development process
- [Valve](https://github.com/ValveSoftware/) for the SteamVR Unity Plugin SDK [licensed under BSD-3](https://github.com/TrevTV/DawnVR/blob/main/LICENSE_STEAMVR)
- `elliotttate#9942` for some assistance in the Flatscreen to VR Modding Discord
- `alecpizz#0311` for helping me with some rig stuff, giving me some code, and testing parts of the mod
- `Parzival#7273` for the idea of a monitor-like border during cutscenes
- Probably a few other people I've forgotten