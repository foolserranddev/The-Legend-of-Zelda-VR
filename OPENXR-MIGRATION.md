# Zelda VR OpenXR migration

The project now targets Unity 2021.3 LTS and Unity OpenXR. The old SteamVR and
VRTK source trees remain as archival assets, but their assembly definitions keep
them out of compilation unless the legacy symbols are deliberately enabled.

## Controller mapping

Gameplay uses `PortableXRInput`, backed by Unity's cross-runtime XR input API:

| Zelda action | OpenXR usage |
| --- | --- |
| Use equipped item / movement strength | Trigger |
| Confirm menu / save gesture | Grip |
| Menu direction | Primary stick deflection (with repeat) |
| Legacy pad click / gameplay item controls | Primary stick click |
| Confirm | Grip or A/X |
| Pause/save menu | Menu button or B/Y |
| Controller motion | Device pose and velocity |

`HandController` retains its original Unity script GUID, so existing scene and
prefab references survive the migration. It automatically selects left or right
input from the object's `LeftHand` or `RightHand` tag.

## Editor setup

1. Install Unity 2021.3.45f1 with Windows Build Support and Android Build Support.
   This checkout can also use the ignored `.unity-toolchain` folder populated on
   this workstation with Unity's supported SDK, NDK, and OpenJDK versions.
2. Open the project and allow Package Manager/import upgrades to complete.
3. Run **Zelda VR > Configure OpenXR** once.
4. Resolve any scene-specific missing-script warnings left by the disabled legacy
   SteamVR/VRTK components. The portable head and hand scripts supply tracking.

## Builds

- **Zelda VR > Build > Quest APK** produces `Builds/Quest/ZeldaVR.apk` using
  Android ARM64, IL2CPP, OpenGLES 3, and OpenXR.
- **Zelda VR > Build > Windows OpenXR** produces `Builds/Windows/ZeldaVR.exe`.
  The active PC OpenXR runtime determines whether SteamVR, Vive Console, Meta,
  or another conformant runtime hosts the application.

Hardware testing is still required for controller offsets, performance, haptics,
and platform permissions.

## Scale and tracking-origin notes

The pre-existing per-model scale overrides were intentionally retained. In
particular, locally corrected values on the tree, Zora, Moblin, bomb, and several
dungeon models must not be reset to importer defaults. Unity treats one world
unit as one meter, while FBX/DAE source units may differ.

The render camera remains below the legacy `Camera (head)` transform. OpenXR
drives that head transform in local tracking-origin space, as it does the two
controller siblings. The player capsule now converts the head's world position
to player-local space before calculating its center and height, avoiding the old
world/local coordinate mixture when the playspace moves, rotates, or scales.
