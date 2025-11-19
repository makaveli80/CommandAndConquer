# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Command and Conquer - RTS 2D** is an educational project recreating a classic Command and Conquer RTS in 2D using Unity 6. The project is developed with Claude Code assistance and follows a modular, feature-focused architecture.

- **Engine**: Unity 6
- **Pipeline**: Universal Render Pipeline (URP) 2D
- **Language**: C# with .NET Standard 2.1
- **Input**: New Input System (NOT legacy Input class)

## Current State

**Branch**: `feature/setup-buggy-unit`
**Phase**: Phase 2 - Buggy unit implementation (refactoring complete)

### Completed Features
- ✅ Grid system (20x20, 1.0 unit cells)
- ✅ Camera controller (WASD, edge scroll, zoom)
- ✅ Buggy unit with grid-based movement
- ✅ State machine for unit movement
- ✅ GridPathfinder utility for path calculation
- ✅ Modular debug visualization system

### Next Steps
See BUGGY_IMPLEMENTATION.md Step 7 and ROADMAP.md for detailed plan.

## Architecture Overview

This project uses a **modular, component-based architecture** with Assembly Definitions for clean separation of concerns.

### Module Structure

```
CommandAndConquer/
├── Core/           # Base classes, interfaces, shared types (no dependencies)
├── Grid/           # Grid system (depends on Core)
├── Camera/         # RTS camera (minimal dependencies)
├── Units/          # Unit implementations (depends on Core, Grid)
│   └── Buggy/      # First unit implementation
└── Map/            # Terrain and tilemap
```

### Dependency Graph

```
Core (foundation)
 ├─> Grid (uses GridPosition)
 ├─> Camera (uses config types)
 ├─> Units (uses UnitBase, IMovable, ISelectable, GridPosition, UnitData)
 │    └─> requires Grid (for movement)
 └─> Map (uses Grid for alignment)
```

## Key Systems

### 1. Grid System (`CommandAndConquer.Grid`)

**Purpose**: Logical 20x20 grid for unit movement and positioning

**Key Files**:
- `Grid/Scripts/GridManager.cs` - Singleton-like grid manager
- `Grid/Scripts/GridCell.cs` - Individual cell state
- `Grid/Scripts/GridPathfinder.cs` - Static pathfinding utility

**Important Details**:
- Grid size: 20x20 cells, each 1.0 Unity unit
- Cell centering: Grid (5,5) → World (5.5, 5.5) - **always add +0.5f**
- Conversion methods: `GetGridPosition(Vector3)`, `GetWorldPosition(GridPosition)`
- Pathfinding: `GridPathfinder.CalculateStraightPath()` for 8-direction movement
- Debug: Green grid lines from GridManager

**Coordinate Conversion**:
```csharp
// Grid → World (add +0.5f for cell center)
Vector3 worldPos = new Vector3(gridPos.x + 0.5f, gridPos.y + 0.5f, 0);

// World → Grid (FloorToInt, no subtraction)
GridPosition gridPos = new GridPosition(
    Mathf.FloorToInt(worldPos.x),
    Mathf.FloorToInt(worldPos.y)
);
```

### 2. Unit System (`CommandAndConquer.Units`)

**Pattern**: Base class + interfaces + composition

```
UnitBase (abstract)
    ↓
BuggyController (implements IMovable, ISelectable)
    ↓ has component
BuggyMovement (state machine)
```

**Key Files**:
- `Core/Scripts/UnitBase.cs` - Abstract base for all units
- `Core/Scripts/IMovable.cs` - Movement capability
- `Core/Scripts/ISelectable.cs` - Selection capability
- `Core/Scripts/GridPosition.cs` - Grid coordinate struct
- `Core/Scripts/UnitData.cs` - ScriptableObject configuration base

**Buggy Implementation** (example unit):
- `Units/Buggy/Scripts/BuggyController.cs` - Unit coordinator
- `Units/Buggy/Scripts/BuggyMovement.cs` - Movement state machine
- `Units/Buggy/Scripts/BuggyMovementDebug.cs` - Optional debug visualization
- `Units/Buggy/Scripts/BuggyTestMovement.cs` - Numpad testing controls
- `Units/Buggy/Data/BuggyData.cs` - Configuration ScriptableObject

### 3. Movement System

**State Machine Pattern**:
```csharp
private enum MovementState { Idle, Moving, Blocked }

private void Update() {
    switch (state) {
        case MovementState.Idle: break;
        case MovementState.Moving: HandleMoving(); break;
        case MovementState.Blocked: HandleBlocked(); break;
    }
}
```

**Movement Flow**:
1. `MoveTo(GridPosition)` called on BuggyController
2. BuggyMovement validates request and calculates path using `GridPathfinder.CalculateStraightPath()`
3. State changes to `Moving` and unit begins moving cell-by-cell
4. For each cell in path:
   - Interpolate with `Vector3.MoveTowards()`
   - Snap when distance < 0.01
   - Update controller's current grid position
5. When path complete, state returns to `Idle`

**Direction Changes**: If `MoveTo()` called during movement, the system recalculates path from current target cell to new destination. The unit smoothly transitions to the new path without stopping.

**Pathfinding**: Uses `GridPathfinder.CalculateStraightPath()` which supports 8-direction movement (N, NE, E, SE, S, SW, W, NW) using `Math.Sign()` for delta calculation.

### 4. Camera System (`CommandAndConquer.Camera`)

**Purpose**: RTS-style camera with WASD, edge scrolling, and zoom

**Key Files**:
- `Camera/Scripts/CameraController.cs` - Main camera logic
- `Camera/Scripts/CameraInputActions.cs` - Input System wrapper
- `Camera/Scripts/CameraBounds.cs` - ScriptableObject for bounds

**Controls**:
- WASD/Arrows: Move camera
- Mouse edges: Edge scrolling
- Mouse wheel: Zoom in/out

## Code Conventions

### Naming Conventions

```csharp
// Classes, Interfaces, Structs: PascalCase
public class UnitController { }
public interface IMovable { }
public struct GridPosition { }

// Constants: UPPER_CASE
private const int MAX_HEALTH = 100;

// Private fields: camelCase
private float moveSpeed;

// SerializeField: camelCase with [Tooltip]
[SerializeField]
[Tooltip("Movement speed in units per second")]
private float moveSpeed = 5f;

// Public properties: PascalCase
public float MoveSpeed => moveSpeed;

// Methods: PascalCase
public void MoveTo(GridPosition target) { }
```

### File Organization

**Code order within a class**:
1. Constants
2. `[SerializeField]` fields
3. Private fields
4. Public properties
5. Unity lifecycle methods (Awake, Start, Update, OnDrawGizmos)
6. Public methods
7. Private methods

**Asset naming**:
- Classes/Scripts: `PascalCase.cs`
- Prefabs: `PascalCase.prefab`
- ScriptableObjects: `PascalCaseData.asset`
- Sprites: `snake_case.png`
- Scenes: `PascalCase.unity`

### Namespaces

```csharp
CommandAndConquer.Core              // Base classes, interfaces
CommandAndConquer.Grid              // Grid system
CommandAndConquer.Camera            // Camera
CommandAndConquer.Units             // Units base
CommandAndConquer.Units.Buggy       // Buggy-specific
CommandAndConquer.Units.Editor      // Editor utilities
CommandAndConquer.Map               // Terrain
```

## Unity-Specific Guidelines

### Input System

**CRITICAL**: This project uses the **New Input System**, NOT `UnityEngine.Input`.

```csharp
// ❌ WRONG - Legacy Input (will error)
if (Input.GetKeyDown(KeyCode.Space)) { }

// ✅ CORRECT - New Input System
using UnityEngine.InputSystem;
if (Keyboard.current.spaceKey.wasPressedThisFrame) { }
```

### Component Discovery

Use `FindFirstObjectByType<T>()` for singleton-like managers (Unity 6 API):

```csharp
// In Awake() or Start()
private void Awake() {
    gridManager = FindFirstObjectByType<GridManager>();
}
```

### Sprites Configuration

- **PPU (Pixels Per Unit)**: 128
- **Filter Mode**: Point (no filter)
- **Compression**: None
- 128px sprite = 1.0 Unity unit = 1 grid cell

### Editor Tools

Located in `Editor/` folders, accessible via **Tools > Command & Conquer**:
- `TerrainSpriteImporter` - Auto-configure terrain sprites
- `UnitSpriteImporter` - Auto-configure unit sprites
- `CreateBuggyData` - Create BuggyData ScriptableObject

## Development Workflows

### Testing in Unity

**Play Mode Testing**:
1. Open `Assets/_Project/Scenes/Game.unity`
2. Press Play ▶️
3. Use numpad for Buggy movement testing:
   - Numpad 1-9: Move to grid positions
   - Numpad 0: Return to (5,5)
   - H: Show help

**Debug Visualization**:
- Green grid lines: Grid cells (from GridManager)
- Buggy movement (from BuggyMovementDebug):
  - White/Green/Red sphere: Unit state (Idle/Moving/Blocked)
  - Gray/Yellow/Cyan cubes: Path cells (visited/current/future)
  - Yellow line: Current path
  - Magenta sphere: Final destination
- Console logs: Tagged with `[ClassName]`

### Adding a New Unit

Follow this template (see `Units/Buggy/` as reference):

1. **Create folder structure**: `Units/YourUnit/{Scripts,Data,Prefabs,Sprites}`
2. **Create YourUnitData.cs**: Inherit from `UnitData`
3. **Create YourUnitController.cs**: Inherit from `UnitBase`, implement `IMovable`, `ISelectable`
4. **Create YourUnitMovement.cs**: State machine for movement logic
5. **Create prefab**: Add all components and configure references
6. **Create ScriptableObject asset**: Configuration in Data folder
7. **Update Assembly Definition**: Add dependencies if needed

### Commit Conventions

Format: `<type>: <concise message>`

**Types**:
- `feat:` - New feature
- `fix:` - Bug fix
- `refactor:` - Code restructuring
- `docs:` - Documentation only
- `chore:` - Maintenance, configuration
- `test:` - Tests

**Examples**:
```
feat: add Buggy unit with grid-based movement
fix: resolve ghost cell occupation on direction change
refactor: simplify pathfinding algorithm
docs: update BUGGY_IMPLEMENTATION.md with step 6
```

## Common Commands

### Unity Operations

**Build/Run**:
- Open Unity Editor
- File > Build Settings > Build (or Ctrl+Shift+B)
- Play Mode: Press Play button or Ctrl+P

**Tests**:
- `/test-game` - Launch Game scene in Unity

### Git Operations

```bash
# View status
git status

# Create commit
git add .
git commit -m "feat: description"

# View recent commits
git log --oneline -5

# Generate commit message
/gen-commit
```

### Custom Slash Commands

- `/create-unit` - Create new unit with template
- `/test-game` - Launch Game scene
- `/gen-commit` - Generate commit message from changes

## Known Issues & Solutions

| Issue | Cause | Solution |
|-------|-------|----------|
| Input System errors | Using legacy `Input` class | Use `UnityEngine.InputSystem` API |
| Sprite not centered in cell | Missing +0.5f offset | Always use `GridManager.GetWorldPosition()` |
| Assembly reference errors | Missing asmdef reference | Add dependency in .asmdef file |
| Direction change not smooth | Old path not cleared | System now recalculates from current cell |

## Important Files Reference

### Documentation
- `README.md` - Project overview
- `ROADMAP.md` - Development plan and next steps
- `BUGGY_IMPLEMENTATION.md` - Buggy unit implementation details
- `CONVENTIONS.md` - Code standards (detailed)
- `UNITS.md` - Unit catalog
- `TOOLS.md` - Editor tools documentation
- `CHANGELOG.md` - Change history

### Core Architecture
- `Core/Scripts/UnitBase.cs` - Abstract unit base (line ~30: virtual methods)
- `Core/Scripts/GridPosition.cs` - Grid coordinate struct (operators at line ~20)
- `Grid/Scripts/GridManager.cs` - Grid logic (conversion methods at line ~50)
- `Grid/Scripts/GridCell.cs` - Cell state management
- `Grid/Scripts/GridPathfinder.cs` - Static pathfinding utility (line ~21: CalculateStraightPath)

### Buggy Implementation
- `Units/Buggy/Scripts/BuggyController.cs` - Main coordinator
- `Units/Buggy/Scripts/BuggyMovement.cs` - State machine (line ~99: MoveTo, line ~242: TryCalculatePath)
- `Units/Buggy/Scripts/BuggyMovementDebug.cs` - Optional Gizmos visualization
- `Units/Buggy/Scripts/BuggyTestMovement.cs` - Numpad testing controls
- `Units/Buggy/Data/BuggyData.cs` - Configuration ScriptableObject

## Tips for Claude Code

1. **Always check coordinate system**: Grid uses integers, World uses floats with +0.5f offset
2. **State machines for complex behavior**: See `BuggyMovement.cs` for pattern
3. **Use Assembly Definitions**: Check dependencies before adding references
4. **New Input System only**: Never use legacy `Input` class
5. **Consult documentation**: ROADMAP.md has detailed next steps
6. **Debug with Gizmos**: Use `OnDrawGizmos()` for visual debugging
7. **One class per file**: File name must match class name exactly
8. **Test frequently**: Use `/test-game` and numpad controls

## Quick Reference

**Grid**: 20x20 cells, 1.0 unit size, +0.5f for centering
**Sprites**: 128 PPU, Point filter, 128px = 1 unit
**Input**: New Input System (`UnityEngine.InputSystem`)
**State Machines**: Enum + switch in Update()
**Managers**: Found via `FindFirstObjectByType<T>()`
**Commit Format**: `type: message` (see conventions above)

---

**Last Updated**: 2025-11-19
**Current Focus**: Buggy unit complete with refactored architecture
**Next Milestone**: Selection and input system (see ROADMAP.md)
