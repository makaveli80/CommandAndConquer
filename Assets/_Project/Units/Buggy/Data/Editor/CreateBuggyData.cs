using UnityEngine;
using UnityEditor;
using CommandAndConquer.Units.Buggy;

namespace CommandAndConquer.Units.Buggy.Editor
{
    /// <summary>
    /// Utilitaire Editor pour créer l'asset BuggyData avec les valeurs par défaut.
    /// </summary>
    public static class CreateBuggyDataAsset
    {
        [MenuItem("Tools/Command & Conquer/Create Buggy Data Asset")]
        public static void CreateAsset()
        {
            // Créer l'instance du ScriptableObject
            BuggyData buggyData = ScriptableObject.CreateInstance<BuggyData>();

            // Configurer les valeurs par défaut
            buggyData.unitName = "Buggy";
            buggyData.description = "Véhicule léger et rapide de reconnaissance. Idéal pour l'exploration et les attaques rapides.";
            buggyData.moveSpeed = 4.0f;  // Rapide
            buggyData.canMove = true;

            // Sauvegarder l'asset
            string path = "Assets/_Project/Units/Buggy/Data/BuggyData.asset";
            AssetDatabase.CreateAsset(buggyData, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Sélectionner l'asset créé
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = buggyData;

            Debug.Log($"[CreateBuggyData] Asset créé avec succès : {path}");
            EditorUtility.DisplayDialog("Buggy Data créé",
                $"L'asset BuggyData a été créé avec succès !\n\nChemin : {path}\n\nVitesse : {buggyData.moveSpeed}",
                "OK");
        }
    }
}
