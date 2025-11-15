# Changelog - Command and Conquer RTS

Historique des modifications du projet.

Le format est basé sur [Keep a Changelog](https://keepachangelog.com/fr/1.0.0/).

## [Non publié]

### Phase 1 : Préparation du projet (Commits 1-5)

### Phase 2 : Systèmes de base (Commits 6+)

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