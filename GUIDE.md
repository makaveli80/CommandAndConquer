# Guide Développeur - Command and Conquer RTS

Guide complet pour développer et étendre le projet.

---

## 📋 Table des matières

1. [Architecture du projet](#architecture-du-projet)
2. [Systèmes principaux](#systèmes-principaux)
3. [Créer une nouvelle unité](#créer-une-nouvelle-unité)
4. [Conventions de code](#conventions-de-code)
5. [Commandes et outils](#commandes-et-outils)
6. [Tests et debug](#tests-et-debug)

---

## Architecture du projet

### Structure modulaire

Le projet utilise une **architecture par composition** avec Assembly Definitions pour une séparation claire :

```
CommandAndConquer/
├── Core/           # Interfaces, types partagés (IMovable, ISelectable, UnitData)
├── Grid/           # Système de grille (depends on Core)
├── Camera/         # Caméra RTS (minimal dependencies)
├── Gameplay/       # Sélection, curseurs (depends on Core, Grid)
├── Units/          # Implémentations unités
│   ├── Common/     # Composants réutilisables 100%
│   ├── Buggy/      # Assets spécifiques Buggy
│   └── Artillery/  # Assets spécifiques Artillery
└── Map/            # Terrain, tilemap
```

### Graphe de dépendances

```
Core (base : interfaces, types)
 ├─> Grid (utilise GridPosition)
 ├─> Camera (config types)
 ├─> Gameplay (utilise Core + Grid)
 ├─> Units (utilise Core + Grid)
 │    └─> Common (composants génériques)
 └─> Map (utilise Grid pour alignement)
```

**Principe** : Composition pure, zéro héritage. Nouvelles unités = assemblage de composants dans Unity Editor.

---

## Systèmes principaux

### 1. Système de grille (`Grid/`)

**Configuration** :
- Grille 20×20 cellules, taille 1.0 unité Unity
- Conversion Grid ↔ World : **toujours ajouter +0.5f pour centrage**
- Pathfinding 8 directions (GridPathfinder.CalculateStraightPath)

**Méthodes clés** :
```csharp
// Lifecycle
bool RegisterUnit(Unit unit, GridPosition pos)    // Appeler dans Start()
void UnregisterUnit(Unit unit)                    // Appeler dans OnDestroy()

// Mouvement atomique (empêche race conditions)
bool TryMoveUnitTo(Unit unit, GridPosition newPos)

// Query (read-only)
bool IsCellAvailableFor(GridPosition pos, Unit unit)
```

**⚠️ Important** : Toujours utiliser `TryMoveUnitTo()` pour mouvement. `IsCellAvailableFor()` seul ne réserve PAS la cellule !

**Conversion coordonnées** :
```csharp
// Grid → World (ajouter +0.5f)
Vector3 worldPos = new Vector3(gridPos.x + 0.5f, gridPos.y + 0.5f, 0);

// World → Grid (FloorToInt, pas de soustraction)
GridPosition gridPos = new GridPosition(
    Mathf.FloorToInt(worldPos.x),
    Mathf.FloorToInt(worldPos.y)
);
```

### 2. Système d'unités (`Units/`)

**Pattern composition** :
```
GameObject "Buggy"
├── Unit (composant générique)
├── VehicleMovement (composant générique)
├── SelectableComponent (composant générique)
├── VehicleAnimator (composant générique)
└── SpriteRenderer
```

**Composants Common (réutilisables)** :
- **Unit.cs** - Données, état, événements (remplace tous les Controllers)
- **VehicleMovement.cs** - State machine mouvement véhicules
- **SelectableComponent.cs** - Feedback visuel sélection
- **VehicleAnimator.cs** - Animations 8 directions

### 3. Système de mouvement

**State Machine** :
```csharp
enum MovementState {
    Idle,                  // Stationnaire
    Moving,                // Déplacement vers case suivante
    WaitingForNextCell,    // Case occupée, retry
    Blocked                // Abandon après max retries
}
```

**Flow avec gestion collision** :
1. `MoveTo(GridPosition)` appelé sur Unit
2. Unit délègue à VehicleMovement
3. Calcul path avec `GridPathfinder.CalculateStraightPath()`
4. **Réservation atomique** 1ère cellule : `TryMoveUnitTo()`
5. Si occupée → State = `WaitingForNextCell`, retry toutes les 0.3s (max 20×)
6. Interpolation fluide `Vector3.MoveTowards()` case par case

**Debug** : BuggyMovementDebug affiche état (Blanc=Idle, Vert=Moving, Orange=Waiting, Rouge=Blocked)

### 4. Système de sélection (`Units/Common/Selection/`)

**Architecture coordinateur** :
```
Unit (événements)
  ↓ OnSelectedEvent / OnDeselectedEvent
SelectableComponent (coordinateur)
  ↓ ShowBrackets() / HideBrackets()
CornerBracketSelector (affichage passif)
  → 4 GameObjects avec sprites L rotatés
```

**Types visuels** :
- `SpriteColor` (legacy) - Change couleur sprite
- `CornerBrackets` (défaut) - Corner brackets blancs en L

**Multi-sélection** (`Gameplay/SelectionManager.cs`) :
- HashSet<ISelectable> pour O(1) lookups
- Drag box : seuil 5px, `Physics2D.OverlapAreaAll()`
- Clic simple ou drag-box

### 5. Système de curseurs (`Gameplay/`)

**Types de curseurs** :
- **Default** - Curseur système
- **Hover** - 6 frames @ 10 FPS (survol unité amie)
- **Move** - 4 frames @ 10 FPS (destination valide)

**Priorité** : Hover > Move > Default

**Setup sprites** : Tous les curseurs doivent avoir `isReadable = true` (configuré par CursorSpriteImporter)

### 6. Système d'animation (`Units/Common/Animation/`)

**8 directions** : E, NE, N, NW, W, SW, S, SE (45° par direction)

**Composants** :
- **DirectionType** - Enum 8 directions
- **VehicleAnimationData** - ScriptableObject (8 sprites)
- **VehicleAnimator** - Passive polling de VehicleMovement

**Pattern** : Polling passif dans Update(), sprite change uniquement si direction change.

---

## Créer une nouvelle unité

### Workflow (5 minutes, zéro code)

#### 1. Créer UnitData asset
```
Project → Right-click → Create → Command & Conquer → Unit Data
```
**Configurer** :
- Unit Name: "Tank"
- Move Speed: 2.5
- Can Move: ✓

#### 2. Créer GameObject
```
Hierarchy → Create Empty → "Tank"
```

#### 3. Ajouter composants

| Composant | Configuration |
|-----------|---------------|
| **Unit** | Assigner UnitData asset |
| **VehicleMovement** | Auto-découvre Unit |
| **SelectableComponent** | Visual Type = CornerBrackets |
| **VehicleAnimator** | Assigner VehicleAnimationData asset |
| **SpriteRenderer** | Assigner sprite, Order = 10 |
| **BoxCollider2D** | Is Trigger ✓, Size = sprite bounds |

#### 4. Créer Prefab
```
Drag GameObject → Project folder → Create Prefab
```

**✅ Terminé !** Aucune ligne de code écrite.

---

## Conventions de code

### C# - Nommage

```csharp
// Classes, Interfaces, Structs : PascalCase
public class UnitController { }
public interface IMovable { }
public struct GridPosition { }

// Constantes : UPPER_CASE
private const int MAX_HEALTH = 100;

// Variables privées : camelCase
private float moveSpeed;

// SerializeField : camelCase + [Tooltip]
[SerializeField]
[Tooltip("Vitesse en unités/seconde")]
private float moveSpeed = 5f;

// Propriétés publiques : PascalCase
public float MoveSpeed => moveSpeed;

// Méthodes : PascalCase
public void MoveTo(GridPosition target) { }
```

### Organisation du code

```csharp
public class ExampleController : MonoBehaviour
{
    // 1. Constantes
    private const float MOVE_SPEED = 5f;

    // 2. SerializeField
    [SerializeField] private Transform model;

    // 3. Variables privées
    private GridPosition currentPosition;

    // 4. Propriétés publiques
    public bool IsMoving { get; private set; }

    // 5. Méthodes Unity (Awake, Start, Update, OnDrawGizmos)
    private void Awake() { }
    private void Update() { }

    // 6. Méthodes publiques
    public void MoveTo(GridPosition target) { }

    // 7. Méthodes privées
    private void UpdatePosition() { }
}
```

### Unity Assets

| Type | Format | Exemple |
|------|--------|---------|
| **Prefabs** | PascalCase | `TankHeavy.prefab` |
| **Scènes** | PascalCase | `Game.unity` |
| **Sprites** | snake_case | `tank_heavy_01.png` |
| **ScriptableObjects** | PascalCase + Data | `TankData.asset` |
| **Materials** | PascalCase + Mat | `GridMat.mat` |

### Namespaces

```csharp
CommandAndConquer.Core              // Base, interfaces
CommandAndConquer.Grid              // Grille
CommandAndConquer.Camera            // Caméra
CommandAndConquer.Gameplay          // Sélection, curseurs
CommandAndConquer.Units             // Unités base
CommandAndConquer.Units.Common      // Composants réutilisables
CommandAndConquer.Units.Buggy       // Buggy spécifique
CommandAndConquer.Map               // Terrain
```

### Git - Messages de commit

**Format** : `<type>: <message concis>`

**Types** :
- `feat:` - Nouvelle fonctionnalité
- `fix:` - Correction de bug
- `refactor:` - Refactoring sans changement fonctionnel
- `docs:` - Documentation uniquement
- `chore:` - Maintenance, configuration

**Exemples** :
```
feat: add Tank unit with heavy armor
fix: resolve collision detection race condition
refactor: extract movement logic to VehicleMovement
docs: update GUIDE.md with animation system
```

---

## Commandes et outils

### Commandes Claude Code

- `/gen-commit` - Générer message commit basé sur changements

### Outils Editor (Menu Unity)

**Tools > Command & Conquer** :
- **Reconfigure All Terrain Sprites** - Configure sprites terrain (PPU=128, Point filter)
- **Reconfigure All Unit Sprites** - Configure sprites unités (PPU=128, Multiple mode)
- **Reconfigure Cursor Sprites** - Configure curseurs (isReadable=true)

**Configuration automatique** : AssetPostprocessors configurent sprites lors de l'import dans :
- `Assets/_Project/Map/Sprites/Terrain/`
- `Assets/_Project/Units/*/Sprites/`

Voir [docs/TOOLS.md](docs/TOOLS.md) pour détails.

---

## Tests et debug

### Tests Play Mode

1. Ouvrir `Assets/_Project/Scenes/Game.unity`
2. Press Play ▶️
3. **Sélection** : Clic gauche (simple) ou drag (multi)
4. **Mouvement** : Clic droit sur grille

### Debug Gizmos

**Grid** (vert) :
- Lignes grille vertes
- Cellules occupées en rouge

**VehiculeMovementDebug** (optionnel) :
- Sphère : Blanc=Idle, Vert=Moving, Orange=Waiting, Rouge=Blocked
- Cubes : Gris=visited, Jaune=current, Cyan=future
- Ligne jaune : path actuel
- Sphère magenta : destination finale

**VehicleAnimator** (Debug Mode) :
- Flèche jaune : direction actuelle

### Console Logs

Format : `[ClassName] Message`

Exemples :
```
[GridManager] Registered unit Buggy at (5, 5)
[VehicleMovement] Buggy moving to (10, 10)
[VehicleAnimator] Direction changed to NE (45°)
```

---

## Bonnes pratiques

### Performance
- Object pooling pour unités (futur)
- Cache composants dans Awake/Start
- Éviter GetComponent dans Update
- Utiliser layers/tags pour optimisation

### Architecture
- Une classe = une responsabilité
- Composer plutôt qu'hériter
- Scripts max 300 lignes
- Commenter méthodes publiques complexes

### Unity
- **TOUJOURS** utiliser New Input System (pas legacy Input)
- `FindFirstObjectByType<T>()` pour managers singleton-like (Unity 6 API)
- Sprites : PPU=128, FilterMode=Point, Compression=None
- Tilemap : Tile Anchor (0.5, 0.5) pour centrage

---

**Documentation complète** : Voir [CLAUDE.md](CLAUDE.md) pour détails techniques avancés.
