# Content metadata and thumbnail files for FOSS-RCAS

Each item of content (so every plane and location) needs a metadata file in order to be found and loaded by the simulator. It's also recommended to create a thumbnail file for display in the content selector. The metadata file is a TOML file, although it must have the extension `.content.toml` in order to be picked up by the game. The `.tscn` file of an aircraft/location and it's thumbnail need to have **exactly the same path as the metadata file, except for the extension**.

Thumbnails need to be in png format and should have a resolution of 1280x720.

Example folder structure:
```
AddonContent/
|-- YourUsername/
    |-- YourAmazingPlane/
        |-- YourAmazingPlane.png
        |-- YourAmazingPlane.content.toml
        |-- YourAmazingPlane.tscn
```

The contents of metadata files are described in [Aircraft techniques](AircraftTechniques.md) and [Creating locations](CreatingLocations.md), but if you're following the beginner pathway you don't need to read about that yet.