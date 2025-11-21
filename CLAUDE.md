# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Command and Conquer - RTS 2D** is an educational project recreating a classic Command and Conquer RTS in 2D using Unity 6. The project is developed with Claude Code assistance and follows a modular, feature-focused architecture.

- **Engine**: Unity 6
- **Pipeline**: Universal Render Pipeline (URP) 2D
- **Language**: C# with .NET Standard 2.1
- **Input**: New Input System (NOT legacy Input class)

## Current State

**Branch**: `feature/selection-visual-upgrade`
**Phase**: Selection system visual enhancement ✅ COMPLETE

### Completed Features
- ✅ Grid system (20x20, 1.0 unit cells)
- ✅ Camera controller (WASD, edge scroll, zoom)
- ✅ Buggy unit with grid-based movement (speed: 4.0)
- ✅ Artillery unit with grid-based movement (speed: 1.5)
- ✅ Mouse-based selection system (click to select/move units)
- ✅ **Corner bracket selection visual** (white L-shaped brackets in 4 corners)
- ✅ **Animated cursor system** (hover feedback with 6-frame animation)
- ✅ State machine for unit movement (Idle, Moving, WaitingForNextCell, Blocked)
- ✅ GridPathfinder utility for path calculation (8 directions)
- ✅ Modular debug visualization system
- ✅ Collision detection and avoidance system with retry mechanism
- ✅ Atomic cell reservation system (prevents race conditions)
- ✅ Multi-unit collision handling validated

### Recent Refactorings
- ✅ **VehicleMovement** - Factorized movement logic (~574 lines eliminated)
- ✅ **VehicleContext** - Centralized shared state for vehicles
- ✅ **SelectableComponent** - Reusable selection visual feedback (~50 lines eliminated)
- ✅ **CornerBracketSelector** - Professional corner bracket selection visual system

### Next Steps
**Option A:** Add cursor system for hover and destination feedback
**Option B:** Add 8-direction animations for vehicles
**Option C:** Continue with 3rd unit (Tank/Harvester)
**Option D:** Implement advanced features (multi-selection, formations, combat)

See BUGGY_IMPLEMENTATION.md and ROADMAP.md for detailed plan.

## Architecture Overview

This project uses a **modular, component-based architecture** with Assembly Definitions for clean separation of concerns.

### Module Structure

```
CommandAndConquer/
├── Core/           # Base classes, interfaces, shared types (no dependencies)
├── Grid/           # Grid system (depends on Core)
├── Camera/         # RTS camera (minimal dependencies)
├── Gameplay/       # Gameplay systems (selection, cursor)
├── Units/          # Unit implementations (depends on Core, Grid)
│   ├── Common/     # Shared vehicle systems
│   │   ├── Vehicle/     # VehicleMovement, VehicleContext
│   │   └── Selection/   # SelectableComponent, CornerBracketSelector
│   ├── Buggy/      # Fast reconnaissance vehicle
│   └── Artillery/  # Slow heavy artillery
└── Map/            # Terrain and tilemap
```

### Dependency Graph

```
Core (foundation)
 ├─> Grid (uses GridPosition)
 ├─> Camera (uses config types)
 ├─> Gameplay (uses Core, Grid - SelectionManager, CursorManager)
 ├─> Units (uses UnitBase, IMovable, ISelectable, GridPosition, UnitData)
 │    ├─> Common (shared: VehicleMovement, SelectableComponent, CornerBracketSelector)
 │    ├─> Buggy (uses Common)
 │    └─> Artillery (uses Common)
 └─> Map (uses Grid for alignment)
```

## Key Systems

### 1. Grid System (`CommandAndConquer.Grid`)

**Purpose**: Logical 20x20 grid for unit movement and positioning

**Key Files**:
- `Grid/Scripts/GridManager.cs` - Singleton-like grid manager
- `Grid/Scripts/GridCell.cs` - Individual cell state
- `Grid/Scripts/GridPathfinder.cs` - Static pathfinding utility

### Important Details

**Grid Configuration**:
- Grid size: 20x20 cells, each 1.0 Unity unit
- Cell centering: Grid (5,5) → World (5.5, 5.5) - **always add +0.5f**
- Conversion methods: `GetGridPosition(Vector3)`, `GetWorldPosition(GridPosition)`
- Pathfinding: `GridPathfinder.CalculateStraightPath()` for 8-direction movement
- Debug: Green grid lines from GridManager, red cells for occupied positions

**Collision Management System** (NEW):
- **Unit Registration**: Units must register with `RegisterUnit()` at spawn and `UnregisterUnit()` at destruction
- **Atomic Reservation**: `TryMoveUnitTo(unit, position)` reserves cells atomically to prevent race conditions
- **Coherence Verification**: LateUpdate() verifies grid coherence every ~60 frames as a safety net
- **Auto-cleanup**: Destroyed units are automatically removed from tracking

**Key Methods**:
```csharp
// Unit lifecycle
bool RegisterUnit(UnitBase unit, GridPosition position)      // Call in Start()
void UnregisterUnit(UnitBase unit)                           // Call in OnDestroy()

// Movement (ATOMIC - use this!)
bool TryMoveUnitTo(UnitBase unit, GridPosition newPos)      // Reserves cell atomically

// Query (read-only, no reservation)
bool IsCellAvailableFor(GridPosition pos, UnitBase unit)    // Check if cell is free

// Debug
GridPosition GetUnitGridPosition(UnitBase unit)             // Get tracked position
int GetRegisteredUnitCount()                                 // Count registered units
```

**IMPORTANT**: Always use `TryMoveUnitTo()` for movement to avoid race conditions. `IsCellAvailableFor()` alone does NOT reserve the cell!

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
private enum MovementState {
    Idle,                  // Stationary
    Moving,                // Moving to next cell
    WaitingForNextCell,    // Next cell occupied, retrying
    Blocked                // Gave up after max retries
}

private void Update() {
    switch (state) {
        case MovementState.Idle: break;
        case MovementState.Moving: HandleMoving(); break;
        case MovementState.WaitingForNextCell: HandleWaitingForNextCell(); break;
        case MovementState.Blocked: break;
    }
}
```

**Movement Flow with Collision Detection**:
1. `MoveTo(GridPosition)` called on BuggyController
2. BuggyMovement validates request and calculates path using `GridPathfinder.CalculateStraightPath()`
3. **Atomic reservation** of first cell with `gridManager.TryMoveUnitTo(unit, firstCell)`:
   - If successful → State = `Moving`, begin physical movement
   - If occupied → State = `WaitingForNextCell`, start retry timer
4. For each cell in path (when `Moving`):
   - Interpolate with `Vector3.MoveTowards()`
   - Snap when distance < 0.01
   - **Try to reserve next cell** with `TryMoveUnitTo()`:
     - If successful → Continue moving
     - If occupied → State = `WaitingForNextCell`
5. When `WaitingForNextCell`:
   - Retry every 0.3s (up to 20 times = 6 seconds)
   - If cell becomes free → Resume movement
   - If timeout → State = `Blocked`

**Debug Visualization** (BuggyMovementDebug.cs):
- **White**: Idle
- **Green**: Moving
- **Orange**: WaitingForNextCell (collision detected, retrying)
- **Red**: Blocked (gave up after timeout)

**Direction Changes**: If `MoveTo()` called during movement, the system recalculates path from current target cell to new destination. The unit smoothly transitions to the new path without stopping.

**Pathfinding**: Uses `GridPathfinder.CalculateStraightPath()` which supports 8-direction movement (N, NE, E, SE, S, SW, W, NW) using `Math.Sign()` for delta calculation.

**Collision Prevention**: Uses atomic `TryMoveUnitTo()` to prevent race conditions where multiple units try to occupy the same cell simultaneously.

### 4. Selection System (`CommandAndConquer.Units.Common`)

**Purpose**: Visual feedback system for unit selection with professional corner brackets

**Key Files**:
- `Units/Common/Selection/SelectableComponent.cs` - Selection coordinator
- `Units/Common/Selection/CornerBracketSelector.cs` - Corner bracket visual display
- `Units/Common/Selection/SelectionVisualType.cs` - Enum for visual types
- `Units/Common/Selection/Sprites/corner_bracket_l.png` - White L-shaped sprite

**Architecture**:

The selection system uses a **coordinator pattern** where `SelectableComponent` acts as the central controller:

```
UnitBase (events)
    ↓ OnSelectedEvent / OnDeselectedEvent
SelectableComponent (coordinator)
    ↓ calls ShowBrackets() / HideBrackets()
CornerBracketSelector (passive display)
    → Creates 4 corner GameObjects with sprites
```

**SelectableComponent** (Coordinator):
- Subscribes to `UnitBase.OnSelectedEvent` and `OnDeselectedEvent`
- Supports multiple visual types via `SelectionVisualType` enum
- Controls `CornerBracketSelector` by calling public methods
- Can switch between `SpriteColor` (legacy) and `CornerBrackets` (new)

**CornerBracketSelector** (Passive Component):
- Creates 4 child GameObjects with rotated L-shaped sprites
- **Does NOT** subscribe to events (passive, controlled by SelectableComponent)
- Exposes `ShowBrackets()` and `HideBrackets()` public methods
- Configurable distance, size, sorting order, and sprite

**Visual Types**:
```csharp
public enum SelectionVisualType {
    SpriteColor,      // Changes sprite color (legacy)
    CornerBrackets    // Shows white L-brackets in corners (default)
}
```

**Corner Bracket Configuration**:
```csharp
[Header("Corner Bracket Settings")]
[SerializeField] private Sprite cornerBracketSprite;    // L-shaped white sprite
[SerializeField] private float cornerDistance = 0.5f;   // Distance from center
[SerializeField] private float cornerSize = 0.25f;      // Sprite scale
[SerializeField] private int sortingOrder = 10;         // Render above unit
```

**Corner Rotations** (for proper bracket orientation):
- **Top-Left**: 0° → ┌
- **Top-Right**: -90° → ┐
- **Bottom-Right**: 180° → ┘
- **Bottom-Left**: 90° → └

**Usage on Units**:

Both `Buggy` and `Artillery` prefabs have:
1. `SelectableComponent` with `visualType = CornerBrackets`
2. `CornerBracketSelector` with sprite and configuration
3. White L-sprite assigned in Inspector

**Key Methods**:
```csharp
// SelectableComponent (called by UnitBase events)
private void HandleSelected()   // Triggers visual feedback
private void HandleDeselected() // Removes visual feedback

// CornerBracketSelector (called by SelectableComponent)
public void ShowBrackets()  // Activates 4 corner sprites
public void HideBrackets()  // Deactivates 4 corner sprites
```

**Design Principles**:
- **Separation of Concerns**: SelectableComponent = logic, CornerBracketSelector = display
- **Reusability**: CornerBracketSelector can be controlled by any system
- **Flexibility**: Easy to add new visual types (health bars, circles, etc.)
- **Performance**: Sprites created once in Awake(), only toggled on/off

### 5. Camera System (`CommandAndConquer.Camera`)

**Purpose**: RTS-style camera with WASD, edge scrolling, and zoom

**Key Files**:
- `Camera/Scripts/CameraController.cs` - Main camera logic
- `Camera/Scripts/CameraInputActions.cs` - Input System wrapper
- `Camera/Scripts/CameraBounds.cs` - ScriptableObject for bounds

**Controls**:
- WASD/Arrows: Move camera
- Mouse edges: Edge scrolling
- Mouse wheel: Zoom in/out

### 6. Cursor System (`CommandAndConquer.Gameplay`)

**Purpose**: Dynamic cursor feedback system with animated cursor support for unit hover

**Key Files**:
- `Gameplay/Scripts/CursorManager.cs` - Singleton cursor manager with animation support
- `Gameplay/Scripts/CursorType.cs` - Enum for cursor types
- `Gameplay/Scripts/SelectionManager.cs` - Handles unit hover detection
- `Gameplay/Editor/CursorSpriteImporter.cs` - Auto-configures cursor sprites

**Architecture**:

The cursor system uses a **singleton pattern** where `CursorManager` manages cursor state and `SelectionManager` triggers cursor changes based on hover detection:

```
SelectionManager (hover detection)
    ↓ calls SetCursor() / ResetCursor()
CursorManager (singleton)
    ↓ uses Unity Cursor API
Cursor.SetCursor(Texture2D, hotspot, mode)
```

**CursorManager** (Singleton):
- Manages cursor textures for different states
- Supports animated cursors (multiple frames with configurable FPS)
- Exposes `SetCursor(CursorType)` and `ResetCursor()` public API
- Updates animation frames in Update() when active

**SelectionManager** (Modified):
- Added `HandleUnitHover()` method called every frame
- Performs raycast to detect units under mouse cursor
- Calls `CursorManager.SetCursor(CursorType.Hover)` when hovering over unit
- Calls `CursorManager.ResetCursor()` when leaving unit

**Cursor Types**:
```csharp
public enum CursorType {
    Default,              // System default cursor
    HoverFriendlyUnit,    // Animated cursor when hovering friendly units (6 frames)
    Move,                 // Movement destination cursor
    Attack                // Enemy target cursor
}
```

**Cursor Configuration**:
```csharp
[Header("Cursor Textures")]
[SerializeField] private Texture2D[] hoverFriendlyUnitFrames;  // 6 frames for animation
[SerializeField] private Texture2D moveCursor;
[SerializeField] private Texture2D attackCursor;

[Header("Animation Settings")]
[SerializeField] private float animationFPS = 10f;              // Animation speed

[Header("Cursor Hotspot")]
[SerializeField] private Vector2 cursorHotspot = new Vector2(24, 24);  // Click point (center for 48x48)
```

**Sprite Import Settings** (Auto-configured by CursorSpriteImporter):
- **isReadable**: true (REQUIRED for Cursor.SetCursor)
- **Filter Mode**: Point (pixel perfect)
- **Compression**: None
- **Alpha Is Transparency**: true
- **Max Size**: 64 (optimal for cursors)
- **Mipmap**: false

**Setup in Unity**:
1. Create empty GameObject "CursorManager" in scene
2. Add `CursorManager` component
3. Assign cursor sprite arrays in Inspector (e.g., 6 frames for hover animation)
4. Configure hotspot (center of cursor sprite, e.g., (24,24) for 48x48 cursor)
5. Link `CursorManager` reference in `SelectionManager`
6. Run menu: `Tools > Command & Conquer > Reconfigure Cursor Sprites` (one-time setup)

**Key Methods**:
```csharp
// CursorManager (public API)
public void SetCursor(CursorType type)  // Changes cursor to specified type
public void ResetCursor()                // Resets to system default

// SelectionManager (hover detection)
private void HandleUnitHover()          // Detects units under cursor, updates CursorManager
```

**Animation System**:
- Cursors with multiple frames (e.g., `hoverFriendlyUnitFrames[]`) are automatically animated
- Animation updates in `CursorManager.Update()` based on `animationFPS`
- Frame index cycles through array using modulo: `currentFrame = (currentFrame + 1) % frames.Length`
- Single-texture cursors (Move, Attack) are static (no animation)

**Design Principles**:
- **Singleton Access**: `CursorManager.Instance` available globally
- **Separation of Concerns**: SelectionManager = detection, CursorManager = display
- **Performance**: Textures loaded once in Inspector, only SetCursor() calls in runtime
- **Extensibility**: Easy to add new cursor types by extending `CursorType` enum

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

**Last Updated**: 2025-01-21
**Current Focus**: Two working vehicle units (Buggy + Artillery) with shared refactored systems
**Next Milestone**: 3rd unit (Tank/Harvester), animations, or advanced features
