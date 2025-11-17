# Guide : Peinture avec Variation Aléatoire

Ce guide explique comment peindre le terrain avec variation aléatoire pour créer un aspect naturel et varié, comme dans Command & Conquer classique.

---

## Pourquoi la Variation Aléatoire ?

Dans C&C classique, chaque type de terrain possède **plusieurs variations** du même sprite pour éviter la répétition visuelle.

**Sans variation :** ❌
```
[Tile A] [Tile A] [Tile A] [Tile A]
[Tile A] [Tile A] [Tile A] [Tile A]
[Tile A] [Tile A] [Tile A] [Tile A]
```
→ Terrain monotone, répétitif, artificiel

**Avec variation :** ✅
```
[Tile A] [Tile C] [Tile B] [Tile F]
[Tile D] [Tile A] [Tile E] [Tile C]
[Tile B] [Tile F] [Tile D] [Tile A]
```
→ Terrain naturel, varié, organique

Nous avons **16 variations** du terrain Clear1, utilisons-les !

---

## Méthode : Brush avec Pick Random Tiles

Cette méthode est simple, rapide et ne nécessite aucune configuration d'asset.

### Étape 1 : Ouvrir la Tile Palette

**Menu Unity :**
```
Window → 2D → Tile Palette
```

Assurez-vous que la palette **"Clear1_Terrain"** est active.

### Étape 2 : Sélectionner l'outil Brush

**Tile Palette - Barre d'outils :**
```
┌─────────────────────────────────────┐
│ [▢] [↔] [🖌] [▭] [🎨] [⌫] [🪣]      │
│          ↑                          │
│        Brush                        │
└─────────────────────────────────────┘
```

Cliquez sur l'icône **Brush** (pinceau) ou appuyez sur **B**.

### Étape 3 : Sélectionner les 16 tiles Clear1

**Dans la Tile Palette :**

**Méthode rapide :**
1. Cliquez sur la **première tile** Clear1
2. Maintenez **Shift** et cliquez sur la **dernière tile** Clear1
3. Toutes les tiles entre les deux sont sélectionnées (16 tiles)

**Méthode manuelle (si nécessaire) :**
- **Windows / Linux :** Maintenez **Ctrl** et cliquez sur chaque tile
- **macOS :** Maintenez **Cmd (⌘)** et cliquez sur chaque tile

**Visuel :**
```
Tile Palette (Clear1_Terrain)
┌──────────────────────────────┐
│ [0📦] [1📦] [2📦] [3📦]      │  ← Toutes entourées
│ [4📦] [5📦] [6📦] [7📦]      │    en bleu
│ [8📦] [9📦] [A📦] [B📦]      │
│ [C📦] [D📦] [E📦] [F📦]      │
└──────────────────────────────┘
    (16 tiles sélectionnées)
```

### Étape 4 : Activer Pick Random Tiles

**En dessous des tiles dans la Tile Palette :**

Vous verrez plusieurs options pour l'outil Brush. Cochez l'option **"Pick Random Tiles"**.

**Options du Brush :**
```
┌─────────────────────────────────────┐
│ Brush Options:                      │
│ ☐ Keep filled                       │
│ ☑ Pick Random Tiles  ← Cochez ceci │
│ ☐ Flip X                            │
│ ☐ Flip Y                            │
│ ☐ Rotate 90°                        │
└─────────────────────────────────────┘
```

Une fois cochée, le brush utilisera automatiquement une tile aléatoire parmi les 16 sélectionnées à chaque clic.

### Étape 5 : Peindre dans la Scene View

**Scene View :**

1. Cliquez pour peindre une cellule
2. Maintenez le clic et glissez pour peindre plusieurs cellules
3. **À chaque clic, Unity sélectionne aléatoirement une des 16 tiles**

**Résultat :**
- Chaque cellule a une tile différente (probabilité : 1/16 pour chaque)
- Le terrain a un aspect naturel et varié
- Pas de répétition évidente

**Raccourcis clavier utiles :**
- **Shift + Glisser** : Peint en ligne droite
- **Ctrl/Cmd + Z** : Annuler la dernière action

---

## Astuces et Bonnes Pratiques

### Astuce 1 : Peindre par Zones

Pour un meilleur contrôle :

1. **Peignez le contour** de votre terrain (20x20)
2. **Remplissez l'intérieur** avec le Brush en mode aléatoire
3. **Retouchez manuellement** les zones qui ne vous plaisent pas

### Astuce 2 : Effacer et Repeindre

Si une zone ne vous plaît pas :

1. Sélectionnez l'outil **Eraser** (gomme) ou **Ctrl/Cmd + Shift + Clic**
2. Effacez la zone
3. Repeignez avec le Brush en mode aléatoire
4. Vous obtenez une nouvelle distribution aléatoire

### Astuce 3 : Vérifier la Variation

**Scene View :**

Zoomez et vérifiez qu'il n'y a pas de **patterns évidents** :

❌ **Mauvais :**
```
[A] [B] [C] [A] [B] [C]
[A] [B] [C] [A] [B] [C]  ← Pattern répétitif
```

✅ **Bon :**
```
[A] [F] [B] [D] [A] [E]
[C] [B] [A] [F] [D] [B]  ← Vraiment aléatoire
```

Si vous voyez un pattern :
- Effacez la zone
- Repeignez avec le Brush en mode aléatoire

### Astuce 4 : Combiner Peinture Manuelle et Aléatoire

**Workflow hybride :**

1. **Peinture aléatoire** pour 90% du terrain
2. **Peinture manuelle** (décochez "Pick Random Tiles") pour les zones spéciales :
   - Bords de la map
   - Zones autour des bâtiments
   - Chemins visuels

---

## Dépannage

### Problème : Toujours la Même Tile

**Cause :** Option "Pick Random Tiles" désactivée ou une seule tile sélectionnée

**Solution :**
1. Vérifiez que **"Pick Random Tiles"** est bien coché en dessous des tiles
2. Vérifiez que les **16 tiles sont sélectionnées** (entourées en bleu)
3. Resélectionnez les 16 tiles avec **Shift + Clic**

### Problème : Pattern Répétitif Évident

**Cause :** Seed aléatoire défavorable

**Solution :**
1. Effacez la zone
2. Repeignez → Unity utilise un nouveau seed aléatoire

### Problème : L'option "Pick Random Tiles" N'Apparaît Pas

**Cause :** Package 2D Tilemap Extras non installé

**Solution :**
1. **Window → Package Manager**
2. En haut à gauche : **Packages: Unity Registry**
3. Cherchez **"2D Tilemap Extras"**
4. Cliquez sur **Install**
5. Redémarrez Unity si nécessaire

---

## Résumé Visuel

### Workflow Complet (5 Étapes)

```
┌─────────────────────────────────────────┐
│ 1. Ouvrir Tile Palette                 │
│    Window → 2D → Tile Palette           │
└─────────────┬───────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ 2. Sélectionner l'outil Brush           │
│    Cliquer sur icône Brush ou [B]       │
└─────────────┬───────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ 3. Sélectionner les 16 tiles            │
│    Clic sur première + Shift + Clic     │
│    sur dernière                         │
└─────────────┬───────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ 4. Activer Pick Random Tiles            │
│    Cocher l'option en dessous des tiles │
└─────────────┬───────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ 5. Peindre dans Scene View              │
│    Cliquer + Glisser                    │
│    → Variation aléatoire automatique    │
└─────────────────────────────────────────┘
```

---

## Appliquer à D'Autres Terrains

Cette technique fonctionne avec **tous les types de terrain** :

**Clear2, Clear3, Rough, Water, etc. :**

1. Importez les sprites dans `Map/Sprites/Terrain/[TypeTerrain]/`
2. Créez une nouvelle palette : `[TypeTerrain]_Terrain`
3. Générez les tiles
4. Sélectionnez l'outil Brush
5. Sélectionnez toutes les variations
6. Activez "Pick Random Tiles"
7. Peignez !

---

**✅ Vous savez maintenant tout sur la peinture avec variation aléatoire !**

Cette technique est **essentielle** pour créer des terrains réalistes et agréables visuellement.

**Prochaine étape :** Peignez votre terrain 20x20 avec les 16 variations Clear1 et admirez le résultat ! 🎨
