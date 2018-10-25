# KoikatuPlugins

## Description
Various koikatu plugins.  
ConfigurationManager from BepisPlugins is required to change settings for some of the plugins.

## Installation
1. Make sure BepInEx and BepisPlugins are installed.
- Download the latest release from [here](https://github.com/Keelhauled/KoikatuPlugins/releases).
- Drop the dll files you want into the folder `Koikatu\BepInEx`.

## Plugins

#### DefaultParamEditor
Allows editing default settings for character/scene parameters such as eye blinking or shadow density.  
This only affects parameters that make sense to be saved.

To use, set your preferred settings normally and then save them in ConfigurationManager.  
Now when loading a character or starting the studio these settings will be the defaults.

#### FixCompilation
Random fixes and tweaks for the game.

```
- Hide the cameratarget
- Avoid exceptions caused certain hair accessories
- Huge performance gains in chara maker
- Disable character name in maker
- Prevent cursor movement in maker while moving the camera
```

#### GraphicsSettings
Exposes the game's graphics settings and some other values for editing.  
The default settings on the plugin may be too heavy for some computers so remember to tweak them.

#### HideUIKK (*Not yet released*)
Hide all the UI with one click, hotkey is `M` by default.  
More menus can be added by editing `HideUI.txt`.

#### LockOnPluginKK (*Not yet released*)
Pretty much the same as the [HS version](https://keelhauled.github.io/LockOnPlugin/).

#### ObjectTreeDebugKK
Show debug info about GameObjects in the scene.  
Default hotkey is `right ctrl`.  
Original version for HS made by Joan6694.

#### StudioAddonLite (*Not yet released*)
A few HSStudioAddon features ported over to KK.  
Simultaneous FK & IK and better object manipulation controls.

#### TitleShortcuts
Title menu keyboard shortcuts to open different modes.  
For example F to open the female editor.

#### TogglePOVKK (*Not yet released*)
A port of the TogglePOV mod from HS.

#### UnlockHPositions
Unlock all H positions right away without having a save file to unlock them.  
Optionally every possible position can be unlocked regardless of if it was meant to be used in that spot or not.

## Credits
Keelhauled  
MarC0  
Joan6694 for ObjectTreeDebug  
Original maker of TogglePOV
