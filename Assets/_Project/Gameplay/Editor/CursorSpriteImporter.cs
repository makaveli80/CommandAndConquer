using UnityEngine;
using UnityEditor;

namespace CommandAndConquer.Gameplay.Editor
{
    /// <summary>
    /// Utilitaire pour configurer automatiquement les sprites de curseur
    /// avec les bons paramètres d'import (Read/Write, Point filter, etc.)
    /// </summary>
    public class CursorSpriteImporter : AssetPostprocessor
    {
        private const string CURSOR_SPRITES_PATH = "Assets/_Project/Gameplay/Sprites/Cursors";

        private void OnPreprocessTexture()
        {
            // Vérifie si l'asset est dans le dossier Cursors
            if (!assetPath.Contains(CURSOR_SPRITES_PATH))
                return;

            TextureImporter textureImporter = (TextureImporter)assetImporter;

            // Configuration pour curseur
            textureImporter.textureType = TextureImporterType.Default;
            textureImporter.isReadable = true; // IMPORTANT: Cursor.SetCursor nécessite Read/Write
            textureImporter.filterMode = FilterMode.Point; // Pixel perfect
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            textureImporter.mipmapEnabled = false;
            textureImporter.alphaIsTransparency = true; // Pour la transparence
            textureImporter.maxTextureSize = 64; // Taille max pour curseurs

            // Configuration avancée
            TextureImporterSettings settings = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(settings);
            settings.readable = true; // Double vérification
            settings.npotScale = TextureImporterNPOTScale.None;
            textureImporter.SetTextureSettings(settings);

            Debug.Log($"[CursorSpriteImporter] Configured cursor {assetPath} (Read/Write enabled)");
        }
    }

    /// <summary>
    /// Menu Editor pour reconfigurer tous les sprites de curseur existants
    /// </summary>
    public static class CursorSpriteImporterMenu
    {
        [MenuItem("Tools/Command & Conquer/Reconfigure Cursor Sprites")]
        public static void ReconfigureCursorSprites()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/_Project/Gameplay/Sprites/Cursors" });

            int count = 0;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

                if (importer == null)
                    continue;

                bool modified = false;

                if (!importer.isReadable)
                {
                    importer.isReadable = true;
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

                if (!importer.alphaIsTransparency)
                {
                    importer.alphaIsTransparency = true;
                    modified = true;
                }

                if (modified)
                {
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    count++;
                    Debug.Log($"[CursorSpriteImporter] Reconfigured {path}");
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"[CursorSpriteImporter] Reconfigured {count} cursor sprites");
            EditorUtility.DisplayDialog("Cursor Sprites Reconfigured",
                $"Successfully reconfigured {count} cursor sprites with Read/Write enabled, FilterMode=Point, Uncompressed", "OK");
        }
    }
}