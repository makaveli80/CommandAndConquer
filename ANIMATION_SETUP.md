# 8-Direction Animation System - Setup Guide

This guide explains how to configure the 8-direction animation system for vehicles in Unity Editor.

## Overview

The animation system consists of three main components:
1. **DirectionType** - Enum defining 8 directions (N, NE, E, SE, S, SW, W, NW)
2. **VehicleAnimationData** - ScriptableObject storing sprites for each direction
3. **VehicleAnimator** - MonoBehaviour that updates sprites based on movement direction

## Step 1: Create VehicleAnimationData Asset

### For Buggy:

1. In Unity Project window, navigate to `Assets/_Project/Units/Buggy/Data/`
2. Right-click → Create → Command & Conquer → Animation → Vehicle Animation Data
3. Name it `BuggyAnimationData`
4. Select the asset in the Inspector

### Assign Sprites (8 Directions):

Based on the Buggy sprite sheet (16 sprites: buggy-0000 to buggy-0030):

**Assuming sprites are numbered clockwise starting from East (right-facing):**

- **Sprite E (0°)**: `buggy-0000` (East/Right)
- **Sprite NE (45°)**: `buggy-0004` (Northeast)
- **Sprite N (90°)**: `buggy-0008` (North/Up)
- **Sprite NW (135°)**: `buggy-0012` (Northwest)
- **Sprite W (180°)**: `buggy-0016` (West/Left)
- **Sprite SW (225°)**: `buggy-0020` (Southwest)
- **Sprite S (270°)**: `buggy-0024` (South/Down)
- **Sprite SE (315°)**: `buggy-0028` (Southeast)

**Default Direction**: `S` (South - facing down/towards camera)

### For Artillery:

Repeat the same process:
1. Navigate to `Assets/_Project/Units/Artillery/Data/`
2. Create → Command & Conquer → Animation → Vehicle Animation Data
3. Name it `ArtilleryAnimationData`
4. Assign Artillery sprites following the same direction mapping

## Step 2: Configure Buggy Prefab

1. Open `Assets/_Project/Units/Buggy/Prefabs/Buggy.prefab`
2. In the Inspector, click **Add Component**
3. Search for `VehicleAnimator` and add it
4. Configure VehicleAnimator:
   - **Animation Data**: Drag `BuggyAnimationData` asset
   - **Debug Mode**: Enable for testing (optional)

**Requirements** (should already be present):
- ✅ SpriteRenderer component
- ✅ BuggyMovement component (inherits from VehicleMovement)

## Step 3: Configure Artillery Prefab

1. Open `Assets/_Project/Units/Artillery/Prefabs/Artillery.prefab`
2. Add `VehicleAnimator` component
3. Configure:
   - **Animation Data**: Drag `ArtilleryAnimationData` asset
   - **Debug Mode**: Enable for testing (optional)

## Step 4: Test in Play Mode

1. Open `Assets/_Project/Scenes/Game.unity`
2. Enter Play Mode
3. Click on a Buggy/Artillery to select it
4. Click elsewhere on the grid to move the unit
5. **Expected behavior**:
   - Unit sprite should update to face the movement direction
   - Smooth transitions between 8 cardinal/intercardinal directions
   - Unit maintains last direction when idle

### Debug Mode (Optional):

With Debug Mode enabled:
- Yellow arrow gizmo shows current facing direction (in Scene view)
- Console logs direction changes: `"[VehicleAnimator] Buggy direction changed to NE (45°)"`

## Sprite Mapping Reference

### Understanding Direction Angles

The system uses standard Unity angle conventions:
- **0° = East (Right)** → Positive X axis
- **90° = North (Up)** → Positive Y axis
- **180° = West (Left)** → Negative X axis
- **270° = South (Down)** → Negative Y axis

### Angle Ranges for Each Direction

Each direction covers a 45° arc (360° / 8):

| Direction | Angle Range | Center Angle |
|-----------|-------------|--------------|
| E         | 337.5° - 22.5° | 0° |
| NE        | 22.5° - 67.5° | 45° |
| N         | 67.5° - 112.5° | 90° |
| NW        | 112.5° - 157.5° | 135° |
| W         | 157.5° - 202.5° | 180° |
| SW        | 202.5° - 247.5° | 225° |
| S         | 247.5° - 292.5° | 270° |
| SE        | 292.5° - 337.5° | 315° |

## Troubleshooting

### Issue: Sprites don't change when moving

**Possible causes:**
1. VehicleAnimationData asset not assigned to VehicleAnimator
2. Sprites not assigned in VehicleAnimationData asset
3. VehicleMovement component missing or disabled
4. SpriteRenderer component missing

**Solution:**
- Check Inspector for missing references (pink warnings)
- Verify all 8 sprites are assigned in AnimationData asset
- Enable Debug Mode to see console logs

### Issue: Wrong sprite for direction

**Possible causes:**
1. Incorrect sprite-to-direction mapping
2. Sprite artwork oriented differently than expected

**Solution:**
- Review sprite artwork to identify actual facing direction
- Reassign sprites in VehicleAnimationData to match correct angles
- Use Debug Mode gizmo to compare expected vs actual direction

### Issue: Jittery animation when moving

**Possible causes:**
1. Direction calculation too sensitive to small movements
2. Sprite updates every frame even without direction change

**Solution:**
- The system only updates sprite when direction **changes** (optimization already implemented)
- If still jittery, check VehicleMovement for unusual path calculations

## Architecture Notes

### How It Works

```
VehicleMovement (state machine)
    ↓ CurrentTargetWorldPosition property
VehicleAnimator.Update()
    ↓ Calculate delta vector
DirectionUtils.GetDirectionFromDelta()
    ↓ Angle-based mapping (Atan2)
DirectionType enum
    ↓ Index into data
VehicleAnimationData.GetSpriteForDirection()
    ↓ Sprite reference
SpriteRenderer.sprite = newSprite
```

### Design Patterns Used

1. **Passive Component Pattern** (like SelectableComponent)
   - VehicleAnimator doesn't subscribe to events
   - Polls VehicleMovement state in Update()
   - Decoupled from movement logic

2. **ScriptableObject Configuration**
   - Sprite mappings stored in reusable assets
   - Easy to swap/modify without changing code
   - Multiple units can share same data or have unique data

3. **Static Utility Pattern**
   - DirectionUtils provides stateless conversion functions
   - Reusable across different systems
   - No memory overhead

### Performance Considerations

- **Sprite update optimization**: Only updates SpriteRenderer when direction changes
- **Direction caching**: Remembers last direction to avoid redundant calculations
- **Minimal GC pressure**: No allocations in Update loop
- **Gizmos only in Editor**: Debug visualization stripped from builds

## Next Steps

After setup is complete:
1. ✅ Test Buggy movement in all 8 directions
2. ✅ Test Artillery movement in all 8 directions
3. ✅ Verify sprite orientation matches movement direction
4. ⬜ Create additional vehicle units using the same system
5. ⬜ Add animation frame support (multiple sprites per direction)

## File Reference

### Core System
- `Units/Common/Animation/DirectionType.cs` - 8-direction enum
- `Units/Common/Animation/DirectionUtils.cs` - Direction calculation utility
- `Units/Common/Animation/VehicleAnimationData.cs` - ScriptableObject for sprites
- `Units/Common/Animation/VehicleAnimator.cs` - Animation component

### Data Assets (Created in Unity)
- `Units/Buggy/Data/BuggyAnimationData.asset`
- `Units/Artillery/Data/ArtilleryAnimationData.asset`

### Prefabs (Modified)
- `Units/Buggy/Prefabs/Buggy.prefab` (+ VehicleAnimator component)
- `Units/Artillery/Prefabs/Artillery.prefab` (+ VehicleAnimator component)

---

**Last Updated**: 2025-01-22
**System Version**: 1.0
**Compatible With**: Command & Conquer RTS 2D (Unity 6)