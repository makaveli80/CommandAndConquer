# Changelog - Command and Conquer RTS

Historique des modifications du projet.

Le format est basé sur [Keep a Changelog](https://keepachangelog.com/fr/1.0.0/).

## [Non publié]

### Phase 1 : Préparation du projet (Commits 1-5)

### Phase 2 : Systèmes de base (Commits 6-10+) ✅ TERMINÉE

## Building System - Production (Phase 2) (2025-11-26) ✅

### Ajouté
- **ProductionItem.cs** - ScriptableObject définissant les items produisibles
  - Propriétés: nom, icône, description, temps de production, prefab
  - Validation automatique dans OnValidate()
  - Flag isBuilding pour différencier unités/bâtiments
- **ProductionQueue.cs** - Composant de gestion de file d'attente
  - Queue FIFO avec timer progressif (0.0 → 1.0)
  - Events: OnItemStarted, OnProgressUpdated, OnItemCompleted
  - API: AddToQueue(), CancelCurrent(), ClearQueue()
  - UI debug en coin supérieur gauche (production + queue count)
- **ProductionQueueTester.cs** - Script de test temporaire (Phase 2)
  - Contrôles clavier: 1=Buggy, 2=Artillery, C=Cancel, Q=Clear
  - Références assignables aux ProductionItems
- **ProductionItemCreator.cs** - Utilitaire éditeur pour création rapide
  - Menu: Assets > Create > Command & Conquer > Production Item (Quick)
- **ProductionItemSetup.cs** - Setup automatique des assets
  - Menu: Tools > Command & Conquer > Setup Production Items
  - Génère BuggyProductionItem.asset (8s) et ArtilleryProductionItem.asset (15s)

### Modifié
- **Building.cs** - Intégration complète du système de production
  - Auto-découverte du composant ProductionQueue dans Awake()
  - Méthode SetupProductionQueue() pour connexion events
  - API publique: AddToProductionQueue(), ProductionQueue property
  - Event OnProductionCompleted pour Phase 3
  - Logs détaillés de progression
- **BuildingData.cs** - Ajout du support de production
  - Nouveau champ: ProductionItem[] canProduce
  - Organisation par sections (Production Phase 2, Spawn Phase 3)

### Architecture
- **Event-driven pattern** : ProductionQueue découplé de Building via events
- **Timer normalisé** : Progress 0.0-1.0 indépendant du temps réel
- **Validation robuste** : Null checks + logs clairs à chaque étape

### Tests Validés
- ✅ Timer progresse correctement (0% → 100%)
- ✅ Queue FIFO avec multiple items
- ✅ Events déclenchés au bon moment
- ✅ Annulation et vidage de queue fonctionnels
- ✅ Logs détaillés pour debugging

## Multi-selection avec drag box (2025-11-22)

### Ajouté
- **DragBoxVisual.cs** - Composant UI pour affichage du rectangle de sélection
  - Rectangle vert transparent avec bordure
  - Mise à jour en temps réel pendant le drag
  - Seuil de 5px pour éviter les drags accidentels
- **SelectionManager** amélioré pour multi-sélection
  - `HashSet<ISelectable>` pour tracking O(1) sans duplicates
  - Détection de drag avec threshold
  - `Physics2D.OverlapAreaAll()` pour sélection par zone
  - Support simultané simple-clic et drag-box
- Tous les corner brackets s'affichent indépendamment sur chaque unité sélectionnée

### Modifié
- SelectionManager passe de single-selection à multi-selection
- Commandes de mouvement appliquées à toutes les unités sélectionnées

## Système d'animation 8 directions (Nov 2025)

### Ajouté
- **DirectionType.cs** - Enum pour 8 directions cardinales/intercardinales
- **DirectionUtils.cs** - Utilitaire de calcul de direction
  - Conversion vecteur → direction via Atan2
  - Mapping angles → 8 directions (plages de 45°)
- **VehicleAnimationData.cs** - ScriptableObject stockage sprites
  - 8 sprites (E, NE, N, NW, W, SW, S, SE)
  - Validation sprites assignés
  - Direction par défaut configurable
- **VehicleAnimator.cs** - Composant d'animation passif
  - Polling de VehicleMovement.IsMoving
  - Calcul direction depuis vecteur mouvement
  - Mise à jour sprite uniquement si direction change
  - Mémorisation dernière direction (idle)
  - Mode debug avec gizmo flèche jaune
- Assets BuggyAnimationData et ArtilleryAnimationData configurés

### Design
- Pattern passive polling (pas d'events, Update() monitoring)
- Performance optimisée (sprite update seulement si changement)
- Réutilisable pour tous véhicules

## Système de curseur animé (Jan 2025)

### Ajouté
- **CursorManager.cs** - Singleton gestionnaire curseurs
  - Support curseurs animés (multiple frames + FPS configurable)
  - API publique `SetCursor()` / `ResetCursor()`
  - Animation frame-based dans Update()
  - 3 types: Default, Hover (6 frames), Move (4 frames)
- **CursorType.cs** - Enum types de curseurs
- **CursorSpriteImporter.cs** - Auto-configuration sprites curseur
  - isReadable = true (requis pour Cursor.SetCursor)
  - FilterMode = Point, Compression = None
  - Menu Tools: "Reconfigure Cursor Sprites"
- SelectionManager amélioré avec détection hover
  - `HandleUnitHover()` - Détection unités sous curseur
  - `HandleDestinationHover()` - Détection destinations valides
  - Système de priorité: Hover > Move > Default

### Configuration
- Hover cursor: 6 frames @ 10 FPS (ICON_SELECT_FRIENDLY_00-05)
- Move cursor: 4 frames @ 10 FPS (ICON_MOVEMENT_COMMAND_00-03)
- Hotspot configurable (ex: 24,24 pour curseur 48x48)

## Système de sélection visuelle corner brackets (Nov 2025)

### Ajouté
- **SelectableComponent.cs** - Coordinateur sélection
  - Subscribe aux events UnitBase.OnSelected/OnDeselected
  - Support multiple types visuels (enum SelectionVisualType)
  - Contrôle CornerBracketSelector
- **CornerBracketSelector.cs** - Affichage brackets passif
  - 4 child GameObjects avec sprites L rotatés
  - Méthodes publiques ShowBrackets() / HideBrackets()
  - Configurable: distance, taille, sorting order
- **SelectionVisualType.cs** - Enum types visuels
  - SpriteColor (legacy)
  - CornerBrackets (défaut)
- Sprite corner_bracket_l.png (forme L blanche)
- Rotations corners: TL=0°, TR=-90°, BR=180°, BL=90°

### Design Pattern
- Séparation: SelectableComponent = logique, CornerBracketSelector = affichage
- Passive component (contrôlé, ne subscribe pas aux events)
- Réutilisable sur tous types d'unités

## Commit 10 - Unité Artillery (Nov 2025)

### Ajouté
- **ArtilleryController.cs** - Contrôleur unité lourde
- **ArtilleryMovement.cs** - Déplacement lent (speed: 1.5)
- **ArtilleryContext.cs** - État partagé Artillery
- **ArtilleryData.cs** + asset - Configuration ScriptableObject
- **ArtilleryAnimationData.asset** - 8 directions configurées
- Prefab Artillery.prefab avec tous composants
- Sprites artillery avec 16 frames animation

### Validé
- Architecture réutilisable validée avec 2e unité
- Vitesses différentes (Buggy: 4.0, Artillery: 1.5)
- Systèmes Common (Vehicle, Selection, Animation) fonctionnels

## Commit 9 - Système de sélection souris (Nov 2025)

### Ajouté
- **SelectionManager.cs** - Module Gameplay
  - Sélection clic gauche via raycast 2D
  - Commande mouvement clic droit
  - Conversion souris → monde → grille automatique
  - Validation position cible
- **CommandAndConquer.Gameplay.asmdef** - Nouveau module
  - Évite dépendances circulaires Core ↔ Grid
  - Couche orchestration au-dessus de Grid
- Feedback visuel sélection dans BuggyController
  - OnSelected(): sprite teinte verte
  - OnDeselected(): couleur originale

### Architecture
- Pattern: Core (base) → Grid (système) → Gameplay (orchestration)
- Physics2D.GetRayIntersection() pour détection unités

## Commit 8 - Unité Buggy (Nov 2025)

### Ajouté
- **BuggyController.cs** - Premier contrôleur unité
  - Hérite UnitBase
  - Implémente IMovable, ISelectable
  - FindFirstObjectByType<GridManager>() (Unity 6 API)
- **BuggyMovement.cs** - Système déplacement case par case
  - State machine: Idle/Moving/WaitingForNextCell/Blocked
  - Pathfinding 8 directions (ligne droite)
  - Interpolation fluide Vector3.MoveTowards()
  - Système destination en attente (pendingTarget)
  - Retry mechanism: 0.3s interval, max 20 tentatives
- **BuggyContext.cs** - État partagé pour composition
- **BuggyData.cs** + asset - Configuration (speed: 4.0)
- **BuggyTestMovement.cs** - Tests pavé numérique
  - Touches 1-9: positions test
  - Touche 0: retour home (5,5)
  - Touche H: aide
- **BuggyMovementDebug.cs** - Visualisation Gizmos
  - Blanc/Vert/Orange/Rouge: états Idle/Moving/Waiting/Blocked
  - Gray/Yellow/Cyan cubes: cellules path
  - Yellow line: chemin actuel
  - Magenta sphere: destination finale
- Prefab Buggy.prefab avec BoxCollider2D (trigger)
- 16 sprites buggy-0000 à buggy-0030 (PPU=128)

### Problèmes résolus
- Cellules fantômes: système pendingTarget + CancelCurrentMovement()
- Input System errors: migration vers Keyboard.current
- Namespace Grid: ajout référence assembly Units.asmdef

## Commit 7 - Système de grille (Nov 2025)

### Ajouté
- **GridManager.cs** - Gestionnaire grille 20x20
  - Génération automatique au démarrage
  - Cell size: 1.0 unité Unity
  - Conversion Grid ↔ World (+0.5f offset pour centrage)
  - Système enregistrement unités: RegisterUnit/UnregisterUnit
  - Réservation atomique: TryMoveUnitTo() (prévient race conditions)
  - Vérification cohérence LateUpdate() toutes les 60 frames
  - Debug Gizmos: vert (libre), rouge (occupé)
- **GridCell.cs** - État cellule individuelle
  - Propriétés: position, occupied, unit reference
  - Méthodes: TryOccupy(), Release(), ForceOccupy()
- **GridPathfinder.cs** - Utilitaire pathfinding statique
  - CalculateStraightPath() pour 8 directions
  - Math.Sign() pour calcul deltaX/deltaY
  - Limite sécurité: 1000 itérations max
- **TerrainSpriteImporter.cs** - Configuration auto sprites
  - PPU=128, FilterMode=Point, Compression=None
  - Menu Tools: "Reconfigure All Terrain Sprites"
- **Map/TILEMAP_SETUP.md** - Guide configuration Tilemap (10 étapes)
- **Map/RANDOM_BRUSH_GUIDE.md** - Guide peinture variation aléatoire

### Modifié
- GridPosition.cs adapté pour 2D (x,y au lieu de x,z)
- Grid.asmdef référence Core

### Configuration
- Grid: 20x20 cellules, 1.0 unité/cellule
- Sprites: PPU=128 (128px = 1.0 unité)
- Tile Anchor: (0.5, 0.5) pour centrage visuel

## Commit 6 - Système de caméra RTS

### Ajouté
- Système de caméra RTS complet dans `Assets/_Project/Camera/`
- **CameraController.cs** - Contrôleur principal de la caméra
  - Déplacement au clavier (WASD / Flèches)
  - Edge scrolling (souris aux bords de l'écran)
  - Zoom avec molette de souris
  - Limites de déplacement configurables
  - Support complet du New Input System
- **CameraBounds.cs** - ScriptableObject pour définir les limites
  - Boundaries de mouvement (minX, maxX, minY, maxY)
  - Boundaries de zoom (minZoom, maxZoom)
- **CameraInputActions.inputactions** - Configuration New Input System
  - Action Move (Vector2) : WASD + Flèches
  - Action Zoom (Axis) : Molette souris
  - Action MousePosition (Vector2) : Position souris
  - Génération automatique de classe C#
- **DefaultCameraBounds.asset** - Asset de configuration par défaut
- **Camera/README.md** - Documentation complète du système
  - Guide d'installation
  - Configuration détaillée
  - API pour développeurs
  - Exemples d'utilisation
  - Troubleshooting

### Modifié
- Architecture vue 2D pure (axes X, Y) pour compatibilité RTS

### Corrigé
- GUIDs invalides dans CameraInputActions.inputactions causant un chargement infini dans l'éditeur

## Commit 5 - Configuration développement

### Ajouté
- Dossier `Assets/_Project/Scenes/` pour les scènes du jeu
- Dossier `Assets/_Project/Audio/` pour les sons et musiques
- Dossier `Assets/_Project/UI/` pour l'interface utilisateur
  - `UI/Scripts/` pour les scripts d'UI
  - `UI/Prefabs/` pour les prefabs d'UI
  - `UI/Sprites/` pour les sprites d'UI
- Fichier `CHANGELOG.md` pour suivre l'évolution du projet

### À faire
- Créer la scène `Game.unity` dans Unity Editor
- Configurer la scène avec Main Camera et EventSystem

## Commit 4 - Structure namespace

### Ajouté
- Assembly Definition `CommandAndConquer.Core.asmdef`
- Assembly Definition `CommandAndConquer.Units.asmdef`
- Assembly Definition `CommandAndConquer.Camera.asmdef`
- Assembly Definition `CommandAndConquer.Grid.asmdef`
- Assembly Definition `CommandAndConquer.Map.asmdef`
- Création des dossiers de fonctionnalités: Camera/, Grid/, Map/, Units/

### Modifié
- Organisation modulaire du code pour compilation rapide
- Dépendances explicites entre modules

## Commit 3 - Templates de code

### Ajouté
- Classe abstraite `UnitBase.cs` dans Core/Scripts
- Interface `IMovable.cs` pour les unités déplaçables
- Interface `ISelectable.cs` pour les objets sélectionnables
- Structure `GridPosition.cs` pour les positions sur grille
- ScriptableObject `UnitData.cs` pour les données d'unités
- Fichier `.editorconfig` pour les standards de formatage C#

## Commit 2 - Intégration Claude Code

### Ajouté
- Commande `/create-unit` pour générer des templates d'unités
- Commande `/test-game` pour lancer la scène de jeu
- Fichier `.claude/preferences.json` avec configuration Unity
  - Conventions de nommage
  - Chemins des assets
  - Paramètres de génération de code

## Commit 1 - Documentation projet

### Ajouté
- Fichier `README.md` avec vue d'ensemble du projet
  - Description et objectifs
  - Structure par fonctionnalité
  - Conventions de nommage (C#, Unity, Git)
- Fichier `CONVENTIONS.md` avec standards de développement
  - Conventions C# détaillées
  - Conventions Unity Assets
  - Organisation des fichiers
  - Bonnes pratiques
- Fichier `UNITS.md` catalogue des types d'unités
  - Liste des unités disponibles (Infantry, TankHeavy)
  - Structure et fichiers de chaque unité
- Fichier `Assets/_Project/README.md` pour organisation des assets
  - Principe d'organisation par fonctionnalité
  - Description des dossiers
  - Règles de développement

---

## Format des entrées

- **Ajouté** : nouvelles fonctionnalités
- **Modifié** : changements dans des fonctionnalités existantes
- **Déprécié** : fonctionnalités bientôt supprimées
- **Supprimé** : fonctionnalités supprimées
- **Corrigé** : corrections de bugs
- **Sécurité** : corrections de vulnérabilités
