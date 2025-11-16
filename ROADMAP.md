# Roadmap - Command and Conquer RTS

Plan de développement du projet Command and Conquer, un RTS 2D éducatif développé avec Unity 6.

---

## 📊 État actuel

### ✅ Phase 1 : Préparation du projet (TERMINÉ)

**Commits 1-5 complétés** - Infrastructure de base en place

- ✅ Documentation complète (README, CONVENTIONS, UNITS, CHANGELOG)
- ✅ Intégration Claude Code (commandes `/create-unit`, `/test-game`)
- ✅ Templates de code Core (UnitBase, interfaces, GridPosition, UnitData)
- ✅ Assembly Definitions pour compilation modulaire
- ✅ Structure de dossiers par fonctionnalité
- ✅ Fichier .editorconfig pour standards de code

**Fichiers Core créés:**
- `Core/Scripts/UnitBase.cs` - Classe abstraite pour unités
- `Core/Scripts/IMovable.cs` - Interface déplacement
- `Core/Scripts/ISelectable.cs` - Interface sélection
- `Core/Scripts/GridPosition.cs` - Structure position grille
- `Core/Scripts/UnitData.cs` - ScriptableObject données unités

**Structure actuelle:**
```
Assets/_Project/
├── Core/               ✅ Templates de base + GridPosition modifié pour 2D
├── Camera/             ✅ Système complet (CameraController, CameraBounds, Input Actions)
├── Grid/               ✅ GridManager + GridCell (système de grille logique)
├── Map/                ✅ TerrainSpriteImporter + Documentation (Tilemap)
├── Units/              📁 Vide (prochaine étape)
├── UI/                 📁 Structure créée
├── Audio/              📁 Structure créée
└── Scenes/             📁 Structure créée
```

### ✅ Commit 6 : Système de caméra RTS (TERMINÉ)

**Implémenté :**
- ✅ CameraController avec déplacement WASD/flèches et edge scrolling
- ✅ CameraBounds (ScriptableObject) pour limites configurables
- ✅ Zoom avec molette de souris
- ✅ Support complet New Input System
- ✅ Documentation complète dans `Camera/README.md`
- ✅ DefaultCameraBounds asset pour configuration par défaut

---

## 🎯 Phase 2 : Systèmes de base (EN COURS)

### Objectif Version 1
Créer un prototype jouable avec les fonctionnalités de base :
- ✅ Caméra déplaçable (WASD/souris)
- ✅ Grille logique et tilemap
- ⏳ Une unité (Infantry) plaçable et déplaçable

---

## 📋 Plan détaillé des prochains commits

### ✅ Commit 7 : Système de grille et tilemap (TERMINÉ)

**Objectif:** Grille logique pour déplacement des unités + support tilemap

**Créé dans `Grid/Scripts/`:**

1. ✅ **GridManager.cs**
   - Namespace: `CommandAndConquer.Grid`
   - Génère la grille au démarrage
   - Dimensions configurables (width=20, height=20)
   - Cell size: 1.0 unité Unity
   - Méthodes: `GetGridPosition(Vector3 worldPos)`, `GetWorldPosition(GridPosition gridPos)`
   - Conversion avec offset +0.5f pour centrage des unités
   - Debug Gizmos pour visualisation de la grille

2. ✅ **GridCell.cs**
   - Représente une cellule de la grille
   - Propriétés: position, occupied
   - Référence à l'unité présente (si occupée)
   - Méthodes: `TryOccupy()`, `Release()`, `ForceOccupy()`

**Créé dans `Map/Editor/`:**

3. ✅ **TerrainSpriteImporter.cs**
   - Script Editor pour auto-configuration des sprites de terrain
   - Configure PPU=128, FilterMode=Point, Compression=Uncompressed
   - Menu Tools: "Reconfigure All Terrain Sprites"

**Créé dans `Core/Scripts/`:**

4. ✅ **GridPosition.cs** (modifié)
   - Adapté pour 2D: utilise `x, y` au lieu de `x, z`
   - Opérateurs +, -, ==, !=
   - HashCode pour utilisation dans dictionnaires

**Documentation créée:**

5. ✅ **Map/TILEMAP_SETUP.md**
   - Guide complet de configuration du Tilemap (10 étapes)
   - Configuration sprites PPU=128 pour cellules 1.0 unité
   - Création Tile Palette et Tiles
   - Setup Grid + Tilemap + GridManager
   - Section dépannage

6. ✅ **Map/RANDOM_BRUSH_GUIDE.md**
   - Guide détaillé peinture avec variation aléatoire
   - 3 méthodes: Sélection multiple, Random Brush, Weighted Random
   - Exemples de configurations
   - Astuces et bonnes pratiques

**Configuration recommandée:**
- Grid size: 20x20
- Cell size: 1.0 unité Unity
- Sprites terrain: PPU=128 (128px = 1.0 unité)
- Tile Anchor: (0.5, 0.5) pour centrage visuel
- Debug Gizmos: Vert transparent pour grille, rouge pour cellules occupées

**Structure actuelle:**
```
Assets/_Project/
├── Core/               ✅ Templates + GridPosition modifié
├── Camera/             ✅ Système complet
├── Grid/               ✅ GridManager + GridCell
├── Map/                ✅ Scripts Editor + Documentation
│   ├── Editor/         ✅ TerrainSpriteImporter
│   ├── Sprites/        📁 Terrain/Clear1/ (sprites utilisateur)
│   ├── Tiles/          📁 À créer par utilisateur
│   └── Palettes/       📁 À créer par utilisateur
├── Units/              📁 Vide (prochaine étape)
└── Scenes/             📁 Structure créée
```

**Tests:**
- ✅ Sprites Clear1 configurés avec PPU=128
- ✅ Tilemap créé et peint (manuel)
- ✅ GridManager ajouté à la scène
- ✅ Grille visualisée avec Gizmos
- ✅ Alignement Tilemap ↔ GridManager vérifié

**Commit message:**
```
feat: implement grid system with tilemap integration

- Add GridManager and GridCell classes for logical grid (1.0 unit cells)
- Update GridPosition to use x,y coordinates for 2D (instead of x,z)
- Fix Grid assembly definition reference to Core module
- Add TerrainSpriteImporter editor script for auto-configuring sprites (PPU=128)
- Create comprehensive tilemap setup documentation (TILEMAP_SETUP.md)
- Add detailed random brush painting guide (RANDOM_BRUSH_GUIDE.md)

Grid system features:
- Cell size: 1.0 Unity unit (128px sprites with PPU=128)
- Centered positioning (+0.5f offset) for optimal unit placement
- Cell occupation tracking for future unit movement
- Debug gizmos for grid visualization and occupied cells
- Simple conversion methods between world and grid positions

Documentation includes:
- Step-by-step tilemap configuration (10 detailed steps)
- Random variation painting techniques (3 methods)
- Troubleshooting section
- Weighted random brush examples

The grid system is ready for unit implementation (Commit 8).
```

---

### Commit 8 : Première unité - Infantry

**Objectif:** Créer l'unité Infantry fonctionnelle

**À créer dans `Units/Infantry/Scripts/`:**

1. **InfantryController.cs**
   - Namespace: `CommandAndConquer.Units.Infantry`
   - Hérite de `UnitBase`
   - Implémente `IMovable`, `ISelectable`
   - Référence au `InfantryData`

2. **InfantryMovement.cs**
   - Gère le déplacement sur la grille
   - Utilise `GridManager` pour navigation
   - Déplacement fluide vers la position cible
   - Callback quand arrivé à destination

3. **InfantryData.cs** (ScriptableObject)
   - Hérite de `UnitData`
   - Stats spécifiques Infantry: moveSpeed = 3f

**À créer dans `Units/Infantry/Prefabs/`:**
- `Infantry.prefab` avec InfantryController

**À créer dans `Units/Infantry/Sprites/`:**
- Placeholder: Capsule ou Sprite simple (peut être remplacé plus tard)

**À créer dans `Units/Infantry/Data/`:**
- `InfantryData.asset` - Instance du ScriptableObject

**Tests:**
- Placer Infantry.prefab dans la scène
- Vérifier qu'il apparaît sur la grille
- (Déplacement testé au Commit 9)

**Commit message:**
```
feat: add Infantry unit

- Add InfantryController inheriting from UnitBase
- Add InfantryMovement for grid-based movement
- Add InfantryData ScriptableObject
- Create Infantry prefab with placeholder sprite
- Update UNITS.md with Infantry implementation
```

---

### Commit 9 : Système de sélection et déplacement

**Objectif:** Sélectionner et déplacer les unités

**À créer dans `Core/Scripts/`:**

1. **SelectionManager.cs**
   - Singleton pour gérer la sélection
   - Détecte clic gauche sur unité → sélectionne
   - Détecte clic droit sur grille → déplace unité sélectionnée
   - Utilise Raycast pour détection

2. **InputManager.cs** (optionnel)
   - Centralise la gestion des inputs
   - Utilise le New Input System
   - Délègue aux managers appropriés

**À créer dans `Units/Infantry/Scripts/`:**

3. **InfantryVisual.cs**
   - Feedback visuel de sélection (highlight, cercle)
   - Change la couleur/scale quand sélectionné

**Tests:**
- Lancer Play mode
- Clic gauche sur Infantry → doit être sélectionné (feedback visuel)
- Clic droit sur une cellule → Infantry se déplace
- Vérifier déplacement fluide

**Commit message:**
```
feat: add unit selection and movement system

- Add SelectionManager for unit selection (left click)
- Add movement commands (right click on grid)
- Add visual feedback for selected units
- Implement IMovable and ISelectable on Infantry
- Units now movable via mouse input
```

---

### Commit 10 : Deuxième unité - TankHeavy

**Objectif:** Ajouter une deuxième unité pour valider l'architecture

**À créer dans `Units/TankHeavy/Scripts/`:**

1. **TankHeavyController.cs**
   - Similaire à InfantryController
   - Namespace: `CommandAndConquer.Units.TankHeavy`

2. **TankHeavyMovement.cs**
   - Déplacement plus lent que Infantry

3. **TankHeavyData.cs**
   - moveSpeed = 2f (plus lent qu'Infantry)

**À créer dans `Units/TankHeavy/Prefabs/`:**
- `TankHeavy.prefab`

**À créer dans `Units/TankHeavy/Sprites/`:**
- Placeholder différent d'Infantry (cube ou autre forme)

**À créer dans `Units/TankHeavy/Data/`:**
- `TankHeavyData.asset`

**Tests:**
- Placer TankHeavy dans la scène à côté d'Infantry
- Sélectionner et déplacer les deux types d'unités
- Vérifier vitesses différentes

**Commit message:**
```
feat: add TankHeavy unit

- Add TankHeavyController, TankHeavyMovement, TankHeavyData
- Create TankHeavy prefab with slower movement speed
- Validate architecture with second unit type
- Update UNITS.md with TankHeavy implementation
```

---

## 🎯 Version 1.0 - Objectifs atteints

Après le Commit 10, vous aurez :
- ✅ Carte avec grille visible et éditable
- ✅ Caméra déplaçable (WASD/souris + zoom)
- ✅ Deux types d'unités (Infantry, TankHeavy)
- ✅ Sélection d'unités (clic gauche)
- ✅ Déplacement sur grille (clic droit)

**= PROTOTYPE JOUABLE** 🎮

---

## 🔮 Phase 3 : Fonctionnalités avancées (Futur)

### Idées pour versions suivantes

**Système de terrain (Map)**
- TileManager pour différents types de terrain
- Terrain affecte vitesse de déplacement
- Obstacles non-traversables

**Système UI**
- Barre de sélection en bas
- Minimap
- Icônes d'unités
- Menu pause

**Système de construction**
- Bâtiments de base
- Production d'unités
- Ressources

**Système de combat**
- Attaque entre unités
- Points de vie
- Portée d'attaque
- Animation d'attaque

**IA basique**
- Unités ennemies contrôlées par IA
- Pathfinding avec A*
- Comportements basiques (patrol, attack)

**Audio**
- Sons de sélection
- Sons de déplacement
- Musique de fond

---

## 📐 Architecture technique

### Dépendances entre modules

```
Core (base)
 ├─> Camera (utilise Core pour configuration)
 ├─> Grid (utilise Core.GridPosition)
 ├─> Units (utilise Core.UnitBase, IMovable, ISelectable)
 │    └─> Infantry (utilise Grid pour déplacement)
 │    └─> TankHeavy (utilise Grid pour déplacement)
 ├─> Map (utilise Grid)
 └─> UI (utilise Core, Units pour affichage)
```

### Flux de sélection/déplacement

```
1. Utilisateur clique → InputManager/SelectionManager
2. Raycast détecte unité → SelectionManager.SelectUnit()
3. Unité.OnSelected() → Feedback visuel
4. Utilisateur clic droit → SelectionManager détecte grille
5. SelectionManager → Unit.MoveTo(GridPosition)
6. UnitMovement calcule chemin et déplace l'unité
7. Arrivé → Callback OnReachedDestination()
```

### Patterns utilisés

- **Singleton**: SelectionManager, GridManager
- **Component Pattern**: Séparation Controller/Movement/Visual
- **Strategy Pattern**: IMovable, ISelectable
- **ScriptableObject Pattern**: UnitData pour configuration

---

## 📝 Utilisation de ce document

### Pour reprendre le développement

1. **Lire l'état actuel** pour savoir ce qui est fait
2. **Consulter le prochain commit** dans le plan détaillé
3. **Suivre les instructions** de création de fichiers
4. **Tester** selon les critères définis
5. **Commiter** avec le message proposé
6. **Mettre à jour CHANGELOG.md**

### Commandes utiles

- `/create-unit` - Créer une nouvelle unité
- `/test-game` - Lancer la scène de jeu
- Voir `README.md` pour plus d'infos

### Fichiers de référence

- `README.md` - Vue d'ensemble
- `CONVENTIONS.md` - Standards de code
- `UNITS.md` - Catalogue des unités
- `CHANGELOG.md` - Historique des changements
- `ROADMAP.md` - Ce document (plan complet)

---

**Dernière mise à jour:** Commit 6 - Système de caméra RTS
**Prochaine étape:** Commit 7 - Système de grille