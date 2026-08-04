# The Legend of Zelda VR

A virtual-reality reimagining of the original *The Legend of Zelda*, designed to preserve the character and nostalgia of the NES game while presenting its world in immersive 3D.

## Gameplay Walkthroughs

See the game in action through the development walkthroughs on the [Fool's Errand Gaming YouTube channel](https://www.youtube.com/@foolserrandgaming6082/videos). These videos are also useful references for the intended look, scale, interactions, and behavior when evaluating a contribution or reporting a regression.

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

## Unity Migration Note

This project was recently migrated from Unity **2017.3.0f3** to Unity **2021.3.45f1** and updated for modern XR targets. Although many migration regressions have already been corrected, lingering or unexpected effects may remain—particularly in physics, shaders/materials, model importing, animation, nested rigidbodies, input behavior, and scene-specific prefab instances.

If something behaves differently from the original implementation, please report the scene, object or enemy involved, expected behavior, actual behavior, headset/platform, and reliable reproduction steps.
