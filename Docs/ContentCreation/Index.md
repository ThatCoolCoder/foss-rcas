# Content creation for FOSS-RCAS

This directory contains a series of documents detailing how to create content (aircraft and flying locations) for FOSS-RCAS.

## Contents:

- [Prerequisites](CreatingLocations.md)
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

1. Follow the instructions in [Prerequisites](CreatingLocations.md) to setup an environment for creating addons.
2. Read through the information regarding [content metadata files](ContentMetadata.md)
3. Create a simple location following the instructions in [Creating locations](CreatingLocations.md). Locations are simpler than aircraft, so even if your end goal is to make aircraft it's recommended to do this first.
4. Try packaging it.
5. Create a simple aircraft through [Creating your first aircraft](CreatingFirstAircraft.md)
6. Model a plane in blender and do more advanced stuff

todo: write more here

If you have any difficulties, create an issue at `https://github.com/ThatCoolCoder/foss-rcas/issues`.