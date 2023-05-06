# Prerequisites and environment setup for FOSS-RCAS addon creation

1. Install Godot Mono version 3.x (todo: check if 3.6 is needed or if 3.5 is fine too) from [https://godotengine.org/download/3.x/](https://godotengine.org/download/3.x/)
    - Arch Linux users can also install it from the AUR as `godot-mono-bin`.
2. Install the [.NET SDK](https://dotnet.microsoft.com/download) or the [Mono SDK](https://www.mono-project.com/download/stable/)
3. Download the source code of FOSS-RCAS [from github](https://github.com/ThatCoolCoder/foss-rcas/archive/refs/heads/main.zip), and unzip it into a directory.
    - If you are a developer of FOSS-RCAS and already have it downloaded through git, it is still recommended to download 
    - Todo: when there is a release scheme, tell people to instead download the code of the latest release.
4. Create a directory within `AddonContent/`. Name it something that is unique to you - for instance a Github username. This prevents clashes between addons with the same name.
    - Use only letters, numbers, `-` and `_` within the name of the directory to ensure that it works properly on all operating systems
5. For packaging your addons, install [godot-pck-tool](https://github.com/hhyyrylainen/GodotPckTool)