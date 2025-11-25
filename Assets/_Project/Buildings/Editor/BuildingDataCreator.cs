using UnityEngine;
using UnityEditor;
using CommandAndConquer.Buildings;

namespace CommandAndConquer.Buildings.Editor
{
    /// <summary>
    /// Script Editor pour créer automatiquement les assets BuildingData.
    /// Temporaire pour Phase 1 - peut être supprimé après.
    /// </summary>
    public static class BuildingDataCreator
    {
        [MenuItem("Command & Conquer/Buildings/Create Airstrip Data")]
        public static void CreateAirstripData()
        {
            // Créer l'asset BuildingData
            BuildingData data = ScriptableObject.CreateInstance<BuildingData>();

            // Configuration de l'Airstrip (4×2)
            data.buildingName = "Airstrip";
            data.description = "Aéroport pour faire atterrir et décoller les unités. Permet de recevoir des renforts.";
            data.width = 4;
            data.height = 2;
            data.spawnOffset = new Vector2Int(2, 0); // Sortie au centre en bas

            // Sauvegarder l'asset
            string path = "Assets/_Project/Buildings/Airstrip/Data/AirstripData.asset";

            // Créer le répertoire s'il n'existe pas
            string directory = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            // Créer l'asset
            AssetDatabase.CreateAsset(data, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Sélectionner l'asset créé
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = data;

            Debug.Log($"[BuildingDataCreator] Created AirstripData at {path} (4×2)");
        }
    }
}
