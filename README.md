# ULTRAKIT
 A library for BepInEx to add custom content to ULTRAKILL, such as weapons and cheats.

![Sentrat mowing down a crowd of filth](https://media.giphy.com/media/bdYGX0uuREyc1DUwpv/giphy.gif)

# Installation
1. Go to the releases page and download ULTRAKIT.zip
2. Extract the contents of ULTRAKIT.zip into [Your ULTRAKILL Directory]\BepInEx\plugins\
3. Install any mods that require ULTRAKIT to function!
### ALTERNATE INSTALLATION
Download ULTRAKIT Reloaded using [Thunderstore Mod Manager](https://thunderstore.io/c/ultrakill/p/ULTRAKIT/ULTRAKIT_Reloaded/) to automatically install the mod and all required dependencies.

## COMPATIBILITY WITH UMM
If UltraModManager is installed, download the [compatiblity patch](https://github.com/ULTRAKIT-Reloaded/ULTRAKIT-Reloaded-UMM-Compatibility-Patch/) and place it in plugins, or install it from [Thunderstore]()

# Editing the config
The config file is located in the installation directory `BepInEx/config/ULTAKIT.core_module.cfg`. Open the file, edit the values and save. The config file generates after running the game with ULTRAKIT Reloaded installed for the first time, and changes take effect after restarting the game.

If using Thunderstore, the config can be edited directly from the `Edit config` tab.

# Making a mod using ULTRAKIT
1. Reference ULTRAKIT .dlls within your project to make use of any extensions and functions available.
2. Register any new cheats and/or buffs on scene load.
3. Register the asset bundle containing new weapons or hats on mod load.

***Further instructions and API are available in the wiki***

# Credits
### ORIGINAL
Loader: Dazegambler

Content Injection: Heckteck

Misc. m1ksu

Original Testing: Cecelune

### RELOADED
Rewrite and Additional Testing: PetersonE1

##

If you have any questions, feel free to DM `Agent of Nyarlathotep#7519` on Discord, or join the ULTRAKIT Reloaded Discord server at https://discord.gg/gW7MAZRm7z

A development roadmap is available in the Projects tab.

***For a reference, a basic mod, asset bundle, and unity asset folder are available at https://github.com/ULTRAKIT-Reloaded/UltrakitTestMod***

***A HatLoader mod to easily drag-and-drop hat bundles into the game is available at https://github.com/ULTRAKIT-Reloaded/HatLoader***
