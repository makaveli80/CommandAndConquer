# CLAUDE.md

Documentation technique pour Claude Code (claude.ai/code).

## Project Overview

**Command and Conquer - RTS 2D** - Educational RTS project recreating Command and Conquer in 2D with Unity 6.

- **Engine**: Unity 6
- **Pipeline**: Universal Render Pipeline (URP) 2D
- **Language**: C# with .NET Standard 2.1
- **Input**: New Input System (**NOT** legacy Input class)

## Current State

**Branch**: `master`
**Phase**: Core RTS systems complete ✅ | Ready for expansion

### Completed Features
- ✅ Grid system (20×20, 1.0 unit cells, pathfinding 8 directions)
- ✅ Camera controller (WASD, edge scroll, zoom)
- ✅ 2 units: Buggy (speed 4.0), Artillery (speed 1.5)
- ✅ Selection system (single click + drag-box multi-selection)
- ✅ Corner bracket selection visual (white L-brackets)
- ✅ Animated cursor system (hover 6 frames, move 4 frames)
- ✅ 8-direction animation system (DirectionType, VehicleAnimator)
- ✅ State machine movement (Idle, Moving, WaitingForNextCell, Blocked)
- ✅ Atomic cell reservation (prevents race conditions)
- ✅ Collision detection with retry mechanism

### Architecture
- ✅ **Component-Based** (Nov 2025) - 100% composition, zero inheritance
  - Eliminated Controllers, Contexts, UnitBase abstract class
  - New units created 100% in Unity Editor (zero code)
  - ~600 lines of code eliminated
- ✅ Generic components: Unit, VehicleMovement, SelectableComponent, VehicleAnimator

### Next Steps
- **Option A**: Add 3rd unit (Tank/Harvester/MCV)
- **Option B**: Formation system (multi-unit formations)
- **Option C**: Combat system (attack, health, damage, death)
- **Option D**: Building system (construction, production)
- **Option E**: AI opponents (pathfinding, behaviors)

## Architecture

### Module Structure
```
CommandAndConquer/
├── Core/        # Interfaces (IMovable, ISelectable), types (UnitData, GridPosition)
├── Grid/        # Grid system (depends on Core)
├── Camera/      # RTS camera (minimal dependencies)
├── Gameplay/    # Selection, cursor (depends on Core, Grid)
├── Units/       # Unit implementations
│   ├── Common/  # Generic components (Unit, VehicleMovement, etc.)
│   ├── Buggy/   # Buggy-specific assets
│   └── Artillery/ # Artillery-specific assets
└── Map/         # Terrain, tilemap
```

### Dependency Graph
```
Core → Grid, Camera, Gameplay, Units, Map
Grid → (no deps)
Gameplay → Core, Grid
Units/Common → Core, Grid
```

**Principle**: Pure composition. Units = assembly of components in Unity Editor.

---

## Key Systems

### 1. Grid System (`Grid/`)

**Configuration**:
- 20×20 cells, 1.0 unit each
- Conversion: Grid (5,5) → World (5.5, 5.5) - **always +0.5f**
- Pathfinding: `GridPathfinder.CalculateStraightPath()` (8 directions)

**Key Methods**:
```csharp
// Lifecycle
bool RegisterUnit(Unit unit, GridPosition pos)         // Call in Start()
void UnregisterUnit(Unit unit)                         // Call in OnDestroy()

// Movement (ATOMIC - prevents race conditions)
bool TryMoveUnitTo(Unit unit, GridPosition newPos)     // Reserves cell atomically

// Query
bool IsCellAvailableFor(GridPosition pos, Unit unit)   // Check availability
```

**⚠️ Important**: Always use `TryMoveUnitTo()` for movement. `IsCellAvailableFor()` does NOT reserve!

**Coordinate Conversion**:
```csharp
// Grid → World (+0.5f)
Vector3 worldPos = new Vector3(gridPos.x + 0.5f, gridPos.y + 0.5f, 0);

// World → Grid (FloorToInt)
GridPosition gridPos = new GridPosition(
    Mathf.FloorToInt(worldPos.x),
    Mathf.FloorToInt(worldPos.y)
);
```

### 2. Unit System (`Units/`)

**Pattern**: 100% Composition
```
GameObject "Buggy"
├── Unit (generic)
├── VehicleMovement (generic)
├── SelectableComponent (generic)
├── VehicleAnimator (generic)
└── SpriteRenderer
```

**Creating New Unit** (zero code):
1. Create → Command & Conquer → Unit Data
2. Create GameObject
3. Add components: Unit, VehicleMovement, SelectableComponent, VehicleAnimator, SpriteRenderer, BoxCollider2D
4. Create Prefab → Done!

See [GUIDE.md](GUIDE.md) for detailed workflow.

### 3. Movement System

**State Machine**:
```csharp
enum MovementState { Idle, Moving, WaitingForNextCell, Blocked }
```

**Flow**:
1. `MoveTo(GridPosition)` on Unit
2. Path calculation via `GridPathfinder.CalculateStraightPath()`
3. **Atomic reservation** with `TryMoveUnitTo()`
4. Interpolation with `Vector3.MoveTowards()`
5. If cell occupied → `WaitingForNextCell` (retry 0.3s × 20 = 6s timeout)

**Debug Colors** (BuggyMovementDebug): White=Idle, Green=Moving, Orange=Waiting, Red=Blocked

### 4. Selection System

**Architecture**:
```
Unit (events) → SelectableComponent (coordinator) → CornerBracketSelector (display)
```

**Multi-Selection** (`SelectionManager`):
- `HashSet<ISelectable>` for O(1) lookups
- Drag box: 5px threshold, `Physics2D.OverlapAreaAll()`
- Single-click or drag-box

**Visual Types**:
- `SpriteColor` (legacy) - Color change
- `CornerBrackets` (default) - White L-brackets in 4 corners

**Corner Rotations**: TL=0°, TR=-90°, BR=180°, BL=90°

### 5. Camera System

RTS camera with WASD, edge scrolling, zoom (mouse wheel).

**Files**: `Camera/Scripts/CameraController.cs`, `CameraBounds.cs` (ScriptableObject)

### 6. Cursor System

**Types**:
- Default - System cursor
- Hover - 6 frames @ 10 FPS (units)
- Move - 4 frames @ 10 FPS (destinations)

**Priority**: Hover > Move > Default

**Setup**: Sprites must have `isReadable = true` (auto-configured by CursorSpriteImporter)

**Key Methods**:
```csharp
CursorManager.SetCursor(CursorType type)
CursorManager.ResetCursor()
```

### 7. Animation System

**8 Directions**: E, NE, N, NW, W, SW, S, SE (45° each)

**Components**:
- `DirectionType` - Enum
- `VehicleAnimationData` - ScriptableObject (8 sprites)
- `VehicleAnimator` - Passive polling component

**Pattern**: Polls `VehicleMovement` in Update(), updates sprite only on direction change.

**Setup**: See [docs/ANIMATION.md](docs/ANIMATION.md)

---

## Unity Guidelines

### Input System

**CRITICAL**: Use **New Input System** only!

```csharp
// ❌ WRONG
if (Input.GetKeyDown(KeyCode.Space)) { }

// ✅ CORRECT
using UnityEngine.InputSystem;
if (Keyboard.current.spaceKey.wasPressedThisFrame) { }
```

### Component Discovery

Use `FindFirstObjectByType<T>()` (Unity 6 API):
```csharp
gridManager = FindFirstObjectByType<GridManager>();
```

### Sprites
- **PPU**: 128
- **Filter Mode**: Point
- **Compression**: None
- 128px = 1.0 Unity unit = 1 grid cell

---

## Workflows

### Testing in Unity

1. Open `Assets/_Project/Scenes/Game.unity`
2. Press Play ▶️

### Adding New Unit

**Zero code workflow** (5 minutes):
1. Create UnitData asset
2. Create GameObject + Add components
3. Create Prefab

See [GUIDE.md](GUIDE.md) for step-by-step instructions.

### Commit Format

`<type>: <message>`

**Types**: feat, fix, refactor, docs, chore, test

**Example**: `feat: add Tank unit with heavy armor`

---

## Common Commands

### Unity
- `/test-game` - Launch Game scene

### Git
```bash
git status
git add . && git commit -m "feat: description"
git log --oneline -5
```

### Claude Code
- `/gen-commit` - Generate commit message

---

## Known Issues

| Issue | Solution |
|-------|----------|
| Input System errors | Use `UnityEngine.InputSystem` API |
| Sprite not centered | Always add +0.5f offset |
| Assembly errors | Add dependency in .asmdef |

---

## Tips for Claude Code

1. **Coordinate system**: Grid = integers, World = floats (+0.5f)
2. **State machines**: See `VehicleMovement.cs` for pattern
3. **New Input System only**: Never use legacy `Input`
4. **Assembly Definitions**: Check dependencies before adding references
5. **Debug with Gizmos**: Use `OnDrawGizmos()` for visualization
6. **Test frequently**: Use `/test-game` + numpad controls

---

## Quick Reference

| Item | Value |
|------|-------|
| **Grid** | 20×20 cells, 1.0 unit, +0.5f centering |
| **Sprites** | 128 PPU, Point filter |
| **Input** | New Input System (`UnityEngine.InputSystem`) |
| **Managers** | `FindFirstObjectByType<T>()` |
| **Commits** | `type: message` |

---

**Last Updated**: 2025-11-24
**Current Focus**: 100% composition architecture - zero code for new units
**Next Milestone**: 3rd unit (Tank/Harvester) OR combat OR buildings

**Documentation**:
- [GUIDE.md](GUIDE.md) - Developer guide (architecture, systems, workflows)
- [CHANGELOG.md](CHANGELOG.md) - Change history
- [docs/](docs/) - Technical documentation (UNITS, TOOLS, ANIMATION)
