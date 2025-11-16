using UnityEngine;
using UnityEditor;

namespace CommandAndConquer.Map.Editor
{
    /// <summary>
    /// Utilitaire pour configurer automatiquement les sprites de terrain
    /// avec les bons paramètres d'import (PPU=128, FilterMode=Point, etc.)
    /// </summary>
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
            textureImporter.filterMode = FilterMode.Point; // Pixel perfect
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

    /// <summary>
    /// Menu Editor pour reconfigurer tous les sprites de terrain existants
    /// </summary>
    public static class TerrainSpriteImporterMenu
    {
        [MenuItem("Tools/Command & Conquer/Reconfigure All Terrain Sprites")]
        public static void ReconfigureAllTerrainSprites()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/_Project/Map/Sprites/Terrain" });

            int count = 0;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
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

                if (modified)
                {
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    count++;
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"[TerrainSpriteImporter] Reconfigured {count} terrain sprites with PPU=128");
            EditorUtility.DisplayDialog("Terrain Sprites Reconfigured",
                $"Successfully reconfigured {count} terrain sprites with PPU=128", "OK");
        }
    }
}
