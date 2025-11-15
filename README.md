# Command and Conquer - RTS Classique 2D

Projet éducatif de recréation d'un RTS classique Command and Conquer en 2D, développé avec Unity 6 et assisté de Claude Code.

## Objectif

Explorer les possibilités de développement de jeux avec l'assistance de l'IA pour créer un RTS fonctionnel avec les mécaniques classiques du genre.

## Fonctionnalités - Version 1

- **Carte éditable**: Terrain composé de sprites éditables depuis l'éditeur Unity
- **Caméra dynamique**: Déplacement avec clavier (WASD/Flèches) ou souris (bords d'écran)
- **Système d'unités**: Placement et gestion d'unités depuis l'éditeur
- **Déplacement par grille**: Mouvement des unités basé sur un système de grille

## Technologies

- **Moteur**: Unity 6
- **Pipeline**: Universal Render Pipeline (URP) 2D
- **Packages clés**: 2D Tilemap, 2D Animation, Input System, Aseprite Importer

## Structure du projet (par fonctionnalité)

```
Assets/_Project/
├── Core/                    # Systèmes de base partagés
│   ├── Scripts/
│   └── Data/
├── Camera/                  # Contrôle de la caméra
│   ├── Scripts/
│   └── Prefabs/
├── Grid/                    # Système de grille
│   ├── Scripts/
│   ├── Prefabs/
│   └── Materials/
├── Units/                   # Toutes les unités (voir UNITS.md)
│   ├── Infantry/
│   │   ├── Scripts/
│   │   ├── Prefabs/
│   │   ├── Sprites/
│   │   └── Data/
│   └── TankHeavy/
│       ├── Scripts/
│       ├── Prefabs/
│       ├── Sprites/
│       └── Data/
├── Map/                     # Terrain et tuiles
│   ├── Scripts/
│   ├── Prefabs/
│   ├── Sprites/
│   └── Tilemaps/
└── Scenes/                  # Scènes du jeu
```

## Conventions de nommage

### Scripts C#
- **Classes**: `PascalCase` (ex: `UnitController`, `GridManager`)
- **Méthodes**: `PascalCase` (ex: `MoveToPosition()`)
- **Variables privées**: `camelCase` (ex: `currentHealth`)
- **Constantes**: `UPPER_CASE` (ex: `GRID_SIZE`)

### Assets Unity
- **Prefabs**: `PascalCase` (ex: `TankHeavy`, `TileSand`)
- **Scènes**: `PascalCase` (ex: `MainMenu`, `GameLevel01`)
- **Sprites**: `snake_case` (ex: `tank_01`, `tile_grass`)
- **ScriptableObjects**: `PascalCase` + `Data` (ex: `TankData`)

### Namespaces
```csharp
CommandAndConquer.Core
CommandAndConquer.Units.Infantry
CommandAndConquer.Units.TankHeavy
CommandAndConquer.Grid
CommandAndConquer.Camera
CommandAndConquer.Map
```

## Conventions Git

### Messages de commit
Format standard avec préfixe, message concis, et description optionnelle:

```
<type>: <message concis>

<description détaillée optionnelle>
```

**Types**:
- `feat:` - Nouvelle fonctionnalité
- `fix:` - Correction de bug
- `docs:` - Documentation uniquement
- `refactor:` - Refactoring sans changement de fonctionnalité
- `chore:` - Maintenance, configuration
- `test:` - Ajout ou modification de tests

**Exemples**:
```
feat: add infantry unit movement

Implement grid-based movement for infantry units with pathfinding.
```

```
docs: add project structure and conventions

- Create README.md with project overview
- Add CONVENTIONS.md with coding standards
- Add UNITS.md with available unit types
```

## Développement

### Commandes Claude Code
- `/create-unit` - Créer une nouvelle unité avec le template
- `/test-game` - Lancer la scène de jeu

## Licence

Projet éducatif - Usage personnel