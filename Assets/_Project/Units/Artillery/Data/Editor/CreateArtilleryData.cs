using UnityEditor;
using UnityEngine;
using CommandAndConquer.Units.Artillery;

namespace CommandAndConquer.Units.Editor
{
    /// <summary>
    /// Outil Editor pour créer facilement un ArtilleryData ScriptableObject.
    /// </summary>
    public static class CreateArtilleryData
    {
        [MenuItem("Tools/Command & Conquer/Create Artillery Data")]
        public static void CreateAsset()
        {
            // Créer l'instance
            ArtilleryData asset = ScriptableObject.CreateInstance<ArtilleryData>();

            // Configuration par défaut
            asset.unitName = "Artillery";
            asset.description = "Unité d'artillerie lourde avec longue portée. Très lente mais puissante.";
            asset.moveSpeed = 1.5f;  // Très lente (Buggy = 4.0f)
            asset.canMove = true;

            // Sauvegarder dans le dossier Data
            string path = "Assets/_Project/Units/Artillery/Data/ArtilleryData.asset";
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();

            // Sélectionner l'asset créé
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            Debug.Log($"[CreateArtilleryData] ArtilleryData created at {path}");
        }
    }
}