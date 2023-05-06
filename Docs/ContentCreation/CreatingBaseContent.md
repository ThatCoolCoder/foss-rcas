# Creating base content for FOSS-RCAS

In addition to supporting addon content, FOSS-RCAS has a small library of base content that is distributed with the simulator. This document details how this content is created. Note that little new content will be accepted, in order to minimise the download size and general bloatedness of the base game. Therefore, this is mostly a guide for developers of the project. 

## Creation

The only difference between base and addon content is the file location. 

Aircraft scenes, metadata files, mixes files, thumbnails, etc should be located in a subdirectory of `Scenes/Aircraft` - EG `Scenes/Aircraft/T28`. Aircraft 3d models, textures, etc should be located in a subdirectory of `Art/Aircraft`. If a propeller sound effect or model has been created for that aircraft, store it in `Art/Aircaft/PropAudio` or `Art/Aircaft/Props` respectively, so that other aircraft can use those assets in the future. 

Locations are the same except that `Aircraft` in the paths is replaced with `Locations`.

## Integration

Those with write access to the repository can simply commit their changes directly. Others should submit their changes via a pull request.