# Packaging addons for FOSS-RCAS

Now that you have completed creating your addon, it is time to distribute it. The process is a little tedious due to some odd quirks of Godot, but it is not that difficult.

1. Export as a zip the resources in your directory using Godot editor. Do it twice because the first time it will be broken.
2. Unzip the zip somewhere
3. Delete everything that's not relevant from inside the unzipped folder. This includes:
    - Common models, scenes and art
    - All the scripts
    - The .mono directory
    - The stuff in the .import directory that does not belong to your addon.
4. Configure your shell to make * match files starting with a dot
5. Repack it with `godotpcktool clean.pck -a a * --set-godot-version 3.5.0` **from within the dir that you unzipped it to**

todo: add images and more information to this document