# Guide de Configuration du Tilemap

Ce guide détaille toutes les étapes pour configurer le système de Tilemap avec le terrain Clear1 et le GridManager.

## Prérequis

- ✅ Sprites Clear1 importés dans `Assets/_Project/Map/Sprites/Terrain/Clear1/` (16 sprites)
- ✅ Scripts Grid créés (GridManager.cs, GridCell.cs)
- ✅ Script Editor TerrainSpriteImporter.cs créé

---

## Étape 1 : Configurer les Sprites avec PPU=128

### 1.1 Exécuter le script de configuration

**Menu Unity :**
```
Tools → Command & Conquer → Reconfigure All Terrain Sprites
```

**Résultat attendu :**
- Une fenêtre de dialogue s'affiche : "Successfully reconfigured 16 terrain sprites with PPU=128"
- Dans la Console : `[TerrainSpriteImporter] Reconfigured 16 terrain sprites with PPU=128`

### 1.2 Vérifier la configuration (optionnel)

Sélectionnez un sprite Clear1 dans le Project (par exemple `CLEAR1.TEM-0000.png`) :

**Inspector → Texture Import Settings :**
```
Texture Type: Sprite (2D and UI)
Sprite Mode: Single
Pixels Per Unit: 128                    ← Doit être 128 !
Filter Mode: Point (no filter)
Compression: None
```

Si les valeurs sont différentes, cliquez sur **"Revert"** puis ré-exécutez le menu Tools.

---

## Étape 2 : Créer la Hiérarchie Grid + Tilemap

### 2.1 Ouvrir la scène Game

**Project :**
```
Assets/_Project/Scenes/Game.unity
```
Double-cliquez pour ouvrir.

### 2.2 Créer le Grid avec Tilemap

**Hierarchy → Clic droit :**
```
2D Object → Tilemap → Rectangular
```

**Hiérarchie créée automatiquement :**
```
Scene: Game
├── Main Camera
├── GridManager (à créer plus tard)
└── Grid                    ← Nouvellement créé
    └── Tilemap            ← Nouvellement créé
```

---

## Étape 3 : Configurer le Grid

### 3.1 Sélectionner le GameObject "Grid"

Cliquez sur **Grid** dans la Hierarchy.

### 3.2 Configurer le composant Grid

**Inspector → Grid Component :**

```
Cell Size:
  X: 1        ← IMPORTANT : 1.0 unité Unity
  Y: 1        ← IMPORTANT : 1.0 unité Unity
  Z: 0

Cell Gap:
  X: 0
  Y: 0
  Z: 0

Cell Layout: Rectangle
Cell Swizzle: XYZ
```

**Pourquoi Cell Size = 1 ?**
- Nos sprites font 128x128 pixels
- Avec PPU=128, chaque sprite = 1.0 unité Unity
- Donc chaque cellule = 1.0 x 1.0 unité Unity
- Cela simplifie tous les calculs (pas de décimales !)

### 3.3 Position du Grid

**Inspector → Transform :**
```
Position:
  X: 0
  Y: 0
  Z: 0

Rotation: (0, 0, 0)
Scale: (1, 1, 1)
```

---

## Étape 4 : Configurer le Tilemap

### 4.1 Sélectionner le GameObject "Tilemap"

Cliquez sur **Tilemap** dans la Hierarchy (enfant de Grid).

### 4.2 Renommer (optionnel mais recommandé)

Renommez en **"Terrain"** pour plus de clarté :
```
Grid
└── Terrain    ← Ancien nom : Tilemap
```

### 4.3 Vérifier le composant Tilemap

**Inspector → Tilemap Component :**
```
Animation Frame Rate: 1
Tile Anchor: (0.5, 0.5, 0)    ← Centre de la tile
Orientation: XY
```

### 4.4 Configurer le Tilemap Renderer

**Inspector → Tilemap Renderer Component :**
```
Material: Default-Material (Built-in Sprite)
Sorting Layer: Default
Order in Layer: 0
Mode: Chunk
Detect Chunk Culling Bounds: Auto
Mask Interaction: None
```

**Important :** Assurez-vous que **Sorting Layer** est en dessous de la caméra et des unités futures.

---

## Étape 5 : Créer la Tile Palette

### 5.1 Ouvrir la fenêtre Tile Palette

**Menu Unity :**
```
Window → 2D → Tile Palette
```

Une fenêtre **Tile Palette** s'ouvre (généralement dockée en bas ou à droite).

### 5.2 Créer une nouvelle palette

**Dans la fenêtre Tile Palette :**

1. Cliquez sur le menu déroulant en haut (écrit "No Palette")
2. Sélectionnez **"Create New Palette"**

**Configuration de la palette :**
```
Name: Clear1_Terrain
Grid: Rectangular
Cell Sizing: Automatic
```

Cliquez sur **"Create"**.

### 5.3 Choisir l'emplacement de sauvegarde

**Fenêtre de sauvegarde :**

Créez/sélectionnez le dossier :
```
Assets/_Project/Map/Palettes/
```

**Si le dossier n'existe pas :**
1. Naviguez vers `Assets/_Project/Map/`
2. Clic droit → Create → Folder → nommez "Palettes"
3. Entrez dans le dossier "Palettes"
4. Cliquez sur **"Select Folder"**

**Résultat :**
- Fichier créé : `Assets/_Project/Map/Palettes/Clear1_Terrain.prefab`
- La palette "Clear1_Terrain" est maintenant active

---

## Étape 6 : Créer les Tiles à partir des Sprites

### 6.1 Préparer le dossier Tiles

**Project :**

Créez le dossier de destination :
```
Assets/_Project/Map/Tiles/Clear1/
```

1. Naviguez vers `Assets/_Project/Map/`
2. Clic droit → Create → Folder → "Tiles"
3. Entrez dans "Tiles"
4. Clic droit → Create → Folder → "Clear1"

### 6.2 Glisser-déposer les sprites dans la palette

**Project :**

1. Naviguez vers `Assets/_Project/Map/Sprites/Terrain/Clear1/`
2. **Sélectionnez TOUS les 16 sprites** :
   - Cliquez sur le premier (`CLEAR1.TEM-0000.png`)
   - Maintenez **Shift** et cliquez sur le dernier (`CLEAR1.TEM-0015.png`)
   - Tous les 16 sprites sont sélectionnés

**Tile Palette :**

3. **Glissez-déposez** les 16 sprites sélectionnés **dans la fenêtre Tile Palette**

**Fenêtre de sauvegarde des Tiles :**

4. Une fenêtre s'ouvre : "Generate Tile Assets"
5. Naviguez vers `Assets/_Project/Map/Tiles/Clear1/`
6. Cliquez sur **"Select Folder"**

**Résultat :**
- 16 fichiers Tile créés dans `Map/Tiles/Clear1/` :
  ```
  CLEAR1.TEM-0000.asset
  CLEAR1.TEM-0001.asset
  ...
  CLEAR1.TEM-0015.asset
  ```
- Les 16 tiles apparaissent dans la Tile Palette

### 6.3 Organiser la palette (optionnel)

Vous pouvez réorganiser les tiles dans la palette pour plus de clarté :

**Tile Palette :**
1. Cliquez sur l'icône **"Edit"** (crayon) en haut de la Tile Palette
2. Glissez-déposez les tiles pour les réorganiser en grille 4x4
3. Cliquez à nouveau sur **"Edit"** pour désactiver le mode édition

---

## Étape 7 : Peindre le Terrain

### 7.1 Sélectionner le Tilemap cible

**Tile Palette :**

En haut de la fenêtre, vérifiez que **"Active Tilemap"** est bien **"Terrain"** (ou "Tilemap" si vous n'avez pas renommé).

Si ce n'est pas le cas :
1. Cliquez sur le menu déroulant "Active Tilemap"
2. Sélectionnez **"Terrain"**

### 7.2 Sélectionner l'outil Brush

**Tile Palette - Barre d'outils :**

```
[Select] [Move] [🖌️ Brush] [Rectangle] [Picker] [Eraser] [Flood Fill]
                    ↑
               Cliquez ici
```

Ou utilisez le raccourci clavier : **B**

### 7.3 Peindre manuellement

**Méthode simple :**

1. Cliquez sur une tile dans la palette (par exemple `CLEAR1.TEM-0000`)
2. Dans la **Scene View**, cliquez pour peindre
3. Maintenez le clic et glissez pour peindre plusieurs cellules
4. Changez de tile pour créer de la variété

**Résultat :**
- Le terrain apparaît dans la Scene View
- Chaque cellule fait exactement 1x1 unité Unity

### 7.4 Peindre avec variation aléatoire (RECOMMANDÉ)

Pour créer de la variété automatiquement :

**Tile Palette - Brush Settings :**

1. Sélectionnez l'outil **Brush**
2. Dans l'Inspector, cherchez **"Tilemap Brush"**
3. **Sélectionnez TOUS les 16 tiles** dans la Tile Palette :
   - Maintenez **Ctrl** (ou **Cmd** sur Mac)
   - Cliquez sur chacune des 16 tiles
4. Les 16 tiles sont maintenant dans le brush
5. Dans la Scene View, peignez normalement

**Résultat :**
- Unity sélectionne aléatoirement parmi les 16 variations à chaque clic
- Cela crée un terrain naturel et varié, comme dans C&C classique

### 7.5 Remplir une zone (Flood Fill)

Pour remplir rapidement une grande zone :

**Tile Palette :**

1. Sélectionnez l'outil **Flood Fill** (seau de peinture)
2. Cliquez sur une tile dans la palette
3. Dans la Scene View, cliquez sur la zone à remplir

**Attention :** Flood Fill ne supporte pas les variations aléatoires, utilisez le Brush pour plus de variété.

### 7.6 Dimensions recommandées

Pour commencer, peignez une zone de **20x20 cellules** (comme configuré dans GridManager) :

**Astuce :**
- Le Grid Gizmo (lignes vertes) apparaît dans la Scene View
- Comptez les cellules ou utilisez l'outil Rectangle pour peindre exactement 20x20

---

## Étape 8 : Ajouter le GridManager à la Scène

### 8.1 Créer le GameObject GridManager

**Hierarchy → Clic droit :**
```
Create Empty
```

**Renommez-le :** `GridManager`

**Hiérarchie finale :**
```
Scene: Game
├── Main Camera
├── GridManager        ← Nouvellement créé
└── Grid
    └── Terrain
```

### 8.2 Ajouter le script GridManager

**Inspector (avec GridManager sélectionné) :**

1. Cliquez sur **"Add Component"**
2. Tapez **"GridManager"**
3. Sélectionnez **"Grid Manager (Script)"**

Ou glissez-déposez le script depuis :
```
Assets/_Project/Grid/Scripts/GridManager.cs
```

### 8.3 Configurer le GridManager

**Inspector → Grid Manager (Script) :**

```
Grid Configuration:
  Width: 20          ← Correspond à votre terrain peint
  Height: 20         ← Correspond à votre terrain peint

Debug:
  Show Debug Gizmos: ✓ (coché)
  Grid Color: (0, 1, 0, 0.2)      ← Vert transparent
  Occupied Color: (1, 0, 0, 0.3)  ← Rouge transparent
```

**Important :** Width et Height doivent correspondre à la taille de votre terrain peint.

---

## Étape 9 : Tester le Système

### 9.1 Vérifier les Gizmos de la grille

**Scene View :**

Vous devriez voir :
- ✅ Le terrain peint (sprites Clear1)
- ✅ Des lignes vertes formant une grille 20x20
- ✅ Les lignes se superposent parfaitement aux tiles

**Si vous ne voyez pas les lignes vertes :**
1. Vérifiez que **Gizmos** est activé (bouton en haut à droite de la Scene View)
2. Vérifiez que **Show Debug Gizmos** est coché dans GridManager

### 9.2 Vérifier l'alignement

**Test rapide :**

Les cellules du Tilemap et du GridManager doivent être parfaitement alignées :

1. Dans la Scene View, zoomez sur une tile
2. La tile doit occuper exactement 1x1 unité Unity
3. Les lignes vertes du GridManager doivent longer les bords de la tile

**Si ce n'est pas aligné :**
- Vérifiez que Grid Cell Size = (1, 1, 0)
- Vérifiez que les sprites ont PPU=128

### 9.3 Vérifier la Console

**Console Unity :**

Au démarrage du Play Mode, vous devriez voir :
```
[GridManager] Grid initialized: 20x20 cells (cellSize=1.0 unity)
```

Si vous voyez ce message, le système est fonctionnel ! ✅

### 9.4 Tester en Play Mode

**Lancez le jeu :**

1. Cliquez sur **Play** (▶️)
2. Le terrain doit s'afficher normalement
3. Les Gizmos de la grille restent visibles
4. Aucune erreur dans la Console

**Pour tester l'occupation des cellules (plus tard) :**
- Les cellules occupées par des unités s'afficheront en rouge
- Pour l'instant, toutes les cellules sont libres (vertes)

---

## Étape 10 : Ajuster la Caméra (Optionnel)

Pour voir l'ensemble du terrain 20x20 :

### 10.1 Calculer l'Orthographic Size

Pour voir exactement 20 cellules verticalement :
```
Orthographic Size = (Cellules verticales / 2) = 20 / 2 = 10
```

### 10.2 Configurer la Main Camera

**Hierarchy → Main Camera :**

**Inspector → Camera :**
```
Projection: Orthographic
Size: 10               ← Pour voir 20 cellules verticalement
Clipping Planes:
  Near: 0.3
  Far: 1000

Position:
  X: 10               ← Centre du terrain (20/2)
  Y: 10               ← Centre du terrain (20/2)
  Z: -10
```

**Résultat :**
- La caméra est centrée sur le terrain 20x20
- Tout le terrain est visible à l'écran

---

## Résumé de la Configuration

### Structure des fichiers

```
Assets/_Project/
├── Map/
│   ├── Sprites/
│   │   └── Terrain/
│   │       └── Clear1/
│   │           ├── CLEAR1.TEM-0000.png (PPU=128)
│   │           ├── CLEAR1.TEM-0001.png (PPU=128)
│   │           └── ... (14 autres sprites)
│   ├── Tiles/
│   │   └── Clear1/
│   │       ├── CLEAR1.TEM-0000.asset
│   │       ├── CLEAR1.TEM-0001.asset
│   │       └── ... (14 autres Tiles)
│   ├── Palettes/
│   │   └── Clear1_Terrain.prefab
│   └── Scripts/
│       └── (Map scripts, si nécessaire)
└── Grid/
    └── Scripts/
        ├── GridManager.cs
        └── GridCell.cs
```

### Hiérarchie de la scène

```
Game.unity
├── Main Camera (Position: 10, 10, -10 | Size: 10)
├── GridManager (Script: GridManager.cs | 20x20)
└── Grid (Cell Size: 1, 1, 0)
    └── Terrain (Tilemap peint avec Clear1)
```

### Paramètres clés

| Élément | Paramètre | Valeur |
|---------|-----------|--------|
| Sprites Clear1 | Pixels Per Unit | **128** |
| Sprites Clear1 | Filter Mode | Point (no filter) |
| Grid | Cell Size | **(1, 1, 0)** |
| GridManager | Width x Height | **20 x 20** |
| GridManager | Cell Size (calculé) | **1.0 unité Unity** |
| Tilemap Cell | Taille réelle | **1.0 x 1.0 unité Unity** |

---

## Dépannage

### Problème : Les tiles apparaissent floues

**Cause :** Filter Mode n'est pas sur "Point"

**Solution :**
1. Sélectionnez un sprite Clear1
2. Inspector → Filter Mode → **Point (no filter)**
3. Ou ré-exécutez : `Tools → Reconfigure All Terrain Sprites`

### Problème : Les tiles ne font pas 1x1 unité Unity

**Cause :** PPU n'est pas à 128

**Solution :**
1. Sélectionnez un sprite Clear1
2. Inspector → Pixels Per Unit → **128**
3. Ou ré-exécutez : `Tools → Reconfigure All Terrain Sprites`

### Problème : La grille n'est pas alignée avec le Tilemap

**Cause :** Grid Cell Size incorrect

**Solution :**
1. Sélectionnez le GameObject **Grid**
2. Inspector → Grid → Cell Size → **(1, 1, 0)**

### Problème : Les Gizmos ne s'affichent pas

**Cause :** Gizmos désactivés

**Solution :**
1. Scene View → Bouton **Gizmos** (en haut à droite) → Activé
2. GridManager → Inspector → Show Debug Gizmos → **Coché**

### Problème : "CommandAndConquer.Core does not exist"

**Cause :** Assembly reference incorrecte

**Solution :**
1. Vérifiez `Assets/_Project/Grid/CommandAndConquer.Grid.asmdef`
2. `"references": ["CommandAndConquer.Core"]` (pas "GUID:...")

---

## Prochaines Étapes

Maintenant que le système de Tilemap et Grid est configuré, vous pouvez :

1. **Créer des unités** (Infantry) - Commit 8 du ROADMAP
2. **Implémenter le système de sélection** - Commit 9
3. **Ajouter le mouvement des unités** sur la grille
4. **Ajouter d'autres types de terrain** (Clear2, Clear3, Rough, etc.)

Consultez le fichier `ROADMAP.md` pour la suite du développement.

---

**✅ Configuration terminée !**

Vous avez maintenant :
- Un terrain 20x20 avec variation visuelle (16 sprites Clear1)
- Un système de grille logique aligné avec le Tilemap
- Des cellules de 1.0 unité Unity pour des calculs simples
- Un système extensible pour ajouter des unités et du gameplay
