# Documentation Technique

Documentation détaillée des systèmes et outils du projet Command and Conquer RTS.

---

## 📚 Fichiers disponibles

### [UNITS.md](UNITS.md)
Catalogue complet des unités du jeu.

**Contenu** :
- Liste des unités disponibles (Buggy, Artillery)
- Structure des dossiers et fichiers pour chaque unité
- Composants Common réutilisables (Vehicle, Selection, Animation)
- Guide pour ajouter une nouvelle unité

---

### [TOOLS.md](TOOLS.md)
Documentation des outils Unity Editor.

**Contenu** :
- Sprite Importers (TerrainSpriteImporter, UnitSpriteImporter)
- Configuration automatique des sprites (PPU=128, Point filter)
- Menus Unity disponibles
- Guide de création d'outils personnalisés
- Dépannage

---

### [ANIMATION.md](ANIMATION.md)
Guide de configuration du système d'animation 8 directions.

**Contenu** :
- Vue d'ensemble du système (DirectionType, VehicleAnimationData, VehicleAnimator)
- Étapes de setup pour nouveaux véhicules
- Mapping des sprites par direction (E, NE, N, NW, W, SW, S, SE)
- Tests et debug
- Tableau de référence des angles

---

## 🔙 Navigation

- [← Retour à la racine](../) - README.md principal
- [Guide développeur](../GUIDE.md) - Architecture, systèmes, workflows
- [Documentation Claude](../CLAUDE.md) - Référence technique pour Claude Code
- [Changelog](../CHANGELOG.md) - Historique des modifications

---

**Structure de la documentation** :

```
Racine/
├── README.md              # Vue d'ensemble du projet
├── GUIDE.md               # Guide développeur complet
├── CLAUDE.md              # Documentation pour Claude Code
├── CHANGELOG.md           # Historique
│
└── docs/                  # Documentation technique (vous êtes ici)
    ├── README.md          # Ce fichier (index)
    ├── UNITS.md           # Catalogue unités
    ├── TOOLS.md           # Outils Editor
    └── ANIMATION.md       # Système animation 8 dir
```
