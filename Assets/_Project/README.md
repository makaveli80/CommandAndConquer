# Organisation des Assets - Command and Conquer

Ce dossier contient tous les assets du jeu organisés **par fonctionnalité**.

## Principe d'organisation

Chaque fonctionnalité regroupe ses propres assets (scripts, prefabs, sprites, etc.) dans un seul dossier.

### Avantages
- Cohésion: tout ce qui concerne une fonctionnalité est au même endroit
- Facilite la navigation et la maintenance
- Simplifie l'ajout/suppression de fonctionnalités
- Réduit les dépendances croisées

## Structure des fonctionnalités

### Core/
Systèmes de base et utilitaires partagés
- `Scripts/`: Classes de base, interfaces, helpers
- `Data/`: ScriptableObjects globaux, constantes

### Camera/
Contrôle de la caméra RTS
- `Scripts/`: `CameraController.cs`, `CameraBounds.cs`
- `Prefabs/`: `MainCamera.prefab`

### Grid/
Système de grille pour le déplacement
- `Scripts/`: `GridManager.cs`, `GridCell.cs`
- `Prefabs/`: Visualisation de la grille
- `Materials/`: Matériaux pour la grille

### Units/
Toutes les unités regroupées par type (voir UNITS.md pour la liste complète)

Exemple: `Units/Infantry/`
- `Scripts/`: `InfantryController.cs`, `InfantryMovement.cs`
- `Prefabs/`: `Infantry.prefab`
- `Sprites/`: `infantry_idle.png`, `infantry_walk.png`
- `Data/`: `InfantryData.asset`

### Map/
Terrain et environnement
- `Scripts/`: `MapGenerator.cs`, `TileController.cs`
- `Prefabs/`: Prefabs de tuiles
- `Sprites/`: Sprites de terrain (sable, herbe, roche)
- `Tilemaps/`: Palettes de tuiles

### Scenes/
Scènes du jeu
- `MainMenu.unity`
- `Game.unity`
- `TestScene.unity`

## Règles de développement

1. **Une fonctionnalité = un dossier**: créer un nouveau dossier pour chaque nouvelle fonctionnalité majeure
2. **Sous-dossiers standards**: Scripts/, Prefabs/, Sprites/, Data/, Materials/
3. **Pas de duplication**: si plusieurs fonctionnalités partagent un asset, le placer dans Core/
4. **Assembly Definitions**: chaque fonctionnalité peut avoir son propre .asmdef pour compilation rapide

## Ajouter une nouvelle fonctionnalité

```
_Project/
└── NouvelleFeature/
    ├── Scripts/
    ├── Prefabs/
    ├── Sprites/ (si besoin)
    └── Data/ (si besoin)
```

Utiliser la commande `/create-unit` pour générer une structure de base.