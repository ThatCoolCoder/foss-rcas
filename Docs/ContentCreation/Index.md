# Content creation for FOSS-RCAS

This directory contains a series of documents detailing how to create content (aircraft and flying locations) for FOSS-RCAS.

## Contents:

- [Prerequisites](Prerequisites.md)
- [Content metadata files](ContentMetadata.md)
- [Creating locations](CreatingLocations.md)
- [Creating your first aircraft](CreatingFirstAircraft.md)
- [More advanced aircraft techniques](AircraftTechniques.md)
- [Reference of all components that can be used on aircraft](AircraftComponentReference.md)
- [Packaging content for distribution](AddonPackaging.md)
- [Creating base content](CreatingBaseContent.md)

todo: write somewhere about how it's safe to use stuff in the common directory but stuff in the specific aircraft or location directories is volatile and should not be relied upon.

## Suggested pathway for beginners

If you are not a programmer, or are simply not familiar with this project, it may seem daunting at first to create addons. These steps provide a suggested order in which to learn about the various aspects of addon creation.

1. Follow the instructions in [Prerequisites](Prerequisites.md) to setup an environment for creating addons.
1. If you haven't used the Godot engine previously, read through the [introduction to Godot](GodotIntroduction.md).
1. Read through the information regarding [content metadata files](ContentMetadata.md).

1. Create a simple aircraft through [Creating your first aircraft](CreatingFirstAircraft.md). Although aircraft are more complex than locations, they are less tedious and require less existing skill, so it's recommended to start with them.
1. Read through [Aircraft techniques](AircraftTechniques.md) and try modelling a plane then implementing it using some of the technquies described in that document.
1. Try [packaging your aircraft](AddonPackaging.md) and sharing it online.

1. If you want to create locations, follow the instructions in [Creating locations](CreatingLocations.md).

If you have any difficulties, create an issue at `https://github.com/ThatCoolCoder/foss-rcas/issues`.