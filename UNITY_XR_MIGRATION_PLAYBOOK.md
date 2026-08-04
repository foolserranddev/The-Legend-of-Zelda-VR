# Post-Upgrade Regression Notes for Related Unity VR Games

Give this file to Codex **after the initial Unity/OpenXR upgrade appears complete**. It is not an upgrade recipe. It records the non-obvious regressions that only surfaced during headset playtesting, so they can be checked proactively in another game built from the same codebase.

Update this file only when testing exposes another surprising regression or project-specific invariant worth carrying forward. Do not add routine migration steps that can be rediscovered from the project and current Unity documentation.

## Checks to run after the initial migration

### Controller behavior inherited from the Vive version

- Vive touchpad menu movement must become thumbstick movement, including directional selection and sensible repeat/dead-zone behavior.
- When the secondary-item modifier is held, the **right stick** must operate the secondary-item selection menu rather than ordinary menu movement.
- On Touch-style controllers, **A activates the equipped secondary item** (bomb, candle, arrow, and similar items).
- Verify every item-specific hand pose. Sword, shield, bow, secondary items, and the empty hand used authored hand shapes; routing everything through a new input/controller wrapper can leave the hand in its generic pose.
- Apply a configurable Touch-controller model rotation offset so the legacy hand/item models have the same practical orientation they had on Vive wands.

These are project behavior requirements, not universal OpenXR conventions. Preserve them explicitly when replacing the old controller code.

### Item-selection rules can be lost when input code is replaced

The old controller path contained gameplay rules as well as button handling. In particular, the **bow must not be selectable without arrows**. Check other selectable items for similar inventory/ammunition prerequisites and confirm the new input wrapper still calls the original eligibility logic rather than bypassing it.

### Terrain-based cave and shop floors

The underground floors are Unity `Terrain` objects, not meshes. After upgrading, inspect **every cave, shop, and other below-overworld room**, not only the starting cave. A broken Terrain material/template can appear white, transparent, swirly, or pink. Adding a `MeshRenderer` is not a fix.

Do not confuse these floors with doorway materials: entrances are intentionally black and exits intentionally white.

### Legacy animated water

The existing water geometry/animation may survive while its third-party legacy shader becomes invisible on the new render path or Quest. Check the water in-headset. Preserve the authored water system and replace/adapt only the incompatible shader/material portion unless evidence shows more is broken.

### Shadows on old low-poly and layered objects

Quest real-time shadows produced crawling/blinking black triangles on enemies, hands, the shield, and the thin `Map` overlay later displayed on the shield. Moving the overlay slightly did not address the underlying problem.

For affected legacy meshes, test disabling **Receive Shadows** while retaining shadow casting where useful. Tune outdoor shadow distance/cascades/bias conservatively. Validate outdoors and dungeons separately: the outdoor correction improved appearance greatly but the same lighting/shadow behavior made later dungeons extremely choppy and required a separate dungeon policy.

### Physics-dependent behavior changed after the Unity upgrade

Test behavior, not merely whether the scripts compile:

- Random walkers may see a momentary near-zero velocity and reroll direction every step. Require a sustained stall or restore the existing heading before selecting a new one.
- Flying enemies with an authored landing duration must still descend softly over that duration (about 1.5 seconds in this codebase). New gravity/drag behavior made them drop abruptly; explicit duration-based movement is safer than depending on the old physics constants.

Do not rewrite general enemy logic merely because one encounter fails. First compare with the old behavior and identify which engine-dependent assumption changed.

### Nested enemy scripts can acquire independent physics bodies

This codebase puts scripts derived from a base enemy class on some child parts. The base class requires a `Rigidbody`. Newer Unity can materialize that requirement on the child with default dynamic settings. The child then remains or simulates in world space while the parent enemy moves, so visuals/hitboxes become detached and attacks originate from the wrong place.

Audit objects where both a parent and one or more children carry enemy-derived scripts. For a child that is meant to stay attached to its parent, serialize its Rigidbody explicitly with:

- `Is Kinematic` on
- `Use Gravity` off
- no leftover velocity

This correction was validated on Manhandla's attached claw parts. Keep that specific example because it identifies the exact structural pattern to search for in the related game. Do **not** apply it blindly to parts meant to detach, orbit, or simulate independently; inspect Gleeok and any other multipart enemies according to their intended behavior.

Also search scenes for unpacked copies. Correcting the prefab did not update the existing Manhandla scene instance, whose child gravity setting remained wrong.

### Re-exported or simplified FBX files may silently break prefab mesh references

Replacing an FBX in place is not sufficient if Blender changes its internal mesh name/file ID. Confirm that the prefab renderer points to the new mesh subasset and still has its material, UVs, normals, and intended import scale. This mattered when simplifying Zol and Gel; one replacement became invisible.

Repeated high-poly animated enemies can dominate Quest dungeon performance. Zol was the actual major source in Dungeon 3, so inspect repeated enemy triangle counts before assuming room geometry or scripts are responsible.

### Room blackout must cooperate with `Distance On Off`

If the related game uses the same dungeon blackout optimization, do not create a second independent activation system that races `Distance OnOff`/`Distance On Off`.

Required transition ordering:

- Going dark: turn the room's mob group off first, then the whole room.
- Restoring: turn the whole room on first, then allow the existing distance script to restore mobs.

The distance script must honor the room's forced-off state. Cycling arbitrary children can enable a Rigidbody before its floor/colliders, causing objects such as push blocks to fall or move before the room is ready. A visible black box alone does not improve performance; confirm the expensive room content is actually inactive.

### Push-block and boss regressions: inspect the scene before changing logic

One apparent push-block failure was interaction with the static lookalike because the actual pushable block was absent/not visible at the time. The original push-block logic was sound. Do not change its collision/reset logic without first confirming the correct block instance is present and active.

Boss invulnerability can be a detached-hitbox problem rather than combat logic. Bombs also failed to damage detached multipart boss pieces, which helped rule out only a small sword collider as the complete explanation.

## Still unresolved: bombs sometimes disappear

Do not carry forward any attempted fix as established. The problem can affect the first bomb, and projecting its launch direction onto the horizontal plane did not fix it. The visible model and collider appeared reasonably aligned.

If reproduced in the related game, instrument these events before altering physics or cooldowns:

1. bomb creation and initial transform/velocity;
2. every early destruction path;
3. fuse completion/detonation;
4. explosion creation, position, and active state.

The observation needed is whether the live bomb is destroyed early, detonates somewhere unexpected, or creates an invisible/inactive explosion.

## How to append a new finding

Add only enough information to prevent the same failed debugging loop:

```text
### Feature or structural pattern

Observed after upgrade:
Actual cause:
Validated correction:
Specific example worth searching for:
Misleading diagnosis or failed fix to avoid:
Where it must be retested:
```
