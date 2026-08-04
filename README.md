# The Legend of Zelda VR

A virtual-reality reimagining of the original *The Legend of Zelda*, designed to preserve the character and nostalgia of the NES game while presenting its world in immersive 3D.

## Gameplay Walkthroughs

[![Watch The Legend of Zelda VR gameplay walkthrough](https://img.youtube.com/vi/hC0rS5lguIw/maxresdefault.jpg)](https://www.youtube.com/watch?v=hC0rS5lguIw&t=15s)

[Watch the featured gameplay walkthrough](https://www.youtube.com/watch?v=hC0rS5lguIw&t=15s), then visit the [Fool's Errand Gaming YouTube channel](https://www.youtube.com/@foolserrandgaming6082/videos) for more videos. The walkthroughs are useful references for the intended look, scale, interactions, and behavior when evaluating a contribution or reporting a regression.

## Try the Quest Preview

Download `ZeldaVR.apk` from the [latest GitHub Release](../../releases/latest). This is a sideloaded preview build rather than a Meta Horizon Store release, so the headset must have Developer Mode enabled.

### Install with SideQuest

1. Follow the [SideQuest setup guide](https://sidequestvr.com/setup-howto) to enable Developer Mode and connect the headset to a computer.
2. Put on the headset and accept the USB debugging prompt. Selecting **Always allow from this computer** makes future updates easier.
3. In SideQuest, choose **Install APK file from folder** and select the downloaded `ZeldaVR.apk`.
4. In the headset, open the App Library and select **Unknown Sources** from its source/filter menu, then launch **The Legend of Zelda VR**.

### Install with ADB

Users who already have Android Platform Tools configured can connect the headset, approve USB debugging, and run:

```text
adb install -r ZeldaVR.apk
```

The `-r` option updates an existing installation while retaining its application data. If installation or launching fails, include the Quest model, Horizon OS version, installation method, and any displayed error when opening an issue.

## Contributions Welcome

Support, fixes, and improvements are welcome—especially:

- Improved character and environment models
- Rigging and animation work
- Bug reports and bug fixes
- Compatibility, performance, and VR-control improvements

Please open an issue or pull request if you find something that could be improved. All changes are reviewed before being merged into `main`.

## Visual Direction

The finished game should remain immediately nostalgic to the original *The Legend of Zelda*. Models should interpret the original sprites as simple, recognizable 3D forms without turning the game into voxel art.

For examples of the current upper-end quality target, see:

- The Fish Statue
- The Bird Statue
- Gleeok
- Aquamentus
- Darknut

New or revised assets should aim for a similar balance: recognizable NES-era silhouettes and colors, clean geometry, and enough detail and animation to feel intentional in VR without losing the original aesthetic.

### Priority Model Updates

The highest-priority candidates for improved models, rigging, or animation are:

1. Fairy
2. Old Man
3. Shopkeep
4. Old Woman
5. Zora
6. Moblins

### Gameplay Wishlist

A particularly welcome future improvement would be replacing the current crossbow-style weapon with a true VR bow: one that the player holds in one hand and physically draws back with the other before releasing the arrow. Any implementation should feel natural across supported controllers and preserve the game's existing item-selection and arrow-inventory rules.

## Unity Migration Note

This project was recently migrated from Unity **2017.3.0f3** to Unity **2021.3.45f1** and updated for modern XR targets. Although many migration regressions have already been corrected, lingering or unexpected effects may remain—particularly in physics, shaders/materials, model importing, animation, nested rigidbodies, input behavior, and scene-specific prefab instances.

If something behaves differently from the original implementation, please report the scene, object or enemy involved, expected behavior, actual behavior, headset/platform, and reliable reproduction steps.
