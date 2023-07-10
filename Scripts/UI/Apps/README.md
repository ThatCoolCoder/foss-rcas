# About UI Apps system

Currently this just consists of some controls that can be stuck on manually through the godot editor, however eventually these will be manageable (add/move/configure/delete) at runtime.

## Storage of available apps

Through a resource that has a list of subresources, which have name, path (or packedscene?), description, thumbnail.

This giga-resource is then exported into AppManager.

## Storage of app profiles

```
(serialized into settings.toml)
class AppList
    profiles: List<AppProfile> # (don't use a dict because people might want to have dots in the name)

class AppProfile
    name: string
    apps: List<AppConfig>

class AppConfig
    path: string
    anchor: vec2
    margin: vec2
    size: vec2
    settings: Dict<string, ?> # only if we add persistent settings
```