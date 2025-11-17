# TerrainSpriteImporter

Outil Editor pour configurer automatiquement les sprites de terrain avec les paramètres optimaux pour le système de grille.

---

## 📋 Vue d'ensemble

`TerrainSpriteImporter.cs` est un **AssetPostprocessor** Unity qui :
- Surveille le dossier `Assets/_Project/Map/Sprites/Terrain/`
- Configure automatiquement chaque sprite importé
- Applique des paramètres optimisés pour le système de grille
- Évite la configuration manuelle répétitive

---

## 🎯 Fonctionnalités

### 1. Configuration automatique à l'import

Lorsque vous ajoutez un sprite dans `Map/Sprites/Terrain/`, il est automatiquement configuré avec :

| Paramètre | Valeur | Raison |
|-----------|--------|--------|
| **PPU** | 128 | Alignement avec grille 1.0 unité |
| **Filter Mode** | Point | Rendu pixel perfect |
| **Compression** | None | Qualité maximale |
| **Sprite Mode** | Single | Un sprite = un fichier |
| **Mipmaps** | Disabled | Non nécessaires en 2D |
| **Mesh Type** | Full Rect | Forme rectangulaire simple |
| **Physics Shape** | Disabled | Pas de collision pour terrain |

### 2. Menu de reconfiguration

Si vous avez des sprites déjà importés avec de mauvais paramètres :

**Menu Unity**: `Tools > Command & Conquer > Reconfigure All Terrain Sprites`

- Scanne tous les sprites dans `Map/Sprites/Terrain/`
- Met à jour uniquement ceux avec des paramètres incorrects
- Affiche le nombre de sprites reconfigurés
- Logs détaillés dans la Console Unity

---

## 🚀 Utilisation

### Workflow standard

1. **Préparez vos sprites**
   - Taille recommandée : 128×128 pixels (ou multiple de 128)
   - Format : PNG avec transparence alpha
   - Nommage : descriptif (ex: `grass_01.png`, `dirt_02.png`)

2. **Placez les sprites**
   ```
   Assets/_Project/Map/Sprites/Terrain/
   ├── Clear/
   │   ├── clear1.png
   │   ├── clear2.png
   │   └── ...
   ├── Grass/
   │   └── grass1.png
   └── Desert/
       └── sand1.png
   ```

3. **Import automatique**
   - Unity détecte les nouveaux fichiers
   - `TerrainSpriteImporter` s'exécute automatiquement
   - Console affiche : `[TerrainSpriteImporter] Configured [chemin] with PPU=128`

4. **Vérification**
   - Sélectionnez le sprite dans Unity
   - Inspector : vérifiez PPU=128, Filter Mode=Point
   - Le sprite est prêt pour le Tilemap

### Reconfiguration manuelle

Si nécessaire :

1. Menu : `Tools > Command & Conquer > Reconfigure All Terrain Sprites`
2. Attendez la popup de confirmation
3. Console affiche : `[TerrainSpriteImporter] Reconfigured X terrain sprites`

---

## 🔧 Code source

### AssetPostprocessor

```csharp
public class TerrainSpriteImporter : AssetPostprocessor
{
    private const int TERRAIN_PPU = 128;
    private const string TERRAIN_SPRITES_PATH = "Assets/_Project/Map/Sprites/Terrain";

    private void OnPreprocessTexture()
    {
        // Vérifie si l'asset est dans le dossier Terrain
        if (!assetPath.Contains(TERRAIN_SPRITES_PATH))
            return;

        TextureImporter textureImporter = (TextureImporter)assetImporter;

        // Configuration pour pixel art 2D
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Single;
        textureImporter.spritePixelsPerUnit = TERRAIN_PPU;
        textureImporter.filterMode = FilterMode.Point;
        textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
        textureImporter.mipmapEnabled = false;

        // Configuration pour 2D
        TextureImporterSettings settings = new TextureImporterSettings();
        textureImporter.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.FullRect;
        settings.spriteGenerateFallbackPhysicsShape = false;
        textureImporter.SetTextureSettings(settings);

        Debug.Log($"[TerrainSpriteImporter] Configured {assetPath} with PPU={TERRAIN_PPU}");
    }
}
```

### Menu Editor

```csharp
public static class TerrainSpriteImporterMenu
{
    [MenuItem("Tools/Command & Conquer/Reconfigure All Terrain Sprites")]
    public static void ReconfigureAllTerrainSprites()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D",
            new[] { "Assets/_Project/Map/Sprites/Terrain" });

        int count = 0;
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer == null) continue;

            bool modified = false;

            // Vérifier et corriger chaque paramètre
            if (importer.spritePixelsPerUnit != 128) {
                importer.spritePixelsPerUnit = 128;
                modified = true;
            }
            // ... autres vérifications

            if (modified)
            {
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                count++;
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Terrain Sprites Reconfigured",
            $"Successfully reconfigured {count} terrain sprites", "OK");
    }
}
```

---

## 🎓 Exemples d'utilisation

### Exemple 1 : Import de sprites Clear terrain

**Avant :**
```
Map/Sprites/Terrain/Clear/
└── (vide)
```

**Action :**
- Copier `clear1.png` (128×128px) dans le dossier Clear

**Résultat :**
- Console : `[TerrainSpriteImporter] Configured .../clear1.png with PPU=128`
- Sprite configuré avec PPU=128, Filter=Point, Uncompressed
- Prêt à être utilisé dans le Tile Palette

### Exemple 2 : Reconfiguration de sprites existants

**Situation :**
- Vous avez 20 sprites de terrain déjà importés
- PPU actuel : 100 (incorrect)
- Filter Mode : Bilinear (incorrect)

**Action :**
1. Menu : `Tools > Command & Conquer > Reconfigure All Terrain Sprites`
2. Popup : "Successfully reconfigured 20 terrain sprites with PPU=128"

**Résultat :**
- Les 20 sprites ont PPU=128, Filter=Point
- Pas besoin de reconfigurer manuellement chaque sprite

---

## 🐛 Dépannage

### Le sprite ne se configure pas automatiquement

**Symptômes :**
- Pas de log dans la Console
- PPU reste à 100 (valeur par défaut Unity)

**Causes possibles :**
1. Le sprite n'est pas dans `Map/Sprites/Terrain/`
2. Erreur de compilation du script
3. Unity n'a pas détecté le changement

**Solutions :**
1. Vérifier le chemin exact du sprite
2. Console → Vérifier erreurs de compilation
3. Sélectionner sprite → Clic droit → Reimport

### Le sprite est flou dans le jeu

**Symptômes :**
- Le sprite apparaît flou ou pixelisé
- Les bords ne sont pas nets

**Causes possibles :**
1. Filter Mode = Bilinear ou Trilinear
2. Compression activée
3. Camera Projection incorrecte

**Solutions :**
1. Utiliser le menu de reconfiguration
2. Vérifier Camera → Projection = Orthographic
3. Vérifier Tilemap Renderer → Material (Default-Sprite)

### Les changements ne s'appliquent pas

**Symptômes :**
- Menu exécuté mais sprites toujours incorrects
- Pas d'effet visible

**Solutions :**
1. Fermer tous les onglets Inspector
2. Menu → Assets → Reimport All
3. Redémarrer Unity Editor

---

## 📚 Références

- **Documentation principale Tools** : [TOOLS.md](../../../../TOOLS.md)
- **Configuration Tilemap** : [TILEMAP_SETUP.md](../TILEMAP_SETUP.md)
- **Guide Random Brush** : [RANDOM_BRUSH_GUIDE.md](../RANDOM_BRUSH_GUIDE.md)
- **Système de grille** : [ROADMAP.md](../../../../ROADMAP.md) - Commit 7

---

## 🔄 Évolutions futures

Fonctionnalités potentielles :

- [ ] Support de sprites multi-tailles (64×64, 256×256)
- [ ] Configuration personnalisable via ScriptableObject
- [ ] Menu contextuel (clic droit sur sprite)
- [ ] Validation automatique avec warnings
- [ ] Génération automatique de tiles

---

**Créé :** Commit 7 - Système de grille et tilemap
**Documenté :** Commit 8 - Documentation des tools
