# Command and Conquer - RTS 2D

Projet éducatif de recréation d'un RTS classique en 2D avec Unity 6 et assistance IA (Claude Code).

## 🎮 Fonctionnalités actuelles

**Version 1.0** - Prototype jouable complet ✅

- ✅ **Grille logique 20×20** - Système de pathfinding 8 directions
- ✅ **Caméra RTS** - WASD/Edge scrolling + zoom molette
- ✅ **2 unités jouables** - Buggy (rapide) et Artillery (lent)
- ✅ **Animations 8 directions** - Sprites directionnels pour véhicules
- ✅ **Sélection multi-unités** - Clic simple ou drag-box
- ✅ **Feedback visuel** - Corner brackets + curseurs animés
- ✅ **Gestion collision** - Système atomique de réservation de cellules

## 🛠️ Technologies

- **Moteur** : Unity 6
- **Pipeline** : Universal Render Pipeline (URP) 2D
- **Langage** : C# (.NET Standard 2.1)
- **Input** : New Input System
- **Packages** : 2D Tilemap, 2D Animation, Aseprite Importer

## 🚀 Quick Start

1. **Cloner le projet**
   ```bash
   git clone <repository-url>
   ```

2. **Ouvrir avec Unity 6**
   - File → Open Project
   - Sélectionner le dossier du projet

3. **Lancer la scène de jeu**
   - Ouvrir `Assets/_Project/Scenes/Game.unity`
   - Appuyer sur Play ▶️

4. **Contrôles**
   - **Caméra** : WASD/Flèches + souris (bords d'écran) + molette (zoom)
   - **Sélection** : Clic gauche (simple) ou drag (multi-sélection)
   - **Mouvement** : Clic droit sur la grille

## 📁 Structure du projet

```
Assets/_Project/
├── Core/           # Interfaces et types partagés
├── Grid/           # Système de grille logique
├── Camera/         # Contrôleur caméra RTS
├── Gameplay/       # Sélection, curseurs, input
├── Units/          # Unités (Buggy, Artillery) + composants communs
└── Map/            # Terrain et tilemap
```

**Architecture** : Composition pure (zero héritage). Nouvelles unités créées 100% dans l'éditeur Unity.

## 📚 Documentation

| Fichier | Description |
|---------|-------------|
| **[GUIDE.md](GUIDE.md)** | Guide développeur complet (architecture, workflows) |
| **[CLAUDE.md](CLAUDE.md)** | Documentation technique pour Claude Code |
| **[CHANGELOG.md](CHANGELOG.md)** | Historique des modifications |
| **[docs/](docs/)** | Documentation technique détaillée |

### Documentation technique

- **[docs/UNITS.md](docs/UNITS.md)** - Catalogue des unités
- **[docs/TOOLS.md](docs/TOOLS.md)** - Outils Editor Unity
- **[docs/ANIMATION.md](docs/ANIMATION.md)** - Système d'animation 8 directions

## 🔧 Commandes Claude Code

- `/create-unit` - Créer une nouvelle unité avec le template
- `/test-game` - Lancer la scène de jeu
- `/gen-commit` - Générer un message de commit

## 🎯 Prochaines étapes

Le projet est **prêt pour expansion**. Options possibles :

- **Option A** : Ajouter 3ème unité (Tank/Harvester/MCV)
- **Option B** : Système de formations multi-unités
- **Option C** : Combat (attaque, santé, dégâts, mort)
- **Option D** : Bâtiments (construction, production)
- **Option E** : IA basique (pathfinding, comportements)

## 📝 Conventions

- **C#** : PascalCase (classes), camelCase (variables privées), UPPER_CASE (constantes)
- **Assets** : PascalCase (prefabs/scènes), snake_case (sprites)
- **Commits** : `type: message` (feat/fix/refactor/docs/chore)

Voir [GUIDE.md](GUIDE.md) pour les conventions détaillées.

## 📄 Licence

Projet éducatif - Usage personnel
