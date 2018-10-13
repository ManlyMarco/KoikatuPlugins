# KoikatuPlugins

## Description
Various koikatu plugins.  
ConfigurationManager from BepisPlugins can be used to change settings for some of the plugins.

## Installation
1. Install BepInEx and BepisPlugins.
2. Unpack the plugin zip file into the koikatu root folder.

## Plugins

#### FixCompilation - [Download](https://github.com/Keelhauled/KoikatuPlugins/releases/download/second/FixCompilation.v1.0.1.zip)
Random fixes and tweaks for the game.  
A restart is required for most things in the options menu to take effect.

<details><summary>Feature list</summary>

```
- Hide the cameratarget
- Fix exceptions in certain hair accessories
- Huge performance gains in chara maker
- Disable character name in maker
```
</details>

<details><summary>Changelog</summary>

```
v1.0.1
- Better descriptions in ConfigurationManager
```
</details>

#### HideUIKK - [~~Download~~](https://github.com/Keelhauled/KoikatuPlugins/releases/download/first/HideUIKK.v1.0.0.zip)
Hide all the UI with one click, hotkey is `M` by default.  
More menus can be added by editing `HideUI.txt`.

#### LockOnPluginKK - [~~Download~~](https://github.com/Keelhauled/KoikatuPlugins/releases/download/first/LockOnPluginKK.v1.0.0.zip)
Pretty much the same as the [HS version](https://keelhauled.github.io/LockOnPlugin/).

#### ObjectTreeDebugKK - [Download](https://github.com/Keelhauled/KoikatuPlugins/releases/download/second/ObjectTreeDebugKK.v1.0.1.zip)
Show debug info about GameObjects in the scene.  
Default hotkey is `right ctrl`.  
Original version for HS made by Joan6694.

<details><summary>Changelog</summary>

```
v1.0.1
- Camera will no longer move when using the UI
```
</details>

#### StudioAddonLite - [~~Download~~](https://github.com/Keelhauled/KoikatuPlugins/releases/download/first/StudioAddonLite.v1.0.0.zip)
A few HSStudioAddon features ported over to KK.  
Simultaneous FK & IK and better object manipulation controls.

#### TogglePOVKK - [~~Download~~](https://github.com/Keelhauled/KoikatuPlugins/releases/download/first/TogglePOVKK.v1.0.0.zip)
A port of the TogglePOV mod from HS.

#### UnlockHPositions - [Download](https://github.com/Keelhauled/KoikatuPlugins/releases/download/fourth/UnlockHPositions.v1.1.0.zip)
Unlock all H positions right away without having a save file to unlock them.  
Optionally every possible position can be unlocked regardless of if it was meant to be used in that spot or not.

<details><summary>Changelog</summary>

```
v1.1.0
- Option to unlock all possible positions
```
</details>

#### TitleShortcuts - [Download](https://github.com/Keelhauled/KoikatuPlugins/releases/download/third/TitleShortcuts.v1.1.1.zip)
Title menu keyboard shortcuts to open different modes.  
For example F to open the female editor.

<details><summary>Changelog</summary>

```
v1.1.1
- Removed method affecting the bepinex gameobject
```
```
v1.1.0
- Autostart options in ConfigurationManager
- Ability to cancel automatic start by holding esc
```
</details>

#### DefaultParamEditor - [Download](https://github.com/Keelhauled/KoikatuPlugins/releases/download/fourth/DefaultParamEditor.v1.1.0.zip)
Allows editing default settings for character/scene parameters such as eye blinking or shadow density.  
This only affects parameters that make sense to be saved.

To use, set your preferred settings normally and then save them in ConfigurationManager.  
Now when loading a character or starting the studio these settings will be the defaults.

<details><summary>Changelog</summary>

```
v1.1.0
- Button to reset options
- Better descriptions
- Bugfixes
```
</details>

## Credits
Keelhauled  
MarC0  
Joan6694 for ObjectTreeDebug  
Original maker of TogglePOV
