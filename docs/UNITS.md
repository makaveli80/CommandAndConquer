# Types d'unités - Command and Conquer

Liste de tous les types d'unités disponibles dans le jeu.

## Structure

Chaque unité est organisée dans `Assets/_Project/Units/<TypeUnité>/` avec:
- `Scripts/`: Code spécifique à l'unité
- `Prefabs/`: Prefab de l'unité
- `Sprites/`: Graphismes de l'unité
- `Data/`: ScriptableObject de configuration

## Unités disponibles ✅

### Buggy 🏎️
**Chemin**: `Assets/_Project/Units/Buggy/`
**Description**: Véhicule rapide de reconnaissance
**Namespace**: `CommandAndConquer.Units.Buggy`
**Vitesse**: 4.0 (rapide)

**Fichiers**:
- Prefab: `Buggy.prefab` (avec SelectableComponent, CornerBracketSelector, VehicleAnimator)
- Sprites: `buggy-0000.png` à `buggy-0030.png` (16 sprites, 8 directions × 2 frames)
- Data:
  - `BuggyData.asset` - Configuration ScriptableObject
  - `BuggyAnimationData.asset` - Sprites 8 directions

**Composants utilisés**:
- `VehicleMovement` (Common) - State machine mouvement
- `SelectableComponent` (Common) - Feedback sélection
- `CornerBracketSelector` (Common) - Corner brackets visuels
- `VehicleAnimator` (Common) - Animation 8 directions

### Artillery 🎯
**Chemin**: `Assets/_Project/Units/Artillery/`
**Description**: Véhicule d'artillerie lourde
**Namespace**: `CommandAndConquer.Units.Artillery`
**Vitesse**: 1.5 (lent)

**Fichiers**:
- Prefab: `Artillery.prefab` (avec SelectableComponent, CornerBracketSelector, VehicleAnimator)
- Sprites: `artillery-*.png` (16 sprites, 8 directions)
- Data:
  - `ArtilleryData.asset` - Configuration ScriptableObject
  - `ArtilleryAnimationData.asset` - Sprites 8 directions

**Composants utilisés**:
- Mêmes composants Common que Buggy
- Valide l'architecture réutilisable

## Systèmes Common (Partagés)

Les deux unités utilisent les systèmes partagés dans `Units/Common/`:

### Vehicle (`Units/Common/Vehicle/`)
- `VehicleMovement.cs` - State machine: Idle/Moving/WaitingForNextCell/Blocked

### Selection (`Units/Common/Selection/`)
- `SelectableComponent.cs` - Coordinateur sélection
- `CornerBracketSelector.cs` - Affichage corner brackets
- `SelectionVisualType.cs` - Enum types visuels

### Animation (`Units/Common/Animation/`)
- `DirectionType.cs` - Enum 8 directions (E, NE, N, NW, W, SW, S, SE)
- `DirectionUtils.cs` - Conversion vecteur → direction (Atan2)
- `VehicleAnimationData.cs` - ScriptableObject sprites
- `VehicleAnimator.cs` - Composant animation passive polling

## Ajouter une nouvelle unité

1. Créer le dossier `Assets/_Project/Units/<NomUnité>/`
2. Créer les sous-dossiers: `Scripts/`, `Prefabs/`, `Sprites/`, `Data/`
3. Utiliser `/create-unit` pour générer les fichiers de base
4. Ajouter l'unité dans cette liste

## Classes de base (Core)

Toutes les unités héritent des classes dans `Assets/_Project/Core/Scripts/`:
- `UnitBase.cs`: Classe de base pour toutes les unités
- `IMovable.cs`: Interface pour les unités déplaçables
- `GridPosition.cs`: Structure pour position sur la grille
