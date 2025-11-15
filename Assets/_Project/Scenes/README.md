# Scènes du jeu

Ce dossier contient toutes les scènes du jeu Command and Conquer.

## Scènes à créer

### Game.unity - Scène principale du jeu

Cette scène doit être créée manuellement dans Unity Editor.

#### Instructions de création

1. **Ouvrir Unity Editor**
   - Ouvrir le projet Command and Conquer

2. **Créer une nouvelle scène**
   - File > New Scene
   - Sélectionner "Basic (URP)" ou "2D (URP)"
   - Cliquer sur "Create"

3. **Configurer la scène**

   **Main Camera**:
   - Devrait déjà être présente
   - Position: (0, 10, 0) pour vue de dessus
   - Rotation: (90, 0, 0) pour regarder vers le bas
   - Projection: Orthographic
   - Size: 10
   - Tag: MainCamera

   **EventSystem** (pour l'Input System):
   - GameObject > UI > Event System
   - Composant: EventSystem
   - Composant: Standalone Input Module (ou Input System UI Input Module)

4. **Sauvegarder la scène**
   - File > Save As...
   - Localisation: `Assets/_Project/Scenes/`
   - Nom: `Game.unity`
   - Cliquer sur "Save"

5. **Ajouter à Build Settings** (optionnel)
   - File > Build Settings...
   - Cliquer sur "Add Open Scenes"
   - Game devrait apparaître dans la liste des scènes

#### Configuration future

Quand les systèmes seront implémentés, ajouter:
- CameraController sur Main Camera (Commit 6+)
- GridManager GameObject (Commit 6+)
- InputManager GameObject (Commit 6+)

## Scènes actuelles

- **SampleScene.unity** (temporaire)
  - Scène de base Unity
  - Utilisée pour les tests initiaux
  - Sera remplacée par Game.unity

## Organisation

- **Game.unity**: Scène principale du jeu
- **MainMenu.unity**: Menu principal (à créer plus tard)
- **TestXXX.unity**: Scènes de test pour développement