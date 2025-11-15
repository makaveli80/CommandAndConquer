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
├── Core/               ✅ Templates de base
├── Camera/             📁 Vide (à implémenter)
├── Grid/               📁 Vide (à implémenter)
├── Units/              📁 Vide (à implémenter)
├── Map/                📁 Vide (à implémenter)
├── UI/                 📁 Structure créée
├── Audio/              📁 Structure créée
└── Scenes/             📁 Structure créée
```

---

## 🎯 Phase 2 : Systèmes de base (PROCHAIN)

### Objectif Version 1
Créer un prototype jouable avec les fonctionnalités de base :
- Caméra déplaçable (WASD/souris)
- Grille visible
- Une unité (Infantry) plaçable et déplaçable

---

## 📋 Plan détaillé des prochains commits

### Commit 6 : Système de caméra RTS

**Objectif:** Caméra contrôlable avec clavier/souris

**À créer dans `Camera/Scripts/`:**

1. **CameraController.cs**
   - Namespace: `CommandAndConquer.Camera`
   - Déplacement WASD ou flèches
   - Déplacement par bords d'écran (souris)
   - Zoom molette de souris
   - Limites de déplacement configurables

2. **CameraBounds.cs**
   - Définit les limites de mouvement de la caméra
   - ScriptableObject pour configuration

**À créer dans `Camera/Prefabs/`:**
- `MainCamera.prefab` avec CameraController attaché

**Tests:**
- Ouvrir Game.unity (ou SampleScene)
- Ajouter CameraController à Main Camera
- Tester déplacement WASD/flèches
- Tester déplacement souris aux bords
- Tester zoom molette

**Commit message:**
```
feat: add RTS camera controller

- Add CameraController with WASD/arrow keys movement
- Add edge scrolling with mouse
- Add mouse wheel zoom
- Add configurable camera bounds
- Create MainCamera prefab
```

---

### Commit 7 : Système de grille

**Objectif:** Grille visible pour déplacement des unités

**À créer dans `Grid/Scripts/`:**

1. **GridManager.cs**
   - Namespace: `CommandAndConquer.Grid`
   - Génère la grille au démarrage
   - Dimensions configurables (width, height)
   - Taille des cellules configurable (cellSize)
   - Méthodes: `GetGridPosition(Vector3 worldPos)`, `GetWorldPosition(GridPosition gridPos)`

2. **GridCell.cs**
   - Représente une cellule de la grille
   - Propriétés: position, walkable, occupied
   - Référence à l'unité présente (si occupée)

3. **GridVisualizer.cs**
   - Affiche la grille visuellement
   - Utilise Debug.DrawLine ou LineRenderer
   - Toggle pour activer/désactiver l'affichage

**À créer dans `Grid/Prefabs/`:**
- `GridManager.prefab` - GameObject avec GridManager

**À créer dans `Grid/Materials/`:**
- `GridLineMat.mat` - Matériau pour les lignes de grille

**Configuration recommandée:**
- Grid size: 20x20
- Cell size: 1.0f
- Couleur grille: Blanc semi-transparent

**Tests:**
- Ajouter GridManager à la scène
- Configurer dimensions (20x20)
- Lancer Play mode
- Vérifier affichage de la grille

**Commit message:**
```
feat: add grid system for unit movement

- Add GridManager to generate and manage grid
- Add GridCell to represent grid positions
- Add GridVisualizer for visual feedback
- Create GridManager prefab with default 20x20 grid
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

**Dernière mise à jour:** Commit 5 - Configuration développement
**Prochaine étape:** Commit 6 - Système de caméra RTS