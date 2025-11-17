# UnitSpriteImporter

Outil Editor pour configurer automatiquement les sprites d'unités avec les paramètres optimaux pour le système de grille et les animations multi-directionnelles.

---

## 📋 Vue d'ensemble

`UnitSpriteImporter.cs` est un **AssetPostprocessor** Unity qui :
- Surveille tous les dossiers `Assets/_Project/Units/*/Sprites/`
- Configure automatiquement chaque sprite d'unité importé
- Support des sprite sheets multi-directions
- Applique des paramètres optimisés pour l'affichage et l'animation

---

## 🎯 Fonctionnalités

### 1. Configuration automatique à l'import

Lorsque vous ajoutez un sprite dans `Units/[NomUnité]/Sprites/`, il est automatiquement configuré avec :

| Paramètre | Valeur | Raison |
|-----------|--------|--------|
| **PPU** | 128 | Alignement avec grille 1.0 unité |
| **Filter Mode** | Point | Rendu pixel perfect |
| **Compression** | None | Qualité maximale |
| **Sprite Mode** | Multiple | Support sprite sheets découpés |
| **Mipmaps** | Disabled | Non nécessaires en 2D |
| **Mesh Type** | Full Rect | Forme rectangulaire simple |
| **Physics Shape** | Enabled | Collision pour unités |

### 2. Menus de reconfiguration

**Menu global** : `Tools > Command & Conquer > Reconfigure All Unit Sprites`
- Scanne tous les sprites dans `Units/*/Sprites/`
- Met à jour toutes les unités d'un coup

**Menu spécifique Buggy** : `Tools > Command & Conquer > Reconfigure Buggy Sprites`
- Scanne uniquement `Units/Buggy/Sprites/`
- Reconfiguration rapide pour une unité

---

## 🚀 Utilisation

### Workflow standard - Nouvelle unité

1. **Créer la structure**
   ```
   Assets/_Project/Units/
   └── [NomUnité]/
       ├── Scripts/
       ├── Prefabs/
       ├── Sprites/     ← Créer ce dossier
       └── Data/
   ```

2. **Préparer les sprites**
   - Taille recommandée : 128×128 pixels
   - Format : PNG avec transparence alpha
   - Nommage : `[unit]-[frame].png` (ex: `buggy-0000.png`)
   - Sprite sheets : toutes les directions dans un fichier ou fichiers séparés

3. **Placer les sprites**
   ```
   Units/Buggy/Sprites/
   ├── buggy-0000.png    (Nord)
   ├── buggy-0002.png    (Nord-Est)
   ├── buggy-0004.png    (Est)
   ├── buggy-0006.png    (Sud-Est)
   ├── buggy-0008.png    (Sud)
   ├── buggy-0010.png    (Sud-Ouest)
   ├── buggy-0012.png    (Ouest)
   └── buggy-0014.png    (Nord-Ouest)
   ```

4. **Import automatique**
   - Unity détecte les nouveaux fichiers
   - `UnitSpriteImporter` s'exécute automatiquement
   - Console : `[UnitSpriteImporter] Configured [chemin] with PPU=128`

5. **Vérification**
   - Sélectionnez un sprite dans Unity
   - Inspector : PPU=128, Filter Mode=Point, Sprite Mode=Multiple
   - Ouvrez le Sprite Editor pour voir/ajuster le découpage

### Sprites multi-directions (8 directions)

Pour un système de rotation comme dans les RTS classiques :

**Organisation recommandée :**
- 16 sprites = 8 directions × 2 frames d'animation
- Numérotation : 0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30
- Ordre : Nord (0°) → Est (90°) → Sud (180°) → Ouest (270°)

**Mapping des directions :**
```
     N (0)
  NW ↑ NE
W ← · → E
  SW ↓ SE
     S (16)

0000: Nord        (↑)
0002: Nord-Est    (↗)
0004: Est         (→)
0006: Sud-Est     (↘)
0008: Sud         (↓)
0010: Sud-Ouest   (↙)
0012: Ouest       (←)
0014: Nord-Ouest  (↖)
```

---

## 🔧 Code source

### AssetPostprocessor

```csharp
public class UnitSpriteImporter : AssetPostprocessor
{
    private const int UNIT_PPU = 128;
    private const string UNIT_SPRITES_PATH = "Assets/_Project/Units";

    private void OnPreprocessTexture()
    {
        // Vérifie si l'asset est dans Units/*/Sprites
        if (!assetPath.Contains(UNIT_SPRITES_PATH) || !assetPath.Contains("/Sprites/"))
            return;

        TextureImporter textureImporter = (TextureImporter)assetImporter;

        // Configuration pour pixel art 2D
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Multiple; // Multiple !
        textureImporter.spritePixelsPerUnit = UNIT_PPU;
        textureImporter.filterMode = FilterMode.Point;
        textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
        textureImporter.mipmapEnabled = false;

        // Configuration pour 2D
        TextureImporterSettings settings = new TextureImporterSettings();
        textureImporter.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.FullRect;
        settings.spriteGenerateFallbackPhysicsShape = true; // Physics pour unités
        textureImporter.SetTextureSettings(settings);

        Debug.Log($"[UnitSpriteImporter] Configured {assetPath} with PPU={UNIT_PPU}");
    }
}
```

### Différences avec TerrainSpriteImporter

| Paramètre | Terrain | Unités | Raison |
|-----------|---------|--------|--------|
| Sprite Mode | Single | **Multiple** | Support découpage/sprite sheets |
| Physics Shape | Disabled | **Enabled** | Collision nécessaire pour unités |
| Chemin | `/Terrain/` | `/Sprites/` | Flexibilité par unité |

---

## 🎓 Exemples d'utilisation

### Exemple 1 : Import Buggy (16 sprites, 8 directions)

**Avant :**
```
Units/Buggy/Sprites/
└── (vide)
```

**Action :**
- Copier 16 sprites `buggy-0000.png` à `buggy-0030.png` (128×128px)

**Résultat :**
```
Console Unity :
[UnitSpriteImporter] Configured .../buggy-0000.png with PPU=128
[UnitSpriteImporter] Configured .../buggy-0002.png with PPU=128
... (16 lignes)
```

- Chaque sprite : PPU=128, Filter=Point, Multiple mode
- Prêts pour système de rotation 8 directions
- Découpage automatique dans Sprite Editor

### Exemple 2 : Reconfiguration après import manuel

**Situation :**
- Vous avez importé 16 sprites Buggy manuellement
- PPU actuel : 100 (défaut Unity)
- Filter Mode : Bilinear (flou)

**Action :**
1. Menu : `Tools > Command & Conquer > Reconfigure Buggy Sprites`
2. Popup : "Successfully reconfigured 16 Buggy sprites!"

**Résultat :**
- Les 16 sprites ont maintenant PPU=128, Filter=Point
- Mode Multiple activé
- Prêts pour l'utilisation

### Exemple 3 : Ajout d'une nouvelle unité (Tank)

**Structure :**
```
Units/
├── Buggy/       (existant)
└── Tank/        (nouveau)
    └── Sprites/
        ├── tank-0000.png
        ├── tank-0002.png
        └── ...
```

**Action :**
- Copier les sprites Tank dans le dossier
- Attendre l'import Unity

**Résultat :**
- `UnitSpriteImporter` détecte automatiquement
- Configure avec les mêmes paramètres que Buggy
- Aucune configuration manuelle nécessaire

---

## 🐛 Dépannage

### Sprite Mode reste en Single

**Symptômes :**
- Sprite Mode = Single au lieu de Multiple
- Impossible de découper dans Sprite Editor

**Causes :**
1. Script n'a pas pu écrire la configuration
2. Import Unity déjà terminé avant le script

**Solutions :**
1. Sélectionner sprite → Clic droit → Reimport
2. Utiliser menu de reconfiguration
3. Manuellement : Inspector → Sprite Mode → Multiple → Apply

### Sprites d'animation ne s'alignent pas

**Symptômes :**
- Les frames d'animation "sautent" visuellement
- Pas d'alignement cohérent

**Causes :**
1. PPU différent entre sprites
2. Pivot points différents
3. Découpage incorrect dans Sprite Editor

**Solutions :**
1. Reconfigurer tous les sprites avec le menu
2. Sprite Editor → vérifier que tous les sprites ont le même pivot (Center)
3. S'assurer que le contenu est centré dans chaque image 128×128

### Le menu "Reconfigure..." n'apparaît pas

**Symptômes :**
- Menu `Tools > Command & Conquer/` existe mais pas d'option Buggy

**Causes :**
1. Script non compilé
2. Erreur dans le code
3. Assembly Definition manquante

**Solutions :**
1. Console → Vérifier erreurs de compilation
2. Vérifier `CommandAndConquer.Units.Editor.asmdef` existe
3. Assets → Reimport All

---

## 📐 Spécifications techniques

### Structure de fichier recommandée

**Sprite unique (128×128px) :**
```
┌──────────────────┐
│                  │ ← Marges transparentes
│   ┌────────┐     │
│   │ UNIT   │     │ ← Contenu centré
│   │        │     │
│   └────────┘     │
│                  │
└──────────────────┘
   128×128 pixels
```

**Découpage dans Sprite Editor :**
- Automatic Slicing pour détecter le contenu
- Ou manuel : définir la zone exacte du sprite
- Pivot : Center (0.5, 0.5) recommandé

### PPU = 128 pour les unités

Avec une unité Buggy de **77×100px** dans une image **128×128px** :
- PPU=128 → Taille Unity : 1.0 × 1.0 unité
- Contenu réel (77×100) → ~0.6 × 0.78 unité
- S'inscrit parfaitement dans une cellule de grille 1.0

**Avantages :**
- Unité ne déborde pas de sa cellule
- Calculs de position simplifiés
- Cohérence avec le système de terrain

---

## 📚 Références

- **Documentation principale Tools** : [TOOLS.md](../../../../TOOLS.md)
- **TerrainSpriteImporter** : [Map/Editor/README.md](../../Map/Editor/README.md)
- **Système de grille** : [ROADMAP.md](../../../../ROADMAP.md) - Commit 7
- **Catalogue unités** : [UNITS.md](../../../../UNITS.md)

---

## 🔄 Évolutions futures

Fonctionnalités potentielles :

- [ ] Génération automatique d'animations 8-directions
- [ ] Détection automatique du nombre de directions
- [ ] Support de frames d'animation variables
- [ ] Génération automatique de préfabs
- [ ] Validation des dimensions (warning si pas 128×128)
- [ ] Export de configuration pour partage

---

**Créé :** Commit 8 - Buggy sprites et UnitSpriteImporter
**Documenté :** Commit 8 - Documentation des tools
