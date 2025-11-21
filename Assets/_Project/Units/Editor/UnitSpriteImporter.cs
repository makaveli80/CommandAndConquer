using UnityEngine;
using UnityEditor;

namespace CommandAndConquer.Units.Editor
{
    /// <summary>
    /// Utilitaire pour configurer automatiquement les sprites d'unités
    /// avec les bons paramètres d'import (PPU=128, FilterMode=Point, etc.)
    /// </summary>
    public class UnitSpriteImporter : AssetPostprocessor
    {
        private const int UNIT_PPU = 128;
        private const string UNIT_SPRITES_PATH = "Assets/_Project/Units";

        private void OnPreprocessTexture()
        {
            // Vérifie si l'asset est dans le dossier Units/*/Sprites
            if (!assetPath.Contains(UNIT_SPRITES_PATH) || !assetPath.Contains("/Sprites/"))
                return;

            TextureImporter textureImporter = (TextureImporter)assetImporter;

            // Configuration pour pixel art 2D
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple; // Multiple car sprites souvent découpés
            textureImporter.spritePixelsPerUnit = UNIT_PPU;
            textureImporter.filterMode = FilterMode.Point; // Pixel perfect
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            textureImporter.mipmapEnabled = false;

            // Configuration pour 2D
            TextureImporterSettings settings = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteGenerateFallbackPhysicsShape = true; // Physics shape pour unités
            textureImporter.SetTextureSettings(settings);

            Debug.Log($"[UnitSpriteImporter] Configured {assetPath} with PPU={UNIT_PPU}");
        }
    }

    /// <summary>
    /// Menu Editor pour reconfigurer tous les sprites d'unités existants
    /// </summary>
    public static class UnitSpriteImporterMenu
    {
        [MenuItem("Tools/Command & Conquer/Reconfigure All Unit Sprites")]
        public static void ReconfigureAllUnitSprites()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/_Project/Units" });

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

                if (modified)
                {
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    count++;
                    Debug.Log($"[UnitSpriteImporter] Reconfigured {path}");
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"[UnitSpriteImporter] Reconfigured {count} unit sprites with PPU=128");
            EditorUtility.DisplayDialog("Unit Sprites Reconfigured",
                $"Successfully reconfigured {count} unit sprites with PPU=128, FilterMode=Point, Uncompressed", "OK");
        }

        [MenuItem("Tools/Command & Conquer/Reconfigure Buggy Sprites")]
        public static void ReconfigureBuggySprites()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/_Project/Units/Buggy/Sprites" });

            int count = 0;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

                if (importer == null)
                    continue;

                // Configuration spécifique
                importer.spritePixelsPerUnit = 128;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;
                importer.spriteImportMode = SpriteImportMode.Multiple;

                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                count++;
                Debug.Log($"[UnitSpriteImporter] Reconfigured {path}");
            }

            AssetDatabase.Refresh();
            Debug.Log($"[UnitSpriteImporter] Reconfigured {count} Buggy sprites");
            EditorUtility.DisplayDialog("Buggy Sprites Reconfigured",
                $"Successfully reconfigured {count} Buggy sprites!", "OK");
        }
    }
}
