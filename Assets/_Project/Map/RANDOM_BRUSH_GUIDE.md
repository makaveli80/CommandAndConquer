# Guide Détaillé : Peinture avec Variation Aléatoire

Ce guide explique en détail comment peindre le terrain avec variation aléatoire pour créer un aspect naturel et varié, comme dans Command & Conquer classique.

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

## Méthode 1 : Sélection Multiple avec Brush Standard (Recommandé)

Cette méthode utilise le brush standard de Unity avec une sélection multiple de tiles.

### Étape par Étape

#### 1. Ouvrir la Tile Palette

**Menu Unity :**
```
Window → 2D → Tile Palette
```

Assurez-vous que la palette **"Clear1_Terrain"** est active.

#### 2. Sélectionner l'outil Brush

**Tile Palette - Barre d'outils :**
```
┌─────────────────────────────────────┐
│ [▢] [↔] [🖌] [▭] [🎨] [⌫] [🪣]      │
│          ↑                          │
│        Brush                        │
└─────────────────────────────────────┘
```

Cliquez sur l'icône **Brush** (pinceau) ou appuyez sur **B**.

#### 3. Sélectionner TOUTES les tiles Clear1

**Dans la Tile Palette :**

**Windows / Linux :**
1. Maintenez **Ctrl** enfoncé
2. Cliquez sur chacune des 16 tiles Clear1
3. Toutes les tiles sélectionnées s'entourent en bleu

**macOS :**
1. Maintenez **Cmd (⌘)** enfoncé
2. Cliquez sur chacune des 16 tiles
3. Toutes les tiles sélectionnées s'entourent en bleu

**Astuce pour sélectionner plus vite :**
- Première tile : Clic simple
- Dernière tile : **Shift + Clic**
- Toutes les tiles entre les deux sont sélectionnées
- Puis **Ctrl/Cmd + Clic** pour ajouter/retirer des tiles individuelles

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

#### 4. Vérifier la sélection dans l'Inspector

**Inspector (avec Tile Palette ouverte) :**

Vous devriez voir :
```
Tilemap Brush
  Brush Cells: 16        ← 16 tiles chargées
  Preview:
    [Aperçu des 16 tiles]
```

Si vous voyez "Brush Cells: 1", recommencez la sélection.

#### 5. Peindre dans la Scene View

**Scene View :**

1. Cliquez pour peindre une cellule
2. Maintenez le clic et glissez pour peindre plusieurs cellules
3. **À chaque clic, Unity sélectionne aléatoirement une des 16 tiles**

**Résultat :**
- Chaque cellule a une tile différente (probabilité : 1/16 pour chaque)
- Le terrain a un aspect naturel et varié
- Pas de répétition évidente

#### 6. Remplir une grande zone

Pour remplir rapidement une zone 20x20 :

1. **Gardez les 16 tiles sélectionnées**
2. **Maintenez le clic et glissez** sur toute la zone
3. Unity remplit avec variation aléatoire automatique

**Raccourci clavier :**
- **Shift + Glisser** : Peint en ligne droite
- **Ctrl/Cmd + Glisser** : Efface les tiles

---

## Méthode 2 : Random Brush (Avancé)

Unity propose un brush spécial pour la randomisation avancée.

### Installation du Random Brush

#### 1. Ouvrir le Package Manager

**Menu Unity :**
```
Window → Package Manager
```

#### 2. Installer 2D Tilemap Extras (si pas déjà fait)

**Package Manager :**
1. En haut à gauche : **Packages: Unity Registry**
2. Cherchez **"2D Tilemap Extras"**
3. Cliquez sur **Install**

**Note :** Ce package devrait déjà être installé dans votre projet.

### Utilisation du Random Brush

#### 1. Créer un Random Brush Asset

**Project :**

Naviguez vers :
```
Assets/_Project/Map/Brushes/
```

(Créez le dossier "Brushes" s'il n'existe pas)

**Clic droit dans le dossier :**
```
Create → Brushes → Random Brush
```

**Nommez-le :** `Clear1_RandomBrush`

#### 2. Configurer le Random Brush

**Sélectionnez Clear1_RandomBrush :**

**Inspector → Random Brush :**

```
Size: 16                      ← Nombre de tiles différentes
Randomize Per Cell: ✓ (coché) ← Randomise à chaque cellule

Brush Cells:
  Element 0: CLEAR1.TEM-0000
  Element 1: CLEAR1.TEM-0001
  Element 2: CLEAR1.TEM-0002
  ...
  Element 15: CLEAR1.TEM-0015
```

**Pour ajouter les tiles :**

1. **Size:** 16
2. Glissez-déposez chaque tile depuis `Map/Tiles/Clear1/` dans chaque Element

**Ou plus rapide :**

Sélectionnez toutes les 16 tiles dans le dossier `Map/Tiles/Clear1/` et glissez-les en bloc sur "Brush Cells".

#### 3. Utiliser le Random Brush

**Tile Palette :**

1. Cliquez sur le menu **"Brush"** en haut (à côté de l'icône pinceau)
2. Sélectionnez **"Random Brush"** (au lieu de "Default Brush")
3. Sélectionnez votre asset **Clear1_RandomBrush** dans le Project
4. Peignez normalement dans la Scene View

**Résultat :**
- À chaque cellule, une tile aléatoire est choisie parmi les 16
- Comportement identique à la Méthode 1, mais configuré via un asset

---

## Méthode 3 : Weighted Random Brush (Contrôle Avancé)

Pour contrôler la **probabilité d'apparition** de chaque tile.

### Pourquoi Contrôler les Probabilités ?

Parfois, vous voulez que certaines tiles apparaissent plus souvent :

**Exemple :**
- Tile 0 (herbe pure) : 40% de chances
- Tile 1-15 (variations) : 4% de chances chacune

Cela crée un terrain où l'herbe pure domine avec des touches subtiles de variation.

### Créer un Weighted Random Brush

#### 1. Créer l'asset

**Project → Assets/_Project/Map/Brushes/ :**

**Clic droit :**
```
Create → Brushes → Weighted Random Brush
```

**Nommez-le :** `Clear1_WeightedBrush`

#### 2. Configurer les poids

**Inspector → Weighted Random Brush :**

```
Randomize Per Cell: ✓

Weighted Brush Cells:
  Element 0:
    Tile: CLEAR1.TEM-0000
    Weight: 10               ← Apparaît 10x plus souvent

  Element 1:
    Tile: CLEAR1.TEM-0001
    Weight: 1                ← Poids normal

  Element 2:
    Tile: CLEAR1.TEM-0002
    Weight: 1

  ...

  Element 15:
    Tile: CLEAR1.TEM-0015
    Weight: 1
```

**Calcul de probabilité :**
```
Total weights = 10 + 1 + 1 + ... + 1 = 10 + 15 = 25

Tile 0: 10/25 = 40% de chances
Tile 1-15: 1/25 = 4% de chances chacune
```

#### 3. Exemples de Configurations

**Configuration "Uniforme" (par défaut) :**
```
Toutes les tiles: Weight = 1
→ Chaque tile: 1/16 = 6.25% de chances
```

**Configuration "Dominant" :**
```
Tile 0: Weight = 50
Tiles 1-15: Weight = 1 chacune
Total = 50 + 15 = 65

→ Tile 0: 50/65 = 77% de chances (domine)
→ Tiles 1-15: 1/65 = 1.5% chacune (rare)
```

**Configuration "Équilibré avec Accent" :**
```
Tiles 0-3: Weight = 5 chacune
Tiles 4-15: Weight = 1 chacune
Total = (5×4) + (1×12) = 20 + 12 = 32

→ Tiles 0-3: 5/32 = 15.6% chacune
→ Tiles 4-15: 1/32 = 3.1% chacune
```

---

## Comparaison des Méthodes

| Méthode | Simplicité | Flexibilité | Contrôle | Recommandation |
|---------|------------|-------------|----------|----------------|
| **Sélection Multiple** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐ | ✅ **Débutant** |
| **Random Brush** | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐ | ✅ **Intermédiaire** |
| **Weighted Random** | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ✅ **Avancé** |

### Quelle Méthode Choisir ?

**Pour commencer :**
→ **Méthode 1** (Sélection Multiple)
- Rapide, aucune configuration
- Parfait pour tester

**Pour un workflow professionnel :**
→ **Méthode 2** (Random Brush)
- Asset réutilisable
- Facile à partager dans l'équipe

**Pour un contrôle artistique fin :**
→ **Méthode 3** (Weighted Random Brush)
- Contrôle total des probabilités
- Création de terrains avec "caractère"

---

## Astuces et Bonnes Pratiques

### Astuce 1 : Prévisualisation Avant Peinture

**Tile Palette :**

Avant de peindre, vérifiez la prévisualisation :
- La zone de prévisualisation montre une tile aléatoire
- Cela change à chaque fois que vous bougez le curseur
- Si vous voyez toujours la même tile → sélection incorrecte

### Astuce 2 : Peindre par Zones

Pour un meilleur contrôle :

1. **Peignez le contour** de votre terrain (20x20)
2. **Remplissez l'intérieur** avec le brush aléatoire
3. **Retouchez manuellement** les zones qui ne vous plaisent pas

### Astuce 3 : Effacer et Repeindre

Si une zone ne vous plaît pas :

1. Sélectionnez l'outil **Eraser** (gomme)
2. Effacez la zone
3. Repeignez avec le brush aléatoire
4. Vous obtenez une nouvelle distribution aléatoire

### Astuce 4 : Combiner Peinture Manuelle et Aléatoire

**Workflow hybride :**

1. **Peinture aléatoire** pour 90% du terrain
2. **Peinture manuelle** pour les zones spéciales :
   - Bords de la map
   - Zones autour des bâtiments
   - Chemins visuels

### Astuce 5 : Vérifier la Variation

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
- Repeignez avec le brush aléatoire

---

## Exemples Pratiques

### Exemple 1 : Terrain Clear1 Uniforme (C&C Classique)

**Configuration :**
- Méthode 1 (Sélection Multiple)
- Toutes les 16 tiles sélectionnées
- Probabilité égale pour chaque tile

**Résultat :**
- Terrain varié, naturel
- Aucune tile ne domine
- Aspect fidèle à C&C original

**Utilisation :**
- Zones de jeu standard
- Bases des joueurs
- Terrain neutre

### Exemple 2 : Terrain avec Zone "Sale" (Weighted)

**Configuration :**
```
Tiles 0-7 (herbe claire): Weight = 1
Tiles 8-15 (herbe foncée): Weight = 5
```

**Résultat :**
- Zone avec plus d'herbe foncée (terre, saleté)
- Aspect de terrain "usé" ou "pollué"

**Utilisation :**
- Zones industrielles
- Autour des usines Tiberium
- Zones de bataille

### Exemple 3 : Terrain "Pur" avec Touches de Variation

**Configuration :**
```
Tile 0 (base): Weight = 20
Tiles 1-15 (variations): Weight = 1 chacune
```

**Résultat :**
- Terrain très homogène (80% tile 0)
- Petites touches de variation (20% tiles 1-15)
- Aspect "propre" et "neuf"

**Utilisation :**
- Zones de départ des joueurs
- Bases bien entretenues
- Zones "safe"

---

## Dépannage

### Problème : Toujours la Même Tile

**Cause :** Une seule tile sélectionnée

**Solution :**
1. Vérifiez l'Inspector → Brush Cells: doit être **16**
2. Resélectionnez les 16 tiles avec **Ctrl/Cmd + Clic**

### Problème : Pattern Répétitif Évident

**Cause :** Seed aléatoire défavorable

**Solution :**
1. Effacez la zone
2. Repeignez → Unity utilise un nouveau seed aléatoire
3. Ou utilisez Weighted Brush avec poids différents

### Problème : Certaines Tiles N'Apparaissent Jamais

**Cause :** Tiles non incluses dans la sélection

**Solution :**
1. Vérifiez que les 16 tiles sont bien sélectionnées
2. Inspector → Brush Cells → Comptez les Elements (doit être 16)
3. Ajoutez les tiles manquantes manuellement

### Problème : Le Brush Ne Change Pas Malgré la Sélection

**Cause :** Brush non actualisé

**Solution :**
1. **Scene View → Bougez le curseur** pour rafraîchir le brush
2. Ou cliquez sur une cellule vide pour forcer l'actualisation
3. Ou resélectionnez les tiles

---

## Pour Aller Plus Loin

### Créer d'Autres Terrains avec Variation

Une fois maîtrisée, cette technique s'applique à **tous les terrains** :

**Clear2, Clear3, Rough, Water, etc. :**

1. Importez les sprites dans `Map/Sprites/Terrain/[TypeTerrain]/`
2. Créez une nouvelle palette : `[TypeTerrain]_Terrain`
3. Générez les tiles
4. Utilisez la même technique de sélection multiple ou Random Brush

### Mélanger Plusieurs Types de Terrain

**Tile Palette avancée :**

Vous pouvez créer une palette qui **combine plusieurs terrains** :

**Exemple :**
```
Palette: "AllTerrains"
├── Clear1 (16 tiles)
├── Clear2 (16 tiles)
├── Rough (16 tiles)
└── Water (16 tiles)
```

Ensuite :
- Peignez Clear1 avec variation aléatoire
- Puis peignez par-dessus avec Clear2 pour créer des transitions
- Puis ajoutez des zones Rough et Water

### Automatisation avec Scripts (Avancé)

Pour générer automatiquement un terrain entier :

**Script C# :**
```csharp
// Exemple de génération procédurale
public void GenerateTerrain()
{
    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            // Sélection aléatoire parmi les 16 tiles
            Tile randomTile = clear1Tiles[Random.Range(0, 16)];
            tilemap.SetTile(new Vector3Int(x, y, 0), randomTile);
        }
    }
}
```

Voir `MapGenerator.cs` (si créé) pour un exemple complet.

---

## Résumé Visuel

### Workflow Recommandé (Méthode 1)

```
┌─────────────────────────────────────────┐
│ 1. Ouvrir Tile Palette                 │
│    Window → 2D → Tile Palette           │
└─────────────┬───────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ 2. Sélectionner Brush                   │
│    Cliquer sur icône Brush ou [B]       │
└─────────────┬───────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ 3. Sélectionner les 16 tiles            │
│    Ctrl/Cmd + Clic sur chaque tile      │
│    ou Shift + Clic pour sélection zone  │
└─────────────┬───────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ 4. Vérifier l'Inspector                 │
│    Brush Cells: 16 ✓                    │
└─────────────┬───────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ 5. Peindre dans Scene View              │
│    Cliquer + Glisser                    │
│    → Variation aléatoire automatique    │
└─────────────────────────────────────────┘
```

---

**✅ Vous savez maintenant tout sur la peinture avec variation aléatoire !**

Cette technique est **essentielle** pour créer des terrains réalistes et agréables visuellement. Prenez le temps de l'expérimenter, c'est un investissement qui paiera sur le long terme.

**Prochaine étape :** Peignez votre terrain 20x20 avec les 16 variations Clear1 et admirez le résultat ! 🎨
