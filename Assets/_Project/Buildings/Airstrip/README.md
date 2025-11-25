# Airstrip - Phase 1 Setup

## Configuration

**Taille** : 4×2 (4 cellules de large, 2 cellules de hauteur)
**Fonction** : Aéroport pour faire atterrir les unités
**Spawn Point** : Centre en bas (offset 2,0)

---

## Étapes de création dans Unity

### 1. Créer l'asset AirstripData

1. Ouvrir Unity et attendre la compilation
2. Menu : **Command & Conquer > Buildings > Create Airstrip Data**
3. L'asset sera créé dans `Airstrip/Data/AirstripData.asset`

### 2. Créer/Importer le sprite de l'Airstrip

**Option A** : Sprite temporaire (pour tests)
- Créer un rectangle 512×256 px (4×2 cellules @ 128 PPU)
- Importer dans `Airstrip/Sprites/`
- Configurer :
  - PPU=128
  - Filter=Point
  - Compression=None
  - **Pivot: Bottom Left (0, 0)** ⚠️ CRUCIAL pour l'alignement !

**Option B** : Sprite C&C original
- Importer le sprite de l'Airstrip depuis les assets C&C
- Placer dans `Airstrip/Sprites/`
- Configurer selon les standards du projet
- **Pivot: Bottom Left (0, 0)** ⚠️ CRUCIAL pour l'alignement !

**Comment changer le pivot manuellement** :
1. Sélectionner le sprite dans le Project
2. Inspector → Sprite Editor (bouton)
3. Dans Sprite Editor : Pivot → **Bottom Left** (ou Custom → X=0, Y=0)
4. Apply

**OU utiliser l'outil automatique** ⚡ :
- Menu : **Tools > Command & Conquer > Reconfigure Airstrip Sprites**
- Configure automatiquement PPU=128, Filter=Point, Pivot=Bottom Left
- Les nouveaux sprites importés dans `Buildings/*/Sprites/` sont auto-configurés !

### 3. Assigner le sprite à AirstripData

1. Sélectionner `AirstripData.asset`
2. Dans l'Inspector, glisser le sprite dans le champ **Sprite**

### 4. Créer le prefab Airstrip

1. Créer un GameObject vide : **Hierarchy > Create Empty**
2. Nommer : `Airstrip`
3. Ajouter composants :
   - `Building` (script)
   - `SpriteRenderer`
   - `BoxCollider2D` (optionnel pour Phase 1)

4. Configurer `Building` :
   - Glisser `AirstripData.asset` dans **Building Data**

5. Configurer `SpriteRenderer` :
   - Glisser le sprite de l'Airstrip
   - Sorting Layer : Default
   - Order in Layer : 0

6. Créer le prefab :
   - Glisser le GameObject dans `Airstrip/Prefabs/`
   - Nommer : `Airstrip.prefab`

### 5. Tester dans la scène

⚠️ **IMPORTANT - Convention de positionnement avec Pivot Bottom Left** :
La position du GameObject = **coin bas-gauche** (origine) du bâtiment directement !
- Pour occuper les cellules (5,9) à (8,10), placer le GameObject à **(5, 9)**
- Sprite avec pivot Bottom Left → position = origine automatiquement
- **Ultra-simple** : Position éditeur = position jeu, aucun calcul !

**Exemple Airstrip 4×2** :
- Position GameObject : **(5, 9)** ← Coin bas-gauche
- Cellules occupées : (5,9), (6,9), (7,9), (8,9), (5,10), (6,10), (7,10), (8,10)
- Le sprite s'étend parfaitement sur ces 8 cellules !

1. Ouvrir `Assets/_Project/Scenes/Game.unity`
2. Glisser le prefab `Airstrip` dans la scène
3. Positionner à l'**origine souhaitée** (ex: X=5, Y=9 pour occuper (5,9) à (8,10))
4. Lancer le jeu (Play ▶️)
5. Vérifier les logs :
   - `[Building] 'Airstrip' initialized at origin (5,9) (4×2)`
   - `[GridManager] Building placed at (5,9) (4×2), occupying 8 cells`

6. Vérifier les Gizmos :
   - 8 cellules bleus (Building) + rouges (GridManager) doivent apparaître (4×2)
   - Sphère jaune au centre (7,10), carré vert à l'origine (5.5,9.5)
   - Les unités ne peuvent pas traverser

---

## Validation Phase 1 ✅

- [ ] AirstripData.asset créé avec les bonnes dimensions (4×2)
- [ ] Sprite assigné et visible
- [ ] Prefab Airstrip fonctionnel
- [ ] Airstrip occupe bien 8 cellules (4×2) dans la grille
- [ ] GridManager détecte correctement les collisions multi-cellules
- [ ] Logs de debug confirment le placement correct

---

## Prochaines Étapes (Phase 2)

- Ajouter `ProductionQueue` component
- Créer `ProductionItem` assets pour Buggy et Artillery
- Implémenter le système de production avec timer

---

**Phase actuelle** : Phase 1 - Core Building System
**Dernière mise à jour** : 2025-11-25
