# Conventions de développement - Command and Conquer

## Conventions C#

### Nommage des classes
```csharp
// PascalCase pour les classes, interfaces, structs
public class UnitController { }
public interface IMovable { }
public struct GridPosition { }
```

### Nommage des variables
```csharp
public class Example
{
    // Constantes: UPPER_CASE avec underscore
    private const int MAX_HEALTH = 100;

    // Variables privées: camelCase
    private int currentHealth;
    private float moveSpeed;

    // SerializeField: camelCase (privé)
    [SerializeField] private Transform targetTransform;

    // Propriétés publiques: PascalCase
    public int CurrentHealth { get; private set; }
    public float MoveSpeed => moveSpeed;
}
```

### Nommage des méthodes
```csharp
// PascalCase pour les méthodes
public void MoveToPosition(Vector2 position) { }
private void UpdateHealth() { }
protected virtual void OnDeath() { }
```

### Namespaces
```csharp
// Structure: CommandAndConquer.<Catégorie>.<Fonctionnalité>
namespace CommandAndConquer.Units.Infantry
{
    public class InfantryController { }
}

namespace CommandAndConquer.Units.TankHeavy
{
    public class TankHeavyController { }
}

namespace CommandAndConquer.Core
{
    public class UnitBase { }
}
```

### Organisation du code
```csharp
public class UnitController : MonoBehaviour
{
    // 1. Constantes
    private const float MOVE_SPEED = 5f;

    // 2. Variables SerializeField
    [SerializeField] private Transform model;

    // 3. Variables privées
    private GridPosition currentPosition;

    // 4. Propriétés publiques
    public bool IsMoving { get; private set; }

    // 5. Méthodes Unity (Awake, Start, Update, etc.)
    private void Awake() { }
    private void Update() { }

    // 6. Méthodes publiques
    public void MoveTo(GridPosition target) { }

    // 7. Méthodes privées
    private void UpdatePosition() { }
}
```

## Conventions Unity Assets

### Prefabs
- **Format**: `PascalCase`
- **Exemples**: `TankHeavy.prefab`, `Infantry.prefab`, `TileSand.prefab`

### Scènes
- **Format**: `PascalCase`
- **Exemples**: `MainMenu.unity`, `Game.unity`, `TestCamera.unity`

### Sprites
- **Format**: `snake_case`
- **Exemples**: `tank_heavy_01.png`, `infantry_idle_00.png`, `tile_sand.png`

### ScriptableObjects
- **Format**: `PascalCase` avec suffixe `Data`
- **Exemples**: `TankHeavyData.asset`, `InfantryData.asset`

### Materials
- **Format**: `PascalCase` avec suffixe `Mat`
- **Exemples**: `GridMat.mat`, `UnitHighlightMat.mat`

## Organisation des fichiers

### Structure par type de fonctionnalité
```
_Project/
├── Core/                    # Code partagé entre fonctionnalités
│   ├── Scripts/
│   │   ├── UnitBase.cs
│   │   ├── IMovable.cs
│   │   └── GridPosition.cs
│   └── Data/
├── Units/                   # Toutes les unités
│   ├── Infantry/            # Fonctionnalité complète Infantry
│   │   ├── Scripts/
│   │   │   ├── InfantryController.cs
│   │   │   └── InfantryMovement.cs
│   │   ├── Prefabs/
│   │   │   └── Infantry.prefab
│   │   ├── Sprites/
│   │   │   ├── infantry_idle.png
│   │   │   └── infantry_walk.png
│   │   └── Data/
│   │       └── InfantryData.asset
│   └── TankHeavy/           # Fonctionnalité complète TankHeavy
│       ├── Scripts/
│       │   ├── TankHeavyController.cs
│       │   └── TankHeavyTurret.cs
│       ├── Prefabs/
│       │   └── TankHeavy.prefab
│       ├── Sprites/
│       │   ├── tank_heavy_body.png
│       │   └── tank_heavy_turret.png
│       └── Data/
│           └── TankHeavyData.asset
├── Camera/
│   ├── Scripts/
│   └── Prefabs/
├── Grid/
│   ├── Scripts/
│   └── Prefabs/
└── Map/
    ├── Scripts/
    ├── Sprites/
    └── Tilemaps/
```

### Règles
- **Une fonctionnalité = un dossier**: chaque type d'unité ou système a son propre dossier
- **Core/ pour le partagé**: classes de base, interfaces, utilitaires communs
- **Pas d'espaces** dans les noms de fichiers
- **Un script par fichier**, nom du fichier = nom de la classe

## Bonnes pratiques

### Scripts
- Éviter les scripts trop longs (max 300 lignes)
- Une classe = une responsabilité
- Commenter les méthodes publiques complexes
- Utiliser `[Tooltip("...")]` pour les SerializeField
- Utiliser Core/ pour les classes de base et interfaces partagées

### Prefabs
- Nommer les GameObjects enfants clairement (Model, Visual, Collider)
- Éviter les références croisées entre fonctionnalités
- Utiliser Core/ pour les dépendances partagées

### Performances
- Utiliser object pooling pour les unités
- Cacher les composants dans Awake/Start
- Éviter GetComponent dans Update
- Utiliser des layers et tags pour l'optimisation