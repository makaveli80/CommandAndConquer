using UnityEngine;
using UnityEditor;

namespace CommandAndConquer.Buildings.Editor
{
    /// <summary>
    /// Utilitaire pour configurer automatiquement les sprites de bâtiments
    /// avec les bons paramètres d'import (PPU=128, FilterMode=Point, Pivot=Bottom Left, etc.)
    /// ⚠️ IMPORTANT : Les sprites de bâtiments doivent avoir le pivot en Bottom Left (0,0)
    /// </summary>
    public class BuildingSpriteImporter : AssetPostprocessor
    {
        private const int BUILDING_PPU = 128;
        private const string BUILDING_SPRITES_PATH = "Assets/_Project/Buildings";

        private void OnPreprocessTexture()
        {
            // Vérifie si l'asset est dans le dossier Buildings/*/Sprites
            if (!assetPath.Contains(BUILDING_SPRITES_PATH) || !assetPath.Contains("/Sprites/"))
                return;

            TextureImporter textureImporter = (TextureImporter)assetImporter;

            // Configuration pour pixel art 2D
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Single; // Single sprite par défaut pour bâtiments
            textureImporter.spritePixelsPerUnit = BUILDING_PPU;
            textureImporter.filterMode = FilterMode.Point; // Pixel perfect
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            textureImporter.mipmapEnabled = false;

            // Configuration pour 2D
            TextureImporterSettings settings = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteGenerateFallbackPhysicsShape = false; // Pas de physics shape pour bâtiments

            // ⚠️ CRUCIAL : Pivot en Bottom Left (0,0) pour alignement parfait sur la grille
            settings.spriteAlignment = (int)SpriteAlignment.BottomLeft;

            textureImporter.SetTextureSettings(settings);

            Debug.Log($"[BuildingSpriteImporter] Configured {assetPath} with PPU={BUILDING_PPU}, Pivot=Bottom Left");
        }
    }

    /// <summary>
    /// Menu Editor pour reconfigurer tous les sprites de bâtiments existants
    /// </summary>
    public static class BuildingSpriteImporterMenu
    {
        [MenuItem("Tools/Command & Conquer/Reconfigure All Building Sprites")]
        public static void ReconfigureAllBuildingSprites()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/_Project/Buildings" });

            int count = 0;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                // Vérifier que c'est bien dans un dossier Sprites
                if (!path.Contains("/Sprites/"))
                    continue;

                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

                if (importer == null)
                    continue;

                bool modified = false;

                if (importer.spritePixelsPerUnit != 128)
                {
                    importer.spritePixelsPerUnit = 128;
                    modified = true;
                }

                if (importer.filterMode != FilterMode.Point)
                {
                    importer.filterMode = FilterMode.Point;
                    modified = true;
                }

                if (importer.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    modified = true;
                }

                if (importer.mipmapEnabled)
                {
                    importer.mipmapEnabled = false;
                    modified = true;
                }

                // ⚠️ Configurer le pivot en Bottom Left
                TextureImporterSettings settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);

                if (settings.spriteAlignment != (int)SpriteAlignment.BottomLeft)
                {
                    settings.spriteAlignment = (int)SpriteAlignment.BottomLeft;
                    importer.SetTextureSettings(settings);
                    modified = true;
                }

                if (modified)
                {
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    count++;
                    Debug.Log($"[BuildingSpriteImporter] Reconfigured {path} with Pivot=Bottom Left");
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"[BuildingSpriteImporter] Reconfigured {count} building sprites with PPU=128, Pivot=Bottom Left");
            EditorUtility.DisplayDialog("Building Sprites Reconfigured",
                $"Successfully reconfigured {count} building sprites with PPU=128, FilterMode=Point, Pivot=Bottom Left", "OK");
        }

        [MenuItem("Tools/Command & Conquer/Reconfigure Airstrip Sprites")]
        public static void ReconfigureAirstripSprites()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/_Project/Buildings/Airstrip/Sprites" });

            int count = 0;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

                if (importer == null)
                    continue;

                // Configuration spécifique Airstrip
                importer.spritePixelsPerUnit = 128;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;
                importer.spriteImportMode = SpriteImportMode.Single;

                // ⚠️ CRUCIAL : Pivot Bottom Left
                TextureImporterSettings settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteAlignment = (int)SpriteAlignment.BottomLeft;
                importer.SetTextureSettings(settings);

                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                count++;
                Debug.Log($"[BuildingSpriteImporter] Reconfigured {path} with Pivot=Bottom Left");
            }

            AssetDatabase.Refresh();
            Debug.Log($"[BuildingSpriteImporter] Reconfigured {count} Airstrip sprites");
            EditorUtility.DisplayDialog("Airstrip Sprites Reconfigured",
                $"Successfully reconfigured {count} Airstrip sprites with Pivot=Bottom Left!", "OK");
        }
    }
}
