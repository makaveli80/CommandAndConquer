# Quick Reference : Système Composition-Based

Guide de référence rapide pour créer des unités avec le nouveau système.

---

## Créer une nouvelle unité (après Phase 2)

### 1. Créer UnitData asset

```
Project → Right-click → Create → Command & Conquer → Unit Data
```

**Configurer** :
- Unit Name: "Tank"
- Description: "Heavy assault vehicle"
- Move Speed: 2.5
- Can Move: ✓

---

### 2. Créer GameObject dans la scène

```
Hierarchy → Right-click → Create Empty → "Tank"
```

---

### 3. Ajouter composants

| Composant | Obligatoire | Configuration |
|-----------|-------------|---------------|
| **Unit** | ✅ Oui | Assigner UnitData asset |
| **VehicleMovement** | ⚠️ Si véhicule | Auto-configuré |
| **SelectableComponent** | ⚠️ Si sélectionnable | Visual Type = CornerBrackets |
| **VehicleAnimator** | ⚠️ Si animations 8-dir | Assigner VehicleAnimationData |
| **SpriteRenderer** | ✅ Oui | Assigner sprite |
| **BoxCollider2D** | ✅ Oui | Taille = sprite |

---

### 4. Créer Prefab

```
Drag GameObject from Hierarchy → Project folder → Create Prefab
```

---

## Hiérarchie des composants

### Véhicule complet

```
Tank (GameObject)
├── Unit ← Données, état, événements
├── VehicleMovement ← Mouvement grille 8-dir
├── SelectableComponent ← Feedback sélection
├── VehicleAnimator ← Sprites directionnels
├── SpriteRenderer ← Visuel
└── BoxCollider2D ← Collision/sélection
```

### Bâtiment (exemple futur)

```
Barracks (GameObject)
├── Unit ← Données, état, événements
├── SelectableComponent ← Feedback sélection
├── SpriteRenderer ← Visuel
└── BoxCollider2D ← Collision/sélection
```

*Pas de VehicleMovement (canMove = false dans UnitData)*

---

## Composants disponibles

### Unit (Core)

**Namespace** : `CommandAndConquer.Units.Common`
**Dépendances** : Aucune
**Configuration** :
- `unitData` : UnitData asset (ScriptableObject)

**Implémente** :
- `IMovable` (délègue à VehicleMovement)
- `ISelectable` (gère événements)

**Propriétés publiques** :
```csharp
GridPosition CurrentGridPosition { get; }
UnitData Data { get; }
GridManager GridManager { get; }
string UnitName { get; }
```

---

### VehicleMovement

**Namespace** : `CommandAndConquer.Units._Project.Units.Common.Vehicle`
**Dépendances** : `[RequireComponent(typeof(Unit))]`
**Configuration** : Auto (trouve Unit via GetComponent)

**Implémente** :
- `IMovementComponent`

**Méthode publique** :
```csharp
void MoveTo(GridPosition targetPosition)
bool IsMoving { get; }
Vector3 CurrentTargetWorldPosition { get; }
```

**Caractéristiques** :
- Mouvement case-par-case
- 8 directions (N, NE, E, SE, S, SW, W, NW)
- Pathfinding ligne droite
- Retry system (collision)

---

### SelectableComponent

**Namespace** : `CommandAndConquer.Units.Common`
**Dépendances** : `[RequireComponent(typeof(Unit))]`
**Configuration** :
- `visualType` : `CornerBrackets` (recommandé) ou `SpriteColor`

**Écoute** :
- `Unit.OnSelectedEvent`
- `Unit.OnDeselectedEvent`

**Contrôle** :
- `CornerBracketSelector` (si CornerBrackets)
- `SpriteRenderer.color` (si SpriteColor)

---

### VehicleAnimator

**Namespace** : `CommandAndConquer.Units.Common`
**Dépendances** : `[RequireComponent(typeof(SpriteRenderer))]`
**Configuration** :
- `animationData` : VehicleAnimationData asset (8 sprites)
- `debugMode` : bool (affiche flèche direction)

**Auto-découvre** :
- `IMovementComponent` (VehicleMovement, InfantryMovement, etc.)

**Méthode publique** :
```csharp
void SetDirection(DirectionType direction)
DirectionType CurrentDirection { get; }
```

---

## Interfaces

### IMovementComponent

Implémentée par : `VehicleMovement`, `InfantryMovement` (futur)

```csharp
void MoveTo(GridPosition targetPosition);
bool IsMoving { get; }
Vector3 CurrentTargetWorldPosition { get; }
```

---

### IMovable

Implémentée par : `Unit`

```csharp
void MoveTo(GridPosition targetPosition);
bool IsMoving { get; }
float MoveSpeed { get; }
```

---

### ISelectable

Implémentée par : `Unit`

```csharp
void OnSelected();
void OnDeselected();
bool IsSelected { get; }
```

---

## ScriptableObjects

### UnitData

**Menu** : `Create → Command & Conquer → Unit Data`

**Champs** :
```csharp
string unitName;
string description;
float moveSpeed = 5f;
bool canMove = true;
GameObject prefab;
```

---

### VehicleAnimationData

**Menu** : `Create → Command & Conquer → Vehicle Animation Data`

**Champs** :
```csharp
Sprite spriteE;   // East (0°)
Sprite spriteNE;  // Northeast (45°)
Sprite spriteN;   // North (90°)
Sprite spriteNW;  // Northwest (135°)
Sprite spriteW;   // West (180°)
Sprite spriteSW;  // Southwest (225°)
Sprite spriteS;   // South (270°)
Sprite spriteSE;  // Southeast (315°)
DirectionType defaultDirection = DirectionType.S;
```

---

## Pattern de composition

### Avant (Héritage)

```csharp
// Nouveau véhicule = 3 fichiers
TankController.cs (inherit UnitBase)
TankMovement.cs (inherit VehicleMovement)
TankContext.cs (inherit VehicleContext)
```

**Problème** : Code dupliqué, couplage fort

---

### Après (Composition)

```
Nouveau véhicule = 0 fichiers de code !
→ Créer UnitData asset
→ Ajouter composants dans Unity
→ Créer prefab
```

**Avantage** : Zéro code, assemblage éditeur

---

## Extensibilité future

### Ajouter nouveau type de mouvement

**Exemple** : InfantryMovement (soldats)

1. Créer `InfantryMovement.cs` :

```csharp
[RequireComponent(typeof(Unit))]
public class InfantryMovement : MonoBehaviour, IMovementComponent
{
    private Unit unit;

    void Awake() {
        unit = GetComponent<Unit>();
    }

    public void MoveTo(GridPosition target) {
        // Logique différente (formation, terrain varié, etc.)
    }

    public bool IsMoving { get; }
    public Vector3 CurrentTargetWorldPosition { get; }
}
```

2. Utiliser :

```
Soldier (GameObject)
├── Unit
├── InfantryMovement ← Nouveau !
├── SelectableComponent
└── SpriteRenderer
```

**VehicleAnimator fonctionne avec n'importe quel IMovementComponent !**

---

## Debug

### Activer debug visuel

**VehicleMovement** : État dans Console
```
[VehicleMovement] Path calculated to (10, 5) (12 steps)
[VehicleMovement] Cell (6, 5) reserved, starting movement
```

**VehicleAnimator** : Flèche direction
```csharp
debugMode = true // Yellow arrow gizmo
```

**GridManager** : Grille verte
```
Gizmos affichent grille + cellules occupées
```

---

## Résumé Architecture

```
CommandAndConquer.Core (interfaces, types de base)
  ↓
CommandAndConquer.Grid (grille, pathfinding)
  ↓
CommandAndConquer.Units (composants d'unités)
  ├─ Common/Unit.cs (générique)
  ├─ Common/Vehicle/VehicleMovement.cs
  ├─ Common/Selection/SelectableComponent.cs
  └─ Common/Animation/VehicleAnimator.cs
```

**Principe** : Composition > Héritage

---

**Dernière mise à jour** : 2025-11-23 (Phase 2)
