# Types d'unités - Command and Conquer

Liste de tous les types d'unités disponibles dans le jeu.

## Structure

Chaque unité est organisée dans `Assets/_Project/Units/<TypeUnité>/` avec:
- `Scripts/`: Code spécifique à l'unité
- `Prefabs/`: Prefab de l'unité
- `Sprites/`: Graphismes de l'unité
- `Data/`: ScriptableObject de configuration

## Unités disponibles

### Infantry
**Chemin**: `Assets/_Project/Units/Infantry/`
**Description**: Unité d'infanterie de base
**Namespace**: `CommandAndConquer.Units.Infantry`

**Fichiers**:
- Scripts: `InfantryController.cs`, `InfantryMovement.cs`
- Prefab: `Infantry.prefab`
- Sprites: `infantry_idle.png`, `infantry_walk.png`
- Data: `InfantryData.asset`

### TankHeavy
**Chemin**: `Assets/_Project/Units/TankHeavy/`
**Description**: Tank lourd avec tourelle rotative
**Namespace**: `CommandAndConquer.Units.TankHeavy`

**Fichiers**:
- Scripts: `TankHeavyController.cs`, `TankHeavyTurret.cs`
- Prefab: `TankHeavy.prefab`
- Sprites: `tank_heavy_body.png`, `tank_heavy_turret.png`
- Data: `TankHeavyData.asset`

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