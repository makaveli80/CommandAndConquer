# Tools et Outils Editor

Documentation des outils de développement Unity pour le projet Command and Conquer.

---

## 📋 Vue d'ensemble

Le projet inclut plusieurs outils Editor personnalisés pour automatiser et faciliter le développement. Ces outils sont accessibles via le menu Unity `Tools > Command & Conquer/`.

---

## 🛠️ Outils disponibles

### 1. Sprite Importers (AssetPostprocessors)

Configurent automatiquement les sprites lors de leur import dans Unity.

#### TerrainSpriteImporter
- **Emplacement**: `Assets/_Project/Map/Editor/TerrainSpriteImporter.cs`
- **Documentation**: [Map/Editor/README.md](Assets/_Project/Map/Editor/README.md)
- **Fonction**: Configure automatiquement les sprites de terrain
- **Chemin surveillé**: `Assets/_Project/Map/Sprites/Terrain/`

**Configuration appliquée:**
- PPU (Pixels Per Unit): 128
- Filter Mode: Point (pixel perfect)
- Compression: None (qualité maximale)
- Sprite Mode: Single
- Mipmaps: Désactivés

**Menus disponibles:**
- `Tools > Command & Conquer > Reconfigure All Terrain Sprites`

#### UnitSpriteImporter
- **Emplacement**: `Assets/_Project/Units/Editor/UnitSpriteImporter.cs`
- **Documentation**: [Units/Editor/README.md](Assets/_Project/Units/Editor/README.md)
- **Fonction**: Configure automatiquement les sprites d'unités
- **Chemin surveillé**: `Assets/_Project/Units/*/Sprites/`

**Configuration appliquée:**
- PPU (Pixels Per Unit): 128
- Filter Mode: Point (pixel perfect)
- Compression: None (qualité maximale)
- Sprite Mode: Multiple (pour découpage)
- Mipmaps: Désactivés

**Menus disponibles:**
- `Tools > Command & Conquer > Reconfigure All Unit Sprites`
- `Tools > Command & Conquer > Reconfigure Buggy Sprites`

---

## 🎯 Utilisation

### Import automatique (Recommandé)

Les sprites sont automatiquement configurés lors de leur ajout dans les dossiers surveillés :

1. Ajoutez vos sprites dans le bon dossier :
   - Terrain : `Assets/_Project/Map/Sprites/Terrain/`
   - Unités : `Assets/_Project/Units/[NomUnité]/Sprites/`

2. Unity détecte automatiquement les nouveaux fichiers

3. Les AssetPostprocessors appliquent la configuration

4. Vos sprites sont prêts à l'emploi avec les bons paramètres

### Reconfiguration manuelle

Si vous avez des sprites déjà importés avec de mauvais paramètres :

1. Ouvrez Unity Editor

2. Menu : `Tools > Command & Conquer > [Choisir l'outil approprié]`

3. Attendez la confirmation dans la console Unity

4. Les sprites sont reconfigurés avec les bons paramètres

---

## 📐 Paramètres techniques

### Pourquoi PPU = 128 ?

Le système de grille utilise des cellules de **1.0 unité Unity**.

Avec des sprites de 128×128 pixels et PPU=128 :
- 1 sprite complet = 1.0 unité Unity = 1 cellule de grille
- Alignement parfait entre sprites, tilemap et grille logique
- Calculs de position simplifiés

### Pourquoi Filter Mode = Point ?

- Style pixel art sans flou
- Rendu net et précis
- Conforme au style des RTS classiques (Command & Conquer, Red Alert)

### Pourquoi Compression = None ?

- Qualité visuelle maximale
- Pas d'artefacts de compression
- Taille de fichier acceptable pour un projet éducatif

---

## 🔧 Développement des outils

### Créer un nouveau AssetPostprocessor

Si vous voulez créer un outil similaire pour un autre type d'asset :

```csharp
using UnityEngine;
using UnityEditor;

public class MyCustomImporter : AssetPostprocessor
{
    private const string MY_PATH = "Assets/_Project/MyFolder/";

    private void OnPreprocessTexture()
    {
        if (!assetPath.Contains(MY_PATH))
            return;

        TextureImporter textureImporter = (TextureImporter)assetImporter;

        // Votre configuration ici
        textureImporter.spritePixelsPerUnit = 128;
        textureImporter.filterMode = FilterMode.Point;
        // etc.
    }
}
```

### Créer un menu Unity

```csharp
[MenuItem("Tools/Command & Conquer/My Custom Tool")]
public static void MyCustomTool()
{
    // Votre logique ici
    Debug.Log("Tool executed!");
}
```

---

## 📚 Références

- **TerrainSpriteImporter**: Voir [Map/Editor/README.md](Assets/_Project/Map/Editor/README.md)
- **UnitSpriteImporter**: Voir [Units/Editor/README.md](Assets/_Project/Units/Editor/README.md)
- **Système de grille**: Voir [ROADMAP.md](ROADMAP.md) - Commit 7
- **Configuration Tilemap**: Voir [Map/TILEMAP_SETUP.md](Assets/_Project/Map/TILEMAP_SETUP.md)

---

## 🐛 Dépannage

### Les sprites ne se configurent pas automatiquement

**Vérifiez :**
1. Le sprite est dans le bon dossier surveillé
2. Le fichier a bien l'extension `.png` ou `.jpg`
3. Unity a terminé l'import (barre de progression en bas)
4. Pas d'erreurs dans la Console Unity

**Solution :**
- Utilisez le menu `Reconfigure...` correspondant
- Ou : Sélectionnez le sprite → Clic droit → Reimport

### Le sprite est configuré mais s'affiche mal

**Vérifiez :**
1. PPU = 128 dans l'Inspector du sprite
2. Filter Mode = Point
3. Le Tilemap/SpriteRenderer utilise le bon Material

**Solution :**
- Reconfigurer avec le menu approprié
- Vérifier l'alignment du sprite (pivot à 0.5, 0.5)

### Les menus n'apparaissent pas dans Unity

**Vérifiez :**
1. Les scripts sont dans un dossier `Editor/`
2. L'Assembly Definition `.asmdef` est présent
3. Pas d'erreurs de compilation dans la Console

**Solution :**
- Assets → Reimport All
- Redémarrer Unity Editor

---

**Dernière mise à jour**: Commit 8 - Buggy sprites et UnitSpriteImporter
